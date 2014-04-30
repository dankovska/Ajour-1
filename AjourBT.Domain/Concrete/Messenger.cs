using AjourBT.Domain.Abstract;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Domino;
using System.Web.Configuration;
using AjourBT.Domain.Entities;
using System.Text.RegularExpressions;

namespace AjourBT.Domain.Concrete
{
    public class Messenger : IMessenger
    {
        public Messenger()
        {

        }

        public Messenger(IRepository repository)
        {
            this.repository = repository;
        }

        IRepository repository;

        public void Notify(IMessage message)
        {
            try
            {
                StoreMessage(message);
                SendToMailingList(GetMailingListForRole(message), message);
            }
            catch (Exception)
            {

            }
        }


        public void StoreMessage(IMessage message)
        {
            if(GetMailingListForRole(message).Length!=0)
            repository.SaveMessage(message);
        }

        public string[] GetMailingListForRole(IMessage message)
        {
            List<string> mailingList = new List<string>();
            if (message != null)
            {
                switch (message.Role)
                {
                    case "EMP":
                        if (message.employee != null)
                        {
                            mailingList.Add(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                            break;
                        }

                        foreach (BusinessTrip bt in message.BTList)
                        {
                            if (!mailingList.Contains(bt.BTof.EID + WebConfigurationManager.AppSettings["MailAlias"]))
                                mailingList.Add(bt.BTof.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                        }
                        break;
                    case "ADM":
                        if (message.employee != null)
                        {
                           Department depNum = (from dep in repository.Departments
                                         where dep.DepartmentID == message.employee.DepartmentID
                                         select dep).FirstOrDefault();
                           foreach (Employee emp in depNum.Employees)
                           {
                               if (System.Web.Security.Roles.IsUserInRole(emp.EID, "ADM"))
                                   mailingList.Add(emp.EID);
                           }
                           break;
                        }

                        foreach (BusinessTrip bt in message.BTList)
                        {
                            if (!mailingList.Contains(bt.LastCRUDedBy + WebConfigurationManager.AppSettings["MailAlias"]))
                                mailingList.Add(bt.LastCRUDedBy + WebConfigurationManager.AppSettings["MailAlias"]);
                        }
                        break;
                    case "Unknown Role":
                        {
                            string[] userIDs;
                            if (message.BTList!=null && message.BTList.Count != 0)
                            {
                                foreach (BusinessTrip bt in message.BTList)
                                {
                                    if (bt.Location.ResponsibleForLoc != null)
                                    {
                                        userIDs = Regex.Split(bt.Location.ResponsibleForLoc, @"\W+");

                                        foreach (string userID in userIDs)
                                        {
                                            if (!mailingList.Contains(userID + WebConfigurationManager.AppSettings["MailAlias"]))
                                                mailingList.Add(userID + WebConfigurationManager.AppSettings["MailAlias"]);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        mailingList = System.Web.Security.Roles.GetUsersInRole(message.Role).ToList<string>();
                        for (int i = 0; i < mailingList.Count(); i++)
                        {
                            mailingList[i] += WebConfigurationManager.AppSettings["MailAlias"];
                        }
                        break;
                }

            }
            return mailingList.ToArray<string>();
        }

        public void SendToMailingList(string[] mailingList, IMessage message)
        {
            string sendingChoice = WebConfigurationManager.AppSettings["WayOfMessageSending"];
            switch (sendingChoice)
            {
                case "LotusNotes":
                    SendUsingLotusNotes(mailingList, message);
                    break;
                case "SendGrid":
                    SendUsingSendGrid(mailingList, message);
                    break;
                default:
                    break;
            }
        }

        private static void SendUsingSendGrid(string[] mailingList, IMessage message)
        {
            //TODO: add sending logic here
            // Setup the email properties.
            var from = new MailAddress(message.ReplyTo);
            var to = new MailAddress[mailingList.Length];
            for (int i = 0; i < mailingList.Length; i++)
            {
                to[i] = new MailAddress(mailingList[i]);
            }
            var cc = new MailAddress[] { };
            var bcc = new MailAddress[] { };
            var subject = message.Subject;
            var html = message.Body.Replace(Environment.NewLine, "<br/>") + "<br/>" + message.Link;

            // Create an email, passing in the the eight properties as arguments.
            SendGrid myMessage = SendGrid.GetInstance(from, to, cc, bcc, subject, html, null);

            var username = WebConfigurationManager.AppSettings["SendGridLogin"];
            var pswd = WebConfigurationManager.AppSettings["SendGridPassword"];

            var credentials = new NetworkCredential(username, pswd);


            var transportSMTP = SMTP.GetInstance(credentials);


            transportSMTP.Deliver(myMessage);
        }

        private static void SendUsingLotusNotes(string[] mailingList, IMessage message)
        {
            string serverName = WebConfigurationManager.AppSettings["LotusNotesServerName"];
            string mailFile = WebConfigurationManager.AppSettings["LotusNotesMailFileName"];
            string password = WebConfigurationManager.AppSettings["LotusNotesPassword"];
            string[] sendTo = mailingList;
            string[] copyTo = { };
            string replyTo = message.ReplyTo;
            string blindCopyTo = "";
            string subject = message.Subject;

            //Create new notes session
            NotesSession notesSession = new NotesSession();
            notesSession.Initialize(password);

            //Get and open NotesDataBase
            NotesDatabase notesDataBase = notesSession.GetDatabase(serverName, mailFile, false);
            if (!notesDataBase.IsOpen) notesDataBase.Open();

            //Create the notes document
            NotesDocument notesDocument = notesDataBase.CreateDocument();

            //Set document type
            notesDocument.ReplaceItemValue("Form", "Memo");

            //Set notes memo fields (To: CC: Bcc: Subject etc) 
            notesDocument.ReplaceItemValue("SendTo", sendTo);
            notesDocument.ReplaceItemValue("CopyTo", copyTo);
            notesDocument.ReplaceItemValue("BlindCopyTo", blindCopyTo);
            notesDocument.ReplaceItemValue("ReplyTo", replyTo);
            notesDocument.ReplaceItemValue("Subject", subject);

            //Set notes Body as HTML
            NotesStream notesStream = notesSession.CreateStream();
            notesStream.WriteText(message.Body);
            notesStream.WriteText("");
            notesStream.WriteText(message.Link);

            NotesMIMEEntity mimeItem = notesDocument.CreateMIMEEntity("Body");
            mimeItem.SetContentFromText(notesStream, "text/html; charset=UTF-8", MIME_ENCODING.ENC_NONE);

            notesDocument.Send(false);
        }
    }
}