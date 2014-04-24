namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Holidays_CheckBox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Holidays", "IsPostponed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Holidays", "IsPostponed");
        }
    }
}
