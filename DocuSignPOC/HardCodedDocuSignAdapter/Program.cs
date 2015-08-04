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
            string statusMsg;
            List<DocInfo> documents = new List<DocInfo>();
            bool bResponse = SignDocument.SendForSigning("my_name", "my_email_id", documents, out statusMsg);
            Console.WriteLine("The response from DocuSignPOC was: " + bResponse + " and the status message was: " + statusMsg);
            Console.ReadKey();
        }
    }
}
