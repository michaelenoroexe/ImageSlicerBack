using SkiaSharp;

namespace Slice.Models
{
    public class SliceInstructions
    {
        public IFormFile Image { get; }
        public bool Landscape { get; }
        public Format Format { get; }
        public int Columns { get; }
        public SKBitmap? Bitmap { get; set; }

        public SliceInstructions(IFormFile _image, Format _format, int _col, bool _orient = false)
        {
            Image = _image;
            Landscape = _orient;
            Format = _format;
            Columns = _col;
        }
    }
}
