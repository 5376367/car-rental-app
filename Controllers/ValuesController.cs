    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using theking.Models;

namespace theking.Controllers
{
    public class ValuesController : ApiController
    {
        private CarRentalDBEntities db = new CarRentalDBEntities();
             
        [Route("api/getcustomer")]
        // was like this: public IHttpActionResult GetCustomers(int id) and return statmenet was return Ok(customer); but it came as an array instead of single object
        public Customer GetCustomer(int id)
        {
            var customer = db.Customers.Where(x => x.id == id);
            return customer.FirstOrDefault();
        }

        [Route("api/getcustomers")]
        public List<Customer> GetCustomers()
        {
            var customer = db.Customers;
            return customer.ToList();
        }

        [HttpPost]
        [Route("api/customer")]
        public IHttpActionResult NewCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            db.Customers.Add(new Customer()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DOB = customer.DOB,
                MemberSince = DateTime.Today,
                Email = customer.Email,
                IDnum = customer.IDnum,
                Notes = "Customer added through WebAPI"
            }); ;

            db.SaveChanges();
            return Ok();
        }

      
        [HttpPost]
        [Route("api/booking")]
        public IHttpActionResult NewBooking(Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            db.Bookings.Add(new Booking()
            {
                Class = booking.Class,
                CustID = booking.CustID,
                DateOut = booking.DateOut,
                DateIn = booking.DateIn,
                Notes = "this booking was made in WebAPI"
            }); ;

            db.SaveChanges();
            return Ok();
        }
       // [HttpPut]
        [Route("editcustomer")]
        public IHttpActionResult Put(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            
                var existingCustomer = db.Customers.Where(s => s.id == customer.id)
                                                        .FirstOrDefault<Customer>();

                if (existingCustomer != null)
                {
                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.IDnum = customer.IDnum;
                existingCustomer.Email = customer.Email;
                existingCustomer.Notes = "Customer edited through WebAPI";

                    db.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            

            return Ok();
        }


    }
}