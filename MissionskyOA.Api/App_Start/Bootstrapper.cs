using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Autofac;
using Autofac.Integration.WebApi;
using System.Web.Mvc;
using System.Web.Http;
using Autofac.Integration.Mvc;
using System.Reflection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using MissionskyOA.Services;
using MissionskyOA.Core.Security;
using MissionskyOA.Services.Interface;

namespace MissionskyOA.Api
{
    /// <summary>
    /// 引导程序
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            // 1、创建一个容器
            ContainerBuilder containerBuilder = new ContainerBuilder();
            
            // 2、注册
            RegisterTypes(containerBuilder);

            // 3、将容器装入到微软默认的依赖注入容器中
            var container = containerBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // 将默认容器放入到全局配置中
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //
            JsonFormmatter();
        }

        /// <summary>
        /// 将需要的类注册到容器中
        /// 参考 ： https://www.cnblogs.com/struggle999/p/6986903.html
        /// Autofac创建类的生命周期:
        /// 1、InstancePerDependency（默认）
        /// 2、InstancePerLifetimeScope
        /// 3、InstancePerMatchingLifetimeScope
        /// 4、InstancePerOwned
        /// 5、SingleInstance
        /// 6、InstancePerHttpRequest  （新版autofac建议使用InstancePerRequest）
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterTypes(ContainerBuilder builder)
        {
            // 1、注册当前程序集中所有的Controller
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // 2、注册当前程序集中所有的的API Controller
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //注册了当前程序集内的所有类。  一次性注入
            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();

            // 3、RegisterType方式，去注册需要的服务类
            // RegisterType 方式指定具体类
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<UserTokenService>().As<IUserTokenService>();
			builder.RegisterType<UserTokenService>().As<IUserTokenService>();
            builder.RegisterType<OvertimeService>().As<IOvertimeService>();
            builder.RegisterType<AskLeaveService>().As<IAskLeaveService>();
            builder.RegisterType<AvatarService>().As<IAvatarService>();
            builder.RegisterType<AttendanceSummaryService>().As<IAttendanceSummaryService>();
            builder.RegisterType<AuditMessageService>().As<IAuditMessageService>();
            builder.RegisterType<AcceptProxyService>().As<IAcceptProxyService>();
            builder.RegisterType<MeetingService>().As<IMeetingService>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<MD5Cryptology>().As<ICryptology>();
            builder.RegisterType<AuditMessageService>().As<IAuditMessageService>();
            builder.RegisterType<ScheduledTaskService>().As<IScheduledTaskService>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<AnnouncementService>().As<IAnnouncementService>(); 
            builder.RegisterType<WorkflowProcessService>().As<IWorkflowProcessService>();
            builder.RegisterType<WorkflowService>().As<IWorkflowService>();
            builder.RegisterType<BookService>().As<IBookService>();
            builder.RegisterType<BookBorrowService>().As<IBookBorrowService>();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>();
            builder.RegisterType<AssetAttributeService>().As<IAssetAttributeService>();
            builder.RegisterType<AssetTypeService>().As<IAssetTypeService>();
            builder.RegisterType<AssetService>().As<IAssetService>();
            builder.RegisterType<AssetTransactionService>().As<IAssetTransactionService>();
            builder.RegisterType<AssetInventoryService>().As<IAssetInventoryService>();
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<ReportService>().As<IReportService>();
            builder.RegisterType<ExpenseService>().As<IExpenseService>();
            builder.RegisterType<WorkTaskService>().As<IWorkTaskService>();
            builder.RegisterType<WorkTaskCommentService>().As<IWorkTaskCommentService>();
            builder.RegisterType<ProjectService>().As<IProjectService>();
            builder.RegisterType<FeedbackService>().As<IFeedbackService>();

            builder.RegisterFilterProvider();
        }



        private static void JsonFormmatter()
        {
            // 添加一个转换器 IsoDateTimeConverter，其用于日期数据的序列化和反序列化
            var dateTimeConverter = new IsoDateTimeConverter();
            dateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.DefaultValueHandling = DefaultValueHandling.Include;        // 包含默认值           
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;               // 空值忽略
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(dateTimeConverter);

            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings = serializerSettings;
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain")); // 默认将POST Body作为JSON字符串进行解析

            GlobalConfiguration.Configuration.Formatters.Insert(0, jsonFormatter);
        }
    }
}