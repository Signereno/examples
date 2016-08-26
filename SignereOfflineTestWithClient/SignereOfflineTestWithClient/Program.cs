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
            string apiID = "80da60ca-09bc-4b77-adf4-a66e009efe9b";
            string primaryApiKey = "L2tBTDBYcm11UGNYc0oxUHpNUWluQT09LEdHQlJ0R3pKU1Nuem9UNkxDSk1jcFNvN1kvT2dnV1VGSUFiY2JQQzVhQVE9";
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
                            Email = "steinolavdavidsen@gmail.com",
                            Mobile = "95767053",
                            FirstName = "Stein-Olav",
                            LastName = "Davidsen"
                        }
                    },
                CreatePADES = true,
                MessageEmail = "steinolavdavidsen@gmail.com",
                MessageSms = "95767053",
                SenderEmail = "stein@signere.no",
                SenderMobile = "95767053",
                TopicEmail = "testsignering",
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
                ReceiptMessageEmail = "stein@signere.no",
                ReceiptTopicEmail = "Test",
                ShowOnSigneesPersonalSite = true,
                CreatedByApplication = "test",
                ConvertToPDFA2b = false,
            };

            var obj = JsonConvert.SerializeObject(document, Formatting.None);

            try
            {
                var documentJobId = client.CreateDocumentJob(new DocumentJob()
                {
                    Contact_Email = "stein@signere.no",
                    Contact_Mobile = "95767053",
                    Contact_Phone = "95767053",
                    Contact_Name = "Stein-Olav Davidsen",
                    Contact_Url = "https://www.signere.no"
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
