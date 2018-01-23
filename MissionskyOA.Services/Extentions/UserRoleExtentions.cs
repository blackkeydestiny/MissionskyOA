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
    public static class UserRoleExtentions
    {
        public static UserRole ToEntity(this UserRolesModel model)
        {
            var entity = new UserRole()
            {
                Id = model.Id,
                RoleId = model.RoleId,
                UserId = model.UserId,

            };
            return entity;
        }

        public static UserRolesModel ToModel(this UserRole entity)
        {
            var model = new UserRolesModel()
            {
                Id = entity.Id,
                RoleId = entity.RoleId,
                UserId = entity.UserId,
            };
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get refrence order
                if (entity.UserId != null)
                {
                    var refUserEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.UserId);
                    if(refUserEntity!=null)
                    {
                        // get user info
                        model.ChineseName = refUserEntity.ChineseName;
                        model.EnglishName = refUserEntity.EnglishName;
                        model.Available = refUserEntity.Available;
                        model.Position = refUserEntity.Position;
                        //get department name
                        var refDeptEntity = dbContext.Departments.FirstOrDefault(it => it.Id == refUserEntity.DeptId);
                        if (refDeptEntity!=null)
                        {
                            model.DeptName = refDeptEntity.Name;
                        }
                        //get project name
                        var refProjectEntity = dbContext.Projects.FirstOrDefault(it => it.Id == refUserEntity.ProjectId);
                        if (refProjectEntity != null)
                        {
                            model.ProjectName = refProjectEntity.Name;
                        }

                    }
                    
                }
            }
            return model;
        }

    }
}
