using Autofac;
using LeapSpring.MJC.Infrastructure.Jobs;
using Quartz;
using System;

namespace LeapSpring.MJC.Infrastructure
{
    public static class JobScheduler
    {
        // Remove this method
        public static bool IsStarted = false;
        public static bool IsSent = false;
        public static bool IsPayDayStatusSent = false;
        public static bool IsRenewSubscription = false;
        public static bool IsRenewSubscriptionNotification = false;
        public static bool IsRemindChorePayment = false;
        static IScheduler makePaymentScheduler;
        static IScheduler sendPayDayStatusScheduler;
        static IScheduler renewSubscriptionScheduler;
        static IScheduler renewSubscriptionNotificationScheduler;
        static IScheduler remindChorePaymentScheduler;
        public static IContainer Container { get; set; }

        #region Todo Remove methods

        // Remove this method
        public static void Start()
        {
            if (!IsStarted)
                return;

            if (makePaymentScheduler == null)
                StartMakePaymentJob(Container);

            if (!makePaymentScheduler.IsStarted)
            {
                makePaymentScheduler.Start();
                IsStarted = false;
                return;
            }

            if (makePaymentScheduler.IsShutdown)
            {
                makePaymentScheduler = null;
                StartMakePaymentJob(Container);
                makePaymentScheduler.Start();
                IsStarted = false;
            }

            makePaymentScheduler.Clear();
            makePaymentScheduler.Shutdown();
            makePaymentScheduler = null;
            StartMakePaymentJob(Container);
            makePaymentScheduler.Start();
            IsStarted = false;
        }

        // Remove this method
        public static void StartMakePaymentJob(IContainer container)
        {
            Container = container;
            makePaymentScheduler = container.Resolve<IScheduler>();
            makePaymentScheduler.JobFactory = new AutofacJobFactory(container);

            // create jobs
            var makeWeeklyPaymentJob = JobBuilder.Create<MakeWeeklyPayment>()
                    .WithIdentity("testWeeklyPaymentJob", "groupTest")
                    .Build();

            //create triggers
            var makeWeeklyPaymentTrigger = TriggerBuilder.Create()
               .WithIdentity("testWeeklyPaymentTrigger", "groupTest")
               .StartNow()
               .ForJob("testWeeklyPaymentJob", "groupTest")
               .Build();

            // Schedule the job using the jobs and triggers 
            makePaymentScheduler.ScheduleJob(makeWeeklyPaymentJob, makeWeeklyPaymentTrigger);
        }

        // Remove this method - sendPayDayStatusScheduler
        public static void StartSendPayDayStatusJob()
        {
            if (!IsPayDayStatusSent)
                return;

            if (sendPayDayStatusScheduler == null)
                SendPayDayStatusJob(Container);

            if (!sendPayDayStatusScheduler.IsStarted)
            {
                sendPayDayStatusScheduler.Start();
                IsPayDayStatusSent = false;
                return;
            }

            if (sendPayDayStatusScheduler.IsShutdown)
            {
                sendPayDayStatusScheduler = null;
                SendPayDayStatusJob(Container);
                sendPayDayStatusScheduler.Start();
                IsPayDayStatusSent = false;
            }

            sendPayDayStatusScheduler.Clear();
            sendPayDayStatusScheduler.Shutdown();
            sendPayDayStatusScheduler = null;
            SendPayDayStatusJob(Container);
            sendPayDayStatusScheduler.Start();
            IsPayDayStatusSent = false;
        }

        // Remove this method
        public static void SendPayDayStatusJob(IContainer container)
        {
            Container = container;
            sendPayDayStatusScheduler = container.Resolve<IScheduler>();
            sendPayDayStatusScheduler.JobFactory = new AutofacJobFactory(container);

            // create jobs
            var sendPayDayStatusJob = JobBuilder.Create<SendWeeklyPaydayStatus>()
                    .WithIdentity("sendPayDayStatusJob", "groupTestOne")
                    .Build();

            //create triggers
            var sendPayDayStatusTrigger = TriggerBuilder.Create()
               .WithIdentity("sendPayDayStatusTrigger", "groupTestOne")
               .StartNow()
               .ForJob("sendPayDayStatusJob", "groupTestOne")
               .Build();

            // Schedule the job using the jobs and triggers 
            sendPayDayStatusScheduler.ScheduleJob(sendPayDayStatusJob, sendPayDayStatusTrigger);
        }

        // Remove this method - renewSubscriptionScheduler
        public static void StartRenewSubscriptonJob()
        {
            if (!IsRenewSubscription)
                return;

            if (renewSubscriptionScheduler == null)
                RenewSubscriptonJob(Container);

            if (!renewSubscriptionScheduler.IsStarted)
            {
                renewSubscriptionScheduler.Start();
                IsRenewSubscription = false;
                return;
            }

            if (renewSubscriptionScheduler.IsShutdown)
            {
                renewSubscriptionScheduler = null;
                RenewSubscriptonJob(Container);
                renewSubscriptionScheduler.Start();
                IsRenewSubscription = false;
            }

            renewSubscriptionScheduler.Clear();
            renewSubscriptionScheduler.Shutdown();
            renewSubscriptionScheduler = null;
            RenewSubscriptonJob(Container);
            renewSubscriptionScheduler.Start();
            IsRenewSubscription = false;
        }

        // Remove this method
        public static void RenewSubscriptonJob(IContainer container)
        {
            Container = container;
            renewSubscriptionScheduler = container.Resolve<IScheduler>();
            renewSubscriptionScheduler.JobFactory = new AutofacJobFactory(container);

            // create jobs
            var renewSubscriptionJob = JobBuilder.Create<RenewSubscriptionJob>()
                    .WithIdentity("renewSubscriptionJob", "groupTesttwo")
                    .Build();

            //create triggers
            var renewSubscriptionTrigger = TriggerBuilder.Create()
               .WithIdentity("renewSubscriptionTrigger", "groupTesttwo")
               .StartNow()
               .ForJob("renewSubscriptionJob", "groupTesttwo")
               .Build();

            // Schedule the job using the jobs and triggers 
            renewSubscriptionScheduler.ScheduleJob(renewSubscriptionJob, renewSubscriptionTrigger);
        }

        // Remove this method - renewSubscriptionNotificationScheduler
        public static void StartSubscriptonRenwalNotificationJob()
        {
            if (!IsRenewSubscriptionNotification)
                return;

            if (renewSubscriptionNotificationScheduler == null)
                SubscriptonRenwalNotificationJob(Container);

            if (!renewSubscriptionNotificationScheduler.IsStarted)
            {
                renewSubscriptionNotificationScheduler.Start();
                IsRenewSubscriptionNotification = false;
                return;
            }

            if (renewSubscriptionNotificationScheduler.IsShutdown)
            {
                renewSubscriptionNotificationScheduler = null;
                SubscriptonRenwalNotificationJob(Container);
                renewSubscriptionNotificationScheduler.Start();
                IsRenewSubscriptionNotification = false;
            }

            renewSubscriptionNotificationScheduler.Clear();
            renewSubscriptionNotificationScheduler.Shutdown();
            renewSubscriptionNotificationScheduler = null;
            SubscriptonRenwalNotificationJob(Container);
            renewSubscriptionNotificationScheduler.Start();
            IsRenewSubscriptionNotification = false;
        }

        // Remove this method
        public static void SubscriptonRenwalNotificationJob(IContainer container)
        {
            Container = container;
            renewSubscriptionNotificationScheduler = container.Resolve<IScheduler>();
            renewSubscriptionNotificationScheduler.JobFactory = new AutofacJobFactory(container);

            // create jobs
            var renewSubscriptionNotificationJob = JobBuilder.Create<NotifySubscriptionRenewalJob>()
                    .WithIdentity("renewSubscriptionNotificationJob", "subscriptionRenew")
                    .Build();

            //create triggers
            var renewSubscriptionNotificationTrigger = TriggerBuilder.Create()
               .WithIdentity("renewSubscriptionNotificationTrigger", "subscriptionRenew")
               .StartNow()
               .ForJob("renewSubscriptionNotificationJob", "subscriptionRenew")
               .Build();

            // Schedule the job using the jobs and triggers 
            renewSubscriptionNotificationScheduler.ScheduleJob(renewSubscriptionNotificationJob, renewSubscriptionNotificationTrigger);
        }


        // Remove this method - remindChorePaymentScheduler
        public static void StartRemindChorePaymentJob()
        {
            if (!IsRemindChorePayment)
                return;

            if (remindChorePaymentScheduler == null)
                RemindChorePaymentJob(Container);

            if (!remindChorePaymentScheduler.IsStarted)
            {
                remindChorePaymentScheduler.Start();
                IsRemindChorePayment = false;
                return;
            }

            if (remindChorePaymentScheduler.IsShutdown)
            {
                remindChorePaymentScheduler = null;
                RemindChorePaymentJob(Container);
                remindChorePaymentScheduler.Start();
                IsRemindChorePayment = false;
            }

            remindChorePaymentScheduler.Clear();
            remindChorePaymentScheduler.Shutdown();
            remindChorePaymentScheduler = null;
            RemindChorePaymentJob(Container);
            remindChorePaymentScheduler.Start();
            IsRemindChorePayment = false;
        }

        // Remove this method
        public static void RemindChorePaymentJob(IContainer container)
        {
            Container = container;
            remindChorePaymentScheduler = container.Resolve<IScheduler>();
            remindChorePaymentScheduler.JobFactory = new AutofacJobFactory(container);

            // create jobs
            var remindChorePaymentJob = JobBuilder.Create<RemindChorePaymentJob>()
                    .WithIdentity("remindChorePaymentJob", "remindChorePayment")
                    .Build();

            //create triggers
            var remindChorePaymentTrigger = TriggerBuilder.Create()
               .WithIdentity("remindChorePaymentTrigger", "remindChorePayment")
               .StartNow()
               .ForJob("remindChorePaymentJob", "remindChorePayment")
               .Build();

            // Schedule the job using the jobs and triggers 
            remindChorePaymentScheduler.ScheduleJob(remindChorePaymentJob, remindChorePaymentTrigger);
        }


        #endregion

        public static void Start(IContainer container)
        {
            Container = container;
            var scheduler = container.Resolve<IScheduler>();
            scheduler.JobFactory = new AutofacJobFactory(container);
            scheduler.Start();

            // create jobs // comment as per client comment
            var dailyChoresJob = JobBuilder.Create<CreateDailyChores>()
                    .WithIdentity("dailyChoresJob", "createChore")
                    .Build();

            var weeklyChoresJob = JobBuilder.Create<CreateWeeklyChores>()
                    .WithIdentity("weeklyChoresJob", "createChore")
                    .Build();

            var makeWeeklyPaymentJob = JobBuilder.Create<MakeWeeklyPayment>()
                    .WithIdentity("makeWeeklyPaymentJob", "createChore")
                    .Build();

            var sendWeeklyPaydayStatusJob = JobBuilder.Create<SendWeeklyPaydayStatus>()
                    .WithIdentity("sendWeeklyPaydayStatusJob", "sendStatus")
                    .Build();

            var notifyIncompleteMemberEnrollmentJob = JobBuilder.Create<NotifyIncompleteMemberEnrollmentJob>()
                .WithIdentity("notifyIncompleteMemberEnrollmentJob", "incompleteNewMemberEnrollment")
                .Build();

            var notifyParentHasNotLoggedInJob = JobBuilder.Create<NotifyParentHasNotLoggedInJob>()
                .WithIdentity("notifyParentHasNotLoggedInJob", "parentHasNotLoggedIn")
                .Build();

            var renewSubscriptionJob = JobBuilder.Create<RenewSubscriptionJob>()
                    .WithIdentity("renewSubscriptionJob", "subscription")
                    .Build();

            var notifyChildHasNotLoggedInJob = JobBuilder.Create<NotifyChildHasNotLoggedInJob>()
                .WithIdentity("notifyChildHasNotLoggedInJob", "childHasNotLoggedIn")
                .Build();

            var notifyNoChoreCompletedJob = JobBuilder.Create<NotifyNoChoreCompletedJob>()
                .WithIdentity("notifyNoChoreCompletedJob", "noChoreCompleted")
                .Build();

            var notifyContinuousChildActivityJob = JobBuilder.Create<NotifyContinuousChildActivityJob>()
                .WithIdentity("notifyContinuousChildActivityJob", "continuousChildActivity")
                .Build();

            var notifySubscriptionRenewalJob = JobBuilder.Create<NotifySubscriptionRenewalJob>()
                .WithIdentity("notifySubscriptionRenewalJob", "subscription")
                .Build();

            var remindSMSApprovalJob = JobBuilder.Create<RemindSMSApprovalJob>()
               .WithIdentity("remindSMSApprovalJob", "reminder")
               .Build();

            var cancelNotRespondedSMSJob = JobBuilder.Create<CancelNotRespondedSMSJob>()
               .WithIdentity("cancelNotRespondedSMSJob", "reminder")
               .Build();

            var remindChorePaymentApprovalJob = JobBuilder.Create<RemindChorePaymentJob>()
               .WithIdentity("remindChorePaymentApprovalJob", "reminder")
               .Build();

            // Azure Service Bus Job
            var coreproMessageJob = JobBuilder.Create<CoreProMessageJob>()
                    .WithIdentity("coreproMessageJob", "notification")
                    .Build();

            var cancelPandingPaydayRequest = JobBuilder.Create<CancelPandingPaydayRequest>()
                  .WithIdentity("cancelPandingPaydayRequest", "cancelChore")
                  .Build();
            

            //create triggers  // comment as per client comment
            var dailyChoresTrigger = TriggerBuilder.Create()
               .WithIdentity("dailyChoresTrigger", "createChore")
               .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(1, 0, DayOfWeek.Sunday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
               .ForJob("dailyChoresJob", "createChore")
               .Build();

            var weeklyChoresTrigger = TriggerBuilder.Create()
               .WithIdentity("weeklyChoresTrigger", "createChore")
               .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(1, 30, DayOfWeek.Sunday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
               .ForJob("weeklyChoresJob", "createChore")
               .Build();


            var notifyIncompleteNewMemberEnrollmentTrigger = TriggerBuilder.Create()
                .WithIdentity("notifyIncompleteNewMemberEnrollmentTrigger", "incompleteNewMemberEnrollment")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 0).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifyIncompleteMemberEnrollmentJob", "incompleteNewMemberEnrollment")
                .Build();

            var notifyParentHasNotLoggedInTrigger = TriggerBuilder.Create()
                .WithIdentity("notifyParentHasNotLoggedInTrigger", "parentHasNotLoggedIn")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 30).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifyParentHasNotLoggedInJob", "parentHasNotLoggedIn")
                .Build();

            var notifyChildHasNotLoggedInTrigger = TriggerBuilder.Create()
                .WithIdentity("notifyChildHasNotLoggedInTrigger", "childHasNotLoggedIn")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(3, 0).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifyChildHasNotLoggedInJob", "childHasNotLoggedIn")
                .Build();

            var notifyNoChoreCompletedTrigger = TriggerBuilder.Create()
                .WithIdentity("notifyNoChoreCompletedTrigger", "noChoreCompleted")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(3, 30).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifyNoChoreCompletedJob", "noChoreCompleted")
                .Build();

            var notifyContinuousChildActivityTrigger = TriggerBuilder.Create()
                .WithIdentity("notifyContinuousChildActivityTrigger", "continuousChildActivity")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 0).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifyContinuousChildActivityJob", "continuousChildActivity")
                .Build();

            var notifySubscriptionRenewalTrigger = TriggerBuilder.Create()
                .WithIdentity("notifySubscriptionRenewalTrigger", "subscription")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 30).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("notifySubscriptionRenewalJob", "subscription")
                .Build();

            // Send weekly payday notification trigger - at 10AM
            var sendWeeklyPaydayStatusTrigger = TriggerBuilder.Create()
               .WithIdentity("sendWeeklyPaydayStatusTrigger", "sendStatus")
               .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(10, 0, DayOfWeek.Thursday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")))
               .ForJob("sendWeeklyPaydayStatusJob", "sendStatus")
               .Build();

            // Remind chore payment notification trigger -at 12PM
            var remindChorePaymentApprovalTrigger = TriggerBuilder.Create()
                 .WithIdentity("remindChorePaymentApprovalTrigger", "reminder")
                 .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(12, 0, DayOfWeek.Thursday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")))
                 .ForJob("remindChorePaymentApprovalJob", "reminder")
                 .Build();

            // Now it's disapprove chore job after 24 hr of payday SMS  - at 10:00 AM Friday
            var makeWeeklyPaymentTrigger = TriggerBuilder.Create()
               .WithIdentity("makeWeeklyPaymentTrigger", "createChore")
               .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(10, 0, DayOfWeek.Friday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")))
               .ForJob("makeWeeklyPaymentJob", "createChore")
               .Build();

            // renewSubscription trigger - at 11PM
            var renewSubscriptionTrigger = TriggerBuilder.Create()
               .WithIdentity("renewSubscriptionTrigger", "subscription")
               .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(23, 0).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
               .ForJob("renewSubscriptionJob", "subscription")
               .Build();


            // SMS Approval Jobs - reminder and cancel approval
            var remindSMSApprovalTrigger = TriggerBuilder.Create()
                .WithIdentity("remindSMSApprovalTrigger", "reminder")
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0 0/1 * * ? *").InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("remindSMSApprovalJob", "reminder")
                .Build();

            var cancelNotRespondedSMSTrigger = TriggerBuilder.Create()
                .WithIdentity("cancelNotRespondedSMSTrigger", "reminder")
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0 0/1 * * ? *").InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time")))
                .ForJob("cancelNotRespondedSMSJob", "reminder")
                .Build();

            // Azure Service Bus Job Trigger
            var coreproMessageTrigger = TriggerBuilder.Create()
                .WithIdentity("coreproMessageTrigger", "notification")
                .WithCronSchedule("0 0/5 * * * ?")
                //.StartNow()
                .ForJob("coreproMessageJob", "notification")
                .Build();

            //Cancel chore after 24 hour of cutoff time

            var cancelChoreTrigger = TriggerBuilder.Create()
               .WithIdentity("cancelChoreTrigger", "cancelChore")
               //.WithCronSchedule("0 0/2 * * * ?")
               .StartNow()
               .ForJob("cancelPandingPaydayRequest", "cancelChore")
               .Build();

            //Send -ve Balance Message Payday Not Proceeded

            var negativeBalancePaydayTrigger = TriggerBuilder.Create()
             .WithIdentity("negativeBalancePaydayTrigger", "cancelChore")
               .WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(10, 0, DayOfWeek.Thursday).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")))
             .ForJob("SendMessagePaydayNotProceed", "cancelChore")
             .Build();

            // Schedule the job using the jobs and triggers 
            // scheduler.ScheduleJob(dailyChoresJob, dailyChoresTrigger); // Client told to stop this job
            // scheduler.ScheduleJob(weeklyChoresJob, weeklyChoresTrigger); // Client told to stop this job
            scheduler.ScheduleJob(makeWeeklyPaymentJob, makeWeeklyPaymentTrigger);
            scheduler.ScheduleJob(sendWeeklyPaydayStatusJob, sendWeeklyPaydayStatusTrigger);
            scheduler.ScheduleJob(notifyIncompleteMemberEnrollmentJob, notifyIncompleteNewMemberEnrollmentTrigger);
            scheduler.ScheduleJob(notifyParentHasNotLoggedInJob, notifyParentHasNotLoggedInTrigger);
            scheduler.ScheduleJob(renewSubscriptionJob, renewSubscriptionTrigger);
            //scheduler.ScheduleJob(notifyChildHasNotLoggedInJob, notifyChildHasNotLoggedInTrigger);  // Client told to stop this job
            scheduler.ScheduleJob(notifyNoChoreCompletedJob, notifyNoChoreCompletedTrigger);
            scheduler.ScheduleJob(notifyContinuousChildActivityJob, notifyContinuousChildActivityTrigger);
            scheduler.ScheduleJob(notifySubscriptionRenewalJob, notifySubscriptionRenewalTrigger);
            scheduler.ScheduleJob(remindSMSApprovalJob, remindSMSApprovalTrigger);
            scheduler.ScheduleJob(cancelNotRespondedSMSJob, cancelNotRespondedSMSTrigger);
            scheduler.ScheduleJob(remindChorePaymentApprovalJob, remindChorePaymentApprovalTrigger);
           // scheduler.ScheduleJob(coreproMessageJob, coreproMessageTrigger);
            //scheduler.ScheduleJob(cancelPandingPaydayRequest, cancelChoreTrigger);
            //scheduler.ScheduleJob(SendMessagePaydayNotProceed, negativeBalancePaydayTrigger);
        }
    }
}
