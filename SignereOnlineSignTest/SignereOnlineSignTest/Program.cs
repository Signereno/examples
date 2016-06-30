using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Unipluss.Sign.Client;
using Unipluss.Sign.ExternalContract.Entities.SignereID;
using Unipluss.Sign.Client.Models;
using Unipluss.Sign.ExternalContract.Entities;

namespace SignereOnlineSignTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "Enter your API ID here";          
            string primaryApiKey = "Enter your primary API key here";
            string baseurl = "https://testapi.signere.no";
            Client client = new Client(baseurl,new Guid(apiID),primaryApiKey,false,true,true);

            NewExternalSignDocument documentRequest = new NewExternalSignDocument()
            {
                ConvertToPDFA2b = false,
                CreatedByApplication = "testSignApplication",
                CreatePADES = true,
                Description = "This is a signature test",
                DocumentType = DocumentType.PDF,
                ExternalDocumentId = "123",
                ExternalRef = "abc",
                GetSocialSecurityNumber = false,
                HideDetailsPage = true,
                IdentityProvider = IdentityProviderType.NO_BANKID_WEB,
                ReturnUrlSuccess = "http://www.vg.no#success",
                ReturnUrlError = "http://www.vg.no#error",
                ReturnUrlUserAbort = "http://www.vg.no#abort",
                Language = Languages.NO,
                Title = "Testdokument",
                UrlExpiresMinutes = 5,
                UseIframe = false,
                SigneeRefs =
                    new List<ExternalSigneeRef>()
                    {
                        new ExternalSigneeRef()
                        {
                            CompanyName = "Testcompany AS",
                            Email = "test@test.com",
                            FirstName = "Testmann",
                            LastName = "Testesen",
                        }
                    },
                UseWebMessaging = true,
            };
            
            //Read bytes for a PDF file
            byte[] fileBytes = File.ReadAllBytes("Testdokument.pdf");
            try
            {
                var response = client.CreateExternalSign(documentRequest, "Testdokument.pdf", fileBytes);
                string url = response.CreatedSigneeRefs[0].SignUrl; //This is the URL for the end user
                Process.Start(url);
                //End user signs the document using BankID and hits the SuccessUrl
                Console.WriteLine("Press a key to finish...\n");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
