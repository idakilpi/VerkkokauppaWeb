using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using VerkkokauppaWeb.Models;
using VerkkokauppaWeb.ViewModels;

namespace VerkkokauppaWeb.Controllers
{
    public class TuotteetController : Controller
    {
        private VerkkokauppaDBEntities db = new VerkkokauppaDBEntities();

        private VerkkokauppaDBEntities objVerkkokauppaDBEntities;
            List<Ostoskori> listOfOstoskoriModels;
        public TuotteetController() 
        {
            objVerkkokauppaDBEntities = new VerkkokauppaDBEntities();
            listOfOstoskoriModels = new List<Ostoskori>();
        }

        // GET: Tuotteet
        public ActionResult Index()
        {
            var tuotteet = db.Tuotteet.Include(t => t.Kategoriat).ToList();
            var login = new Logins();
            var viewModel = new TuotteetAndLoginsViewModel
            {
                Tuotteet = tuotteet,
                Logins = login
            };
            return View(viewModel);

            //var tuotteet = db.Tuotteet.Include(t => t.Kategoriat);
            //return View(tuotteet.ToList());
        }
        [HttpPost]
        public JsonResult Index(int tuoteid) 
        {
            
            Ostoskori objOstoskoriModel = new Ostoskori();
            Tuotteet objtuote = objVerkkokauppaDBEntities.Tuotteet.Single(model => model.TuoteID == tuoteid);
            if (Session["OstoskoriCounter"] != null)
            {
                listOfOstoskoriModels = Session["OstoskoriTuote"] as List<Ostoskori>;
            }
            if(listOfOstoskoriModels.Any(model => model.TuoteID == tuoteid))
            {
                objOstoskoriModel = listOfOstoskoriModels.Single(model => model.TuoteID == tuoteid);
                objOstoskoriModel.Määrä = objOstoskoriModel.Määrä + 1;
                objOstoskoriModel.Summa = ((decimal)objOstoskoriModel.Määrä * objOstoskoriModel.Kappalehinta);
            }
            else
            {
                objOstoskoriModel.TuoteID = tuoteid;
                objOstoskoriModel.Kuva = objtuote.Kuva;
                objOstoskoriModel.Nimi = objtuote.Nimi;
                objOstoskoriModel.Määrä = 1;
                objOstoskoriModel.Summa = objtuote.Hinta;
                objOstoskoriModel.Kappalehinta = objtuote.Hinta;
                listOfOstoskoriModels.Add(objOstoskoriModel);
            }

            Session["OstoskoriCounter"] = listOfOstoskoriModels.Count;
            Session["OstoskoriTuote"] = listOfOstoskoriModels;

            return Json(new {Success = true, Counter = listOfOstoskoriModels.Count}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Ostoskori()
        {
            listOfOstoskoriModels = Session["OstoskoriTuote"] as List<Ostoskori>;
            return View(listOfOstoskoriModels);
        }
        [HttpPost]
        public ActionResult LisaaTilaus()
        {
            int TilausID = 0;
            listOfOstoskoriModels = Session["OstoskoriTuote"] as List<Ostoskori>;
            Tilaukset tilausObj = new Tilaukset()
            {
                AsiakasID = 1,
                TilausPvm = DateTime.Now,
                LahetysPvm = DateTime.Now.AddDays(7),

            };

            objVerkkokauppaDBEntities.Tilaukset.Add(tilausObj);
            objVerkkokauppaDBEntities.SaveChanges();
            TilausID = tilausObj.TilausID;

            foreach(var tuot in listOfOstoskoriModels)
            {
                Tilausrivit tilausrivitObj = new Tilausrivit();
                tilausrivitObj.Hinta = tuot.Kappalehinta;
                tilausrivitObj.TuoteID = tuot.TuoteID;
                tilausrivitObj.TilausID = TilausID;
                tilausrivitObj.Maara = tuot.Määrä;
                objVerkkokauppaDBEntities.Tilausrivit.Add(tilausrivitObj);
                objVerkkokauppaDBEntities.SaveChanges();
            }
            Session["OstoskoriTuote"] = null;
            Session["OstoskoriCounter"] = null;
            return RedirectToAction("Index");
        }

        // GET: Tuotteet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            return View(tuotteet);
        }

        // GET: Tuotteet/Create
        public ActionResult Create()
        {
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "Nimi");
            return View();
        }

        // POST: Tuotteet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuoteID,KategoriaID,Nimi,Hinta,Varastomaara,Kuvaus,Kuva")] Tuotteet tuotteet, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                byte[] tuotekuva = new byte[ImageFile.ContentLength];
                ImageFile.InputStream.Read(tuotekuva, 0, tuotekuva.Length);
                tuotteet.Kuva = tuotekuva;
                db.Tuotteet.Add(tuotteet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "Nimi", tuotteet.KategoriaID);
            return View(tuotteet);
        }

        // GET: Tuotteet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "Nimi", tuotteet.KategoriaID);
            return View(tuotteet);
        }

        // POST: Tuotteet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TuoteID,KategoriaID,Nimi,Hinta,Varastomaara,Kuvaus,KuvaPolku,Kuva,kuvitus")] HttpPostedFileBase kuvitus, Tuotteet tuotteet)
        {
            if (ModelState.IsValid)
            {

                int count = Request.Files.Count;
                var file = Request.Files[0];
                string fileName = file.FileName;
                byte[] buffer =new byte[file.InputStream.Length];
                file.InputStream.Read(buffer, 0, (int)file.InputStream.Length);
                db.Entry(tuotteet).State = EntityState.Modified;
                tuotteet.Kuva = buffer;
                tuotteet.KuvaPolku = fileName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "Nimi", tuotteet.KategoriaID);
            return View(tuotteet);
        }

        // GET: Tuotteet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            return View(tuotteet);
        }

        // POST: Tuotteet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            db.Tuotteet.Remove(tuotteet);
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
