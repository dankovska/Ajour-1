namespace AjourBT.Domain.Migrations
{
    using AjourBT.Domain.Concrete;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    
    public partial class JourneyEntity : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Journeys",
            //    c => new
            //        {
            //            JourneyID = c.Int(nullable: false, identity: true),
            //            BusinessTripID = c.Int(nullable: false),
            //            Date = c.DateTime(nullable: false),
            //            ReclaimDate = c.DateTime(),
            //            DayOff = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.JourneyID)
            //    .ForeignKey("dbo.BusinessTrips", t => t.BusinessTripID, cascadeDelete: true)
            //    .Index(t => t.BusinessTripID);
            
        }
        
        public override void Down()
        {
            //DropIndex("dbo.Journeys", new[] { "BusinessTripID" });
            //DropForeignKey("dbo.Journeys", "BusinessTripID", "dbo.BusinessTrips");
            //DropTable("dbo.Journeys");
        }
    }
}
