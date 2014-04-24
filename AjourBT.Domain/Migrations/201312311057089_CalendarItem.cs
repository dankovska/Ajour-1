namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalendarItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CalendarItems",
                c => new
                    {
                        CalendarItemID = c.Int(nullable: false, identity: true),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CalendarItemID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CalendarItems", new[] { "EmployeeID" });
            DropForeignKey("dbo.CalendarItems", "EmployeeID", "dbo.Employees");
            DropTable("dbo.CalendarItems");
        }
    }
}
