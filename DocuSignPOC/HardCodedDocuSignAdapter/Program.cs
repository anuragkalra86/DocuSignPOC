using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocuSignPOC;
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
            CopyDocuSignExample.Hello();
            
            Console.ReadKey();
        }

        private static void MakeRequest()
        {
            string customerName = "Anurag Kalra";
            string customerEmail = "anurag.kalra@safeauto.com";

            DocInfo document1 = new DocInfo("GA_Application", "74f811c9-d8f2-4c9a-920c-70b7cf4e0348", "pol1000");
            DocInfo document2 = new DocInfo("GA_Excluded_Driver_Form", "b7e31bd0-1303-4d2c-aa7d-f2b04fd82bf5", "pol1000");
            DocInfo document3 = new DocInfo("GA_UM_Selection_Form", "b57163df-fb47-4e0b-b21c-9654ab77c651", "pol1000");
            List<DocInfo> documents = new List<DocInfo>();
            documents.Add(document1);
            documents.Add(document2);
            documents.Add(document3);

            string statusMsg;
            bool bResponse = SignDocument.SendForSigning(customerName, customerEmail, documents, out statusMsg);
            Console.WriteLine("The response from DocuSignPOC was: " + bResponse + " and the status message was: " + statusMsg);

        }
    }
}
