﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Event.Data;
using Event.Models;
using Event.ViewModals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Event.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment WebHostEnvironment;
        private readonly INotyfService _notyf;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        private AppDbContext Context { get; }
        public HomeController(AppDbContext _context,IWebHostEnvironment webHostEnvironment, INotyfService notyf,ILogger<HomeController> logger)
        {
            this.Context = _context;
            _notyf = notyf;
            _logger = logger;
            WebHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            
            return View(Context.Events.ToList());
        }

        public IActionResult Privacy()
        {
            return View(Context.Events.ToList());
        }
        public IActionResult AddEvent(EventsVM events)
        {
            string stringFile = upload(events);
            var data = new Eventss
            {
                Name = events.Name,
                Venue = events.Venue,
                Price = events.Price,
                Time = events.Time,
                Image = stringFile
            };
            this.Context.Events.Add(data);
            this.Context.SaveChanges();
            _notyf.Success("Event Inserted Successfully");
            return RedirectToAction("Index");
        }
        public IActionResult EditEvent(int id)
        {
            var data = Context.Events.Find(id);
            return View(data);
        }
        [HttpPost]
        public IActionResult UpdateEvent(EventsVM update)
        {
            string stringFile = upload(update);
            if (update.Image !=null)
            {
                var data = Context.Events.Find(update.Id);
                string delDir = Path.Combine(WebHostEnvironment.WebRootPath, "Images", data.Image);
                FileInfo f1 = new FileInfo(delDir);
                if (f1.Exists)
                {
                    System.IO.File.Delete(delDir);
                    f1.Delete();
                }
                data.Name = update.Name;
                data.Venue = update.Venue;
                data.Price = update.Price;
                data.Time = update.Time;
                data.Image = stringFile;
                Context.SaveChanges();
            }

            if (update.Image == null)
            {
                var data = Context.Events.Find(update.Id);
                data.Name = update.Name;
                data.Venue = update.Venue;
                data.Price = update.Price;
                data.Time = update.Time;
                Context.SaveChanges();

            }
            _notyf.Success("Updated Successfully");

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var data = Context.Events.Find(id);
            Context.Events.Remove(data);
            Context.SaveChanges();
            _notyf.Success("Deleted Successfully");
            return RedirectToAction("Index");
        }
        private string upload(EventsVM s)
        {
            string fileName = "";
            if (s.Image != null)
            {
                string uploadDir = Path.Combine(WebHostEnvironment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + s.Image.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    s.Image.CopyTo(fileStream);
                }
            }
            return fileName;
        }
        public IActionResult BookEvent(int id)
        {
            var data = Context.Events.Find(id);
            _notyf.Warning("Please Register Yourself First");
            return View(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}