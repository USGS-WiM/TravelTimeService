using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TravelTimeDB.Test
{
    [TestClass]
    public class TravelTimeDBTest
    {
        private string connectionstring = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build().GetConnectionString("wateruseConnection");

        [TestMethod]
        public void ConnectionTest()
        {
            using (TravelTimeDBContext context = new TravelTimeDBContext(new DbContextOptionsBuilder<TravelTimeDBContext>().UseNpgsql(this.connectionstring).Options))
            {
                try
                {
                    if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) throw new Exception("db does ont exist");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
            }
        }
        [TestMethod]
        public void QueryTest()
        {
            using (TravelTimeDBContext context = new TravelTimeDBContext(new DbContextOptionsBuilder<TravelTimeDBContext>().UseNpgsql(this.connectionstring).Options))
            {
                try
                {
                    var testQuery = context.TravelTime.ToList();
                    Assert.IsNotNull(testQuery, testQuery.Count.ToString());
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
                finally
                {
                }

            }
        }
    }
}
