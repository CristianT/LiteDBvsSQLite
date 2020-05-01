Description:

I wanted to evaluate the raw speed of inserting records in a file based database from a C# application.
The 2 most interesting alterntives I found are SQLite + Entity Framework, which is a well known and well stablish solution,
and LiteDB, which is the no-SQL alternative written completely in .Net.

Since the evaluated use case is inserting records one by one the code to write is this:

LiteDB:

    using var db = new LiteDatabase($"Filename={dbName};");
    var insertCollection = db.GetCollection<ExampleObject>("insertData");
    for(int i = 0;i<1000000;i++)
    {
        var obj = new ExampleObject()
        {
            Name = "example name",
            Value = 1234567890
        };

        insertCollection.Insert(obj);
    }

SQLite:

    using var db = new SampleDBContext();
    for(int i = 0;i<1000000;i++)
    {
        var obj = new ExampleObject()
        {
            Name = "example name",
            Value = 1234567890
        };

        db.ExampleObjects.Add(obj);
        db.SaveChanges();
    }

Result:
See the image ./DashboardExample/DashboardExample.png

LiteDB Results
It took 3.5 minutes to insert 1M of simple objects.
It inserting rate was:
    Min: 3229 objects/second
    Max: 4655 objects/second
    Avg: 4359 objects/second
There is no tendency observed, the rate remains quite stable, and actually on of the highest rates is at the end of the run.

The final Database size was ~130MB for 1 million records

And the used memory was:
    Min: 14 MB
    Max: 18 MB
    Avg: 16 MB

SQLite Results
I stopped the run before it finished but here is the intermediate result:
It took 35.70 minutes to insert 25687 records, for the 1 million records it would have taken at this rate around 1 day, 
but this is false due to the next data:
It inserting rate was:
    Min: 6 objects/second
    Max: 23 objects/second
    Avg: 12 objects/second

But the minimum is found towards the end and the maximum at the begining, probably being the unique key integrity checking more expensive when there are more registries to check.

The size was 0.65 MB, which would have resulted in 25MB for 1 million records.
The diference is noticeable, probably in LiteDB the Guid is what increases the size, which is probably less % when the data inserted is bigger, and more % when it is smaller.

And the used memory was:
    Min: 1.9 MB
    Max: 17 MB
    Avg: 9 MB

This is not a good indicator since the run did not finished and the memory management in .Net is not predictable.

Conslusions:
- LiteDB is much faster inserting registers one by one.
- SQLite inserting speed decreases when the number of registers increases.
- Probably there is something wrong in the SQLite example and there are optimizations possible (batch inserting is not one of them)
- It is possible that Entity Framework is part of the problem, but that means the development of LiteDB is easier and faster and it is more
difficult to screw up with the performance.


How to use this example:

Prerequisites:
- Docker

Start:
- Start the infrastructure servers:
docker-compose -f "docker-compose-infrastructure.yml" up -d
- Start the runs
docker-compose -f "docker-compose-dbruns.yml" up --build

That will start 4 containers:
- Prometheus
- Grafana
- SqLiteRun
- LiteDBRun

Use and configure:
- Go to http://localhost:3000
- Enter user - password: admin - admin
- Specify a new password for admin
- Click on add data source
- Select prometheus and specify http://prometheus:9090
- Click Save and test
- On the left menu put the mouse on the + icon and click on import
- Click on upload json and upload the file ./DashboardExample/DashboardExample.json
- See the image ./DashboardExample/DashboardExample.png to see how it should look like
