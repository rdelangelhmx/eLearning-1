using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class LibraryResource
    {
        public int Id { get; set; }
        public string Original_Name { get; set; }
        public string Generated_Name { get; set; }
        public string Category { get; set; }
        public bool isOfficial { get; set; }
    }
}
