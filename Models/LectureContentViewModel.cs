using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class LectureContentViewModel
    {
        public Lecture Lecture { get; set; }
        public List<CourseResources> CourseResources { get; set;}
        public List<VideoCourseResource> VideoResources { get; set; }
    }
}
