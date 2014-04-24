namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessTrips", "AccComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessTrips", "AccComment");
        }
    }
}
