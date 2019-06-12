using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using DDAC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DDAC.Controllers
{
    public class DrugController : Controller
    {
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var items = await DocumentDBRespository<Drugs>.GetItemsAsync();
            return View(items);
        }

        [ActionName("AddDrug")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddDrug")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("drugname,description,price,stock")] Drugs drug, IFormFile file)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                var imageUrl = UploadBlob(file);
                drug.imageURL = imageUrl.ToString();
                var items = await DocumentDBRespository<Drugs>.GetItemsAsync();
                foreach (var item in items)
                {
                    count++;
                }
                int convert = count + 1;
                string convertid = convert.ToString();
                drug.Id = convertid;
                await DocumentDBRespository<Drugs>.CreateItemAsync(drug);
                return RedirectToAction("Index");
            }
            return View(drug);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
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

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>
        EditAsync([Bind("Id,drugname,description,price,stock")] Drugs item, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var imageUrl = UploadBlob(file);
                item.imageURL = imageUrl.ToString();
                await DocumentDBRespository<Drugs>.UpdateItemAsync(item.Id,item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [ActionName("DeleteDrug")]
        public async Task<ActionResult> DeleteAsync(string id)
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

        [HttpPost]
        [ActionName("DeleteDrug")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>
        DeleteAsync([Bind("Id,drugname,description,price,stock")] Drugs item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRespository<Drugs>.DeleteItemAsync(item.Id, item);
                return RedirectToAction("Index");
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

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot Configuration = builder.Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("drug-image-container");
            return container;
        }

        public string UploadBlob(IFormFile imageToUpload)
        {
            string imageFullPath = null;
            string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(imageName);
            blob.Properties.ContentType = imageToUpload.ContentType;

            //using (var fileStream = System.IO.File.OpenRead(@"<file-to-upload>"))
            //{
            //    blob.UploadFromStreamAsync(fileStream).Wait();
            //}

            blob.UploadFromStreamAsync(imageToUpload.OpenReadStream()).Wait();

            imageFullPath = blob.Uri.ToString();

            return imageFullPath;
        }

        public ActionResult CreateBlobContainer()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            ViewBag.Success = container.CreateIfNotExistsAsync().Result;
            ViewBag.BlobContainerName = container.Name;

            return View();
        }

        [ActionName("Patients")]
        public async Task<ActionResult> PatientsAsync()
        {
            var items = await DocumentDBRespository<Users>.GetPatientsAsync(d => (d.Type.Equals("patient")));
            return View(items);
        }

        [ActionName("PatientDetail")]
        public async Task<ActionResult> PatientDetailAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Users item = await DocumentDBRespository<Users>.GetUserAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        //// GET: Image  
        //public ActionResult Upload()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Upload(IFormFile photo)
        //{
        //    var imageUrl = UploadBlob(photo);
        //    TempData["LatestImage"] = imageUrl.ToString();
        //    return RedirectToAction("LatestImage");
        //}

        //public ActionResult LatestImage()
        //{
        //    var latestImage = string.Empty;
        //    if (TempData["LatestImage"] != null)
        //    {
        //        ViewBag.LatestImage = Convert.ToString(TempData["LatestImage"]);
        //    }

        //    return View();
        //}
    }

}