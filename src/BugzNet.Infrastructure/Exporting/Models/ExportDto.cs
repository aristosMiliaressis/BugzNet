using System;
using System.Collections.Generic;
using System.Text;

namespace BugzNet.Infrastructure.Exporting.Models
{
    public class ExportDto
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
    }
}
