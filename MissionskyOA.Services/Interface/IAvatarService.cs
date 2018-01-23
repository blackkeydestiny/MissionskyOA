using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 头像管理
    /// </summary>
    public partial interface IAvatarService
    {
        /// <summary>
        /// 保存头像
        /// </summary>
        /// <param name="avatar">头像详细信息</param>
        /// <returns>true or false</returns>
        bool Save(AvatarModel avatar);

        /// <summary>
        /// 根据会员ID获取头像
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <returns>头像详细信息</returns>
        AvatarModel GetByUserId(int userId);
    }

}
