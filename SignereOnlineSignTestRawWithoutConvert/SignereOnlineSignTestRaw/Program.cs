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
            string apiID = "78a45d9f-4de6-4833-ba15-a5f500bf4a27";
            string primaryApiKey =
                "WklvSkEzK2YySE5tMVhuM1lMaVZqQT09LEUxWHMxRTRzcHA2U0VKUWxvVlJFcWNRRnBuOEV3b3ZSVkZuUmFyQWVndHc9";
            string baseUrl = "https://testapi.signere.no/api/";
            string documentString = Convert.ToBase64String(File.ReadAllBytes("Testdokument.pdf"));

            //Oppretter en signeringsrequest
            var signRequestObj =
                new
                {
                    ConvertToPDFA2b = false,
                    CreatedByApplication = "testSignApplication",
                    CreatePADES = true,
                    Description = "Dette er en signeringstest",
                    DocumentType = "PDF",
                    ExternalDocumentId = "123",
                    ExternalRef = "abc",
                    Filename = "Testdokument.pdf",
                    FileContent = documentString,
                    GetSocialSecurityNumber = false,
                    HideDetailsPage = true,
                    IdentityProvider = "NO_BANKID_WEB",
                    ReturnUrlSuccess = "http://www.otovo.no#success",
                    ReturnUrlError = "http://www.otovo.no#error",
                    ReturnUrlUserAbort = "http://www.otovo.no#abort",
                    Language = "NO",
                    Title = "Testdokument",
                    UrlExpiresMinutes = 5,
                    UseIframe = false,
                    SigneeRefs =
                        new List<object>()
                        {
                            new
                            {
                                CompanyName = "Otovo AS",
                                Email = "simen@otovo.no",
                                FirstName = "Simen",
                                LastName = "Jørgensen",
                                Mobile = "92030173"
                            }
                        },
                    UseWebMessaging = true,
                };
            var reqBody = JsonConvert.SerializeObject(signRequestObj, Formatting.None);

            WebClient webClient=new WebClient();
            string timestamp= webClient.DownloadString(baseUrl + "Status/ServerTime").Replace("\"","");
            //string timestamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
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

            }
            Console.WriteLine("Trykk en tast når du er ferdig med å signere...");
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

