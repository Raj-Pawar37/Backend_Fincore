using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Backend_Fincore.Models
{
    public class User : BaseEntity
    {
        public string? TotpSecretKey { get; set; }
        public bool Is2FAEnabled { get; set; }
        [Key]
        public int UserId { get; set; }

        public int RoleId { get; set; }

        // EmployeeId, VendorId or CustomerId
        public int MasterId { get; set; }

        // Employee / Vendor / Customer
        public string MasterType { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? MobileNo { get; set; }

        public int FailedLoginAttempts { get; set; }

        public byte IsEmailVerified { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public Role Role { get; set; } = null!;

        public ICollection<UserToken> UserTokens { get; set; }
            = new List<UserToken>();

        public ICollection<RolePermission> CreatedRolePermissions { get; set; }
            = new List<RolePermission>();

        public ICollection<Budget> ApprovedBudgets { get; set; }
            = new List<Budget>();

        public ICollection<CapexRequest> RequestedCapexRequests { get; set; }
            = new List<CapexRequest>();

        public ICollection<CapexRequest> ApprovedCapexRequests { get; set; }
            = new List<CapexRequest>();


        public ICollection<GRN> ReceivedGRNs { get; set; }
            = new List<GRN>();

        public ICollection<Asset> AssignedAssets { get; set; }
            = new List<Asset>();


        public ICollection<OpexRequest> RequestedOpexRequests { get; set; }
            = new List<OpexRequest>();

        public ICollection<OpexRequest> ApprovedOpexRequests { get; set; }
            = new List<OpexRequest>();

        public ICollection<ExpenseClaim> ClaimedExpenseClaims { get; set; }
            = new List<ExpenseClaim>();

        public ICollection<ExpenseClaim> ApprovedExpenseClaims { get; set; }
            = new List<ExpenseClaim>();
    }
}
