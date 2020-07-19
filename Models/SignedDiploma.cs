using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class SignedDiploma
    {
        public int Id { get; set; }

        [DisplayName("Holder's name")]
        public string HolderFullName { get; set; }

        [DisplayName("Date of issue")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMMM/yyyy}")]
        public DateTime DateOfIssue { get; set; }

        [DisplayName("Course name")]
        public string CourseName { get; set; }

        public string TransactionId { get; set; }
    }
}
