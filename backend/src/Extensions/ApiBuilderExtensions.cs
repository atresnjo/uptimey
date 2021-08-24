using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using uptimey.Jobs;

namespace uptimey.Extensions
{
    public static class ApiBuilderExtensions
    {
        public static void ConfigureAuthentication(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Configuration.GetSection(nameof(SupabaseSettings));

            var secret = webApplicationBuilder.Configuration.GetValue<string>("SupabaseSettings:Secret");

            webApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                var signingKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = signingKey,
                    ValidateAudience = true,
                    ValidAudience = "authenticated",
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            webApplicationBuilder.Services.AddAuthorization();
        }

        public static void ConfigureJobScheduling(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = new JobKey("WebsiteMonitorJob");

                q.AddJob<WebsiteMonitorJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("monitor-trigger")
                    .WithCronSchedule("0/10 * * * * ?"));
            });

            webApplicationBuilder.Services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
        }
    }
}