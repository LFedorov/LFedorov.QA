using System.Collections.Generic;
using Domain.Entities;
using FluentNHibernate.Testing.Values;

namespace Presentation.Web.Models
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
            Disciplines = new List<Discipline>();
        }

        public int TotalAccounts { get; set; }
        public int TotalDisciplines { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAnswers { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; } 
    }

    public class HomeDisciplineViewModel
    {
        public HomeDisciplineViewModel()
        {
            Questions = new List<Question>();
        }

        public Discipline Discipline { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<Question> Questions { get; set; } 
    }

    public class HomeSearchViewModel
    {
        public HomeSearchViewModel()
        {
            Questions = new List<Question>();
        }

        public string SearchQuery { get; set; }
        public IEnumerable<Question> Questions { get; set; } 
    }

    public class HomeLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}