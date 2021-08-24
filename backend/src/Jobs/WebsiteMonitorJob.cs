using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using uptimey.Entities;

namespace uptimey.Jobs
{
    [DisallowConcurrentExecution]
    public class WebsiteMonitorJob : IJob
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WebsiteMonitorJob> _logger;

        public WebsiteMonitorJob(ILogger<WebsiteMonitorJob> logger, DatabaseContext dbContext,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Processing websites");
            var userSites = await _dbContext.UserSites.ToListAsync();

            var client = _httpClientFactory.CreateClient("client");
            var reports = new List<SiteReport>();

            foreach (var userSite in userSites)
            {
                var report = new SiteReport {UserSite = userSite, Id = Guid.NewGuid()};
                var watch = new Stopwatch();
                try
                {
                    watch.Start();
                    await client.GetAsync(userSite.Url);
                }
                catch (Exception ex)
                {
                    report.HasError = true;
                    report.ErrorMessage = ex.Message;
                }

                finally
                {
                    watch.Stop();
                    report.ResponseTime = watch.Elapsed.TotalMilliseconds;
                    report.DateChecked = DateTime.UtcNow;
                    reports.Add(report);
                }
            }

            await _dbContext.AddRangeAsync(reports);
            await _dbContext.SaveChangesAsync();
        }
    }
}