using Camara.Models;
using DemoWebCam.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Camara.Controllers
{
    public class CameraController : Controller
    {
        public readonly IWebHostEnvironment _environment;
        private readonly DatabaseContext _context;


        public CameraController(IHostingEnvironment hostingEnvironment, DatabaseContext context)
        {
            _environment = (IWebHostEnvironment)hostingEnvironment;
            _context = context;
        }
        

        public IActionResult Capture()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Capture(string name)
        {
            try
            {
                //RECIBE EL CONTEXTO DE LA PETICION GET DEL NAVEGADOR

                var files = HttpContext.Request.Form.Files;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Obtiene el nombre del archivo
                            var fileName = file.FileName;
                            // Guid es para identificar el archivo por un id"
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            // el tipo de extension del archivo
                            var fileExtension = Path.GetExtension(fileName);
                            //concatena el nombre del archivo con mi extension
                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
                            //  GENERA LA RUTA PARA QUE SE GUARDE LA FOTO
                            var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                            if (!string.IsNullOrEmpty(filepath))
                            {
                                // GUARDA LA IMAGEN EN UN FOLDER
                                StoreInFolder(file, filepath);
                            }

                            var imageBytes = System.IO.File.ReadAllBytes(filepath);
                            if (imageBytes != null)
                            {
                                //GUARDA LA IMAGEN EN LA BASE DE DATOS
                                StoreInDatabase(imageBytes);
                            }

                        }
                    }
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
       

        //guardar en folder
        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }
        //guardar en base de datos
        private void StoreInDatabase(byte[] imageBytes)
        {
            try
            {
                if (imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat(base64String, "data:image/jpg;base64,");


                    ImageStore imageStore = new ImageStore()
                    {

                        ImageBase64String = imageUrl,
                        ImageId = 0,

                    };

                    _context.ImageStore.Add(imageStore);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
