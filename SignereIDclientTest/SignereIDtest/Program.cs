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
            try
            {
                var res = id.CreateRequest(req);
                string uri = res.Url; //This is the URL for the end user
                Process.Start(uri);
                //The end user identifies himself/herself using BankID and hits the SuccessUrl
                Console.WriteLine("Press a key when you are done identifying yourself\n");
                Console.ReadKey();
                var result = id.GetResponse(res.RequestId); //Fetching back the result using the RequestID
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                Console.WriteLine("\nPress a key to finish");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
