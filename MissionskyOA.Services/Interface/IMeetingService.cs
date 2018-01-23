using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// Meeting Interface
    /// </summary>
    public partial interface IMeetingService
    {
        /// <summary>
        /// 获取会议室信息
        /// </summary>
        /// <returns>获取会议室信息</returns>
        List<MeetingRoomModel> GetMeetingRoomList();

        /// <summary>
        /// 分页获取会议室信息
        /// </summary>
        /// <returns>获取会议室信息</returns>
        ListResult<MeetingRoomModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 预定会议
        /// <param name="isAddMeetingByWeek">是否为批量添加会议</param>
        /// </summary>
        /// <returns>预定会议信息</returns>
        MeetingCalendarModel AddMeeting(MeetingCalendarModel meeting, bool isAddMeetingByWeek);

        /// <summary>
        /// 更新会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        bool UpdateMeetingRoom(MeetingRoomModel model);

        /// <summary>
        /// 添加会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        bool AddMeetingRoom(MeetingRoomModel model);

        /// <summary>
        /// 删除会议室信息
        /// </summary>
        /// <returns>是否更新成功</returns>
        bool Remove(int id);

        /// <summary>
        /// 根据ID查询会议具体信息
        /// </summary>
        /// <returns>会议具体信息</returns>
        MeetingCalendarModel GetMeetingDetailsById(int id);

        /// <summary>
        /// 查询具体会议信息
        /// </summary>
        /// <returns>会议信息</returns>
        MeetingCalendarModel GetMeetingDetailsById(MissionskyOAEntities dbcontext, int id);

         /// <summary>
        /// 根据ID查询会议具体信息
        /// </summary>
        /// <returns>会议具体信息</returns>
        MeetingRoomModel GetMeetingRoomById(int id);

        /// <summary>
        /// 会议室是否占用
        /// </summary>
        /// <returns>会议室是否占用</returns>
        bool isMeetingRoomAvailiable(int meetingRoomID, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 根据会议室ID,日期时间返回会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        List<MeetingDateGroupModel> GetMeetingListByDateTime(MeetingSearchModel model);

         /// <summary>
        /// 根据用户ID,日期时间返回会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        List<MeetingCalendarModel> GetUserReserveMeetingHistory(MeetingSearchModel model);
        
        /// <summary>
        /// 更新会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        bool UpdateReserveMeetingHistory(int id, MeetingCalendarModel model, int userID);

        /// <summary>
        /// 更新会议纪要
        /// </summary>
        /// <returns>更新会议纪要</returns>
        bool UpdateMeetingSummary(MeetingCalendarModel model);

        /// <summary>
        /// 取消会议预定
        /// </summary>
        /// <returns>取消成功：true 失败：false</returns>
        bool cancelMeeting(int id,int userID);

        /// <summary>
        /// 根据会议室ID,查询会议预定记录
        /// </summary>
        /// <returns>会议预定记录</returns>
        List<MeetingCalendarModel> GetMeetingListByMeetingRoomId(int id);

    }
}
