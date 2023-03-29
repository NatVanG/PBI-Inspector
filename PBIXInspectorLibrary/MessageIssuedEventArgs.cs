namespace PBIXInspectorLibrary
{
    public class MessageIssuedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MessageTypeEnum MessageType { get; set; }
    }

    public enum MessageTypeEnum
    {
        Error,
        Warning,
        Information
    }
}
