using System.Collections.Generic;

namespace TestRubius.Models
{
    public class RecordRange
    {
        public IEnumerable<Record> Records { get; set; }

        public RouteParams RouteParams { get; set; }
    }
}
