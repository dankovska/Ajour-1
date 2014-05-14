using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using System.Globalization;
using System.Threading;
using AjourBT.Models;
using System.Data.Objects;
using AjourBT.Infrastructure;
using AjourBT.Domain.Entities;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Text;
using AjourBT.Exeptions;

namespace AjourBT.Controllers // Add Items to CalendarItem (Employee)
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif

    [Authorize(Roles = "ADM, DIR, BTM, ACC")]
    public class BusinessTripController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

        private string btDuplication = "BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.";

        private string btDatesOverlay = "BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'";

        private string btCreationError ="Absence already planned on this period for this user. "
                                      + "Please change OrderDates or if BT haven\'t OrderDates "
                                      + "change \'From\' or \'To\'";

        private StringBuilder comment = new StringBuilder();
        private string defaultAccComment;

        public BusinessTripController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;
            this.comment = this.comment.Append("ВКО №   від   , cума:   UAH.");
            this.comment = this.comment.AppendLine();
            this.comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }

        private void LocationsDropDownList(int selectedLocationID = 0)
        {
            var locationList = from loc in repository.Locations
                               orderby loc.Title
                               select loc;

            ViewBag.LocationsList = new SelectList(locationList, "LocationID", "Title", selectedLocationID);
        }


        private void UnitsDropDownList(int selectedUnitID = 0)
        {
            var unitList = from unit in repository.Units
                           orderby unit.ShortTitle
                           select unit;

            ViewBag.UnitsList = new SelectList(unitList, "UnitID", "ShortTitle", selectedUnitID);
        }
        public void AddLastCRUDDataToBT(BusinessTrip selectedBT)
        {
            selectedBT.LastCRUDedBy = ControllerContext.HttpContext.User.Identity.Name;
            selectedBT.LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure();
        }


        #region ADM

        [Authorize(Roles = "ADM")]
        public PartialViewResult GetBusinessTripDataADM(string selectedDepartment = null, string selectedUserDepartment = null)
        {
            IEnumerable<Employee> data;
            data = SelectEmployees(selectedDepartment, selectedUserDepartment);

            ViewBag.SelectedDepartment = selectedUserDepartment == null ? selectedDepartment : selectedUserDepartment;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(data.ToList());
        }

        private IEnumerable<Employee> SelectEmployees(string selectedDepartment, string selectedUserDepartment)
        {
            IEnumerable<Employee> data;
            if (selectedDepartment == null)
            {
                data = from emp in repository.Employees.AsEnumerable()
                       join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                       where ((emp.Department.DepartmentName == selectedUserDepartment && (emp.DateDismissed == null)))
                       orderby emp.IsManager descending, emp.LastName
                       select emp;


            }
            else
            {
                data = from emp in repository.Employees.AsEnumerable()
                       join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                       where ((emp.Department.DepartmentName == selectedDepartment || (selectedDepartment == String.Empty)
    || (selectedDepartment == null)) && (emp.DateDismissed == null))
                       orderby emp.IsManager descending, emp.LastName
                       select emp;



            }
            return data;
        }

        [Authorize(Roles = "ADM")]
        public ViewResult GetBusinessTripADM(string userName = "", string selectedDepartment = null)
        {

            var selectedUserDepartment = (from e in repository.Employees
                                          where e.EID == userName
                                          select e.Department.DepartmentName).FirstOrDefault();

            var allDepartments = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(allDepartments, "DepartmentName", "DepartmentName", selectedUserDepartment);
            ViewBag.UserDepartment = selectedUserDepartment;
            ViewBag.SelectedDepartment = selectedDepartment;
            return View();

        }

        // POST: /BusinessTrip/RegisterPlannedBTs

        [Authorize(Roles = "ADM")]
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult RegisterPlannedBTs(string[] selectedPlannedBTs, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedPlannedBTs != null && selectedPlannedBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedPlannedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Planned) == BTStatus.Planned) && selectedBT.RejectComment == null)
                        {
                            selectedBT.Status = (selectedBT.Status | BTStatus.Registered) & ~BTStatus.Planned;
                            try
                            {
                                repository.SaveBusinessTrip(selectedBT);
                                AddLastCRUDDataToBT(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                            //catch (VacationAlreadyExistException)
                            //{
                            //    return Json(new { error = btCreationError });
                            //}
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        //[Authorize(Roles = "ADM")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult RegisterPlannedBT(BusinessTrip bTrip, string selectedDepartment = null)
        //{
        //    LocationsDropDownList(bTrip.LocationID);
        //    BusinessTrip businessTrip = bTrip;

        //    if (bTrip.BusinessTripID != 0)
        //    {
        //        businessTrip = RewriteBTsPropsAfterPlanningFromRepository(bTrip);
        //        businessTrip.RejectComment = null;
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        if (((businessTrip.Status & BTStatus.Planned) == BTStatus.Planned) && businessTrip.RejectComment == null)
        //        {
        //            Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
        //            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
        //            businessTrip.Status = (businessTrip.Status | BTStatus.Registered) & ~BTStatus.Planned;
        //            AddLastCRUDDataToBT(businessTrip);
        //            repository.SaveBusinessTrip(businessTrip);
        //            selectedBusinessTripsList.Add(businessTrip);
        //            messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, selectedBusinessTripsList, author));
        //            messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, selectedBusinessTripsList, author));
        //        }
        //        return RedirectToAction("ADMView", "Home", new { tab = 1, selectedDepartment = selectedDepartment });

        //    }
        //    BusinessTripViewModel btViewModel = new BusinessTripViewModel(bTrip);
        //    LocationsDropDownList(bTrip.LocationID);
        //    btViewModel.BTof = repository.BusinessTrips.Where(bt => bt.BusinessTripID == bTrip.BusinessTripID).Select(b => b.BTof).FirstOrDefault();
        //    return View("EditPlannedBT", btViewModel);
        //}


        [Authorize(Roles = "ADM")]
        [HttpPost]
        //     [ValidateAntiForgeryToken]
        public ActionResult ConfirmPlannedBTs(string[] selectedPlannedBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedPlannedBTs != null && selectedPlannedBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedPlannedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Planned) == BTStatus.Planned) && selectedBT.RejectComment == null)
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Confirmed) & ~BTStatus.Planned;
                                AddLastCRUDDataToBT(selectedBT);
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult ConfirmRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered) && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Confirmed) & ~BTStatus.Registered;
                                AddLastCRUDDataToBT(selectedBT);
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult ReplanRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if ((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Planned | BTStatus.Modified) & ~BTStatus.Registered;
                                selectedBT.OldLocationID = selectedBT.LocationID;
                                selectedBT.OldLocationTitle = selectedBT.Location.Title;
                                selectedBT.OldStartDate = selectedBT.StartDate;
                                selectedBT.OldEndDate = selectedBT.EndDate;
                                AddLastCRUDDataToBT(selectedBT);
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        //        [ValidateAntiForgeryToken]
        public ActionResult CancelRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if ((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Cancelled) & ~BTStatus.Modified;
                                AddLastCRUDDataToBT(selectedBT);
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT != null)
            {
                if ((selectedBT.Status & BTStatus.Confirmed) == BTStatus.Confirmed && !selectedBT.Status.HasFlag(BTStatus.Reported))
                {
                    try
                    {
                        Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                        List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                        selectedBT.Status = (selectedBT.Status | BTStatus.Cancelled);
                        AddLastCRUDDataToBT(selectedBT);
                        repository.SaveBusinessTrip(selectedBT);
                        selectedBusinessTripsList.Add(selectedBT);
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                }
            }
            return RedirectToAction("ADMView", "Home", new { tab = 1, selectedDepartment = selectedDepartment });
        }

        //[Authorize(Roles = "ADM")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ConfirmPlannedBT(BusinessTrip bTrip, string selectedDepartment = null)
        //{
        //    LocationsDropDownList(bTrip.LocationID);
        //    BusinessTrip businessTrip = bTrip;

        //    if (bTrip.BusinessTripID != 0)
        //    {
        //        businessTrip = RewriteBTsPropsAfterPlanningFromRepository(bTrip);
        //        businessTrip.RejectComment = null;
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        if (((businessTrip.Status & BTStatus.Planned) == BTStatus.Planned) && businessTrip.RejectComment == null)
        //        {
        //            Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
        //            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
        //            businessTrip.Status = (businessTrip.Status | BTStatus.Confirmed) & ~BTStatus.Planned;
        //            AddLastCRUDDataToBT(businessTrip);
        //            repository.SaveBusinessTrip(businessTrip);
        //            selectedBusinessTripsList.Add(businessTrip);
        //            messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, selectedBusinessTripsList, author));
        //            messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, selectedBusinessTripsList, author));
        //            messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, selectedBusinessTripsList, author));
        //        }
        //        return RedirectToAction("ADMView", "Home", new { tab = 1, selectedDepartment = selectedDepartment });

        //    }
        //    BusinessTripViewModel btViewModel = new BusinessTripViewModel(bTrip);
        //    LocationsDropDownList(bTrip.LocationID);
        //    btViewModel.BTof = repository.BusinessTrips.Where(bt => bt.BusinessTripID == bTrip.BusinessTripID).Select(b => b.BTof).FirstOrDefault();
        //    return View("EditPlannedBT", btViewModel);
        //}


        //GET: Delete Planned BT

        [Authorize(Roles = "ADM")]
        public ActionResult DeletePlannedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (selectedBT == null || (selectedBT.Status != (BTStatus.Planned) && selectedBT.Status != (BTStatus.Planned | BTStatus.Modified)))
            {
                return HttpNotFound();
            }
            ViewBag.SelectedDepartment = selectedDepartment;
            return View(selectedBT);
        }

        //POST: Delete Planned BT

        [Authorize(Roles = "ADM")]
        [HttpPost, ActionName("DeletePlannedBT")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePlannedBTConfirmed(int id = 0, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT != null)
            {
                if (selectedBT.Status == BTStatus.Planned)
                {
                    AddLastCRUDDataToBT(selectedBT);
                    repository.DeleteBusinessTrip(id);
                }
                else if (selectedBT.Status == (BTStatus.Planned | BTStatus.Modified))
                {
                    try
                    {
                        selectedBT.Status = (BTStatus.Planned | BTStatus.Cancelled);
                        AddLastCRUDDataToBT(selectedBT);
                        Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                        List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                        repository.SaveBusinessTrip(selectedBT);
                        selectedBusinessTripsList.Add(selectedBT);
                        messenger.Notify(new Message(MessageType.ADMCancelsPlannedModifiedToBTM, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ADMCancelsPlannedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }

                }
            }
            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        //
        // GET: /BusinessTrip/Plan

        [Authorize(Roles = "ADM")]
        public ActionResult Plan(int id = 0, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == id select e).FirstOrDefault();

            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                LocationsDropDownList();
                UnitsDropDownList();
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            }

            return View();
        }

        //
        // POST: /BusinessTrip/Plan

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Plan(BusinessTrip BTrip, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            LocationsDropDownList(BTrip.LocationID);
            UnitsDropDownList(BTrip.UnitID);
            BusinessTrip businessTrip = BTrip;

            if (businessTrip.BusinessTripID == 0)
            {
                BusinessTrip BussinesTrip = (from e in repository.Employees
                                             where e.EmployeeID == businessTrip.EmployeeID
                                             from bts in e.BusinessTrips
                                             where
                                                 (businessTrip.StartDate == bts.StartDate ||
                                                 businessTrip.EndDate == bts.EndDate)
                                             select bts).FirstOrDefault();

                if (BussinesTrip != null && !BussinesTrip.Status.HasFlag(BTStatus.Cancelled))
                {
                    if (businessTrip.StartDate == businessTrip.EndDate)
                    {
                        if (BussinesTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                    else
                    {
                        if ((businessTrip.StartDate == BussinesTrip.StartDate && businessTrip.EndDate == BussinesTrip.EndDate) || BussinesTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                }

                foreach (BusinessTrip bt in repository.BusinessTrips.Where(bt => bt.EmployeeID == BTrip.EmployeeID))
                {
                    if (!bt.Status.HasFlag(BTStatus.Cancelled))
                    {

                        if ((BTrip.StartDate == bt.StartDate) && (BTrip.EndDate == bt.EndDate))
                        {
                            return Json(new { error = btDuplication });
                        }

                        if ((BTrip.EndDate == bt.StartDate) || (BTrip.StartDate == bt.EndDate) || (BTrip.StartDate == bt.StartDate) || (BTrip.EndDate == bt.EndDate))
                        {
                            if (BTrip.LocationID == bt.LocationID)
                            {
                                return Json(new { error = btDuplication });
                            }
                        }

                        if (isBetween(BTrip.StartDate, bt) || isBetween(BTrip.EndDate, bt) || isBetween(bt.StartDate, BTrip) || isBetween(bt.EndDate, BTrip))
                        {

                            return Json(new { error = btDatesOverlay });

                        }
                    }
                }
            }

            if (BTrip.BusinessTripID != 0)
            {
                BusinessTrip bsTrip = (from e in repository.Employees
                                       where e.EmployeeID == BTrip.EmployeeID
                                       from bt in e.BusinessTrips
                                       where
                                            (BTrip.StartDate == bt.StartDate ||
                                            BTrip.EndDate == bt.EndDate) //&& BTrip.LocationID == bt.LocationID
                                       select bt).FirstOrDefault();

                if (bsTrip != null && bsTrip.BusinessTripID != BTrip.BusinessTripID && !bsTrip.Status.HasFlag(BTStatus.Cancelled))
                {
                    if (businessTrip.StartDate == businessTrip.EndDate)
                    {
                        if (bsTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                    else
                    {
                        if ((businessTrip.StartDate == bsTrip.StartDate && businessTrip.EndDate == bsTrip.EndDate) || bsTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                }

                foreach (BusinessTrip bt in repository.BusinessTrips.Where(bt => (bt.EmployeeID == BTrip.EmployeeID && bt.BusinessTripID != BTrip.BusinessTripID)))
                {
                    if (!bt.Status.HasFlag(BTStatus.Cancelled))
                    {

                        if ((BTrip.StartDate == bt.StartDate) && (BTrip.EndDate == bt.EndDate))
                        {
                            return Json(new { error = btDuplication });
                        }

                        if ((BTrip.EndDate == bt.StartDate) || (BTrip.StartDate == bt.EndDate) || (BTrip.StartDate == bt.StartDate) || (BTrip.EndDate == bt.EndDate))
                        {
                            if (BTrip.LocationID == bt.LocationID)
                            {
                                return Json(new { error = btDuplication });
                            }
                        }

                        if (isBetween(BTrip.StartDate, bt) || isBetween(BTrip.EndDate, bt) || isBetween(bt.StartDate, BTrip) || isBetween(bt.EndDate, BTrip))
                        {
                            return Json(new { error = btDatesOverlay });
                        }
                    }
                }

                businessTrip = RewriteBTsPropsAfterPlanningFromRepository(BTrip);
                businessTrip.RejectComment = null;
            }

            if (ModelState.IsValid)
            {

                try
                {
                    AddLastCRUDDataToBT(businessTrip);
                    businessTrip.Location = repository.Locations.Where(l => l.LocationID == businessTrip.LocationID).FirstOrDefault();
                    businessTrip.Unit = repository.Units.Where(l => l.UnitID == businessTrip.UnitID).FirstOrDefault();
                    repository.SaveBusinessTrip(businessTrip);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                catch (VacationAlreadyExistException)
                {
                    Console.WriteLine("VacancyAlreadyExistException");
                    return Json(new { error = btCreationError });
                }

                IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
                return View("TableViewBTADM", empList.ToList());
            }

            BusinessTripViewModel bTripModel = new BusinessTripViewModel(businessTrip);
            return View("Plan", bTripModel);
        }


        public bool isBetween(DateTime date, BusinessTrip btFromRepository)
        {
            if ((date > btFromRepository.StartDate) && (date < btFromRepository.EndDate))
                return true;
            else
                return false;
        }


        [Authorize(Roles = "ADM")]
        public BusinessTrip RewriteBTsPropsAfterPlanningFromRepository(BusinessTrip businessTripModel)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == businessTripModel.BusinessTripID).FirstOrDefault();

            if (businessTripFromRepository.Status.HasFlag(BTStatus.Modified))
            {
                businessTripFromRepository.UnitID = businessTripFromRepository.UnitID;
                businessTripFromRepository.OldLocationID = businessTripFromRepository.LocationID;
                businessTripFromRepository.OldLocationTitle = businessTripFromRepository.Location.Title;
                businessTripFromRepository.OldStartDate = businessTripFromRepository.StartDate;
                businessTripFromRepository.OldEndDate = businessTripFromRepository.EndDate;
            }

            //businessTrip.LocationID = businessTripModel.LocationID;
            //businessTrip.Location = repository.Locations.Where(l => l.LocationID == businessTrip.LocationID).FirstOrDefault();
            //businessTrip.StartDate = businessTripModel.StartDate;
            //businessTrip.EndDate = businessTripModel.EndDate;
            //businessTrip.Purpose = businessTripModel.Purpose;
            //businessTrip.Manager = businessTripModel.Manager;
            //businessTrip.Responsible = businessTripModel.Responsible;
            //businessTrip.Comment = businessTripModel.Comment;
            //businessTrip.RowVersion = businessTripModel.RowVersion;
            businessTripFromRepository.UnitID = businessTripFromRepository.UnitID;
            businessTripModel.BTof = businessTripFromRepository.BTof;
            businessTripModel.CancelComment = businessTripFromRepository.CancelComment;
            businessTripModel.Flights = businessTripFromRepository.Flights;
            businessTripModel.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            businessTripModel.Habitation = businessTripFromRepository.Habitation;
            businessTripModel.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            businessTripModel.Invitation = businessTripFromRepository.Invitation;
            businessTripModel.OldEndDate = businessTripFromRepository.OldEndDate;
            businessTripModel.OldLocationID = businessTripFromRepository.OldLocationID;
            businessTripModel.OldLocationTitle = businessTripFromRepository.OldLocationTitle;
            businessTripModel.OldStartDate = businessTripFromRepository.OldStartDate;
            businessTripModel.RejectComment = businessTripFromRepository.RejectComment;
            businessTripModel.BTMComment = businessTripFromRepository.BTMComment;
            businessTripModel.Status = businessTripFromRepository.Status;
            businessTripModel.LastCRUDedBy = businessTripFromRepository.LastCRUDedBy;
            businessTripModel.LastCRUDTimestamp = businessTripFromRepository.LastCRUDTimestamp;
            businessTripModel.OrderStartDate = businessTripFromRepository.OrderStartDate;
            businessTripModel.OrderEndDate = businessTripFromRepository.OrderEndDate;
            businessTripModel.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;


            return businessTripModel;
        }

        //
        // GET: /BusinessTrip/Edit/5

        [Authorize(Roles = "ADM")]
        public ActionResult EditPlannedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (businessT == null || (!businessT.Status.HasFlag(BTStatus.Planned)))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = businessT.BTof.LastName + " " + businessT.BTof.FirstName + " (" + businessT.BTof.EID + ") from " + businessT.BTof.Department.DepartmentName;
            }

            BusinessTripViewModel businessTrip = new BusinessTripViewModel(businessT);
            ViewBag.SelectedDepartment = selectedDepartment;
            LocationsDropDownList(businessT.LocationID);
            UnitsDropDownList(businessT.UnitID);
            return View("EditPlannedBT", businessTrip);
        }

        //[Authorize(Roles = "ADM")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ProcessCommand(BusinessTrip businesstrip, int id = 0, string commandName = "", string[] selectedPlannedBTs = null, string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        //{
        //    switch (commandName)
        //    {

        //        case "EditPlannedBT":
        //            return RedirectToAction("EditPlannedBT", new { id = id });

        //        case "EditRegisteredBT":
        //            return RedirectToAction("EditRegisteredBT", new { id = id });

        //        case "EditConfirmedBT":
        //            return RedirectToAction("EditConfirmedBT", new { id = id });

        //        case "Register":
        //            return this.RegisterPlannedBTs(selectedPlannedBTs, selectedDepartment);

        //        case "Confirm":
        //            return this.ConfirmPlannedBTs(selectedPlannedBTs, selectedDepartment);

        //        case "Plan ":
        //            return this.Plan(businesstrip, selectedDepartment);

        //        //case "Register ":
        //        //    return this.RegisterPlannedBT(businesstrip, selectedDepartment);

        //        //case "Confirm ":
        //        //    return this.ConfirmPlannedBT(businesstrip, selectedDepartment);

        //        case "Confirm  ":
        //            return this.ConfirmRegisteredBTs(selectedRegisteredBTs, selectedDepartment);

        //        case "Replan":
        //            return this.ReplanRegisteredBTs(selectedRegisteredBTs, selectedDepartment);

        //        case "Cancel":
        //            return this.CancelRegisteredBTs(selectedRegisteredBTs, selectedDepartment);

        //        case "Cancel ":
        //            return this.CancelConfirmedBT(id, selectedDepartment);

        //        default:
        //            return RedirectToAction("ADMView", "Home", new { tab = 1, selectedDepartment = selectedDepartment });
        //    }
        //}

        #endregion

        #region BTM
        #region Commented methods for dropdownlist
        // ****** Important please do not delete when doing merge
        /* for right work of dropdownlist 
           must pass and take selectedDepartment as argument
           in methods which make POST request to save
        */

        //public PartialViewResult GetBusinessTripDataBTM(string selectedDepartment = "")
        //{
        //    var query = from e in repository.Employees
        //                join dep in repository.Departments on e.DepartmentID equals dep.DepartmentID
        //                where ((selectedDepartment == null||selectedDepartment == String.Empty || dep.DepartmentName == selectedDepartment) && (e.DateDismissed == null))
        //                orderby e.IsManager descending, e.LastName
        //                select e;

        //    ViewBag.JSDAtePattern = MvcApplication.JSDatePattern;
        //    ViewBag.SelectedDepartment = selectedDepartment;
        //    return PartialView(query.ToList());
        //}


        //public ActionResult GetBusinessTripBTM(string selectedDepartment = null)
        //{
        //    var selected = from dep in repository.Departments
        //                   orderby dep.DepartmentName
        //                   select dep;

        //    ViewBag.DepartmentsList = new SelectList(selected, "DepartmentName", "DepartmentName");
        //    ViewBag.SelectedDepartment = selectedDepartment;
        //    return View();
        //}
        #endregion


        public PartialViewResult GetBusinessTripDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selectedEmpl = repository.Employees.ToList();

            selectedEmpl = SearchBusinessTripDataBTM(selectedEmpl, searchString);

            ViewBag.JSDAtePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            // ViewBag.SelectedDepartment = selectedDepartment;
            return PartialView(selectedEmpl);
        }


        public ActionResult GetBusinessTripBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        public List<Employee> SearchBusinessTripDataBTM(List<Employee> empList, string searchString)
        {
            List<Employee> selectedEmpl = (from emp in empList
                                           where emp.DateDismissed == null && emp.BusinessTrips.Count != 0 &&
                                           (emp.EID.ToLower().Contains(searchString.ToLower())
                                                || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                                || emp.LastName.ToLower().Contains(searchString.ToLower()))

                                           orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                                           select emp).ToList();

            return selectedEmpl;
        }

        [Authorize(Roles = "BTM")]
        public ActionResult BTMArrangeBT(int id = 0, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            BusinessTrip bt = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (bt == null)
            {
                return HttpNotFound();
            }

            if (bt.Status.HasFlag(BTStatus.Confirmed)
                && bt.OrderEndDate == null
                && bt.OrderStartDate == null)
            {
                bt.OrderStartDate = bt.StartDate.AddDays(-1);
                bt.OrderEndDate = bt.EndDate.AddDays(1);
                bt.DaysInBtForOrder = (bt.OrderEndDate.Value - bt.OrderStartDate.Value).Days + 1;
            }
            BusinessTripViewModel businesstrip = new BusinessTripViewModel(bt);

            return View(businesstrip);
        }

        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveArrangedBT(BusinessTrip bTrip, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            bTrip = RewriteBTsPropsFromRepositoryBTM(bTrip);

            if (ModelState.IsValid)
            {
                if ((bTrip.Status == (BTStatus.Confirmed | BTStatus.Modified)) || (bTrip.Status == (BTStatus.Registered | BTStatus.Modified)))
                {
                    bTrip.Status = bTrip.Status & ~BTStatus.Modified;
                }
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                try
                {
                    repository.SaveBusinessTrip(bTrip);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                catch (VacationAlreadyExistException)
                {
                    return Json(new { error = btCreationError });
                }
                selectedBusinessTripsList.Add(bTrip);
                if (bTrip.Status == BTStatus.Confirmed)
                {
                    messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author)); 
                    messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                }
            }
            else
            {
                return View("BTMArrangeBT", new BusinessTripViewModel(bTrip));
            }

            //return RedirectToAction("BTMView", "Home", new { tab = 1, searchString = searchString });
            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
            // return View("OneRowBTM", empList);
        }

        [Authorize(Roles = "BTM")]
        public BusinessTrip RewriteBTsPropsFromRepositoryBTM(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();
            //bTrip.BTMComment = businessTripFromRepository.BTMComment;
            //bTrip.Habitation = businessTripFromRepository.Habitation;
            //bTrip.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            //bTrip.Flights = businessTripFromRepository.Flights;
            //bTrip.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            //bTrip.Invitation = businessTripFromRepository.Invitation;
            //bTrip.RowVersion = businessTripFromRepository.RowVersion;

            //if ( !( (bTrip.Status == BTStatus.Confirmed || bTrip.Status == (BTStatus.Confirmed | BTStatus.Modified)) && (bTrip.Status != BTStatus.Cancelled) && (bTrip.Status != BTStatus.Reported)) )
            if ((bTrip.Status == (BTStatus.Planned | BTStatus.Modified)) || (bTrip.Status.HasFlag(BTStatus.Registered)))
            {
                bTrip.OrderStartDate = businessTripFromRepository.OrderStartDate;
                bTrip.OrderEndDate = businessTripFromRepository.OrderEndDate;
                bTrip.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;
            }

            if (bTrip.RejectComment == null || bTrip.RejectComment == String.Empty)
            {
                bTrip.RejectComment = businessTripFromRepository.RejectComment;
            }


            if (bTrip.AccComment == null || bTrip.AccComment == String.Empty || bTrip.AccComment == defaultAccComment)
            {
                bTrip.AccComment = businessTripFromRepository.AccComment;
            }

            bTrip.CancelComment = businessTripFromRepository.CancelComment;
            bTrip.Comment = businessTripFromRepository.Comment;
            bTrip.EmployeeID = businessTripFromRepository.EmployeeID;
            bTrip.BTof = businessTripFromRepository.BTof;
            bTrip.EndDate = businessTripFromRepository.EndDate;
            bTrip.LocationID = businessTripFromRepository.LocationID;
            bTrip.Manager = businessTripFromRepository.Manager;
            bTrip.OldEndDate = businessTripFromRepository.OldEndDate;
            bTrip.OldLocationID = businessTripFromRepository.OldLocationID;
            bTrip.OldLocationTitle = businessTripFromRepository.OldLocationTitle;
            bTrip.OldStartDate = businessTripFromRepository.OldStartDate;
            bTrip.Purpose = businessTripFromRepository.Purpose;
            bTrip.Responsible = businessTripFromRepository.Responsible;
            bTrip.StartDate = businessTripFromRepository.StartDate;
            bTrip.Status = businessTripFromRepository.Status;
            bTrip.LastCRUDedBy = businessTripFromRepository.LastCRUDedBy;
            bTrip.LastCRUDTimestamp = businessTripFromRepository.LastCRUDTimestamp;
            bTrip.Location = businessTripFromRepository.Location;

            return bTrip;
        }


        [Authorize(Roles = "BTM")]
        public BusinessTrip RewriteBTsPropsFromRepositoryWhenReject(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();

            //all editable data for BTM
            //bTrip.RowVersion = businessTripFromRepository.RowVersion;

            bTrip.BTMComment = businessTripFromRepository.BTMComment;
            bTrip.Habitation = businessTripFromRepository.Habitation;
            bTrip.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            bTrip.Flights = businessTripFromRepository.Flights;
            bTrip.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            bTrip.Invitation = businessTripFromRepository.Invitation;
            bTrip.OrderStartDate = businessTripFromRepository.OrderStartDate;
            bTrip.OrderEndDate = businessTripFromRepository.OrderEndDate;
            bTrip.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;

            return bTrip;
        }

        //[Authorize(Roles = "BTM")]
        //public BusinessTrip RewriteBTsPropsFromRepositoryBTMWithOrdersDates(BusinessTrip bTrip)
        //{
        //    BusinessTrip businessTripFromRepository = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault());

        //    //businessTrip.BTMComment = businessTripModel.BTMComment;
        //    //businessTrip.Habitation = businessTripModel.Habitation;
        //    //businessTrip.HabitationConfirmed = businessTripModel.HabitationConfirmed;
        //    //businessTrip.Flights = businessTripModel.Flights;
        //    //businessTrip.FlightsConfirmed = businessTripModel.FlightsConfirmed;
        //    //businessTrip.Invitation = businessTripModel.Invitation;
        //    //businessTrip.OrderStartDate = businessTripModel.OrderStartDate;
        //    //businessTrip.OrderEndDate = businessTripModel.OrderEndDate;
        //    //businessTrip.DaysInBtForOrder = businessTripModel.DaysInBtForOrder;
        //    //businessTrip.RowVersion = businessTripModel.RowVersion;

        //    bTrip.CancelComment = businessTripFromRepository.CancelComment;
        //    bTrip.Comment = businessTripFromRepository.Comment;
        //    bTrip.EmployeeID = businessTripFromRepository.EmployeeID;
        //    bTrip.BTof = businessTripFromRepository.BTof;
        //    bTrip.EndDate = businessTripFromRepository.EndDate;
        //    bTrip.LocationID = businessTripFromRepository.LocationID;
        //    bTrip.Manager = businessTripFromRepository.Manager;
        //    bTrip.OldEndDate = businessTripFromRepository.OldEndDate;
        //    bTrip.OldLocationID = businessTripFromRepository.OldLocationID;
        //    bTrip.OldLocationTitle = businessTripFromRepository.OldLocationTitle;
        //    bTrip.OldStartDate = businessTripFromRepository.OldStartDate;
        //    bTrip.Purpose = businessTripFromRepository.Purpose;
        //    bTrip.RejectComment = businessTripFromRepository.RejectComment;
        //    bTrip.Responsible = businessTripFromRepository.Responsible;
        //    bTrip.StartDate = businessTripFromRepository.StartDate;
        //    bTrip.Status = businessTripFromRepository.Status;
        //    bTrip.LastCRUDedBy = businessTripFromRepository.LastCRUDedBy;
        //    bTrip.LastCRUDTimestamp = businessTripFromRepository.LastCRUDTimestamp;
        //    bTrip.Location = businessTripFromRepository.Location;

        //    return bTrip;
        //}

        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportConfirmedBTs(string[] selectedConfirmedBTs = null, string searchString = "")
        {


            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (selectedConfirmedBTs != null && selectedConfirmedBTs.Length != 0)
            {
                Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedConfirmedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = null;
                        if (repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault() != null)
                            selectedBT = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault());
                        if (selectedBT != null
                             && (selectedBT.Status == (BTStatus.Confirmed)
                             || (selectedBT.Status == (BTStatus.Confirmed | BTStatus.Modified))))
                        {
                            selectedBT.Status = BTStatus.Confirmed | BTStatus.Reported;
                            if (selectedBT.BTof.Visa != null)
                            {
                                selectedBT.BTof.Visa.DaysUsedInBT += CountingDaysUsedInBT(selectedBT);
                                selectedBT.BTof.Visa.EntriesUsedInBT++;
                            }
                            try
                            {
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));

                }
            }
            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
        }

        [Authorize(Roles = "BTM")]
        //public ActionResult Reject_BT_BTM(int id = 0, string searchString = "")
        public ActionResult Reject_BT_BTM(int id = 0, string jsonRowVersionData = "", string searchString = "")
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            if (jsonRowVersionData != "")
            {
                businessTrip.RowVersion = JsonConvert.DeserializeObject<byte[]>(jsonRowVersionData);
            }

            ViewBag.SearchString = searchString;
            BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);
            return View(btripModel);
        }

        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("Reject_BT_BTM")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject_BT_BTM_Confirm(BusinessTrip businessTrip, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                businessTrip = RewriteBTsPropsFromRepositoryWhenReject(businessTrip);
                businessTrip = RewriteBTsPropsFromRepositoryBTM(businessTrip);

                if ((businessTrip.RejectComment != null) && (businessTrip.RejectComment != String.Empty))
                {
                    Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                    BTStatus oldBTStatus = businessTrip.Status;
                    businessTrip.Status = BTStatus.Planned | BTStatus.Modified;
                    businessTrip.OrderStartDate = null;
                    businessTrip.OrderEndDate = null;
                    businessTrip.DaysInBtForOrder = null;
                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                    selectedBusinessTripsList.Add(businessTrip);
                    if (oldBTStatus == (BTStatus.Registered | BTStatus.Modified) || (oldBTStatus == BTStatus.Registered))
                    {
                        messenger.Notify(new Message(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                    }
                    if (oldBTStatus == (BTStatus.Confirmed | BTStatus.Modified) || (oldBTStatus == BTStatus.Confirmed))
                    {
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
                    return View("TableViewBTM", empList);
                }
                else
                {
                    ModelState.AddModelError("", "Please, enter reject comment.");
                    BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);
                    return View("Reject_BT_BTM", btripModel);
                }
            }
        }

        //GET: Delete BT by BTM

        [Authorize(Roles = "BTM")]
        public ActionResult DeleteBTBTM(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT == null || !selectedBT.Status.HasFlag(BTStatus.Cancelled))
            {
                return HttpNotFound();
            }
            return View(selectedBT);
        }

        //POST: Delete BT by BTM

        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("DeleteBTBTM")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBTBTMConfirmed(int id = 0, string searchString = "")
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            ViewBag.SearchString = searchString;

            if (selectedBT != null)
            {
                if (selectedBT.Status.HasFlag(BTStatus.Cancelled))
                {
                    repository.DeleteBusinessTrip(id);
                }
            }
            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
        }

        #endregion

        #region ADM, BTM, ACC

        [Authorize(Roles = "ADM, BTM, ACC")]
        public ActionResult ShowBTData(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel businessTripModel = new BusinessTripViewModel(businessTrip);

            return View(businessTripModel);
        }

        //[Authorize(Roles = "ADM, BTM, ACC")]
        //public ActionResult ShowBTsDataForEmployee(int id = 0)
        //{
        //    IEnumerable<BusinessTrip> businesstrips = repository.BusinessTrips
        //        .Where(b => (b.EmployeeID == id)
        //           && ((b.Status == (BTStatus.Confirmed | BTStatus.Reported)) /*|| (b.Status == (BTStatus.Confirmed | BTStatus.Cancelled)) */ ))
        //        .OrderByDescending(b => b.StartDate);

        //    ViewBag.EmployeeInformation = repository.Employees.Where(e => e.EmployeeID == id).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();
        //    if (businesstrips.Count() == 0)
        //    {
        //        return View("NoLastBTs");
        //    }

        //    return View(businesstrips);
        //}

        #endregion

        #region DIR
        [Authorize(Roles = "DIR")]
        public PartialViewResult GetBusinessTripDataDIR(string selectedDepartment = null)
        {
            DateTime StartDateToCompare = DateTime.Now.ToLocalTimeAzure().Date;
            var query = from bt in repository.BusinessTrips
                        join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                        join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                        where (selectedDepartment == null || selectedDepartment == String.Empty || d.DepartmentName == selectedDepartment)
                              && (e.DateDismissed == null)
                              && ((bt.Status == BTStatus.Confirmed || (bt.Status == (BTStatus.Confirmed | BTStatus.Modified))) && (bt.StartDate > StartDateToCompare))
                        orderby e.IsManager descending, e.LastName, bt.StartDate
                        select bt;

            ViewBag.SelectedDepartment = selectedDepartment;

            return PartialView(query.ToList());
        }

        [Authorize(Roles = "DIR")]
        public ActionResult GetBusinessTripDIR(string selectedDepartment = null)
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;

            return View();
        }


        //
        // GET: /BusinessTrip/Reject/
        [Authorize(Roles = "DIR")]
        public ActionResult Reject_BT_DIR(int id = 0, string jsonRowVersionData = "", string selectedDepartment = null)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            if (jsonRowVersionData != "")
            {
                businessTrip.RowVersion = JsonConvert.DeserializeObject<byte[]>(jsonRowVersionData);
            }

            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);

            return View(btripModel);
        }


        //
        // POST: /BusinessTrip/Reject/5

        [Authorize(Roles = "DIR")]
        [HttpPost, ActionName("Reject_BT_DIR")]
        [ValidateAntiForgeryToken]
        //public ActionResult Reject_BT_DIR_Confirm(int id = 0, string rejectComment = "", string selectedDepartment = null)
        public ActionResult Reject_BT_DIR_Confirm(BusinessTrip businessTrip, string selectedDepartment = null)
        {
            //BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                businessTrip = RewriteBTsPropsFromRepositoryWhenReject(businessTrip);
                businessTrip = RewriteBTsPropsFromRepositoryBTM(businessTrip);

                if ((businessTrip.RejectComment != null) && (businessTrip.RejectComment != String.Empty))
                {
                    Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                    businessTrip.Status = BTStatus.Planned | BTStatus.Modified;
                    businessTrip.OrderStartDate = null;
                    businessTrip.OrderEndDate = null;
                    businessTrip.DaysInBtForOrder = null;

                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                    selectedBusinessTripsList.Add(businessTrip);
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToADM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToACC, selectedBusinessTripsList, author));
                    return RedirectToAction("DIRView", "Home", new { tab = 0, selectedDepartment = selectedDepartment });
                }
                else
                {
                    ModelState.AddModelError("", "Please, enter reject comment.");
                    BusinessTripViewModel bTripModel = new BusinessTripViewModel(businessTrip);
                    ViewBag.SelectedDepartment = selectedDepartment;

                    return View(bTripModel);
                }
            }
        }

        #endregion

        #region ACC
        [Authorize(Roles = "ACC")]
        public PartialViewResult GetBusinessTripDataACC(string selectedDepartment = null, string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<BusinessTrip> selectedData = new List<BusinessTrip>();

            selectedData = SearchBusinessTripDataACC(repository.BusinessTrips.ToList(), selectedDepartment, searchString);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(selectedData);
        }

        public List<BusinessTrip> SearchBusinessTripDataACC(List<BusinessTrip> btList, string selectedDepartment, string searchString)
        {
            List<BusinessTrip> query = (from bt in btList
                                        join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                                        join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                                        where ((selectedDepartment == null || selectedDepartment == String.Empty || d.DepartmentName == selectedDepartment)
                                              && (e.DateDismissed == null && (e.EID.ToLower().Contains(searchString.ToLower()) || e.FirstName.ToLower().Contains(searchString.ToLower()) || e.LastName.ToLower().Contains(searchString.ToLower())))
                                              && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && ((bt.EndDate.Date >= DateTime.Now.ToLocalTimeAzure().Date) || (bt.AccComment == null || bt.AccComment == "" || bt.AccComment == defaultAccComment))))
                                        orderby bt.StartDate, e.LastName
                                        select bt).ToList();
            return query;
        }


        [Authorize(Roles = "ACC")]
        public ActionResult GetBusinessTripACC(string selectedDepartment = null, string searchString = "")
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View();
        }


        //
        // GET: /BusinessTrip/EditReportedBTACC/5

        [Authorize(Roles = "ACC")]
        public ActionResult EditReportedBT(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            BusinessTrip businessT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessT == null || (!businessT.Status.HasFlag(BTStatus.Confirmed | BTStatus.Reported)))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = businessT.BTof.LastName + " " + businessT.BTof.FirstName + " (" + businessT.BTof.EID + ") from " + businessT.BTof.Department.DepartmentName;
            }

            if (businessT.AccComment == null || businessT.AccComment == String.Empty)
            {
                businessT.AccComment = defaultAccComment;
            }

            BusinessTripViewModel businessTrip = new BusinessTripViewModel(businessT);

            ViewBag.SelectedDepartment = selectedDepartment;
            LocationsDropDownList(businessT.LocationID);

            if (businessT.StartDate > DateTime.Now.ToLocalTimeAzure().Date && businessT.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFutureBT", businessTrip);
            }
            else if (businessT.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedForAccComment", businessTrip);
            }
            else
            {
                return View("EditReportedCurrentBT", businessTrip);
            }

        }

        //
        // POST: /BusinessTrip/EditReportedBTACC/

        [Authorize(Roles = "ACC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReportedBT(BusinessTrip businessTrip, string selectedDepartment = null)
        {
            BusinessTrip btFromRepository = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == businessTrip.BusinessTripID).FirstOrDefault());

            if (businessTrip == null || btFromRepository == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (ModelState.IsValid && businessTrip.Status.HasFlag(BTStatus.Reported))
                {
                    LocationsDropDownList(businessTrip.LocationID);

                    if (!(businessTrip.StartDate == btFromRepository.StartDate
                        && businessTrip.EndDate == btFromRepository.EndDate
                        && businessTrip.LocationID == btFromRepository.LocationID
                        && businessTrip.OrderStartDate == btFromRepository.OrderStartDate
                        && businessTrip.OrderEndDate == btFromRepository.OrderEndDate))
                    {
                        Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                        List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();

                        businessTrip.Status = (btFromRepository.Status | BTStatus.Modified) & ~BTStatus.Reported;
                        businessTrip.OldStartDate = btFromRepository.StartDate;
                        businessTrip.OldEndDate = btFromRepository.EndDate;
                        businessTrip.OldLocationID = btFromRepository.LocationID;
                        businessTrip.OldLocationTitle = btFromRepository.Location.Title;

                        int oldDaysUsedInBT = CountingDaysUsedInBT(btFromRepository);

                        //btFromRepository.StartDate = businessTrip.StartDate;
                        //btFromRepository.EndDate = businessTrip.EndDate;
                        //btFromRepository.LocationID = businessTrip.LocationID;
                        //btFromRepository.OrderStartDate = businessTrip.OrderStartDate;
                        //btFromRepository.OrderEndDate = businessTrip.OrderEndDate;
                        //btFromRepository.DaysInBtForOrder = businessTrip.DaysInBtForOrder;
                        //btFromRepository.RowVersion = businessTrip.RowVersion;

                        businessTrip.BTof = btFromRepository.BTof;
                        businessTrip.CancelComment = btFromRepository.CancelComment;
                        businessTrip.Comment = btFromRepository.Comment;
                        businessTrip.Flights = btFromRepository.Flights;
                        businessTrip.FlightsConfirmed = btFromRepository.FlightsConfirmed;
                        businessTrip.Habitation = btFromRepository.Habitation;
                        businessTrip.HabitationConfirmed = btFromRepository.HabitationConfirmed;
                        businessTrip.Invitation = btFromRepository.Invitation;
                        businessTrip.Manager = btFromRepository.Manager;
                        businessTrip.Purpose = btFromRepository.Purpose;
                        businessTrip.RejectComment = btFromRepository.RejectComment;
                        businessTrip.BTMComment = btFromRepository.BTMComment;
                        businessTrip.Responsible = btFromRepository.Responsible;
                        businessTrip.LastCRUDedBy = btFromRepository.LastCRUDedBy;
                        businessTrip.LastCRUDTimestamp = btFromRepository.LastCRUDTimestamp;
                        businessTrip.Location = btFromRepository.Location;
                        businessTrip.AccComment = btFromRepository.AccComment;



                        if (businessTrip.BTof.Visa != null)
                        {
                            businessTrip.BTof.Visa.DaysUsedInBT -= oldDaysUsedInBT;
                            businessTrip.BTof.Visa.EntriesUsedInBT--;
                        }

                        try
                        {
                            repository.SaveBusinessTrip(businessTrip);
                            selectedBusinessTripsList.Add(businessTrip);

                            messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToADM, selectedBusinessTripsList, author));
                            messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToBTM, selectedBusinessTripsList, author));
                            messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToDIR, selectedBusinessTripsList, author));
                            messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToEMP, selectedBusinessTripsList, author));
                            messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToResponsible, selectedBusinessTripsList, author)); 
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Json(new { error = modelError });
                        }


                    }

                    //return RedirectToAction("ACCView", "Home", new { tab = 0, selectedDepartment = selectedDepartment });
                    return Json(new { success = "success" });

                }
            }
            BusinessTripViewModel btTripModel = new BusinessTripViewModel(businessTrip);
            LocationsDropDownList(businessTrip.LocationID);
            btTripModel.BTof = repository.BusinessTrips.Where(bt => bt.BusinessTripID == businessTrip.BusinessTripID).Select(b => b.BTof).FirstOrDefault();
            ViewBag.SelectedDepartment = selectedDepartment;

            if (businessTrip.StartDate > DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFutureBT", btTripModel);
            }
            else
            {
                return View("EditReportedCurrentBT", btTripModel);
            }
        }

        [Authorize(Roles = "ACC")]
        public ViewResult IndexACCforAccountableBTs()
        {
            var query = from bt in repository.BusinessTrips as IEnumerable<BusinessTrip>
                        join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                        join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                        where (e.DateDismissed == null
                              && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported))
                              && ((bt.OrderEndDate != null && bt.OrderEndDate.Value.Date <= DateTime.Now.ToLocalTimeAzure().Date) && (bt.OrderEndDate != null && bt.OrderEndDate.Value.Date >= (DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5))))))
                        orderby bt.OrderEndDate.Value descending, e.LastName
                        select bt;

            return View(query.ToList());
        }

        [Authorize(Roles = "ACC")]
        public ActionResult CancelReportedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel btTripmodel = new BusinessTripViewModel(businessTrip);
            return View(btTripmodel);
        }

        [Authorize(Roles = "ACC")]
        [HttpPost, ActionName("CancelReportedBT")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReportedBTConfirm(int id = 0, string cancelComment = "", string selectedDepartment = null)
        {
            BusinessTrip businessTrip = null;
            if (repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault() == null)
                return HttpNotFound();
            else
            {
                businessTrip = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault());
                businessTrip.CancelComment = cancelComment;

                if ((businessTrip.Status == (BTStatus.Confirmed | BTStatus.Reported))
                    && ((businessTrip.CancelComment != null) && (businessTrip.CancelComment != String.Empty)))
                {
                    Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                    businessTrip.Status = BTStatus.Confirmed | BTStatus.Cancelled;

                    if (businessTrip.BTof.Visa != null)
                    {
                        businessTrip.BTof.Visa.DaysUsedInBT -= CountingDaysUsedInBT(businessTrip);
                        businessTrip.BTof.Visa.EntriesUsedInBT--;
                    }
                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                    selectedBusinessTripsList.Add(businessTrip);
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToADM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToResponsible, selectedBusinessTripsList, author));
                    //return RedirectToAction("ACCView", "Home", new { tab = 0, selectedDepartment = selectedDepartment });
                    return Json(new { success = "success" });

                }
                else
                {
                    ModelState.AddModelError("", "Please, enter cancel comment.");
                    BusinessTripViewModel bTripmodel = new BusinessTripViewModel(businessTrip);
                    ViewBag.SelectedDepartment = selectedDepartment;

                    return View(bTripmodel);
                }
            }
        }


        [Authorize(Roles = "ACC")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveAccComment(BusinessTrip bTrip)
        {
            if (bTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (bTrip.Status == (BTStatus.Confirmed | BTStatus.Reported))
                {
                    bTrip = RewriteBTsPropsFromRepositoryBTM(bTrip);
                    bTrip = RewriteBTsPropsFromRepositoryWhenReject(bTrip);

                    try
                    {
                        repository.SaveBusinessTrip(bTrip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }

                    //return RedirectToAction("ACCView", "Home", new { selectedDepartment = selectedDepartment });
                    return Json(new { success = "success" });
                }
            }
            return Json(new { error = "error" });
        }

        [Authorize(Roles = "ACC")]
        public ActionResult ShowAccountableBTData(int id = 0)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            BusinessTripViewModel businessTripModel = new BusinessTripViewModel(businessTrip);

            return View(businessTripModel);
        }


        #endregion

        #region BTM, ACC

        [Authorize(Roles = "BTM, ACC")]
        public int CountingDaysUsedInBT(BusinessTrip businessTrip)
        {
            //'+1' day for counting last day too
            return ((businessTrip.EndDate - businessTrip.StartDate).Days + 1);
        }

        #endregion

    }
}