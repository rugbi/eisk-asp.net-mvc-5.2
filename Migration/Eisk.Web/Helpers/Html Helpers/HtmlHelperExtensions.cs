﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Eisk;
using Eisk.Models;

public static class HtmlHelperExtensions
{
    public static IHtmlString RenderMessages(this HtmlHelper htmlHelper)
    {
        var messages = string.Empty;
        foreach (var messageType in Enum.GetNames(typeof (MessageType)))
        {
            var message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
                ? htmlHelper.ViewContext.ViewData[messageType]
                : htmlHelper.ViewContext.TempData.ContainsKey(messageType)
                    ? htmlHelper.ViewContext.TempData[messageType]
                    : null;
            if (message != null)
            {
                MessageType messageTypeEnum;
                Enum.TryParse(messageType, out messageTypeEnum);
                var messageViewModel = new MessageViewModel
                {
                    MessageType = messageTypeEnum,
                    Message = message.ToString()
                };
                messages += RenderRazorViewToString(htmlHelper.ViewContext, "_Message", messageViewModel);
            }
        }

        return MvcHtmlString.Create(messages);
    }

    public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
    {
        controllerContext.Controller.ViewData.Model = model;

        using (var sw = new StringWriter())
        {
            var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
            var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData,
                controllerContext.Controller.TempData, sw);
            viewResult.View.Render(viewContext, sw);
            viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
            return sw.GetStringBuilder().ToString();
        }
    }

    public static IHtmlString RequiredSymbol(this HtmlHelper htmlHelper, string symbol = "* ")
    {
        ModelMetadata modelMetadata = ModelMetadata.FromStringExpression("", htmlHelper.ViewData);

        if (modelMetadata.IsRequired)
        {
            return MvcHtmlString.Create(symbol);
        }

        return MvcHtmlString.Create(string.Empty);
    }

}