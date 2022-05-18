namespace Slice.Models
{
    public class Format
    {
        public string Type { get; }
        public float Width { get; }
        public float Height { get; }

        public Format (string _type, float _width, float _height)
        {
            Type = _type;
            Width = _width;
            Height = _height;
        }
    }
}
