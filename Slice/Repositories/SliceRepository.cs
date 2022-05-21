using SkiaSharp;
using Slice.Models;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Slice.Repositories
{
    public class SliceRepository
    {
        public SliceRepository()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public async Task<SliceExecutionResult> SliceAsync(SliceInstructions _inst)
        {
            // Create directory of current slice operation and save file to it
            string path = "../../"+Guid.NewGuid().ToString();
            Directory.CreateDirectory(path);
            string reservePath = path;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await _inst.Image.CopyToAsync(memoryStream);
                    using (var stream = new FileStream(path + "/file.jpg", FileMode.Create))
                    {
                        _inst.Image.CopyTo(stream);
                    }
                }
                // Read saved file and create pdf based on it.            
                using (var input = File.OpenRead(path + "/file.jpg"))
                using (var inputStream = new SKManagedStream(input))
                using (var original = SKBitmap.Decode(inputStream))
                {
                    _inst.Bitmap = original;
                    List<SKBitmap> pieces = ImageSlice(_inst);
                    SaveToPdf(pieces, path, _inst.Landscape);
                    return new SliceExecutionResult(path);
                }
            }
            catch (Exception ex)
            {
                return new SliceExecutionResult(reservePath, true);
            }
        }
        // Slice image to a multiple parts
        public List<SKBitmap> ImageSlice (SliceInstructions _instr)
        {
            // Initializing
            float mm = 3.7795f;
            int formatWidth = (int)(!_instr.Landscape ? _instr.Format.Width * mm : _instr.Format.Height * mm);
            int formatHeight = (int)(_instr.Landscape ? _instr.Format.Width * mm : _instr.Format.Height * mm);
            int totalWidth = (int)(formatWidth * _instr.Columns);
            int totalHeight = (int)(_instr.Bitmap.Height * totalWidth / _instr.Bitmap.Width);
            int rownumber = (int)Math.Ceiling((float)totalHeight / (float)formatHeight);
            var resized = _instr.Bitmap.Resize(new SKImageInfo(totalWidth, totalHeight), SKFilterQuality.High);
            var pieces = new List<SKBitmap>();
            SKRectI bitmapRect = new SKRectI(0, 0, formatWidth, formatHeight);
            // Split
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
        public void SaveToPdf(List<SKBitmap> _pieces, string path, bool _landscape=false)
        {
            using (var pdfDocument = new PdfDocument())
            {
                // Create an empty page
                PdfPage page;
                // Get an XGraphics object for drawing
                XGraphics gfx;
                foreach (SKBitmap piece in _pieces)
                {
                    // Creating 1 page of pdf
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
