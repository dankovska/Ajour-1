using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Abstract
{
    public interface IRepository
    {
        IQueryable<Employee> Employees { get; }
        IQueryable<Department> Departments { get; }
        IQueryable<Visa> Visas { get; }
        IQueryable<VisaRegistrationDate> VisaRegistrationDates { get; }
        IQueryable<Permit> Permits { get; }
        IQueryable<BusinessTrip> BusinessTrips { get; }
        IQueryable<Location> Locations { get; }
        IQueryable<IMessage> Messages { get; }
        IQueryable<Passport> Passports { get; }
        IQueryable<PrivateTrip> PrivateTrips { get; }
        IQueryable<Position> Positions { get; }
        IEnumerable<Country> Countries { get; }
        IEnumerable<Holiday> Holidays { get; }
        IEnumerable<Journey> Journeys { get; }
        IEnumerable<CalendarItem> CalendarItems { get; }
        IEnumerable<Unit> Units { get; }
        IEnumerable<Overtime> Overtimes { get; }
        IEnumerable<Vacation> Vacations { get; }
        IEnumerable<Sickness> Sicknesses { get; }
        IEnumerable<User> Users { get; }

        void SaveEmployee(Employee employee);
        void SaveDepartment(Department department);
        void SaveVisa(Visa visa, int id);
        void SaveVisaRegistrationDate(VisaRegistrationDate visaRegistDate, int id);
        void SavePermit(Permit permit, int id);
        void SaveBusinessTrip(BusinessTrip bt);
        void SaveLocation(Location location);
        void SaveRolesForEmployee(string username, string[] roles);
        void SaveMessage(IMessage message);
        void SavePassport(Passport passport);
        void SavePrivateTrip(PrivateTrip pt);
        void SavePosition(Position position);
        void SaveCountry(Country Country);
        void SaveHoliday(Holiday Holiday, DateTime date, int countryID, bool isPostphoned);
        void SaveJourney(Journey Journey);
        void SaveCalendarItem(CalendarItem CalendarItem);
        void SaveUnit(Unit unit);
        void SaveOvertime(Overtime overtime);
        void SaveVacation(Vacation vacation);
        void SaveSick(Sickness sick);
        void SaveUser(User user);

        Employee DeleteEmployee(int employeeID);
        Department DeleteDepartment(int departmentID);
        Visa DeleteVisa(int employeeID);
        VisaRegistrationDate DeleteVisaRegistrationDate(int employeeID);
        Permit DeletePermit(int employeeID);
        BusinessTrip DeleteBusinessTrip(int employeeID);
        Location DeleteLocation(int employeeID);
        void DeleteUser(string username);
        IMessage DeleteMessage(int messageID);
        Passport DeletePassport(int employeeID);
        PrivateTrip DeletePrivateTrip(int employeeID);
        Position DeletePosition(int positionID);
        Country DeleteCountry(int countryID);
        Holiday DeleteHoliday(int holidayID);
     //  Journey DeleteJourney(int journeyID);
        CalendarItem DeleteCalendarItem(int calendarID);
        Unit DeleteUnit(int unitID);
        Overtime DeleteOvertime(int OvertimeID);
        Vacation DeleteVacation(int VacationID);
        Sickness DeleteSick(int SickID); 
        User DeleteUser(int UserId);
    }
}