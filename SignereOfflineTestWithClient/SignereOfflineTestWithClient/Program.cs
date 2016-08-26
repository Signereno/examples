using System;
using System.Collections.Generic;
using Unipluss.Sign.Client;
using Unipluss.Sign.Client.Models;
using Unipluss.Sign.ExternalContract.Entities;
using Newtonsoft.Json;


namespace SignereOfflineTestWithClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "Enter your API ID here"
            string primaryApiKey = "Enter your primary API key here"
            Client client = new Client("https://testapi.signere.no", new Guid(apiID), primaryApiKey,false,true,true);
            NewDocument document = new NewDocument()
            {
                Language = NewDocument.Languages.NO,
                Description = "Dette er en test",
                RedirectFinishUrl = "http://www.vg.no#success",
                Title = "Testdokument",
                SigneeRefs =
                    new List<SigneeRef>()
                    {
                        new SigneeRef()
                        {
                            ExternalSigneeId = "12345",
                            Email = "enter the recipients' email address here",
                            Mobile = "enter the recipients' mobile number here",
                            FirstName = "Testmann",
                            LastName = "Testesen"
                        }
                    },
                CreatePADES = true,
                MessageEmail = "enter the email text here",
                MessageSms = "enter the sms text here",
                SenderEmail = "enter the sender's email address here",
                SenderMobile = "enter the sender's mobile number here",
                TopicEmail = "enter the topic of the email here",
                ExternalDocumentId = "123435",
                SignDeadline = DateTime.Today.AddDays(3),
                DoNotNotifySigneeRefs = false,
                NotifySenderWhenCanceledEmail = false,
                NotifySenderWhenCanceledSMS = false,
                NotifySenderWhenSignedEmail = false,
                NotifySenderWhenSignedSMS = false,
                NotifySigneerefBeforeSignDeadlineEmail = false,
                NotifySigneerefBeforeSignDeadlineSMS = false,
                NotifySigneerefsWhenSignedEmail = false,
                NotifySigneerefsWhenSignedSMS = false,
                Private = false,
                ReceiptMessageEmail = "enter the text of the receipt email here",
                ReceiptTopicEmail = "enter the topic of the receipt email here",
                ShowOnSigneesPersonalSite = true,
                CreatedByApplication = "test",
                ConvertToPDFA2b = false,
            };

            var obj = JsonConvert.SerializeObject(document, Formatting.None);

            try
            {
                var documentJobId = client.CreateDocumentJob(new DocumentJob()
                {
                    Contact_Email = "enter contact email address here",
                    Contact_Mobile = "enter contact mobile number here",
                    Contact_Phone = "enter contact phone number here",
                    Contact_Name = "enter contact person's name here",
                    Contact_Url = "enter contact website here"
                });
                client.CreateDocumentInJob(document, documentJobId, "Testdokument.pdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            
        }
    }
}
