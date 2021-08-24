using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase.Gotrue;
using uptimey.Entities;

namespace uptimey.Extensions
{
    public static class ApiExtensions
    {
        public static void MapApiRoutes(this WebApplication app)
        {
            app.MapPost("/login", HandleLogin).AllowAnonymous();
            app.MapPost("/signup", HandleSignup).AllowAnonymous();
            app.MapPost("/user/sites", CreateMonitorWebsite).RequireAuthorization();
            app.MapDelete("/user/sites/{id}", DeleteMonitorWebsite).RequireAuthorization();
            app.MapGet("/user/sites", GetMonitorWebsites).RequireAuthorization();
        }

        private static async Task GetMonitorWebsites(HttpContext httpContext, DatabaseContext context)
        {
            var user = httpContext.Items["User"] as User;

            var id = Guid.Parse(user.Id);
            var websites = await context.UserSites.Include(x => x.Reports).Where(x => x.UserId == id).ToListAsync();
            var reports = new List<SiteReportViewModel>();

            foreach (var orderedResults in websites.Select(userSite => userSite.Reports.OrderBy(x => x.DateChecked)))
                reports.AddRange(orderedResults.Select(siteReport => new SiteReportViewModel(siteReport.UserSite.Url,
                    siteReport.ResponseTime, siteReport.ErrorMessage, siteReport.DateChecked)).ToList());

            var viewWebsites = websites.Select(x => new UserWebsiteViewModel(x.Id, x.Url)).ToList();
            var response = new GetSitesResponse(viewWebsites, reports);
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsJsonAsync(response);
        }

        private static async Task DeleteMonitorWebsite(HttpContext httpContext, Guid id, DatabaseContext context)
        {
            var website = await context.UserSites.FirstOrDefaultAsync(x => x.Id == id);
            context.Remove(website);
            await context.SaveChangesAsync();
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
        }

        private static async Task CreateMonitorWebsite(HttpContext httpContext, MonitorWebsiteRequest request,
            DatabaseContext context)
        {
            var user = httpContext.Items["User"] as User;
            await context.UserSites.AddAsync(
                new UserSite {Url = request.Url, UserId = Guid.Parse(user.Id), Id = Guid.NewGuid()});
            await context.SaveChangesAsync();
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
        }

        private static async Task HandleLogin(HttpContext httpContext, LoginRequest request, DatabaseContext context)
        {
            if (string.IsNullOrEmpty(request?.Email) || string.IsNullOrEmpty(request.Password))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new {error = "Please provide an email and password"});
                return;
            }

            try
            {
                var response = await Client.Instance.SignIn(request.Email, request.Password);
                httpContext.Response.StatusCode =
                    response.User != null ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new LoginResponse(response.AccessToken));
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        private static async Task HandleSignup(HttpContext httpContext, SignupRequest request, DatabaseContext context)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new {error = "Please provide an email and password"});
                return;
            }

            try
            {
                var response = await Client.Instance.SignUp(request.Email, request.Password);
                httpContext.Response.StatusCode =
                    response.User != null ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new LoginResponse(response.AccessToken));
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task ConfigureSupabase(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection(nameof(SupabaseSettings)));
            var url = builder.Configuration.GetValue<string>("SupabaseSettings:Url");
            var key = builder.Configuration.GetValue<string>("SupabaseSettings:Key");
            await Supabase.Client.InitializeAsync(url, key);
        }
    }

    public record GetSitesResponse(List<UserWebsiteViewModel> Websites, List<SiteReportViewModel> Reports);

    public record SiteReportViewModel(string Url, double ResponseTime, string ErrorMessage, DateTime DateChecked);

    public record UserWebsiteViewModel(Guid Id, string Url);

    public record SignupRequest(string Email, string Password);

    public record MonitorWebsiteRequest(string Url);

    public record LoginRequest(string Email, string Password);

    public record LoginResponse(string AccessToken);

    public class SupabaseSettings
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public string Secret { get; set; }
    }
}