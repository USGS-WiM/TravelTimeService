//------------------------------------------------------------------------------
//----- DB Context ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Responsible for interacting with Database 
//
//discussion:   The primary class that is responsible for interacting with data as objects. 
//              The context class manages the entity objects during run time, which includes 
//              populating objects with data from a database, change tracking, and persisting 
//              data to the database.
//              
//
//   

using Microsoft.EntityFrameworkCore;
using TravelTimeDB.Resources;
using Microsoft.EntityFrameworkCore.Metadata;
//specifying the data provider and connection string
namespace TravelTimeDB
{
    public class TravelTimeDBContext:DbContext
    {
        public DbSet<TravelTimeResource> TravelTime { get; set; }

        public TravelTimeDBContext() : base()
        {
        }
        public TravelTimeDBContext(DbContextOptions<TravelTimeDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);             
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning Add connectionstring for migrations
            var connectionstring = "User ID=;Password=;Host=;Port=5432;Database=;Pooling=true;";
            //optionsBuilder.UseNpgsql(connectionstring);
        }
    }
}
