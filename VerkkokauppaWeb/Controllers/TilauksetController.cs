using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VerkkokauppaWeb.Models;

namespace VerkkokauppaWeb.Controllers
{
    public class TilauksetController : Controller
    {
        private VerkkokauppaDBEntities db = new VerkkokauppaDBEntities();

        const int PageSize = 10;

        public ActionResult Index(int page = 1)
        {
            var tilaukset = db.Tilaukset.Include(t => t.Asiakkaat).Include(t => t.Postitoimipaikat);

            if (Session["UserName"] as string != "teppo@jetiside.fi")
            {
                tilaukset = tilaukset.Where(t => t.Asiakkaat.Email == Session["UserName"] as string);
                return RedirectToAction("Index", "Home");
            }

            var pagedTilaukset = tilaukset.OrderBy(t => t.TilausPvm)
            .Skip((page - 1) * PageSize)
                                           .Take(PageSize)
                                           .ToList();

            var totalItems = tilaukset.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedTilaukset);
        }

        public ActionResult Create()
        {
            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Etunimi");
            ViewBag.ToimitusPostinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka");
            return View();
        }

        // POST: Tilaukset/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TilausID,AsiakasID,TilausPvm,LähetysPvm,ToimitusOsoite,ToimitusPostinumero")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Tilaukset.Add(tilaukset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Etunimi", tilaukset.AsiakasID);
            ViewBag.ToimitusPostinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", tilaukset.ToimitusPostinumero);
            return View(tilaukset);
        }

        // GET: Tilaukset/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }
            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Etunimi", tilaukset.AsiakasID);
            ViewBag.ToimitusPostinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", tilaukset.ToimitusPostinumero);
            return View(tilaukset);
        }

        // POST: Tilaukset/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TilausID,AsiakasID,TilausPvm,LähetysPvm,ToimitusOsoite,ToimitusPostinumero")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tilaukset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Etunimi", tilaukset.AsiakasID);
            ViewBag.ToimitusPostinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", tilaukset.ToimitusPostinumero);
            return View(tilaukset);
        }

        // GET: Tilaukset/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }
            return View(tilaukset);
        }

        // POST: Tilaukset/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            db.Tilaukset.Remove(tilaukset);
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
        public ActionResult OrderLines(int id)
        {
            var order = db.Tilaukset.Include("Tilausrivit.Tuotteet").SingleOrDefault(x => x.TilausID == id);
            return PartialView("_OrderLines", order);
        }
    }
}
