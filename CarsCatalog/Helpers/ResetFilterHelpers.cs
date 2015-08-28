using System.Text;
using System.Web.Mvc;
using CarsCatalog.Models;

namespace CarsCatalog.Helpers
{
    public static class ResetFilterHelpers
    {
        public static MvcHtmlString ResetFilter(this HtmlHelper html, FilterParams filter, string brandName,
            string modelName)
        {
            TagBuilder tagUl = new TagBuilder("ul");

            StringBuilder builder = new StringBuilder();

            tagUl.AddCssClass("filter-active");
            TagBuilder tagLi;
            TagBuilder divContent = new TagBuilder("div");
            divContent.AddCssClass("selected-filter-link");

            TagBuilder crossBtn = new TagBuilder("span");
            crossBtn.AddCssClass("remove-filter");
            crossBtn.InnerHtml = "x";

            if (filter.BrandId.HasValue)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                divContent.InnerHtml = brandName + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.MergeAttribute("id", "remove-brand-filter");
                builder.Append(tagLi);
            }
            if (filter.ModelId.HasValue)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                divContent.InnerHtml = modelName + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.MergeAttribute("id", "remove-model-filter");
                builder.Append(tagLi);
            }
            if (filter.Color != null)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                divContent.InnerHtml = filter.Color + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.MergeAttribute("id", "remove-color-filter");
                builder.Append(tagLi);
            }
            if (filter.Engine != null)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                divContent.InnerHtml = filter.Engine + "L" + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.MergeAttribute("id", "remove-engine-filter");
                builder.Append(tagLi);
            }
            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue || filter.Date.HasValue)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                string priceFilter = "";
                if (filter.MinPrice.HasValue)
                {
                    priceFilter += "Price from " + filter.MinPrice.Value.ToString("c0");

                }
                if (filter.MaxPrice.HasValue)
                {

                    if (filter.MinPrice.HasValue)
                    {
                        priceFilter += " to " + filter.MaxPrice.Value.ToString("c0");

                    }
                    else
                    {
                        priceFilter = "Price to " + filter.MaxPrice.Value.ToString("c0");
                    }
                }
                if (filter.Date.HasValue)
                {
                    if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
                    {
                        priceFilter += " for " + filter.Date.Value.ToString("d");
                    }
                    else
                    {
                        priceFilter = "Price for " + filter.Date.Value.ToString("d");
                    }

                }

                divContent.InnerHtml = priceFilter + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.MergeAttribute("id", "remove-price-filter");
                builder.Append(tagLi);
            }

            if (filter.BrandId.HasValue || filter.ModelId.HasValue || filter.Color != null || filter.Engine != null ||
                filter.MinPrice.HasValue || filter.MaxPrice.HasValue || filter.Date.HasValue)
            {
                tagLi = new TagBuilder("li");
                tagLi.AddCssClass("selected-filter");

                divContent.InnerHtml = "Clear All" + crossBtn;
                tagLi.InnerHtml = divContent.ToString();
                tagLi.GenerateId("remove-all-filters");
                builder.Append(tagLi);
            }

            tagUl.InnerHtml = builder.ToString();
            return new MvcHtmlString(tagUl.ToString());
        }
    }
}