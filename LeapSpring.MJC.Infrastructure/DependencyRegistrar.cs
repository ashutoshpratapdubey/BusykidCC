using Autofac;
using Autofac.Integration.WebApi;
using LeapSpring.MJC.BusinessLogic.Services.Account;
using LeapSpring.MJC.BusinessLogic.Services.Member;
using LeapSpring.MJC.BusinessLogic.Services.Security;
using LeapSpring.MJC.Data.Repository;
using System.Web.Http;
using LeapSpring.MJC.BusinessLogic.Services.ChoreService;
using LeapSpring.MJC.BusinessLogic.Services.PhoneConfirmation;
using LeapSpring.MJC.BusinessLogic.Services.Cloud;
using LeapSpring.MJC.BusinessLogic.Services.Settings;
using LeapSpring.MJC.BusinessLogic.Services.RecurringChore;
using Quartz;
using Quartz.Impl;
using System.Reflection;
using LeapSpring.MJC.BusinessLogic.Services.Banking;
using LeapSpring.MJC.BusinessLogic.Services.Earnings;
using LeapSpring.MJC.BusinessLogic.Services.Sms;
using LeapSpring.MJC.BusinessLogic.Services.Invitation;
using LeapSpring.MJC.BusinessLogic.Services.Spend;
using LeapSpring.MJC.BusinessLogic.Services.Notification;
using LeapSpring.MJC.BusinessLogic.Services.Charities;
using LeapSpring.MJC.BusinessLogic.Services.Save;
using LeapSpring.MJC.BusinessLogic.Services.Emails;
using LeapSpring.MJC.BusinessLogic.Services.SubscriptionService;

namespace LeapSpring.MJC.Infrastructure
{
    public class DependencyRegistrar
    {
        public static void Register(ContainerBuilder builder, HttpConfiguration config)
        {
            //builder.RegisterType<MJCDbContext>().InstancePerRequest();
            builder.RegisterType<Repository>().As<IRepository>().InstancePerRequest();

            #region Services

            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();
            builder.RegisterType<CryptoService>().As<ICryptoService>().InstancePerRequest();
            builder.RegisterType<FamilyService>().As<IFamilyService>().InstancePerRequest();
            builder.RegisterType<SignUpProgressService>().As<ISignUpProgressService>().InstancePerRequest();
            builder.RegisterType<PhoneConfirmationService>().As<IPhoneConfirmationService>().InstancePerRequest();
            builder.RegisterType<ChoreService>().As<IChoreService>().InstancePerRequest();
            builder.RegisterType<StorageService>().As<IStorageService>().InstancePerRequest();
            builder.RegisterType<AppSettingsService>().As<IAppSettingsService>().InstancePerRequest();
            builder.RegisterType<RecurringChoreService>().As<IRecurringChoreService>().InstancePerRequest();
            builder.RegisterType<BankAuthorizeService>().As<IBankAuthorizeService>().InstancePerRequest();
            builder.RegisterType<AllocationSettingsService>().As<IAllocationSettingsService>().InstancePerRequest();
            builder.RegisterType<EarningsService>().As<IEarningsService>().InstancePerRequest();
            builder.RegisterType<CurrentUserService>().As<ICurrentUserService>().InstancePerRequest();
            builder.RegisterType<TransactionService>().As<ITransactionService>().InstancePerRequest();
            builder.RegisterType<TransactionHistoryService>().As<ITransactionHistoryService>().InstancePerRequest();
            builder.RegisterType<TextMessageService>().As<ITextMessageService>().InstancePerRequest();
            builder.RegisterType<SmsBotService>().As<ISmsBotService>().InstancePerRequest();
            builder.RegisterType<InvitationService>().As<IInvitationService>().InstancePerRequest();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerRequest();
            builder.RegisterType<SpendService>().As<ISpendService>().InstancePerRequest();
            builder.RegisterType<GyftService>().As<IGyftService>().InstancePerRequest();
            builder.RegisterType<CharityService>().As<ICharityService>().InstancePerRequest();
            builder.RegisterType<StockPileService>().As<IStockPileService>().InstancePerRequest();
            builder.RegisterType<SaveService>().As<ISaveService>().InstancePerRequest();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerRequest();
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>().InstancePerRequest();
            builder.RegisterType<EmailHistoryService>().As<IEmailHistoryService>().InstancePerRequest();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().InstancePerRequest();
            builder.RegisterType<SMSApprovalService>().As<ISMSApprovalService>().InstancePerRequest();
            builder.RegisterType<SMSApprovalHistory>().As<ISMSApprovalHistory>().InstancePerRequest();
            builder.RegisterType<CoreProService>().As<ICoreProService>().InstancePerRequest();
            builder.RegisterType<PlaidService>().As<IPlaidService>().InstancePerRequest();
            builder.RegisterType<BankService>().As<IBankService>().InstancePerRequest();
            builder.RegisterType<CoreProMessageService>().As<ICoreProMessageService>().InstancePerRequest();

            #endregion

            // Scheduler
            builder.Register(x => new StdSchedulerFactory().GetScheduler()).As<IScheduler>();

            // Scheduler jobs
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            JobScheduler.Start(container);
        }
    }
}
