using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Unipluss.Sign;
using Unipluss.Sign.Client;
using Unipluss.Sign.ExternalContract.Entities.SignereID;
using Newtonsoft.Json;

namespace SignereIDtest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "Enter your API ID here";
            string primaryApiKey = "Enter your primary API key here";
            string baseurl = "https://testapi.signere.no";

            SignereID id = new SignereID(baseurl, new Guid(apiID), primaryApiKey, false, true, true);

            SignereIdRequest req = new SignereIdRequest()
            {
                SuccessUrl = "http://www.vg.no",
                CancelUrl = "http://www.vg.no",
                ErrorUrl = "http://www.vg.no",
                ExternalReference = "1234",
                IdentityProvider = IdentityProviderType.NO_BANKID_WEB
            };

            var res = id.CreateRequest(req);
            string uri = res.Url; //Dette er URLen som sluttbruker må bruke – putt den i nettleseren og prøv

            Process.Start(uri);
            //Sluttbruker gjennomfører BankID-innlogging og hitter SuccessUrl...
            Console.WriteLine("Trykk en tast når du er ferdig med innloggingen...\n");
            Console.ReadKey();
            var result = id.GetResponse(res.RequestId); //Henter tilbake BankID-data med bruk av requestID
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.WriteLine("\nTrykk en tast for å avslutte");
            Console.ReadKey();
        }
    }
}
