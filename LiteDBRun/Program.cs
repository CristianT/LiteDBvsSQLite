using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;
using LiteDB;
using Prometheus;

namespace LiteDBRun
{
    class Program
    {
        static void Main()
        {
            try
            {
                var server = new MetricServer(port: 5724);
                server.Start();

                // Run test
                string dbName = Guid.NewGuid().ToString();

                using var db = new LiteDatabase($"Filename={dbName};");
                
                Task.Run(() =>
                {
                    while(true)
                    {
                        DatabaseSize.Set(new FileInfo(dbName).Length);
                        Task.Delay(5000).Wait();
                    }
                });

                // Insert 1M of objects
                var insertCollection = db.GetCollection<ExampleObject>("insertData");
                Log("Insert 1000000 of objects");
                for(int i = 0;i<1000000;i++)
                {
                    var obj = new ExampleObject()
                    {
                        Name = "example name",
                        Value = 1234567890
                    };

                    insertCollection.Insert(obj);
                    InsertObjectCounter.Inc();
                }

                Log("Process finished");

            }
            catch(Exception ex)
            {
                Log($"Error: {ex.Message} - {ex.StackTrace}");
            }
        }

        private static Counter InsertObjectCounter = Metrics.CreateCounter("database_insert_object", "Number of Inserted objects.");
        private static Gauge DatabaseSize = Metrics.CreateGauge("database_size", "Number of Inserted objects.");

        private static void Log(string s)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - {s}");
        }
    }
}
