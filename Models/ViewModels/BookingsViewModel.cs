using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace theking.Models.ViewModels
{
    public class BookingsViewModel
    {
        public Customer Customer { get; set; }
        public Booking Booking { get; set; }
       // public Price Price { get; set; }
    }
}