using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Role { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<BookEvent> BookEvents { get; set; } = new List<BookEvent>();

    public virtual ICollection<Buffet> Buffets { get; set; } = new List<Buffet>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = new List<CustomerFeedback>();

    public virtual ICollection<EmployeeAnnouncement> EmployeeAnnouncements { get; set; } = new List<EmployeeAnnouncement>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> NotificationSenders { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<SalaryRecord> SalaryRecords { get; set; } = new List<SalaryRecord>();

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<WorkStaff> WorkStaffs { get; set; } = new List<WorkStaff>();
}
