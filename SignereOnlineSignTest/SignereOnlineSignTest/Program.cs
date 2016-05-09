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
            //string apiID = "Enter your API ID here";
            //string apiID = "a0dd4cb3-33d5-45da-938b-a5fe00a6ea7f";
            string apiID = "9c97fb78-13b6-4ed7-a720-a5f301424f65";
            //string primaryApiKey = "Enter your primary API key here";
            //string primaryApiKey = "TVh0cERRMlMvVldPNEV1aWdOMjN5QT09LFJxMFdUQWZ5cG5UQXdHY1BvYWxkSCs1VWozbEtqWXBKVjhmRXlaNitWdnM9";
            string primaryApiKey = "R0h0V0tXVWpnQ2d4T2xPWUVPQlgyUT09LDJtMFRkVEU2V0pqb0NjTDZZckRCdFFUTkhFbWF4UHdRTlZJUlVWRUZHcGc9";
            string baseurl = "https://testapi.signere.no";
            Client client = new Client(baseurl,new Guid(apiID),primaryApiKey,false,true,true);

            NewExternalSignDocument documentRequest = new NewExternalSignDocument()
            {
                ConvertToPDFA2b = false,
                CreatedByApplication = "testSignApplication",
                CreatePADES = true,
                Description = "Dette er en signeringstest",
                DocumentType = DocumentType.PDF,
                ExternalDocumentId = "123",
                ExternalRef = "abc",
                GetSocialSecurityNumber = false,
                HideDetailsPage = true,
                IdentityProvider = IdentityProviderType.NO_BANKID_WEB,
                ReturnUrlSuccess = "http://www.tiseit.com#success",
                ReturnUrlError = "http://www.tiseit.com#error",
                ReturnUrlUserAbort = "http://www.tiseit.com#abort",
                Language = Languages.NO,
                Title = "Testdokument",
                UrlExpiresMinutes = 5,
                UseIframe = false,
                SigneeRefs =
                    new List<ExternalSigneeRef>()
                    {
                        new ExternalSigneeRef()
                        {
                            CompanyName = "Tise AS",
                            Email = "hello@tiseit.com",
                            FirstName = "Axel",
                            LastName = "Næss",
                            Mobile = "99999999"
                        }
                    },
                UseWebMessaging = true,
            };
            
            //Les inn bytes for en PDF-fil
            byte[] fileBytes = File.ReadAllBytes("Testdokument.pdf");
            try
            {
                var response = client.CreateExternalSign(documentRequest, "Testdokument.pdf", fileBytes);
                string url = response.CreatedSigneeRefs[0].SignUrl; //Dette er URLen som sluttbruker må bruke – putter den i nettleseren og signerer med BankIDen
                Process.Start(url);
                //Sluttbruker gjennomfører BankID-signering og hitter SuccessUrl...
                Console.WriteLine("Trykk en tast for å avslutte...\n");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
