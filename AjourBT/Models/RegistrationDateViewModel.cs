using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Models
{
    public class RegistrationDateViewModel
    {

        public int EmployeeID { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string VisaType { get; set; }
        [Required]
        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [Display(Name = "Registration Time")]
        public string RegistrationTime { get; set; }
        
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } 

        public RegistrationDateViewModel(VisaRegistrationDate visaRegistrationDate)
        {
            EmployeeID = visaRegistrationDate.EmployeeID;
            VisaType = visaRegistrationDate.VisaType;
            RegistrationDate = string.Format("{0:d}", visaRegistrationDate.RegistrationDate);
            RegistrationTime = visaRegistrationDate.RegistrationTime;
            City = visaRegistrationDate.City;
            RegistrationNumber = visaRegistrationDate.RegistrationNumber;
            RowVersion = visaRegistrationDate.RowVersion;
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}