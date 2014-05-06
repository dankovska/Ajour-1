using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using AjourBT.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class EMPController : Controller
    {
        public EMPController()
        {
            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;
        }

        private IRepository repository;

        public EMPController(IRepository repo)
            : this()
        {
            repository = repo;
        }

        public ViewResult Index(string userName = "")
        {
            ViewBag.UserName = userName;
            return View();
        }



        //
        // GET: /EMP/
        [Authorize(Roles = "EMP")]
        public ActionResult GetLastBTDataPerEmployee(string userName = "")
        {
            DateTime currDate = DateTime.Now.ToLocalTimeAzure().Date;

            Employee employee = repository.Employees.FirstOrDefault(e => e.EID == userName);
            if (employee == null)
            {
                return View("NoData");
            }
            BusinessTrip bt = (from e in repository.Employees
                               where e.EID == userName
                               from b in e.BusinessTrips
                               where (b.Status == (BTStatus.Confirmed | BTStatus.Reported) && (b.Status != BTStatus.Cancelled))
                               && b.StartDate <= currDate
                               orderby b.EndDate descending
                               select b).FirstOrDefault();
            if (bt == null)
            {
                return View("NoData");
            }

            ViewBag.BTsGeneralInformation = bt;

            if (bt.OrderStartDate != null && bt.OrderEndDate != null)
            {
                var bts = (from b in employee.BusinessTrips
                           where (b.OrderStartDate != null && b.OrderEndDate != null) && (b.OrderStartDate.Value == bt.OrderStartDate.Value && b.OrderEndDate.Value == bt.OrderEndDate.Value)
                           && b.StartDate <= currDate
                           orderby b.StartDate
                           select b).ToList();



                return View(bts);
            }
            else
            {
                List<BusinessTrip> lastBtList = new List<BusinessTrip>();
                lastBtList.Add(bt);
                return View(lastBtList);
            }


        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetVisaDataPerEmployee(string userName = "")
        {
            Employee employee = repository.Employees.Where(e => e.EID == userName).FirstOrDefault();
            if (employee == null)
            {
                return View("NoData");
            }
            return View(employee);
        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetAbsencePerEMP()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.FromText = new DateTime(currYear, currMonth, 01);
            ViewBag.ToText = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));

            return View();
        }

        [Authorize(Roles = "EMP")]
        public PartialViewResult GetAbsenceDataPerEMP(string FromAbsence = "", string ToAbsence = "", string userName = "")
        {
            DateTime fromParsed = DateTime.Now;
            DateTime toParse = DateTime.Now;
            int FromYear = DateTime.Now.Year;
            int ToYear = DateTime.Now.Year;

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            if (FromAbsence != "" && ToAbsence != "")
            {
                fromParsed = DateTime.ParseExact(FromAbsence, "dd.MM.yyyy", null); // check this
                toParse = DateTime.ParseExact(ToAbsence, "dd.MM.yyyy", null);// --------^
                FromYear = fromParsed.Year;
                ToYear = toParse.Year;
            }
            else
            {
                return PartialView("~/Views/WTR/GetWTRDataEmpty.cshtml");
            }

            Employee employee = repository.Employees.Where(e => e.EID == userName).FirstOrDefault();

            if (employee == null)
            {
                return PartialView("NoData");
            }

            AbsenceViewModelForEMP absenceData = new AbsenceViewModelForEMP { ID = employee.EID, FirstName = employee.FirstName, LastName = employee.LastName, FactorDetails = new Dictionary<CalendarItemType, List<AbsenceFactorData>>() };

            List<AbsenceFactorData> sicknessList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> paidVacationList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> unpaidVacationList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> overtimeList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> paidOvertimeList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> privateOvertimeList = new List<AbsenceFactorData>();
            List<AbsenceFactorData> journeyList = new List<AbsenceFactorData>();

            if (employee.Sicknesses.Count != 0)
            {
                foreach (Sickness sick in employee.Sicknesses)
                {
                    if (sick.From >= fromParsed && sick.To <= toParse)
                    {
                        AbsenceFactorData data = new AbsenceFactorData();
                        data.AbsenceFactorDataID = sick.SickID;
                        data.From = sick.From;
                        data.To = sick.To;

                        sicknessList.Add(data);
                    }
                }
            }

            if (employee.Vacations.Count != 0)
            {
                foreach (Vacation vacation in employee.Vacations.OrderBy(d => d.From))
                {
                    if (vacation.From >= fromParsed && vacation.To <= toParse)
                    {
                        switch (vacation.Type)
                        {
                            case VacationType.PaidVacation:
                                AbsenceFactorData data = new AbsenceFactorData();
                                data.AbsenceFactorDataID = vacation.VacationID;
                                data.From = vacation.From;
                                data.To = vacation.To;

                                paidVacationList.Add(data);
                                break;

                            case VacationType.UnpaidVacation:
                                AbsenceFactorData dataUnpaid = new AbsenceFactorData();
                                dataUnpaid.AbsenceFactorDataID = vacation.VacationID;
                                dataUnpaid.From = vacation.From;
                                dataUnpaid.To = vacation.To;

                                unpaidVacationList.Add(dataUnpaid);
                                break;
                        }
                    }
                }
            }

            if (employee.Overtimes.Count != 0)
            {
                List<Overtime> overtimeDaysOffTrue = employee.Overtimes.Where(d => d.DayOff == true).OrderBy(d => d.Date).ToList();
                foreach (Overtime overtime in overtimeDaysOffTrue)
                {
                    if (overtime.Date >= fromParsed && overtime.Date <= toParse)
                    {
                        switch (overtime.Type)
                        {
                            //case OvertimeType.Overtime:
                            //    AbsenceFactorData overtimeData = new AbsenceFactorData();
                            //    overtimeData.AbsenceFactorDataID = overtime.OvertimeID;
                            //    overtimeData.From = overtime.Date;
                            //    overtimeData.To = overtime.Date;
                            //    overtimeData.ReclaimDate = overtime.ReclaimDate;

                            //    overtimeList.Add(overtimeData);
                            //    break;

                            case OvertimeType.Paid:
                                AbsenceFactorData paidOvertime = new AbsenceFactorData();
                                paidOvertime.AbsenceFactorDataID = overtime.OvertimeID;
                                paidOvertime.From = overtime.Date;
                                paidOvertime.To = overtime.Date;
                                paidOvertime.ReclaimDate = overtime.ReclaimDate;

                                paidOvertimeList.Add(paidOvertime);
                                if (overtime.ReclaimDate != null && overtime.ReclaimDate != DateTime.MinValue)
                                {
                                    AbsenceFactorData overtimeData = new AbsenceFactorData();
                                    overtimeData.AbsenceFactorDataID = overtime.OvertimeID;
                                    overtimeData.From = overtime.ReclaimDate.Value;
                                    overtimeData.To = overtime.ReclaimDate.Value;
                                    overtimeData.ReclaimDate = overtime.Date;

                                    overtimeList.Add(overtimeData);
                                }
                                break;

                            case OvertimeType.Private:
                                AbsenceFactorData privateOvertime = new AbsenceFactorData();
                                privateOvertime.AbsenceFactorDataID = overtime.OvertimeID;
                                privateOvertime.From = overtime.Date;
                                privateOvertime.To = overtime.Date;
                                privateOvertime.ReclaimDate = overtime.ReclaimDate;

                                privateOvertimeList.Add(privateOvertime);
                                break;
                        }
                    }
                }
            }

            if (employee.BusinessTrips.Count != 0)
            {
                List<Journey> employeeJourneys = (from bts in repository.BusinessTrips
                                                  where bts.EmployeeID == employee.EmployeeID
                                                  from j in bts.Journeys
                                                  where j.BusinessTripID == bts.BusinessTripID && j.DayOff == true
                                                  orderby j.Date
                                                  select j).ToList();

                if (employeeJourneys.Count != 0)
                {
                    foreach (Journey journey in employeeJourneys)
                    {
                        if (journey.Date >= fromParsed && journey.Date <= toParse)
                        {
                            AbsenceFactorData journeyData = new AbsenceFactorData();
                            journeyData.AbsenceFactorDataID = journey.JourneyID;
                            journeyData.From = journey.Date;
                            journeyData.To = journey.Date;
                            journeyData.ReclaimDate = journey.ReclaimDate;

                            journeyList.Add(journeyData);
                            if (journey.ReclaimDate != null && journey.ReclaimDate != DateTime.MinValue && journey.DayOff == true)
                            {
                                AbsenceFactorData overtimeData = new AbsenceFactorData();
                                overtimeData.AbsenceFactorDataID = journey.JourneyID;
                                overtimeData.From = journey.ReclaimDate.Value;
                                overtimeData.To = journey.ReclaimDate.Value;
                                overtimeData.ReclaimDate = journey.Date;

                                overtimeList.Add(overtimeData);
                            }
                        }
                    }
                }
            }


            absenceData.FactorDetails.Add(CalendarItemType.SickAbsence, sicknessList);
            absenceData.FactorDetails.Add(CalendarItemType.PaidVacation, paidVacationList);
            absenceData.FactorDetails.Add(CalendarItemType.UnpaidVacation, unpaidVacationList);
            absenceData.FactorDetails.Add(CalendarItemType.ReclaimedOvertime, overtimeList);
            absenceData.FactorDetails.Add(CalendarItemType.OvertimeForReclaim, paidOvertimeList);
            absenceData.FactorDetails.Add(CalendarItemType.PrivateMinus, privateOvertimeList);
            absenceData.FactorDetails.Add(CalendarItemType.Journey, journeyList);


            if (sicknessList.Count == 0 && paidVacationList.Count == 0 && unpaidVacationList.Count == 0 &&
                overtimeList.Count == 0 && paidVacationList.Count == 0 && privateOvertimeList.Capacity == 0 &
                journeyList.Count == 0)
            {
                return PartialView("NoData");
            }

            ViewBag.FromWeek = cal.GetWeekOfYear(fromParsed, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.ToWeek = cal.GetWeekOfYear(toParse, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;

            return PartialView("GetAbsenceDataPerEMP", absenceData);

        }

        [Authorize(Roles = "EMP")]
        [HttpGet]
        public ActionResult GetBirthdays()
        {
            DateTime fromDate = DateTime.Now.Date;
            DateTime toDate = fromDate.AddDays(30).Date;
            List<Employee> birthList = new List<Employee>();
            List<Employee> emp = (from e in repository.Employees
                                  where
                                      e.BirthDay.HasValue
                                  orderby e.BirthDay.Value
                                  select e).ToList();
            DateTime date;
            foreach (Employee e in emp)
            {
                date = TransformBirthDate(e.BirthDay.Value, DateTime.Now.Date);
                if (date >= fromDate.AddDays(-10) && date <= toDate)
                {
                    birthList.Add(e);
                }
            }

            birthList = birthList.OrderBy(b => b.BirthDay.Value.Month).ThenBy(b => b.BirthDay.Value.Day).ToList();

            if (birthList.Count != 0)
            {
                return View(birthList);
            }
            else
            {
                return View("NoData");
            }
        }

        public DateTime TransformBirthDate(DateTime Birthdate, DateTime nowDate)
        {
            DateTime tempDate = new DateTime(nowDate.Year, Birthdate.Month, Birthdate.Day);
            DateTime transfDate = new DateTime();
            if (nowDate.Month == 12 && nowDate.Day >= 2)
            {
                transfDate = tempDate.AddYears(1);
                return transfDate;
            }
            else return tempDate;
        }

        public ActionResult GetReportedBTs(string userName = "")
        {
            Employee emp = (from e in repository.Employees where e.EID == userName select e).FirstOrDefault();

            if (emp == null)
            {
                return PartialView("NoData");
            }

            List<BusinessTrip> reportedBTs = (from bts in emp.BusinessTrips
                                              where
                                                  ((bts.Status == (BTStatus.Reported | BTStatus.Confirmed) && bts.Status != BTStatus.Cancelled) ||
                                                  bts.Status == (BTStatus.Reported) ||
                                                  bts.Status == (BTStatus.Confirmed))
                                              orderby bts.StartDate
                                              select bts).ToList();

            if (reportedBTs.Count == 0)
            {
                return PartialView("NoData");
            }

            return View(reportedBTs);
        }
    }
}
