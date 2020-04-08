using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class LicenseKey
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int Course_id { get; set; }

        public bool Used { get; set; }
    }
}
