namespace CustomCommandsClient.Models
{
    public class MessageDisplay
    {
        public Sender From { get; set; }

        public string Message { get; set; }

        public MessageDisplay(string message, Sender from)
            => (Message, From) = (message, from);

        public override string ToString()
            => $"{From}: {Message}";

        public string Visibility
            => string.IsNullOrEmpty(Message) ? "Collapsed" : "Visible";
    }
}
