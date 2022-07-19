using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using AllinaHealth.Models.Extensions;
using AllinaHealth.Models.ViewModels;
using Sitecore;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Web.Controllers
{
    public class FormsController : Controller
    {
        [HttpGet]
        public ActionResult EmailUsForm()
        {
            return View("~/Views/Forms/EmailUsForm.cshtml", new EmailUsFormModel());
        }

        [HttpPost]
        public ActionResult EmailUsForm(EmailUsFormModel model)
        {
            model.IsSuccess = ModelState.IsValid;

            if (!model.IsSuccess)
            {
                return View("~/Views/Forms/EmailUsForm.cshtml", model);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("<p>An Email Us form has been submitted.<br/><br/>Here are the details:<br/><br/>" +
                            "Name: {0}<br/>" +
                            "Email: {1}<br/>" +
                            "Phone: {2}<br/>" +
                            "Question: {3}<br/>" +
                            "</p>",
                model.Name,
                model.Email,
                model.Phone,
                model.Question);

            var emailMessage = new MailMessage("allina.webmaster@allina.com", RenderingContext.Current.Rendering.Item.GetFieldValue("Email To Address") ?? "kristin.drews@allina.com", RenderingContext.Current.Rendering.Item.GetFieldValue("Email Subject") ?? "Email Us Form Submission", sb.ToString());
            emailMessage.IsBodyHtml = true;
            MainUtil.SendMail(emailMessage);

            return View("~/Views/Forms/EmailUsForm.cshtml", model);
        }

        public ActionResult AccountSignin()
        {
            return View("~/Views/Forms/AccountSignIn.cshtml");
        }
    }
}