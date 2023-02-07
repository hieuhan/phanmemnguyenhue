using System;
using System.Configuration;

namespace phanmemnguyenhue.helper
{
    public class ConstantHelper
    {
        public static string WebsiteTitle = ConfigurationManager.AppSettings["WebsiteTitle"] ?? "";
        public static string WebsiteDescription = ConfigurationManager.AppSettings["WebsiteDescription"] ?? "";
        public static string WebsiteKeywords = ConfigurationManager.AppSettings["WebsiteKeywords"] ?? "";
        public static string WebsiteCanonical = ConfigurationManager.AppSettings["WebsiteCanonical"] ?? "";
        public static string RootPath = ConfigurationManager.AppSettings["RootPath"] ?? "/";
        public static string CommonConstr = ConfigurationManager.AppSettings["CommonConstr"] ?? string.Empty;
        public static byte ActionStatusIdActivated = Convert.ToByte(ConfigurationManager.AppSettings["ActionStatusIdActivated"] ?? "1");
        public static byte ActionStatusIdUnapproved = Convert.ToByte(ConfigurationManager.AppSettings["ActionStatusIdUnapproved"] ?? "3");
        public static string ActionStatusSuccess = ConfigurationManager.AppSettings["ActionStatusSuccess"] ?? "OK";
        public static byte UserStatusIdApproved = Convert.ToByte(ConfigurationManager.AppSettings["ActionStatusSuccess"] ?? "1");
        public static byte UserStatusIdUnapproved = Convert.ToByte(ConfigurationManager.AppSettings["UserStatusIdUnapproved"] ?? "2");
        public static byte ReviewStatusIdUnApproved = Convert.ToByte(ConfigurationManager.AppSettings["ReviewStatusIdUnApproved"] ?? "1");
        public static byte ReviewStatusIdApproved = Convert.ToByte(ConfigurationManager.AppSettings["ReviewStatusIdApproved"] ?? "2");
        public static string FileAllowedUpload = ConfigurationManager.AppSettings["FileAllowedUpload"] ?? "jpeg,jpg,png,gif";
        public static int MediaWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MediaWidth"] ?? "0");
        public static int MediaHeight = Convert.ToInt32(ConfigurationManager.AppSettings["MediaHeight"] ?? "0");
        public static int MediaMobileWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MediaMobileWidth"] ?? "0");
        public static int MediaMobileHeight = Convert.ToInt32(ConfigurationManager.AppSettings["MediaMobileHeight"] ?? "0");
        public static int MediaThumnailWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MediaThumnailWidth"] ?? "0");
        public static int MediaThumnailHeight = Convert.ToInt32(ConfigurationManager.AppSettings["MediaThumnailHeight"] ?? "0");
        public static int MediaIconWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MediaIconWidth"] ?? "0");
        public static int MediaIconHeight = Convert.ToInt32(ConfigurationManager.AppSettings["MediaIconHeight"] ?? "0");
        public static string MediaPath = ConfigurationManager.AppSettings["MediaPath"] ?? "Uploaded/";
        public static string NoImageUrl = ConfigurationManager.AppSettings["NoImageUrl"] ?? "~/assets/images/no-image.png";
        public static int SiteId = Convert.ToInt32(ConfigurationManager.AppSettings["SiteId"] ?? "1000");
        public static int SaleActionTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["SaleActionTypeId"] ?? "1000");
        public static int RentActionTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["RentActionTypeId"] ?? "1001");
    }
}
