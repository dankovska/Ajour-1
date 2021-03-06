﻿using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Abstract;

namespace AjourBT.Domain.Concrete
{
    public class Message : IMessage
    {

        public Message()
        {

        }

        public Message(MessageType messageType, List<BusinessTrip> businessTripList, Employee author, Employee employee = null)
        {
            this.MessageID = 0;
            this.messageType = messageType;
            this.BTList = businessTripList;
            this.Author = author;
            this.employee = employee;
            if (author != null)
                ReplyTo = author.FirstName + " " + author.LastName;
            TimeStamp = DateTime.Now.ToLocalTimeAzure();
            Role = GetRole();
            Subject = GetSubject();
            Body = GetBody();
            Link = GetLink();
        }

        [NotMapped]
        public MessageType messageType { get; set; }
        [NotMapped]
        public List<BusinessTrip> BTList { get; set; }
        [NotMapped]
        public Employee Author { get; set; }
        [NotMapped]
        public Employee employee { get; set; }

        public int MessageID { get; set; }
        public string Role { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ReplyTo { get; set; }

        public string GetSubject()
        {
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                    return "For BTM: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                    return "For DIR: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                    return "For EMP: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                    return "For ACC: BT Confirmation";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                    return "For BTM: BT Registration";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                    return "For EMP: BT Registration";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                    return "For ACC: BT Registration";
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                    return "For BTM: BT Replanning";
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Replanning";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                    return "For DIR: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Cancellation";
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Update";
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Update";
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Report";
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Report";
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.ACCCancelsConfirmedReportedToADM:
                    return "For ADM: BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ACCModifiesConfirmedReportedToADM:
                    return "For ADM: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                    return "For BTM: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    return "For DIR: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToEMP:
                    return "For EMP: BT Update";
                case MessageType.DIRRejectsConfirmedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.DIRRejectsConfirmedToEMP:
                    return "For EMP: BT Rejection";
                case MessageType.DIRRejectsConfirmedToBTM:
                    return "For BTM: BT Rejection";
                case MessageType.DIRRejectsConfirmedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.BTMCancelsPermitToADM:
                    return "For ADM: Permit Cancellation";
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                    return "For BTM: Planned Modified BT Cancellation";
                case MessageType.ADMCancelsPlannedModifiedToACC:
                    return "For ACC: Planned Modified BT Cancellation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation:
                    return "BT Confirmation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation:
                    return "BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation:
                    return "BT Cancellation";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation:
                    return "BT Rejection";
                case MessageType.DIRRejectsConfirmedToResponsibleInLocation:
                    return "BT Rejection";
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                    return "Visa Registration Date Creation";
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                    return "Visa Registration Date Update";
                default:
                    return "Unknown Subject";
            }
        }

        public string GetRole()
        {
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                case MessageType.DIRRejectsConfirmedToBTM:
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                    return "BTM";

                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    return "DIR";

                case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.ACCCancelsConfirmedReportedToEMP:
                case MessageType.ACCModifiesConfirmedReportedToEMP:
                case MessageType.DIRRejectsConfirmedToEMP:
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                    return "EMP";

                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                case MessageType.DIRRejectsConfirmedToACC:
                case MessageType.ADMCancelsPlannedModifiedToACC:
                    return "ACC";

                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                case MessageType.ACCCancelsConfirmedReportedToADM:
                case MessageType.ACCModifiesConfirmedReportedToADM:
                case MessageType.DIRRejectsConfirmedToADM:
                case MessageType.BTMCancelsPermitToADM:
                    return "ADM";
                
                default:
                    return "Unknown Role";
            }
        }

        public string GetLink()
        {
            //return HtmlHelper.GenerateLink(HttpContext.Current.Request.RequestContext, System.Web.Routing.RouteTable.Routes, "Goto Ajour page", "Default", "GetBusinessTripDataBTM", "BusinessTrip", null, null); 
            //return UrlHelper.GenerateUrl("Default","GetBusinessTripDataADM","BusinessTrip",null, RouteTable.Routes, HttpContext.Current.Request.RequestContext,true);
            StringBuilder hyperLink = new StringBuilder();
            string urlLeftPart = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string pathAndQuery;
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                case MessageType.DIRRejectsConfirmedToBTM:
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                    pathAndQuery = "/Home/BTMView/?tab=1";
                    break;
                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    pathAndQuery = "/Home/DIRView/?tab=0";
                    break;
                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                case MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation:
                case MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation:
                    pathAndQuery = "/Home/VUView/?tab=2";
                    break;
                
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    pathAndQuery = "/Home/ACCView/?tab=0";
                    break;

                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                case MessageType.ACCCancelsConfirmedReportedToADM:
                case MessageType.ACCModifiesConfirmedReportedToADM:
                case MessageType.DIRRejectsConfirmedToADM:
                    pathAndQuery = "/Home/ADMView/?tab=1";
                    break;
               
                case MessageType.BTMCancelsPermitToADM:
                    pathAndQuery = "/Home/ADMView/?tab=0";
                    break;
               
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                    pathAndQuery = "/Home/EMPView/?tab=4";
                    break;
 
                default:
                    pathAndQuery = "";
                    break;
            }
            if (pathAndQuery != "")
            {
                hyperLink.AppendFormat("<a href=\"{0}{1}\"> Goto Ajour page </a>", urlLeftPart, pathAndQuery);
            }
            return hyperLink.ToString();
        }

        public string GetBTTemplate(BusinessTrip businessTrip)
        {
            StringBuilder BTTemplate = new StringBuilder();
            if (businessTrip != null)
            {
                if (businessTrip.BTof != null && businessTrip.BTof.LastName != null)
                    BTTemplate.AppendFormat("<b>{0}</b>", businessTrip.BTof.LastName);
                if (businessTrip.BTof != null && businessTrip.BTof.FirstName != null)
                    BTTemplate.AppendFormat(" <b>{0}</b>", businessTrip.BTof.FirstName);
                if (businessTrip.BTof != null && businessTrip.BTof.EID != null)
                    BTTemplate.AppendFormat(" (<b>{0}</b>)", businessTrip.BTof.EID);
                if (businessTrip.BTof != null && businessTrip.BTof.Department != null &&
                        businessTrip.BTof.Department.DepartmentName != null
                    )
                    BTTemplate.AppendFormat(", {0}", businessTrip.BTof.Department.DepartmentName);
                if (businessTrip.Manager != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Manager);
                if (businessTrip.Responsible != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Responsible);
                if (businessTrip.Purpose != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Purpose);
                if (businessTrip.Location != null && businessTrip.Location.Title != null)
                    BTTemplate.AppendFormat(", <b>{0}</b>", businessTrip.Location.Title);
                BTTemplate.AppendFormat("<b>, {0:dd'.'MM'.'yyyy} - </b>", businessTrip.StartDate);
                BTTemplate.AppendFormat("<b>{0:dd'.'MM'.'yyyy}</b>", businessTrip.EndDate);
                if (businessTrip.OrderStartDate != null && businessTrip.OrderEndDate != null)
                {
                    BTTemplate.Append(", Order dates: ");
                    BTTemplate.AppendFormat("{0:dd'.'MM'.'yyyy} - ", businessTrip.OrderStartDate);
                    BTTemplate.AppendFormat("{0:dd'.'MM'.'yyyy}", businessTrip.OrderEndDate);
                }
                if (businessTrip.Habitation != null)
                {
                    BTTemplate.Append("<br/>");
                    string habitationConfirmed = "(not confirmed)";
                    if (businessTrip.HabitationConfirmed == true)
                        habitationConfirmed = "(confirmed)";
                    BTTemplate.AppendFormat("<b>Habitation {1}:</b> {0}", businessTrip.Habitation, habitationConfirmed);
                }
                if (businessTrip.Flights != null)
                {
                    BTTemplate.Append("<br/>");
                    string flightsConfirmed = "(not confirmed)";
                    if (businessTrip.FlightsConfirmed == true)
                        flightsConfirmed = "(confirmed)";
                    BTTemplate.AppendFormat("<b>Flights {1}:</b> {0}", businessTrip.Flights, flightsConfirmed);
                }
                if (businessTrip.Invitation == true)
                {
                    BTTemplate.Append("<br/><b>Invitation:</b> confirmed");
                }
                if (businessTrip.Comment != null && businessTrip.Comment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Comment:</b> {0}", businessTrip.Comment);
                if (businessTrip.BTMComment != null && businessTrip.BTMComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>BTM comment:</b> {0}", businessTrip.BTMComment);
                if (businessTrip.RejectComment != null && businessTrip.RejectComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Reject comment:</b> {0}", businessTrip.RejectComment);
                if (businessTrip.CancelComment != null && businessTrip.CancelComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Cancel comment:</b> {0}", businessTrip.CancelComment);
            }
            return BTTemplate.ToString().Trim(new char[] { ' ', ',' });
        }

        public string GetMessageTemplate()
        {
            if (Author != null)
            {
                switch (messageType)
                {
                    case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                        return string.Format("<b>BT confirmation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                        return string.Format("<b>BT registration</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                    case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT replanning</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));


                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                        return string.Format("<b>BT(s) cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                        return string.Format("<b>BT update</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                        return string.Format("<b>BT(s) report</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                    case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT rejection</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                        return string.Format("<b>BT rejection</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ACCCancelsConfirmedReportedToADM:
                    case MessageType.ACCCancelsConfirmedReportedToBTM:
                    case MessageType.ACCCancelsConfirmedReportedToEMP:
                        return string.Format("<b>BT cancellation</b> by ACC {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ACCModifiesConfirmedReportedToADM:
                    case MessageType.ACCModifiesConfirmedReportedToBTM:
                    case MessageType.ACCModifiesConfirmedReportedToDIR:
                    case MessageType.ACCModifiesConfirmedReportedToEMP:
                        return string.Format("<b>BT modification</b> by ACC {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.DIRRejectsConfirmedToADM:
                    case MessageType.DIRRejectsConfirmedToEMP:
                    case MessageType.DIRRejectsConfirmedToBTM:
                    case MessageType.DIRRejectsConfirmedToACC:
                        return string.Format("<b>BT rejection</b> by DIR {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMCancelsPermitToADM:
                        return string.Format("<b>Permit cancellation</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Cancel for permit of {0} {1} ({2}) with dates {3:dd.MM.yyyy} - {4:dd.MM.yyyy} requested at {5:dd.MM.yyyy}", employee.FirstName, employee.LastName, employee.EID,
                            employee.Permit.StartDate, employee.Permit.EndDate, employee.Permit.CancelRequestDate);

                    case MessageType.ADMCancelsPlannedModifiedToBTM:
                    case MessageType.ADMCancelsPlannedModifiedToACC:
                        return string.Format("<b>Planned modified BT cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation:
                        return string.Format("<b>BT confirmation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation:
                        return string.Format("<b>BT(s) cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                    case MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation:
                        return string.Format("<b>BT cancellation</b> by ACC {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation:
                        return string.Format("<b>BT rejection</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                    case MessageType.DIRRejectsConfirmedToResponsibleInLocation:
                        return string.Format("<b>BT rejection</b> by DIR {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMCreateVisaRegistrationDateToEMP:
                        return string.Format("<b>Visa Registration Date Creation</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName,
                            TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}", employee.VisaRegistrationDate.VisaType,
                            employee.VisaRegistrationDate.RegistrationDate, employee.VisaRegistrationDate.RegistrationTime,
                            employee.VisaRegistrationDate.City, employee.VisaRegistrationDate.RegistrationNumber);

                    case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                        return string.Format("<b>Visa Registration Date Update</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName,
                            TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}", employee.VisaRegistrationDate.VisaType,
                            employee.VisaRegistrationDate.RegistrationDate, employee.VisaRegistrationDate.RegistrationTime,
                            employee.VisaRegistrationDate.City, employee.VisaRegistrationDate.RegistrationNumber);

                    default:
                        return string.Format("Unknown Message Type by {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                }
            }
            return "";
        }

        public string GetBody()
        {
            StringBuilder messageBody = new StringBuilder(GetMessageTemplate());
            if (BTList != null)
            {
                messageBody.Append("<br/><br/>");
                foreach (BusinessTrip businessTrip in BTList)
                {
                    messageBody.Append(GetBTTemplate(businessTrip));
                    messageBody.Append("<br/><br/>");
                }
            }
            return messageBody.ToString();
        }

    }
}