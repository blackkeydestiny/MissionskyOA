using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;
using System.Collections.ObjectModel;

namespace MissionskyOA.Services
{
    public static class MeetingCalendarExtentions
    {
        public static MeetingCalendar ToEntity(this MeetingCalendarModel model)
        {
            var entity = new MeetingCalendar()
            {
                MeetingRoomId = model.MeetingRoomId,
                Title = model.Title,
                StartDate = model.StartDate,
                StartTime = model.StartTime,
                Host = model.Host,
                EndDate = model.EndDate,
                EndTime = model.EndTime,
                MeetingContext = model.MeetingContext,
                MeetingSummary = model.MeetingSummary,
                ApplyUserId = model.ApplyUserId,
                CreatedTime = model.CreatedTime,
            };

            Collection<MeetingParticipant> result = new Collection<MeetingParticipant>();

            if (model.MeetingParticipants != null && model.MeetingParticipants.Count > 0)
            {
                foreach (MeetingParticipantModel item in model.MeetingParticipants)
                {
                    result.Add(item.ToEntity());
                }
            }
            entity.MeetingParticipants = result;
            return entity;
        }
        public static MeetingCalendarModel ToModel(this MeetingCalendar entity)
        {
            var model = new MeetingCalendarModel()
            {
                Id = entity.Id,
                MeetingRoomId = entity.MeetingRoomId,
                Title = entity.Title,
                StartDate = entity.StartDate,
                StartTime = entity.StartTime,
                EndDate = entity.EndDate,
                EndTime = entity.EndTime,
                Host = entity.Host,
                ApplyUserId = entity.ApplyUserId,
                MeetingContext = entity.MeetingContext,
                MeetingSummary = entity.MeetingSummary,
                MeetingRoom = entity.MeetingRoom.ToModel(),
                CreatedTime = entity.CreatedTime
            };
            model.StartDate = Convert.ToDateTime(entity.StartDate.ToShortDateString() + " " + entity.StartTime.ToString());
            model.EndDate = Convert.ToDateTime(entity.EndDate.ToShortDateString() + " " + entity.EndTime.ToString());
            ICollection<MeetingParticipant> meetingParticipant = entity.MeetingParticipants;
            List<MeetingParticipantModel> result = new List<MeetingParticipantModel>();

            if (meetingParticipant != null && meetingParticipant.Count > 0)
            {
                foreach (MeetingParticipant item in meetingParticipant)
                {
                    result.Add(item.ToModel());
                }
            }
            //Get english name
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.ApplyUserId);
                if (userEntity != null)
                {
                    model.ApplyUserName = userEntity.EnglishName;
                }
            }

            model.MeetingParticipants = result;
            return model;
        }
    }
}
