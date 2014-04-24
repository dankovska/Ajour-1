namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BeforeAbsenceStart : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Countries", "CountryName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Countries", "CountryName", c => c.String());
        }
    }
}
