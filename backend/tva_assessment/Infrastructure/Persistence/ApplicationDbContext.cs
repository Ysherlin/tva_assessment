using Microsoft.EntityFrameworkCore;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the database context for the assessment application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// The persons set in the database.
        /// </summary>
        public DbSet<Person> Persons { get; set; } = null!;

        /// <summary>
        /// The accounts set in the database.
        /// </summary>
        public DbSet<Account> Accounts { get; set; } = null!;

        /// <summary>
        /// The account statuses set in the database.
        /// </summary>
        public DbSet<AccountStatus> AccountStatuses { get; set; } = null!;

        /// <summary>
        /// The transactions set in the database.
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; } = null!;

        /// <summary>
        /// Creates a new instance of the database context.
        /// </summary>
        /// <param name="options">The options used to configure the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the entity mappings for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurePerson(modelBuilder);
            ConfigureAccount(modelBuilder);
            ConfigureAccountStatus(modelBuilder);
            ConfigureTransaction(modelBuilder);
        }

        /// <summary>
        /// Configures the person entity.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model.</param>
        private static void ConfigurePerson(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Person>();

            entity.ToTable("Persons");

            entity.HasKey(p => p.Code);

            entity.Property(p => p.Code)
                  .HasColumnName("code")
                  .ValueGeneratedOnAdd();

            entity.Property(p => p.IdNumber)
                  .HasColumnName("id_number")
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(p => p.Name)
                  .HasColumnName("name")
                  .HasMaxLength(50);

            entity.Property(p => p.Surname)
                  .HasColumnName("surname")
                  .HasMaxLength(50);

            entity.HasIndex(p => p.IdNumber)
                  .IsUnique()
                  .HasDatabaseName("IX_Person_id");

            entity.HasMany(p => p.Accounts)
                  .WithOne(a => a.Person)
                  .HasForeignKey(a => a.PersonCode)
                  .HasConstraintName("FK_Account_Person");
        }

        /// <summary>
        /// Configures the account entity.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model.</param>
        private static void ConfigureAccount(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Account>();

            entity.ToTable("Accounts");

            entity.HasKey(a => a.Code);

            entity.Property(a => a.Code)
                  .HasColumnName("code")
                  .ValueGeneratedOnAdd();

            entity.Property(a => a.PersonCode)
                  .HasColumnName("person_code")
                  .IsRequired();

            entity.Property(a => a.AccountNumber)
                  .HasColumnName("account_number")
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(a => a.OutstandingBalance)
                  .HasColumnName("outstanding_balance")
                  .IsRequired()
                  .HasColumnType("money");

            entity.HasIndex(a => a.AccountNumber)
                  .IsUnique()
                  .HasDatabaseName("IX_Account_num");

            entity.HasMany(a => a.Transactions)
                  .WithOne(t => t.Account)
                  .HasForeignKey(t => t.AccountCode)
                  .HasConstraintName("FK_Transaction_Account");
        }

        /// <summary>
        /// Configures the account status entity.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model.</param>
        private static void ConfigureAccountStatus(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AccountStatus>();

            entity.ToTable("Accounts_status");

            entity.HasKey(s => s.AccountCode);

            entity.Property(s => s.AccountCode)
                  .HasColumnName("account_code")
                  .IsRequired();

            entity.Property(s => s.IsClosed)
                  .HasColumnName("status")
                  .IsRequired();

            entity.HasOne(s => s.Account)
                  .WithOne(a => a.Status)
                  .HasForeignKey<AccountStatus>(s => s.AccountCode)
                  .HasConstraintName("FK_Accounts_status_Account");
        }

        /// <summary>
        /// Configures the transaction entity.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure the model.</param>
        private static void ConfigureTransaction(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Transaction>();

            entity.ToTable("Transactions");

            entity.HasKey(t => t.Code);

            entity.Property(t => t.Code)
                  .HasColumnName("code")
                  .ValueGeneratedOnAdd();

            entity.Property(t => t.AccountCode)
                  .HasColumnName("account_code")
                  .IsRequired();

            entity.Property(t => t.TransactionDate)
                  .HasColumnName("transaction_date")
                  .IsRequired();

            entity.Property(t => t.CaptureDate)
                  .HasColumnName("capture_date")
                  .IsRequired();

            entity.Property(t => t.Amount)
                  .HasColumnName("amount")
                  .IsRequired()
                  .HasColumnType("money");

            entity.Property(t => t.Description)
                  .HasColumnName("description")
                  .IsRequired()
                  .HasMaxLength(100);
        }
    }
}
