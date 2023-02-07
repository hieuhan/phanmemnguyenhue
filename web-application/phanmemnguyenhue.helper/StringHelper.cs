using System.Collections.Specialized;
using System.Web;

namespace phanmemnguyenhue.helper
{
    public static class StringHelper
    {
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return str;
        }

        public static string ActionStatusBootstrap(this byte statusId)
        {
            string resultVar = "bg-warning";
            if (statusId == ConstantHelper.ActionStatusIdActivated)
                resultVar = "bg-success";
            else if (statusId == ConstantHelper.ActionStatusIdUnapproved)
                resultVar = "bg-danger";
            return resultVar;
        }

        public static string UserStatusBootstrap(this byte statusId)
        {
            string retVal = "bg-warning";
            if (statusId == ConstantHelper.UserStatusIdApproved)
                retVal = "bg-success";
            else if (statusId == ConstantHelper.UserStatusIdUnapproved)
                retVal = "bg-danger";
            return retVal;
        }

        public static string ReviewStatusBootstrap(this byte statusId)
        {
            string resultVar = "bg-danger";
            if (statusId == ConstantHelper.ReviewStatusIdApproved)
                resultVar = "bg-success";
            else if (statusId == ConstantHelper.ReviewStatusIdUnApproved)
                resultVar = "bg-warning";
            return resultVar;
        }

        public static string GetUrl(this string url)
        {
            string resultVar = string.Empty;
            if (!string.IsNullOrEmpty(url))
            {
                resultVar = url;
                if (resultVar.Equals("javascript:void(0)") || resultVar.Equals("#"))
                {
                    return resultVar;
                }
                if (!resultVar.Contains("://"))
                {
                    while (resultVar.StartsWith("/"))
                    {
                        resultVar = resultVar.Substring(1);
                    }
                    resultVar = string.Concat(ConstantHelper.RootPath, resultVar);
                }
            }

            return resultVar;
        }

        public static string GetImageUrl(this string filePath)
        {
            string retVal = filePath;

            if (string.IsNullOrEmpty(retVal))
            {
                return ConstantHelper.NoImageUrl;
            }

            if (!retVal.Contains("://"))
            {
                while (retVal.StartsWith("/"))
                {
                    retVal = retVal.Substring(1);
                }

                retVal = string.Concat(ConstantHelper.RootPath, retVal);
            }
            return retVal;
        }

        public static string GetImageUrl_Mobile(this string filePath)
        {
            return filePath.GetImageUrl().Replace("/Original/", "/Mobile/");
        }

        public static string GetImageUrl_Icon(this string filePath)
        {
            return filePath.GetImageUrl().Replace("/Original/", "/Icon/");
        }
        public static string StripTags(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                char[] arr = new char[value.Length];
                int arrIndex = 0;
                bool check = false;
                foreach (var item in value)
                {
                    if (item == '<')
                    {
                        check = true;
                        continue;
                    }
                    if (item == '>')
                    {
                        check = false;
                        continue;
                    }
                    if (!check)
                    {
                        arr[arrIndex] = item;
                        arrIndex++;
                    }
                }
                return new string(arr, 0, arrIndex);
            }
            return value;
        }

        public static string DefaultIfEmpty(this string str, string defaultValue = "", bool considerWhiteSpaceIsEmpty = false)
        {
            return (considerWhiteSpaceIsEmpty ? string.IsNullOrWhiteSpace(str) : string.IsNullOrEmpty(str)) ? defaultValue : str;
        }

        public static string BuildQueryStringUrl(this string url, NameValueCollection parameters)
        {
            string urlWithoutQuery = url.IndexOf('?') >= 0
                ? url.Substring(0, url.IndexOf('?'))
                : url;

            string queryString = url.IndexOf('?') >= 0
                ? url.Substring(url.IndexOf('?'))
                : null;

            var queryParamList = queryString != null
                ? HttpUtility.ParseQueryString(queryString)
                : HttpUtility.ParseQueryString(string.Empty);

            foreach (string key in parameters.AllKeys)
            {
                string value = parameters[key] ?? string.Empty;
                if (queryParamList[key] != null)
                {
                    if (value == "")
                        queryParamList.Remove(key);
                    else queryParamList[key] = value;
                }
                else
                {
                    if (value == "")
                        queryParamList.Remove(key);
                    else queryParamList.Add(key, value);
                }
            }

            return $"{urlWithoutQuery}{(queryParamList.Count > 0 ? "?" : string.Empty)}{queryParamList}";
        }

        public static string GetTitle(this string value)
        {
            return value.DefaultIfEmpty().Replace("\"", "'");
        }
    }
}
