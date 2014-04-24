namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sickMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sicknesses",
                c => new
                    {
                        SickID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        SicknessType = c.String(),
                        Workdays = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SickID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Sicknesses", new[] { "EmployeeID" });
            DropForeignKey("dbo.Sicknesses", "EmployeeID", "dbo.Employees");
            DropTable("dbo.Sicknesses");
        }
    }
}
