using phanmemnguyenhue.Services.ToastrNotifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace phanmemnguyenhue.Services.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ToastrNotifications(this HtmlHelper htmlHelper)
        {
            var messages = new ToastrService(htmlHelper.ViewContext.TempData);
            if (messages.Count <= 0)
                return new MvcHtmlString(string.Empty);

            var toastrCalls = new StringBuilder();
            foreach (var message in messages)
            {
                if (string.IsNullOrWhiteSpace(message.Message))
                    continue;
                if (!string.IsNullOrEmpty(message.FunctionName))
                {
                    toastrCalls.AppendLine(string.Format("{0}({1});",
                        message.FunctionName, GetToastrFunctionParameters(message)));
                }
                else
                    toastrCalls.AppendLine(string.Format("toastr.{0}({1});",
                        GetToastrFunctionCall(message.Severity), GetToastrFunctionParameters(message)));
            }

            return new MvcHtmlString(GetScriptBlock(toastrCalls.ToString()));
        }

        public static MvcHtmlString ToastrScripts(this HtmlHelper htmlHelper)
        {
            var messages = new ToastrService(htmlHelper.ViewContext.TempData);
            if (messages.Count <= 0)
                return new MvcHtmlString(string.Empty);

            var toastrCalls = new StringBuilder();
            foreach (var message in messages)
            {
                if (string.IsNullOrWhiteSpace(message.Message))
                    continue;

                toastrCalls.AppendLine(GetToastrFunctionParameters(message).ToString());
            }

            return new MvcHtmlString(GetScriptBlock(toastrCalls.ToString()));
        }

        private static object GetToastrFunctionParameters(Toastr message)
        {
            var parameters = new List<string>
            {
                string.Format("'{0}'", message.Message.Replace("'", "\\'"))
            };

            if (!string.IsNullOrWhiteSpace(message.Title))
                parameters.Add(string.Format("'{0}'", message.Title.Replace("'", "\\'")));

            return string.Join(", ", parameters);
        }

        private static object GetToastrFunctionCall(ToastrTypes severity)
        {
            switch (severity)
            {
                case ToastrTypes.Success:
                    return "success";
                case ToastrTypes.Info:
                    return "info";
                case ToastrTypes.Warning:
                    return "warning";
                case ToastrTypes.Error:
                    return "error";
                default:
                    throw new ArgumentException("Unknown severity value", "severity");
            }
        }

        private static string GetScriptBlock(string toastrCalls)
        {
            return string.Format(@"
                    <script type=""text/javascript"">
                    window.onload = function () {{
                        {0}
                    }};
                    </script>", toastrCalls);
        }
    }
}