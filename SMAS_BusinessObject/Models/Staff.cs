using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Staff
{
    public int UserId { get; set; }

    public decimal? Salary { get; set; }

    public string? ExperienceLevel { get; set; }

    public DateOnly HireDate { get; set; }

    public string? Position { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankName { get; set; }

    public decimal? Rating { get; set; }

    public bool? IsWorking { get; set; }

    public string? TaxId { get; set; }

    public virtual ICollection<BookEvent> BookEvents { get; set; } = new List<BookEvent>();

    public virtual ICollection<Combo> Combos { get; set; } = new List<Combo>();

    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = new List<CustomerFeedback>();

    public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; } = new List<DeliveryDetail>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<FoodRecipe> FoodRecipes { get; set; } = new List<FoodRecipe>();

    public virtual ICollection<ImExport> ImExports { get; set; } = new List<ImExport>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<StaffLog> StaffLogs { get; set; } = new List<StaffLog>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
