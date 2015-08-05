using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignPOC
{
    public class SignDocument
    {
        public static void Test()
        {
            Console.WriteLine("Hello Worjkjhhhhlld!");

        }

        public static bool SendForSigning(string customerName, string customerEmail, List<DocInfo> documents, out string statusMsg)
        {
            statusMsg = "All is well!"; // 
            return true;
        }
    }
}
