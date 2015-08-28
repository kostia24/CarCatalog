using System;
using System.Text;
using System.Web.Mvc;
using CarsCatalog.Models;

namespace CarsCatalog.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUlr)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                if (i != 1 && (i < pagingInfo.CurrentPage - 3 || i > pagingInfo.CurrentPage + 3) &&
                    i != pagingInfo.TotalPages) continue; // don`t show not necessary  pages
                if (i != 1 && (i == pagingInfo.CurrentPage - 3 || pagingInfo.CurrentPage + 3 == i) && i != pagingInfo.TotalPages)
                {
                    builder.Append("<span class='btn'>...</span>"); // show interval instead redundant pages 
                    continue;
                }
                TagBuilder tag = new TagBuilder("a");

                tag.MergeAttribute("href", pageUlr(i));
                tag.InnerHtml = i.ToString();

                tag.AddCssClass(i == pagingInfo.CurrentPage ? "page-link btn btn-primary" : "page-link btn btn-default");
                builder.Append(tag);
            }
            return new MvcHtmlString(builder.ToString());
        }
    }
}