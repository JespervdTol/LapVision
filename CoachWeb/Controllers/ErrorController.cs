using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoachWeb.Controllers
{
    public class ErrorReportController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReportError(string context, string error, string returnUrl)
        {
            TempData["ErrorMessage"] = "Thank you! The issue has been reported.";
            TempData["ShowReportButton"] = false;
            TempData["ErrorContext"] = context;

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}