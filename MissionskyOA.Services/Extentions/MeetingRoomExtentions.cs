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
    public static class MeetingRoomExtentions
    {
        public static MeetingRoom ToEntity(this MeetingRoomModel model)
        {
            var entity = new MeetingRoom()
            {
                MeetingRoomName=model.MeetingRoomName,
                Capacity=model.Capacity,
                Equipment=model.Equipment,
                Remark=model.Remark,
                Status=model.Status,
                CreateUserName=model.CreateUserName,
                CreateDate=model.CreateDate
            };
            return entity;
        }
        public static MeetingRoomModel ToModel(this MeetingRoom entity)
        {
            var model = new MeetingRoomModel()
            {
                Id=entity.Id,
                MeetingRoomName = entity.MeetingRoomName,
                Capacity = entity.Capacity,
                Equipment = entity.Equipment,
                Remark = entity.Remark,
                Status = entity.Status,
                CreateUserName = entity.CreateUserName,
                CreateDate = entity.CreateDate
            };
            return model;
        }
    }
}
