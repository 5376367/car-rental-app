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
            var query = from ren in db.Rentals
                        join cust in db.Customers on ren.CustID equals cust.id
                        join car in db.Cars on ren.CarID equals car.id
                        where ren.id == id
                        orderby ren.id descending
                        select new RentalsViewModel { Customer = cust, Car = car, Rental = ren };
            return View(query.FirstOrDefault());
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

      
        //GET: Rentals/CreateFromCar
        public ActionResult CreateFromCar(int CarID)
        {
            List<Customer> CustomersList = new List<Customer>();
            CustomersList = (from cust in db.Customers
                             select cust).ToList();
            ViewBag.ListOfCustomers = CustomersList;
            return View();
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

        // POST: Rentals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,Notes")] Rental rental)
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
                Price price = (from pri in db.Prices
                               where pri.Class == car.Class
                               select pri).FirstOrDefault();
                //the following line will return the wrong info if car was returned different year to when it was taken
                int daysExpected = (rental.DateIn.Value.DayOfYear - rental.DateOut.Value.DayOfYear) + 1;
                int? freeKms = daysExpected * price.FreeKMs;
                //the following code will only get high season if the whole rental was within high season. this must be fixed
                var season = (from sea in db.Seasons
                              where rental.DateOut >= sea.StartDate
                              where rental.DateIn <= sea.EndDate
                              select sea.High).FirstOrDefault();
                if (season == true)
                {
                    rental.PriceDay = price.HighSeasonDay;
                    rental.PriceKM = price.HighSeasonKM;
                }
                else
                {
                    rental.PriceDay = price.LowSeasonDay;
                    rental.PriceKM = price.LowSeasonKM;

                }
                rental.FreeKMs = freeKms / daysExpected;
                rental.SeasonHigh = season;
                rental.Days = daysExpected;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = rental.id });
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

        //// POST: Rentals/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Return([Bind(Include = "id,CustID,CarID,DateOut,DateIn,Status,MileageOut, MileageIn, Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                rental.Status = false;
                rental.DateIn = DateTime.Now;
                var car = db.Cars.SingleOrDefault(b => b.id == rental.CarID);
                car.StatusRented = false;
                car.KMs = rental.MileageIn;
                Price price = (from pri in db.Prices
                               where pri.Class == car.Class
                               select pri).FirstOrDefault();
                int? kmsUsed = rental.MileageIn - rental.MileageOut;
                //the following line will return the wrong info if car was returned different year to when it was taken
                int daysUsed = (rental.DateIn.Value.DayOfYear - rental.DateOut.Value.DayOfYear) + 1;
                int? freeKms = daysUsed * price.FreeKMs;
                int? kmsToCharge = 0;
                if (kmsUsed > freeKms)
                {
                    kmsToCharge = kmsUsed - freeKms;
                };
                //the following code will only get high season if the whole rental was within high season. this must be fixed
                var season = (from sea in db.Seasons
                              where rental.DateOut >= sea.StartDate
                              where rental.DateIn <= sea.EndDate
                              select sea.High).FirstOrDefault();
                if (season == true)
                {
                    rental.Price = price.HighSeasonDay * daysUsed + kmsToCharge * price.HighSeasonKM;
                    rental.PriceDay = price.HighSeasonDay; 
                    rental.PriceKM = price.HighSeasonKM;
                }
                else
                {
                    rental.Price = price.LowSeasonDay * daysUsed + kmsToCharge * price.LowSeasonKM;
                    rental.PriceDay = price.LowSeasonDay; 
                    rental.PriceKM = price.LowSeasonKM;

                }
                rental.KMsUsed = kmsUsed;
                rental.FreeKMs = freeKms/daysUsed;
                rental.SeasonHigh = season;
                rental.Days = daysUsed;
                db.SaveChanges();
                return RedirectToAction("Details", new {id=rental.id });
            }
            return View(rental);
        }
     
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
                var car = db.Cars.SingleOrDefault(b => b.id == rental.CarID);
                car.StatusRented = false;
                car.KMs = rental.MileageIn;
                Price price = (from pri in db.Prices
                               where pri.Class == car.Class
                               select pri).FirstOrDefault();
                int? kmsUsed = rental.MileageIn - rental.MileageOut;
                //the following line will return the wrong info if car was returned different year to when it was taken
                int daysUsed = (rental.DateIn.Value.DayOfYear - rental.DateOut.Value.DayOfYear) + 1;
                int? freeKms = daysUsed * price.FreeKMs;
                int? kmsToCharge = 0;
                if (kmsUsed > freeKms)
                {
                    kmsToCharge = kmsUsed - freeKms;
                };
                //the following code will only get high season if the whole rental was within high season. this must be fixed
                var season = (from sea in db.Seasons
                              where rental.DateOut >= sea.StartDate
                              where rental.DateIn <= sea.EndDate
                              select sea.High).FirstOrDefault();
                if (season == true)
                {
                    rental.Price = price.HighSeasonDay * daysUsed + kmsToCharge * price.HighSeasonKM;
                    rental.PriceDay = price.HighSeasonDay;
                    rental.PriceKM = price.HighSeasonKM;
                }
                else
                {
                    rental.Price = price.LowSeasonDay * daysUsed + kmsToCharge * price.LowSeasonKM;
                    rental.PriceDay = price.LowSeasonDay;
                    rental.PriceKM = price.LowSeasonKM;

                }
                rental.KMsUsed = kmsUsed;
                rental.FreeKMs = freeKms;
                rental.SeasonHigh = season;
                rental.Days = daysUsed;


                db.SaveChanges();
                return RedirectToAction("Details", new { id = rental.id });
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
