using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        public int? InSession { 
            get { return HttpContext.Session.GetInt32("userid"); }
            set { HttpContext.Session.SetInt32("userid", (int)value); }
        }
        
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            if(InSession != null)
            {
                return RedirectToAction("Dashboard");
            }
            IndexViewModel viewmod = new IndexViewModel();
            return View(viewmod);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost("NewUser")]
        public IActionResult NewUser(IndexViewModel newuser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.users.Any(u => u.Email == newuser.SingleUser.Email))
                {
                    ModelState.AddModelError("Email", "Email alreayd in use!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newuser.SingleUser.Password = Hasher.HashPassword(newuser.SingleUser, newuser.SingleUser.Password);

                dbContext.Add(newuser.SingleUser);
                dbContext.SaveChanges();
                InSession =  newuser.SingleUser.UserId;
                return RedirectToAction("Dashboard");
            }
            else{
                return View("Index");
            }
        }

        [HttpPost("NewLogin")]
        public IActionResult NewLogin(IndexViewModel user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == user.SingleLogin.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }

                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(user.SingleLogin, userInDb.Password, user.SingleLogin.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                    return View("Index");
                }
                User currentUser = dbContext.users.FirstOrDefault(u => u.Email == user.SingleLogin.Email);
                HttpContext.Session.SetInt32("userid", currentUser.UserId);
                return RedirectToAction("Dashboard");
            }
            else{
                return View("Index");
            }
        }
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return RedirectToAction("Index");
            }
            IndexViewModel viewmod = new IndexViewModel();
            viewmod.AllWeddings = dbContext.weddings.OrderByDescending(d => d.CreatedAt).Include(g => g.guests).ThenInclude(l =>l.User).ToList();
            viewmod.SingleUser = dbContext.users
            .Include(c => c.Weddings)
            .ThenInclude(p => p.Wedding)
            .FirstOrDefault( u => u.UserId == HttpContext.Session.GetInt32("userid"));

            viewmod.AllAss = dbContext.associations
            .Where( a => a.UserId == (int)HttpContext.Session.GetInt32("userid")).ToList();

            viewmod.NotAttending = dbContext.weddings
            .Include(g => g.guests)
            .ThenInclude(l =>l.User)
            .Where(u => !viewmod.SingleUser.Weddings
            .Any(x => x.Wedding.WeddingId == u.WeddingId)).ToList();
            
            viewmod.Attending = dbContext.weddings
            .Include(g => g.guests)
            .ThenInclude(l =>l.User)
            .Where(u => viewmod.SingleUser.Weddings
            .Any(x => x.Wedding.WeddingId == u.WeddingId)).ToList();

            viewmod.Creator = dbContext.weddings
            .Where(q => q.CreatorId == HttpContext.Session.GetInt32("userid")).ToList();
            
            viewmod.UserId = (int)HttpContext.Session.GetInt32("userid");
            
            return View(viewmod);
        }

        public IActionResult NewWedding()
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost("AddWedding")]
        public IActionResult AddWedding(Wedding fromform)
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                User currentUser = dbContext.users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userid"));
                
                fromform.CreatorId = currentUser.UserId;
                dbContext.Add(fromform);
                dbContext.SaveChanges();

                Wedding currentWedding = dbContext.weddings.LastOrDefault();

                Association newAsso = new Association();
                newAsso.UserId = currentUser.UserId;
                newAsso.WeddingId = currentWedding.WeddingId;
                dbContext.Add(newAsso);
                dbContext.SaveChanges();

                return RedirectToAction("PlanInfo", new {PlanId=fromform.WeddingId});
            }
            return View("NewWedding");

        }

        [HttpPost("RSVP")]
        public IActionResult RSVP(IndexViewModel fromform)
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            fromform.UserId = (int)HttpContext.Session.GetInt32("userid");
            if(!dbContext.associations.Any(a => a.UserId == fromform.UserId && a.WeddingId == fromform.NewAssos.WeddingId))
            {
                dbContext.Add(fromform.NewAssos);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost("UNRSVP")]
        public IActionResult UNRSVP(IndexViewModel fromform)
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            if(fromform.NewAssos.UserId != (int)HttpContext.Session.GetInt32("userid"))
            {
                return RedirectToAction("Dashboard");
            }
            Association Breakthis = dbContext.associations.FirstOrDefault(a => a.UserId == fromform.NewAssos.UserId && a.WeddingId == fromform.NewAssos.WeddingId);
            if(Breakthis != null)
            {
                dbContext.Remove(Breakthis);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");

        }

        [HttpPost("Delete")]
        public IActionResult Delete(IndexViewModel fromform)
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            if(fromform.NewAssos.UserId != (int)HttpContext.Session.GetInt32("userid"))
            {
                return RedirectToAction("Dashboard");
            }
            Wedding Deletethis = dbContext.weddings.FirstOrDefault(a => a.CreatorId == fromform.NewAssos.UserId && a.WeddingId == fromform.NewAssos.WeddingId);
            dbContext.Remove(Deletethis);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
           
        }

        [HttpGet("PlanInfo/{PlanId}")]
        public IActionResult PlanInfo(int PlanId)
        {
            if(InSession == null)
            {
                return RedirectToAction("Index");
            }
            Wedding ThisInfo = dbContext.weddings.Include(u => u.guests).ThenInclude(p => p.User).FirstOrDefault(a => a.WeddingId == PlanId);
            return View(ThisInfo);
        }

    }
}
