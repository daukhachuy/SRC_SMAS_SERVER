
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.AuthRepositories;
using SMAS_Services.AuthServices;
using System.Text;

namespace SMAS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SmasDatabaseContext>(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();



            
            builder.Services.AddScoped<AuthDAO>();
            builder.Services.AddScoped<IUserRepositories, UserRepositories>();
            builder.Services.AddScoped<IUserServices, UserServices>();

            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

            builder.Services.AddScoped<TokenService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
