using Backend_Fincore.Domain.Models;

using Backend_Fincore.Models;
using Backend_Fincore.Models.Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        // Authentication
        public DbSet<User> User { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }


        // Company masters
        public DbSet<Company> Company { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<Customer> Customer { get; set; }

        // Document helpers
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<Document> Document { get; set; }

        // Location masters
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }


        public DbSet<AccountMaster> AccountMaster { get; set; }

        public DbSet<BudgetCategory> BudgetCategory { get; set; }

        public DbSet<Budget> Budget { get; set; }

        public DbSet<BudgetLine> BudgetLine { get; set; }


        // Cpex
        public DbSet<CapexRequest> CapexRequest { get; set; }

        public DbSet<PurchaseRequisition> PurchaseRequisition { get; set; }

        public DbSet<RFQ> RFQ { get; set; }

        public DbSet<RFQVendor> RFQVendor { get; set; }

        public DbSet<RFQItem> RFQItem { get; set; }

        public DbSet<Quotation> Quotation { get; set; }

        


        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }

        public DbSet<PurchaseOrderItem> PurchaseOrderItem { get; set; }

        public DbSet<GRN> GRN { get; set; }

        public DbSet<Asset> Asset { get; set; }

        public DbSet<APInvoice> APInvoice { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<OpexRequest> OpexRequest { get; set; }

        public DbSet<ExpenseClaim> ExpenseClaim { get; set; }

        public DbSet<WorkOrder> WorkOrder { get; set; }

        public DbSet<RevenueEntry> RevenueEntry { get; set; }

        public DbSet<ARInvoice> ARInvoice { get; set; }

        public DbSet<JournalEntry> JournalEntry { get; set; }

        public DbSet<DocumentNumberMaster> DocumentNumberMasters { get; set; }

        public DbSet<QuotationItem> QuotationItem { get; set; }

        public DbSet<GRNItem> GRNItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            /*
             * ROLE
             */
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(x => x.RoleId);

                entity.Property(x => x.RoleId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.RoleName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.RoleCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.RoleDescription)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasIndex(x => x.RoleCode)
                    .IsUnique();
            });


            /*
             * PERMISSION
             */
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");

                entity.HasKey(x => x.PermissionId);

                entity.Property(x => x.PermissionId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.PermissionName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ModuleName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasIndex(x => new
                {
                    x.ModuleName,
                    x.PermissionName
                })
                .IsUnique();
            });


            /*
             * ROLE PERMISSION
             */
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");

                entity.HasKey(x => x.RolePermissionId);

                entity.Property(x => x.RolePermissionId)
                    .ValueGeneratedOnAdd();

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Permission)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.PermissionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.CreatedRolePermissions)
                    .HasForeignKey(x => x.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent same permission being assigned twice
                entity.HasIndex(x => new
                {
                    x.RoleId,
                    x.PermissionId
                })
                .IsUnique();
            });


            /*
             * USER
             */
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.UserId);

                entity.Property(x => x.UserId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.MasterType)
                    .HasColumnType("varchar(30)")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.Username)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.MobileNo)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20);

                entity.Property(x => x.FailedLoginAttempts)
                    .HasDefaultValue(0)
                    .IsRequired();

                entity.Property(x => x.IsEmailVerified)
                    .HasColumnType("tinyint")
                    .HasDefaultValue((byte)0)
                    .IsRequired();

                entity.Property(x => x.LastLoginDate)
                    .HasColumnType("datetime");

                entity.HasIndex(x => x.Username)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasIndex(x => new
                {
                    x.MasterType,
                    x.MasterId
                });

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            /*
             * USER TOKEN
             */
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserTokens");

                entity.HasKey(x => x.UserTokenId);

                entity.Property(x => x.UserTokenId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Token)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.TokenType)
                    .HasColumnType("varchar(30)")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.ExpiryDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            /*
             * COMPANY
             */
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Companies");

                entity.HasKey(x => x.CompanyId);

                entity.Property(x => x.CompanyId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CompanyName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.CompanyCode)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.GSTNo)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50);

                entity.Property(x => x.PANNo)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ContactEmail)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.CompanyCode)
                    .IsUnique();



                entity.HasOne(x => x.Country)
                    .WithMany(x => x.Companies)
                    .HasForeignKey(x => x.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.State)
                    .WithMany(x => x.Companies)
                    .HasForeignKey(x => x.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.City)
                    .WithMany(x => x.Companies)
                    .HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            /*
             * DEPARTMENT
             */
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");

                entity.HasKey(x => x.DepartmentId);

                entity.Property(x => x.DepartmentId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.DepartmentName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.DepartmentCode)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Departments)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Department code unique inside one company
                entity.HasIndex(x => new
                {
                    x.CompanyId,
                    x.DepartmentCode
                })
                .IsUnique();
            });


            /*
             * EMPLOYEE
             */
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(x => x.EmployeeId);

                entity.Property(x => x.EmployeeId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.EmployeeCode)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.FirstName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50);

                entity.Property(x => x.Designation)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.JoiningDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Employees)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Department)
                    .WithMany(x => x.Employees)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ReportingManager)
                    .WithMany(x => x.ReportingEmployees)
                    .HasForeignKey(x => x.ReportingManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CompanyId,
                    x.EmployeeCode
                })
                .IsUnique();
            });


            /*
             * VENDOR
             */
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("Vendors");

                entity.HasKey(x => x.VendorId);

                entity.Property(x => x.VendorId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.VendorName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.VendorCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.PANNo)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Vendors)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CompanyId,
                    x.VendorCode
                })
                .IsUnique();
            });


            /*
             * CUSTOMER
             */
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");

                entity.HasKey(x => x.CustomerId);

                entity.Property(x => x.CustomerId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CustomerName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.CustomerCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.PANNo)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Customers)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CompanyId,
                    x.CustomerCode
                })
                .IsUnique();
            });


            /*
 * DOCUMENT TYPE
 */
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentTypes");

                entity.HasKey(x => x.DocumentTypeId);

                entity.Property(x => x.DocumentTypeId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.DocumentTypeName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(x => x.DocumentTypeName)
                    .IsUnique();
            });


            /*
             * DOCUMENT
             */
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documents");

                entity.HasKey(x => x.DocumentId);

                entity.Property(x => x.DocumentId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.MasterType)
                    .HasColumnType("varchar(30)")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.FilePath)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.FileType)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.FileName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Remarks)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasOne(x => x.DocumentType)
                    .WithMany(x => x.Documents)
                    .HasForeignKey(x => x.DocumentTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.MasterType,
                    x.MasterId
                });
            });


            /*
             * COUNTRY
             */
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");

                entity.HasKey(x => x.CountryId);

                entity.Property(x => x.CountryId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CountryName)
                    .HasColumnType("varchar(30)")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.CountryCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(x => x.CountryCode)
                    .IsUnique();

                entity.HasIndex(x => x.CountryName)
                    .IsUnique();
            });


            /*
             * STATE
             */
            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("States");

                entity.HasKey(x => x.StateId);

                entity.Property(x => x.StateId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.StateCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.StateName)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasOne(x => x.Country)
                    .WithMany(x => x.States)
                    .HasForeignKey(x => x.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CountryId,
                    x.StateCode
                })
                .IsUnique();

                entity.HasIndex(x => new
                {
                    x.CountryId,
                    x.StateName
                })
                .IsUnique();
            });


            /*
             * CITY
             */
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");

                entity.HasKey(x => x.CityId);

                entity.Property(x => x.CityId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CityName)
                    .HasColumnType("varchar(30)")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasOne(x => x.State)
                    .WithMany(x => x.Cities)
                    .HasForeignKey(x => x.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.StateId,
                    x.CityName
                })
                .IsUnique();
            });

            /*
----------------------------------------
ACCOUNT MASTER
----------------------------------------
*/

            modelBuilder.Entity<AccountMaster>(entity =>
            {
                entity.ToTable("AccountMasters");

                entity.HasKey(x => x.AccountMasterId);

                entity.Property(x => x.AccountCode)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.AccountName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.AccountType)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.HasIndex(x => x.AccountCode).IsUnique();
            });


            /*
            ----------------------------------------
            BUDGET CATEGORY
            ----------------------------------------
            */

            modelBuilder.Entity<BudgetCategory>(entity =>
            {
                entity.ToTable("BudgetCategories");

                entity.HasKey(x => x.BudgetCategoryId);

                entity.Property(x => x.CategoryName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.CategoryCode)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.HasIndex(x => x.CategoryCode).IsUnique();
            });


            /*
            ----------------------------------------
            BUDGET
            ----------------------------------------
            */

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.ToTable("Budgets");

                entity.HasKey(x => x.BudgetId);

                entity.Property(x => x.FinancialYear)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.TotalBudget)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.ApprovedDate)
                    .HasColumnType("datetime");

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Budgets)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Department)
                    .WithMany(x => x.Budgets)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ApprovedByUser)
                    .WithMany(x => x.ApprovedBudgets)
                    .HasForeignKey(x => x.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CompanyId,
                    x.DepartmentId,
                    x.FinancialYear
                }).IsUnique();
            });


            /*
            ----------------------------------------
            BUDGET LINE
            ----------------------------------------
            */

            modelBuilder.Entity<BudgetLine>(entity =>
            {
                entity.ToTable("BudgetLines");

                entity.HasKey(x => x.BudgetLineId);

                entity.Property(x => x.CostCenter)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.AllocatedAmount)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.Budget)
                    .WithMany(x => x.BudgetLines)
                    .HasForeignKey(x => x.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.BudgetCategory)
                    .WithMany(x => x.BudgetLines)
                    .HasForeignKey(x => x.BudgetCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.BudgetId,
                    x.CostCenter,
                    x.BudgetCategoryId
                }).IsUnique();
            });


            /*
----------------------------------------
CAPEX REQUEST
----------------------------------------
*/

            modelBuilder.Entity<CapexRequest>(entity =>
            {
                entity.ToTable("CapexRequests");

                entity.HasKey(x => x.CapexRequestId);

                entity.Property(x => x.CapexRequestId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Title)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.Property(x => x.ApprovedDate)
                    .HasColumnType("datetime");

                entity.HasOne(x => x.BudgetLine)
                    .WithMany(x => x.CapexRequests)
                    .HasForeignKey(x => x.BudgetLineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.RequestedByUser)
                    .WithMany(x => x.RequestedCapexRequests)
                    .HasForeignKey(x => x.RequestedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ApprovedByUser)
                    .WithMany(x => x.ApprovedCapexRequests)
                    .HasForeignKey(x => x.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            /*
            ----------------------------------------
            PURCHASE REQUISITION
            ----------------------------------------
            */

            modelBuilder.Entity<PurchaseRequisition>(entity =>
            {
                entity.ToTable("PurchaseRequisitions");

                entity.HasKey(x => x.PRId);

                entity.Property(x => x.PRId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.PRNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Title)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.HasOne(x => x.CapexRequest)
                    .WithMany(x => x.PurchaseRequisitions)
                    .HasForeignKey(x => x.CapexRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.PRNumber)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            RFQ
            ----------------------------------------
            */

            modelBuilder.Entity<RFQ>(entity =>
            {
                entity.ToTable("RFQs");

                entity.HasKey(x => x.RFQId);

                entity.Property(x => x.RFQId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.RFQNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Title)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.IssueDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.ClosingDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Draft")
                    .IsRequired();

                entity.HasOne(x => x.PurchaseRequisition)
                    .WithMany(x => x.RFQs)
                    .HasForeignKey(x => x.PRId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.RFQNumber)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            RFQ VENDOR
            ----------------------------------------
            */

            modelBuilder.Entity<RFQVendor>(entity =>
            {
                entity.ToTable("RFQVendors");

                entity.HasKey(x => x.RFQVendorId);

                entity.Property(x => x.RFQVendorId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.SentDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.ResponseStatus)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Invited")
                    .IsRequired();

                entity.Property(x => x.ResponseDate)
                    .HasColumnType("datetime");

                entity.HasOne(x => x.RFQ)
                    .WithMany(x => x.RFQVendors)
                    .HasForeignKey(x => x.RFQId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Vendor)
                    .WithMany(x => x.RFQVendors)
                    .HasForeignKey(x => x.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent same vendor being invited twice
                entity.HasIndex(x => new
                {
                    x.RFQId,
                    x.VendorId
                })
                .IsUnique();
            });


            /*
            ----------------------------------------
            RFQ ITEM
            ----------------------------------------
            */

            modelBuilder.Entity<RFQItem>(entity =>
            {
                entity.ToTable("RFQItems");

                entity.HasKey(x => x.RFQItemId);

                entity.Property(x => x.RFQItemId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Quantity)
                    .HasColumnType("int")
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.Property(x => x.AttachmentPath)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.HasOne(x => x.RFQ)
                    .WithMany(x => x.RFQItems)
                    .HasForeignKey(x => x.RFQId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            /*
            ----------------------------------------
            QUOTATION
            ----------------------------------------
            */

            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.ToTable("Quotations");

                entity.HasKey(x => x.QuotationId);

                entity.Property(x => x.QuotationId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.QuotationNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Submitted")
                    .IsRequired();

                entity.Property(x => x.AttachmentPath)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasOne(x => x.RFQVendor)
                    .WithMany(x => x.Quotations)
                    .HasForeignKey(x => x.RFQVendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.QuotationNumber)
                    .IsUnique();

                // One vendor quotation per RFQ invitation
                entity.HasIndex(x => x.RFQVendorId)
                    .IsUnique();
            });






            /*
             * 
             * 

----------------------------------------
PURCHASE ORDER
----------------------------------------
*/

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("PurchaseOrders");

                entity.HasKey(x => x.PurchaseOrderId);

                entity.Property(x => x.PurchaseOrderId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.PONumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Draft")
                    .IsRequired();

                entity.HasOne(x => x.Vendor)
                    .WithMany(x => x.PurchaseOrders)
                    .HasForeignKey(x => x.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Quotation)
                    .WithOne(x => x.PurchaseOrder)
                    .HasForeignKey<PurchaseOrder>(x => x.QuotationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.PONumber)
                    .IsUnique();

                entity.HasIndex(x => x.QuotationId)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            PURCHASE ORDER ITEM
            ----------------------------------------
            */

            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.ToTable("PurchaseOrderItems");

                entity.HasKey(x => x.POItemId);

                entity.Property(x => x.POItemId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ItemName)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ItemType)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20);

                entity.Property(x => x.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Tax)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Discount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Qty)
                    .HasColumnType("int")
                    .IsRequired();

                entity.HasOne(x => x.PurchaseOrder)
                    .WithMany(x => x.PurchaseOrderItems)
                    .HasForeignKey(x => x.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);


                
                    entity.HasOne(x => x.QuotationItem)
                    .WithOne(x => x.PurchaseOrderItem)
                    .HasForeignKey<PurchaseOrderItem>(x => x.QuotationItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity .HasIndex(x => x.QuotationItemId)
                     .IsUnique();
                   });


            /*
            ----------------------------------------
            GRN
            ----------------------------------------
            */

            modelBuilder.Entity<GRN>(entity =>
            {
                entity.ToTable("GRNs");

                entity.HasKey(x => x.GRNId);

                entity.Property(x => x.GRNId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.GRNNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.ReceivedDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Remarks)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.DeliveryChallanNumber)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100);

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Received")
                    .IsRequired();

                entity.HasOne(x => x.PurchaseOrder)
                    .WithMany(x => x.GRNs)
                    .HasForeignKey(x => x.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ReceivedByUser)
                    .WithMany(x => x.ReceivedGRNs)
                    .HasForeignKey(x => x.ReceivedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.GRNNumber)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            ASSET
            ----------------------------------------
            */

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("Assets");

                entity.HasKey(x => x.AssetId);

                entity.Property(x => x.AssetId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.AssetCode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.AssetName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.PurchaseCost)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.PurchaseDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Available")
                    .IsRequired();

                entity.HasOne(x => x.GRN)
                    .WithMany(x => x.Assets)
                    .HasForeignKey(x => x.GRNId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.PurchaseOrderItem)
                    .WithMany(x => x.Assets)
                    .HasForeignKey(x => x.POItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.AssignedToUser)
                    .WithMany(x => x.AssignedAssets)
                    .HasForeignKey(x => x.AssignedTo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.AssetCode)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            AP INVOICE
            ----------------------------------------
            */

            modelBuilder.Entity<APInvoice>(entity =>
            {
                entity.ToTable("APInvoices");

                entity.HasKey(x => x.APInvoiceId);

                entity.Property(x => x.APInvoiceId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.MasterType)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.InvoiceNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.InvoiceAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.InvoiceDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.HasOne(x => x.Vendor)
                    .WithMany(x => x.APInvoices)
                    .HasForeignKey(x => x.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.VendorId,
                    x.InvoiceNumber
                })
                .IsUnique();

                entity.HasIndex(x => new
                {
                    x.MasterType,
                    x.MasterId
                });
            });


            /*
            ----------------------------------------
            PAYMENT
            ----------------------------------------
            */

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(x => x.PaymentId);

                entity.Property(x => x.PaymentId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.MasterType)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.PaymentDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.TransactionType)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.PaymentMode)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Remarks)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.HasIndex(x => new
                {
                    x.MasterType,
                    x.MasterId
                });
            });


            /*
----------------------------------------
OPEX REQUEST
----------------------------------------
*/

            modelBuilder.Entity<OpexRequest>(entity =>
            {
                entity.ToTable("OpexRequests");

                entity.HasKey(x => x.OpexRequestId);

                entity.Property(x => x.OpexRequestId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Title)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.Property(x => x.ApprovedDate)
                    .HasColumnType("datetime");

                entity.HasOne(x => x.BudgetLine)
                    .WithMany(x => x.OpexRequests)
                    .HasForeignKey(x => x.BudgetLineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.RequestedByUser)
                    .WithMany(x => x.RequestedOpexRequests)
                    .HasForeignKey(x => x.RequestedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ApprovedByUser)
                    .WithMany(x => x.ApprovedOpexRequests)
                    .HasForeignKey(x => x.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            /*
            ----------------------------------------
            EXPENSE CLAIM
            ----------------------------------------
            */

            modelBuilder.Entity<ExpenseClaim>(entity =>
            {
                entity.ToTable("ExpenseClaims");

                entity.HasKey(x => x.ExpenseClaimId);

                entity.Property(x => x.ExpenseClaimId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ClaimNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.ExpenseAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.ExpenseDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(200)")
                    .HasMaxLength(200);

                entity.Property(x => x.BillFilePath)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.Property(x => x.ApprovedDate)
                    .HasColumnType("datetime");

                entity.HasOne(x => x.OpexRequest)
                    .WithMany(x => x.ExpenseClaims)
                    .HasForeignKey(x => x.OpexRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ClaimedByUser)
                    .WithMany(x => x.ClaimedExpenseClaims)
                    .HasForeignKey(x => x.ClaimedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ApprovedByUser)
                    .WithMany(x => x.ApprovedExpenseClaims)
                    .HasForeignKey(x => x.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.ClaimNumber)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            WORK ORDER
            ----------------------------------------
            */

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.ToTable("WorkOrders");

                entity.HasKey(x => x.WorkOrderId);

                entity.Property(x => x.WorkOrderId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.WorkOrderNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Title)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.StartDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.EndDate)
                    .HasColumnType("datetime");

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Draft")
                    .IsRequired();

                entity.HasOne(x => x.OpexRequest)
                    .WithMany(x => x.WorkOrders)
                    .HasForeignKey(x => x.OpexRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Vendor)
                    .WithMany(x => x.WorkOrders)
                    .HasForeignKey(x => x.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.WorkOrderNumber)
                    .IsUnique();
            });


            /*
            ----------------------------------------
            REVENUE ENTRY
            ----------------------------------------
            */

            modelBuilder.Entity<RevenueEntry>(entity =>
            {
                entity.ToTable("RevenueEntries");

                entity.HasKey(x => x.RevenueEntryId);

                entity.Property(x => x.RevenueEntryId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.ProfitCenterName)
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.RevenueDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.RevenueEntries)
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            /*
            ----------------------------------------
            AR INVOICE
            ----------------------------------------
            */

            modelBuilder.Entity<ARInvoice>(entity =>
            {
                entity.ToTable("ARInvoices");

                entity.HasKey(x => x.ARInvoiceId);

                entity.Property(x => x.ARInvoiceId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.InvoiceNumber)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.InvoiceAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.InvoiceDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending")
                    .IsRequired();

                entity.Property(x => x.PONumber)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50);

                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.ARInvoices)
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.RevenueEntry)
                    .WithMany(x => x.ARInvoices)
                    .HasForeignKey(x => x.RevenueEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CustomerId,
                    x.InvoiceNumber
                })
                .IsUnique();
            });


            /*
            ----------------------------------------
            JOURNAL ENTRY
            ----------------------------------------
            */

            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.ToTable("JournalEntries");

                entity.HasKey(x => x.JournalEntryId);

                entity.Property(x => x.JournalEntryId)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.MasterType)
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.TransactionType)
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasColumnType("varchar(500)")
                    .HasMaxLength(500);

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.JournalEntries)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.AccountMaster)
                    .WithMany(x => x.JournalEntries)
                    .HasForeignKey(x => x.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.MasterType,
                    x.MasterId
                });
            });


            modelBuilder.Entity<QuotationItem>(entity =>
            {
                entity.HasOne(x => x.RFQItem)
                    .WithMany(x => x.QuotationItems)
                    .HasForeignKey(x => x.RFQItemId)
                    .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<GRNItem>(entity =>
            {
                entity.HasOne(x => x.GRN)
                    .WithMany(x => x.GRNItems)
                    .HasForeignKey(x => x.GRNItemId)
                    .OnDelete(DeleteBehavior.Restrict);

            });






        }
    }
}