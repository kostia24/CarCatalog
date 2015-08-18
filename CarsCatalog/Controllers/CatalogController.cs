using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;

namespace CarsCatalog.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICarFiltersRepository _carRepository;
        private readonly IBrandCarTreeRepository _brandRepository;
        private readonly IPriceRepository _changePriceRepository;


        private int _elementsPerPage;
        private int _columns;

        public CatalogController()
            : this(null, null, null)
        {
        }

        public CatalogController(ICarFiltersRepository carRepository, IBrandCarTreeRepository brandRepository,
            IPriceRepository changePriceRepository)
        {
            _carRepository = carRepository;
            _brandRepository = brandRepository;
            _changePriceRepository = changePriceRepository;

            _columns = 4;
            _elementsPerPage = _columns * 5;
        }

        public ActionResult Index()
        {
            CheckCookies();
            var elementsOnPageList = Enumerable.Range(_columns * 3, _columns * 5).Where(e => e % _columns == 0);
            ViewBag.ElementsPerPageList = new SelectList(elementsOnPageList, _elementsPerPage);
            ViewBag.ColumnsList = new SelectList(Enumerable.Range(2, 4), _columns);
            return View();
        }

        public ActionResult GetCars(int page = 1)
        {
            var cars = GetCarsFromTree(null, null);
            var selectedCars = PagingSelectedCars(cars, page);
            return PartialView("Cars", selectedCars);
        }

        public ActionResult GetCarsByBrand(int? brandId, int page = 1)
        {
            if (brandId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cars = _carRepository.GetCarsByBrandId(brandId);
            return DecorateWithPage(cars, page);
        }

        public ActionResult GetCarsByModel(int? modelId, int page = 1)
        {
            if (modelId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cars = _carRepository.GetCarsByModelId(modelId);
            return DecorateWithPage(cars, page);
        }

        public ActionResult GetCarsByPrice(int? brandId, int? modelId, string date, int min = 0, int max = int.MaxValue,
            int page = 1)
        {
            DateTime searchDate;
            if (!DateTime.TryParse(date, out searchDate))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = GetCarsFromTree(brandId, modelId);
            cars = _carRepository.GetCarsFromPriceRangeWithSpecificDate(cars, min, max, searchDate);
            cars = PagingSelectedCars(cars, page);

            var listCar = cars as IList<Car> ?? cars.ToList();
            if (!listCar.Any())
                return PartialView("NotFoundCar");

            var carPrices = _changePriceRepository.GetListPriceCars(listCar, min, max, searchDate);
            var carWithNewPrice = listCar.Join(carPrices,
                car => car.Id, history => history.CarId,
                (car, history) =>
                {
                    car.Price = history.Price;
                    return car;
                });

            return PartialView("Cars", carWithNewPrice);
        }

        public ActionResult GetBrandsModelsTree()
        {
            var brandsModelsTree = _brandRepository.GetBrandsModelsTree();
            return PartialView("LeftSidebar", brandsModelsTree);
        }

        public ActionResult GetCarsByColor(int? brandId, int? modelId, string color, int page = 1)
        {
            if (color.Length > 20)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = GetCarsFromTree(brandId, modelId);
            var selectedCars = _carRepository.GetCarsByColor(cars, color);
            return DecorateWithPage(selectedCars, page);
        }

        public ActionResult GetDistinctColors(string color, int? brandId, int? modelId)
        {
            if (color.Length > 20)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = GetCarsFromTree(brandId, modelId);
            var distinctColors = _carRepository.DistinctCarsColor(cars, color);
            return Json(distinctColors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCarsByEngineCapacity(int? brandId, int? modelId, string capacity, int page = 1)
        {
            if (capacity == null || capacity.Length > 5)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = GetCarsFromTree(brandId, modelId);
            var selectedCars = _carRepository.GetCarsByEngineCapacity(cars, capacity);
            return DecorateWithPage(selectedCars, page);
        }

        public ActionResult GetDistinctEngineCapacity(string capacity, int? brandId, int? modelId)
        {
            if (capacity.Length > 5)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = GetCarsFromTree(brandId, modelId);
            var distinctCapacities = _carRepository.DistinctCarsEngineCapacity(cars, capacity);
            return Json(distinctCapacities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetElementsCountOnPage(int elementsCount = 100, int columns = 5)
        {
            if (columns < 2 || columns > 7 || elementsCount > 100 || elementsCount % columns != 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            _columns = columns;
            _elementsPerPage = elementsCount;
            Response.Cookies.Set(new HttpCookie("columns", _columns.ToString()));
            Response.Cookies.Set(new HttpCookie("elementsPerPage", _elementsPerPage.ToString()));
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Contact()
        {
            return View();
        }

        private int CheckPage(int page)
        {
            if (page <= 0) page = 1;
            return page;
        }

        private ActionResult DecorateWithPage(IEnumerable<Car> cars, int page)
        {
            cars = PagingSelectedCars(cars, page);

            if (!cars.Any())
                return PartialView("NotFoundCar");

            return PartialView("Cars", cars);
        }

        private IEnumerable<Car> PagingSelectedCars(IEnumerable<Car> cars, int page)
        {
            CheckCookies();
            int pageCount;
            var selectedCars = _carRepository.GetCarsFromPageRange(cars, CheckPage(page), _elementsPerPage,
                out pageCount);
            ViewBag.pageCount = pageCount;
            ViewBag.pageNumbers = Enumerable.Range(1, page);
            ViewBag.CurrentPageNumber = page;
            return selectedCars;
        }

        private IEnumerable<Car> GetCarsFromTree(int? brandId, int? modelId)
        {
            if (modelId != null)
                return _carRepository.GetCarsByModelId(modelId);

            if (brandId != null)
                return _carRepository.GetCarsByBrandId(brandId);
            return _carRepository.GetAll();
        }

        private void CheckCookies()
        {
            if (Request.Cookies["columns"] != null)
            {
                if (int.TryParse(Request.Cookies["columns"].Value, out _columns))
                {
                    if (_columns < 2 || _columns > 7)
                        _columns = 5;
                }
                Response.Cookies.Set(new HttpCookie("columns", _columns.ToString()));
            }
            else
                Response.Cookies.Add(new HttpCookie("columns", _columns.ToString()));
            ViewBag.Columns = _columns;

            if (Request.Cookies["elementsPerPage"] != null)
            {
                if (int.TryParse(Request.Cookies["elementsPerPage"].Value, out _elementsPerPage))
                    if (_elementsPerPage % _columns != 0)
                        _elementsPerPage = _columns * 5;
                Response.Cookies.Set(new HttpCookie("elementsPerPage", _elementsPerPage.ToString()));
            }
            else
                Response.Cookies.Add(new HttpCookie("elementsPerPage", _elementsPerPage.ToString()));

            ViewBag.ElementsOnPage = _elementsPerPage;
        }
    }
}