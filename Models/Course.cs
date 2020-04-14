using System;
using System.ComponentModel.DataAnnotations;

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
