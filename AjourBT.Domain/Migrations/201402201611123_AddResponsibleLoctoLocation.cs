namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class responsibleForLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "ResponsibleForLoc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "ResponsibleForLoc");
        }
    }
}
