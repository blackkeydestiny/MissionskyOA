using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class OrderExtentions
    {
        public static Order ToEntity(this OrderModel model)
        {
            var entity = new Order()
            {
                Id = model.Id,
                OrderType = (int) model.OrderType,
                Status = (int) model.Status,
                UserId = model.UserId,
                NextAudit = model.NextAudit,
                ApplyUserId = model.ApplyUserId,
                RefOrderId = model.RefOrderId,
                CreatedTime = model.CreatedTime,
                NextStep = model.NextStep,
                WorkflowId = model.WorkflowId.HasValue ? model.WorkflowId.Value : 0,
                OrderNo = model.OrderNo

            };
            return entity;
        }

        public static OrderModel ToModel(this Order entity)
        {
            var model = new OrderModel()
            {
                Id = entity.Id,
                OrderType = (OrderType)entity.OrderType,
                Status = entity.Status.HasValue ? (OrderStatus)entity.Status : OrderStatus.Apply,
                ApplyUserId = entity.ApplyUserId.Value,
                NextAudit=entity.NextAudit,
                RefOrderId = entity.RefOrderId,
                CreatedTime = entity.CreatedTime.Value,
                NextStep = entity.NextStep,
                WorkflowId = entity.WorkflowId,
                OrderNo = entity.OrderNo
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                UserService userServer = new UserService();

                //申请人
                var applyUser = userServer.GetUserDetail(entity.ApplyUserId.Value);
                model.ApplyUserName = applyUser == null ? string.Empty : applyUser.EnglishName;

                //批量申请用户
                model.OrderUsers = (new UserService()).GetOrderUsers(dbContext, entity.OrderNo);

                //Get refrence order
                if (entity.RefOrderId.HasValue && entity.RefOrderId.Value > 0)
                {
                    var refOrderEntity = dbContext.Orders.FirstOrDefault(it => it.Id == entity.RefOrderId);

                    if (refOrderEntity != null)
                    {
                        model.RefOrder = refOrderEntity.ToModel();
                        model.RefOrder.Attachments = (new AttachmentService()).GetAttathcmentIds(dbContext,
                            refOrderEntity.OrderNo, Constant.ATTACHMENT_TYPE_ORDER_ATTACHMENT); //附件
                        model.Attachments = (new AttachmentService()).GetAttathcmentIds(dbContext,
                            refOrderEntity.OrderNo, Constant.ATTACHMENT_TYPE_ORDER_ATTACHMENT); //附件
                    }
                }

                var auditMessageEntity = dbContext.WorkflowProcesses.FirstOrDefault(it => it.OrderNo == entity.OrderNo && it.StepId == entity.NextStep);
                if (auditMessageEntity!=null)
                {
                    model.AuditAdvice = auditMessageEntity.OperationDesc;
                }
                var nextAuditEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.NextAudit);
                if (nextAuditEntity != null)
                {
                    model.NextAuditName = nextAuditEntity.EnglishName;
                }

                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.UserId);
                if (userEntity != null)
                {
                    model.UserName = userEntity.EnglishName;
                    //get department name
                    var refDeptEntity = dbContext.Departments.FirstOrDefault(it => it.Id == userEntity.DeptId);
                    if (refDeptEntity != null)
                    {
                        model.DeptName = refDeptEntity.Name;
                    }
                    //get project name
                    var refProjectEntity = dbContext.Projects.FirstOrDefault(it => it.Id == userEntity.ProjectId);
                    if (refProjectEntity != null)
                    {
                        model.ProjectName = refProjectEntity.Name;
                    }

                }
                else
                {
                    throw new Exception("This User not exsit");
                }
            }
            //var applyUser = userServer.GetUserDetail(entity.ApplyUserId.Value);
            using (var dbContext = new MissionskyOAEntities())
            {
                UserService userServer = new UserService();
                //申请人
                if (entity.OrderDets != null && entity.OrderDets.Count > 0)
                {
                    foreach (var orderDetail in entity.OrderDets)
                    {
                        model.OrderDets = new List<OrderDetModel>();
                        model.OrderDets.Add(OrderDetailExtentions.ToModel(orderDetail));
                    }
                }
            }

            return model;
        }
        public static Order Copyto(this Order entity)
        {
            if (entity == null)
            {
                throw new InvalidOperationException("申请单无效。");
            }

            var detEntity = entity.OrderDets.FirstOrDefault();

            if (detEntity == null)
            {
                throw new InvalidOperationException("申请单详细无效。");
            }

            var detail = new OrderDet()
            {
                IOHours = detEntity.IOHours,
                StartDate = detEntity.StartDate,
                StartTime = detEntity.StartTime,
                EndDate = detEntity.EndDate,
                EndTime = detEntity.EndTime,
                Audit = detEntity.Audit,
                Description = detEntity.Description
            };

            var order = new Order()
            {
                OrderNo = entity.OrderNo,
                OrderType = entity.OrderType,
                ApplyUserId = entity.ApplyUserId,
                WorkflowId = entity.WorkflowId,
                NextStep = entity.NextStep,
                NextAudit = entity.NextAudit,
                CreatedTime = entity.CreatedTime,
                Status = entity.Status,
                RefOrderId = entity.RefOrderId,
                UserId = entity.UserId
            };

            order.OrderDets.Add(detail);

            return order;
        }

        #region 申请单验证
        /// <summary>
        /// 是否为有效的申请类型
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsValidOrderType(this OrderModel order)
        {
            Array values = Enum.GetValues(typeof(OrderType));
            if (order.OrderType == OrderType.None)
            {
                return false;
            }

            string temp = "";
            foreach (int v in values)
            {
                temp += "," + v;
            }

            temp += ",";

            return temp.Contains(string.Format(",{0},", order.OrderType));
        }
        
        /// <summary>
        /// 是否是请假类型
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsAskLeave(this OrderModel order)
        {
            if (order.OrderType == OrderType.AnnualLeave || order.OrderType == OrderType.DaysOff ||
                order.OrderType == OrderType.SickLeave ||
                order.OrderType == OrderType.MarriageLeave || order.OrderType == OrderType.MaternityLeave ||
                order.OrderType == OrderType.FuneralLeave || order.OrderType == OrderType.AskLeave ||
                order.OrderType == OrderType.BreastfeedingLeave || order.OrderType == OrderType.PaternityLeave ||
                order.OrderType == OrderType.Other)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 是否是加班类型
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsOvertime(this OrderModel order)
        {
            if (order.OrderType == OrderType.Overtime)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 是否是为[领导审批]状态
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsApproveStatus(this OrderModel order)
        {
            if (order.Status != OrderStatus.Apply && order.Status != OrderStatus.Approving)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否是为[行政或财务审批]状态
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsReviewStatus(this OrderModel order)
        {
            if ((order.Status == OrderStatus.Approved || order.Status == OrderStatus.Revoked) && order.NextStep.HasValue)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否为申请撤销单
        /// </summary>
        /// <param name="order">申请单</param>
        /// <returns>true or false</returns>
        public static bool IsRevokedOrder(this OrderModel order)
        {
            if (order.RefOrderId.HasValue && order.RefOrderId.Value > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 用效的撤销单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool CanRevokeOrder(this OrderModel order)
        {
            return (order.Status == OrderStatus.Apply || order.Status == OrderStatus.Approving ||
                    order.Status == OrderStatus.Approved);
        }
        #endregion

        /// <summary>
        /// TimeSpan ToString去掉尾巴上的8位小数
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string FormatTimeSpan(TimeSpan? ts)
        {
            return System.Text.RegularExpressions.Regex.Replace(ts.ToString(), @"\.\d+$", string.Empty);
        }
    }
}
