namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalendarUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CalendarItems", "Location", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CalendarItems", "Location");
        }
    }
}
