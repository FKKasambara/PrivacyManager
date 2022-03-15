using PrivacyManager.Services;
using PrivacyManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyManager.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public ActionResult Index(string search, int? page, int? items)
        {
            HomeViewModel model = new HomeViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "POPIA GAP ANALYSIS",
                PageDescription = "POPIA TOOL"
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 9;

            var quizzesSearch = QuizzesService.Instance.GetQuizzesForHomePage(model.searchTerm, model.pageNo, model.pageSize);

            model.Quizzes = quizzesSearch.Quizzes;
            model.TotalCount = quizzesSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return View(model);
        }
        public ActionResult Index2(string search, int? page, int? items)
        {
            HomeViewModel model = new HomeViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "POPIA GAP ANALYSIS",
                PageDescription = "POPIA TOOL"
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 9;

            var quizzesSearch = QuizzesService.Instance.GetQuizzesForHomePage(model.searchTerm, model.pageNo, model.pageSize);

            model.Quizzes = quizzesSearch.Quizzes;
            model.TotalCount = quizzesSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return View(model);
        }
    }
}