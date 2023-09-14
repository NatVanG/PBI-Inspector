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
        public bool DialogOKResponse { get;  set;}
    }

    /*
     *     echo "##[group]Beginning of a group"
    echo "##[warning]Warning message"
    echo "##[error]Error message"
    echo "##[section]Start of a section"
    echo "##[debug]Debug text"
    echo "##[command]Command-line being run"
    echo "##[endgroup]"
     * 
     * 
     * 
     */

    public enum MessageTypeEnum
    {
        Error,
        Warning,
        Information,
        Dialog,
        Group,
        Section,
        Debug,
        Command,
        EndGroup
    }
}