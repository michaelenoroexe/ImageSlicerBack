using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Slice.Models;
using System.Drawing;
using SkiaSharp;
using System.Net.Http;
using Slice.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Slice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_AllowAll")]
    public class Slice : ControllerBase
    {
        private readonly List<Format> _formats;
        private readonly SliceRepository _slice;
        public Slice(List<Format> formats, SliceRepository slice)
        {
            _formats = formats;
            _slice = slice;
        }
        // GET: api/<Slice>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<Slice>
        // Reciewing instructions from front and generate output sliced picture
        [HttpPost]        
        [EnableCors("_AllowAll")]
        public async Task<ActionResult> Post()
        {
            string path = "";
            try
            {
                // initializing of slice
                if (Request.Form.Files == null || Request.Form.Files.Count == 0) throw new Exception("Empty Image");
                IFormFile image = Request.Form.Files.SingleOrDefault()!;
                string userFormat = Request.Form.FirstOrDefault(reqest => reqest.Key == "type").Value.ToString();
                int colNumber = Convert.ToInt32(Request.Form.First(reqest => reqest.Key == "colNum").Value);
                bool orient = Request.Form.FirstOrDefault(reqest => reqest.Key == "orientation").Value.ToString() == "landscape" ? true : false;
                // Slice image
                path = await _slice.SliceAsync(new SliceInstructions(image, _formats.First(format => format.Type == userFormat), colNumber, orient));
                // Get Image and return to client
                var bytes = await System.IO.File.ReadAllBytesAsync(path + "/Poster.pdf");                
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }
            finally { Directory.Delete(path, true); }
        }

    }
}
