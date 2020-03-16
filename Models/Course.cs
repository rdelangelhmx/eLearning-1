using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }
        public bool HasCertificate { get; set; }
    }
}
