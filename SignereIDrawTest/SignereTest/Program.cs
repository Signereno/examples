using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Unipluss.Sign;
using Unipluss.Sign.Client;
using Unipluss.Sign.ExternalContract.Entities.SignereID;

namespace SignereTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "9be54fd9-6983-4ecf-8bab-a33e00cebecb";
            string primaryApiKey = "N0NFclJyZkpZV0lqbVdEUG1ySG5Wdz09LDAzQTd2MDhuQkx2WnFvcGNsK2RRSUQ5V2w2N1hUamUrdTFNN3ZUMkZ5VFk9";
            string url = "https://api.signere.no/api/SignereId";

            var requestObj = new {CancelUrl = "https://www.vg.no#cancel", ErrorUrl="https://www.vg.no#error", SuccessUrl="http://www.vg.no#success", ExternalReference="123", IdentityProvider="NO_BANKID_WEB"};
            var reqBody = JsonConvert.SerializeObject(requestObj, Formatting.None);
            string timestamp = DateTime.UtcNow.ToString("s");
            var token = GenerateTokenForJson(reqBody, "POST", primaryApiKey, timestamp);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);            
            request.Headers.Add(string.Format("API-ID:{0}", apiID));
            request.Headers.Add(string.Format("API-TIMESTAMP:{0}",timestamp));
            request.Headers.Add(string.Format("API-TOKEN:{0}",token));
            request.Headers.Add("API-RETURNERRORHEADER:true");
            request.Headers.Add("API-ALGORITHM:SHA256");
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.Method = "POST";
            Encoding encoding = new UTF8Encoding();
            byte[] reqBodyBytes = encoding.GetBytes(reqBody);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(reqBodyBytes,0,reqBodyBytes.Length);
            dataStream.Close();
            try
            {
                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                string line;
                ResponseModel model;
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                    model = JsonConvert.DeserializeObject<ResponseModel>(line);
                }
                Process.Start(model.Url);
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
            Console.ReadLine();
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
        public string Url { get; set; }
        public string RequestId { get; set; }
        public string Started { get; set; }

    }
}
