using SkiaSharp;
using Slice.Models;
using System.Drawing;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Slice.Repositories
{
    public class SliceRepository
    {
        private readonly List<Format> _formats;
        const int quality = 100;
        public SliceRepository(List<Format> formats)
        {
            _formats = formats;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public async Task<string> Slice(SliceInstructions _inst)
        {
            if (_inst.Image == null || _inst.Image.Length == 0)
            {
                throw new Exception("Null");
            }
            string path = "../../"+Guid.NewGuid().ToString();
            Directory.CreateDirectory(path);
            using (var memoryStream = new MemoryStream())
            {
                await _inst.Image.CopyToAsync(memoryStream);
                using (var stream = new FileStream(path+"/file.jpg", FileMode.Create))
                {
                    _inst.Image.CopyTo(stream);
                }
            }

            using (var input = File.OpenRead(path + "/file.jpg"))
            using (var inputStream = new SKManagedStream(input))
            using (var original = SKBitmap.Decode(inputStream))
            {
                _inst.Bitmap = original;
                List<SKBitmap> pieces = await ImageSlice(_inst);
                await SaveToPdf(pieces, path, _inst.Landscape);
                return path;       
            }
        }
        // Slice image to a multiple parts
        public async Task<List<SKBitmap>> ImageSlice (SliceInstructions _instr)
        {            
            float mm = 3.7795f;
            int formatWidth = (int)(!_instr.Landscape ? _instr.Format.Width * mm : _instr.Format.Height * mm);
            int formatHeight = (int)(_instr.Landscape ? _instr.Format.Width * mm : _instr.Format.Height * mm);
            int totalWidth = (int)(formatWidth * _instr.Columns);
            int totalHeight = (int)(_instr.Bitmap.Height * totalWidth / _instr.Bitmap.Width);
            int rownumber = (int)Math.Ceiling((float)totalHeight / (float)formatHeight);
            var resized = _instr.Bitmap.Resize(new SKImageInfo(totalWidth, totalHeight), SKFilterQuality.High);
            var pieces = new List<SKBitmap>();
            SKRectI bitmapRect = new SKRectI(0, 0, formatWidth, formatHeight);

            for (int i = 0; i < rownumber; i++)
            for (int j = 0; j < _instr.Columns; j++)
            {
                var part = new SKBitmap(formatWidth, formatHeight);
                var vtmp = new SKCanvas(part);
                vtmp.DrawColor(SKColor.FromHsl(360, 100, 100));
                vtmp.DrawBitmap(resized, new SKRectI(j * formatWidth, i * formatHeight, (j+1) * formatWidth, (i+1) * formatHeight), bitmapRect);                
                vtmp.Save();         
                pieces.Add(part);
            }            

            return pieces;      
        }
        // Save multiple picture parts in to a pdf file
        public async Task SaveToPdf(List<SKBitmap> _pieces, string path, bool _landscape=false)
        {
            using (var pdfDocument = new PdfDocument())
            {
                // Create an empty page
                PdfPage page;
                // Get an XGraphics object for drawing
                XGraphics gfx;
                foreach (SKBitmap piece in _pieces)
                {
                    page = pdfDocument.AddPage();
                    page.Height = !_landscape?(piece.Height * 0.7501573317):(piece.Width * 0.7501573317);
                    page.Width = _landscape ? (piece.Height * 0.7501573317) : (piece.Width * 0.7501573317);
                    if (_landscape) page.Orientation = PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                    using (var p = SKImage.FromBitmap(piece).Encode(SKEncodedImageFormat.Jpeg, 100).AsStream())
                    {
                        gfx.DrawImage(XBitmapImage.FromStream(p), new XPoint(0, 0));
                    }
                }
                // Save the document...
                string filename = path+"/Poster.pdf";
                pdfDocument.Save(filename);
            }
        }
    }
}

//using (var image = SKImage.FromBitmap(resized))
//{
//    using (var memstrim = new MemoryStream(image.Encode().ToArray()))
//    {
//        IFormFile resizedImage = new FormFile(memstrim, 0, memstrim.Length, null, "resizedImage.jpg")
//        {
//            Headers = new HeaderDictionary(),
//            ContentType = "image/*"
//        };
//        return resizedImage;
//    }
//}
