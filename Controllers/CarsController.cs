using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using theking.Models;

namespace theking.Controllers
{
    public class CarsController : Controller
    {
        private CarRentalDBEntities db = new CarRentalDBEntities();

        // GET: Cars

        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.ClassSortParm = String.IsNullOrEmpty(sortOrder) ? "class_desc" : "";
            ViewBag.TypeSortParm = sortOrder == "type" ? "type_desc" : "type";
            ViewBag.MakeSortParm = sortOrder == "make" ? "make_desc" : "make";
            ViewBag.ModelSortParm = sortOrder == "model" ? "model_desc" : "model";
            ViewBag.ColorSortParm = sortOrder == "color" ? "color_desc" : "color";
            ViewBag.DoorsSortParm = sortOrder == "doors" ? "doors_desc" : "doors";
            var cars = from c in db.Cars
                           select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Make.Contains(searchString)
                                       || c.Model.Contains(searchString)
                                       || c.Color.Contains(searchString)
                                       || c.CarType.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "class_desc":
                    cars = cars.OrderByDescending(c => c.Class);
                    break;
                case "type":
                    cars = cars.OrderBy(c => c.CarType);
                    break;
                case "type_desc":
                    cars = cars.OrderByDescending(c => c.CarType);
                    break;
                case "make":
                    cars = cars.OrderBy(c => c.Make);
                    break;
                case "make_desc":
                    cars = cars.OrderByDescending(c => c.Make);
                    break;
                case "model":
                    cars = cars.OrderBy(c => c.Model);
                    break;
                case "model_desc":
                    cars = cars.OrderByDescending(c => c.Model);
                    break;
                case "color":
                    cars = cars.OrderBy(c => c.Color);
                    break;
                case "color_desc":
                    cars = cars.OrderByDescending(c => c.Color);
                    break;
                case "doors":
                    cars = cars.OrderBy(c => c.Doors);
                    break;
                case "doors_desc":
                    cars = cars.OrderByDescending(c => c.Doors);
                    break;

                default:
                    cars = cars.OrderBy(c => c.Class);
                    break;
            }
            return View(cars.ToList());
        }
     

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            List<string> avaialableClasses = new List<string>();
            avaialableClasses = (from clas in db.Prices
                                 select clas.Class).ToList();
            ViewBag.availableClasses = avaialableClasses;
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Class,CarType,Make,Model,Color,Doors,KMs,LicensePlate,DateOnRoad,NextServiceDate,NextServiceKMS,StatusInService,StatusRented,CustID,Notes")] Car car)
        {
            if (ModelState.IsValid)
            {
                car.StatusInService = true;
                car.StatusRented = false;
                db.Cars.Add(car);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Class,CarType,Make,Model,Color,Doors,KMs,LicensePlate,DateOnRoad,NextServiceDate,NextServiceKMS,StatusInService,StatusRented,CustID,Notes")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        // GET: Cars/service/5
        public ActionResult Service(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            car.StatusInService = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Cars/BackFromSerivce/5
        public ActionResult BackFromService(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/BackFromService/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BackFromService([Bind(Include = "id,Class,CarType,Make,Model,Color,Doors,KMs,LicensePlate,DateOnRoad,NextServiceDate,NextServiceKMS,StatusInService,StatusRented,CustID,Notes")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                car.StatusInService = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }
        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
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
