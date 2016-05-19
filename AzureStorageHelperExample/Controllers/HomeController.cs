using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureStorageHelperLibrary;

namespace AzureStorageHelperExample.Controllers
{
    public class HomeController : Controller
    {

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // note the HTML Element name in this case the file must match the HttpPostedFileBase
        // variable name or you'll always get a numm
        public async Task<ActionResult> UploadAsStream(HttpPostedFileBase theFile, string userId, string thePath)
        {
            CloudBlockBlob blobInfo = null;
            if (ModelState.IsValid)
            {
                if (theFile != null && theFile.ContentLength != 0)
                {
                    blobInfo = await AzureStorageHelper.UploadStreamToBlobAsync(theFile.InputStream,theFile.FileName, userId, thePath);

                    Trace.TraceInformation("the uri to the file is {0}", blobInfo.Uri.ToString());
                }

                ViewBag.blobUri = blobInfo.Uri.ToString();
            }

            return View("Upload");
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // note the HTML Element name in this case the file must match the HttpPostedFileBase
        // variable name or you'll always get a numm
        public async Task<ActionResult> Upload(HttpPostedFileBase theFile, string userId ,string thePath)
        {
            CloudBlockBlob blobInfo=null;
            if (ModelState.IsValid)
            {
                if (theFile != null && theFile.ContentLength != 0)
                {
                    blobInfo = await AzureStorageHelper.UploadStreamToBlobAsync(theFile.InputStream,theFile.FileName, userId, thePath);
                    
                    Trace.TraceInformation("the uri to the file is {0}",blobInfo.Uri.ToString());
                }

                ViewBag.blobUri = blobInfo.Uri.ToString();
            }

            return View();
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // note the HTML Element name in this case the file must match the HttpPostedFileBase
        // variable name or you'll always get a numm
        public async Task<ActionResult> Download(string theFilePath)
        {
            System.IO.Stream blobInfo = null;
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(theFilePath))
                {
                    blobInfo = await AzureStorageHelper.DownloadBlobAsyncSecure(theFilePath);

                    Trace.TraceInformation("the uri to the file is {0}", theFilePath);
                }

                //ViewBag.blobUri = blobInfo.Uri.ToString();
            }


            // get MIME type based on extension https://msdn.microsoft.com/en-us/library/system.web.mimemapping.getmimemapping
            string fileType = MimeMapping.GetMimeMapping(theFilePath);
                
            return this.File(blobInfo, fileType);
            
            //return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Downloadform()
        {
            ViewBag.Message = "Your contact page.";

            //return View();
            return View();
        }

        public ActionResult UploadStream()
        {
            ViewBag.Message = "Your contact page.";

            //return View();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}