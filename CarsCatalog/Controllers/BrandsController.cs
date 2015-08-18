using System.Net;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;

namespace CarsCatalog.Controllers
{
    public class BrandsController : Controller
    {
        private readonly IBrandRepository _brandRepository;

        public BrandsController() : this(null)
        {
        }

        public BrandsController(IBrandRepository brandRepo)
        {
            _brandRepository = brandRepo;
        }

        public ActionResult Index()
        {
            return View(_brandRepository.GetAll());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarBrand brand = _brandRepository.GetBrandById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name")] CarBrand brand)
        {
            if (ModelState.IsValid)
            {
                TempData["OperationStatus"] = _brandRepository.Add(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarBrand brand = _brandRepository.GetBrandById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] CarBrand brand)
        {
            if (ModelState.IsValid)
            {
                TempData["OperationStatus"] = _brandRepository.Update(brand);
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarBrand brand = _brandRepository.GetBrandById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CarBrand brand = _brandRepository.GetBrandById(id);
            TempData["OperationStatus"] = _brandRepository.Delete(brand);
            return RedirectToAction("Index");
        }
    }
}