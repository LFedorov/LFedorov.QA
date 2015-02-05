using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Domain.Common.Command;
using Domain.Common.Query;
using Domain.Entities.Commands.QuestionCommands;
using Domain.Entities.Queries.AccountQueries;
using Domain.Entities.Queries.AnswerQueries;
using Domain.Entities.Queries.DisciplineQueries;
using Domain.Entities.Queries.QuestionQueries;
using LFedorov.Moodle;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Parser _pageParser = new Parser();
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public HomeController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        public ActionResult Index()
        {
            var user = (ClaimsIdentity)AuthenticationManager.User.Identity;
            ViewBag.User = user.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).SingleOrDefault();

            var viewModel = new HomeIndexViewModel
            {
                TotalAccounts = _queryDispatcher.Ask(new AccountsCountQuery()),
                TotalDisciplines = _queryDispatcher.Ask(new DisciplinesCountQuery()),
                TotalQuestions = _queryDispatcher.Ask(new QuestionsCountQuery()),
                TotalAnswers = _queryDispatcher.Ask(new AnswersCountQuery()),
                Disciplines = _queryDispatcher.Ask(new DisciplinesQuery())
            };

            return View(viewModel);
        }

        [Route("discipline-{id:int:min(1)}/page-{page:int:min(1)=1}")]
        public ActionResult Discipline(int id, int page = 1)
        {
            const int QuestionsPerPage = 10;

            if (id == 0) return RedirectToAction("Index");

            var discipline = _queryDispatcher.Ask(new DisciplineByIdQuery(id));

            if (discipline == null) return RedirectToAction("Index");

            var _totalQuestions = _queryDispatcher.Ask(new QuestionsByDisciplineCount(id));
            var totalPages = (int)Math.Ceiling((0.0 + _totalQuestions) / QuestionsPerPage);
            var questions = _queryDispatcher.Ask(new QuestionsByDisciplinePaged(id, page, QuestionsPerPage));

            var viewModel = new HomeDisciplineViewModel
            {
                Discipline = discipline,
                CurrentPage = page,
                TotalPages = totalPages,
                Questions = questions
            };

            return View(viewModel);
        }

        public ActionResult Search(string searchQuery, int page = 1)
        {
            ViewBag.SearchQuery = searchQuery;

            var viewModel = new HomeSearchViewModel
            {
                SearchQuery = searchQuery,
                Questions = _queryDispatcher.Ask(new QuestionsSearchQuery(searchQuery))
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> postedFiles)
        {
            if (postedFiles == null) return RedirectToAction("Index");

            foreach (var questions in postedFiles.Select(file => _pageParser.GetContent(file.InputStream, file.ContentType)))
            {
                _commandDispatcher.Send(new SaveQuestionsCommand(questions));
            }

            return RedirectToAction("Index");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (AuthenticationManager.User.Identity.IsAuthenticated) return RedirectToAction("Index");
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(HomeLoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = _queryDispatcher.Ask(new AccountVerificationQuery(viewModel.Email, viewModel.Password));

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(viewModel);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index");
        }

        #region Test
        public ActionResult Test()
        {
            return View(new HomeTestingViewModel());
        }

        [HttpPost]
        public ActionResult Test(IEnumerable<HttpPostedFileBase> postedFiles)
        {
            var viewModel = new HomeTestingViewModel();

            foreach (var qestion in postedFiles.Select(file => _pageParser.GetContent(file.InputStream, file.ContentType)).SelectMany(qestions => qestions))
            {
                viewModel.Questions.Add(qestion);
            }

            return View(viewModel);
        }
        #endregion
    }
}