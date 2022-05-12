using Microsoft.AspNet.Identity;
using PrivacyManager.Commons;
using PrivacyManager.Entities;
using PrivacyManager.Services;
using PrivacyManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PrivacyManager.Controllers
{
  
    public class AttemptQuizController : BaseController
    {
        [HttpGet]
        [Authorize]
        public ActionResult Attempt(int QuizID)
        {
            var quiz = QuizzesService.Instance.GetQuiz(QuizID);

            if (quiz == null) return HttpNotFound();

            QuizDetailViewModel model = new QuizDetailViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = string.Format("Gap Analysis : {0}", quiz.Name),
                PageDescription = quiz.Description
            };

            model.ID = quiz.ID;
            model.Name = quiz.Name;
            model.Description = quiz.Description;
            model.TimeDuration = quiz.TimeDuration;
            model.EnableQuizTimer = quiz.EnableQuizTimer;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Attempt(AttemptQuizViewModel model, HttpPostedFileBase file)
        {
            var quiz = QuizzesService.Instance.GetQuiz(model.QuizID);

            if (quiz == null) return HttpNotFound();

            StudentQuiz studentQuiz = new StudentQuiz();

            studentQuiz.StudentID = User.Identity.GetUserId();
            studentQuiz.QuizID = quiz.ID;
            studentQuiz.StartedAt = DateTime.Now;
            studentQuiz.ModifiedOn = DateTime.Now;

            if (await UsersQuestionnairesService.Instance.NewStudentQuiz(studentQuiz))
            {
                model.QuizType = quiz.QuizType;
                model.StudentQuizID = studentQuiz.ID;
                model.TotalQuestions = quiz.Questions.Count;
                model.Question = quiz.Questions.FirstOrDefault();
                model.QuestionIndex = 0;

                model.Options = new List<Option>();
                model.Options.AddRange(model.Question.Options);
                model.Options.Shuffle();

                model.EnableQuestionTimer = quiz.EnableQuestionTimer;
                model.Seconds = Calculator.CalculateAllowedQuestionTime(quiz);

                return PartialView("_QuizQuestion", model);
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AnswerQuestion(AttemptQuizViewModel model)
        {
            Thread.Sleep(2000);

            var studentQuiz = UsersQuestionnairesService.Instance.GetStudentQuiz(model.StudentQuizID);

            if (studentQuiz == null) return new HttpStatusCodeResult(500);

            if (model.TimerExpired)
            {
                studentQuiz.CompletedAt = DateTime.Now;
                studentQuiz.ModifiedOn = DateTime.Now;

                if (await UsersQuestionnairesService.Instance.UpdateStudentQuiz(studentQuiz))
                {
                    StudentQuizViewModel studentQuizModel = new StudentQuizViewModel();

                    studentQuizModel.StudentQuiz = studentQuiz;
                    studentQuizModel.TimerExpired = model.TimerExpired;

                    return PartialView("_AttemptDetails", studentQuizModel);
                }
                else return new HttpStatusCodeResult(500);
            }
            else
            {
                var quiz = QuizzesService.Instance.GetQuiz(model.QuizID);

                if (quiz == null) return HttpNotFound();

                var question = QuestionsService.Instance.GetQuizQuestion(quiz.ID, model.QuestionID);

                if (question == null) return HttpNotFound();

                var selectedOptions = QuestionsService.Instance.GetOptionsByIDs(model.SelectedOptionIDs.CSVToListInt());

                if (selectedOptions == null) return HttpNotFound();

                AttemptedQuestion attemptedQuestion = new AttemptedQuestion();

                attemptedQuestion.StudentQuizID = studentQuiz.ID;
                attemptedQuestion.QuestionID = question.ID;
                attemptedQuestion.SelectedOptions = selectedOptions.Select(x => new AttemptedOption() { AttemptedQuestionID = attemptedQuestion.ID, Option = x, OptionID = x.ID }).ToList();
                attemptedQuestion.Score = Calculator.CalculateAttemptedQuestionScore(question.Options, attemptedQuestion.SelectedOptions);
                attemptedQuestion.AnsweredAt = DateTime.Now;
                attemptedQuestion.ModifiedOn = DateTime.Now;
                if(model.UploadFile != null )
                {
                    attemptedQuestion.UploadFileName = Path.GetFileName(model.UploadFile.FileName);
                    attemptedQuestion.UploadFilePath = Path.Combine(Server.MapPath("~/UploadedFiles/" + studentQuiz.Quiz.Name), attemptedQuestion.UploadFileName); ;
                }

                if (await UsersQuestionnairesService.Instance.NewAttemptedQuestion(attemptedQuestion))
                {
                    if (model.QuestionIndex != quiz.Questions.Count() - 1)
                    {
                        model.QuizType = quiz.QuizType;

                        model.TotalQuestions = quiz.Questions.Count;
                        model.Question = quiz.Questions.ElementAtOrDefault(model.QuestionIndex + 1);
                        model.QuestionIndex = model.QuestionIndex + 1;

                        model.Options = new List<Option>();
                        model.Options.AddRange(model.Question.Options);
                        model.Options.Shuffle();

                        model.EnableQuestionTimer = quiz.EnableQuestionTimer;
                        model.Seconds = Calculator.CalculateAllowedQuestionTime(quiz);

                        return PartialView("_QuizQuestion", model);
                    }
                    else //this was the Last question so display the result
                    {
                        studentQuiz.CompletedAt = DateTime.Now;

                        if (!await UsersQuestionnairesService.Instance.UpdateStudentQuiz(studentQuiz))
                            return new HttpStatusCodeResult(500);
                        
                        return RedirectToAction("AttemptDetails", new { studentQuizID = studentQuiz.ID, isPartial = true, timerExpired = model.TimerExpired });
                    }
                }
                else return new HttpStatusCodeResult(500);
            }
        }

		[HttpGet]
		public ActionResult AttemptDetails(int studentQuizID, bool? timerExpired, bool? isPartial = false)
		{
			var studentQuiz = UsersQuestionnairesService.Instance.GetStudentQuiz(studentQuizID);

            if (studentQuiz == null) return HttpNotFound();

			StudentQuizViewModel model = new StudentQuizViewModel();

            model.TimerExpired = timerExpired.HasValue ? timerExpired.Value : false;

			model.PageInfo = new PageInfo()
			{
				PageTitle = "Questionnaire Attempt Details",
				PageDescription = "Details of Attempted Questionnaire"
			};

			model.StudentQuiz = studentQuiz;

            if(isPartial.HasValue && isPartial.Value)
            {
                return PartialView("_AttemptDetails", model);
            }
            else
            {
                return View(model);
            }
		}

        [HttpGet]
        public ActionResult MyResults(string search, int? page, int? items, bool? isPartial)
        {
            StudentQuizListViewModel model = new StudentQuizListViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Self Assessment Results",
                PageDescription = "Results of Selft Assessment"
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 10;
            
            var resultsSearch = UsersQuestionnairesService.Instance.GetUserQuizAttempts(User.Identity.GetUserId(), model.searchTerm, model.pageNo, model.pageSize);

            model.StudentQuizzes = resultsSearch.StudentQuizzes;
            model.TotalCount = resultsSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            if (isPartial.HasValue && isPartial.Value)
            {
                model.isPartialView = true;
                return PartialView(model);
            }
            else return View(model);
        }

        [HttpGet]
        public ActionResult PrintResult(int studentQuizID)
        {
            var studentQuiz = UsersQuestionnairesService.Instance.GetStudentQuiz(studentQuizID);

            if (studentQuiz == null) return HttpNotFound();

            StudentQuizViewModel model = new StudentQuizViewModel();
            
            model.PageInfo = new PageInfo()
            {
                PageTitle = "Questionnaire Attempt Details",
                PageDescription = "Details of Attempted Questionnaire"
            };

            model.StudentQuiz = studentQuiz;

            return PartialView("_PrintResult", model);
        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    file.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
    }
}