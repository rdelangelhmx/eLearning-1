using System;
using System.Collections.Generic;
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
    }
}
