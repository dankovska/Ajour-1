namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Overtime : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Overtimes",
                c => new
                    {
                        OvertimeID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReclaimDate = c.DateTime(),
                        DayOff = c.Boolean(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OvertimeID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Overtimes", new[] { "EmployeeID" });
            DropForeignKey("dbo.Overtimes", "EmployeeID", "dbo.Employees");
            DropTable("dbo.Overtimes");
        }
    }
}
