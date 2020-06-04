using System;
using System.ComponentModel.DataAnnotations;

namespace eLearning.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime StartingDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime EndingDate { get; set; }

        public bool HasCertificate { get; set; }

        public byte[] CourseImage { get; set; }
    }
}
