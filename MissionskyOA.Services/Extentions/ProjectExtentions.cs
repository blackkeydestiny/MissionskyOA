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
    public static class ProjectExtentions
    {
        public static Project ToEntity(this ProjectModel model)
        {
            var entity = new Project()
            {
                Name=model.Name,
                CreatedTime=model.CreatedTime,
                CreateUserName=model.CreateUserName,
                ProjectBegin=model.ProjectBegin,
                ProjectEnd=model.ProjectEnd,
                ProjectNo=model.ProjectNo,
                Status=model.Status,
                ProjectManager=model.ProjectManager
                
            };
            return entity;
        }

        public static ProjectModel ToModel(this Project entity)
        {
            var model = new ProjectModel()
            {
                Id = entity.Id,
                ProjectId=entity.Id,
                Name = entity.Name,
                CreatedTime = entity.CreatedTime,
                CreateUserName = entity.CreateUserName,
                ProjectBegin = entity.ProjectBegin,
                ProjectEnd = entity.ProjectEnd,
                ProjectNo = entity.ProjectNo,
                Status = entity.Status,
                ProjectManager = entity.ProjectManager
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.ProjectManager);
                if(userEntity!=null)
                {
                    model.ProjectManagerName = userEntity.EnglishName;
                }
            }
            return model;
        }
    }
}
