using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Models
{
    public class FilterBusinessTripByUnitsModelForExcel
    {
        public FilterBusinessTripByUnitsModelForExcel()
        {

        }

        //string[] caption = new string[] { "ID", "EID", "Name", "Loc", "From", "To", "Unit", "Purpose", "Mgr", "Resp" };

                //workSheet.Cells[i, 0].Value = businessTripViewModel.BusinessTripID;
                //workSheet.Cells[i, 1].Value = businessTripViewModel.BTof.EID;
                //workSheet.Cells[i, 2].Value = businessTripViewModel.BTof.LastName + " " + businessTripViewModel.BTof.FirstName;
                //workSheet.Cells[i, 3].Value = businessTripViewModel.Title;
                //workSheet.Cells[i, 4].Value = businessTripViewModel.StartDate;
                //workSheet.Cells[i, 5].Value = businessTripViewModel.EndDate;
                //workSheet.Cells[i, 6].Value = businessTripViewModel.Unit;
                //workSheet.Cells[i, 7].Value = businessTripViewModel.Purpose;
                //workSheet.Cells[i, 8].Value = businessTripViewModel.Manager;
                //workSheet.Cells[i, 9].Value = businessTripViewModel.Responsible;

        public string Year { get; set; }
        public string Locations { get; set; }
        public string FromLowerBound { get; set; }
        public string FromUpperBound { get; set; }
        public string ToFromLowerBound { get; set; }
        public string ToFromUpperBound { get; set; }
        public string Unit { get; set; }
    }
}