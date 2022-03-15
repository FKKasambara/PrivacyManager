using PrivacyManager.Data;
using PrivacyManager.Entities;
using PrivacyManager.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Services
{
    public class UsersQuestionnairesService
    {
        #region Define as Singleton
        private static UsersQuestionnairesService _Instance;

        public static UsersQuestionnairesService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new UsersQuestionnairesService();
                }

                return (_Instance);
            }
        }

        private UsersQuestionnairesService()
        {
        }
        #endregion
        public StudentQuizzesSearch GetQuizAttempts(string searchTerm, int pageNo, int pageSize)
        {
            using (var context = new PrivacyManagerContext())
            {
                var search = new StudentQuizzesSearch();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    search.StudentQuizzes = context.StudentQuizzes
                                        .Include("AttemptedQuestions")
                                        .Include("Quiz")
                                        .Include("Quiz.Questions")
                                        .Include("Quiz.Questions.Options")
                                        .Include("Student")
                                        .OrderByDescending(x => x.ModifiedOn)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.StudentQuizzes.Count();
                }
                else
                {
                    search.StudentQuizzes = context.StudentQuizzes
                                        .Include("AttemptedQuestions")
                                        .Include("Quiz")
                                        .Include("Quiz.Questions")
                                        .Include("Quiz.Questions.Options")
                                        .Include("Student")
                                        .Where(x => x.Quiz.Name.ToLower().Contains(searchTerm.ToLower()))
                                        .OrderByDescending(x => x.ModifiedOn)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.StudentQuizzes.Include(q => q.Quiz)
                                        .Where(x => x.Quiz.Name.ToLower().Contains(searchTerm.ToLower())).Count();
                }

                return search;
            }
        }

        public StudentQuizzesSearch GetUserQuizAttempts(string userID, string searchTerm, int pageNo, int pageSize)
        {
            using (var context = new PrivacyManagerContext())
            {
                var search = new StudentQuizzesSearch();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    search.StudentQuizzes = context.StudentQuizzes
                                        .Where(x => x.StudentID == userID)
                                        .Include("AttemptedQuestions")
                                        .Include("Quiz")
                                        .Include("Quiz.Questions")
                                        .Include("Quiz.Questions.Options")
                                        .Include("Student")
                                        .OrderByDescending(x => x.ModifiedOn)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.StudentQuizzes
                                        .Where(x => x.StudentID == userID).Count();
                }
                else
                {
                    search.StudentQuizzes = context.StudentQuizzes
                                        .Where(x => x.StudentID == userID)
                                        .Include("AttemptedQuestions")
                                        .Include("Quiz")
                                        .Include("Quiz.Questions")
                                        .Include("Quiz.Questions.Options")
                                        .Include("Student")
                                        .Where(x => x.Quiz.Name.ToLower().Contains(searchTerm.ToLower()))
                                        .OrderByDescending(x => x.ModifiedOn)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.StudentQuizzes
                                        .Where(x => x.StudentID == userID).Include(q => q.Quiz)
                                        .Where(x => x.Quiz.Name.ToLower().Contains(searchTerm.ToLower())).Count();
                }

                return search;
            }
        }

        public StudentQuiz GetStudentQuiz(int ID)
        {
            using (var context = new PrivacyManagerContext())
            {
                return context.StudentQuizzes
                    .Where(x=>x.ID == ID)
                    .Include("AttemptedQuestions")
                    .Include("AttemptedQuestions.SelectedOptions")
                    .Include("AttemptedQuestions.SelectedOptions.Option")
                    .Include("AttemptedQuestions.SelectedOptions.Option.Image")
                    .Include("Quiz")
                    .Include("Quiz.Questions")
                    .Include("Quiz.Questions.Options")
                    .Include("Quiz.Questions.Options.Image")
                    .Include("Student")
                    .FirstOrDefault();
            }
        }

        public async Task<bool> NewStudentQuiz(StudentQuiz studentQuiz)
        {
            using (var context = new PrivacyManagerContext())
            {
                context.StudentQuizzes.Add(studentQuiz);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateStudentQuiz(StudentQuiz studentQuiz)
        {
            using (var context = new PrivacyManagerContext())
            {
                context.Entry(studentQuiz).State = System.Data.Entity.EntityState.Modified;

                return await context.SaveChangesAsync() > 0;
            }
        }
        
        public async Task<bool> NewAttemptedQuestion(AttemptedQuestion attemptedQuestion)
        {
            using (var context = new PrivacyManagerContext())
            {
                context.AttemptedQuestions.Add(attemptedQuestion);

                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
