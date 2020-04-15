using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class CourseResources
    {
        public int Id { get; set; }
        public string Original_Name { get; set;}
        public string Generated_Name { get; set; }
        public int Course_Id { get; set; }
        public int Lecture_Id { get; set; }
    }
}
