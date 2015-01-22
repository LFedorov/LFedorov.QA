using System.Collections.Generic;
using Domain.Entities;

namespace Presentation.Web.Models
{
    public class HomeTestingViewModel
    {
        public HomeTestingViewModel()
        {
            Questions = new List<Question>();
        }

        public List<Question> Questions { get; set; }
    }
}