namespace HL7App.Models
{
    public class HL7Message
    {
        public HL7Message(String messageContent)
        {
            if (String.IsNullOrEmpty(messageContent))
            {
                throw new ArgumentException("Message content cannot be null or empty.", nameof(messageContent));
            }
            if (messageContent.Length < 10) // Arbitrary length check for a valid HL7 message, can be adjusted as needed
            {
                throw new ArgumentException("Message content is too short to be a valid HL7 message.", nameof(messageContent));
            }
            if (!messageContent.StartsWith("MSH"))
            {
                throw new ArgumentException("Message content must start with 'MSH' segment.", nameof(messageContent));
            }
            _messageContent = messageContent;
            Initialize();
        }
        public HL7Message()
        {
            _messageContent = "";
            MessageId = "";
            MessageType = "";
            MessageEvent = "";
            MessageVersion = "";
        }

        private void Initialize()
        {
            // sanitize segment endings, since files are uploaded from disk some OS' try to help out by
            // replacing carriage returns and line feeds with the environment's newline character which
            // can cause issues when trying to parse the message content
            _messageContent = _messageContent.Replace("\n", "\r").Replace("\r\r", "\r");
            FieldDelimiter = MessageContent[3]; // The field delimiter is the fourth character in the MSH segment   
            ComponentDelimiter = MessageContent[4]; // The component delimiter is the fifth character in the MSH segment
            SubfieldDelimiter = MessageContent[5]; // The subfield delimiter is the sixth character in the MSH segment
            MessageId = GetValueFromMessage("MSH", 10);
            MessageType = GetValueFromMessage("MSH", 9, 1);
            MessageEvent = GetValueFromMessage("MSH", 9, 2);
            MessageVersion = GetValueFromMessage("MSH", 12);
        }
        // For this implementation we'll assume the segment delimiter is a carriage return (\r)
        private const char _segmentDelimiter = '\r';
        private String _messageContent;

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public char FieldDelimiter { get; set; } = '|';
        public char ComponentDelimiter { get; set; } = '^';
        public char SubfieldDelimiter { get; set; } = '&';
        public String MessageId { get; set; }
        public String MessageType { get; set; }
        public String MessageEvent { get; set; }
        public String MessageVersion { get; set; }
        public String MessageContent
        {
            get
            {
                return _messageContent;
            }
            set
            {
                _messageContent = value;
                Initialize();
            }
        }

        // Naive implementation to extract a value from the HL7 message content based on segment, component, and field.
        // Currently ignores subfields, escape characters and repeating fields, will return the first instance of the specified segment and field.
        public String GetValueFromMessage(String segName, int? field, int? component = null)
        {
            if (String.IsNullOrEmpty(MessageContent))
            {
                throw new InvalidOperationException("Message content is not set.");
            }
            foreach (var segment in MessageContent.Split(_segmentDelimiter))
            {
                if (segment.StartsWith(segName + FieldDelimiter))
                {
                    if (!field.HasValue)
                    {
                        return segment;
                    }
                    var fields = segment.Split(FieldDelimiter);
                    if (segName == "MSH")
                    {
                        // MSH field 1 is the field separator itself so the segment is treated as a special case 
                        if (field == 1)
                        {
                            return FieldDelimiter.ToString();
                        }
                        else
                        {
                            field -= 1;
                        }
                    }
                    if (field < fields.Length)
                    {
                        if (component.HasValue && component > 0)
                        {
                            var components = fields[field.Value].Split(ComponentDelimiter);
                            if (component.Value <= components.Length)
                            {
                                return components[component.Value - 1];
                            }
                        }
                        else
                        {
                            return fields[field.Value]; // Return the whole field if the specified component index is not included
                        }
                    }
                }
            }
            return String.Empty; // Return empty String if the specified segment, field, or component is not found in the message
        }
    }
}