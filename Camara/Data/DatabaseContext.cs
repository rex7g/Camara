using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoWebCam.Models;
using Microsoft.EntityFrameworkCore;

namespace Camara.Models
{ 
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<ImageStore> ImageStore { get; set; }
    }
}
