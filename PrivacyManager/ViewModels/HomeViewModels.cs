using PrivacyManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrivacyManager.ViewModels
{
    public class HomeViewModel : ListingBaseViewModel
    {
        public List<Quiz> Quizzes { get; set; }
    }
}