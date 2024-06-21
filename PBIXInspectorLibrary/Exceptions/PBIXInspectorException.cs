namespace PBIXInspectorLibrary.Exceptions
{
    public class PBIXInspectorException : Exception
    {
        public PBIXInspectorException()
        {
        }

        public PBIXInspectorException(string message)
            : base(message)
        {
        }

        public PBIXInspectorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
