namespace AjourBT.Domain.Migrations
{
using AjourBT.Domain.Concrete;
using System;
using System.Linq;
using System.Data.Entity.Migrations;
    
    public partial class Journey : DbMigration
    {
      
        public override void Up()
        {
            AjourDbRepository repository = new AjourDbRepository();

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

            var btList = (from bt in repository.BusinessTrips
                          where bt.OrderStartDate != null && bt.OrderEndDate != null
                          select bt).ToList();

            foreach (var bt in btList)
            {
                repository.CreateJourney(bt);
            }

        }
        
        public override void Down()
        {
            DropIndex("dbo.Journeys", new[] { "BusinessTripID" });
            DropForeignKey("dbo.Journeys", "BusinessTripID", "dbo.BusinessTrips");
            //DropTable("dbo.Journeys");
        }
    }
}
