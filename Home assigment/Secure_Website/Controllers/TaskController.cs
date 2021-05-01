using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Secure_Website.Data;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    [Authorize(Roles = "Teacher, Student")]
    public class TaskController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        public TaskController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;
            
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentView()
        {

            List<ScheduleTaskModel> taskList = new List<ScheduleTaskModel>();

            var  loggedstudent = await _userManager.GetUserAsync(HttpContext.User);
            StudentModel studentdb = _db.Student.Where(b => b.StudentId == loggedstudent.Id).FirstOrDefault();
            taskList = _db.ScheduleTask.Where(b => b.TeacherId == studentdb.TeacherId).ToList();


            TaskViewModel model = new TaskViewModel();
            model.ScheduleTasks = taskList;
            /*
                 1. first get the student it
                2. compare student id with the student table to get teacher id 
                3. use teacher id to get the associated tasks
                4. put tasks in model and return them
             */

            return View(model);
        }



        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult TaskCreation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TaskCreation (ScheduleTaskModel model)
        {
            int result = DateTime.Compare(model.Date, DateTime.Now.Date);

            if(result < 0)
            {
                TempData["Error"] = "Date Is Less Then Current Date";
                return View();
            }

            var teacher = await _userManager.GetUserAsync(HttpContext.User);
            var task = new ScheduleTaskModel { Date = model.Date, Description = model.Description, TaskName = model.TaskName, TeacherId = teacher.Id };

            _db.Add(task);
            await _db.SaveChangesAsync();

            /*if (_db.ScheduleTask.Find(task) != null)
            {
                TempData["Success"] = "Successfully Added Task";
                return View();
            }
            TempData["Error"] = "Error Occured While Creating Task";*/

            return View();
        }
    }
}
