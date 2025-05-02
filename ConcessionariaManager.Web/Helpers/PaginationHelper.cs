using ConcessionariaManager.Core.Interfaces;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConcessionariaManager.Web.Helpers
{
    public static class PaginationHelper
    {
        public static IHtmlContent CustomPager<T>(
            this IHtmlHelper html,
            IPagedList<T> pagedList,
            Func<int, string> generatePageUrl)
        {
            if (pagedList.PageCount <= 1)
                return HtmlString.Empty;

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            void AddPageItem(int pageNumber, string text, bool active = false, bool disabled = false)
            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (active) li.AddCssClass("active");
                if (disabled) li.AddCssClass("disabled");

                var a = new TagBuilder("a");
                a.AddCssClass("page-link");
                a.InnerHtml.Append(text);

                if (!disabled)
                    a.Attributes["href"] = generatePageUrl(pageNumber);
                else
                    a.Attributes["tabindex"] = "-1";

                li.InnerHtml.AppendHtml(a);
                ul.InnerHtml.AppendHtml(li);
            }

            // Página anterior
            AddPageItem(pagedList.PageNumber - 1, "«", disabled: pagedList.PageNumber == 1);

            // Páginas
            for (int i = 1; i <= pagedList.PageCount; i++)
            {
                AddPageItem(i, i.ToString(), active: pagedList.PageNumber == i);
            }

            // Próxima página
            AddPageItem(pagedList.PageNumber + 1, "»", disabled: pagedList.PageNumber == pagedList.PageCount);

            return ul;
        }
    }
}