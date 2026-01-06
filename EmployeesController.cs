using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollMvc.Data;
using PayrollMvc.Models;
using Microsoft.AspNetCore.Authorization;

namespace PayrollMvc.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _db;
        public EmployeesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Employees.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(e => e.FullName.Contains(q));
            return View(await query.OrderBy(e=>e.FullName).ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            return emp == null ? NotFound() : View(emp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            _db.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            return emp == null ? NotFound() : View(emp);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp != null) { _db.Remove(emp); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            return emp == null ? NotFound() : View(emp);
        }
    }
}
