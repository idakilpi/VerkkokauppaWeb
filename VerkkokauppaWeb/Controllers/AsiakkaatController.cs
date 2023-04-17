using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            //var asiakkaat = db.Asiakkaat.ToList();
            //var login = db.Logins.ToList();
            //var viewModel = new AsiakkaatAndLoginsViewModel
            //{
            //    Asiakkaat = asiakkaat,
            //    Logins = login
            //};
            //return View(viewModel);

            var modelList = from a in db.Asiakkaat
                            join l in db.Logins on a.AsiakasID equals l.AsiakasID
                            select new AsiakkaatAndLoginsViewModel
                            {
                                AsiakasID = a.AsiakasID,
                                Nimi = a.Nimi,
                                Katuosoite = a.Katuosoite,
                                Postinumero = a.Postinumero,
                                Postitoimipaikka = a.Postitoimipaikka,
                                Email = a.Email,
                                Puhelinnumero = a.Puhelinnumero,
                                Salasana = l.Salasana,
                                Tunnus = l.Email
                            };

            return View(modelList);
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
        public ActionResult Create([Bind(Include = "AsiakasID,Nimi,Katuosoite,Postinumero,Postitoimipaikka,Email,Puhelinnumero,Salasana,Tunnus")] AsiakkaatAndLoginsViewModel uusiAsiakas)
        {
            if (ModelState.IsValid)
            {
                var asiakas = new Asiakkaat
                {
                    Nimi = uusiAsiakas.Nimi,
                    Katuosoite = uusiAsiakas.Katuosoite,
                    Postinumero = uusiAsiakas.Postinumero,
                    Postitoimipaikka = uusiAsiakas.Postitoimipaikka,
                    Email = uusiAsiakas.Email,
                    Puhelinnumero = uusiAsiakas.Puhelinnumero
                };

                var login = new Logins
                {
                    Salasana = uusiAsiakas.Salasana,
                    Email = uusiAsiakas.Tunnus
                };

                db.Asiakkaat.Add(asiakas);
                db.Logins.Add(login);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uusiAsiakas);
        }

        // GET: Asiakkaat/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Asiakkaat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsiakasID,Nimi,Katuosoite,Postinumero,Postitoimipaikka,Email,Puhelinnumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakkaat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asiakkaat);
        }

        // GET: Asiakkaat/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Asiakkaat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            db.Asiakkaat.Remove(asiakkaat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
