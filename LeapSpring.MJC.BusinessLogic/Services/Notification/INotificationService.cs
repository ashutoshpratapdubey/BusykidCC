namespace LeapSpring.MJC.BusinessLogic.Services.Notification
{
    public interface INotificationService
    {
        /// <summary>
        /// Send pay day notification.
        /// </summary>
        void SendPayDayMessage();

        /// <summary>
        /// Notify incomplete member entrollment
        /// </summary>
        void NotifyIncompleteNewMemberEntrollment();

        /// <summary>
        /// Notify parent has not loggedin
        /// </summary>
        void NotifyParentHasNotLoggedIn();

        /// <summary>
        /// Notify child has not loggedin
        /// </summary>
        void NotifyChildHasNotLoggedIn();

        /// <summary>
        /// Notify no chore completed
        /// </summary>
        void NotifyNoChoreCompleted();

        /// <summary>
        /// Notify continuous child activity
        /// </summary>
        void NotifyContinuousChildActivity();

        /// <summary>
        /// Notifies the admin about subscription renewal.
        /// </summary>
        void NotifySubscriptionRenewal();

        void NotifyVerifyPendingAccount();
    }
}
