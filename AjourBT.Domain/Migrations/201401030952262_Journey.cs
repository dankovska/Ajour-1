namespace AjourBT.Domain.Migrations
{
    using AjourBT.Domain.Abstract;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
    
    public partial class Journey : DbMigration
    {
        private AjourDbRepository ajourDbRepository = new AjourDbRepository();

        
        public override void Up()
        {
            CreateTable(
                "dbo.Journeys",
                c => new
                    {
                        JourneyID = c.Int(nullable: false, identity: true),
                        BusinessTripID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReclaimDate = c.DateTime(),
                        DayOff = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.JourneyID)
                .ForeignKey("dbo.BusinessTrips", t => t.BusinessTripID, cascadeDelete: true)
                .Index(t => t.BusinessTripID);

            var btsList = (from bt in ajourDbRepository.BusinessTrips
                           where bt.OrderStartDate != null && bt.OrderEndDate != null
                           select bt).ToList();
            
            foreach (var bt in btsList)
            {
                ajourDbRepository.CreateJourney(bt);
            }
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Journeys", new[] { "BusinessTripID" });
            DropForeignKey("dbo.Journeys", "BusinessTripID", "dbo.BusinessTrips");
            DropTable("dbo.Journeys");
        }
    }
}
