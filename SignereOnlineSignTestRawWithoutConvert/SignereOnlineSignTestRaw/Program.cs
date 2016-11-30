using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SignereTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "Enter your API ID here";
            string primaryApiKey = "Enter your primary API key here";
            string baseUrl = "https://testapi.signere.no/api/";
            string documentString = Convert.ToBase64String(File.ReadAllBytes("Testdokument.pdf"));

            var signRequestObj =
                new
                {
                    ConvertToPDFA2b = false,
                    CreatedByApplication = "testSignApplication",
                    CreatePADES = true,
                    Description = "This is a signature test",
                    DocumentType = "PDF",
                    ExternalDocumentId = "123",
                    ExternalRef = "abc",
                    Filename = "Testdokument.pdf",
                    FileContent = documentString,
                    GetSocialSecurityNumber = false,
                    HideDetailsPage = true,
                    IdentityProvider = "NO_BANKID_WEB",
                    ReturnUrlSuccess = "http://www.vg.no#success",
                    ReturnUrlError = "http://www.vg.no#error",
                    ReturnUrlUserAbort = "http://www.vg.no#abort",
                    Language = "NO",
                    Title = "Testdokument",
                    UrlExpiresMinutes = 5,
                    UseIframe = false,
                    SigneeRefs =
                        new List<object>()
                        {
                            new
                            {
                                CompanyName = "Testcompany AS",
                                Email = "test@test.com",
                                FirstName = "Testmann",
                                LastName = "Testesen",
                                Mobile="+4799999999"
                            }
                        },
                    UseWebMessaging = true,
                };
            var reqBody = JsonConvert.SerializeObject(signRequestObj, Formatting.None);

            WebClient webClient=new WebClient();
            string timestamp= webClient.DownloadString(baseUrl + "Status/ServerTime").Replace("\"","");
            var token = GenerateTokenForJson(reqBody, "POST", primaryApiKey, timestamp);
            Encoding encoding = new UTF8Encoding();
            HttpWebRequest signRequest = (HttpWebRequest) WebRequest.Create(baseUrl + "ExternalSign");
            signRequest.Headers.Add(string.Format("API-ID:{0}", apiID));
            signRequest.Headers.Add(string.Format("API-TIMESTAMP:{0}", timestamp));
            signRequest.Headers.Add(string.Format("API-TOKEN:{0}", token));
            signRequest.Headers.Add("API-RETURNERRORHEADER:true");
            signRequest.Headers.Add("API-ALGORITHM:SHA256");
            signRequest.ContentType = "application/json; charset=UTF-8";
            signRequest.Accept = "application/json";
            signRequest.Method = "POST";
            byte[] reqBodyBytes = encoding.GetBytes(reqBody);
            Stream dataStream = signRequest.GetRequestStream();
            dataStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            dataStream.Close();
            try
            {
                var response = signRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                string line;
                ResponseModel model;
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                    model = JsonConvert.DeserializeObject<ResponseModel>(line);
                }
                Process.Start(model.CreatedSigneeRefs[0].SignUrl);
                response.Close();

            }
            catch (WebException e)
            {
                string h = e.Response.Headers.ToString();
                Console.WriteLine(h);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press a key when you are done signing...\n");
            Console.ReadKey();  
        }
    

    

    public static string GenerateTokenForJson(string json, string httpverb, string secretKey, string timestamp)
        {
            StringBuilder sb = new StringBuilder(json);
            sb.Append("{");
            sb.AppendFormat("Timestamp:\"{0}\",Httpverb:\"{1}\"", timestamp, httpverb);
            sb.Append("}");

            string jsonWithTimeStamp = sb.ToString();
            return GetSHA256(jsonWithTimeStamp, secretKey);
        }

        public static string GenerateTokenForUrl(string url, string httpverb, string secretKey, string timestamp)
        {
            string urlWithTimeStamp = String.Format("{0}&Timestamp={1}&Httpverb={2}", url, timestamp, httpverb);
            return GetSHA256(urlWithTimeStamp, secretKey);
        }


        public static string GetSHA256(string text, string key)
        {
            Encoding encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(text);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return ByteToString(hashmessage);
        }


        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }


    }

    public class ResponseModel
    {
        public string DocumentId { get; set; }
        public List<SigneeRef> CreatedSigneeRefs { get; set; }
    }

    public class SigneeRef
    {
        public string SigneeRefId { get; set; }
        public string OriginatorUniqueRef { get; set; }
        public string SignUrl { get; set; }

    }
}

