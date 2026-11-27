using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Models;
using RC.Services;
using RC.ViewModels;
using System.Security.Claims;

namespace RC.Controllers
{
    [Authorize(Roles = "Subscriber")]
    public class SubController : Controller
    {
        private readonly DbS _db;
        private readonly ImgS _img;

        public SubController(DbS db, ImgS img)
        {
            _db = db;
            _img = img;
        }

        // 1) عرض سيارات المشترك
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cars = await _db.GetCarsByOwner(userId!);
            var user = await _db.GetUserById(userId!);

            ViewBag.Quota = user?.CarQuota ?? 4;
            return View(cars);
        }

        // 2) صفحة إضافة سيارة (GET)
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // 3) حفظ السيارة الجديدة (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CarVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string imgUrl = "";

            if (model.ImageFile != null)
            {
                // رفع الصورة باستخدام خدمة ImgBB
                var uploadedUrl = await _img.Upload(model.ImageFile);

                if (!string.IsNullOrEmpty(uploadedUrl))
                {
                    imgUrl = uploadedUrl;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "فشل رفع الصورة إلى السحابة. يرجى التأكد من مفتاح الـ API أو المحاولة لاحقاً.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("ImageFile", "يرجى اختيار صورة للسيارة.");
                return View(model);
            }

            try
            {
                var car = new Car
                {
                    OwnerId = userId!,
                    Brand = model.Brand,
                    ModelName = model.ModelName,
                    Description = model.Description,
                    Price = model.Price,
                    Location = model.Location,
                    ImageUrl = imgUrl
                };

                await _db.AddCar(car);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // إذا فشل الحفظ في الداتابيس (مثلاً وصل للحد الأقصى)
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // 4) حذف سيارة (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var car = await _db.GetCar(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // التأكد من أن السيارة موجودة وأنها تخص هذا المستخدم
            if (car != null && car.OwnerId == userId)
            {
                await _db.DelCar(id);
            }
            return RedirectToAction("Index");
        }
    }
}