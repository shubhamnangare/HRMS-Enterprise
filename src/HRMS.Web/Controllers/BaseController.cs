using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HRMS.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected void AddSuccessMessage(string message)
        {
            TempData["Success"] = message;
        }

        protected void AddErrorMessage(string message)
        {
            TempData["Error"] = message;
        }

        protected void AddWarningMessage(string message)
        {
            TempData["Warning"] = message;
        }

        protected void AddInfoMessage(string message)
        {
            TempData["Info"] = message;
        }

        protected IActionResult HandleException(Exception ex, string customMessage = null)
        {
            // Log the exception here
            var message = customMessage ?? "An error occurred while processing your request.";
            AddErrorMessage(message);

#if DEBUG
            // In development, show the actual error
            AddErrorMessage($"Details: {ex.Message}");
#endif

            return RedirectToAction("Index", "Home");
        }
    }
}