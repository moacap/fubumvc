using System;
using System.Linq.Expressions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;
using FubuMVC.Core.Util;
using FubuMVC.Core;

namespace FubuMVC.UI
{
    public static class FubuPageExtensions
    {
        public static TagGenerator<T> Tags<T>(this IFubuPage<T> page) where T : class
        {
            var generator = page.Get<TagGenerator<T>>();
            generator.Model = page.Model;
            return generator;
        }

        public static void Partial<TInputModel>(this IFubuPage page) where TInputModel : class
        {
            page.Get<IPartialFactory>().BuildPartial(typeof(TInputModel)).InvokePartial();
        }

        public static HtmlTag LinkTo<TInputModel>(this IFubuPage page) where TInputModel : class, new()
        {
            return page.LinkTo(new TInputModel());
        }

        public static HtmlTag LinkTo(this IFubuPage page, object inputModel)
        {
            return new LinkTag("", page.Urls.UrlFor(inputModel));
        }

        public static HtmlTag InputFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().InputFor(expression);
        }

        public static HtmlTag LabelFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().LabelFor(expression);
        }

        public static HtmlTag DisplayFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Tags().DisplayFor(expression);
        }

        
        public static string ElementNameFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            return page.Get<IElementNamingConvention>().GetName(typeof (T), expression.ToAccessor());
        }

        public static TextboxTag TextBoxFor<T>(this IFubuPage<T> page, Expression<Func<T, object>> expression) where T : class
        {
            string name = ElementNameFor(page, expression);
            string value = page.Model.ValueOrDefault(expression).ToString();
            return new TextboxTag(name, value);
        }

        public static FormTag FormFor(this IFubuPage page)
        {
            return new FormTag();
        }

        public static FormTag FormFor(this IFubuPage page, string url)
        {
            url = UrlContext.GetFullUrl(url);
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page) where TInputModel : new()
        {
            string url = page.Get<IUrlRegistry>().UrlFor(new TInputModel());
            url = UrlContext.GetFullUrl(url);
            return new FormTag(url);
        }

        public static FormTag FormFor<TInputModel>(this IFubuPage page, TInputModel model)
        {
            string url = page.Get<IUrlRegistry>().UrlFor(model);
            url = UrlContext.GetFullUrl(url);
            return new FormTag(url);
        }

        public static string EndForm(this IFubuPage page)
        {
            return "</form>";
        }
    }
}