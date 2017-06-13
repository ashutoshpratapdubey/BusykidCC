using LeapSpring.MJC.Core.Domain.Account;
using LeapSpring.MJC.Core.Domain.Banking;
using LeapSpring.MJC.Core.Domain.Bonus;
using LeapSpring.MJC.Core.Domain.Charities;
using LeapSpring.MJC.Core.Domain.Chore;
using LeapSpring.MJC.Core.Domain.Earnings;
using LeapSpring.MJC.Core.Domain.Email;
using LeapSpring.MJC.Core.Domain.Family;
using LeapSpring.MJC.Core.Domain.Invitation;
using LeapSpring.MJC.Core.Domain.Save;
using LeapSpring.MJC.Core.Domain.Settings;
using LeapSpring.MJC.Core.Domain.Sms;
using LeapSpring.MJC.Core.Domain.Spend;
using LeapSpring.MJC.Core.Domain.Subscription;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LeapSpring.MJC.Data
{
    public class MJCDbContext : DbContext
    {
        #region Family

        public DbSet<Family> Family { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<FamilyMember> FamilyMember { get; set; }

        public DbSet<State> State { get; set; }

        #endregion

        #region Chore

        public DbSet<SystemChore> SystemChore { get; set; }

        public DbSet<Chore> Chore { get; set; }

        public DbSet<ChildEarnings> ChildEarnings { get; set; }

        #endregion

        #region Account

        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }
        public DbSet<PhoneNumberConfirmation> PhoneNumberConfirmation { get; set; }

        #endregion

        #region Banking

        public DbSet<BankTransaction> BankTransaction { get; set; }
        public DbSet<TransactionLog> TransactionLog { get; set; }
        public DbSet<FinancialAccount> FinancialAccount { get; set; }
        public DbSet<CoreproSettings> CoreproSettings { get; set; }

        #endregion

        #region Sms

        public DbSet<ChoreWorkflow> ChoreWorkflow { get; set; }
        public DbSet<Joke> Joke { get; set; }
        public DbSet<SMSApproval> SMSApproval { get; set; }

        #endregion

        #region Settings

        public DbSet<AllocationSettings> AllocationSettings { get; set; }
        public DbSet<AllocationByAge> AllocationByAge { get; set; }
        public DbSet<GyftSettings> GyftSettings { get; set; }

        #endregion

        #region Bonus

        public DbSet<ChildBonus> ChildBonus { get; set; }

        #endregion

        #region Invitation

        public DbSet<FamilyInvitation> FamilyInvitation { get; set; }

        #endregion

        #region Charity

        public DbSet<Charity> Charity { get; set; }

        public DbSet<Donation> Donation { get; set; }

        #endregion

        #region Spend

        public DbSet<PurchasedGiftCard> PurchasedGiftCard { get; set; }
        public DbSet<GiftCard> GiftCard { get; set; }

        public DbSet<CashOut> CashOut { get; set; }

        #endregion

        #region Email

        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<EmailHistory> EmailHistory { get; set; }

        #endregion

        #region Save

        public DbSet<StockItem> StockItem { get; set; }

        public DbSet<StockPurchaseRequest> PurchasedStock { get; set; }

        #endregion

        #region Subscription

        DbSet<FamilySubscription> FamilySubscription { get; set; }

        DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }

        DbSet<SubscriptionPromoCode> SubscriptionPromoCode { get; set; }

        DbSet<SubscriptionCancellationRequest> SubscriptionCancellationRequest { get; set; }

        DbSet<TransactionStatusFileDetails> TransactionStatusFileDetails { get; set; }

        DbSet<EventStatusLog> EventStatusLog { get; set; }
        #endregion

        #region Credit Card 
        DbSet<CreditCardAccount> CreditCardAccount { get; set; }

        #endregion


        // Ctor
        public MJCDbContext()
        {
            Configuration.LazyLoadingEnabled = false;
        }

        // Remove morethan one foreign key restriction
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Ignore field from zipform document table
            //modelBuilder.Entity<ZipFormDocument>().Ignore(p => p.ContentType);

            base.OnModelCreating(modelBuilder);
        }
    }
}
