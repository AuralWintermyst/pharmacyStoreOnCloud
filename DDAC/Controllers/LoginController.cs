using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDAC.Models;

namespace DDAC.Controllers
{
    public class LoginController : Controller
    {
        [ActionName("Login")]
        public async Task<ActionResult> LoginAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<ActionResult> LoginAsync(Users user)
        {
            if(user.Username == null)
            {
                user.Username = "";
            }
            if(user.Password == null)
            {
                user.Password = "";
            }
            var items = await DocumentDBRespository<Users>.GetUsersAsync();
            foreach (var item in items)
            {
                if(user.Username.Equals(item.Username) && user.Password.Equals(item.Password) && item.Type.Equals("admin"))
                {
                    return RedirectToAction("Index", "Home", null);
                }
                else if (user.Username.Equals(item.Username) && user.Password.Equals(item.Password) && item.Type.Equals("patient"))
                {
                    return RedirectToAction("Index", "Patient", null);
                }
                else
                {
                    ViewBag.NotValidUser = "Invalid username or password";
                }
            }
            return View();
        }

        [ActionName("Register")]
        public async Task<ActionResult> RegisterAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<ActionResult> RegisterAsync([Bind("Name,Username,Password")] Users user)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                user.Type = "patient";
                var items = await DocumentDBRespository<Drugs>.GetUsersAsync();
                foreach (var item in items)
                {
                    count++;
                }
                int convert = count + 1;
                string convertid = convert.ToString();
                user.Id = convertid;
                await DocumentDBRespository<Users>.CreateUsersAsync(user);
                return RedirectToAction("Index", "Patient", null);
            }
            return View();
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Description Page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact Page.";

            return View();
        }
    }
}
