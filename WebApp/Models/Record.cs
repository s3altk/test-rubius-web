using System;

namespace TestRubius.Models
{
    public partial class Record
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Comment { get; set; }
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
