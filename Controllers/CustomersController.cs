﻿using System;
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
    public class CustomersController : Controller
    {
        private CarRentalDBEntities db = new CarRentalDBEntities();

        // GET: Customers
        public ActionResult Index(string sortOrder)
        {
            ViewBag.FirstSortParm = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewBag.LastSortParm = sortOrder == "last" ? "last_desc" : "last";
            ViewBag.DOBSortParm = sortOrder == "DOB" ? "DOB_desc" : "DOB";
            ViewBag.MemberSortParm = sortOrder == "member" ? "member_desc" : "member";
            var customers = from c in db.Customers
                       select c;
           switch (sortOrder)
            {
                case "first_desc":
                    customers = customers.OrderByDescending(c => c.FirstName);
                    break;
                case "last":
                    customers = customers.OrderBy(c => c.LastName);
                    break;
                case "last_desc":
                    customers = customers.OrderByDescending(c => c.LastName);
                    break;
                case "DOB":
                    customers = customers.OrderBy(c => c.DOB);
                    break;
                case "DOB_desc":
                    customers = customers.OrderByDescending(c => c.DOB);
                    break;
                case "member":
                    customers = customers.OrderBy(c => c.MemberSince);
                    break;
                case "member_desc":
                default:
                    customers = customers.OrderBy(c => c.FirstName);
                    break;
            }
            return View(customers.ToList());
        }




        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,FirstName,LastName,DOB,IDnum,Email,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                customer.MemberSince = DateTime.Today;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,FirstName,LastName,DOB,IDnum,Email,MemberSince,Notes")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
