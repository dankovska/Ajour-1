using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    [Authorize(Roles = "ABM")]
    public class HolidayController : Controller
    {
        private IRepository repository;
        public HolidayController(IRepository repo)
        {
            this.repository = repo;
        }
        
        public ActionResult GetHoliday()
        {        
            ViewBag.YearDropdownList = YearDropDownList();
            ViewBag.CountryDropdownList = CountryDropDownList();
                 
            var year = (from hol in repository.Holidays
                        orderby hol.HolidayDate descending
                        select hol.HolidayDate.Year).FirstOrDefault();
            if (repository.Holidays.Any(h => h.HolidayDate.Year == DateTime.Now.Year))
            {
                ViewBag.DefaultYear = DateTime.Now.Year;
            }
            else
            {
                ViewBag.DefaultYear = year;
            }
            ViewBag.DefaultCountry = SelectDefaultCountryID();
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return View();
        }

        public SelectList CountryDropDownList()
        {
            var countryList = (from countries in repository.Countries
                               select countries).ToList();
            int countryID = SelectDefaultCountryID();
            return new SelectList(countryList, "CountryID", "CountryName", countryID);
        }

        public int SelectDefaultCountryID()
        {
            return (from country in repository.Countries
                    where country.CountryName == "Ukraine"
                    select country.CountryID).First();

        }

        public SelectList YearDropDownList()
        {
            var yearList = (from hol in repository.Holidays
                            select hol.HolidayDate.Date.Year).Distinct().ToList();

            var year = (from hol in repository.Holidays
                        orderby hol.HolidayDate descending
                        select hol.HolidayDate.Year).FirstOrDefault();
                       
            
              if (repository.Holidays.Any(h => h.HolidayDate.Year == DateTime.Now.Year))
              {
                  return new SelectList(yearList, DateTime.Now.Year );
              }
              else
                      return new SelectList(yearList, year);
            
        }

        public PartialViewResult GetHolidayData(string selectedYear, string selectedCountryID)
        {
            List<Holiday> holList = (from holiday in repository.Holidays
                                     where (holiday.HolidayDate.Year.ToString() == selectedYear && holiday.CountryID == Int32.Parse(selectedCountryID))
                                     orderby holiday.HolidayDate
                                     select holiday).ToList();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(holList);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Holiday holiday = (from h in repository.Holidays where h.HolidayID == id select h).FirstOrDefault();

            if (holiday == null)
            {
                return HttpNotFound();
            }

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return View(holiday);
        }

        [HttpPost]
        public ActionResult Edit(Holiday holiday, string date, int countryID, bool isPostponed)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    repository.SaveHoliday(holiday, DateTime.Parse(date), countryID, isPostponed);
                    return RedirectToAction("ABMView", "Home", new { tab = Tabs.ABM.Holidays });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                     + "was modified by another user after you got the original value. The "
                                     + "edit operation was canceled.";
            }

            return Json(new { error = ModelError });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            Holiday holiday = (from h in repository.Holidays where h.HolidayID == id select h).FirstOrDefault();
            if (holiday == null)
            {
                return HttpNotFound();
            }

            return View(holiday);
        }

        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteHoliday(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            return RedirectToAction("ABMView", "Home", new { tab = Tabs.ABM.Holidays});
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CountryList = CountryDropDownList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Holiday holiday, string HolidayDate, int countryID, bool isPostponed)
        {
            if (ModelState.IsValid)
            {
                DateTime prs = DateTime.Parse(HolidayDate);
                repository.SaveHoliday(holiday, prs, countryID, isPostponed);
                return RedirectToAction("ABMView", "Home", new {tab = Tabs.ABM.Holidays});
            }

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return View(holiday);
        }
    }
}
