using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using uptimey;
using uptimey.Extensions;
using uptimey.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddCors(options =>
    options.AddPolicy("cors", policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
await builder.ConfigureSupabase();
services.AddHttpClient();
builder.ConfigureJobScheduling();
builder.ConfigureAuthentication();
await using var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
app.UseCors("cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapApiRoutes();
app.UseMiddleware<UserAccesorMiddleware>();
await app.RunAsync();