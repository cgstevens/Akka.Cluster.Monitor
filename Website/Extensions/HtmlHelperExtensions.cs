using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Website.Extensions
{
    public static class HtmlHelperExtensions
    {
        #region Authorized action link - http://vivien-chevallier.com/Articles/create-an-authorized-action-link-extension-for-aspnet-mvc-3

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, routeValues, new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, routeValues, htmlAttributes, showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool showActionLinkAsDisabled)
        {
            if (htmlHelper.ActionAuthorized(actionName, controllerName))
            {
                return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }
            if (showActionLinkAsDisabled)
            {
                TagBuilder tagBuilder = new TagBuilder("span");
                tagBuilder.InnerHtml = linkText;
                return MvcHtmlString.Create(tagBuilder.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public static bool ActionAuthorized(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            ControllerBase controllerBase = string.IsNullOrEmpty(controllerName) ? htmlHelper.ViewContext.Controller : htmlHelper.GetControllerByName(controllerName);
            ControllerContext controllerContext = new ControllerContext(htmlHelper.ViewContext.RequestContext, controllerBase);
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            if (actionDescriptor == null)
                return false;

            FilterInfo filters = new FilterInfo(FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor));

            AuthorizationContext authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (IAuthorizationFilter authorizationFilter in filters.AuthorizationFilters)
            {
                authorizationFilter.OnAuthorization(authorizationContext);
                if (authorizationContext.Result != null)
                    return false;
            }
            return true;
        }

        public static ControllerBase GetControllerByName(this HtmlHelper htmlHelper, string controllerName)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(htmlHelper.ViewContext.RequestContext, controllerName);
            if (controller == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "The IControllerFactory '{0}' did not return a controller for the name '{1}'.", factory.GetType(), controllerName));
            }
            return (ControllerBase)controller;
        }

        #endregion
    }
}