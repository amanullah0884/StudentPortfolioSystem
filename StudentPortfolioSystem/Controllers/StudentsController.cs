using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortfolioSystem.Models;

namespace StudentPortfolioSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.Include(s => s.Department).ToListAsync();
            return View(students);
        }

        // Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        //Create
        [HttpPost]
       
        public async Task<IActionResult> Create(Student student, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null)
                        student.ImagePath = await SaveImage(ImageFile);

                    _context.Add(student);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Student added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving data: " + ex.Message);
                }
            }

            LoadDropdowns();
            return View(student);
        }

        // Edit
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            LoadDropdowns();
            return View(student);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, IFormFile? ImageFile)
        {
            if (id != student.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null)
                        student.ImagePath = await SaveImage(ImageFile);

                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = " Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(s => s.Id == id))
                        return NotFound();
                    throw;
                }
            }

            LoadDropdowns();
            return View(student);
        }

        // Details
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }
        [HttpPost]
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Student deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Save uploaded image
        private async Task<string?> SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only JPG, JPEG, or PNG files are allowed.");

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/" + uniqueFileName;
        }

        // Dropdown list
        private void LoadDropdowns()
        {
            var departments = _context.Departments.ToList();

            ViewBag.Departments = departments != null && departments.Count > 0
                ? new SelectList(departments, "Id", "DepartmentName")
                : new SelectList(Enumerable.Empty<SelectListItem>());

            ViewBag.Majors = new List<string> {  "Artificial Intelligence","Software Engineering", "Computer Networking",
                "Accounting and Information Systems", "Computer Hardware Engineering","Textile Engineering" };
            ViewBag.Years = Enumerable.Range(2000, 50).ToList();
        }
        //private void LoadDropdowns()
        //{
        //    ViewBag.Departments = new SelectList(new List<Department>
        //    {
        //        new Department
        //        {

        //            Id=1,
        //            DepartmentName="Test"
        //        }

        //    }, "Id", "DepartmentName");
        //    ViewBag.Majors = new List<string> { "CSE", "EEE", "BBA", "Civil", "Textile" };
        //    ViewBag.Years = Enumerable.Range(2000, 50).ToList();
        //}
    }
}
