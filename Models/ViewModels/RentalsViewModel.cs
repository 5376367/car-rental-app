using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace theking.Models.ViewModels
{
    public class RentalsViewModel
    {
        public Car Car { get; set; }
        public Customer Customer {get; set;}
        public Rental Rental { get; set; }

    }
}