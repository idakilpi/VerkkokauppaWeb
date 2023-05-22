using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
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
        string asiakasEmail;
        public TuotteetController() 
        {
            objVerkkokauppaDBEntities = new VerkkokauppaDBEntities();
            listOfOstoskoriModels = new List<Ostoskori>();
        }

        public ActionResult Index(string searchBy, string searchValue)
        {
            var viewModel = new TuotteetAndLoginsViewModel();

            try
            {
                var tuotteet = db.Tuotteet.Include(t => t.Kategoriat).ToList();

                if (tuotteet.Count == 0)
                {
                    TempData["InfoMessage"] = "Tuotetta ei ole saatavilla.";
                }
                else
                {
                    if (string.IsNullOrEmpty(searchValue))
                    {
                        TempData["InfoMessage"] = "Kirjoita hakusana.";
                        viewModel.Tuotteet = tuotteet;
                    }
                    else
                    {
                        if (searchBy.ToLower() == "productname")
                        {
                            var searchByProductName = tuotteet.Where(p => p.TuoteNimi.ToLower().Contains(searchValue.ToLower())).ToList();
                            viewModel.Tuotteet = searchByProductName;
                        }
                        else if (searchBy.ToLower() == "price")
                        {
                            var searchByProductPrice = tuotteet.Where(p => p.Hinta == decimal.Parse(searchValue.ToLower())).ToList();
                            viewModel.Tuotteet = searchByProductPrice;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Virheilmoitus."] = ex.Message;
                return View();
            }

            var login = new Logins();
            viewModel.Logins = login;

            return View(viewModel);

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
            if (listOfOstoskoriModels.Any(model => model.TuoteID == tuoteid))
            {
                objOstoskoriModel = listOfOstoskoriModels.Single(model => model.TuoteID == tuoteid);
                objOstoskoriModel.Määrä = objOstoskoriModel.Määrä + 1;
                objOstoskoriModel.Summa = ((decimal)objOstoskoriModel.Määrä * objOstoskoriModel.Kappalehinta);
            }
            else
            {
                objOstoskoriModel.TuoteID = tuoteid;
                objOstoskoriModel.Kuva = objtuote.Kuva;
                objOstoskoriModel.TuoteNimi = objtuote.TuoteNimi;
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
            List<Asiakkaat> asiakkaat = new List<Asiakkaat>();
            asiakasEmail = Session["Username"] as string;
            Asiakkaat Asiakas = objVerkkokauppaDBEntities.Asiakkaat.Single(model => model.Email == asiakasEmail);
            listOfOstoskoriModels = Session["OstoskoriTuote"] as List<Ostoskori>;

            var firstItem = listOfOstoskoriModels.FirstOrDefault();

            firstItem.AsiakasID = Asiakas.AsiakasID;
            firstItem.Etunimi = Asiakas.Etunimi;
            firstItem.Sukunimi = Asiakas.Sukunimi;
            firstItem.Email = Asiakas.Email;

            listOfOstoskoriModels[0] = firstItem;
            //foreach(var item in listOfOstoskoriModels)
            //{
            //    item.Etunimi= Asiakas.Etunimi;
            //    item.Sukunimi = Asiakas.Sukunimi;
            //    item.Email = Asiakas.Email;
            //}
            return View(listOfOstoskoriModels);
        }

        [HttpPost]
        public ActionResult LisaaTilaus([Bind(Include = "ToimitusOsoite,ToimitusPostinumero")] Ostoskori firstItem)
        {
            string toimitusosoite = firstItem.ToimitusOsoite;
            string postinumero = firstItem.ToimitusPostinumero;
            int TilausID = 0;
            listOfOstoskoriModels = Session["OstoskoriTuote"] as List<Ostoskori>;
            string etunimi = listOfOstoskoriModels[0].Etunimi;
            string sukunimi = listOfOstoskoriModels[0].Sukunimi;
            Tilaukset tilausObj = new Tilaukset()
            {
                AsiakasID = listOfOstoskoriModels[0].AsiakasID,
                TilausPvm = DateTime.Now,
                LähetysPvm = DateTime.Now.AddDays(3),
                ToimitusOsoite = firstItem.ToimitusOsoite,
                ToimitusPostinumero = firstItem.ToimitusPostinumero
            };

            objVerkkokauppaDBEntities.Tilaukset.Add(tilausObj);
            objVerkkokauppaDBEntities.SaveChanges();

            TilausID = tilausObj.TilausID;

            foreach(var tuot in listOfOstoskoriModels)
            {
                Tilausrivit tilausrivitObj = new Tilausrivit();
                tilausrivitObj.KappaleHinta = tuot.Kappalehinta;
                tilausrivitObj.TuoteID = tuot.TuoteID;
                tilausrivitObj.TilausID = TilausID;
                tilausrivitObj.Maara = (int)tuot.Määrä;
                objVerkkokauppaDBEntities.Tilausrivit.Add(tilausrivitObj);
                objVerkkokauppaDBEntities.SaveChanges();
            }
            asiakasEmail = Session["Username"] as string;
            decimal? totalPrice = listOfOstoskoriModels.Sum(item => item.Summa);

            string cartString = "";

            foreach (var item in listOfOstoskoriModels)
            {
                cartString += $"      {item.TuoteID}      {item.TuoteNimi} {item.Määrä} kpl = {item.Summa}€\n";
            }
            cartString += $"Yhteensä: {totalPrice}€";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("jetiside@outlook.com");
            mail.To.Add(asiakasEmail);
            mail.Subject = "Tilausvahvistus Jetiside, Tilausnumero: " + TilausID;
            mail.Body = "Hei!\nKiitos tilauksestasi!\nTilaamasi tuotteet toimitetaan n.3-7 arkipäivän kuluttua.\n\n" +
                "Tilausnumero: " + TilausID + "\nTuoteluettelo:\n| Tuote ID | Nimi | Määrä | Hinta |\n" + cartString +
                "Tilaamasi tuotteet toimitetaan osoitteeseen:\n" + etunimi +" "+ sukunimi + "\n" + toimitusosoite + "\n" + postinumero + "\n\nTervetuloa asioimaan uudelleen!";
                

            SmtpClient smtp = new SmtpClient("smtp.office365.com");
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("jetiside@outlook.com", "Salasana1");
            smtp.EnableSsl = true;

            smtp.Send(mail);


            Session["OstoskoriTuote"] = null;
            Session["OstoskoriCounter"] = null;
            return RedirectToAction("Index");
        }

        // GET: Tuotteet/Create
        public ActionResult Create()
        {
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "KategoriaNimi");
            return View();
        }

        // POST: Tuotteet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuoteID,KategoriaID,TuoteNimi,Hinta,Varastomaara,Kuvaus,Kuva")] Tuotteet tuotteet, HttpPostedFileBase ImageFile)
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

            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "KategoriaNimi", tuotteet.KategoriaID);
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
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "KategoriaNimi", tuotteet.KategoriaID);
            return View(tuotteet);
        }

        // POST: Tuotteet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TuoteID,KategoriaID,TuoteNimi,Hinta,Varastomaara,Kuvaus,KuvaPolku,Kuva,kuvitus")] HttpPostedFileBase kuvitus, Tuotteet tuotteet)
        {
            if (ModelState.IsValid)
            {

                int count = Request.Files.Count;
                var file = Request.Files[0];
                string fileName = file.FileName;
                byte[] buffer = new byte[file.InputStream.Length];
                file.InputStream.Read(buffer, 0, (int)file.InputStream.Length);
                db.Entry(tuotteet).State = EntityState.Modified;
                tuotteet.Kuva = buffer;
                tuotteet.KuvaPolku = fileName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KategoriaID = new SelectList(db.Kategoriat, "KategoriaID", "KategoriaNimi", tuotteet.KategoriaID);
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
        public ActionResult ProductDetails(int id)
        {
            var product = db.Tuotteet.FirstOrDefault(p => p.TuoteID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var model = new TuotteetAndLoginsViewModel()
            {
                TuoteID = product.TuoteID,
                TuoteNimi = product.TuoteNimi,
                Hinta = product.Hinta,
                Kuvaus = product.Kuvaus,
                KuvaPolku = product.KuvaPolku,
                Kuva = product.Kuva
            };

            return PartialView("_ProductDetails", model);
        }
    }
}
