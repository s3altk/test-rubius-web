using System;
using System.Collections.Generic;

namespace TestRubius.Models
{
    public partial class Project
    {
        public Project()
        {
            Record = new HashSet<Record>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Record> Record { get; set; }
    }
}
