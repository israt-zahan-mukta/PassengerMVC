using PassengerMVC.Models;
using PassengerMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PassengerMVC.Controllers
{
    public class PassengersController : Controller
    {
        private readonly PassengerDatabaseEntities db = new PassengerDatabaseEntities();
        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<Passenger> passengerList = db.Passengers.Include(f => f.SeatFacilities).Include(s => s.SeatType).ToList();
            return View(passengerList);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Passenger passenger = db.Passengers.Find(id);
            if (passenger == null) return HttpNotFound();
            db.Entry(passenger).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            PassengerViewModel passsenger = new PassengerViewModel();
            //passsenger.SeatTypes = db.SeatTypes.ToList();
            ViewBag.SeatId = new SelectList(db.SeatTypes.ToList(), "SeatId", "SeatName", passsenger.SeatId);
            return View(passsenger);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(PassengerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = GetImageName(model.ProfileFile);
                Passenger newpassenger = new Passenger
                {
                    PassengerName = model.PassengerName,
                    SeatId = model.SeatId,
                    BookingDate = model.BookingDate,
                    ImageUrl = uniqueFileName,
                    IsTransit = model.IsTransit,
                    MobileNo = model.MobileNo,
                    BasicPrice = model.BasicPrice
                };
                db.Passengers.Add(newpassenger);
                db.SaveChanges();
                if (model.SeatFacilities != null)
                {
                    foreach (var item in model.SeatFacilities)
                    {
                        db.SeatFacilities.Add(new SeatFacility
                        {
                            PassengerId = newpassenger.PassengerId,
                            FacilityName = item.FacilityName,
                            Price = item.Price
                        });

                    }
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Passenger saved successfully" });
            }
            return Json(new { success = false, message = "Failed to save passenger" });

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var model = db.Passengers.Include(f => f.SeatFacilities).Include(t => t.SeatType).Where(p => p.PassengerId == id).FirstOrDefault();
            if (model == null) return HttpNotFound("Passenger not found");

            var viewModel = new PassengerViewModel
            {
                PassengerId = model.PassengerId,
                PassengerName = model.PassengerName,
                MobileNo = model.MobileNo,
                BookingDate = model.BookingDate,
                SeatId = model.SeatId,
                BasicPrice = model.BasicPrice,
                IsTransit = model.IsTransit,
                ImageUrl = model.ImageUrl,
                SeatFacilities = model.SeatFacilities.ToList(),
                SeatTypes = db.SeatTypes.ToList(),
            };
            ViewBag.SeatId = new SelectList(db.SeatTypes.ToList(), "SeatId", "SeatName", model.SeatId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PassengerViewModel model, string OldImageUrl)
        {
            Passenger domObj = db.Passengers.Include(f => f.SeatFacilities).Include(t => t.SeatType).Where(p => p.PassengerId == model.PassengerId).FirstOrDefault();

            if (domObj != null)
            {
                domObj.PassengerName = model.PassengerName;
                domObj.MobileNo = model.MobileNo;
                domObj.BookingDate = model.BookingDate;
                domObj.SeatId = model.SeatId;
                domObj.BasicPrice = model.BasicPrice;
                domObj.IsTransit = model.IsTransit;
                domObj.ImageUrl = model.ImageUrl;

                if (model.ProfileFile != null)
                {
                    string uniqueFileName = GetImageName(model.ProfileFile);
                    domObj.ImageUrl = uniqueFileName;
                }
                else
                {
                    domObj.ImageUrl = OldImageUrl;
                }
                var existingSeatFacilities = db.SeatFacilities.Where(p => p.PassengerId == domObj.PassengerId).ToList();
                db.SeatFacilities.RemoveRange(existingSeatFacilities);

                foreach (var m in model.SeatFacilities)
                {
                    db.SeatFacilities.Add(new SeatFacility
                    {
                        PassengerId = domObj.PassengerId,
                        FacilityName = m.FacilityName,
                        Price = m.Price
                    });
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Passenger updated successfully" });
            }
            ViewBag.SeatId = new SelectList(db.SeatTypes.ToList(), "SeatId", "SeatName", model.SeatId);
            return Json(new { success = false, message = "Failed to update passenger" });
        }

        private string GetImageName(HttpPostedFileBase profileFile)
        {
            string filePath = "";
            if (profileFile != null)
            {
                filePath = Path.Combine("/images/", Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName));
                profileFile.SaveAs(Server.MapPath(filePath));
            }
            return filePath;
        }
    }
}