using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Secure_Website.Data;
using Secure_Website.Models;

namespace Secure_Website.Controllers
{
    [Authorize(Roles = "Teacher, Student")]
    public class TaskController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TaskController> _logger;
        public TaskController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger<TaskController> logger)
        {
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentView()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation(user.FirstName + " " + user.LastName + "has accessed StudentView");

            List<ScheduleTaskModel> taskList = new List<ScheduleTaskModel>();

            var  loggedstudent = await _userManager.GetUserAsync(HttpContext.User);
            StudentModel studentdb = _db.Student.Where(b => b.StudentId == loggedstudent.Id).FirstOrDefault();
            taskList = _db.ScheduleTask.Where(b => b.TeacherId == studentdb.TeacherId).ToList();


            TaskViewModel model = new TaskViewModel();
            model.ScheduleTasks = taskList;

            return View(model);
        }

    

        public IActionResult Redirect(ScheduleTaskModel model)
        {
            return RedirectToAction("TaskSubmission",model);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TaskSubmission(ScheduleTaskModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation(user.FirstName+" "+user.LastName+"has accessed TaskSubmission");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public IActionResult TaskSubmission(IFormFile file)
        {

            if (ModelState.IsValid)
            {
                string filename;

                if(file == null)
                {
                    ModelState.AddModelError("file", "File is not valid and acceptable");
                    return View();
                }

                if(Path.GetExtension(file.FileName) == ".pdf" )
                {
                    byte[] whitelist = new byte[] {37,80,68,70,45};

                    if(file != null)
                    {
                        MemoryStream userFile = new MemoryStream();

                        using(var f = file.OpenReadStream())
                        {
                            byte[] buffer = new byte[5];
                            f.Read(buffer, 0, 5);

                            for(int i = 0; i < whitelist.Length; i++)
                            {
                                if(whitelist[i] != buffer[i])
                                {
                                    ModelState.AddModelError("file", "File is not valid and acceptable");
                                    return View();
                                }
                            }

                            f.Position = 0;

                            filename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                            string absolutePath = @"StudentFiles\" + filename;

                            try
                            {
                                using(FileStream fsOut = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    f.CopyTo(fsOut);
                                }
                                f.Close();
                            }catch(Exception ex)
                            {
                                //logger
                                //error page
                                _logger.LogError(ex, "Error happend while saving file");
                                return View("Error", new ErrorViewModel() { Message = "Error while saving the file. Try again later" });
                            }

                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("file", "File is not valid and acceptable or size is greater than 10Mb");
                    return View();
                }
            }
            return View();
        }



        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TaskCreation()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            _logger.LogInformation(user.FirstName + " " + user.LastName + "has accessed TaskCreation");

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

            try
            {
                _db.Add(task);
                await _db.SaveChangesAsync();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error Occured while creating task");
                return View("Error", new ErrorViewModel() { Message = "Error occured while creating a task" });
            }

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
