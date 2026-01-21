using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class SmasDatabaseContext : DbContext
{
    public SmasDatabaseContext()
    {
    }

    public SmasDatabaseContext(DbContextOptions<SmasDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BookEvent> BookEvents { get; set; }

    public virtual DbSet<BookEventService> BookEventServices { get; set; }

    public virtual DbSet<Buffet> Buffets { get; set; }

    public virtual DbSet<BuffetFood> BuffetFoods { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboFood> ComboFoods { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }

    public virtual DbSet<DeliveryDetail> DeliveryDetails { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<EmployeeAnnouncement> EmployeeAnnouncements { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventFood> EventFoods { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<FoodRecipe> FoodRecipes { get; set; }

    public virtual DbSet<ImExport> ImExports { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryLog> InventoryLogs { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<SalaryRecord> SalaryRecords { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<StaffLog> StaffLogs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableOrder> TableOrders { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkShift> WorkShifts { get; set; }

    public virtual DbSet<WorkStaff> WorkStaffs { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=localhost;Database=SMAS_DATABASE;User Id=sa;Password=huy123;TrustServerCertificate=True");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    private string GetConnectionString()
    {

        IConfiguration config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", true, true)
              .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        return strConn;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AdminLog__5E548648A6718DC0");

            entity.ToTable("AdminLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.TableName).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminLog__UserId__47DBAE45");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E303B7738FC");

            entity.ToTable("Blog");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blog__AuthorId__6A30C649");
        });

        modelBuilder.Entity<BookEvent>(entity =>
        {
            entity.HasKey(e => e.BookEventId).HasName("PK__BookEven__01FE44D78CC48ABB");

            entity.ToTable("BookEvent");

            entity.HasIndex(e => e.BookingCode, "UQ__BookEven__C6E56BD5DC51DA09").IsUnique();

            entity.Property(e => e.BookingCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsContract).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.ConfirmedBy)
                .HasConstraintName("FK__BookEvent__Confi__0D7A0286");

            entity.HasOne(d => d.Contract).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__BookEvent__Contr__0E6E26BF");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Custo__0C85DE4D");

            entity.HasOne(d => d.Event).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Event__0B91BA14");
        });

        modelBuilder.Entity<BookEventService>(entity =>
        {
            entity.HasKey(e => new { e.BookEventId, e.ServiceId }).HasName("PK__BookEven__BDAFFFD79DD58809");

            entity.ToTable("BookEventService");

            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.BookEventServices)
                .HasForeignKey(d => d.BookEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__BookE__18EBB532");

            entity.HasOne(d => d.Service).WithMany(p => p.BookEventServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Servi__19DFD96B");
        });

        modelBuilder.Entity<Buffet>(entity =>
        {
            entity.HasKey(e => e.BuffetId).HasName("PK__Buffet__2A9FCF8546A1BFE8");

            entity.ToTable("Buffet");

            entity.Property(e => e.ChildrenPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.MainPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.SidePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Buffets)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Buffet__CreatedB__55F4C372");
        });

        modelBuilder.Entity<BuffetFood>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.BuffetId }).HasName("PK__BuffetFo__C7C44F1308D34ABC");

            entity.ToTable("BuffetFood");

            entity.Property(e => e.IsUnlimited).HasDefaultValue(true);
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Buffet).WithMany(p => p.BuffetFoods)
                .HasForeignKey(d => d.BuffetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BuffetFoo__Buffe__5CA1C101");

            entity.HasOne(d => d.Food).WithMany(p => p.BuffetFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BuffetFoo__FoodI__5BAD9CC8");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B660621F8");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__737584F61DE799D5").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.IsProcessedGoods).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combo__DD42582E5A0B9EEC");

            entity.ToTable("Combo");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NumberOfUsed).HasDefaultValue(0);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Combos)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Combo__CreatedBy__498EEC8D");
        });

        modelBuilder.Entity<ComboFood>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.ComboId }).HasName("PK__ComboFoo__78B996698FFFDAC9");

            entity.ToTable("ComboFood");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboFoods)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboFood__Combo__503BEA1C");

            entity.HasOne(d => d.Food).WithMany(p => p.ComboFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboFood__FoodI__4F47C5E3");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D346945E11D6D");

            entity.ToTable("Contract");

            entity.HasIndex(e => e.ContractCode, "UQ__Contract__CBECF8338831CC43").IsUnique();

            entity.Property(e => e.ContractCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContractFileUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepositAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EventType).HasMaxLength(100);
            entity.Property(e => e.RemainingAmount)
                .HasComputedColumnSql("([TotalAmount]-[DepositAmount])", false)
                .HasColumnType("decimal(19, 2)");
            entity.Property(e => e.SignMethod).HasMaxLength(50);
            entity.Property(e => e.SignedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.BookEventId)
                .HasConstraintName("FK__Contract__BookEv__10566F31");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contract__Custom__7F2BE32F");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__C050D8777C943BD2");

            entity.ToTable("Conversation");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMessageAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversat__UserI__5629CD9C");
        });

        modelBuilder.Entity<CustomerFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Customer__6A4BEDD6E50BA3EF");

            entity.ToTable("CustomerFeedback");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FeedbackType).HasMaxLength(50);
            entity.Property(e => e.RespondedAt).HasColumnType("datetime");
            entity.Property(e => e.ResponseStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__CustomerF__Order__3CF40B7E");

            entity.HasOne(d => d.RespondedByNavigation).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.RespondedBy)
                .HasConstraintName("FK__CustomerF__Respo__3DE82FB7");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerF__UserI__3BFFE745");
        });

        modelBuilder.Entity<DeliveryDetail>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Delivery__626D8FCE6278EF23");

            entity.ToTable("DeliveryDetail");

            entity.HasIndex(e => e.DeliveryCode, "UQ__Delivery__9EB035EC16E3C3A6").IsUnique();

            entity.Property(e => e.ActualDeliveryTime).HasColumnType("datetime");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeliveryFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.EstimatedDeliveryTime).HasColumnType("datetime");
            entity.Property(e => e.RecipientName).HasMaxLength(100);
            entity.Property(e => e.RecipientPhone).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.AssignedStaff).WithMany(p => p.DeliveryDetails)
                .HasForeignKey(d => d.AssignedStaffId)
                .HasConstraintName("FK__DeliveryD__Assig__208CD6FA");

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__DeliveryD__Order__17C286CF");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D96DADA2BEA");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.Code, "UQ__Discount__A25C5AA73E5A9312").IsUnique();

            entity.Property(e => e.ApplicableFor).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountType).HasMaxLength(50);
            entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MinOrderAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.UsedCount).HasDefaultValue(0);
            entity.Property(e => e.Value).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Discount__Create__03BB8E22");
        });

        modelBuilder.Entity<EmployeeAnnouncement>(entity =>
        {
            entity.HasKey(e => e.EmployeeAnnouncementId).HasName("PK__Employee__AC9E1FD864674FA1");

            entity.ToTable("EmployeeAnnouncement");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Normal");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeeAnnouncements)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__EmployeeA__Creat__5165187F");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C8101788864F");

            entity.ToTable("Event");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EventType).HasMaxLength(100);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Event__CreatedBy__04E4BC85");
        });

        modelBuilder.Entity<EventFood>(entity =>
        {
            entity.HasKey(e => new { e.BookEventId, e.FoodId }).HasName("PK__EventFoo__A9A89FE9410AAF08");

            entity.ToTable("EventFood");

            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.EventFoods)
                .HasForeignKey(d => d.BookEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventFood__BookE__2BC97F7C");

            entity.HasOne(d => d.Food).WithMany(p => p.EventFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventFood__FoodI__2CBDA3B5");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3EBBFA59B01");

            entity.ToTable("Food");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.IsDirectSale).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OrderCount).HasDefaultValue(0);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PromotionalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Rating).HasColumnType("decimal(2, 1)");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("Phần");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasMany(d => d.Categories).WithMany(p => p.Foods)
                .UsingEntity<Dictionary<string, object>>(
                    "FoodCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FoodCateg__Categ__3493CFA7"),
                    l => l.HasOne<Food>().WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FoodCateg__FoodI__339FAB6E"),
                    j =>
                    {
                        j.HasKey("FoodId", "CategoryId").HasName("PK__FoodCate__24FD204B20AA0FD7");
                        j.ToTable("FoodCategory");
                    });
        });

        modelBuilder.Entity<FoodRecipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__FoodReci__FDD988B0F82CA062");

            entity.ToTable("FoodRecipe");

            entity.HasIndex(e => new { e.FoodId, e.IngredientId }, "UQ_FoodRecipe").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByStaff).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.CreatedByStaffId)
                .HasConstraintName("FK__FoodRecip__Creat__42E1EEFE");

            entity.HasOne(d => d.Food).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodRecip__FoodI__40F9A68C");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodRecip__Ingre__41EDCAC5");
        });

        modelBuilder.Entity<ImExport>(entity =>
        {
            entity.HasKey(e => e.ImExportId).HasName("PK__ImExport__3EC3CB5804E7E552");

            entity.ToTable("ImExport");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FromWarehouse).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PricePerUnit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ToWarehouse).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UnitOfMeasurement).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByStaff).WithMany(p => p.ImExports)
                .HasForeignKey(d => d.CreatedByStaffId)
                .HasConstraintName("FK__ImExport__Create__74794A92");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.ImExports)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImExport__Ingred__73852659");

            entity.HasOne(d => d.Inventory).WithMany(p => p.ImExports)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImExport__Invent__72910220");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB25ADA52F3FD");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CCDD230367").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentStock).HasDefaultValue(0.0);
            entity.Property(e => e.IngredientName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UnitOfMeasurement).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.WarningLevel).HasDefaultValue(10.0);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6B32D5636BE");

            entity.ToTable("Inventory");

            entity.Property(e => e.BatchCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PricePerUnit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.WarehouseLocation).HasMaxLength(100);

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Ingre__6CD828CA");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__Inventory__Trans__6DCC4D03");
        });

        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.HasKey(e => e.InventoryLogId).HasName("PK__Inventor__97857072731D8D72");

            entity.ToTable("InventoryLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("FK__Inventory__Ingre__7B264821");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__Inventory__Inven__7A3223E8");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__UserI__7C1A6C5A");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C0C9CBA12EE73");

            entity.ToTable("Message");

            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.MessageType)
                .HasMaxLength(50)
                .HasDefaultValue("Text");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__Convers__5CD6CB2B");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__SenderI__5DCAEF64");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12F59A2F1A");

            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.Severity)
                .HasMaxLength(50)
                .HasDefaultValue("Info");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Sender).WithMany(p => p.NotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__Notificat__Sende__6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__6383C8BA");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF7813DFD6");

            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderCode, "UQ__Order__999B52291DF560FA").IsUnique();

            entity.Property(e => e.ClosedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeliveryPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.OrderType)
                .HasMaxLength(50)
                .HasDefaultValue("Dine-in");
            entity.Property(e => e.SubTotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BookEventId)
                .HasConstraintName("FK__Order__BookEvent__12FDD1B2");

            entity.HasOne(d => d.Delivery).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryId)
                .HasConstraintName("FK__Order__DeliveryI__14E61A24");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Order__DiscountI__13F1F5EB");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK__Order__Reservati__1209AD79");

            entity.HasOne(d => d.ServedByNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ServedBy)
                .HasConstraintName("FK__Order__ServedBy__15DA3E5D");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__UserId__11158940");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06815C120D72");

            entity.ToTable("OrderItem");

            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OpeningTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.ServedTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", false)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Buffet).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BuffetId)
                .HasConstraintName("FK__OrderItem__Buffe__251C81ED");

            entity.HasOne(d => d.Combo).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__OrderItem__Combo__2610A626");

            entity.HasOne(d => d.Food).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__OrderItem__FoodI__24285DB4");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__2334397B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A38FA1392F1");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.PaymentCode, "UQ__Payment__106D3BA8B4DC73A6").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__Payment__Contrac__345EC57D");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Payment__OrderId__336AA144");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReceivedBy)
                .HasConstraintName("FK__Payment__Receive__3552E9B6");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F244A0D9D65");

            entity.ToTable("Reservation");

            entity.HasIndex(e => e.ReservationCode, "UQ__Reservat__2081C0BB2B182FA0").IsUnique();

            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReservationCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ConfirmedByNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ConfirmedBy)
                .HasConstraintName("FK__Reservati__Confi__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__UserI__76969D2E");
        });

        modelBuilder.Entity<SalaryRecord>(entity =>
        {
            entity.HasKey(e => e.SalaryRecordId).HasName("PK__SalaryRe__F9633938BAF5E79A");

            entity.ToTable("SalaryRecord");

            entity.HasIndex(e => new { e.UserId, e.Month, e.Year }, "UQ_SalaryRecord").IsUnique();

            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Bonus)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Penalty)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalaryPerHour).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalWorkingDay).HasDefaultValue(0);
            entity.Property(e => e.TotalWorkingHours)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.SalaryRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalaryRec__UserI__4316F928");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB00AF8CD5557");

            entity.ToTable("Service");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.ServicePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Staff__1788CC4CEA32A9B8");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.ExperienceLevel).HasMaxLength(50);
            entity.Property(e => e.IsWorking).HasDefaultValue(true);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Rating).HasColumnType("decimal(2, 1)");
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__UserId__32E0915F");
        });

        modelBuilder.Entity<StaffLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__StaffLog__5E5486480AB12E52");

            entity.ToTable("StaffLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.StaffLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StaffLog__UserId__4BAC3F29");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4CC4186D9");

            entity.ToTable("Supplier");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Table__7D5F01EEADA701F9");

            entity.ToTable("Table");

            entity.HasIndex(e => e.TableName, "UQ__Table__733652EED732823F").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.QrCode).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.TableName).HasMaxLength(100);
            entity.Property(e => e.TableType).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TableOrder>(entity =>
        {
            entity.HasKey(e => new { e.TableId, e.OrderId }).HasName("PK__TableOrd__916604524DF6EEDB");

            entity.ToTable("TableOrder");

            entity.Property(e => e.IsMainTable).HasDefaultValue(false);
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LeftAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.TableOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableOrde__Order__1D7B6025");

            entity.HasOne(d => d.Table).WithMany(p => p.TableOrders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableOrde__Table__1C873BEC");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BDB6AD83D");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.TransactionCode, "UQ__Transact__D85E7026A042C6BE").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Transacti__Creat__662B2B3B");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Transacti__Suppl__671F4F74");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CF79B6202");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D1053455556BC7").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fullname).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Customer");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkShift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__WorkShif__C0A838811F228524");

            entity.ToTable("WorkShift");

            entity.Property(e => e.AdditionalWork).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ShiftName).HasMaxLength(100);
            entity.Property(e => e.TypeStaff).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkStaff>(entity =>
        {
            entity.HasKey(e => e.WorkStaffId).HasName("PK__WorkStaf__DB73C04AC5D907E8");

            entity.ToTable("WorkStaff");

            entity.HasIndex(e => new { e.UserId, e.ShiftId, e.WorkDay }, "UQ_WorkStaff").IsUnique();

            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
            entity.Property(e => e.DailyTime)
                .HasComputedColumnSql("(datediff(minute,[CheckInTime],[CheckOutTime])/(60.0))", false)
                .HasColumnType("numeric(17, 6)");
            entity.Property(e => e.IsWorking).HasDefaultValue(true);
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Shift).WithMany(p => p.WorkStaffs)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkStaff__Shift__398D8EEE");

            entity.HasOne(d => d.User).WithMany(p => p.WorkStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkStaff__UserI__38996AB5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
