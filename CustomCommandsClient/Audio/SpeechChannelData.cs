using Newtonsoft.Json;

namespace CustomCommandsClient.Audio
{
    public class SpeechChannelData
    {
        [JsonProperty("conversationalAiData")]
        public ConvAiData ConversationalAiData { get; set; }
    }

    public class ConvAiData
    {
        [JsonProperty("requestInfo")]
        public RequestDetails RequestInfo { get; set; }
    }

    public class RequestDetails
    {
        [JsonProperty("interactionId")]
        public string InteractionId { get; set; }
    }
}
