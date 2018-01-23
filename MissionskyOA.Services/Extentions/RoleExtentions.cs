using System.Collections.Generic;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Extentions
{
    public static class RoleExtentions
    {
        public static Role ToEntity(this RoleModel model)
        {
            var entity = new Role()
            {
                Id = model.Id,
                Name = model.RoleName,
                ApprovedDays=model.ApprovedDays,
                CreatedTime=model.CreatedTime,
                Abbreviation = model.Abbreviation,
                IsInitRole = model.IsInitRole,
                CreateUser = model.CreateUser,
                Status=model.Status
            };
            return entity;
        }

        public static RoleModel ToModel(this Role entity)
        {
            var model = new RoleModel()
            {
                Id = entity.Id,
                RoleName = entity.Name,
                ApprovedDays = entity.ApprovedDays,
                CreatedTime = entity.CreatedTime,
                Abbreviation = entity.Abbreviation,
                IsInitRole = entity.IsInitRole,
                CreateUser = entity.CreateUser,
                Status = entity.Status
            };
            return model;
        }
    }
}
