using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace phanmemnguyenhue.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowFileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        public int FileSize { get; set; } = 1 * 1024 * 1024 * 1024;

        public override bool IsValid(
           object value)
        {
            if (value is HttpPostedFileBase file)
            {
                return IsValidMaximumFileSize(file.ContentLength);
            }
            return true;
        }

        private bool IsValidMaximumFileSize(int fileSize)
        {
            return (ConvertBytesToMegabytes(fileSize) <= FileSize);
        }

        private double ConvertBytesToMegabytes(
            int bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "allowfilesize"
            };

            clientValidationRule.ValidationParameters.Add("filesize", FileSize);

            return new[] { clientValidationRule };
        }
    }
}