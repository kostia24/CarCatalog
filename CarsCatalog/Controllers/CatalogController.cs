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
        private readonly IBrandRepository _brandRepository;
        private readonly IPriceRepository _changePriceRepository;
        private readonly IModelCarRepository _modelRepository;


        private int _elementsPerPage;
        private int _columns;

        public CatalogController()
            : this(null, null, null, null)
        {
        }

        public CatalogController(ICarFiltersRepository carRepository, IBrandRepository brandRepository,
            IPriceRepository changePriceRepository, IModelCarRepository modelCarRepository)
        {
            _carRepository = carRepository;
            _brandRepository = brandRepository;
            _modelRepository = modelCarRepository;
            _changePriceRepository = changePriceRepository;

            _columns = 4;
            _elementsPerPage = 12;
        }

        public ActionResult Index()
        {
            CheckCookies();
            ViewBag.Columns = _columns;
            ViewBag.elementsPerPageList = new SelectList(new[] {12, 24, 36, 48}, _elementsPerPage);
            ViewBag.columnsList = new SelectList(new[] {2, 3, 4}, _columns);
            return View();
        }

        public ActionResult GetCars(FilterParams filter, int page = 1)
        {
            CheckCookies();
            var cars = _carRepository.GetCarsFromFilter(filter);
            int totalElements = cars.Count();
            if (page < 1) page = 1;
            var selectedCars = _carRepository.GetCarsFromPageRange(cars, page, _elementsPerPage);

            if (filter.Date.HasValue) // add price to car 
            {
                var carPrices = _changePriceRepository.GetListPriceCars(selectedCars, filter.MinPrice, filter.MaxPrice, filter.Date.Value);
                selectedCars = selectedCars.Join(carPrices,
                    car => car.Id, history => history.CarId,
                    (car, history) =>
                    {
                        car.Price = history.Price;
                        return car;
                    }).ToList();
            }

            if (filter.ModelId.HasValue)
            {
                var filteredModel = _modelRepository.GetModelById(filter.ModelId);
                if (filter.BrandId.HasValue)
                {
                    if (filteredModel.BrandId != filter.BrandId)
                        filter.BrandId = filteredModel.BrandId;

                    ViewBag.BrandName = _brandRepository.GetBrandById(filter.BrandId).Name;
                }
                ViewBag.ModelName = _modelRepository.GetModelById(filter.ModelId).Name;
            }
            else
                if (filter.BrandId.HasValue)
                    ViewBag.BrandName = _brandRepository.GetBrandById(filter.BrandId).Name;


            CarsListViewModel model = new CarsListViewModel()
            {
                Cars = selectedCars,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = _elementsPerPage,
                    TotalItems = totalElements
                },
                FilterParams = filter
            };
            return PartialView("Cars", model);
        }

        public ActionResult GetDistinctColors(FilterParams filter)
        {
            if (filter.Color.Length > 20)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = _carRepository.GetCarsFromFilter(filter);
            var distinctColors = _carRepository.DistinctCarsColor(cars, filter.Color);
            return Json(distinctColors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDistinctEngineCapacity(FilterParams filter)
        {
            if (filter.Engine == null || filter.Engine.Length > 5)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cars = _carRepository.GetCarsFromFilter(filter);
            var distinctEngines = _carRepository.DistinctCarsEngineCapacity(cars, filter.Engine);
            return Json(distinctEngines, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetElementsCountOnPage(int elementsPerPage = 12, int columns = 4)
        {
            if (columns < 2 || columns > 7 || elementsPerPage < 5 || elementsPerPage > 100)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            _columns = columns;
            _elementsPerPage = elementsPerPage;
            Response.Cookies.Set(new HttpCookie("columns", _columns.ToString()));
            Response.Cookies.Set(new HttpCookie("elementsPerPage", _elementsPerPage.ToString()));
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBrandsModelsTree()
        {
            var brandsModelsTree = _brandRepository.GetBrandsModelsTree();
            return PartialView("LeftSidebar", brandsModelsTree);
        }

        public ActionResult Contact()
        {
            return View();
        }

        private void CheckCookies()
        {
            if (Request.Cookies["columns"] != null)
            {
                if (int.TryParse(Request.Cookies["columns"].Value, out _columns))
                {
                    if (_columns < 2 || _columns > 7)
                        _columns = 4;
                }
            }
            Response.Cookies.Set(new HttpCookie("columns", _columns.ToString()));
            ViewBag.Columns = _columns;


            if (Request.Cookies["elementsPerPage"] != null)
            {
                if (int.TryParse(Request.Cookies["elementsPerPage"].Value, out _elementsPerPage))
                {
                    if (_elementsPerPage < 5 || _elementsPerPage > 100)
                        _elementsPerPage = 12;
                }
            }
            Response.Cookies.Set(new HttpCookie("elementsPerPage", _elementsPerPage.ToString()));
            ViewBag.ElementsOnPage = _elementsPerPage;
        }
    }
}