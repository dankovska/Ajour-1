using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EID { get; set; }

        [Required]
        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        public DateTime DateEmployed { get; set; }

        public int PositionID { get; set; }
        public virtual Position Position { get; set; }

        public DateTime? BirthDay { get; set; }
        public string Comment { get; set; }
        public string FullNameUk { get; set; }

        [GreaterThan("DateEmployed", ErrorMessage = "Date Dismissed should be greater than Date Employed")]
        public DateTime? DateDismissed { get; set; }
        public bool IsManager { get; set; }
        public virtual List<BusinessTrip> BusinessTrips { get; set; }
        public virtual Visa Visa { get; set; }
        public virtual VisaRegistrationDate VisaRegistrationDate { get; set; }
        public virtual Permit Permit { get; set; }
        public virtual Passport Passport { get; set; }
        public virtual List<CalendarItem> CalendarItems { get; set;}
        public virtual List<Overtime> Overtimes { get; set; }
        public virtual List<Vacation> Vacations { get; set; }
        public virtual List<Sickness> Sicknesses { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string BTRestrictions { get; set; }
    }
}

