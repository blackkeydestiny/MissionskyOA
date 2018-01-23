using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class ExpenseMemberExtentions
    {
        public static ExpenseMember ToEntity(this ExpenseMemberModel model)
        {
            var entity = new ExpenseMember()
            {
                DId = model.DId,
                MemberId = model.MemberId,

            };
            return entity;
        }

        public static ExpenseMemberModel ToModel(this ExpenseMember entity)
        {
            var model = new ExpenseMemberModel()
            {
                Id=entity.Id,
                DId = entity.DId,
                MemberId = entity.MemberId,
                User=entity.User.ToModel()
            };
            return model;
        }
    }
}
