namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Vacation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vacations",
                c => new
                    {
                        VacationID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VacationID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Vacations", new[] { "EmployeeID" });
            DropForeignKey("dbo.Vacations", "EmployeeID", "dbo.Employees");
            DropTable("dbo.Vacations");
        }
    }
}
