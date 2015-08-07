using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DocuSignPOC
{
    public class SignDocument
    {
        private static string baseURL = null;
        public static void Test()
        {
            Console.WriteLine("Hello Worjkjhhhhlld!");

        }

        public static bool SendForSigning(string customerName, string customerEmail, List<DocInfo> documents, out string statusMsg)
        {
            statusMsg = "All is well!";

            Console.WriteLine("Customer name: " + customerName);


            return true;
        }

        public static string GetAllTemplateXMLInfo()
        {
           /*
            * To do:
            * Get all available template information and return the response from DocuSign as is.
            * Might change the return type from string to something that better handles XML
            */
            if (string.IsNullOrEmpty(baseURL))
            {
                loginDocuSign();
            }
            return null;
        }

        private static void loginDocuSign()
        {
            //---------------------------------------------------------------------------------------------------
            // ENTER VALUES FOR THE FOLLOWING 7 VARIABLES:
            //---------------------------------------------------------------------------------------------------
            string username = "ronnie.davies@safeauto.com";		// your account email
            string password = "PAUSEBREAK401";		// your account password
            string integratorKey = "DOCU-7e719f55-a2f6-4999-ac22-d6a96018dfd6";		// your account Integrator Key (found on Preferences -> API page)

            // Endpoint for Login api call (in demo environment):
            string url = "https://demo.docusign.net/restapi/v2/login_information";

            // set request url, method, and headers.  No body needed for login api call
            HttpWebRequest request = initializeRequest(url, "GET", null, username, password, integratorKey);

          


        }
        public static string GetAllTemplateParsedInfo()
        {
            /*
             * To do:
             * Get all available template information and return the response from DocuSign after parsing and just showing template name
             * and corresponding template ID
             */
            return null;
        }
        //***********************************************************************************************
        // --- HELPER FUNCTIONS ---
        //***********************************************************************************************
        public static HttpWebRequest initializeRequest(string url, string method, string body, string email, string password, string intKey)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            addRequestHeaders(request, email, password, intKey);
            if (body != null)
                addRequestBody(request, body);
            return request;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestHeaders(HttpWebRequest request, string email, string password, string intKey)
        {
            // authentication header can be in JSON or XML format.  XML used for this walkthrough:
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Accept = "application/xml";
            request.ContentType = "application/xml";
        }
        public static void addRequestHeadersJSON(HttpWebRequest request, string email, string password, string intKey)
        {
            // authentication header can be in JSON or XML format.  XML used for this walkthrough:
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Accept = "application/xml";
            request.ContentType = "application/xml";
            request.ContentLength = 0;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestBody(HttpWebRequest request, string requestBody)
        {
            // create byte array out of request body and add to the request object
            byte[] body = System.Text.Encoding.UTF8.GetBytes(requestBody);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(body, 0, requestBody.Length);
            dataStream.Close();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string getResponseBody(HttpWebRequest request)
        {
            // read the response stream into a local string
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            string responseText = sr.ReadToEnd();
            return responseText;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string parseDataFromResponse(string response, string searchToken)
        {
            // look for "searchToken" in the response body and parse its value
            using (XmlReader reader = XmlReader.Create(new StringReader(response)))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == searchToken))
                        return reader.ReadString();
                }
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string prettyPrintXml(string xml)
        {
            // print nicely formatted xml
            try
            {
                XDocument doc = XDocument.Parse(xml);
                string docStr = doc.ToString();
                File.WriteAllText("c:\\xmlOutput.txt", docStr);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }
    }
}
