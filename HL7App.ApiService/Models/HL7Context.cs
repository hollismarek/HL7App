using Microsoft.EntityFrameworkCore;
using HL7App.Models;

namespace HL7App.ApiService.Models
{
    public class HL7Context : DbContext
    {        
        public HL7Context(DbContextOptions<HL7Context> options) : base(options)
        {
        }
        public DbSet<HL7Message> Messages { get; set; }
    }
}
