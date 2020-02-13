using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using theking.Models;
using theking.Models.ViewModels;

namespace theking.Controllers
{
    public class RentalsController : Controller
    {
        private CarRentalDBEntities db = new CarRentalDBEntities();

     

        public ActionResult Index()
        {

            var query = from ren in db.Rentals
                        join cust in db.Customers on ren.CustID  equals cust.id
                        join car in db.Cars on ren.CarID equals car.id  
                        orderby ren.id descending
                        select new RentalsViewModel { Customer = cust, Car = car, Rental = ren };
            return View(query.ToList());
        }
        //GET: Rentals/CustomerRentals/5
        public ActionResult CustomerRentals(int CustID)
        {

            var query = from ren in db.Rentals
                        join cust in db.Customers on ren.CustID equals cust.id
                        join car in db.Cars on ren.CarID equals car.id
                        where ren.CustID == CustID
                        orderby ren.id descending
                        select new RentalsViewModel { Customer = cust, Car = car, Rental = ren };
            return View(query.ToList());
        }


        // GET: Rentals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Rentals/CreateNoParams
        public ActionResult CreateNoParams()
        {
            List<Car> AvailableCars = new List<Car>();
            AvailableCars = (from car in db.Cars
                             where car.StatusRented == false && car.StatusInService == true
                             select car).ToList();
            ViewBag.AvailableCars = AvailableCars;
            List<Customer> CustomersList = new List<Customer>();
            CustomersList = (from cust in db.Customers
                             select cust).ToList();
            ViewBag.ListOfCustomers = CustomersList;
            return View();
        }


        //all four of these posts are exactky the same. is tehre a way to use post method from different gets to same post??
        // POST: Rentals/CreateNoParams
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNoParams([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                rental.Status = true;
                rental.DateOut = DateTime.Now;
                Car car = db.Cars.Find(rental.CarID);
                rental.MileageOut = car.KMs;
                car.CustID = rental.CustID;
                car.StatusRented = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rental);
        }

        // GET: Rentals/CreateFromCustomer
        public ActionResult CreateFromCustomer(int CustID)
        {
            List<Car> AvailableCars = new List<Car>();
            AvailableCars = (from car in db.Cars
                             where car.StatusRented == false && car.StatusInService == true
                             select car).ToList();
            ViewBag.AvailableCars = AvailableCars;
            return View();
        }

      

        // POST: Rentals/CreateFromCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromCustomer([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                rental.Status = true;
                rental.DateOut = DateTime.Now;
                Car car = db.Cars.Find(rental.CarID);
                rental.MileageOut = car.KMs;
                car.CustID = rental.CustID;
                car.StatusRented = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rental);
        }
        // GET: Rentals/CreateFromCar
        public ActionResult CreateFromCar(int CarID)
        {
            List<Customer> CustomersList = new List<Customer>();
            CustomersList = (from cust in db.Customers
                             select cust).ToList();
            ViewBag.ListOfCustomers = CustomersList;
            return View();
        }



        // POST: Rentals/CreateFromCar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromCar([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                rental.Status = true;
                rental.DateOut = DateTime.Now;
                Car car = db.Cars.Find(rental.CarID);
                rental.MileageOut = car.KMs;
                car.CustID = rental.CustID;
                car.StatusRented = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rental);
        }


        // GET: Rentals/CreateFromBooking
        public ActionResult CreateFromBooking(int CustID, string Class, DateTime DateIn, string Notes)
        {
            List<Car> AvailableCars = new List<Car>();
            AvailableCars = (from car in db.Cars
                             where car.StatusRented == false && car.StatusInService == true
                             && car.Class == Class
                             select car).ToList();
            ViewBag.Message = "Please choose a car:";

            if (AvailableCars?.Any() != true)
            {
                AvailableCars = (from car in db.Cars
                                 where car.StatusRented == false && car.StatusInService == true
                                 select car).ToList();
                ViewBag.Message = "No cars available for the class you booked. Please choose another car:";
            }
            if (AvailableCars?.Any() != true)
            {
                ViewBag.Message = "Sorry, No cars available!";
            }
                ViewBag.AvailableCars = AvailableCars;
            ViewBag.DateIn = DateIn.ToString("yyyy-MM-dd");
            return View();
        }



        // POST: Rentals/CreateFromBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromBooking([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                rental.Status = true;
                rental.DateOut = DateTime.Now;
                Car car = db.Cars.Find(rental.CarID);
                rental.MileageOut = car.KMs;
                car.CustID = rental.CustID;
                car.StatusRented = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rental);
        }

        // GET: Rentals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;

                db.SaveChanges();
             
                return RedirectToAction("Index");
            }
            return View(rental);
        }

        // GET: Rentals/Return/5
        public ActionResult Return(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Return([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,MileageOut, MileageIn, Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                rental.Status = false;
                rental.DateIn = DateTime.Now;
               
                var result = db.Cars.SingleOrDefault(b => b.id == rental.CarID);
                if (result != null)
                    {
                     result.StatusRented = false;
                     result.KMs = rental.MileageIn;
                     db.SaveChanges();
                    }
                
                return RedirectToAction("Index");
            }
            return View(rental);
        }
        // GET: Rentals/ReturnFromCar/5
        public ActionResult ReturnFromCar(int CarID)
        {
            var rental = (from rent in db.Rentals
                          where rent.CarID == CarID && rent.Status == true  
                          select rent).FirstOrDefault();
         
            return View(rental);
        }

        // POST: Rentals/ReturnFromCar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnFromCar([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,MileageOut, MileageIn, Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                rental.Status = false;
                rental.DateIn = DateTime.Now;
                db.SaveChanges();

                var result = db.Cars.SingleOrDefault(b => b.id == rental.CarID);
                if (result != null)
                {
                    result.StatusRented = false;
                    result.KMs = rental.MileageIn;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(rental);
        }


        // GET: Rentals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);
            db.Rentals.Remove(rental);
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
