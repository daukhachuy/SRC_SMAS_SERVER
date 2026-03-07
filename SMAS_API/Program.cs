
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.AuthRepositories;
using SMAS_Repositories.BlogRepositories;
using SMAS_Repositories.ComboRepositories;
using SMAS_Repositories.CustomerFeedbackRepositories;
using SMAS_Repositories.DiscountRepositories;
using SMAS_Repositories.EventRepositories;
using SMAS_Repositories.FoodRepositories;
using SMAS_Repositories.ServiceRepositories;
using SMAS_Services.AuthServices;
using SMAS_Services.EventServices;
using SMAS_Services.BlogServices;
using SMAS_Services.ComboServices;
using SMAS_Services.CustomerFeedbackServices;
using SMAS_Services.DiscountServices;
using SMAS_Services.EmailServices;

using SMAS_Services.FoodServices;
using SMAS_Services.ServiceServices;
using System.Security.Claims;
using System.Text;
using SMAS_Repositories.CategoryRepositories;
using SMAS_Services.CategoryServices;
using SMAS_Repositories.BuffetRepositories;
using SMAS_Services.BufferServices;
using SMAS_Repositories.ReservationRepositories;
using SMAS_Services.ReservationServices;
using SMAS_Repositories.OrderRepositories;
using SMAS_Services.OrderServices;
using SMAS_Services.PaymentServices;
using SMAS_BusinessObject.DTOs.PayOSDTO;

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

            var jwtSettings = builder.Configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            // Add Authentication
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

            });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "https://brave-hill-0480d9600.1.azurestaticapps.net",
                        "http://localhost:3000"
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

            builder.Services.AddScoped<ReservationDAO>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.AddScoped<OrderDAO>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection(PayOSSettings.SectionName));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
