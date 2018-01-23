using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MissionskyOA.Core.Security;
using MissionskyOA.Services;

namespace MissionskyOA.Portal
{
    public class StartUp
    {
        public static void Start()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            RegisterTypes(containerBuilder);
            var container = containerBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }



        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<AskLeaveService>().As<IAskLeaveService>();
            builder.RegisterType<OvertimeService>().As<IOvertimeService>();
            builder.RegisterType<UserRoleService>().As<IUserRoleService>();
            builder.RegisterType<DepartmentService>().As<IDepartmentService>();
            builder.RegisterType<AcceptProxyService>().As<IAcceptProxyService>();
            builder.RegisterType<AuditMessageService>().As<IAuditMessageService>();            
            builder.RegisterType<ProjectService>().As<IProjectService>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<MeetingService>().As<IMeetingService>();
            builder.RegisterType<BookService>().As<IBookService>(); 
            builder.RegisterType<AttendanceSummaryService>().As<IAttendanceSummaryService>();
            builder.RegisterType<AnnouncementService>().As<IAnnouncementService>();
            builder.RegisterType<MD5Cryptology>().As<ICryptology>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<WorkflowService>().As<IWorkflowService>();
            builder.RegisterType<AssetAttributeService>().As<IAssetAttributeService>();
            builder.RegisterType<AssetTypeService>().As<IAssetTypeService>();
            builder.RegisterType<AssetService>().As<IAssetService>();
            builder.RegisterType<AssetTransactionService>().As<IAssetTransactionService>();
            builder.RegisterType<AssetInventoryService>().As<IAssetInventoryService>();
            builder.RegisterType<AuditMessageService>().As<IAuditMessageService>();
            builder.RegisterType<ScheduledTaskService>().As<IScheduledTaskService>();
            builder.RegisterType<ReportService>().As<IReportService>();
            builder.RegisterType<ExpenseService>().As<IExpenseService>();
            builder.RegisterType<WorkTaskService>().As<IWorkTaskService>();
            builder.RegisterFilterProvider();
        }
    }
}