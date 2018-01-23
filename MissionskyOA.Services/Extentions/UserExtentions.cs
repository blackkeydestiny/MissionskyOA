using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class UserExtentions
    {
        public static User ToEntity(this UserModel model)
        {
            var entity = new User()
            {
                Id = model.Id,
                ChineseName = model.ChineseName,
                EnglishName = model.EnglishName,
                Email = model.Email,
                QQID = model.QQID,
                Gender = (int)model.Gender,
                DirectlySupervisorId = model.DirectlySupervisorId,
                Phone = model.Phone,
                ProjectId = model.ProjectId,
                Status = (int)model.Status,
                No = model.No,
                Available = model.Available,
                TodayStatus = (int)model.TodayStatus,
                DeptId = model.DeptId,
                Position = model.Position,
                JoinDate = model.JoinDate,
                ServerYear = model.ServerYear,
                ServerYearType = (int)model.ServerYearType
            };
            return entity;
        }

        public static UserModel ToModel(this User entity)
        {
            // 根据ServerYear处理ServerYearType
            int year = entity.ServerYear.HasValue ? entity.ServerYear.Value : 0;
            //int year = (int)entity.ServerYear;
            if (year < 0)
            {
                year = Math.Abs(year);
            }
            if (year >= 0 && year < 10)
            {
                entity.ServerYearType = (int)UserServiceYearType.TenYears;
            }
            else if (year >= 10 && year < 20)
            {
                entity.ServerYearType = (int)UserServiceYearType.TwentyYears;
            }
            else if (year >= 20)
            {
                entity.ServerYearType = (int)UserServiceYearType.ThirtyYears;
            }

            var model = new UserModel()
            {
                Id = entity.Id,
                ChineseName = entity.ChineseName,
                EnglishName = entity.EnglishName,
                Email = entity.Email,
                QQID = entity.QQID,
                Gender = entity.Gender.HasValue ? (Gender)entity.Gender.Value : Gender.Privacy,
                DirectlySupervisorId = entity.DirectlySupervisorId,
                Phone = entity.Phone,
                ProjectId = entity.ProjectId,
                Status = entity.Status.HasValue ? (AccountStatus)entity.Status.Value : AccountStatus.Normal,
                CreatedTime = entity.CreatedTime,
                No = entity.No,
                Available = entity.Available,
                TodayStatus = (UserTodayStatus)entity.TodayStatus,
                DeptId = entity.DeptId,
                Position = entity.Position,
                JoinDate = entity.JoinDate.HasValue ? entity.JoinDate.Value : new DateTime(),
                ServerYear = year,
                ServerYearType = (UserServiceYearType)entity.ServerYearType
            };

            if (entity.Orders != null && entity.Orders.Count > 0)
            {
                foreach (var order in entity.Orders)
                {
                    var orderModel = new OrderModel()
                    {
                        Id = order.Id,
                        OrderType = (OrderType) order.OrderType,
                        CreatedTime = order.CreatedTime.Value,
                        Status = order.Status.HasValue ? (OrderStatus) order.Status.Value : OrderStatus.Approved,
                        UserId = order.UserId,
                        RefOrderId = order.RefOrderId
                    };

                    if (order.OrderDets != null && order.OrderDets.Count > 0)
                    {
                        orderModel.OrderDets = new List<OrderDetModel>();
                        order.OrderDets.ToList().ForEach(det =>
                            orderModel.OrderDets.Add(new OrderDetModel()
                            {
                                Id = det.Id,
                                StartDate = det.StartDate,
                                StartTime = det.StartTime,
                                EndDate = det.EndDate,
                                EndTime = det.EndTime,
                                IOHours = det.IOHours,
                                OrderId = det.OrderId
                            })
                            );
                    }
                    //for deleting UserRoles
                    //model.Orders.Add(orderModel);
                }
            }

            #region 权限
            entity.UserRoles = entity.UserRoles ?? new Collection<UserRole>();

            // 是否是资产管理员
            model.IsAssetManager = entity.UserRoles.Any(it => it.Role != null && it.Role.Name == Constant.ROLE_ASSET_ADMIN);

            //for deleting UserRoles
            //model.UserRoles = new List<RoleModel>();
            //entity.UserRoles.ToList().ForEach(it =>
            //    model.UserRoles.Add(new RoleModel()
            //    {
            //        Id = it.Id,
            //        Name = it.Role.Name,
            //        ApprovedDays = it.Role.ApprovedDays
            //    })
            //    );

            // 是否为行政专员
            model.IsAdminStaff = entity.UserRoles.Any(it => it.Role != null && it.Role.Name == Constant.ROLE_ADMIN_STAFF);
            if (!model.IsAdminStaff)
            {
                using (var dbContext = new MissionskyOAEntities())
                {
                    //是否是行政管理员
                    var dept = dbContext.Departments.FirstOrDefault(it => it.Id == entity.DeptId);

                    model.IsAdminStaff = !(dept == null || string.IsNullOrEmpty(dept.Name) ||
                                           !dept.Name.Equals(Constant.DEPT_ADMIN,
                                               StringComparison.InvariantCultureIgnoreCase));
                }
            }
            #endregion

            return model;
        }

        /// <summary>
        /// 电话号码不可见
        /// </summary>
        /// <param name="current">当前登录用户</param>
        /// <param name="user">用户</param>
        public static void HidePhone(UserModel current, UserModel user)
        {
            if (current == null)
            {
                throw new NullReferenceException("Invalid current user model when hide phone.");
            }

            if (user == null)
            {
                throw new NullReferenceException("Invalid user model when hide phone.");
            }

            var shown = false;

            shown = user.Id == current.Id; //用户自己能看到

            shown =
                shown ||
                (user.DirectlySupervisorId.HasValue && current.Id == user.DirectlySupervisorId.Value
                && (current.RoleName == "项目经理" || current.RoleName.ToUpper().Contains("PM"))); //是直接领导且为PM能看到

            shown = shown || current.RoleName == "总经理" || current.RoleName == "副总经理" || current.RoleName == "总监"; //领导能看到

            shown = shown || user.IsAdminStaff; //管理员能看到

            shown = shown || (current.DirectlySupervisorId != null && user.Id == current.DirectlySupervisorId) || user.ProjectId == current.ProjectId; //能查看上级 || 查看同一项目组

            if (!shown)
            {
                user.Phone = (string.IsNullOrEmpty(user.Phone) || user.Phone.Length <= 3
                    ? string.Empty
                    : user.Phone.Substring(0, 3) + "********");
            }
        }


        /// <summary>
        /// 填充user关联信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="user">用户model</param>
        public static void FillRelatedDetail(MissionskyOAEntities dbContext, UserModel user)
        {
            if (dbContext == null)
            {
                throw new NullReferenceException("Invalid db context when fill user detail.");
            }

            if (user == null)
            {
                throw new NullReferenceException("Invalid user model when fill user detail.");
            }

            //汇总信息
            user.VacationSummary = (new AttendanceSummaryService()).GetAttendanceSummariesByUserId(dbContext, user.Id);

            //部门
            var dept = dbContext.Departments.FirstOrDefault(it => it.Id == user.DeptId);
            user.DeptName = dept == null ? string.Empty : dept.Name;

            //角色
            var userrole = dbContext.UserRoles.FirstOrDefault(it => it.UserId == user.Id);
            user.RoleName = userrole == null || userrole.Role == null ? string.Empty : userrole.Role.Name;
            if (userrole != null)
            {
                user.Role = userrole.RoleId;
            }

            //项目
            var project = dbContext.Projects.FirstOrDefault(it => it.Id == user.ProjectId);
            user.ProjectName = project == null ? string.Empty : project.Name;
        }

        /// <summary>
        /// 生成随机6位密码
        /// </summary>
        /// <returns>随机密码</returns>
        public static String RandomPassword()
        {
            Random ran = new Random();
            var password = string.Empty;

            for (int i = 0; i < 6; i++)
            {
                while (true)
                {
                    var temp = ran.Next(0, 10).ToString();

                    if (!password.Contains(temp))
                    {
                        password += temp;
                        break;
                    }
                }
            }

            return password;
        }
    }
}
