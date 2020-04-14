using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using eLearning.Models;

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
    }
}
