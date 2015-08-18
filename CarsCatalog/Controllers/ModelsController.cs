using System.Net;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;

namespace CarsCatalog.Controllers
{
    public class ModelsController : Controller
    {
        private readonly IModelCarRepository _modelRepository;
        private readonly IBrandRepository _brandRepository;

        public ModelsController() : this(null, null)
        {
        }

        public ModelsController(IModelCarRepository modelRepository, IBrandRepository brandRepository)
        {
            _modelRepository = modelRepository;
            _brandRepository = brandRepository;
        }

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var brand = _brandRepository.GetBrandById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            ViewBag.brand = brand;
            var models = _modelRepository.GetModelsByBrandId(id);
            return View(models);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CarModel model = _modelRepository.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create(int? brandId)
        {
            if (brandId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.currentbrandId = brandId;
            ViewBag.BrandId = new SelectList(_brandRepository.GetAll(), "Id", "Name", brandId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name,BrandId")] CarModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["OperationStatus"] = _modelRepository.Add(model);
                return RedirectToAction("Index", new {Id = model.BrandId});
            }

            ViewBag.BrandId = new SelectList(_brandRepository.GetAll(), "Id", "Name", model.BrandId);
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CarModel model = _modelRepository.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandId = new SelectList(_brandRepository.GetAll(), "Id", "Name", model.BrandId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,BrandId")] CarModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["OperationStatus"] = _modelRepository.Update(model);
                return RedirectToAction("Index", new {Id = model.BrandId});
            }
            ViewBag.BrandId = new SelectList(_brandRepository.GetAll(), "Id", "Name", model.BrandId);
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarModel model = _modelRepository.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CarModel model = _modelRepository.GetModelById(id);
            TempData["OperationStatus"] = _modelRepository.Delete(model);
            return RedirectToAction("Index", new {Id = model.BrandId});
        }
    }
}