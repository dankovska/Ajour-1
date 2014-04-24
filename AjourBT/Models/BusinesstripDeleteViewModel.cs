using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Models
{
    public class BusinesstripDeleteViewModel
    {
        public int BusinessTripID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OldStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OldEndDate { get; set; }

        private BTStatus status;
        public BTStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Location { get; set; }

        [Display(Name = "Old Location")]
        public string OldLocation { get; set; }


        public int EmployeeID { get; set; }

        public virtual Employee BTof { get; set; }

        public string Purpose { get; set; }
        public string Manager { get; set; }
        public string Responsible { get; set; }
        public string Comment { get; set; }
        [Display(Name = "Reject Comment")]
        public string RejectComment { get; set; }
        [Display(Name = "Cancel Comment")]
        public string CancelComment { get; set; }
        public string Habitation { get; set; }
        [Display(Name = "Habitation Confirmed")]
        public bool HabitationConfirmed { get; set; }
        public string Flights { get; set; }
        [Display(Name = "Flights Confirmed")]
        public bool FlightsConfirmed { get; set; }
        public bool Invitation { get; set; }
        public string Title { get; set; }
    }
}