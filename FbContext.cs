namespace Parser
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class FbContext : DbContext
    {
        // Your context has been configured to use a 'FbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Parser.FbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'FbContext' 
        // connection string in the application configuration file.
        public FbContext()
            : base("name=FbContext1")
        {
            Database.SetInitializer<FbContext>(new DropCreateDatabaseAlways<FbContext>());
            //Database.SetInitializer<FbContext>(new DropCreateDatabaseIfModelChanges<FbContext>());
        }
        public DbSet<FbThread> FbThreads { get; set; }
        public DbSet<FbMessage> FbMessages { get; set; }
        public DbSet<FbUser> FbUsers { get; set; }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}