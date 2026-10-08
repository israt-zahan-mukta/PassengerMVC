using PassengerMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassengerMVC.ViewModels
{
    public class PassengerViewModel
    {
        public int PassengerId { get; set; }
        [Display (Name ="Passenger Name")]
        [Required(ErrorMessage ="Passenger New Required..")]
        public string PassengerName { get; set; }
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime BookingDate { get; set; } = DateTime.Now;
       
        public bool IsTransit { get; set; }
        
        
        public string MobileNo { get; set; }
        public decimal BasicPrice { get; set; }
       
        public HttpPostedFileBase ProfileFile { get; set; }
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Seat-Type Required..")]
        public int SeatId { get; set; }

        public virtual IList<SeatType> SeatTypes { get; set; }
        public virtual IList<SeatFacility> SeatFacilities { get; set; } = new List<SeatFacility>();
    }
}