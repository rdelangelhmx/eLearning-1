using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eLearning.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<eLearning.Models.Course> Course { get; set; }
        public DbSet<eLearning.Models.LicenseKey> LicenseKey { get; set; }
        public DbSet<eLearning.Models.UserCourse> UserCourse { get; set; }
        public DbSet<eLearning.Models.Training> Training { get; set; }
        public DbSet<eLearning.Models.Lecture> Lecture { get; set; }
        public DbSet<eLearning.Models.LectureInformation> LectureInformation { get; set; }
        public DbSet<eLearning.Models.CourseResources> CourseResources { get; set; }
        public DbSet<eLearning.Models.LibraryResource> LibraryResources { get; set; }
        public DbSet<eLearning.Models.VideoCourseResource> VideoCourseResources { get; set; }
        public DbSet<eLearning.Models.ContactViewModel> ContactMessages { get; set; }
        public DbSet<eLearning.Models.SignedDiploma> SignedDiplomas { get; set; }
        public DbSet<eLearning.Models.SurveyTaken> SurveyTaken { get; set; }
    }
}
