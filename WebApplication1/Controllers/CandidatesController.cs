using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly CandidateDbContext db;
        private readonly IWebHostEnvironment he;
        public CandidatesController(CandidateDbContext _db, IWebHostEnvironment _he)
        {
            db= _db;
            he= _he;
        }
        public IActionResult Index()
        {
            IQueryable<Candidate> candidates = db.Candidates.Include(x => x.CandidateSkills).ThenInclude(y => y.Skill);
            return View(candidates);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult AddSkill(int id)
        {
            ViewBag.skill = new SelectList(db.Skills, "SkillId", "SkillName", id.ToString() ?? "");
            return PartialView("_addSkill");
        }
        [HttpPost]
        public IActionResult Create(CandidateVM candidateVM, int[] skillId)
        {
            if(ModelState.IsValid)
            {
                Candidate candidate = new Candidate
                {
                    CandidateName=candidateVM.CandidateName,
                    DateOfBirth=candidateVM.DateOfBirth,
                    Phone=candidateVM.Phone,
                    Fresher=candidateVM.Fresher
                };
                //for Image
                var file = candidateVM.ImageFile;
                string webroot = he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(candidateVM.ImageFile.FileName);
                string fileToSave=Path.Combine(webroot, folder,imgFileName);
                if (file != null)
                {
                    using(var stream =new FileStream(fileToSave, FileMode.Create))
                    {
                        candidateVM.ImageFile.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                //for skill
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill
                    {
                        Candidate = candidate,
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    db.CandidateSkills.Add(candidateSkill);
                }
                db.Candidates.Add(candidate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            var candidates = db.Candidates.Find(id);
            if (id != null)
            {
                var extSkill = db.CandidateSkills.Where(x => x.CandidateId == id).ToList();
                db.CandidateSkills.RemoveRange(extSkill);
                db.Candidates.Remove(candidates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            var candidates = db.Candidates.FirstOrDefault(x => x.CandidateId == id);
            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId=candidates.CandidateId,
                CandidateName=candidates.CandidateName,
                DateOfBirth=candidates.DateOfBirth,
                Phone=candidates.Phone,
                Fresher=candidates.Fresher,
                Image=candidates.Image,
            };
            var extSkill = db.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            //for skill
            foreach (var item in extSkill)
            {
                candidateVM.candidateskill.Add(item.SkillId);
            }
            return View(candidateVM);
        }
        [HttpPost]
        public IActionResult Edit(CandidateVM candidateVM, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate
                {
                    CandidateId = candidateVM.CandidateId,
                    CandidateName = candidateVM.CandidateName,
                    DateOfBirth = candidateVM.DateOfBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher,
                    Image = candidateVM.Image
                };
                //for Image
                var file = candidateVM.ImageFile;
                if (file != null)
                {
                    string webroot = he.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Path.GetFileName(candidateVM.ImageFile.FileName);
                    string fileToSave = Path.Combine(webroot, folder, imgFileName);
                    if (file != null)
                    {
                        using (var stream = new FileStream(fileToSave, FileMode.Create))
                        {
                            candidateVM.ImageFile.CopyTo(stream);
                            candidate.Image = "/" + folder + "/" + imgFileName;
                        }
                    }
                }
                else
                {
                    candidate.Image = candidateVM.Image;
                }
                var extskill = db.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                foreach (var item in extskill)
                {
                    db.CandidateSkills.Remove(item);
                }
                //for skill
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill
                    {
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    db.CandidateSkills.Add(candidateSkill);
                }
                db.Update(candidate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
