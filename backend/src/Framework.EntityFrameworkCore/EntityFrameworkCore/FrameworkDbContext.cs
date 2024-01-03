using Framework.Admin;
using Abp.IdentityServer4vNext;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Framework.Authorization.Delegation;
using Framework.Authorization.Roles;
using Framework.Authorization.Users;
using Framework.Chat;
using Framework.Editions;
using Framework.Friendships;
using Framework.MultiTenancy;
using Framework.MultiTenancy.Payments;
using Framework.Storage;
using Framework.AService;
using Framework.Bill;
using Framework.ServiceRegister;
using Framework.Feedback;
using Stripe;
using Invoice = Framework.MultiTenancy.Accounting.Invoice;
using Framework.ApartmentManagement;
using Framework.Authorization.Accounts;

namespace Framework.EntityFrameworkCore
{
    public class FrameworkDbContext : AbpZeroDbContext<Tenant, Role, User, FrameworkDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<People> Peoples { get; set; }

        public virtual DbSet<EmailAddressOTP> EmailAddressOTPs { get; set; }

        /* Define an IDbSet for each entity of the application */
        //Apartment Service
        public virtual DbSet<ApartmentService> AparmentServices { get; set; }

        //Apartment Management
        public virtual DbSet<Apartment> Apartments { get; set; }

        //Bill
        
        public virtual DbSet<ServiceBill> ServiceBills { get; set; }
      
        public virtual DbSet<BillingInformation> BillingInformations { get; set; }
        public virtual DbSet<ApartmentBill> ApartmentBills { get; set; }
        public virtual DbSet<UserFeedBack> UserFeedbacks { get; set; }

        //Register Service
        public virtual DbSet<UserServiceRegister2> UserServiceRegisters { get; set; }

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        public FrameworkDbContext(DbContextOptions<FrameworkDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<People>(p =>
            {
                p.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<BinaryObject>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}