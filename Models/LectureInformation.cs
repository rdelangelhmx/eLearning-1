using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class LectureInformation
    {
        public int Id { get; set; }
        public int Course_Id { get; set; }
        public string Text_Content { get; set; }
    }
}
