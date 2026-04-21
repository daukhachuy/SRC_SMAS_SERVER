using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SMAS_API.BackgroundJobs;
using SMAS_API.Helpers;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.AIDTO;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.AdminRepository;
using SMAS_Repositories.AuthRepositories;
using SMAS_Repositories.BlogRepositories;
using SMAS_Repositories.BookEventRepository;
using SMAS_Repositories.BuffetRepositories;
using SMAS_Repositories.CategoryRepositories;
using SMAS_Repositories.ComboRepositories;
using SMAS_Repositories.ContractRepository;
using SMAS_Repositories.ContractWorkflow;
using SMAS_Repositories.ConversationRepositories;
using SMAS_Repositories.CustomerFeedbackRepositories;
using SMAS_Repositories.DiscountRepositories;
using SMAS_Repositories.EventRepositories;
using SMAS_Repositories.FoodRepositories;
using SMAS_Repositories.IngredientReposotories;
using SMAS_Repositories.Inventoryrepositories;
using SMAS_Repositories.ManagerRepositories;
using SMAS_Repositories.Notificationrepositories;
using SMAS_Repositories.OrderRepositories;
using SMAS_Repositories.PdfRepositories;
using SMAS_Repositories.ReservationRepositories;
using SMAS_Repositories.SalaryRepository;
using SMAS_Repositories.ServiceRepositories;
using SMAS_Repositories.StaffRepository;
using SMAS_Repositories.TableRepository;
using SMAS_Repositories.WorkStaffRepository;
using SMAS_Services.AdminServices;
using SMAS_Services.AiBaseServices;
using SMAS_Services.AuthServices;
using SMAS_Services.BlogServices;
using SMAS_Services.BookEventService;
using SMAS_Services.BufferServices;
using SMAS_Services.CategoryServices;
using SMAS_Services.ComboServices;
using SMAS_Services.ContractService;
using SMAS_Services.ContractWorkflow;
using SMAS_Services.ConversationServices;
using SMAS_Services.CustomerFeedbackServices;
using SMAS_Services.DiscountServices;
using SMAS_Services.EmailServices;
using SMAS_Services.EventServices;
using SMAS_Services.FoodServices;
using SMAS_Services.IngredientServices;
using SMAS_Services.InventoryServices;
using SMAS_Services.ManagerServices;
using SMAS_Services.NotificationServices;
using SMAS_Services.OrderItemServices;
using SMAS_Services.OrderServices;
using SMAS_Services.PaymentServices;
using SMAS_Services.PdfServices;
using SMAS_Services.ReservationServices;
using SMAS_Services.SalaryService;
using SMAS_Services.ServiceServices;
using SMAS_Services.StaffService;
using SMAS_Services.TableService;
using System.Security.Claims;
using System.Text;
using SMAS_API.Hubs;
using SMAS_Services.Realtime;
using static SMAS_DataAccess.DAO.AdminDao;

namespace SMAS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<RestaurantDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStringDB")));
            // Bind JwtSettings
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings")
            );

            builder.Services.Configure<GeminiSettings>(
            builder.Configuration.GetSection("Gemini"));

            var jwtSettings = builder.Configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        //"https://brave-hill-0480d9600.1.azurestaticapps.net",
                        "https://zealous-sky-0d8578e00.2.azurestaticapps.net",
                        "http://localhost:3000",
                        "https://samsrestaurant.app"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });


            //   [Authorize(Roles = "Admin,Customer,Waiter,Kitchen,Manager")]
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Input JWT : Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                 }
                            },
                        new string[] {}
                    }
                 });
            });
            //builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<AuthDAO>();
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<IUserRepositories, UserRepositories>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<FoodDAO>();
            builder.Services.AddScoped<IFoodRepository, FoodRepository>();
            builder.Services.AddScoped<IFoodService, FoodService>();

            builder.Services.AddScoped<EventDAO>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IEventService, EventService>();

            builder.Services.AddScoped<ServiceDAO>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IServiceService, ServiceService>();

            builder.Services.AddScoped<ContractDAO>();
            builder.Services.AddScoped<PaymentDAO>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IContractRepository, ContractRepository>();
            builder.Services.AddScoped<IContractService, ContractService>();
            builder.Services.AddScoped<IContractWorkflowRepository, ContractWorkflowRepository>();
            builder.Services.AddScoped<IContractWorkflowService, ContractWorkflowService>();

            builder.Services.AddScoped<BookEventDAO>();
            builder.Services.AddScoped<IBookEventRepository, BookEventRepository>();
            builder.Services.AddScoped<IBookEventService, SMAS_Services.BookEventService.BookEventService>();

            builder.Services.AddScoped<ComboDAO>();
            builder.Services.AddScoped<IComboRepository, ComboRepository>();
            builder.Services.AddScoped<IComboService, ComboService>();

            builder.Services.AddScoped<CustomerFeedbackDAO>();
            builder.Services.AddScoped<ICustomerFeedbackRepository, CustomerFeedbackRepository>();
            builder.Services.AddScoped<ICustomerFeedbackService, CustomerFeedbackService>();

            builder.Services.AddScoped<BlogDAO>();
            builder.Services.AddScoped<IBlogRepository, BlogRepository>();
            builder.Services.AddScoped<IBlogServices, BlogService>();

            builder.Services.AddScoped<DiscountDao>();
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();

            builder.Services.AddScoped<CategoryDAO>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<BuffetDAO>();
            builder.Services.AddScoped<IBuffetRepository, BuffetRepository>();
            builder.Services.AddScoped<IBufferServices, BufferService>();

            builder.Services.AddScoped<SalaryRecordDAO>();
            builder.Services.AddScoped<ISalaryRecordRepository, SalaryRecordRepository>();
            builder.Services.AddScoped<ISalaryRecordService, SalaryRecordService>();

            builder.Services.AddScoped<WorkStaffDAO>();
            builder.Services.AddScoped<IWorkStaffRepository, WorkStaffRepository>();
            builder.Services.AddScoped<IWorkStaffService, WorkStaffService>();

            builder.Services.AddScoped<StaffProfileDAO>();
            builder.Services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();
            builder.Services.AddScoped<IStaffProfileService, StaffProfileService>();


            builder.Services.AddScoped<ReservationDAO>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.AddScoped<OrderDAO>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<OrderItemDAO>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();

            builder.Services.AddScoped<ManagerDAO>();
            builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
            builder.Services.AddScoped<IManagerService, ManagerService>();

            builder.Services.AddScoped<IngredientDAO>();
            builder.Services.AddScoped<IIngredientReposotory, IngredientReposotory>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();


            builder.Services.AddScoped<InventoryDAO>();
            builder.Services.AddScoped<IInventoryrepository, Inventoryrepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();

            builder.Services.AddScoped<NotificationDAO>();
            builder.Services.AddScoped<INotificationrepository, Notificationrepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
            builder.Services.AddHostedService<ContractDepositExpirationHostedService>();
            builder.Services.AddHostedService<AutoCancelExpiredDeliveryOrdersJob>();
            builder.Services.AddHostedService<UpcomingBookEventReminderHostedService>();
            builder.Services.AddHostedService<MonthlySalaryCalculationJob>();
            builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection(PayOSSettings.SectionName));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddScoped<AdminDAO>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();



            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<TableDAO>();
            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<ITableService, TableService>();
            builder.Services.AddSingleton<ITableTokenHelper, TableTokenHelper>();

            builder.Services.AddScoped<IAIService, AIService>();
            builder.Services.AddScoped<IAIAnalysisServices, AIAnalysisServices>();

            builder.Services.AddScoped<ConversationDAO>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IConversationService, ConversationService>();

            builder.Services.AddScoped<PdfDao>();
            builder.Services.AddScoped<IPdfRepository,PdfRepository>();
            builder.Services.AddScoped<IPdfService, PdfService>();

            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

            builder.Services.AddSignalR();
            builder.Services.AddScoped<IKitchenNotifier, KitchenNotifier>();
            builder.Services.AddScoped<IChatNotifier, ChatNotifier>();

            builder.Services.AddScoped<TokenService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<KitchenHub>("/hubs/kitchen");
            app.MapHub<ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}
