using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Services;

namespace RC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdmController : Controller
    {
        private readonly DbS _db;
        public AdmController(DbS db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var users = await _db.GetAllSubs();
            var cars = await _db.GetAllCars();
            ViewBag.AllCars = cars;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _db.DelUser(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuota(string id, int quota)
        {
            if (quota >= 0 && quota <= 10) // حد أقصى منطقي
            {
                await _db.UpdateQuota(id, quota);
            }
            return RedirectToAction("Index");
        }
    }
}