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
            string apiID = "Enter your API ID here";
            string primaryApiKey = "Enter your primary API key here";
            Client client = new Client("https://testapi.signere.no", new Guid(apiID), primaryApiKey, false, true, true);
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
                            Email = "test@test.com", //Enter signee's email address to receive signing link
                            Mobile = "+4799999999", //Enter signee's phone number to receive SMS notification, including country code (+XXX)"
                            FirstName = "Testmann",
                            LastName = "Testesen"
                        }
                    },
                CreatePADES = true,
                MessageEmail = "Hi! You have received a document to sign. Press the link below to sign", //enter the email text here
                MessageSms = "Hi! You have received a document to sign. Please check your e-mail.", //enter the SMS text here
                SenderEmail = "test@test.com", //enter the sender's email address here
                SenderMobile = "+4799999999", //enter the sender's mobile number here
                TopicEmail = "Document to sign", //"enter the topic of the email here
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
                ReceiptMessageEmail = "Receipt", //enter the text of the receipt email here,
                ReceiptTopicEmail = "Test document has been signed", //enter the topic of the receipt email here"
                ShowOnSigneesPersonalSite = true,
                CreatedByApplication = "test",
                ConvertToPDFA2b = false,
            };

            var obj = JsonConvert.SerializeObject(document, Formatting.None);

            try
            {
                var documentJobId = client.CreateDocumentJob(new DocumentJob()
                {
                    //contact information for the sender of the document
                    Contact_Email = "test@test.com",
                    Contact_Mobile = "+4799999999",
                    Contact_Name = "Testmann Testesen",
                    Contact_Phone = "+4799999999",
                    Contact_Url = "https://www.test.com"
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
