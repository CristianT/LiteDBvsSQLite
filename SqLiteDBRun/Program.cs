using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace SqliteDBRun
{
    class Program
    {
        static void Main()
        {
            try
            {
                var server = new MetricServer(port: 5725);
                server.Start();

                // Run test
                string dbName = Guid.NewGuid().ToString();

                using var db = new SampleDBContext();
                
                
                Task.Run(() =>
                {
                    while(true)
                    {
                        if(File.Exists(db.DbName))
                        {
                            DatabaseSize.Set(new FileInfo(db.DbName).Length);
                        }
                        Task.Delay(5000).Wait();
                    }
                });

                Log("Insert 1000000 of objects");
                for(int i = 0;i<1000000;i++)
                {
                    var obj = new ExampleObject()
                    {
                        Name = "example name",
                        Value = 1234567890
                    };

                    db.ExampleObjects.Add(obj);
                    db.SaveChanges();
                    InsertObjectCounter.Inc();
                }

                Log("Process finished");
            }
            catch(Exception ex)
            {
                Log($"Error: {ex.Message} - {ex.StackTrace}");
            }
        }

        private static readonly Counter InsertObjectCounter = Metrics.CreateCounter("database_insert_object", "Number of Inserted objects.");
        private static readonly Gauge DatabaseSize = Metrics.CreateGauge("database_size", "Number of Inserted objects.");

        private static void Log(string s)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - {s}");
        }
    }
    public class SampleDBContext : DbContext
    {
        private static bool _created = false;
        public SampleDBContext()
        {
            this.DbName = Guid.NewGuid().ToString() + ".db";
            
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        public string DbName { get; private set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            optionbuilder.UseSqlite($"Data Source={DbName}");
        }
    
        public DbSet<ExampleObject> ExampleObjects { get; set; }
    }

}
