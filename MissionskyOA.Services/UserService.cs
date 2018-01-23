using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Email;
using MissionskyOA.Core.Security;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;

namespace MissionskyOA.Services
{
    public class UserService : IUserService
    {
        private readonly IUserTokenService _userTokenService = new UserTokenService();

        // 定义私有的接口变量，然后在构造函数中将IOC容器中相应的对象赋值给这个变量
        private ICryptology Cryptology { get; set; }
        private IAttendanceSummaryService AttendanceSummaryService { get; set; }

        public UserService()
        {
        }

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="cryptology"></param>
        /// <param name="attendanceSummaryService"></param>
        public UserService(ICryptology cryptology, IAttendanceSummaryService attendanceSummaryService)
        {
            this.Cryptology = cryptology;
            this.AttendanceSummaryService = attendanceSummaryService;
        }

        /// <summary>
        /// 根据用户ID 获取员工的基本信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>用户基本信息</returns>
        public UserModel GetUserDetail(int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.FirstOrDefault(it => it.Id == userId);

                if (entity != null)
                {
                    var user = entity.ToModel();
                    user.AuthNotify = ConvertNotifyAuth(entity.AuthNotify);
                    UserExtentions.FillRelatedDetail(dbContext, user); //填相关详细信息

                    return user;
                }

                return null;
            }
        }

        /// <summary>
        /// 根据电话号码/邮箱/中文名/英文名/QQ号获取员工的基本信息
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns>用户信息</returns>
        public UserModel GetUserDetail(SingleUserModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity =
                    dbContext.Users.FirstOrDefault(
                        it =>
                            (!string.IsNullOrEmpty(model.Phone) && model.Phone == it.Phone) ||
                            (!string.IsNullOrEmpty(model.Email) && model.Email == it.Email)
                            || (!string.IsNullOrEmpty(model.ChineseName) && model.ChineseName == it.ChineseName) ||
                            (!string.IsNullOrEmpty(model.EnglishName) && model.EnglishName == it.EnglishName)
                            || (!string.IsNullOrEmpty(model.QQID) && model.QQID == it.QQID));

                if (entity != null)
                {
                    var user = entity.ToModel();
                    user.AuthNotify = ConvertNotifyAuth(entity.AuthNotify);
                    UserExtentions.FillRelatedDetail(dbContext, user); //填相关详细信息

                    return user;
                }

                return null;
            }
        }

        /// <summary>
        /// 查询用户(员工)
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <returns>满足条件的用户</returns>
        public List<UserModel> GetUserList(SearchUserModel model = null)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.Users.AsEnumerable();

                //关键字查询
                if (model != null && !string.IsNullOrEmpty(model.Keyword))
                {
                    query =
                        query.Where(
                            it => it.ChineseName != null && it.ChineseName.ToLower().Contains(model.Keyword.ToLower())
                                  ||
                                  (it.EnglishName != null && it.EnglishName.ToLower().Contains(model.Keyword.ToLower())));
                }

                //项目查询
                if (model != null && model.ProjectId.HasValue && model.ProjectId.Value > 0)
                {
                    query = query.Where(it => it.ProjectId.HasValue && it.ProjectId.Value == model.ProjectId.Value);
                }

                //邮箱查询
                if (model != null && !string.IsNullOrEmpty(model.Email))
                {
                    query = query.Where(it => it.Email != null && it.Email.ToLower().Contains(model.Email.ToLower()));
                }

                //电话查询
                if (model != null && !string.IsNullOrEmpty(model.Phone))
                {
                    query = query.Where(it => it.Phone != null && it.Phone.ToLower().Contains(model.Phone.ToLower()));
                }

                //QQID
                if (model != null && !string.IsNullOrEmpty(model.QQID))
                {
                    query = query.Where(it => it.QQID != null && it.QQID.ToLower().Contains(model.QQID.ToLower()));
                }

                //账号状态，在职状态
                if (model != null)
                {
                    query =
                        query.Where(
                            it => (!it.Status.HasValue && model.Status == AccountStatus.Normal) //字段Status为空，默认为Normal状态
                                  || (it.Status.HasValue && it.Status.Value == (int) model.Status));
                }

                List<UserModel> result = new List<UserModel>();
                query.ToList().ForEach(entity =>
                {
                    var user = entity.ToModel();
                    var userRole = dbContext.UserRoles.FirstOrDefault(it => it.UserId == entity.Id) ?? new UserRole();

                    user.Role = userRole.UserId; //用户角色
                    user.Password = null; //清除Password
                    result.Add(user);
                });

                return result;
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns>是否重置成功</returns>
        public bool ResetPassword(ResetPasswordModel model)
        {
            string tempPassword = UserExtentions.RandomPassword();
            string title = "[OA]重置密码成功";
            string body = "{0}，你好。\r\n密码重置成功，密码为{1}。\r\n请及时修改密码。";

            if (model == null || (string.IsNullOrEmpty(model.Phone) && string.IsNullOrEmpty(model.Email)) ||
                (!string.IsNullOrEmpty(model.Email) && !model.Email.IsEmail()))
            {
                throw new Exception("Invalid reset password request.");
            }

            try
            {
                //更新用户信息
                using (var dbContext = new MissionskyOAEntities())
                {
                    //1.查找用户
                    var entity =
                        dbContext.Users.Where(
                            it =>
                                (!string.IsNullOrEmpty(it.Email) &&
                                 it.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(it.Phone) &&
                                 it.Phone.Equals(model.Phone, StringComparison.InvariantCultureIgnoreCase)))
                            .FirstOrDefault();

                    if (entity == null)
                    {
                        throw new KeyNotFoundException("cannot find user.");
                    }

                    //2.更新用户密码
                    entity.Password = (new MD5Cryptology()).Encrypt(tempPassword);
                    entity.Status = (int) AccountStatus.RestPassword;

                    dbContext.SaveChanges();

                    //3.发送邮件
                    EmailClient.Send(new List<string> {entity.Email}, null, title,
                        string.Format(body, entity.EnglishName, tempPassword));
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="token">登录用户Token</param>
        /// <param name="model">新旧密码</param>
        /// <returns>是否重置成功</returns>
        public bool UpdatePassowrd(string token, UpdatePasswordModel model)
        {
            //获取用户信息
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.Where(it => it.Token == token).FirstOrDefault(); //获取当前登录用户

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid user token.");
                }

                //旧密码不正确
                if (!entity.Password.Equals(model.Password, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Invalid old password.");
                }

                //新旧密码相同
                if (model.NewPassword.Equals(model.Password, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Invalid new password.");
                }

                //更新密码
                entity.Password = model.NewPassword;
                entity.Status = (int) AccountStatus.Normal;

                dbContext.SaveChanges();

                //过期Token，重新登录
                (new UserTokenService()).SetTokenExpired(token);
            }

            return true;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="model">用户信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        public bool UpdateUser(int id, UpdateUserModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.Where(it => it.Id == id).FirstOrDefault();
                var existedUser =
                    dbContext.Users.FirstOrDefault(
                        it => it.Id != id && (it.QQID.Equals(model.QQID) || it.Phone.Equals(model.Phone))); //已存在用户

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid user id.");
                }

                #region 验证是否有数据需要更新

                //英文名
                if (!String.IsNullOrEmpty(model.EnglishName) && model.EnglishName != entity.EnglishName)
                {
                    entity.EnglishName = model.EnglishName;
                }

                //中文名
                if (!String.IsNullOrEmpty(model.ChineseName) && model.ChineseName != entity.ChineseName)
                {
                    entity.ChineseName = model.ChineseName;
                }

                //电话号码
                if (!String.IsNullOrEmpty(model.Phone) && model.Phone != entity.Phone)
                {
                    if (existedUser != null && existedUser.Phone.Equals(model.Phone))
                    {
                        throw new InvalidOperationException("电话号码已存在");
                    }

                    entity.Phone = model.Phone;
                }

                //QQID
                if (!String.IsNullOrEmpty(model.QQID) && model.QQID != entity.QQID)
                {
                    if (existedUser != null && existedUser.QQID.Equals(model.QQID))
                    {
                        throw new InvalidOperationException("QQ号码已存在");
                    }

                    entity.QQID = model.QQID;
                }


                if (model.Email != null && (!model.Email.EqualsIgnoreCase(entity.Email)))
                {
                    entity.Email = model.Email;
                }
                //性别，如果是默认值则不更新
                if (model.Gender != Gender.Privacy)
                {
                    entity.Gender = (int) model.Gender;
                }

                //在职状态, 如果是默认值则不更新
                if (model.Status.HasValue)
                {
                    entity.Status = (int) model.Status.Value;
                }

                if (model.Available.HasValue)
                {
                    entity.Available = model.Available.Value;
                }

                if (model.TodayStatus.HasValue)
                {
                    entity.TodayStatus = (int) model.TodayStatus.Value;
                }

                //直接领导
                if (model.DirectlySupervisorId.HasValue)
                {
                    if (model.DirectlySupervisorId.Value != entity.DirectlySupervisorId)
                    {
                        //改变直接领导修改假单下一审批人
                        var orderEntity = dbContext.Orders.Where(it => it.NextAudit == entity.DirectlySupervisorId);
                        orderEntity.ToList().ForEach(item =>
                        {
                            item.NextAudit = model.DirectlySupervisorId.Value;
                        });

                        //修改直接领导人
                        entity.DirectlySupervisorId = model.DirectlySupervisorId.Value;
                    }
                }
                if (model.DeptId != null)
                {
                    entity.DeptId = model.DeptId;
                }
                //所在项目
                if (!String.IsNullOrEmpty(model.No))
                {
                    entity.No = model.No;
                }

                //所在项目
                if (model.ProjectId.HasValue)
                {
                    entity.ProjectId = model.ProjectId;
                }
                if (model.Position != null)
                {
                    entity.Position = model.Position;
                }
                if (model.Role != 0)
                {
                    var userRole = dbContext.UserRoles.Where(it => it.UserId == id).FirstOrDefault();
                    if (userRole != null)
                    {
                        userRole.RoleId = model.Role;
                    }
                    else
                    {
                        UserRole roleEntity = new UserRole()
                        {
                            UserId = id,
                            RoleId = model.Role
                        };
                        dbContext.UserRoles.Add(roleEntity);
                    }

                }
                if (model.JoinDate != null && model.JoinDate > DateTime.Parse("2012-01-01 09:00"))
                {
                    entity.JoinDate = model.JoinDate;
                }

                //工龄
                if ((int)model.ServerYearType != entity.ServerYearType)
                {
                    //entity.ServerYearType = (int)GetServerYearTypeForUpdateUserModel(model);
                    entity.ServerYearType = (int)model.ServerYearType;
                }

                if (model.ServerYear != entity.ServerYear)
                {
                    entity.ServerYear = model.ServerYear;
                }
                #endregion

                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 分页查询用户(员工)
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的用户</returns>
        public IPagedList<UserModel> SearchUsers(SearchUserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.Users.Where(it => it.Available);

                //关键字查询
                if (model != null && !string.IsNullOrEmpty(model.Keyword))
                {
                    query =
                        query.Where(
                            it => it.ChineseName != null && it.ChineseName.ToLower().Contains(model.Keyword.ToLower())
                                  ||
                                  (it.EnglishName != null && it.EnglishName.ToLower().Contains(model.Keyword.ToLower())));
                }

                //项目查询
                if (model != null && model.ProjectId.HasValue && model.ProjectId.Value > 0)
                {
                    query = query.Where(it => it.ProjectId.HasValue && it.ProjectId.Value == model.ProjectId.Value);
                }

                //员工编号
                if (model != null && !string.IsNullOrEmpty(model.No))
                {
                    query = query.Where(it => it.No != null && it.No.ToLower().Contains(model.No.ToLower()));
                }

                //邮箱查询
                if (model != null && !string.IsNullOrEmpty(model.Email))
                {
                    query = query.Where(it => it.Email != null && it.Email.ToLower().Contains(model.Email.ToLower()));
                }

                //电话查询
                if (model != null && !string.IsNullOrEmpty(model.Phone))
                {
                    query = query.Where(it => it.Phone != null && it.Phone.ToLower().Contains(model.Phone.ToLower()));
                }

                //QQID
                if (model != null && !string.IsNullOrEmpty(model.QQID))
                {
                    query = query.Where(it => it.QQID != null && it.QQID.ToLower().Contains(model.QQID.ToLower()));
                }

                //账号状态
                if (model != null)
                {
                    query =
                        query.Where(
                            it =>
                                (model.Status == AccountStatus.Normal &&
                                 (!it.Status.HasValue || it.Status == 0 || it.Status == 2))
                                || (it.Status.HasValue && it.Status.Value == (int) model.Status));
                }

                //今天工作状态
                if (model != null)
                {
                    query =
                        query.Where(
                            it =>
                                model.TodayStatus == UserTodayStatus.None ||
                                (model.TodayStatus != UserTodayStatus.None && it.TodayStatus == (int) model.TodayStatus));
                }

                List<UserModel> result = new List<UserModel>();
                query.ToList().ForEach(it =>
                {
                    it.Password = null; //清除Password
                    result.Add(it.ToModel());
                });

                return new PagedList<UserModel>(result, pageIndex, pageSize);
            }
        }

        public UserModel Login(LoginUserModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                string pwd = Cryptology.Encrypt(model.Password);
                var user =
                    dbContext.Users.Where(it => it.Email == model.UserName && it.Password == pwd).FirstOrDefault();
                if (user != null)
                {
                    return user.ToModel();
                }
            }

            return null;
        }

        public ListResult<UserModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Users.AsEnumerable();
                if (filter != null && filter.Member == "ChineseName")
                {
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.ChineseName == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.ChineseName != filter.Value);
                            break;
                        case "StartsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        !it.ChineseName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().EndsWith(filter.Value.ToLower()));
                            break;
                    }
                }
                if (filter != null && filter.Member == "EnglishName")
                {
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.EnglishName == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.EnglishName != filter.Value);
                            break;
                        case "StartsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            list =
                                list.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            list =
                                list.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        !it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().EndsWith(filter.Value.ToLower()));
                            break;
                    }
                }
                if (filter != null && filter.Member == "Phone")
                {
                    switch (filter.Operator)
                    {
                        case "Contains":
                            list =
                                list.Where(it => it.Phone != null && it.Phone.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        default:
                            break;
                    }
                }

                if (filter != null && filter.Member == "ChineseName")
                {
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.ChineseName == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.ChineseName != filter.Value);
                            break;
                        case "StartsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        !it.ChineseName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            list =
                                list.Where(
                                    it =>
                                        it.ChineseName != null &&
                                        it.ChineseName.ToLower().EndsWith(filter.Value.ToLower()));
                            break;
                    }
                }
                if (filter != null && filter.Member == "No")
                {
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.No == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.No != filter.Value);
                            break;
                        case "StartsWith":
                            list = list.Where(it => it.No != null && it.No.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            list = list.Where(it => it.No != null && it.No.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            list = list.Where(it => it.No != null && !it.No.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            list = list.Where(it => it.No != null && it.No.ToLower().EndsWith(filter.Value.ToLower()));
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

                list = list.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();

                ListResult<UserModel> result = new ListResult<UserModel>();
                result.Data = new List<UserModel>();
                list.ToList().ForEach(item =>
                {
                    var userModel = item.ToModel();
                    UserExtentions.FillRelatedDetail(dbContext, userModel); //填相关详细信息
                    result.Data.Add(userModel);

                });

                result.Total = count;
                return result;
            }
        }

        /// <summary>
        /// 获取领导信息
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetLeaders()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var leaderIds = dbContext.Users.Select(it => it.DirectlySupervisorId).Distinct();
                var leaderEntities = dbContext.Users.Where(it => leaderIds.Contains(it.Id) && it.Available);
                var leaders = new List<UserModel>();

                leaderEntities.ToList().ForEach(it => leaders.Add(it.ToModel()));

                return leaders;
            }
        }

        /// <summary>
        /// 获取部门数据
        /// </summary>
        /// <returns></returns>
        public List<DepartmentModel> GetDepartments()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entities = dbContext.Departments;
                var depts = new List<DepartmentModel>();

                entities.ToList().ForEach(it =>  depts.Add(it.ToModel()));

                return depts;
            }
        }

        public bool AddUser(UserModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //设置初始密码
                model.Password = (new MD5Cryptology()).Encrypt("123456");
                model.CreatedTime = DateTime.Now;

                // 判断输入的工龄，如果输入为负数，直接赋值为0
                if(model.ServerYear < 0)
                {
                    model.ServerYear = Math.Abs(model.ServerYear);
                }

                // 根据ServerYear处理ServerYearType
                //model.ServerYearType = GetServerYearType(model);

                //int year = model.ServerYear;
                if (model.ServerYear >= 0 && model.ServerYear < 10)
                {
                    model.ServerYearType = UserServiceYearType.TenYears;
                }
                else if (model.ServerYear >= 10 && model.ServerYear < 20)
                {
                    model.ServerYearType = UserServiceYearType.TwentyYears;
                }
                else if (model.ServerYear >= 20)
                {
                    model.ServerYearType = UserServiceYearType.ThirtyYears;
                }
              
                var UserEntity = model.ToEntity();
                dbContext.Users.Add(model.ToEntity());

                //初始化角色
                UserRole userRole = new UserRole()
                {
                    UserId = UserEntity.Id,
                    RoleId = model.Role,
                };
                dbContext.UserRoles.Add(userRole);

                //初始化所有假期
                for (int i = 1; i < 7; i++)
                {
                    AttendanceSummary attendance = new AttendanceSummary()
                    {
                        UserId = UserEntity.Id,
                        Year = DateTime.Now.Year,
                        Type = i,
                        LastValue = 0,
                        BaseValue = 0,
                        RemainValue = 0
                    };

                    dbContext.AttendanceSummaries.Add(attendance);
                }
                dbContext.SaveChanges();
                return true;
            }

        }

        public List<UserModel> GetManagementUsers()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.UserRoles.Where(it => it.RoleId != 5 && it.RoleId != 6);
                List<UserModel> result = new List<UserModel>();
                query.ToList().ForEach(it =>
                {
                    var userQuery = dbContext.Users.Where(its => its.Id == it.UserId).FirstOrDefault();
                    result.Add(userQuery.ToModel());
                });

                return result;
            }
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="id">删除用户信息</param>
        /// <returns></returns>
        /// <remarks>是否删除成功</remarks>
        public bool Remove(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.FirstOrDefault(it => it.Id == id);

                if (entity == null)
                {
                    throw new KeyNotFoundException("无效的user id");
                }
                if (entity.Available == false)
                {
                    entity.Available = true;
                }
                else if (entity.Available == true)
                {
                    entity.Available = false;

                    var roleQuery = dbContext.UserRoles.Where(its => its.UserId == entity.Id).FirstOrDefault();
                    dbContext.UserRoles.Remove(roleQuery);

                    //var attendancyQuery = dbContext.AttendanceSummaries.Where(its => its.UserId == entity.Id);
                    //attendancyQuery.ToList().ForEach(it =>
                    //{
                    //    dbContext.AttendanceSummaries.Remove(it);
                    //});
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        public List<DepartmentModel> GetDets()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.Departments.AsEnumerable();
                List<DepartmentModel> result = new List<DepartmentModel>();
                query.ToList().ForEach(it =>
                {
                    var deptMode = new DepartmentModel()
                    {
                        Id = it.Id,
                        Name = it.Name,
                        No = it.No,
                        CreatedDate = it.CreatedDate
                    };
                    result.Add(deptMode);
                });

                return result;
            }
        }

        /// <summary>
        /// 导入用户假期基准信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>假期基准信息</returns>
        public NameValueCollection ImportUserBaseVacation(int id)
        {
            //To-do: the value may be from othe data source

            var baseVacations = new NameValueCollection();
            baseVacations.Add(OrderType.AnnualLeave.ToString(), "0.0");
            baseVacations.Add(OrderType.SickLeave.ToString(), "0.0");
            baseVacations.Add(OrderType.FuneralLeave.ToString(), "0.0");
            baseVacations.Add(OrderType.MarriageLeave.ToString(), "0.0");
            baseVacations.Add(OrderType.MaternityLeave.ToString(), "0.0");

            return baseVacations;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<UserModel>();
                dbContext.Users.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return result;
            }

        }

        /// <summary>
        /// 获取多个用户英文名
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userIds">用户Id</param>
        /// <returns></returns>
        public string GetUsersName(MissionskyOAEntities dbContext, int[] userIds)
        {
            string userInfo = string.Empty;

            /*
             * userIds.ToList().ForEach()的使用，遍历数组中的每一项，然后为每一项做操作
             * 
             * **/
            userIds.ToList().ForEach(it => userInfo += "," + it);

            // 从下标1开始截取
            userInfo = userInfo.Substring(1);

            /*
             * SELECT ',' + EnglishName FROM [MissionskyOA].[dbo].[User] WHERE Id IN (1,2,3) FOR XML PATH('')
             * 
             *      XML_F52E2B61-18A1-11d1-B105-00805F49916B
             *      ,Carly Xu,James Xu,Felix Chen
             * 
             * **/
            var sql = @"SELECT ',' + EnglishName FROM [User] WHERE Id IN ({0}) FOR XML PATH('');";
            var data = dbContext.Database.SqlQuery<string>(string.Format(sql, userInfo));

            if (data != null)
            {
                userInfo = data.FirstOrDefault();

                if (!string.IsNullOrEmpty(userInfo) && userInfo.Length > 2)
                {
                    userInfo = userInfo.Substring(1);
                }
            }

            return userInfo;
        }

        /// <summary>
        /// 获取申请单用户信息
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userIds">申请单用户</param>
        /// <returns></returns>
        public IList<OrderUserModel> GetOrderUsers(MissionskyOAEntities dbContext, int[] userIds)
        {
            // 1、新建一个OrderUserModel的IList变量，用于返回
            IList<OrderUserModel> users = new List<OrderUserModel>();

            // 2、判断UserIds是否为空
            if (userIds != null && userIds.Count() > 0)
            {
                /*
                 * 此设计Lamda表达式的使用
                 * 
                 * **/
                userIds.ToList().ForEach(userId =>
                {
                    // 3、获取申请用户 根据userId 从User表中
                    var user = dbContext.Users.FirstOrDefault(it => it.Id == userId);

                    // 判断获取的user是否为空
                    if (user != null)
                    {
                        //用户信息
                        // 4、将User 转换为 OrderUserModel
                        var orderUser = new OrderUserModel()
                        {
                            Id = userId,
                            UserName = user.EnglishName,
                            /*
                             * 三目运算 和 枚举类型的实例使用
                             * 
                             * **/
                            Gender = user.Gender.HasValue ? (Gender) user.Gender.Value : Gender.Privacy,
                        };

                        // 5、获取部门信息，并添加到OrderUserModel
                        var dept = dbContext.Departments.FirstOrDefault(it => it.Id == user.DeptId);

                        orderUser.DeptName = (dept == null ? string.Empty : dept.Name);

                        // 6、获取 项目名称，并添加到OrderUserModel
                        var project = dbContext.Projects.FirstOrDefault(it => it.Id == user.ProjectId);
                        orderUser.ProjectName = (project == null ? string.Empty : project.Name);

                        // 添加到IList<OrderuserModel>中
                        users.Add(orderUser);
                    }
                });
            }

            return users;
        }

        /// <summary>
        /// 获取申请单用户信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="orderNo">批量申请Id</param>
        /// <returns></returns>
        public IList<OrderUserModel> GetOrderUsers(MissionskyOAEntities dbContext, int orderNo)
        {
            IList<OrderUserModel> users = new List<OrderUserModel>();
            var orders = dbContext.Orders.Where(it => it.OrderNo == orderNo);

            orders.ToList().ForEach(order =>
            {
                var user = dbContext.Users.FirstOrDefault(it => it.Id == order.UserId);

                if (user != null)
                {
                    //用户信息
                    var orderUser = new OrderUserModel()
                    {
                        Id = order.UserId,
                        OrderId = order.Id,
                        UserName = user.EnglishName,
                        Gender = user.Gender.HasValue ? (Gender) user.Gender.Value : Gender.Privacy,
                    };

                    //部门信息
                    var dept = dbContext.Departments.FirstOrDefault(it => it.Id == user.DeptId);
                    orderUser.DeptName = (dept == null ? string.Empty : dept.Name);

                    //项目名称
                    var project = dbContext.Projects.FirstOrDefault(it => it.Id == user.ProjectId);
                    orderUser.ProjectName = (project == null ? string.Empty : project.Name);

                    users.Add(orderUser);
                }
            });

            return users;
        }

        /// <summary>
        /// 用户消息推送授权
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updatedItems">变更后的授权</param>
        /// <returns></returns>
        public bool ChangeNotifyAuth(string token, Dictionary<string, bool> updatedItems)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("无效的用户Token");
            }

            if (updatedItems == null)
            {
                throw new InvalidOperationException("无效的用户消息推送授权");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.FirstOrDefault(it => it.Token.Equals(token, StringComparison.OrdinalIgnoreCase));

                if (entity == null)
                {
                    throw new InvalidOperationException("无效的用户信息"); 
                }

                var tempItems = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                updatedItems.ToList().ForEach(it => tempItems.Add(it.Key, it.Value));

                var authItems = ConvertNotifyAuth(entity.AuthNotify);

                //合并授权
                authItems.ToList().ForEach(it =>
                {
                    if (tempItems.ContainsKey(it.Key))
                    {
                        authItems[it.Key] = tempItems[it.Key];
                    }
                });

                entity.AuthNotify = ConvertNotifyAuth(authItems);
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 转换用户推送授权成键值对
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> ConvertNotifyAuth(string auth)
        {
            if (string.IsNullOrEmpty(auth))
            {
                auth = Constant.USER_NOTIFY_DEFAULT_AUTH;
            }

            auth = auth.EndsWith(";") ? auth.Substring(0, auth.Length - 1) : auth;

            var items = auth.Split(new[] { ':', ';' });
            var authItems = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            if (items.Length%2 != 0)
            {
                throw new InvalidOperationException("无效的用户消息推送授权");
            }

            for (int i = 0; i < items.Length;)
            {
                authItems.Add(items[i++], items[i++] == "1");
            }

            return authItems;
        }

        /// <summary>
        /// 转换用户推送授权为字符串
        /// </summary>
        /// <param name="authItems"></param>
        /// <returns></returns>
        public static string ConvertNotifyAuth(Dictionary<string, bool> authItems)
        {
            if (authItems == null)
            {
                throw new InvalidOperationException("无效的用户消息推送授权");
            }

            var notifyAuth = string.Empty;

            authItems.ToList().ForEach(it =>
            {
                notifyAuth += string.Format("{0}:{1};", it.Key, it.Value ? "1" : "0");
            });

            return notifyAuth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserServiceYearType GetServerYearTypeForUpdateUserModel(UpdateUserModel model) 
        {
            int year = model.ServerYear;
            if (year >= 0 && year < 10)
            {
                model.ServerYearType = UserServiceYearType.TenYears;
            }
            else if(year >= 10 && year < 20)
            {
                model.ServerYearType = UserServiceYearType.TwentyYears;
            }
            else if (year >= 20)
            {
                model.ServerYearType = UserServiceYearType.ThirtyYears;
            }

            return model.ServerYearType;
        }

    }
}