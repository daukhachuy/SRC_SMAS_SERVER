using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class RestaurantDbContext : DbContext
{
    public RestaurantDbContext()
    {
    }

    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
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
    //        => optionsBuilder.UseSqlServer("Server=tcp:restaurant-sv.database.windows.net,1433;Initial Catalog=RestaurantDB;User ID=sqladmin;Password=Huy872002!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
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
            entity.HasKey(e => e.LogId).HasName("PK__AdminLog__5E5486480DE4FFC6");

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
                .HasConstraintName("FK__AdminLog__UserId__1332DBDC");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E3011989CD4");

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
                .HasConstraintName("FK__Blog__AuthorId__3587F3E0");
        });

        modelBuilder.Entity<BookEvent>(entity =>
        {
            entity.HasKey(e => e.BookEventId).HasName("PK__BookEven__01FE44D77604C9C2");

            entity.ToTable("BookEvent");

            entity.HasIndex(e => e.BookingCode, "UQ__BookEven__C6E56BD5C9C3233B").IsUnique();

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
                .HasConstraintName("FK__BookEvent__Confi__58D1301D");

            entity.HasOne(d => d.Contract).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__BookEvent__Contr__59C55456");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Custo__57DD0BE4");

            entity.HasOne(d => d.Event).WithMany(p => p.BookEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Event__56E8E7AB");
        });

        modelBuilder.Entity<BookEventService>(entity =>
        {
            entity.HasKey(e => new { e.BookEventId, e.ServiceId }).HasName("PK__BookEven__BDAFFFD77E3A0B74");

            entity.ToTable("BookEventService");

            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.BookEventServices)
                .HasForeignKey(d => d.BookEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__BookE__6442E2C9");

            entity.HasOne(d => d.Service).WithMany(p => p.BookEventServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookEvent__Servi__65370702");
        });

        modelBuilder.Entity<Buffet>(entity =>
        {
            entity.HasKey(e => e.BuffetId).HasName("PK__Buffet__2A9FCF85ADF63978");

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
                .HasConstraintName("FK__Buffet__CreatedB__214BF109");
        });

        modelBuilder.Entity<BuffetFood>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.BuffetId }).HasName("PK__BuffetFo__C7C44F133E5D2C59");

            entity.ToTable("BuffetFood");

            entity.Property(e => e.IsUnlimited).HasDefaultValue(true);
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Buffet).WithMany(p => p.BuffetFoods)
                .HasForeignKey(d => d.BuffetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BuffetFoo__Buffe__27F8EE98");

            entity.HasOne(d => d.Food).WithMany(p => p.BuffetFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BuffetFoo__FoodI__2704CA5F");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B5A1EEFD1");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__737584F65C55B7FB").IsUnique();

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
            entity.HasKey(e => e.ComboId).HasName("PK__Combo__DD42582E5E576C8E");

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
                .HasConstraintName("FK__Combo__CreatedBy__14E61A24");
        });

        modelBuilder.Entity<ComboFood>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.ComboId }).HasName("PK__ComboFoo__78B996693D9601BC");

            entity.ToTable("ComboFood");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboFoods)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboFood__Combo__1B9317B3");

            entity.HasOne(d => d.Food).WithMany(p => p.ComboFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboFood__FoodI__1A9EF37A");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34698D681640");

            entity.ToTable("Contract");

            entity.HasIndex(e => e.ContractCode, "UQ__Contract__CBECF833814784BA").IsUnique();

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
                .HasConstraintName("FK__Contract__BookEv__5BAD9CC8");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contract__Custom__4A8310C6");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__C050D8771D0AA682");

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
                .HasConstraintName("FK__Conversat__UserI__2180FB33");
        });

        modelBuilder.Entity<CustomerFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Customer__6A4BEDD689CAA7E8");

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
                .HasConstraintName("FK__CustomerF__Order__084B3915");

            entity.HasOne(d => d.RespondedByNavigation).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.RespondedBy)
                .HasConstraintName("FK__CustomerF__Respo__093F5D4E");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerF__UserI__075714DC");
        });

        modelBuilder.Entity<DeliveryDetail>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Delivery__626D8FCE32E37E20");

            entity.ToTable("DeliveryDetail");

            entity.HasIndex(e => e.DeliveryCode, "UQ__Delivery__9EB035EC64128B90").IsUnique();

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
                .HasConstraintName("FK__DeliveryD__Assig__6BE40491");

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__DeliveryD__Order__6319B466");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D96B38620D1");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.Code, "UQ__Discount__A25C5AA73F662EB1").IsUnique();

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
                .HasConstraintName("FK__Discount__Create__4F12BBB9");
        });

        modelBuilder.Entity<EmployeeAnnouncement>(entity =>
        {
            entity.HasKey(e => e.EmployeeAnnouncementId).HasName("PK__Employee__AC9E1FD8F6C14FB8");

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
                .HasConstraintName("FK__EmployeeA__Creat__1CBC4616");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C81053A226F3");

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
                .HasConstraintName("FK__Event__CreatedBy__503BEA1C");
        });

        modelBuilder.Entity<EventFood>(entity =>
        {
            entity.HasKey(e => new { e.BookEventId, e.FoodId }).HasName("PK__EventFoo__A9A89FE923886DB2");

            entity.ToTable("EventFood");

            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.BookEvent).WithMany(p => p.EventFoods)
                .HasForeignKey(d => d.BookEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventFood__BookE__7720AD13");

            entity.HasOne(d => d.Food).WithMany(p => p.EventFoods)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventFood__FoodI__7814D14C");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PK__Food__856DB3EBCDB354E8");

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
                        .HasConstraintName("FK__FoodCateg__Categ__7FEAFD3E"),
                    l => l.HasOne<Food>().WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__FoodCateg__FoodI__7EF6D905"),
                    j =>
                    {
                        j.HasKey("FoodId", "CategoryId").HasName("PK__FoodCate__24FD204BCF6880ED");
                        j.ToTable("FoodCategory");
                    });
        });

        modelBuilder.Entity<FoodRecipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__FoodReci__FDD988B080388EFD");

            entity.ToTable("FoodRecipe");

            entity.HasIndex(e => new { e.FoodId, e.IngredientId }, "UQ_FoodRecipe").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByStaff).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.CreatedByStaffId)
                .HasConstraintName("FK__FoodRecip__Creat__0E391C95");

            entity.HasOne(d => d.Food).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodRecip__FoodI__0C50D423");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.FoodRecipes)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoodRecip__Ingre__0D44F85C");
        });

        modelBuilder.Entity<ImExport>(entity =>
        {
            entity.HasKey(e => e.ImExportId).HasName("PK__ImExport__3EC3CB58C8170B2A");

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
                .HasConstraintName("FK__ImExport__Create__3FD07829");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.ImExports)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImExport__Ingred__3EDC53F0");

            entity.HasOne(d => d.Inventory).WithMany(p => p.ImExports)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImExport__Invent__3DE82FB7");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB25A05A5982E");

            entity.ToTable("Ingredient");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CC79CA2BE0").IsUnique();

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
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6B3DB790001");

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
                .HasConstraintName("FK__Inventory__Ingre__382F5661");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__Inventory__Trans__39237A9A");
        });

        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.HasKey(e => e.InventoryLogId).HasName("PK__Inventor__978570726F3C28F7");

            entity.ToTable("InventoryLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("FK__Inventory__Ingre__467D75B8");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__Inventory__Inven__4589517F");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__UserI__477199F1");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C0C9C74590789");

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
                .HasConstraintName("FK__Message__Convers__282DF8C2");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__SenderI__29221CFB");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1214E09AA8");

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
                .HasConstraintName("FK__Notificat__Sende__2FCF1A8A");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__2EDAF651");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF1DA93542");

            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderCode, "UQ__Order__999B52299448BBF7").IsUnique();

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
                .HasConstraintName("FK__Order__BookEvent__5E54FF49");

            entity.HasOne(d => d.Delivery).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryId)
                .HasConstraintName("FK__Order__DeliveryI__603D47BB");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Order__DiscountI__5F492382");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK__Order__Reservati__5D60DB10");

            entity.HasOne(d => d.ServedByNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ServedBy)
                .HasConstraintName("FK__Order__ServedBy__61316BF4");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__UserId__5C6CB6D7");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681D3AA6F1D");

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
                .HasConstraintName("FK__OrderItem__Buffe__7073AF84");

            entity.HasOne(d => d.Combo).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__OrderItem__Combo__7167D3BD");

            entity.HasOne(d => d.Food).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.FoodId)
                .HasConstraintName("FK__OrderItem__FoodI__6F7F8B4B");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__6E8B6712");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A38C46BB9D4");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.PaymentCode, "UQ__Payment__106D3BA843E1447A").IsUnique();

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
                .HasConstraintName("FK__Payment__Contrac__7FB5F314");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Payment__OrderId__7EC1CEDB");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReceivedBy)
                .HasConstraintName("FK__Payment__Receive__00AA174D");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F24A37BDC5F");

            entity.ToTable("Reservation");

            entity.HasIndex(e => e.ReservationCode, "UQ__Reservat__2081C0BB5E8FA37B").IsUnique();

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
                .HasConstraintName("FK__Reservati__Confi__42E1EEFE");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__UserI__41EDCAC5");
        });

        modelBuilder.Entity<SalaryRecord>(entity =>
        {
            entity.HasKey(e => e.SalaryRecordId).HasName("PK__SalaryRe__F9633938A36C4D3D");

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
                .HasConstraintName("FK__SalaryRec__UserI__0E6E26BF");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB00AC1B7BF8B");

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
            entity.HasKey(e => e.UserId).HasName("PK__Staff__1788CC4C14D89BC0");

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
                .HasConstraintName("FK__Staff__UserId__7E37BEF6");
        });

        modelBuilder.Entity<StaffLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__StaffLog__5E5486484DFA6493");

            entity.ToTable("StaffLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.StaffLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StaffLog__UserId__17036CC0");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4D4635067");

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
            entity.HasKey(e => e.TableId).HasName("PK__Table__7D5F01EEB7B9A2E5");

            entity.ToTable("Table");

            entity.HasIndex(e => e.TableName, "UQ__Table__733652EE0465880A").IsUnique();

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
            entity.HasKey(e => new { e.TableId, e.OrderId }).HasName("PK__TableOrd__9166045279CFDB13");

            entity.ToTable("TableOrder");

            entity.Property(e => e.IsMainTable).HasDefaultValue(false);
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LeftAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.TableOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableOrde__Order__68D28DBC");

            entity.HasOne(d => d.Table).WithMany(p => p.TableOrders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TableOrde__Table__67DE6983");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BCE5934A0");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.TransactionCode, "UQ__Transact__D85E7026BE361898").IsUnique();

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
                .HasConstraintName("FK__Transacti__Creat__318258D2");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Transacti__Suppl__32767D0B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C9156E8E3");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105345D1E3156").IsUnique();

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
            entity.HasKey(e => e.ShiftId).HasName("PK__WorkShif__C0A838811B55D273");

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
            entity.HasKey(e => e.WorkStaffId).HasName("PK__WorkStaf__DB73C04A84BAD5DE");

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
                .HasConstraintName("FK__WorkStaff__Shift__04E4BC85");

            entity.HasOne(d => d.User).WithMany(p => p.WorkStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkStaff__UserI__03F0984C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
