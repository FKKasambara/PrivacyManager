using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Entities.CustomEntities
{
    public class PrivacyManagerSearch
    {
        public List<Quiz> Quizzes { get; set; }
        public int TotalCount { get; set; }
    }
}
