using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignPOC
{
    public class DocInfo
    {
        public string DocName;
        public string TemplateId;
        public string FolderName;

        public DocInfo(string docName, string templateId, string folderName)
        {
            this.DocName = docName;
            this.TemplateId = templateId;
            this.FolderName = folderName;
        }
    }
}
