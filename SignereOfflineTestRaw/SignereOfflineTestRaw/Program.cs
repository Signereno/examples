using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SignereOfflineTestRaw
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiID = "Enter your API ID here";
            string primaryApiKey = "Enter your primary API key here";
            string baseUrl = "https://testapi.signere.no/api/";
            var documentJobRequestObj =
                new
                { //contact information for the sender of the document
                    Contact_Email = "test@test.com", 
                    Contact_Mobile = "+4799999999",
                    Contact_Name = "Testmann Testesen",
                    Contact_Phone = "+4799999999",
                    Contact_Url = "https://www.test.com"
                };

            var documentJobReqBody = JsonConvert.SerializeObject(documentJobRequestObj, Formatting.None);
            string timestamp;
            using (WebClient webClient = new WebClient())
            {
                timestamp = webClient.DownloadString(baseUrl + "Status/ServerTime").Replace("\"", "");
            }

            var token = GenerateTokenForJson(documentJobReqBody, "POST", primaryApiKey, timestamp);
            Encoding encoding = new UTF8Encoding();
            HttpWebRequest documentJobRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "DocumentJob");
            documentJobRequest.Headers.Add(string.Format("API-ID:{0}", apiID));
            documentJobRequest.Headers.Add(string.Format("API-TIMESTAMP:{0}", timestamp));
            documentJobRequest.Headers.Add(string.Format("API-TOKEN:{0}", token));
            documentJobRequest.Headers.Add("API-RETURNERRORHEADER:true");
            documentJobRequest.Headers.Add("API-ALGORITHM:SHA256");
            documentJobRequest.ContentType = "application/json; charset=UTF-8";
            documentJobRequest.Accept = "application/json";
            documentJobRequest.Method = "POST";
            byte[] documentJobRequestBytes = encoding.GetBytes(documentJobReqBody);
            Stream dataStream = documentJobRequest.GetRequestStream();
            dataStream.Write(documentJobRequestBytes, 0, documentJobRequestBytes.Length);
            dataStream.Close();
            string documentJobId="";
            try
            {
                var response = documentJobRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                string line;
                CreateDocumentResponseModel documentJobIdModel;
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                    documentJobIdModel = JsonConvert.DeserializeObject<CreateDocumentResponseModel>(line);
                    documentJobId = documentJobIdModel.Id;
                }
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

            //Generating a signing request
            string documentString = Convert.ToBase64String(File.ReadAllBytes("Testdokument.pdf"));
            string checksum = CalculateMD5FromByteArray("Testdokument.pdf");

            NewDocument document = new NewDocument()
            {
                SignJobId=documentJobId,
                FileContent = documentString,
                FileName = "Testdokument.pdf",
                FileMD5CheckSum = checksum,
                Language = Languages.NO,
                Description = "This is a test",
                RedirectFinishUrl = "http://www.vg.no#success",
                Title = "Test document",
                SigneeRefs =
                   new List<SigneeRef>()
                   {
                        new SigneeRef()
                        {
                            ExternalSigneeId = "12345",
                            Email = "test@test.com", //Enter signee's email address to receive signing link
                            Mobile = "+4799999999", //Enter signee's phone number to receive SMS notification, including country code (+XXX)"
                            FirstName = "John",
                            LastName = "Doe"
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
                ReceiptMessageEmail = "Receipt", //enter the text of the receipt email here
                ReceiptTopicEmail = "Test document has been signed", //enter the topic of the receipt email here"
                ShowOnSigneesPersonalSite = true,
                CreatedByApplication = "testapp",
                ConvertToPDFA2b = false,
            };

            var documentReqBody = JsonConvert.SerializeObject(document, Formatting.None);
            using (WebClient webClient = new WebClient())
            {
                timestamp = webClient.DownloadString(baseUrl + "Status/ServerTime").Replace("\"", "");
            }
            token = GenerateTokenForJson(documentReqBody, "POST", primaryApiKey, timestamp);
            string url = baseUrl + "Document";
            HttpWebRequest signRequest = (HttpWebRequest)WebRequest.Create(url);
            signRequest.Headers.Add(string.Format("API-ID:{0}", apiID));
            signRequest.Headers.Add(string.Format("API-TIMESTAMP:{0}", timestamp));
            signRequest.Headers.Add(string.Format("API-TOKEN:{0}", token));
            signRequest.Headers.Add("API-RETURNERRORHEADER:true");
            signRequest.Headers.Add("API-ALGORITHM:SHA256");
            signRequest.ContentType = "application/json; charset=UTF-8";
            signRequest.Accept = "application/json";
            signRequest.Method = "POST";
            byte[] reqBodyBytes = encoding.GetBytes(documentReqBody);
            dataStream = signRequest.GetRequestStream();
            dataStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            dataStream.Close();
            try
            {
                var response = signRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                string line;
                DocumentResponseModel model;
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                    model = JsonConvert.DeserializeObject<DocumentResponseModel>(line);
                }
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

        
        public static string GetSHA256(string text, string key)
        {
            Encoding encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] messageBytes = encoding.GetBytes(text);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return ByteToStringUpper(hashmessage);
        }

        public static string GenerateTokenForUrl(string url, string httpverb, string secretKey, string timestamp)
        {
            string urlWithTimeStamp = String.Format("{0}&Timestamp={1}&Httpverb={2}", url, timestamp, httpverb);
            return GetSHA256(urlWithTimeStamp, secretKey);
        }

        private static string ByteToStringUpper(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }


        public static string CalculateMD5FromByteArray(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            MD5 md5 = new MD5CryptoServiceProvider();
            var retur = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retur.Length; i++)
            {
                sb.Append(retur[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public class CreateDocumentResponseModel
    {
        public string Id { get; set; }
    }

    public class DocumentResponseModel
    {
        public string CancelationExplanation { get; set; }
        public string CancelationSignature { get; set; }
        public bool CanceledByProvider { get; set; }
        public bool CanceledBySignee { get; set; }
        public DateTime CanceledDate { get; set; }
        public string Contact_Email { get; set; }
        public string Contact_Mobile { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Phone { get; set; }
        public string Contact_Url { get; set; }
        public string Description { get; set; }
        public string ExternalDocumentId { get; set; }
        public Guid Id { get; set; }
        public string Language { get; set; }
        public DateTime LastSignature { get; set; }
        public string MessageEmail { get; set; }
        public bool NotifiySenderWhenSignedSMS { get; set; }
        public bool NotifySenderWhenCanceledEmail { get; set; }
        public bool NotifySenderWhenCanceledSMS { get; set; }

        public bool NotifySenderWhenSignedEmail { get; set; }
        public bool NotifySigneerefsWhenSignedEmail { get; set; }
        public bool NotifySigneerefsWhenSignedSMS { get; set; }
        public bool Private { get; set; }
        public string SenderEmail { get; set; }
        public string SenderMobile { get; set; }
        public DateTime SignDeadline { get; set; }
        public bool Signed { get; set; }
        public Guid SignJobId { get; set; }
        public string Title { get; set; }
        public string TopicEmail { get; set; }
        public DateTime UploadTime { get; set; }
        public bool ValidPDFA_2b { get; set; }
        public string ValidPDFA_2b_Report { get; set; }
        public List<SigneeRef> SigneeRefs { get; set; }

    }

    public class SigneeRef
    {
        public string ExternalSigneeId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrgNo { get; set; }
        public string CompanyName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int SignOrder { get; set; }

    }

    public class NewDocument
    {
        public string FileContent { get; set; }
        public string FileName { get; set; }
        public string FileMD5CheckSum { get; set; }
        public string ExternalDocumentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Languages Language { get; set; }
        public List<DocumentMetaData> MetaData { get; set; }
        public List<SigneeRef> SigneeRefs { get; set; }
        public DateTime? SignDeadline { get; set; }
        public string MessageSms { get; set; }
        public string MessageEmail { get; set; }
        public string TopicEmail { get; set; }

        public string ReceiptMessageEmail { get; set; }
        public string ReceiptTopicEmail { get; set; }

        public bool ShowOnSigneesPersonalSite { get; set; }
        public bool Private { get; set; }
        public string CreatedByApplication { get; set; }
        public string PushNotificationUrl { get; set; }
        public string RedirectFinishUrl { get; set; }

        public bool NotifySigneerefBeforeSignDeadlineSMS { get; set; }
        public bool NotifySigneerefBeforeSignDeadlineEmail { get; set; }

        public bool NotifySenderWhenCanceledSMS { get; set; }
        public bool NotifySenderWhenCanceledEmail { get; set; }
        public string SenderEmail { get; set; }
        public string SenderMobile { get; set; }
        public bool NotifySenderWhenSignedSMS { get; set; }
        public bool NotifySenderWhenSignedEmail { get; set; }

        public bool NotifySigneerefsWhenSignedSMS { get; set; }
        public bool NotifySigneerefsWhenSignedEmail { get; set; }
        public bool CreatePADES { get; set; }
        public bool DoNotNotifySigneeRefs { get; set; }
        public bool ConvertToPDFA2b { get; set; }

        public string SignJobId { get; set; }
    }

    public enum Languages
    {
        NO,
        EN,
        SV,
        DA,
    }

    public class DocumentMetaData
    {
        public virtual string KeyName { get; set; }
        public virtual string Value { get; set; }
    }
}

