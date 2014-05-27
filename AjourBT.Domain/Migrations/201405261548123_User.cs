namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employees", "PositionID", "dbo.Positions");
            DropIndex("dbo.Employees", new[] { "PositionID" });
            AddColumn("dbo.Employees", "IsUserOnly", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Employees", "DateEmployed", c => c.DateTime());
            AlterColumn("dbo.Employees", "PositionID", c => c.Int());
            AddForeignKey("dbo.Employees", "PositionID", "dbo.Positions", "PositionID");
            CreateIndex("dbo.Employees", "PositionID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Employees", new[] { "PositionID" });
            DropForeignKey("dbo.Employees", "PositionID", "dbo.Positions");
            AlterColumn("dbo.Employees", "PositionID", c => c.Int(nullable: false));
            AlterColumn("dbo.Employees", "DateEmployed", c => c.DateTime(nullable: false));
            DropColumn("dbo.Employees", "IsUserOnly");
            CreateIndex("dbo.Employees", "PositionID");
            AddForeignKey("dbo.Employees", "PositionID", "dbo.Positions", "PositionID", cascadeDelete: true);
        }
    }
}
