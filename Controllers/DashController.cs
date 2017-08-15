using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Factory;

namespace WeddingPlanner.Controllers
{
    public class DashController : Controller
    {

        private readonly WeddingFactory weddingFactory;
        private readonly UserFactory userFactory;
        public DashController(WeddingFactory connection, UserFactory connection2) {
            weddingFactory = connection;
            userFactory = connection2;
        }

        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Home()
        {
            var user = HttpContext.Session.GetObjectFromJson<User>("user");
            if(user == null){
                return Redirect("/");
            }
            ViewBag.user = user;
            ViewBag.weddings = weddingFactory.GetAllWeddings();
            return View("Home");
        }
        [HttpGet]
        [Route("/add_wedding")]
        public IActionResult AddWedding(){
            var user = HttpContext.Session.GetObjectFromJson<User>("user");
            if(user == null){
                return Redirect("/");
            }

            user = userFactory.GetUserById(user.Id);
            if(user.MyWeddingId == 0){
                return RedirectToAction("AlreadyMarried");
            }

            ViewBag.user = user;
            ViewBag.unwedUsers = userFactory.GetUnwedUsers(user.Id);

            return View("AddWedding");
        }

        [HttpPost]
        [Route("/make_wedding")]
        public IActionResult MakeWedding(AddWeddingViewModel newWedd){
            if(ModelState.IsValid){
                Wedding wedding = new Wedding {
                    WedderOneId = newWedd.WedderOneId,
                    WedderTwoId = newWedd.WedderTwoId,
                    Date = newWedd.Date,
                    Address = newWedd.Address
                };
                weddingFactory.AddWedding(wedding);
                return RedirectToAction("Home");
            }
            return View("AddWedding");
        }
        [HttpGet]
        [Route("alread_married")]
        public IActionResult AlreadyMarried(){
            var user = HttpContext.Session.GetObjectFromJson<User>("user");
            if(user == null){
                return RedirectToAction("/");
            }
            return View("alreadyMarried");
        }
    }
}