using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Entities
{
    public class AttemptedOptionFile : BaseEntity
    {
        public int AttemptedOptionID { get; set; }
        public virtual AttemptedOption AttemptedOption { get; set; }

        public int UploadFileID { get; set; }
        public virtual UploadFile UploadFile { get; set; }
    }
}
