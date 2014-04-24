namespace AjourBT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessTrips", "UnitID", c => c.Int(nullable: false));

            CreateTable(
                "dbo.Units",
                c => new
                {
                    UnitID = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false),
                    ShortTitle = c.String(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.UnitID);



            Sql("INSERT INTO Units (Title, ShortTitle) VALUES ('unknown', 'unknown')");
            Sql("INSERT INTO Units (Title, ShortTitle) VALUES ('Business Development Unit', 'BD')");
            Sql("INSERT INTO Units (Title, ShortTitle) VALUES ('EPUA Board', 'EPUA_B')");
            Sql("INSERT INTO Units (Title, ShortTitle) VALUES ('EPOL Board', 'B')");
            Sql("INSERT INTO Units (Title, ShortTitle) VALUES ('Finance Unit', 'F')");

            Sql("UPDATE dbo.BusinessTrips SET UnitID = 1");

            AddForeignKey("dbo.BusinessTrips", "UnitID", "dbo.Units", "UnitID", cascadeDelete: true);
            CreateIndex("dbo.BusinessTrips", "UnitID");
        }

        public override void Down()
        {
            DropIndex("dbo.BusinessTrips", new[] { "UnitID" });
            DropForeignKey("dbo.BusinessTrips", "UnitID", "dbo.Units");
            DropColumn("dbo.BusinessTrips", "UnitID");
            DropTable("dbo.Units");
        }
    }
}
