using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using System.Globalization;


namespace MissionskyOA.Services
{
    public class OvertimeService : IOvertimeService
    {
        /// <summary>
        /// 显示所有加班单
        /// </summary>
        /// <returns>所有加班单分页信息</returns>
        public IPagedList<OrderModel> MyOvertimeList(UserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<OrderModel> result = new List<OrderModel>();

                var orders =
                    dbContext.Orders.Where(
                        it =>
                            ((it.UserId == model.Id || it.ApplyUserId == model.Id) && //查询用户申请的或者用户自己的
                             it.OrderType == (int)OrderType.Overtime)
                        ).OrderByDescending(item => item.CreatedTime);

                orders.ToList().ForEach(item =>
                {
                    var order = item.ToModel();
                    if (result.Any(it => it.OrderNo == order.OrderNo) == false) //过滤批量申请，重复的申请单
                    {
                        OrderService.DoFill(dbContext, order);
                        result.Add(order);
                    }
                });

                return new PagedList<OrderModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 显示加班单具体信息根据具体加班单信息id
        /// </summary>
        /// <returns>具体加班信息</returns>
        public OrderDetModel GetOvertimeDetailsByID(int orderDetailID)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var askLeaveDetails = dbContext.OrderDets.FirstOrDefault(item => item.Id == orderDetailID);
                if (askLeaveDetails == null)
                {
                    throw new Exception("InvalidId");
                }
                return askLeaveDetails.ToModel();
            }
        }


        /// <summary>
        /// 根据加班单id显示所有加班单
        /// </summary>
        /// <returns>具体加班信息</returns>
        public ListResult<OrderDetModel> GetOvertimeDetailsByOrderID(int orderID)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var askLeaveDetails = dbContext.OrderDets.Where(item => item.OrderId == orderID);
                if (askLeaveDetails == null)
                {
                    throw new Exception("InvalidId");
                }
                ListResult<OrderDetModel> result = new ListResult<OrderDetModel>();
                result.Data = new List<OrderDetModel>();
                askLeaveDetails.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });
                result.Total = askLeaveDetails.Count();
                return result;
            }
        }
        /// <summary>
        /// 根据userid,显示所有加班历史记录
        /// </summary>
        /// <returns>加班单分页信息</returns>
        public IPagedList<OrderModel> GetOvertimeHistoryByUserID(UserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<OrderModel> result = new List<OrderModel>();

                var askLeaveList = dbContext.Orders.Where(it => ((it.UserId == model.Id && it.OrderType == (int)OrderType.Overtime && it.Status == (int)OrderStatus.Approved))).OrderByDescending(item => item.CreatedTime);

                askLeaveList.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return new PagedList<OrderModel>(result, pageIndex, pageSize);
            }
        }
        /// <summary>
        /// 显示所有加班记录
        /// </summary>
        /// <returns>加班单信息</returns>
        public ListResult<OrderModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list =  dbContext.Orders.Where(it => ((it.OrderType == (int)OrderType.Overtime))).AsEnumerable();
                if (filter != null && filter.Member == "UserName")
                {
                    var userList = dbContext.Users.Where(it => it.EnglishName == filter.Value);
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            userList = dbContext.Users.Where(it => it.EnglishName == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            userList = dbContext.Users.Where(it => it.EnglishName != filter.Value);
                            break;
                        case "StartsWith":
                            userList = dbContext.Users.Where(it => it.EnglishName != null && it.EnglishName.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            userList = dbContext.Users.Where(it => it.EnglishName != null && it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            userList = dbContext.Users.Where(it => it.EnglishName != null && !it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            userList = dbContext.Users.Where(it => it.EnglishName != null && it.EnglishName.ToLower().EndsWith(filter.Value.ToLower()));
                            break;
                    }
                    List<int> userIds = new List<int>();
                    userList.ToList().ForEach(item =>
                    {
                        userIds.Add(item.Id);
                    });
                    list = list.Where(it => userIds.Contains(it.UserId));
                }
                if (filter != null && filter.Member == "CreatedTime")
                {
                    System.Globalization.DateTimeFormatInfo dtFormat =new System.Globalization.DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    DateTime dt = Convert.ToDateTime(filter.Value, dtFormat);

                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date == dt);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date != dt);
                            break;
                        case "IsGreaterThan":
                            list = list.Where(it => it.CreatedTime.Value.Date > dt);
                            break;
                        case "IsLessThan":
                            list = list.Where(it => it.CreatedTime.Value.Date < dt);
                            break;
                        case "IsGreaterThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date >= dt);
                            break;
                        case "IsLessThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date >= dt);
                            break;   
                    }
                }
                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedTime":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.CreatedTime);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.CreatedTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<OrderModel> result = new ListResult<OrderModel>();
                result.Data = new List<OrderModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        
        }
    }
}
