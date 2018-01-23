using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Data;
using MissionskyOA.Models;
using System.Threading;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 用户会话管理
    /// </summary>
    public class UserTokenService : ServiceBase, IUserTokenService
    {
        /// <summary>
        /// 用户登录，生成Token
        /// </summary>
        /// <param name="model">用户信息</param>
        /// <returns></returns>
        public TokenModel GetUserToken(LoginModel model)
        {
            var tokenModel = new TokenModel();

            using (var dbContext = new MissionskyOAEntities())
            {
                if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.QQID))
                {
                    throw new KeyNotFoundException("Invalid login account.");
                }

                var existEntity =
                    dbContext.Users.FirstOrDefault(it => (!string.IsNullOrEmpty(model.Email) && it.Email == model.Email)
                                                         ||
                                                         (!string.IsNullOrEmpty(model.QQID) && it.QQID == model.QQID));

                var validEntity =
                    dbContext.Users.FirstOrDefault(
                        it =>
                            ((!string.IsNullOrEmpty(model.Email) && it.Email == model.Email)
                             || (!string.IsNullOrEmpty(model.QQID) && it.QQID == model.QQID))
                            &&
                            it.Password == model.Password);

                if (existEntity == null)
                {
                    Log.Error("用户不存在。");
                    throw new KeyNotFoundException("用户不存在。");
                }

                if (validEntity == null)
                {
                    Log.Error("用户名密码无效。");
                    throw new KeyNotFoundException("用户名密码无效。");
                }

                if (!validEntity.Available)
                {
                    Log.Error("用户已经离职。");
                    throw new KeyNotFoundException("用户已经离职。");
                }

                validEntity.Token = this.GenerateToken(); // 获取用户Token                
                validEntity.ExpirationTime = DateTime.Now.AddDays(2); // 设置Token有效期
                (new AttendanceSummaryService()).InitAttendanceSummary(dbContext, validEntity.Id); //初使化假期信息

                dbContext.SaveChanges();
                tokenModel.Token = validEntity.Token;
                tokenModel.Id = validEntity.Id;
                tokenModel.JPushAlias = string.IsNullOrEmpty(validEntity.Email)
                    ? null
                    : validEntity.Email.ToLower().Split('@')[0].Replace(".", "").Replace(" ", "");
                if (validEntity.ProjectId.HasValue)
                {
                    var project = dbContext.Projects.FirstOrDefault(it => it.Id == validEntity.ProjectId.Value);

                    if (project != null)
                    {
                        tokenModel.JPushTag = project.Name.Replace(" ", "");
                    }
                }
            }

            return tokenModel;
        }

        /// <summary>
        /// 根据Token获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserModel GetMemberByToken(string token)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.FirstOrDefault(it => !string.IsNullOrEmpty(it.Token) && it.Token == token);

                if (entity == null)
                {
                    Log.Error(string.Format("无效的Token: {0}", token));
                    throw new KeyNotFoundException("Cannot find the current user by token.");
                }

                var user = entity.ToModel();
                user.AuthNotify = UserService.ConvertNotifyAuth(entity.AuthNotify);
                UserExtentions.FillRelatedDetail(dbContext, user); //填相关详细信息

                return user;
            }
        }

        /// <summary>
        /// 验证Token是否过期
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsVaild(string token)
        {
            bool isValid = false;

            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.Where(it => it.Token == token).FirstOrDefault();
                if (entity != null && !String.IsNullOrEmpty(entity.Token) && DateTime.Now <= entity.ExpirationTime)
                {
                    entity.ExpirationTime = DateTime.Now.AddDays(2);
                    dbContext.SaveChanges();
                    isValid = true;
                }
            }

            return isValid;
        }

        /// <summary>
        /// 设置Token过期
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool SetTokenExpired(string token)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.Where(it => it.Token == token).FirstOrDefault();

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid token.");
                }

                //清空Token
                entity.Token = null;
                //设置Token过期
                entity.ExpirationTime = DateTime.Now;
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 验证用户是否已经锁定
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsLockStatus(string token)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Users.Where(it => it.Token == token).FirstOrDefault();

                if (entity == null)
                {
                    throw new KeyNotFoundException("Invalid token.");
                }

                return entity.Status == (int)AccountStatus.Lock;
            }
        }

        private Random rnd = new Random();
        private int seed = 0;

        /// <summary>
        /// 生成用户Token
        /// </summary>
        /// <returns></returns>
        private string GenerateToken()
        {
            var rndData = new byte[48];
            rnd.NextBytes(rndData);
            var seedValue = Interlocked.Add(ref seed, 1);
            var seedData = BitConverter.GetBytes(seedValue);
            var tokenData = rndData.Concat(seedData).OrderBy(_ => rnd.Next());

            return Convert.ToBase64String(tokenData.ToArray()).TrimEnd('=');
        }
    }
}
