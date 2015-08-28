using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CarsCatalog.Models;
using CarsCatalog.Repository;

namespace CarsCatalog.Controllers
{
    public class BrandsController : Controller
    {
        private readonly IBrandRepository _brandRepository;

        public BrandsController()
            : this(null)
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
                var opStatus = new OperationStatus { Status = true, Message = "Brand added" };

                try
                {
                    _brandRepository.Add(brand);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error adding brand car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
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
                var opStatus = new OperationStatus { Status = true, Message = "Brand updated" };

                try
                {
                    _brandRepository.Update(brand);
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error updating brand car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
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
            ViewBag.CanDelete = !brand.Models.Any();

            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            CarBrand brand = _brandRepository.GetBrandById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            if (!brand.Models.Any())
            {
                var opStatus = new OperationStatus { Status = true, Message = "Brand deleted" };
                try
                {
                    _brandRepository.Delete(brand);
                    
                }
                catch (Exception exp)
                {
                    opStatus = OperationStatus.CreateFromExeption("Error deleting brand car.", exp);
                }

                TempData["OperationStatus"] = opStatus;
            }
            else
                TempData["OperationStatus"] = new OperationStatus()
                {
                    Status = true,
                    Message = "You must delete all models for this brand first"
                };

            return RedirectToAction("Index");
        }
    }
}