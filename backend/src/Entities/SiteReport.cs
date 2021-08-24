using System;

namespace uptimey.Entities
{
    public class SiteReport
    {
        public Guid Id { get; set; }
        public double ResponseTime { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public UserSite UserSite { get; set; }

        public DateTime DateChecked { get; set; }
    }
}