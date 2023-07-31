namespace PBIXInspectorLibrary
{
    public class MessageIssuedEventArgs : EventArgs
    {
        public MessageIssuedEventArgs(string message, MessageTypeEnum messageType)
        {
            Message = message;
            MessageType = messageType;
        }

        public string Message { get; private set; }
        public MessageTypeEnum MessageType { get; private set; }
    }

    public enum MessageTypeEnum
    {
        Error,
        Warning,
        Information
    }
}
