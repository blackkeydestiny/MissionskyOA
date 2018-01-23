using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 部门扩展处理
    /// </summary>
    public static class DepartmentExtensions
    {
        public static DepartmentModel ToModel(this Department entity)
        {
            var model = new DepartmentModel()
            {
                Id = entity.Id,
                No = entity.No,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                CreateUserName=entity.CreateUserName,
                DepartmentHead=entity.DepartmentHead,
                Status=entity.Status
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.DepartmentHead);
                if (userEntity != null)
                {
                    model.DepartmentHeadName = userEntity.EnglishName;
                }
            }
            return model;
        }

        public static Department ToEntity(this DepartmentModel model)
        {
            var entity = new Department()
            {
                Id = model.Id,
                No = model.No,
                Name = model.Name,
                CreatedDate = model.CreatedDate,
                CreateUserName = model.CreateUserName,
                DepartmentHead = model.DepartmentHead,
                Status = model.Status
            };

            return entity;
        }
    }
}
