using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebMatrix.WebData;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AjourBT.Domain.Concrete
{
    public class AjourDbRepository : IRepository
    {

        public AjourDbRepository()
        {
            context = new AjourDbContext();
        }

        private AjourDbContext context;

        public AjourDbRepository(string connectionString)
        {
            context = new AjourDbContext(connectionString);
        }

        #region Employees
        public IQueryable<Employee> Employees
        {
            get { return context.Employees.Where(e => e.IsUserOnly == false); }
        }

        public void SaveEmployee(Employee employee)
        {
            if (employee.EmployeeID.Equals(default(int)))
            {
                context.Employees.Add(employee);

            }
            else
            {
                Employee emp = context.Employees.Find(employee.EmployeeID);

                if (emp != null)
                {
                    if (!emp.RowVersion.SequenceEqual(employee.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }
                    emp.FirstName = employee.FirstName;
                    emp.LastName = employee.LastName;
                    emp.DepartmentID = employee.DepartmentID;
                    emp.EID = employee.EID;
                    emp.DateEmployed = employee.DateEmployed;
                    emp.DateDismissed = employee.DateDismissed;
                    emp.IsManager = employee.IsManager;
                    emp.BirthDay = employee.BirthDay;
                    emp.Comment = employee.Comment;
                    emp.FullNameUk = employee.FullNameUk;
                    emp.PositionID = employee.PositionID;
                    emp.IsUserOnly = employee.IsUserOnly; 
                }
            }
            context.SaveChanges();
        }

        public Employee DeleteEmployee(int employeeID)
        {
            Employee emp = context.Employees.Find(employeeID);

            if (emp != null)
            {
                context.Employees.Remove(emp);
            }

            context.SaveChanges();

            return emp;
        }

        #endregion

        #region Departments
        public IQueryable<Department> Departments
        {
            get { return context.Departments; }
        }

        public void SaveDepartment(Department department)
        {
            if (department.DepartmentID == 0)
            {
                context.Departments.Add(department);
                context.SaveChanges();
            }
            else
            {
                Department dbEntry = context.Departments.Find(department.DepartmentID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(department.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.DepartmentName = department.DepartmentName;
                    dbEntry.RowVersion = department.RowVersion;
                    context.SaveChanges();
                }
            }

        }

        public Department DeleteDepartment(int departmentID)
        {
            Department dbEntry = context.Departments.Find(departmentID);

            if (dbEntry != null)
            {
                //if (!dbEntry.RowVersion.SequenceEqual(department.RowVersion))
                //{
                //    throw new DbUpdateConcurrencyException();
                //}
                context.Departments.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Visas
        public IQueryable<Visa> Visas
        {
            get { return context.Visas; }
        }

        public void SaveVisa(Visa visa, int id)
        {
            Visa dbEntry = (from v in context.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (dbEntry != null && visa.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(visa.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = visa.EmployeeID;
                dbEntry.VisaType = visa.VisaType;
                dbEntry.StartDate = visa.StartDate;
                dbEntry.DueDate = visa.DueDate;
                dbEntry.Days = visa.Days;

                if (visa.DaysUsedInBT == null)
                    dbEntry.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                    dbEntry.DaysUsedInPrivateTrips = 0;
                else
                    dbEntry.DaysUsedInPrivateTrips = visa.DaysUsedInPrivateTrips;

                if (visa.CorrectionForVisaDays == null)
                    dbEntry.CorrectionForVisaDays = 0;
                else
                    dbEntry.CorrectionForVisaDays = visa.CorrectionForVisaDays;

                dbEntry.Entries = visa.Entries;

                if (visa.EntriesUsedInBT == null)
                    dbEntry.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                    dbEntry.EntriesUsedInPrivateTrips = 0;
                else
                    dbEntry.EntriesUsedInPrivateTrips = visa.EntriesUsedInPrivateTrips;

                if (visa.CorrectionForVisaEntries == null)
                    dbEntry.CorrectionForVisaEntries = 0;
                else
                    dbEntry.CorrectionForVisaEntries = visa.CorrectionForVisaEntries;
            }
            else
            {
                //Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();

                if (visa.DaysUsedInBT == null)
                    visa.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                    visa.DaysUsedInPrivateTrips = 0;
                if (visa.CorrectionForVisaDays == null)
                    visa.CorrectionForVisaDays = 0;

                if (visa.EntriesUsedInBT == null)
                    visa.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                    visa.EntriesUsedInPrivateTrips = 0;
                if (visa.CorrectionForVisaEntries == null)
                    visa.CorrectionForVisaEntries = 0;

                context.Visas.Add(visa);
            }
            context.SaveChanges();
        }

        public Visa DeleteVisa(int employeeID)
        {
            Visa dbEntry = Visas.Where(v => v.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.Visas.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region VisaRegistrationDates
        public IQueryable<VisaRegistrationDate> VisaRegistrationDates
        {
            get { return context.VisaRegistrationDates; }
        }

        public void SaveVisaRegistrationDate(VisaRegistrationDate visaRegistrationDate, int id)
        {
            VisaRegistrationDate dbEntry = (from vrd in context.VisaRegistrationDates where vrd.EmployeeID == visaRegistrationDate.EmployeeID select vrd).FirstOrDefault();

            if (dbEntry != null && visaRegistrationDate.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(visaRegistrationDate.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = visaRegistrationDate.EmployeeID;
                dbEntry.VisaType = visaRegistrationDate.VisaType;
                dbEntry.RegistrationDate = visaRegistrationDate.RegistrationDate;
                dbEntry.City = visaRegistrationDate.City;
                dbEntry.RegistrationNumber = visaRegistrationDate.RegistrationNumber;
                dbEntry.RegistrationTime = visaRegistrationDate.RegistrationTime;
                dbEntry.PaymentDate = visaRegistrationDate.PaymentDate;
                dbEntry.PaymentTime = visaRegistrationDate.PaymentTime;
                dbEntry.PaymentPIN = visaRegistrationDate.PaymentPIN;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();

                context.VisaRegistrationDates.Add(visaRegistrationDate);
            }

            context.SaveChanges();
        }

        public VisaRegistrationDate DeleteVisaRegistrationDate(int employeeID)
        {
            VisaRegistrationDate dbEntry = context.VisaRegistrationDates.Where(vrd => vrd.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.VisaRegistrationDates.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Permits
        public IQueryable<Permit> Permits
        {
            get { return context.Permits; }
        }

        public void SavePermit(Permit permit, int id)
        {
            Permit dbEntry = (from p in context.Permits where p.EmployeeID == permit.EmployeeID select p).FirstOrDefault();

            if (dbEntry != null && permit.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(permit.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = permit.EmployeeID;
                dbEntry.Number = permit.Number;
                dbEntry.StartDate = permit.StartDate;
                dbEntry.EndDate = permit.EndDate;
                dbEntry.IsKartaPolaka = permit.IsKartaPolaka;
                dbEntry.CancelRequestDate = permit.CancelRequestDate;
                dbEntry.ProlongRequestDate = permit.ProlongRequestDate;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                //permit.PermitOf = employee;

                context.Permits.Add(permit);
            }
            context.SaveChanges();
        }

        public Permit DeletePermit(int employeeID)
        {
            Permit dbEntry = context.Permits.Where(p => p.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.Permits.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region BusinessTrips
        public IQueryable<BusinessTrip> BusinessTrips
        {
            get { return context.BusinessTrips; }
        }


        int DaysBetweenStartDateAndOrderStartDate = new int();
        int DaysBetweenOrderEndDateAndEndDate = new int();


        public void SaveBusinessTrip(BusinessTrip bt)
        {
            if (!CheckBTCreationPossibility(bt))
            {
                throw new VacationAlreadyExistException();
            }

            BusinessTrip dbEntry = (from b in BusinessTrips where b.BusinessTripID == bt.BusinessTripID select b).FirstOrDefault();

            if (dbEntry != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(bt.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                if ((dbEntry.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (dbEntry.Status != BTStatus.Cancelled) && (dbEntry.Status != bt.Status))
                {
                    DeleteBusinessTripCalendarItem(bt, dbEntry);
                }

                else if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled) && (dbEntry.Status != bt.Status))
                {
                    CreateBusinessTripCalendarItem(bt, dbEntry);
                }

                dbEntry.StartDate = (bt.StartDate == default(DateTime)) ? dbEntry.StartDate : bt.StartDate;
                dbEntry.EndDate = bt.EndDate == default(DateTime) ? dbEntry.EndDate : bt.EndDate;
                dbEntry.Status = bt.Status;
                dbEntry.LocationID = bt.LocationID == 0 ? dbEntry.LocationID : bt.LocationID;
                dbEntry.Comment = bt.Comment;
                dbEntry.CancelComment = bt.CancelComment;
                dbEntry.RejectComment = bt.RejectComment;
                dbEntry.Flights = bt.Flights;
                dbEntry.FlightsConfirmed = bt.FlightsConfirmed;
                dbEntry.Habitation = bt.Habitation;
                dbEntry.HabitationConfirmed = bt.HabitationConfirmed;
                dbEntry.Invitation = bt.Invitation;
                dbEntry.Manager = (bt.Manager == null) ? "" : bt.Manager;
                dbEntry.OldStartDate = (bt.OldStartDate == default(DateTime)) ? dbEntry.OldStartDate : bt.OldStartDate;
                dbEntry.OldEndDate = (bt.OldEndDate == default(DateTime)) ? dbEntry.OldEndDate : bt.OldEndDate;
                dbEntry.OldLocationID = bt.OldLocationID == 0 ? dbEntry.OldLocationID : bt.OldLocationID;
                dbEntry.OldLocationTitle = (bt.OldLocationTitle == null) ? dbEntry.OldLocationTitle : bt.OldLocationTitle;
                dbEntry.Purpose = (bt.Purpose == null) ? "" : bt.Purpose;
                dbEntry.Responsible = (bt.Responsible == null) ? "" : bt.Responsible;
                dbEntry.LastCRUDedBy = (bt.LastCRUDedBy == null) ? dbEntry.LastCRUDedBy : bt.LastCRUDedBy;
                dbEntry.LastCRUDTimestamp = (bt.LastCRUDTimestamp == default(DateTime)) ? dbEntry.LastCRUDTimestamp : bt.LastCRUDTimestamp;
                dbEntry.BTMComment = bt.BTMComment;
                dbEntry.AccComment = bt.AccComment;
                dbEntry.UnitID = bt.UnitID == 0 ? dbEntry.UnitID : bt.UnitID;

                #region CreateJourney
                if((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled))
                {
                    if (bt.OrderStartDate != null && bt.OrderEndDate != null)
                    {
                        BusinessTrip existingBtsWithSameOrderDates;

                        existingBtsWithSameOrderDates = (from b in context.BusinessTrips
                                                         where b.EmployeeID == bt.EmployeeID
                                                         && b.BusinessTripID != bt.BusinessTripID
                                                         && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                                         && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                                         select b).FirstOrDefault();

                        if (existingBtsWithSameOrderDates == null)
                        {
                            if (dbEntry.OrderStartDate != null && dbEntry.OrderEndDate != null)
                            {
                                List<int> journeysID = new List<int>();
                                foreach (Journey jour in dbEntry.Journeys)
                                {
                                    journeysID.Add(jour.JourneyID);
                                }

                                foreach (int id in journeysID)
                                {
                                    DeleteJourney(id);
                                }
                            }
                            CreateJourney(bt);
                        }
                        else
                        {
                            var bts = (from b in context.BusinessTrips
                                       where b.EmployeeID == bt.EmployeeID
                                             && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                             && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                       select b).ToList();

                            List<int> journeysID = new List<int>();
                            List<CalendarItem> calendarItems = new List<CalendarItem>();

                            //BusinessTrip bussinesTrip = (from b in context.BusinessTrips
                            //                             where b.BusinessTripID == bt.BusinessTripID
                            //                             select b).FirstOrDefault();

                            //bts.Add(bussinesTrip);

                            foreach (BusinessTrip b in bts)
                            {
                                if (b.Journeys != null)
                                {
                                    foreach (Journey jour in b.Journeys)
                                    {
                                        journeysID.Add(jour.JourneyID);
                                    }
                                }
                            }



                            //foreach (int id in journeysID)
                            //{
                            //    DeleteJourney(id);

                            //}
                            foreach (BusinessTrip bTrip in bts)
                            {
Employee employee = (from emp in Employees where emp.EmployeeID == bTrip.EmployeeID select emp).FirstOrDefault();
                                if(GetBTCalendarItem(bTrip, employee)!=null)
                                { 
                                DeleteBusinessTripCalendarItem(bTrip, bTrip);
                                CreateBusinessTripCalendarItem(bTrip, bTrip);
                                }

                            }

                            foreach (int id in journeysID)
                            {
                                DeleteJourney(id);

                            }

                            context.SaveChanges();
                            CreateJourneyFromPairedBts(bts);
                        }
                    }
                }

                #endregion

                dbEntry.OrderStartDate = (bt.OrderStartDate == null) ? dbEntry.OrderStartDate : bt.OrderStartDate;
                dbEntry.OrderEndDate = (bt.OrderEndDate == null) ? dbEntry.OrderEndDate : bt.OrderEndDate;

                dbEntry.OrderStartDate = bt.OrderStartDate;
                dbEntry.OrderEndDate = bt.OrderEndDate;
                dbEntry.DaysInBtForOrder = bt.DaysInBtForOrder;
                dbEntry.RowVersion = bt.RowVersion;
            }
            else
            {
                context.BusinessTrips.Add(bt);
            }
            context.SaveChanges();

        }

        private void CreateBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            Employee employee = (from emp in Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = new CalendarItem();
                item.Employee = employee;
                item.EmployeeID = employee.EmployeeID;
                item.From = bt.StartDate;
                item.To = bt.EndDate;
                item.Location = bt.Location.Title;
                item.Type = CalendarItemType.BT;

                employee.CalendarItems.Add(item);
                SaveCalendarItem(item);
            }
        }

        private void DeleteBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            //delete CalendarItem
            Employee employee = (from emp in Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = GetBTCalendarItem(dbEntry, employee);

                List<Journey> journey = GetJourneysForBusinessTrip(bt);

                List<CalendarItem> items = GetJourneyCalendarItem(employee);

                if (item != null)
                {
                    employee.CalendarItems.Remove(item);
                    DeleteCalendarItem(item.CalendarItemID);
                }

                DeleteJourneyCalendarItems(employee, journey, items);

            }
        }

        private void DeleteJourneyCalendarItems(Employee employee, List<Journey> journey, List<CalendarItem> items)
        {
            foreach (CalendarItem it in items)
            {
                foreach (Journey j in journey)
                {
                    if (it.From == j.Date && it.To == j.Date)
                    {
                        employee.CalendarItems.Remove(it);
                        DeleteCalendarItem(it.CalendarItemID);
                    }
                }
            }
        }

        private List<CalendarItem> GetJourneyCalendarItem(Employee employee)
        {
            List<CalendarItem> items = (from it in CalendarItems
                                        where
                                            it.EmployeeID == employee.EmployeeID &&
                                            it.Type == CalendarItemType.Journey
                                        select it).ToList();
            return items;
        }

        private List<Journey> GetJourneysForBusinessTrip(BusinessTrip bt)
        {
            List<Journey> journey = (from journ in Journeys
                                     where
                                         journ.BusinessTripID == bt.BusinessTripID
                                     select journ).ToList();
            return journey;
        }

        private static CalendarItem GetBTCalendarItem(BusinessTrip dbEntry, Employee employee)
        {
            CalendarItem item = (from i in employee.CalendarItems
                                 where i.Type == CalendarItemType.BT &&
                                     i.Location == dbEntry.Location.Title &&
                                     i.From == dbEntry.StartDate &&
                                     i.To == dbEntry.EndDate
                                 select i).FirstOrDefault();
            return item;
        }

        private void CreateJourney(BusinessTrip bt)
        {
            DaysBetweenStartDateAndOrderStartDate = (bt.StartDate - bt.OrderStartDate.Value).Days;
            DaysBetweenOrderEndDateAndEndDate = (bt.OrderEndDate.Value - bt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = bt.OrderStartDate.Value;
            DateTime? StartDateForJourneyForEndDifference = bt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            Employee emp = (from e in context.Employees where e.EmployeeID == bt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = bt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                SaveJourney(journey);

                    item.EmployeeID = bt.EmployeeID;
                    item.From = journey.Date;
                    item.To = journey.Date;
                    item.Type = CalendarItemType.Journey;

                    emp.CalendarItems.Add(item);
                    SaveCalendarItem(item);

                    item = new CalendarItem();

                StartDateForJourneyForStartDifference = new DateTime?(StartDateForJourneyForStartDifference.Value.Date.AddDays(1));
            }

            for (int i = 0; i < DaysBetweenOrderEndDateAndEndDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = bt.BusinessTripID;
                journey.Date = StartDateForJourneyForEndDifference.Value;

                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }

                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                SaveJourney(journey);

                    item.EmployeeID = bt.EmployeeID;
                    item.From = journey.Date;
                    item.To = journey.Date;
                    item.Type = CalendarItemType.Journey;

                    emp.CalendarItems.Add(item);
                    SaveCalendarItem(item);
                    item = new CalendarItem();
                
                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.Date.AddDays(1));
            }
        }

        private void CreateJourneyFromPairedBts(List<BusinessTrip> bts)
        {
            BusinessTrip firstBt = bts.OrderBy(bt => bt.StartDate).FirstOrDefault();
            BusinessTrip lastBt = bts.OrderByDescending(bt => bt.EndDate).FirstOrDefault();

            DaysBetweenStartDateAndOrderStartDate = (firstBt.StartDate - firstBt.OrderStartDate.Value).Days;
            DaysBetweenOrderEndDateAndEndDate = (lastBt.OrderEndDate.Value - lastBt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = firstBt.OrderStartDate;
            DateTime? StartDateForJourneyForEndDifference = lastBt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            Employee empFirstBt = (from e in context.Employees where e.EmployeeID == firstBt.EmployeeID select e).FirstOrDefault();
            Employee empLastBt = (from e in context.Employees where e.EmployeeID == lastBt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = firstBt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                if (Journeys.Where(j => ((j.BusinessTripID == firstBt.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    SaveJourney(journey);

                    if ((firstBt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (firstBt.Status != BTStatus.Cancelled))
                    {
                        item.EmployeeID = firstBt.EmployeeID;
                        item.From = journey.Date;
                        item.To = journey.Date;
                        item.Type = CalendarItemType.Journey;

                        empFirstBt.CalendarItems.Add(item);
                        SaveCalendarItem(item);
                        item = new CalendarItem();
                    }
                }
                StartDateForJourneyForStartDifference = new DateTime?(StartDateForJourneyForStartDifference.Value.AddDays(1));
            }

            for (int i = 0; i < DaysBetweenOrderEndDateAndEndDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = lastBt.BusinessTripID;
                journey.Date = StartDateForJourneyForEndDifference.Value;

                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }

                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                if (Journeys.Where(j => ((j.BusinessTripID == lastBt.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    SaveJourney(journey);



                        item.EmployeeID = lastBt.EmployeeID;
                        item.From = journey.Date;
                        item.To = journey.Date;
                        item.Type = CalendarItemType.Journey;

                        empLastBt.CalendarItems.Add(item);
                        SaveCalendarItem(item);
                        item = new CalendarItem();
                    
                }
                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.AddDays(1));
            }
        }

        private bool CheckBTCreationPossibility(BusinessTrip bTrip)
        {
            //Select all CalendarItems for current User
            List<CalendarItem> vacationsList = (from item in CalendarItems 
                                                where item.EmployeeID == bTrip.EmployeeID &&
                                                (item.Type != CalendarItemType.BT && item.Type != CalendarItemType.Journey)
                                                select item).ToList();
            if (vacationsList.Count > 0)
            {
                //Check time periods by OrderDates
                if(bTrip.OrderStartDate.HasValue && bTrip.OrderEndDate.HasValue)
                {
                    foreach (CalendarItem vacations in vacationsList)
                    {
                        if (bTrip.OrderStartDate.Value <= vacations.From && bTrip.OrderEndDate.Value >=vacations.From ||
                            bTrip.OrderStartDate.Value >= vacations.From && bTrip.OrderStartDate.Value <= vacations.To)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                    //Check time periods by Start and EndDates
                    else
                    {
                        foreach (CalendarItem vacations in vacationsList)
                        {
                            if (bTrip.StartDate <= vacations.From && bTrip.EndDate >= vacations.From ||
                                bTrip.StartDate >= vacations.From && bTrip.StartDate <= vacations.To)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
            }
            return true;
        }

        public BusinessTrip DeleteBusinessTrip(int btID)
        {
            BusinessTrip dbEntry = BusinessTrips.Where(b => b.BusinessTripID == btID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.BusinessTrips.Remove(dbEntry);
            }

            context.SaveChanges();

            return dbEntry;
        }

        #endregion

        #region Locations
        public IQueryable<Location> Locations
        {
            get { return context.Locations; }
        }

        public void SaveLocation(Location location)
        {
            if (location.LocationID == 0)
                context.Locations.Add(location);
            else
            {
                Location dbEntry = context.Locations.Find(location.LocationID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(location.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.Title = location.Title;
                    dbEntry.Address = location.Address;
                    dbEntry.CountryID = location.CountryID;
                    dbEntry.ResponsibleForLoc = location.ResponsibleForLoc;
                }
            }
            context.SaveChanges();
        }

        public Location DeleteLocation(int locationID)
        {
            Location dbEntry = context.Locations.Find(locationID);

            if (dbEntry != null)
            {
                context.Locations.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion


        #region Units

        public IEnumerable<Unit> Units
        {
            get { return context.Units; }
        }

        public void SaveUnit(Unit unit)
        {
            if (unit.UnitID == 0)
                context.Units.Add(unit);
            else
            {
                Unit dbEntry = context.Units.Find(unit.UnitID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(unit.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.Title = unit.Title;
                    dbEntry.ShortTitle = unit.ShortTitle;
                }
            }
            context.SaveChanges();
        }

        public Unit DeleteUnit(int unitID)
        {
            Unit dbEntry = context.Units.Find(unitID);

            if (dbEntry != null)
            {
                context.Units.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion

        #region Sicknesses
        public IEnumerable<Sickness> Sicknesses
        {
            get { return context.Sicknesses; }
        }
        public void SaveSick(Sickness sick)
        {
            if (sick.SickID == 0)
            {
                sick.Workdays = CalculateWorkDays(sick);
                context.Sicknesses.Add(sick);
            }
            else
            {
                Sickness dbEntry = context.Sicknesses.Find(sick.SickID);
                if (dbEntry != null)
                {
                    dbEntry.From = sick.From;
                    dbEntry.EmployeeID = sick.EmployeeID;
                    dbEntry.To = sick.To;
                    dbEntry.SicknessType = sick.SicknessType;
                    dbEntry.Workdays = CalculateWorkDays(sick);
                }

            }
            context.SaveChanges();
        }

        private int CalculateWorkDays(Sickness sick)
        {

            int numberOfWorkingDays = 0;
            DateTime loopFrom = sick.From;
            while (loopFrom <= sick.To)
            {
                if (ISWorkDay(sick.From) && IsNotNationalHolidayExcludeWeekends(sick.From))
                {
                    numberOfWorkingDays++;
                }
                loopFrom = new DateTime(loopFrom.Year, loopFrom.Month, loopFrom.Day).Date.AddDays(1);
            }

            return numberOfWorkingDays;

        }
        private static bool ISWorkDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private bool IsNotNationalHolidayExcludeWeekends(DateTime date)
        {
            List<DateTime> holidays = (from h in Holidays where (h.CountryID == 1) && (h.HolidayDate.DayOfWeek != DayOfWeek.Saturday && h.HolidayDate.DayOfWeek != DayOfWeek.Sunday) select h.HolidayDate).ToList();
            return !holidays.Contains(date);
        }
        public Sickness DeleteSick(int SickID)
        {
            Sickness dbEntry = context.Sicknesses.Find(SickID);
            if (dbEntry != null)
            {
                context.Sicknesses.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


        #endregion

        #region Roles  

        public void SaveRolesForEmployee(string username, string[] roles)
        {
            if (Membership.GetUser(username) != null)
            {
                UpdateUserAndRoles(username, roles);
            }
            else
            {
                CreateUserWithRoles(username, roles);
            }
        }

        private void UpdateUserAndRoles(string username, string[] roles)
        {
            if (roles != null)
            {
                RemoveRolesFromUser(username);
                AddRolesForUser(username, roles);
            }
            else
            {
                DeleteUser(username);
            }
        }

        private static void CreateUserWithRoles(string username, string[] roles)
        {
            if (roles != null)
            {
                WebSecurity.CreateUserAndAccount(username, ConfigurationManager.AppSettings["DefaultPassword"]);
                AddRolesForUser(username, roles);
            }
        }

        private static void RemoveRolesFromUser(string username)
        {
            foreach (string role in Roles.GetRolesForUser(username))
            {
                Roles.RemoveUserFromRole(username, role);
            }
        }

        private static void AddRolesForUser(string username, string[] roles)
        {
            foreach (string role in roles)
            {
                if (!Roles.IsUserInRole(username, role))
                {
                    Roles.AddUserToRole(username, role);
                }
            }
        }

        public void DeleteUser(string username)
        {
            if (Membership.GetUser(username) != null)
            {
                RemoveRolesFromUser(username);
                Membership.DeleteUser(username);
            }
        }

        #endregion

        #region Messages

        public IQueryable<IMessage> Messages
        {
            get { return context.Messages; }
        }

        public void SaveMessage(IMessage Message)
        {
            if (Message.MessageID == 0)
                context.Messages.Add(Message as Message);
            else
            {
                IMessage dbEntry = context.Messages.Find(Message.MessageID);
                if (dbEntry != null)
                {
                    dbEntry.MessageID = Message.MessageID;
                    dbEntry.Role = Message.Role;
                    dbEntry.Subject = Message.Subject;
                    dbEntry.Body = Message.Body;
                    dbEntry.Link = Message.Link;
                    dbEntry.TimeStamp = Message.TimeStamp;
                    dbEntry.ReplyTo = Message.ReplyTo;
                }
            }
            context.SaveChanges();
        }

        public IMessage DeleteMessage(int MessageID)
        {
            IMessage dbEntry = context.Messages.Find(MessageID);

            if (dbEntry != null)
            {
                context.Messages.Remove(dbEntry as Message);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Passports

        public IQueryable<Passport> Passports
        {
            get { return context.Passports; }
        }

        public void SavePassport(Passport passport)
        {
            Passport dbEntry = context.Passports.Find(passport.EmployeeID);

            if (dbEntry != null && passport.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(passport.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = passport.EmployeeID;
                dbEntry.EndDate = (passport.EndDate == null) ? dbEntry.EndDate : passport.EndDate;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == passport.EmployeeID).SingleOrDefault();
                //passport.PassportOf = employee;

                context.Passports.Add(passport);
            }
            context.SaveChanges();
        }

        public Passport DeletePassport(int employeeID)
        {
            Passport dbEntry = context.Passports.Find(employeeID);
            if (dbEntry != null)
            {
                context.Passports.Remove(dbEntry as Passport);
                context.SaveChanges();
            };
            return dbEntry;
        }

        #endregion

        #region PrivateTrip
        public IQueryable<PrivateTrip> PrivateTrips
        {
            get { return context.PrivateTrips; }
        }

        public void SavePrivateTrip(PrivateTrip pt)
        {
            PrivateTrip dbEntry = (from p in PrivateTrips where p.PrivateTripID == pt.PrivateTripID select p).FirstOrDefault();

            if (dbEntry != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(pt.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }
                dbEntry.StartDate = (pt.StartDate == default(DateTime)) ? dbEntry.StartDate : pt.StartDate;
                dbEntry.EndDate = (pt.EndDate == default(DateTime)) ? dbEntry.EndDate : pt.EndDate;
            }
            else
            {
                context.PrivateTrips.Add(pt);
            }

            context.SaveChanges();
        }

        public PrivateTrip DeletePrivateTrip(int ptID)
        {
            PrivateTrip dbEntry = PrivateTrips.Where(p => p.PrivateTripID == ptID).SingleOrDefault();

            if (dbEntry != null)
            {

                context.PrivateTrips.Remove(dbEntry);
            }

            context.SaveChanges();

            return dbEntry;
        }

        #endregion

        #region Position

        public IQueryable<Position> Positions
        {
            get { return context.Positions; }
        }

        public void SavePosition(Position position)
        {
            if (position.PositionID == 0)
            {
                context.Positions.Add(position);
            }
            else
            {
                Position dbEntry = context.Positions.Find(position.PositionID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(position.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.TitleEn = position.TitleEn;
                    dbEntry.TitleUk = position.TitleUk;
                    dbEntry.Employees = position.Employees;
                }
            }

            context.SaveChanges();
        }

        public Position DeletePosition(int positionID)
        {
            Position dbEntry = context.Positions.Find(positionID);

            if (dbEntry != null)
            {
                context.Positions.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Countries

        public IEnumerable<Country> Countries
        {
            get { return context.Countries; }
        }

        public void SaveCountry(Country country)
        {
            if (country.CountryID == 0)
            {
                context.Countries.Add(country);
            }
            else
            {
                Country dbentry = context.Countries.Find(country.CountryID);
                if (dbentry != null)
                {
                    dbentry.Comment = country.Comment;
                    dbentry.CountryName = country.CountryName;
                    dbentry.Holidays = country.Holidays;
                    dbentry.Locations = country.Locations;
                }
            }
            context.SaveChanges();
        }

        public Country DeleteCountry(int countryID)
        {
            Country dbentry = context.Countries.Find(countryID);

            if (dbentry != null)
            {
                context.Countries.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Holidays

        public IEnumerable<Holiday> Holidays
        {
            get { return context.Holidays; }
        }

        public void SaveHoliday(Holiday holiday, DateTime date, int countryID, bool isPostponed)
        {
            if (holiday.HolidayID == 0)
            {
                Country con = context.Countries.Where(c => c.CountryID == countryID).FirstOrDefault();

                holiday.HolidayDate = date;
                holiday.CountryID = countryID;
                holiday.Country = con;
                holiday.IsPostponed = isPostponed;

                context.Holidays.Add(holiday);
            }
            else
            {
                Holiday dbentry = context.Holidays.Find(holiday.HolidayID);

                if (dbentry != null)
                {
                    dbentry.HolidayDate = date;
                    dbentry.CountryID = countryID;
                    dbentry.IsPostponed = isPostponed;
                }
            }

            context.SaveChanges();
        }

        public Holiday DeleteHoliday(int holidayID)
        {
            Holiday dbentry = context.Holidays.Find(holidayID);

            if (dbentry != null)
            {
                context.Holidays.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Journeys

        public IEnumerable<Journey> Journeys
        {
            get { return context.Journeys; }
        }

        public void SaveJourney(Journey journey)
        {
            if (journey.JourneyID == 0)
            {
                context.Journeys.Add(journey);
            }
            else
            {
                Journey dbEntry = context.Journeys.Find(journey.JourneyID);
                if (dbEntry != null)
                {
                    dbEntry = journey;
                    //dbEntry.Date = journey.Date;
                    //dbEntry.DayOff = journey.DayOff;
                    //dbEntry.ReclaimDate = journey.ReclaimDate;
                }

            }
            context.SaveChanges();
        }

        public Journey DeleteJourney(int journeyID)
        {
            Journey dbEntry = context.Journeys.Find(journeyID);

            if (dbEntry != null)
            {
                context.Journeys.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region CalendarItem

        public IEnumerable<CalendarItem> CalendarItems
        {
            get { return context.CalendarItems; }
        }

        public void SaveCalendarItem(CalendarItem calendarItem)
        {
            if (calendarItem.CalendarItemID == 0)
            {
                context.CalendarItems.Add(calendarItem);
            }
            else
            {
                CalendarItem dbentry = context.CalendarItems.Find(calendarItem.CalendarItemID);
                if (dbentry != null)
                {
                    dbentry.CalendarItemID = calendarItem.CalendarItemID;
                    dbentry.Employee = calendarItem.Employee;
                    dbentry.EmployeeID = calendarItem.EmployeeID;
                    dbentry.From = calendarItem.From;
                    dbentry.Location = calendarItem.Location;
                    dbentry.To = calendarItem.To;
                    dbentry.Type = calendarItem.Type;
                }
            }
            context.SaveChanges();
        }

        public CalendarItem DeleteCalendarItem(int calendarID)
        {
            CalendarItem dbEntry = context.CalendarItems.Find(calendarID);

            if (dbEntry != null)
            {
                context.CalendarItems.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Overtime

        public IEnumerable<Overtime> Overtimes
        {
            get { return context.Overtimes; }
        }

        public void SaveOvertime(Overtime overtime)
        {
            if (overtime.OvertimeID == 0)
            {
                context.Overtimes.Add(overtime);
            }
            else
            {
                Overtime dbentry = context.Overtimes.Find(overtime.OvertimeID);

                if (dbentry != null)
                {
                    dbentry.Date = overtime.Date;
                    dbentry.Employee = overtime.Employee;
                    dbentry.EmployeeID = overtime.EmployeeID;
                    dbentry.OvertimeID = overtime.OvertimeID;
                    dbentry.ReclaimDate = overtime.ReclaimDate;
                    dbentry.Type = overtime.Type;
                    dbentry.DayOff = overtime.DayOff;
                }
            }

            context.SaveChanges();
        }

        public Overtime DeleteOvertime(int overtimeID)
        {
            Overtime dbEntry = context.Overtimes.Find(overtimeID);

            if (dbEntry != null)
            {
                context.Overtimes.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion

        #region Vacation

        public IEnumerable<Vacation> Vacations
        {
            get { return context.Vacations; }
        }

        public void SaveVacation(Vacation vacation)
        {
            if (vacation.VacationID == 0)
            {
                context.Vacations.Add(vacation);
            }
            else
            {
                Vacation dbentry = context.Vacations.Find(vacation.VacationID);

                if (dbentry != null)
                {
                    dbentry = vacation;
                }
            }
            context.SaveChanges();
        }

        public Vacation DeleteVacation(int vacationID)
        {
            Vacation dbentry = context.Vacations.Find(vacationID);

            if (dbentry != null)
            {
                context.Vacations.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Users
        public IQueryable<Employee> Users
        {
            get { return context.Employees; }
            //get { return context.Users; }
        }

        #endregion

        #region Greeting

        public IEnumerable<Greeting> Greetings
        {
            get { return context.Greetings; }
        }

        public void SaveGreeting(Greeting greeting)
        {
            if (greeting.GreetingId == 0)
            {
                context.Greetings.Add(greeting);
            }
            else
            {
                Greeting dbentry = context.Greetings.Find(greeting.GreetingId);

                if (dbentry != null)
                {
                    dbentry.GreetingHeader = greeting.GreetingHeader;
                    dbentry.GreetingBody = greeting.GreetingBody;
                }
            }
            context.SaveChanges();
        }

        public Greeting DeleteGreeting(int GreetingId)
        {

            Greeting dbentry = context.Greetings.Find(GreetingId);

            if (dbentry != null)
            {
                context.Greetings.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;

        }

        #endregion

    }
}