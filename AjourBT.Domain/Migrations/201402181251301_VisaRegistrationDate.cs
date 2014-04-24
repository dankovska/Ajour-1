namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VisaRegistrationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VisaRegistrationDates", "RegistrationTime", c => c.String());
            AddColumn("dbo.VisaRegistrationDates", "City", c => c.String());
            AddColumn("dbo.VisaRegistrationDates", "RegistrationNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VisaRegistrationDates", "RegistrationNumber");
            DropColumn("dbo.VisaRegistrationDates", "City");
            DropColumn("dbo.VisaRegistrationDates", "RegistrationTime");
        }
    }
}
