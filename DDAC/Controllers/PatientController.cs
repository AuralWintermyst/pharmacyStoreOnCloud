using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDAC.Models;

namespace DDAC.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ActionName("DrugList")]
        public async Task<ActionResult> DrugListAsync()
        {
            ViewBag.message = TempData["redirect"];
            var items = await DocumentDBRespository<Drugs>.GetItemsAsync();
            return View(items);
        }

        [ActionName("BuyItem")]
        public async Task<ActionResult> OrderAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Drugs item = await DocumentDBRespository<Drugs>.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            if(item.stock == 0)
            {
                TempData["redirect"] = "No stock for this drug";
                return RedirectToAction("DrugList");
            }
            return View(item);
        }

        [HttpPost]
        [ActionName("BuyItem")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrderAsync([Bind("Id,drugname,description,price,imageURL,stock")] Drugs item, string quantity)
        {
            int number = Convert.ToInt32(quantity);
            if (ModelState.IsValid)
            {
                item.stock = item.stock - number;
                await DocumentDBRespository<Drugs>.UpdateItemAsync(item.Id, item);
                TempData["redirect"] = number * item.price;
                //ViewBag.payment = number * item.price;
                return RedirectToAction("Payment");
            }
            return View(item);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Drugs item = await DocumentDBRespository<Drugs>.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Payment()
        {
            ViewBag.payment = TempData["redirect"];
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