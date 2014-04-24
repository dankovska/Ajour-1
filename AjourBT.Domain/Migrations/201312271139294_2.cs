namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(),
                        Comment = c.String(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.CountryID);

            CreateTable(
                "dbo.Holidays",
                c => new
                    {
                        HolidayID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CountryID = c.Int(nullable: false),
                        HolidayDate = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.HolidayID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);

            AddColumn("dbo.Locations", "CountryID", c => c.Int(nullable: false));
            Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Ukraine', '')");
            Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Poland', '')");
            Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Sweden', '')");
            Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Belarus', '')");
            Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 1");
            Sql("UPDATE dbo.Locations SET CountryID = 3 WHERE LocationID = 2");
            Sql("UPDATE dbo.Locations SET CountryID = 4 WHERE LocationID = 3");
            Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 4");
            Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 5");
            Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 6");
            Sql("UPDATE dbo.Locations SET CountryID = 3 WHERE LocationID = 7");
            Sql("UPDATE dbo.Locations SET CountryID = 3 WHERE LocationID = 8");

            //Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Ukraine', '')");
            //Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Poland', '')");
            //Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Sweden', '')");
            //Sql("INSERT INTO Countries (CountryName, Comment) VALUES ('Belarus', '')");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 1");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 2");
            //Sql("UPDATE dbo.Locations SET CountryID = 3 WHERE LocationID = 3");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 4");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 6");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 9");
            //Sql("UPDATE dbo.Locations SET CountryID = 2 WHERE LocationID = 12");
            //Sql("UPDATE dbo.Locations SET CountryID = 3 WHERE LocationID = 14");

            AddForeignKey("dbo.Locations", "CountryID", "dbo.Countries", "CountryID", cascadeDelete: true);
            CreateIndex("dbo.Locations", "CountryID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Holidays", new[] { "CountryID" });
            DropIndex("dbo.Locations", new[] { "CountryID" });
            DropForeignKey("dbo.Holidays", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.Locations", "CountryID", "dbo.Countries");
            DropColumn("dbo.Locations", "CountryID");
            DropTable("dbo.Holidays");
            DropTable("dbo.Countries");
        }
    }
}
