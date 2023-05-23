using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VerkkokauppaWeb.Models;
using VerkkokauppaWeb.ViewModels;

namespace VerkkokauppaWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.LoginError = 0;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Rakkaudesta luontoon ja seikkailuun.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
        [HttpPost]
        public ActionResult Authorize(Logins LoginModel)
        {
            VerkkokauppaDBEntities db = new VerkkokauppaDBEntities();
            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Logins.SingleOrDefault(x => x.Kayttajatunnus == LoginModel.Kayttajatunnus && x.Salasana == LoginModel.Salasana);
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                ViewBag.LoginError = 0; //Ei virhettä
                Session["UserName"] = LoggedUser.Kayttajatunnus;
                Session["UserFirstName"] = LoggedUser.Asiakkaat.Etunimi;
                Session["LoginID"] = LoggedUser.LoginID;
                Session["UserID"] = LoggedUser.AsiakasID;
                ViewBag.LoginError = 0;
                //Session["AccessLevel"] = LoggedUser.AccessLevel;
                return RedirectToAction("Index", "Home"); //Tässä määritellään mihin onnistunut kirjautuminen johtaa --> Home/Index
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                ViewBag.LoginError = 1; //Pakotetaan modaali login-ruutu uudelleen koska kirjautuminen epäonnistui
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Index", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Out";
            return RedirectToAction("Index", "Home"); //Uloskirjautumisen jälkeen pääsivulle
        }


        [HttpPost]
        public ActionResult SendEmail(string nimi, string email, string input)
        {

            if (string.IsNullOrWhiteSpace(nimi) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(input))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return View();
            }

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("jetiside2@outlook.com");
            mail.To.Add(new MailAddress("jetiside@outlook.com"));
            mail.Subject = "Asiakaspalaute sähköpostiosoitteesta " + email ;
            mail.Body = input + "\n\nLähettäjä: " + nimi;

            SmtpClient smtp = new SmtpClient("smtp.office365.com");
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("jetiside@outlook.com", "Salasana1");
            smtp.Credentials = new NetworkCredential("jetiside2@outlook.com", "Salasana1");

            smtp.EnableSsl = true;

            smtp.Send(mail);
            ViewBag.SuccessMessage = "Sähköpostin lähetys onnistui!";
            TempData["AlertMessage"] = "Sähköpostin lähetys onnistui! Vastaamme mahdollisimman pian.";
            return RedirectToAction("Index", "Home");
        }
    }
}
