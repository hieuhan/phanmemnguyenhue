using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace phanmemnguyenhue.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowFileExtensionsAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _fileExtensions;
        private IEnumerable<string> AllowedExtensions { get; set; }

        public AllowFileExtensionsAttribute(string fileExtensions)
        {
            _fileExtensions = fileExtensions;
            AllowedExtensions = fileExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;

            if (file != null)
            {
                var fileName = file.FileName;

                return AllowedExtensions.Any(y => fileName.EndsWith(y));
            }

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "allowfileextensions"
            };

            clientValidationRule.ValidationParameters.Add("fileextensions", _fileExtensions);

            return new[] { clientValidationRule };
        }
    }
}