using System;

namespace TestRubius.Models
{
    public class RouteParams
    {
        public Guid ProjectId { get; set; }

        public DateTime StartDatetime { get; set; }

        public DateTime EndDatetime { get; set; }

        public int RecordNumber { get; set; }

        public int Order { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageCount { get; set; }
    }
}
