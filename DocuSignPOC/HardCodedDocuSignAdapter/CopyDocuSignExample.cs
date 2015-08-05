﻿// DocuSign API Walkthrough 01 in C# - Request Signature from Template
//
// To run this sample: 
// 	1) Create a new .NET project.
//	2) Add 4 assembly references to the project:  System, System.Net, System.XML, and System.XML.Linq
//	3) Update the email, password, integrator key, and template variables in the code
//	4) Compile and Run
//
// NOTE 1: This sample requires that you first create a Template through the DocuSign member Console.
//
// NOTE 2: The DocuSign REST API accepts both JSON and XML formatted http requests.  These C# API walkthroughs
// 	   demonstrate the use of XML format, whereas the other walkthroughs show examples in JSON format.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HardCodedDocuSignAdapter
{
    public class CopyDocuSignExample
    {	
		public static void Hello ()
		{
            //---------------------------------------------------------------------------------------------------
			// ENTER VALUES FOR THE FOLLOWING 7 VARIABLES:
			//---------------------------------------------------------------------------------------------------
			string username = "ronnie.davies@safeauto.com";		// your account email
			string password = "PAUSEBREAK401";		// your account password
            string integratorKey = "DOCU-7e719f55-a2f6-4999-ac22-d6a96018dfd6";		// your account Integrator Key (found on Preferences -> API page)
            string templateId = "ea9d6da2-2b2f-46d1-83da-b03dfad00817";		// valid templateId from a template in your account
			string templateRole = "sign";		// template role that exists on above template
			string recipientName = "Anurag Kalra";		// recipient (signer) name
			string recipientEmail = "kalra.25@osu.edu";		// recipient (signer) email
			//---------------------------------------------------------------------------------------------------
 
			// additional variable declarations
			string baseURL = "";			// we will retrieve this through the Login API call
 
			try {
				//============================================================================
				//  STEP 1 - Login API Call (used to retrieve your baseUrl)
				//============================================================================
 
				// Endpoint for Login api call (in demo environment):
				string url = "https://demo.docusign.net/restapi/v2/login_information";
 
				// set request url, method, and headers.  No body needed for login api call
				HttpWebRequest request = initializeRequest( url, "GET", null, username, password, integratorKey);
 
				// read the http response
				string response = getResponseBody(request);
 
				// parse baseUrl value from response body
				baseURL = parseDataFromResponse(response, "baseUrl");
                Console.WriteLine("\n baseURL: " + baseURL);
 
				//--- display results
				Console.WriteLine("\nAPI Call Result: \n\n" + prettyPrintXml(response));

                 
				//============================================================================
				//  STEP 2 - Send Signature Request from Template
				//============================================================================
 
				// append "/envelopes" to baseURL and use for signature request api call
                //url = baseURL + "/envelopes";
                url = baseURL + "/templates";
 
				// construct an outgoing XML formatted request body (JSON also accepted)
                /*
				string requestBody = 
					"<envelopeDefinition xmlns=\"http://www.docusign.com/restapi\">" +
						"<status>sent</status>" + 
						"<emailSubject>DocuSign API - Signature Request from Template</emailSubject>" +
						"<templateId>" + templateId + "</templateId>" +
                        "<templateRoles>" +
                            "<templateRole>" +
                                "<name>" + recipientName + "</name>" +
                                "<email>" + recipientEmail + "</email>" +
                                "<roleName>" + templateRole + "</roleName>" +
                            "</templateRole>" +
                        "</templateRoles>" + 
					"</envelopeDefinition>";
                */
                string requestBody = 
                    "<envelopeDefinition xmlns=\"http://www.docusign.com/restapi\"> " +
                    "<status>sent</status>" +
                    "<compositeTemplates>" +
                    "<compositeTemplate>" +
                    "<serverTemplates>" +
                    "<serverTemplate>" +
                    "<sequence>1</sequence>" +
                    "<templateId>" + templateId + "</templateId>" +
                    "</serverTemplate>" +
                    "</serverTemplates>" +
                    "<inlineTemplates>" +
                    "<inlineTemplate>" +
                    "<envelope>" +
                    "<emailBlurb>Seeing if I can run Kenny code</emailBlurb> <emailSubject>Template sticker testing</emailSubject> </envelope>"+
                "<sequence>2</sequence> <recipients> <signers> <signer>" +
                "<email>" +recipientEmail + "</email> <name>" + recipientName + "</name> <recipientId>1</recipientId> <roleName>sign</roleName>" +
                "</signer> </signers> </recipients> </inlineTemplate> </inlineTemplates> <document> <documentId>1</documentId>" +
                "<name>Title without wordart.pdf</name> <transformPdfFields>true</transformPdfFields>" +
                "<documentBase64>BYTES</documentBase64></document> </compositeTemplate> </compositeTemplates> </envelopeDefinition>"+
                " --AAA "+
                " Content-Type:application/pdf" +
                "Content-Disposition: file; filename=\"Title without wordart.pdf\"; documentid=1 "+
                File.ReadAllBytes("Title without wordart.pdf") +
                " --AAA--";
//                GET https://{server}/restapi/{apiVersion}/accounts/{accountId}/templates

 

//X-DocuSign-Authentication: <DocuSignCredentials><Username>{name}</Username><Password>{password}</Password><IntegratorKey>{integrator_key}</IntegratorKey></DocuSignCredentials>

//Accept: application/json

//Content-Type: application/json

				// set request url, method, body, and headers
                //request = initializeRequest(url, "POST", requestBody, username, password, integratorKey);
                request = initializeRequest(url, "GET", null, username, password, integratorKey);
 
				// read the http response
				response = getResponseBody(request);
 
				//--- display results
				Console.WriteLine("\nAPI Call Result: \n\n" + prettyPrintXml(response));
			}
			catch (WebException e) {
				using (WebResponse response = e.Response) {
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						string text = new StreamReader(data).ReadToEnd();
						Console.WriteLine(prettyPrintXml(text));
					}
				}
			}
            Console.ReadKey();
		} // end main()
 
		//***********************************************************************************************
		// --- HELPER FUNCTIONS ---
		//***********************************************************************************************
		public static HttpWebRequest initializeRequest(string url, string method, string body, string email, string password, string intKey)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Method = method;
			addRequestHeaders( request, email, password, intKey );
			if( body != null )
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
			request.Headers.Add ("X-DocuSign-Authentication", authenticateStr);
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
			byte[] body = System.Text.Encoding.UTF8.GetBytes (requestBody);
			Stream dataStream = request.GetRequestStream ();
			dataStream.Write (body, 0, requestBody.Length);
			dataStream.Close ();
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string getResponseBody(HttpWebRequest request)
		{
			// read the response stream into a local string
			HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse ();
			StreamReader sr = new StreamReader(webResponse.GetResponseStream());
			string responseText = sr.ReadToEnd();
			return responseText;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string parseDataFromResponse(string response, string searchToken)
		{
			// look for "searchToken" in the response body and parse its value
			using (XmlReader reader = XmlReader.Create(new StringReader(response))) {
				while (reader.Read()) {
					if((reader.NodeType == XmlNodeType.Element) && (reader.Name == searchToken))
						return reader.ReadString();
				}
			}
			return null;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string prettyPrintXml(string xml)
		{
			// print nicely formatted xml
			try {
				XDocument doc = XDocument.Parse(xml);
                string docStr = doc.ToString();
                File.WriteAllText("c:\\xmlOutput.txt", docStr);
				return doc.ToString();
			}
			catch (Exception) {
				return xml;
			}
		}
	} // end class
} // end namespace