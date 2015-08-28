using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;
using Microsoft.AspNet.Identity;

namespace CarsCatalog.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IBrandRepository _brandsTreeRepository;
        private readonly IModelCarRepository _modelCarRepository;
        private readonly IPriceRepository _changePriceRepository;

        public CarsController()
            : this(null, null, null, null)
        {
        }

        public CarsController(ICarRepository carRepository, IBrandRepository brandsTreeRepository,
            IPriceRepository changePriceRepository, IModelCarRepository modelCarRepository)
        {
            _carRepository = carRepository;
            _brandsTreeRepository = brandsTreeRepository;
            _changePriceRepository = changePriceRepository;
            _modelCarRepository = modelCarRepository;
        }

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
                return HttpNotFound();

            var cars = _carRepository.GetCarsByUserId(userId);

            return View(cars);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Car car = _carRepository.GetCarById(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.datePrices = _changePriceRepository.GetChangePriceForCar(id).OrderBy(h => h.DateChange);
           
            return View(car);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.BrandTree = _brandsTreeRepository.GetBrandsModelsTree();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Color,Price,EngineCapacity,Description,ModelId")] Car car, HttpPostedFileBase image)
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
                return HttpNotFound();

            var model = _modelCarRepository.GetModelById(car.ModelId);
            if (model == null)
            {
                ModelState.AddModelError("ModelId", "No existing model");
            }

            if (ModelState.IsValid)
            {
                car.UserId = userId;
                if (image != null)
                {
                    car.ImageMimeType = image.ContentType;
                    car.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(car.ImageData, 0, image.ContentLength);
                }
                var opStatus = new OperationStatus { Status = true, Message = "Car added" };

                try
                {
                    _carRepository.Add(car);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error adding car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
                return RedirectToAction("Index");
            }

            ViewBag.BrandTree = _brandsTreeRepository.GetBrandsModelsTree();
            return View(car);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Car car = _carRepository.GetCarById(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarModel = car.Model;
            ViewBag.BrandTree = _brandsTreeRepository.GetBrandsModelsTree();
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Color,Price,EngineCapacity,Description,ModelId")] Car car,
            HttpPostedFileBase image)
        {
            var model = _modelCarRepository.GetModelById(car.ModelId);
            if (model == null)
            {
                ModelState.AddModelError("ModelId", "No existing model");
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    car.ImageMimeType = image.ContentType;
                    car.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(car.ImageData, 0, image.ContentLength);
                }
                var opStatus = new OperationStatus { Status = true, Message = "Car updated" };

                try
                {
                    _carRepository.Update(car);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error updating car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
                return RedirectToAction("Index", new { id = car.ModelId });
            } 
            ViewBag.BrandTree = _brandsTreeRepository.GetBrandsModelsTree();
            ViewBag.CarModel = car.Model;
            return View(car);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = _carRepository.GetCarById(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = _carRepository.GetCarById(id);
            var opStatus = new OperationStatus { Status = true, Message = "Car deleted" };

            try
            {
               _carRepository.Delete(car);
            }
            catch (Exception exp)
            {
                opStatus = OperationStatus.CreateFromExeption("Error deleting car.", exp);
            }

            TempData["OperationStatus"] = opStatus;
            return RedirectToAction("Index", new { Id = car.ModelId });
        }

        public ActionResult GetChartData(int id)
        {
            return Json(_changePriceRepository.GetChangePriceForCar(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckColor(string color)
        {
            return Json(Color.FromName(color).IsKnownColor, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult GetImage(int id)
        {
            Car car = _carRepository.GetCarById(id);
            return car != null ? File(car.ImageData, car.ImageMimeType) : null;
        }
    }
}