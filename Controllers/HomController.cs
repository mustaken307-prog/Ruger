using Microsoft.AspNetCore.Mvc;
using RC.Services;

namespace RC.Controllers
{
    public class HomController : Controller
    {
        private readonly DbS _db;
        public HomController(DbS db) { _db = db; }

        public async Task<IActionResult> Index(string search)
        {
            var cars = await _db.GetAllCars(search);
            ViewBag.Search = search;
            return View(cars);
        }

        [HttpPost]
        public IActionResult Buy(string phone, string carId)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length != 10)
                return Json(new { success = false, message = "الرجاء إدخال رقم هاتف صحيح (10 أرقام)." });

            // هنا يمكنك إضافة كود لحفظ الطلب في الداتابيس لاحقاً
            return Json(new { success = true, message = "تم استلام طلبك بنجاح! سنتواصل معك قريباً." });
        }
    }
}