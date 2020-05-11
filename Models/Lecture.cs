using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class Lecture
    {
        public int Id { get; set; }
        public int Course_Id { get; set; }
        public string Text_Content { get; set; }
        public string Index { get; set; }
        public string Lecture_Title { get; set; }
        public string Owner_ID { get; set; }
        public bool Is_Zoom_Enabled { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartTime { get; set; }

        public string Zoom_Invite_Link { get; set; }

    }
}
