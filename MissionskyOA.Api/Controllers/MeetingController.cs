using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;
using MissionskyOA.Core.Common;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 会议管理
    /// </summary>
    [RoutePrefix("api/meeting")]
    public class MeetingController : BaseController
    {

        private IMeetingService MeetingService { get; set; }
        private IUserService UserService { get; set; }
        private readonly INotificationService _notificationService = new NotificationService();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="meetingService">会议管理服务</param>
        public MeetingController(IMeetingService meetingService, IUserService userService)
        {
            this.MeetingService = meetingService;
            this.UserService = userService;
        }

        /// <summary>
        /// 获取会议室列表
        /// </summary>
        /// <returns></returns>
        [Route("meetingRoomList")]
        [HttpGet]
        public ApiListResponse<MeetingRoomModel> GetMeetingRoomList()
        {

            ApiListResponse<MeetingRoomModel> response = new ApiListResponse<MeetingRoomModel>()
            {
                Result = this.MeetingService.GetMeetingRoomList()
            };

            return response;
        }

        /// <summary>
        /// 新增单条会议
        /// </summary>
        /// <returns></returns>
        [Route("addMeeting")]
        [HttpPost]
        [RequestBodyFilter]
        public ApiResponse<MeetingCalendarModel> AddMeeting(ReserveMeetingModel model)
        {
            // 1. 检查输入参数
            if (model == null)
            {
                throw new ApiBadRequestException("无请求数据");
            }
            if (model.MeetingRoomId==0)
            {
                throw new ApiBadRequestException("会议室不存在");
            }
            if(model.StartDate.Date!=model.EndDate.Date)
            {
                throw new ApiBadRequestException("开始日期与结束日期需在同一天");
            }
            if (model.StartDate >= model.EndDate)
            {
                throw new ApiBadRequestException("开始时间必须小于结束时间");
            }
            if (model.StartDate < DateTime.Now.AddMinutes(10))
            {
                throw new ApiBadRequestException("必须提前十分钟预定会议室");
            }
            if(MeetingService.isMeetingRoomAvailiable(model.MeetingRoomId,model.StartDate,model.EndDate)==false)
            {
                throw new ApiBadRequestException("会议室已经占用");
            }

            // 2. 构造会议预约记录
            var meetingCalendar = new MeetingCalendarModel()
            {
                ApplyUserId = this.Member.Id,
                MeetingRoomId = model.MeetingRoomId,
                Title = model.Title,
                Host=model.Host,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MeetingContext=model.MeetingContext,
                StartTime = model.StartDate.TimeOfDay,
                EndTime = model.EndDate.TimeOfDay,
                CreatedTime = DateTime.Now
            };
            //2.1 构造会议必须参与人员模型
            List<MeetingParticipantModel> meetingParticipants = new List<MeetingParticipantModel>();
            if (model.participants != null && (model.participants.Length > 0))
            {
                foreach (int item in model.participants)
                {
                    var meetingParticipantModel = new MeetingParticipantModel()
                    {
                        UserId = item,
                        IsOptional="No"
                    };
                    meetingParticipants.Add(meetingParticipantModel);
                }
            }

            //2.1 构造会议可选参与人员模型
            if (model.optionalParticipants != null && (model.optionalParticipants.Length > 0))
            {
                foreach (int item in model.optionalParticipants)
                {
                    var meetingParticipantModel = new MeetingParticipantModel()
                    {
                        UserId = item,
                        IsOptional="Yes"
                    };
                    meetingParticipants.Add(meetingParticipantModel);
                }
            }
            meetingCalendar.MeetingParticipants = meetingParticipants;
            //新增会议预约
            // 3. Construct API Response
            ApiResponse<MeetingCalendarModel> response = new ApiResponse<MeetingCalendarModel>()
            {
                Result = MeetingService.AddMeeting(meetingCalendar,false)
            };
            return response;
        }


        /// <summary>
        /// 批量新增会议
        /// </summary>
        /// <returns></returns>
        [Route("addMeetingByWeeks")]
        [HttpPost]
        [RequestBodyFilter]
        public ApiResponse<bool> AddMeetingByWeeks(ReserveMeetingModel model,int loopCount)
        {
            // 1. 检查输入参数
            if (model == null)
            {
                throw new ApiBadRequestException("无请求数据");
            }
            if (model.MeetingRoomId == 0)
            {
                throw new ApiBadRequestException("会议室不存在");
            }
            if (loopCount>10)
            {
                throw new ApiBadRequestException("会议循环次数不能大于十次");
            }
            if(model.MeetingDayInWeek==null||model.MeetingDayInWeek.Length<1)
            {
                throw new ApiBadRequestException("请选择会议举办日期");
            }
            if (model.StartDate >= model.EndDate)
            {
                throw new ApiBadRequestException("会议开始时间必须小于结束时间");
            }

            bool isMeetingAdded = false;
            if(model.MeetingDayInWeek!=null && model.MeetingDayInWeek.Length>0)
            {
                DateTime today = DateTime.Now;

                int TodayInDayOfWeek = (int)today.DayOfWeek;
                DateTime WeekStartDateStartTime = today.AddDays(-TodayInDayOfWeek).Date+model.StartDate.TimeOfDay;//当前星期的星期天会议开始时间
                DateTime WeekStartDateEndTime = today.AddDays(-TodayInDayOfWeek).Date+model.EndDate.TimeOfDay;//当前星期的星期天会议结束时间
                for (int i = 0; i <= loopCount;i++ )
                {
                    for (int j = 0; j < model.MeetingDayInWeek.Length;j++ )
                    {
                        DateTime StartDate=WeekStartDateStartTime.AddDays((int)model.MeetingDayInWeek[j]);
                        DateTime EndDate=WeekStartDateEndTime.AddDays((int)model.MeetingDayInWeek[j]);
                        if(StartDate>today)
                        {
                            if (MeetingService.isMeetingRoomAvailiable(model.MeetingRoomId,StartDate,EndDate) == false)
                            {
                                throw new ApiBadRequestException("会议室已经占用");
                            }
                            else
                            {
                                // 2. 构造会议预约记录
                                var meetingCalendar = new MeetingCalendarModel()
                                {
                                    ApplyUserId = this.Member.Id,
                                    MeetingRoomId = model.MeetingRoomId,
                                    Title = model.Title,
                                    Host = model.Host,
                                    StartDate = StartDate,
                                    EndDate = EndDate,
                                    MeetingContext = model.MeetingContext,
                                    StartTime = model.StartDate.TimeOfDay,
                                    EndTime = model.EndDate.TimeOfDay,
                                    CreatedTime = DateTime.Now
                                };
                                //2.1 构造会议必须参与人员模型
                                List<MeetingParticipantModel> meetingParticipants = new List<MeetingParticipantModel>();
                                if (model.participants != null && (model.participants.Length > 0))
                                {
                                    foreach (int item in model.participants)
                                    {
                                        var meetingParticipantModel = new MeetingParticipantModel()
                                        {
                                            UserId = item,
                                            IsOptional = "No"
                                        };
                                        meetingParticipants.Add(meetingParticipantModel);
                                    }
                                }

                                //2.1 构造会议可选参与人员模型
                                if (model.optionalParticipants != null && (model.optionalParticipants.Length > 0))
                                {
                                    foreach (int item in model.optionalParticipants)
                                    {
                                        var meetingParticipantModel = new MeetingParticipantModel()
                                        {
                                            UserId = item,
                                            IsOptional = "Yes"
                                        };
                                        meetingParticipants.Add(meetingParticipantModel);
                                    }
                                }
                                meetingCalendar.MeetingParticipants = meetingParticipants;
                                MeetingService.AddMeeting(meetingCalendar,true);
                                isMeetingAdded = true;
                            }
                        }
    
                    }
                    WeekStartDateStartTime = WeekStartDateStartTime.AddDays(7);//下次预定会议的星期天
                    WeekStartDateEndTime = WeekStartDateEndTime.AddDays(7);//下次的星期天会议结束数据
                }
            }
            if (isMeetingAdded)
            {
                if (model.participants != null && (model.participants.Length > 0))
                {
                    foreach (int item in model.participants)
                    {
                        var userEntity = this.UserService.GetUserDetail(item);
                        var notificationModel = new NotificationModel()
                        {
                            Target = userEntity.Email,
                            CreatedUserId = this.Member.Id,
                            //MessageType = NotificationType.PushMessage,
                            MessageType = NotificationType.Email,
                            BusinessType = BusinessType.BookMeetingRoomSuccessAnnounce,
                            Title = "Missionsky OA Notification",
                            MessagePrams = "test",
                            Scope = NotificationScope.User,
                            CreatedTime = DateTime.Now,
                            TargetUserIds = new List<int> { userEntity.Id }
                        };

                        if(loopCount>0)
                        {
                            string MeetingHappenedDate = null;
                            for (int i = 0; i < model.MeetingDayInWeek.Length; i++)
                            {
                                MeetingHappenedDate = MeetingHappenedDate + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(model.MeetingDayInWeek[i]) + ",";
                            }
                            notificationModel.MessageContent = string.Format("请参加，于每周{0}{1}召开,由{2}主持的{3}会议,会议持续循环召开{4}周", MeetingHappenedDate, model.StartDate.TimeOfDay, model.Host, model.Title, loopCount + 1);
                        }
                        else
                        {
                            string MeetingHappenedDate = null;
                            for (int i = 0; i < model.MeetingDayInWeek.Length; i++)
                            {
                                if (model.MeetingDayInWeek[i]>=DateTime.Now.DayOfWeek)
                                {
                                    MeetingHappenedDate = MeetingHappenedDate + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(model.MeetingDayInWeek[i]) + ",";
                                }                                
                            }
                            notificationModel.MessageContent = string.Format("请参加，于本周{0}{1}召开,由{2}主持的{3}会议", MeetingHappenedDate, model.StartDate.TimeOfDay, model.Host, model.Title, loopCount + 1);
                        }
                        
                        this._notificationService.Add(notificationModel, Global.IsProduction); //消息推送
                    }
                }

            }

            //新增会议预约
            // 3. Construct API Response
            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = isMeetingAdded
            };
            return response;
        }


        /// <summary>
        /// 根据会议ID,获取会议详情
        /// </summary>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ApiResponse<MeetingCalendarModel> GetMeetingDetailsById(int id)
        {
            if(id==0)
            {
                throw new ApiBadRequestException("无请求数据");
            }
            ApiResponse<MeetingCalendarModel> response = new ApiResponse<MeetingCalendarModel>()
            {
                Result = this.MeetingService.GetMeetingDetailsById(id)
            };
            return response;
        }

        /// <summary>
        /// 根据会议室ID,获取会议室信息
        /// </summary>
        /// <returns></returns>
        [Route("{id}/meetingRoom")]
        [HttpGet]
        public ApiResponse<MeetingRoomModel> GetMeetingRoomById(int id)
        {
            if (id == 0)
            {
                throw new ApiBadRequestException("无请求数据");
            }
            ApiResponse<MeetingRoomModel> response = new ApiResponse<MeetingRoomModel>()
            {
                Result = this.MeetingService.GetMeetingRoomById(id)
            };
            return response;
        }


        /// <summary>
        /// 根据会议室ID,获取某一段日期内的会议信息（拿取会议信息只精确到天，不精确到小时）
        /// </summary>
        /// <returns></returns>
        [Route("getMeetingListByDateTime")]
        [HttpPost]
        public ApiListResponse<MeetingDateGroupModel> GetMeetingListByDateTime(MeetingSearchModel model)
        {
            if(model==null)
            {
                throw new ApiBadRequestException("无效请求数据");
            }
            if(model.MeetingRoomId==0)
            {
                throw new ApiBadRequestException("会议室ID不能为空");
            }
            if(model.StartDate>model.EndDate)
            {
                throw new ApiBadRequestException("开始日期必须小于结束日期");
            }
            ApiListResponse<MeetingDateGroupModel> response = new ApiListResponse<MeetingDateGroupModel>()
            {
                Result = this.MeetingService.GetMeetingListByDateTime(model)
            };

            return response;
        }

        /// <summary>
        /// 根据用户ID,获取某一段日期内的会议信息,如果不传用户ID默认返回当前用户预约会议信息（拿取会议信息只精确到天，不精确到小时）
        /// </summary>
        /// <returns></returns>
        [Route("getUserReserveMeetingHistory")]
        [HttpPost]
        public ApiListResponse<MeetingCalendarModel> GetUserReserveMeetingHistory(MeetingSearchModel model)
        {
            if (model == null)
            {
                throw new ApiBadRequestException("无效请求数据");
            }
            if (model.UserId == 0)
            {
                model.UserId = this.Member.Id;   
            }
            if (model.StartDate > model.EndDate)
            {
                throw new ApiBadRequestException("开始日期必须小于结束日期");
            }
            ApiListResponse<MeetingCalendarModel> response = new ApiListResponse<MeetingCalendarModel>()
            {
                Result = this.MeetingService.GetUserReserveMeetingHistory(model)
            };

            return response;
        }

        /// <summary>
        /// 更新会议记录
        /// </summary>
        /// <returns></returns>
        [Route("{id}/update")]
        [HttpPut]
        public ApiResponse<bool> updateMeeting(int id, ReserveMeetingModel model)
        {
            if (id<0&&model == null)
            {
                throw new ApiBadRequestException("无效请求数据");
            }
            if (model.StartDate > model.EndDate)
            {
                throw new ApiBadRequestException("开始日期必须小于结束日期");
            }
            if (model.MeetingRoomId == 0)
            {
                throw new ApiBadRequestException("会议室不存在");
            }
            if (model.StartDate.Date != model.EndDate.Date)
            {
                throw new ApiBadRequestException("开始日期与结束日期需在同一天");
            }
            if (model.StartDate < DateTime.Now)
            {
                throw new ApiBadRequestException("开始时间必须大于现在时间");
            }
            if (MeetingService.isMeetingRoomAvailiable(model.MeetingRoomId, model.StartDate, model.EndDate) == false)
            {
                throw new ApiBadRequestException("会议室已经占用");
            }

            // 2. 构造会议预约记录
            var meetingCalendar = new MeetingCalendarModel()
            {
                ApplyUserId = this.Member.Id,
                MeetingRoomId = model.MeetingRoomId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MeetingContext = model.MeetingContext,
                StartTime = model.StartDate.TimeOfDay,
                EndTime = model.EndDate.TimeOfDay,
                CreatedTime = DateTime.Now
            };

            //2.1 构造会议必须参与人员模型
            List<MeetingParticipantModel> meetingParticipants = new List<MeetingParticipantModel>();
            if (model.participants != null && (model.participants.Length > 0))
            {
                foreach (int item in model.participants)
                {
                    var meetingParticipantModel = new MeetingParticipantModel()
                    {
                        UserId = item,
                        IsOptional = "No"
                    };
                    meetingParticipants.Add(meetingParticipantModel);
                }
            }

            //2.1 构造会议可选参与人员模型
            if (model.optionalParticipants != null && (model.optionalParticipants.Length > 0))
            {
                foreach (int item in model.optionalParticipants)
                {
                    var meetingParticipantModel = new MeetingParticipantModel()
                    {
                        UserId = item,
                        IsOptional = "Yes"
                    };
                    meetingParticipants.Add(meetingParticipantModel);
                }
            }
            meetingCalendar.MeetingParticipants = meetingParticipants;
            ApiResponse<bool> response = new ApiResponse<bool> ()
            {
                Result = this.MeetingService.UpdateReserveMeetingHistory(id, meetingCalendar, this.Member.Id)
            };

            return response;
        }

         /// <summary>
        /// 取消会议,会议已经开始status变为0,会议未开始直接从DB删除此数据
        /// </summary>
        /// <returns></returns>
        [Route("{id}/cancel")]
        [HttpPut]
        public ApiResponse<bool> cancelMeeting(int id)
        {
            if (id<=0)
            {
                throw new ApiBadRequestException("无效请求数据");
            }

            ApiResponse<bool> response = new ApiResponse<bool> ()
            {
                Result = this.MeetingService.cancelMeeting(id,this.Member.Id)
            };

            return response;
        }
    }
}
