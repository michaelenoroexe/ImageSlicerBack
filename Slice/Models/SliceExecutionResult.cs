namespace Slice.Models
{
    public class SliceExecutionResult
    {
        public string Path { get; }
        public bool ErrStatus { get; }
        public SliceExecutionResult (string _path, bool _errStatus=false)
        {
            Path = _path;
            ErrStatus = _errStatus;
        }
    }
}
