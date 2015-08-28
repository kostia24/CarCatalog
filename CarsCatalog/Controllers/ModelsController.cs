using System;
using System.Linq;
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
            var brand = _brandRepository.GetBrandById(brandId);
            if (brand == null)
            {
                return HttpNotFound();
            }

            ViewBag.Brand = brand;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,Name,BrandId")] CarModel model)
        {
            if (ModelState.IsValid)
            {
                var opStatus = new OperationStatus { Status = true, Message = "Brand added" };

                try
                {
                    _modelRepository.Add(model);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error adding model car.", exp);
                }

                TempData["OperationStatus"] = opStatus;

                return RedirectToAction("Index", new {Id = model.BrandId});
            }

            ViewBag.Brand = model.Brand; 
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
                var opStatus = new OperationStatus { Status = true, Message = "model updated" };

                try
                {
                    _modelRepository.Update(model);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error updating model car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
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
            ViewBag.CanDelete = !model.Cars.Any();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CarModel model = _modelRepository.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var opStatus = new OperationStatus { Status = true, Message = "model deleted" };

            if (!model.Cars.Any())
            {
                try
                {
                    _modelRepository.Delete(model);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error deleting model car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
            }
            else
                TempData["OperationStatus"] = new OperationStatus() { Status = true, Message = "You must delete all cars for this model first" };


            return RedirectToAction("Index", new {Id = model.BrandId});
        }
    }
}