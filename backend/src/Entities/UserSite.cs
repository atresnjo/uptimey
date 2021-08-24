using System;
using System.Collections.Generic;

namespace uptimey.Entities
{
    public class UserSite
    {
        public Guid Id { get; set; }

        public string Url { get; set; }

        public Guid UserId { get; set; }

        public List<SiteReport> Reports { get; set; }
    }
}