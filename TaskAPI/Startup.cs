using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using System.Text;
using Hangfire.Common;
using TaskAPI.Data;
using TaskAPI.Interfaces;
using TaskAPI.Services;
using Microsoft.AspNetCore.Builder;
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

        services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));
        //services.AddHangfireServer(); !

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<TaskSchedulerService>();

        var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services.AddControllers();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
    {
        app.UseHangfireDashboard();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        recurringJobManager.AddOrUpdate(
            "TaskReminderJob",
            Job.FromExpression(() => serviceProvider.GetService<TaskSchedulerService>().CheckTasksDueSoon(24)),
            Cron.Hourly());
    }
}
