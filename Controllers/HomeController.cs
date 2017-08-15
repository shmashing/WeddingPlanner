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
    public class HomeController : Controller
    {

        private readonly UserFactory userFactory;
        public HomeController(UserFactory connection) {
            userFactory = connection;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(TempData["loginError"] != null){
                ViewBag.loginError = TempData["loginError"];
            }
            return View();
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register(){
            return View("Register");
        }

        [HttpPost]
        [Route("add_user")]
        public IActionResult RegisterUser(RegisterViewModel model){
            if(ModelState.IsValid){
                if(userFactory.GetUserByEmail(model.Email) == null){
                    User newuser = new User {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password
                    };
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newuser.Password = Hasher.HashPassword(newuser, newuser.Password);
                    userFactory.AddUser(newuser);
                } else {
                    ViewBag.EmailError = "Email taken";
                    return View("Register");
                }

            } else {
                return View("Register");
            }
            return Redirect("/");
        }
        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password){
            if(userFactory.ValidateUser(email, password)){
                User user = userFactory.GetUserByEmail(email);
                HttpContext.Session.SetObjectAsJson("user", user);
                return RedirectToAction("Home", "Dash");
            } else {
                TempData["loginError"] = "Invalid email/password";
                return RedirectToAction("Index"); 
            }
        }
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return View("Logout");
        }
    }
}
