using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Models
{
    public class AbsenceViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Field First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Field Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Field EID is required")]
        public string EID { get; set; }

        public string Department { get; set; }

        public List<CalendarItem> Journeys { get; set; }
        public List<CalendarItem> BusinessTrips { get; set; }
        public List<CalendarItem> Overtimes { get; set; }
        public List<CalendarItem> Sickness { get; set; }
        public List<CalendarItem> Vacations { get; set; }
    }
}