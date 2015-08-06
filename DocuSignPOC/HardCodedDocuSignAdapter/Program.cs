using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocuSignPOC.RoughCut;
using log4net;

namespace HardCodedDocuSignAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log = LogManager.GetLogger(typeof(Program));
            log.Info("Started the program.");

            //MakeRequest();
            RequestSignatureOnDocument.DemoStandalone();
            ////StandaloneSingleDoc.TestMe();
        }

        private static void MakeRequest()
        {
            Console.WriteLine("Creating Signing request for 1st customer");
            string customerName = "Anurag Kalra";
            string customerEmail = "anurag.kalra@safeauto.com";

            string docName1 = "GA_Application";
            string docName2 = "GA_Excluded_Driver_Form";
            string docName3 = "GA_UM_Selection_Form";
            string templateId1 = GetCorrespondingOverlayingTemplate(docName1);
            string templateId2 = GetCorrespondingOverlayingTemplate(docName2);
            string templateId3 = GetCorrespondingOverlayingTemplate(docName3);

            DocuSignPOC.DocInfo document1 = new DocuSignPOC.DocInfo(docName1, templateId1, "pol1000");
            DocuSignPOC.DocInfo document2 = new DocuSignPOC.DocInfo(docName2, templateId2, "pol1000");
            DocuSignPOC.DocInfo document3 = new DocuSignPOC.DocInfo(docName3, templateId3, "pol1000");
            List<DocuSignPOC.DocInfo> documents = new List<DocuSignPOC.DocInfo>();
            documents.Add(document1);
            documents.Add(document2);
            documents.Add(document3);

            string statusMsg;
            bool bResponse = SignDocument.SendForSigning(customerName, customerEmail, documents, out statusMsg);
            Console.WriteLine("The response from DocuSignPOC was: " + bResponse + " and the status message was: " + statusMsg);

            //------------------------
            Console.WriteLine("Creating Signing request for 2nd customer");
            Console.WriteLine("2 documents will be sent for signing");

            customerName = "Ronnie Davies";
            customerEmail = "Ronnie.Davies@safeauto.com";
            docName1 = "GA_Application";
            docName2 = "GA_UM_Selection_Form";
            templateId1 = GetCorrespondingOverlayingTemplate(docName1);
            templateId2 = GetCorrespondingOverlayingTemplate(docName2);
            document1 = new DocuSignPOC.DocInfo(docName1, templateId1, "pol1001");
            document2 = new DocuSignPOC.DocInfo(docName2, templateId2, "pol1001");
            documents = new List<DocuSignPOC.DocInfo>();
            documents.Add(document1);
            documents.Add(document2);
            bResponse = SignDocument.SendForSigning(customerName, customerEmail, documents, out statusMsg);
            Console.WriteLine("The response from DocuSignPOC was: " + bResponse + " and the status message was: " + statusMsg);

            //---------------------
            Console.WriteLine("Creating Signing request for 3rd customer");
            Console.WriteLine("1 document will be sent for signing");

            customerName = "Anurag Kalra";
            customerEmail = "kalra.25@osu.edu";
            docName1 = "GA_UM_Selection_Form";
            templateId1 = GetCorrespondingOverlayingTemplate(docName1);
            document1 = new DocuSignPOC.DocInfo(docName1, templateId1, "pol1002");
            documents = new List<DocuSignPOC.DocInfo>();
            documents.Add(document1);
            bResponse = SignDocument.SendForSigning(customerName, customerEmail, documents, out statusMsg);
            Console.WriteLine("The response from DocuSignPOC was: " + bResponse + " and the status message was: " + statusMsg);
        }

        private static string GetCorrespondingOverlayingTemplate(string docName)
        {
            if (docName.Equals("GA_Application", StringComparison.OrdinalIgnoreCase))
            {
                return "74f811c9-d8f2-4c9a-920c-70b7cf4e0348";
            }
            else if (docName.Equals("GA_Excluded_Driver_Form", StringComparison.OrdinalIgnoreCase))
            {
                return "b7e31bd0-1303-4d2c-aa7d-f2b04fd82bf5";
            }
            else if (docName.Equals("GA_UM_Selection_Form", StringComparison.OrdinalIgnoreCase))
            {
                return "b57163df-fb47-4e0b-b21c-9654ab77c651";
            }
            else
            {
                throw new ApplicationException("No matching template was found for document: " + docName );
            }
        }
    }
}
