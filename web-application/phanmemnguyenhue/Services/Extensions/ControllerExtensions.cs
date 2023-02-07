using phanmemnguyenhue.Services.ToastrNotifications;
using System.Web.Mvc;

namespace phanmemnguyenhue.Services.Extensions
{
    public static class ControllerExtensions
    {
        public static void ToastrSuccess(this Controller controller, string title, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Success, title, message)
            };
        }

        public static void ToastrSuccess(this Controller controller, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Success, message)
            };
        }

        public static void ToastrError(this Controller controller, string title, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Error, title, message)
            };
        }

        public static void ToastrError(this Controller controller, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Error, message)
            };
        }

        public static void ToastrInfo(this Controller controller, string title, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Info, title, message)
            };
        }

        public static void ToastrInfo(this Controller controller, string message)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(ToastrTypes.Info, message)
            };
        }

        public static void ToastrFunction(this Controller controller, string function, string scripts)
        {
            var messages = new ToastrService(controller.TempData)
            {
                new Toastr(function, scripts)
            };
        }
    }
}