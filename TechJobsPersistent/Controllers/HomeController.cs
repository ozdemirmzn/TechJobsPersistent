using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        
        public IActionResult AddJob()
        {
            List<Employer> employers = context.Employers.ToList();
            List<Skill> skills = context.Skills.ToList();
            AddJobViewModel addJobViewModel = new AddJobViewModel(employers, skills);
            return View(addJobViewModel);
           
        }

        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel, List<string> selectedSkills)
        {

            if (ModelState.IsValid)
            {

                Job newJob = new Job
                {
                    Name = addJobViewModel.Name,
                    EmployerId = (int)addJobViewModel.EmployerId
                };

                context.Jobs.Add(newJob);
                context.SaveChanges();


                foreach (var Id in selectedSkills)
                {

                    int jobId = newJob.Id;
                    int skillId = Int32.Parse(Id);

                    List<JobSkill> existingItems = context.JobSkills
                        .Where(js => js.JobId == jobId)
                        .Where(js => js.SkillId == skillId)
                        .ToList();

                    if (existingItems.Count == 0)
                    {
                        JobSkill jobSkill = new JobSkill
                        {
                            JobId = jobId,
                            SkillId = skillId
                        };
                        context.JobSkills.Add(jobSkill);
                    }
                }

                context.SaveChanges();
                return Redirect("/Home");
            }


            List<Employer> employers = context.Employers.ToList();
            List<Skill> skills = context.Skills.ToList();
            return View("AddJob", new AddJobViewModel(employers, skills));
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }

        public IActionResult Delete(int id)
        {
            Job theJob = context.Jobs
             .Single(e => e.Id == id);

            context.Jobs.Remove(theJob);
            context.SaveChanges();

            return Redirect("/Home");
        }
    }
}
