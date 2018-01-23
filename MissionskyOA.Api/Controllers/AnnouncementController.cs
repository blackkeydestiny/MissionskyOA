using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using System.Configuration;


namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/announcement")]
    public class AnnouncementController : BaseController
    {
        private IAnnouncementService AnnouncementService { get; set; }
        private IRoleService RoleService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="announcementService">通告管理服务</param>
        public AnnouncementController(IAnnouncementService announcementService, IRoleService roleService)
        {
            this.AnnouncementService = announcementService;
            this.RoleService = roleService;
        }

        /// <summary>
        /// 新增通告,员工只能申请员工消息和活动消息类型的通告
        /// </summary>
        /// <returns>通告信息</returns>
        [Route("add")]
        [HttpPost]
        public ApiResponse<AnnouncementModel> AddAnnouncement(ApplyAnnoucementModel model)
        {
            if (model == null)
            {
                throw new ApiBadRequestException("无效参数");
            }
            if (AnnouncementService.isUserHaveAuthority2Announcement(model.Type, this.Member.Role))
            {
                throw new ApiBadRequestException("普通员工只能申请员工消息和活动消息类型的通告");
            }
            var annocumentModel = new AnnouncementModel()
            {
                Type = model.Type,
                Title = model.Title,
                Content = model.Content,
                ApplyUserId = model.ApplyUserId!=0?model.ApplyUserId:this.Member.Id,
                EffectiveDays = model.EffectiveDays,
                CreateUserId = this.Member.Id,
                CreatedTime =DateTime.Now,
                Status = AnnouncementStatus.Apply
            };
            //如果是行政，通告状态直接为同意发布状态
            RoleModel role = RoleService.SearchRole(this.Member.Role);
            if (role.RoleName.Contains("行政"))
            {
                annocumentModel.Status = AnnouncementStatus.AllowPublish;
            }
            // 3. Construct API Response
            ApiResponse<AnnouncementModel> response = new ApiResponse<AnnouncementModel>()
            {
                Result = AnnouncementService.AddAnnouncement(annocumentModel)
            };

            return response;
        }

        /// <summary>
        /// 获取公告
        /// </summary>
        /// <returns>获取公告</returns>
        [Route("get")]
        [HttpGet]
        public ApiPagingListResponse<AnnouncementModel> GetAnnocument(int pageIndex = 0, int pageSize = 10)
        {
            // 3. 获取所有请假单信息
            var query = this.AnnouncementService.AnnouncementList(pageIndex, pageSize);

            var result = new PaginationModel<AnnouncementModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<AnnouncementModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 获取待处理的公告申请,行政专员才能取得数据
        /// </summary>
        /// <returns>审批任务列表</returns>
        [Route("pendinglist")]
        [HttpGet]
        public ApiListResponse<AnnouncementModel> GetPendingAnnocument()
        {
            if (this.Member == null || this.Member.Id < 1)
            {
                throw new Exception("无效的User");
            }
            RoleModel role = RoleService.SearchRole(this.Member.Role);
            if (!role.RoleName.Contains("行政"))
            {
                throw new Exception("角色为行政专员的用户才有权限操作该api");
            }
            ApiListResponse<AnnouncementModel> response = new ApiListResponse<AnnouncementModel>()
            {
                Result = this.AnnouncementService.GetPendingAnnocument()
            };

            return response;
        }

        /// <summary>
        /// 审批通告
        /// </summary>
        /// <param name="model">审批通告</param>
        /// <returns>是否审批成功</returns>
        [Route("audit")]
        [HttpPut]
        public ApiResponse<bool> Audit(AuditAnnouncementModel model)
        {
            if (model == null || model.announcementID < 1)
            {
                throw new Exception("无效的参数");
            }
            if(model.auditStatus==false&&String.IsNullOrEmpty(model.AuditReason))
            {
                throw new Exception("请输入拒绝理由");
            }
            RoleModel role = RoleService.SearchRole(this.Member.Role);
            if (!role.RoleName.Contains("行政"))
            {
                throw new Exception("没有审批通告权限");
            }
            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = this.AnnouncementService.Audit(model, this.Member.Id,this.Member.EnglishName)
            };

            return response;
        }


    }
}
