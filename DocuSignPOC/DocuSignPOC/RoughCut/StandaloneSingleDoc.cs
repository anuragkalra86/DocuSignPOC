// DocuSign API Walkthrough 04 in C# - Request Signature on a Document

//

// To run this sample: 

//     1) Create a new .NET project.

//     2) Add 4 assembly references to the project:  System, System.Net, System.XML, and System.XML.Linq

//     3) Update the email, password, integrator key, recipient name, and document name in the code

//     4) Copy a sample PDF file into project directory with same name as the document name you set in the code

//     5) Compile and Run

//

// NOTE 1: The DocuSign REST API accepts both JSON and XML formatted http requests.  These C# API walkthroughs

//        demonstrate the use of XML format, whereas the other walkthroughs show examples in JSON format.

using System;

using System.IO;

using System.Net;

using System.Xml;

using System.Xml.Linq;



namespace DocuSignPOC.RoughCut
{

    public class StandaloneSingleDoc
    {

        public static void TestMe()
        {

            //---------------------------------------------------------------------------------------------------

            // ENTER VALUES FOR THE FOLLOWING 6 VARIABLES:

            //---------------------------------------------------------------------------------------------------

            string username = "Ronnie.Davies@safeauto.com";                // your account email

            string password = "PAUSEBREAK401";                // your account password

            string integratorKey = "DOCU-7e719f55-a2f6-4999-ac22-d6a96018dfd6";                 // your account Integrator Key (found on Preferences -> API page)

            string recipientName = "Jared Brinn";                    // recipient (signer) name

            string recipientEmail = "anurag.kalra@safeauto.com";                 // recipient (signer) email

            string documentName = "GA_Application.pdf";                    // copy document with same name and extension into project directory (i.e. "test.pdf")

            string contentType = "application/pdf";           // default content type is PDF

            string templateId = "f7bfb10e-ba2e-4019-a3e8-8c8d009e49fb";

            string secondDocument = "";

            //---------------------------------------------------------------------------------------------------



            // additional variable declarations

            string baseURL = "";                // - we will retrieve this through the Login API call



            try
            {

                //============================================================================

                //  STEP 1 - Login API Call (used to retrieve your baseUrl)

                //============================================================================



                // Endpoint for Login api call (in demo environment):

                string url = "https://demo.docusign.net/restapi/v2/login_information";



                // set request url, method, and headers.  No body needed for login api call

                HttpWebRequest request = initializeRequest(url, "GET", null, username, password, integratorKey);



                // read the http response

                string response = getResponseBody(request);



                // parse baseUrl from response body

                baseURL = parseDataFromResponse(response, "baseUrl");



                //--- display results

                Console.WriteLine("\nAPI Call Result: \n\n" + prettyPrintXml(response));



                //============================================================================

                //  STEP 2 - Send Signature Request from Template

                //============================================================================



                /*

                    This is the only DocuSign API call that requires a "multipart/form-data" content type.  We will be 

                    constructing a request body in the following format (each newline is a CRLF):

                    --AAA

                    Content-Type: application/xml

                    Content-Disposition: form-data

                    <XML BODY GOES HERE>

                    --AAA

                    Content-Type:application/pdf

                    Content-Disposition: file; filename="document.pdf"; documentid=1 

                    <DOCUMENT BYTES GO HERE>

                   --AAA

                    Content-Type:application/pdf

                    Content-Disposition: file; filename="document.pdf"; documentid=1 

                    <DOCUMENT BYTES GO HERE>

                    --AAA--

                 */



                // append "/envelopes" to baseURL and use for signature request api call

                url = baseURL + "/envelopes";



                // construct an outgoing XML formatted request body (JSON also accepted)

                // .. following body adds one signer and places a signature tab 100 pixels to the right

                // and 100 pixels down from the top left corner of the document you supply

                string xmlBody =

                        "<envelopeDefinition xmlns=\"http://www.docusign.com/restapi\">" +

                           "<status>sent</status>" +

                           "<emailBlurb>Trying to get two documents to sit in the same envelope</emailBlurb> " +

                           "<emailSubject>Get 2 docs please</emailSubject> " +

                           "<compositeTemplates>" +

                                "<compositeTemplate>" +

                                    "<serverTemplates>" +

                                        "<serverTemplate>" +

                                        "<sequence>1</sequence>" +

                                        "<templateId>" + templateId + "</templateId>" +

                                        "</serverTemplate>" +

                                    "</serverTemplates>" +

                                    //"<serverTemplates>" +

                                    //    "<serverTemplate>" +

                                    //    "<sequence>2</sequence>" +

                                    //    "<templateId>" + "acde0484-83bb-4a85-bbe0-bfe7e0269344" + "</templateId>" +

                                    //    "</serverTemplate>" +

                                    //"</serverTemplates>" +

                                    "<inlineTemplates>" +

                                    "<inlineTemplate>" +

                                        "<sequence>1</sequence>" +

                                        "<recipients> " +

                                            "<signers> " +

                                                "<signer> " +

                                                    "<email>" + recipientEmail + "</email>" +

                                                    "<name>" + recipientName + "</name> " +

                                                    "<recipientId>1</recipientId>" +

                                                    "<roleName>Sign</roleName> " +

                                                "</signer>" +

                                            "</signers>" +

                                        "</recipients>" +

                                    "</inlineTemplate>" +

                                    //"<inlineTemplate>" +

                                    //    "<sequence>2</sequence>" +

                                    //    "<recipients> " +

                                    //        "<signers> " +

                                    //            "<signer> " +

                                    //                "<email>" + recipientEmail + "</email>" +

                                    //                "<name>" + recipientName + "</name> " +

                                    //                "<recipientId>1</recipientId>" +

                                    //                "<roleName>Sign</roleName> " +

                                    //            "</signer>" +

                                    //        "</signers>" +

                                    //    "</recipients>" +

                                    //"</inlineTemplate>" +

                                    "</inlineTemplates>" +

                                    "<document>" +

                                        "<documentId>1</documentId>" +

                                        "<name>" + documentName + "</name>" +

                                    "</document>" +

                                    //"<document>" +

                                    //    "<documentId>2</documentId>" +

                                    //    "<name>" + "GA_Application.pdf" + "</name>" +

                                    //"</document>" +

                                "</compositeTemplate>" +

                            "</compositeTemplates>" +

                        "</envelopeDefinition>";





                // set request url, method, headers.  Don't set the body yet, we'll set that separelty after

                // we read the document bytes and configure the rest of the multipart/form-data request

                request = initializeRequest(url, "POST", null, username, password, integratorKey);



                // some extra config for this api call

                configureMultiPartFormDataRequest(request, xmlBody, documentName, contentType);



                // read the http response

                response = getResponseBody(request);



                //--- display results

                Console.WriteLine("\nAPI Call Result: \n\n" + prettyPrintXml(response));

            }

            catch (WebException e)
            {

                using (WebResponse response = e.Response)
                {

                    HttpWebResponse httpResponse = (HttpWebResponse)response;

                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);

                    using (Stream data = response.GetResponseStream())
                    {

                        string text = new StreamReader(data).ReadToEnd();

                        Console.WriteLine(prettyPrintXml(text));

                    }

                }

            }

        } // end main()



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

        public static void configureMultiPartFormDataRequest(HttpWebRequest request, string xmlBody, string docName, string contentType)
        {

            // overwrite the default content-type header and set a boundary marker

            request.ContentType = "multipart/form-data; boundary=BOUNDARY";



            // start building the multipart request body

            string requestBodyStart = "\r\n\r\n--BOUNDARY\r\n" +

                "Content-Type: application/xml\r\n" +

                    "Content-Disposition: form-data\r\n" +

                    "\r\n" +

                    xmlBody + "\r\n\r\n--BOUNDARY\r\n" +      // our xml formatted envelopeDefinition

                    "Content-Type: " + contentType + "\r\n" +

                    "Content-Disposition: file; filename=\"" + docName + "\"; documentId=1\r\n" +

                    //"\r\n"+

                    //"\r\n\r\n--BOUNDARY\r\n" + //second dcoument now

                    //"Content-Type: " + contentType + "\r\n" +

                    //"Content-Disposition: file; filename=\"" + "GA_Application.pdf" + "\"; documentId=2\r\n" +

                    "\r\n";

            //string secondDocument = "\r\n\r\n--BOUNDARY\r\n" + //second dcoument now

            //    "Content-Type: " + contentType + "\r\n" +

            //    "Content-Disposition: file; filename=\"" + "GA_Application.pdf" + "\"; documentId=2\r\n" +

            //    "\r\n";

            //string secondDoc

            string requestBodyEnd = "\r\n--BOUNDARY--\r\n\r\n";



            // read contents of provided document into the request stream

            FileStream documentOneStream = File.OpenRead(docName);

            //FileStream documentTwoStream = File.OpenRead("GA_Application.pdf");



            // write the body of the request

            byte[] bodyStart = System.Text.Encoding.UTF8.GetBytes(requestBodyStart.ToString());

            //byte[] secondDoc = System.Text.Encoding.UTF8.GetBytes(secondDocument.ToString()); // my addition for a second document

            byte[] bodyEnd = System.Text.Encoding.UTF8.GetBytes(requestBodyEnd.ToString());

            Stream dataStream = request.GetRequestStream();

            dataStream.Write(bodyStart, 0, requestBodyStart.ToString().Length);



            // Read the file contents and write them to the request stream.  We read in blocks of 4096 bytes

            byte[] buf = new byte[4096];

            int len;

            while ((len = documentOneStream.Read(buf, 0, 4096)) > 0)
            {

                dataStream.Write(buf, 0, len);

            }

            //dataStream.Write(secondDoc, 0, secondDocument.ToString().Length);     //second document

            //int leng;

            //while ((leng = documentTwoStream.Read(buf, 0, 4096)) > 0)

            //{

            //    dataStream.Write(buf, 0, len);

            //}

            dataStream.Write(bodyEnd, 0, requestBodyEnd.ToString().Length);

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

                return doc.ToString();

            }

            catch (Exception)
            {

                return xml;

            }

        }

    } // end class

} // end namespace
