using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VerkkokauppaWeb.Models;
using VerkkokauppaWeb.ViewModels;


namespace VerkkokauppaWeb.Controllers
{
    public class AsiakkaatController : Controller
    {
        private VerkkokauppaDBEntities db = new VerkkokauppaDBEntities();

        // GET: Asiakkaat
        public ActionResult Index()
        {
            return View();
            //return View(db.Asiakkaat.ToList());
        }

        // GET: Asiakkaat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            return View(asiakkaat);
        }


        
        // GET: Asiakkaat/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Asiakkaat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
	public ActionResult Create([Bind(Include = "AsiakasID,Etunimi,Sukunimi,Email,Salasana,VahvistaSalasana")] Register uusiAsiakas)
        {
            if (ModelState.IsValid)
            {
                var varattu = db.Asiakkaat.Any(x => x.Email == uusiAsiakas.Email);
                if (varattu)
                {
                    ModelState.AddModelError("Email", "Sähköpostiosoite on jo käytössä.");
                    return View(uusiAsiakas);
                }
                

                var asiakas = new Asiakkaat {
                Etunimi = uusiAsiakas.Etunimi,
                Sukunimi = uusiAsiakas.Sukunimi,
                Email = uusiAsiakas.Email,
                };

                var login = new Logins
                {
                    Salasana = uusiAsiakas.Salasana,
                    Kayttajatunnus = uusiAsiakas.Email
                };

                db.Asiakkaat.Add(asiakas);
                db.Logins.Add(login);
                db.SaveChanges();
                ViewBag.Message = string.Format("User {0} Successfully Created", uusiAsiakas.Etunimi);
                return RedirectToAction("Index","Asiakkaat");
                }

            return View(uusiAsiakas);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AdminPage()
        {

            var modelList = from a in db.Asiakkaat
                            join l in db.Logins on a.AsiakasID equals l.AsiakasID
                            select new AsiakkaatAndLoginsViewModel
                            {
                                AsiakasID = a.AsiakasID,
                                Etunimi = a.Etunimi,
                                Sukunimi = a.Sukunimi,
                                Email = a.Email,
                                Salasana = l.Salasana,
                            };

            return View(modelList);
        }

        public ActionResult Editprofile(int id, string view)
        {
            var asiakas = (from a in db.Asiakkaat
                           join l in db.Logins on a.AsiakasID equals l.AsiakasID
                           where l.AsiakasID == id
                           select new AsiakkaatAndLoginsViewModel
                           {
                               AsiakasID = a.AsiakasID,
                               Etunimi = a.Etunimi,
                               Sukunimi = a.Sukunimi,
                               Email = a.Email,
                               Salasana = l.Salasana,
                           }).SingleOrDefault();
                return View(asiakas);
            //return View(asiakas);
        }
        [HttpPost]
        public ActionResult EditProfile(int? id, string view,[Bind(Include = "AsiakasID,Etunimi,Sukunimi,Email,Salasana,Kayttajatunnus")] AsiakkaatAndLoginsViewModel päivitäAsiakas) 
        { 
            if (id == null) { 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            } 
            if (ModelState.IsValid) { 
                var asiakas = db.Asiakkaat.Find(id);
                var login = db.Logins.FirstOrDefault(l => l.AsiakasID == id);

                if (asiakas == null || login == null) { 
                    return HttpNotFound(); 
                } 
                asiakas.Etunimi = päivitäAsiakas.Etunimi; 
                asiakas.Sukunimi = päivitäAsiakas.Sukunimi; 
                asiakas.Email = päivitäAsiakas.Email; 
                login.Salasana = päivitäAsiakas.Salasana; 
                login.Kayttajatunnus = päivitäAsiakas.Email; 
                db.Entry(asiakas).State = EntityState.Modified; 
                db.Entry(login).State = EntityState.Modified; 
                db.SaveChanges();
                return RedirectToAction("Index", "Tuotteet");
            }
                return View(päivitäAsiakas); 
        }
        public ActionResult Delete(int? id)
        {
            var asiakas = (from a in db.Asiakkaat
                           join l in db.Logins on a.AsiakasID equals l.AsiakasID
                           where l.AsiakasID == id
                           select new AsiakkaatAndLoginsViewModel
                           {
                               AsiakasID = a.AsiakasID,
                               Etunimi = a.Etunimi,
                               Sukunimi = a.Sukunimi,
                               Email = a.Email,
                               Salasana = l.Salasana,
                           }).SingleOrDefault();
            return View(asiakas);
        }

        // POST: Asiakkaat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var asiakas = db.Asiakkaat.Find(id);
            var login = db.Logins.FirstOrDefault(l => l.AsiakasID == id);

            db.Asiakkaat.Remove(asiakas);
            db.Logins.Remove(login);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
