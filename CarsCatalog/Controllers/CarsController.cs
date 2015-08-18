using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;
namespace CarsCatalog.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IModelCarRepository _modelRepository;
        private readonly IPriceRepository _changePriceRepository;

        public CarsController() : this(null, null, null)
        {
        }

        public CarsController(ICarRepository carRepository, IModelCarRepository modelRepository,
            IPriceRepository changePriceRepository)
        {
            _carRepository = carRepository;
            _modelRepository = modelRepository;
            _changePriceRepository = changePriceRepository;
        }

        public ActionResult Index(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = _modelRepository.GetModelById(id);

            if (model == null)
                return HttpNotFound();

            ViewBag.model = model;
            var cars = _carRepository.GetCarsByModelId(id);
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
            var his = _changePriceRepository.GetChangePriceForCar(id);
            var data =
                his.OrderBy(h => h.DateChange)
                    .Select(h => new {date = h.DateChange.ToString(CultureInfo.InvariantCulture), price = h.Price});
            ViewBag.PriceHistory = data;
            return View(car);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create(int? modelId)
        {
            if (modelId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.currentModelId = modelId;
            ViewBag.ModelId = new SelectList(_modelRepository.GetAll(), "Id", "Name", modelId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Color,Price,EngineCapacity,Description,ModelId")] Car car,
            string startedDate, HttpPostedFileBase image)
        {
            DateTime date;
            if (!DateTime.TryParse(startedDate, out date))
            {
                ModelState.AddModelError("startedDate", "Wrong date format, should be mm/dd/yyyy");
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    car.ImageMimeType = image.ContentType;
                    car.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(car.ImageData, 0, image.ContentLength);
                }
                TempData["OperationStatus"] = _carRepository.AddCarWithDate(car, date);
                return RedirectToAction("Index", new {id = car.ModelId});
            }

            ViewBag.ModelId = new SelectList(_modelRepository.GetAll(), "Id", "Name", car.ModelId);
            return View(car);
        }

        [Authorize(Roles = "admin")]
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
            ViewBag.ModelId = new SelectList(_modelRepository.GetAll(), "Id", "Name", car.ModelId);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Color,Price,EngineCapacity,Description,ModelId")] Car car,
            HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    car.ImageMimeType = image.ContentType;
                    car.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(car.ImageData, 0, image.ContentLength);
                }
                TempData["OperationStatus"] = _carRepository.Update(car);
                return RedirectToAction("Index", new {id = car.ModelId});
            }
            ViewBag.ModelId = new SelectList(_modelRepository.GetAll(), "Id", "Name", car.ModelId);
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
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = _carRepository.GetCarById(id);
            TempData["OperationStatus"] = _carRepository.Delete(car);
            return RedirectToAction("Index", new {Id = car.ModelId});
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