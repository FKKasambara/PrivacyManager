using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Entities
{
    public class UploadFile : BaseEntity
    {
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
    }
}
