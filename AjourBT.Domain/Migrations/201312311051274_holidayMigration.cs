namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class holidayMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Holidays", "HolidayComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Holidays", "HolidayComment");
        }
    }
}
