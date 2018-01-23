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
    public static class MeetingParticipantExtentions
    {
        public static MeetingParticipant ToEntity(this MeetingParticipantModel model)
        {
            var entity = new MeetingParticipant()
            {
                MeetingCalendarId=model.MeetingCalendarId,
                UserId=model.UserId,
                IsOptional=model.IsOptional
            };
            return entity;
        }
        public static MeetingParticipantModel ToModel(this MeetingParticipant entity)
        {
            UserModel userModel = null;
            if (entity.User != null)
            {
                userModel = new UserModel()
                {
                    Id = entity.User.Id,
                    ChineseName = entity.User.ChineseName,
                    EnglishName = entity.User.EnglishName,
                    Email = entity.User.Email,
                    Phone = entity.User.Phone
                };
            }

            var model = new MeetingParticipantModel()
            {
                Id=entity.Id,
                MeetingCalendarId = entity.MeetingCalendarId,
                UserId = entity.UserId,
                IsOptional = entity.IsOptional,
                User = userModel
            };
            return model;
        }
    }
}
