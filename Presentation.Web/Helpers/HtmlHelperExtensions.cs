using System;
using System.Text;
using System.Web.Mvc;

namespace Presentation.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Paging(this HtmlHelper html, Func<int, string> pageUrl, int current, int total)
        {
            var builder = new StringBuilder();
            builder.Append("<ul class=\"pagination\">");

            var first = (current == 1) ? GetPageLink(pageUrl(1), "<<", false, true) : GetPageLink(pageUrl(1), "<<");
            builder.Append(first);

            var prev = (current == 1) ? GetPageLink(pageUrl(current - 1), "<", false, true) : GetPageLink(pageUrl(current - 1), "<");
            builder.Append(prev);

            var next = (current == total) ? GetPageLink(pageUrl(current + 1), ">", false, true) : GetPageLink(pageUrl(current + 1), ">");
            builder.Append(next);

            var last = (current == total) ? GetPageLink(pageUrl(total), ">>", false, true) : GetPageLink(pageUrl(total), ">>");
            builder.Append(last);

            builder.Append("</ul>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string GetPageLink(string url, string display, bool active = false, bool disabled = false)
        {
            var liTag = new TagBuilder("li");

            if (disabled)
            {
                liTag.MergeAttribute("class", "disabled");
                var spanTag = new TagBuilder("span") { InnerHtml = display };
                liTag.InnerHtml = spanTag.ToString();
            }
            else
            {
                if (active)
                {
                    liTag.MergeAttribute("class", "active");
                }

                var aTag = new TagBuilder("a");
                aTag.MergeAttribute("href", url);
                aTag.InnerHtml = display;
                liTag.InnerHtml = aTag.ToString();
            }

            return liTag.ToString();
        }
    }
}