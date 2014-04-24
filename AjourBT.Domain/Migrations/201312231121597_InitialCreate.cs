namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EID = c.String(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                        DateEmployed = c.DateTime(nullable: false),
                        PositionID = c.Int(nullable: false),
                        BirthDay = c.DateTime(),
                        Comment = c.String(),
                        FullNameUk = c.String(),
                        DateDismissed = c.DateTime(),
                        IsManager = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        BTRestrictions = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID)
                .ForeignKey("dbo.Positions", t => t.PositionID, cascadeDelete: true)
                .Index(t => t.DepartmentID)
                .Index(t => t.PositionID);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        PositionID = c.Int(nullable: false, identity: true),
                        TitleUk = c.String(nullable: false),
                        TitleEn = c.String(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.PositionID);
            
            CreateTable(
                "dbo.BusinessTrips",
                c => new
                    {
                        BusinessTripID = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        OldStartDate = c.DateTime(),
                        EndDate = c.DateTime(nullable: false),
                        OldEndDate = c.DateTime(),
                        OrderStartDate = c.DateTime(),
                        OrderEndDate = c.DateTime(),
                        DaysInBtForOrder = c.Int(),
                        Status = c.Int(nullable: false),
                        LocationID = c.Int(nullable: false),
                        OldLocationID = c.Int(nullable: false),
                        OldLocationTitle = c.String(),
                        EmployeeID = c.Int(nullable: false),
                        Purpose = c.String(),
                        Manager = c.String(),
                        Responsible = c.String(),
                        Comment = c.String(),
                        RejectComment = c.String(),
                        CancelComment = c.String(),
                        BTMComment = c.String(),
                        Habitation = c.String(),
                        HabitationConfirmed = c.Boolean(nullable: false),
                        Flights = c.String(),
                        FlightsConfirmed = c.Boolean(nullable: false),
                        Invitation = c.Boolean(nullable: false),
                        LastCRUDedBy = c.String(),
                        LastCRUDTimestamp = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.BusinessTripID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .Index(t => t.LocationID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.LocationID);
            
            CreateTable(
                "dbo.Visas",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        VisaType = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Days = c.Int(nullable: false),
                        DaysUsedInBT = c.Int(),
                        DaysUsedInPrivateTrips = c.Int(),
                        Entries = c.Int(nullable: false),
                        EntriesUsedInBT = c.Int(),
                        EntriesUsedInPrivateTrips = c.Int(),
                        CorrectionForVisaDays = c.Int(),
                        CorrectionForVisaEntries = c.Int(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.PrivateTrips",
                c => new
                    {
                        PrivateTripID = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.PrivateTripID)
                .ForeignKey("dbo.Visas", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.VisaRegistrationDates",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        VisaType = c.String(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Permits",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        IsKartaPolaka = c.Boolean(nullable: false),
                        Number = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CancelRequestDate = c.DateTime(),
                        ProlongRequestDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Passports",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        EndDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageID = c.Int(nullable: false, identity: true),
                        Role = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        Link = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        ReplyTo = c.String(),
                    })
                .PrimaryKey(t => t.MessageID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Passports", new[] { "EmployeeID" });
            DropIndex("dbo.Permits", new[] { "EmployeeID" });
            DropIndex("dbo.VisaRegistrationDates", new[] { "EmployeeID" });
            DropIndex("dbo.PrivateTrips", new[] { "EmployeeID" });
            DropIndex("dbo.Visas", new[] { "EmployeeID" });
            DropIndex("dbo.BusinessTrips", new[] { "EmployeeID" });
            DropIndex("dbo.BusinessTrips", new[] { "LocationID" });
            DropIndex("dbo.Employees", new[] { "PositionID" });
            DropIndex("dbo.Employees", new[] { "DepartmentID" });
            DropForeignKey("dbo.Passports", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Permits", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.VisaRegistrationDates", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.PrivateTrips", "EmployeeID", "dbo.Visas");
            DropForeignKey("dbo.Visas", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.BusinessTrips", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.BusinessTrips", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.Employees", "PositionID", "dbo.Positions");
            DropForeignKey("dbo.Employees", "DepartmentID", "dbo.Departments");
            DropTable("dbo.Messages");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Passports");
            DropTable("dbo.Permits");
            DropTable("dbo.VisaRegistrationDates");
            DropTable("dbo.PrivateTrips");
            DropTable("dbo.Visas");
            DropTable("dbo.Locations");
            DropTable("dbo.BusinessTrips");
            DropTable("dbo.Positions");
            DropTable("dbo.Departments");
            DropTable("dbo.Employees");
        }
    }
}
