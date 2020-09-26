using NAudio.Wave;

namespace CustomCommandsClient.Audio
{
    public class WavQueueEntry
    {
        public WavQueueEntry(string id, bool playInitiated, ProducerConsumerStream stream, RawSourceWaveStream reader) =>
            (Id, PlayInitiated, Stream, Reader) = (id, playInitiated, stream, reader);

        public string Id { get; }

        public bool PlayInitiated { get; set; } = false;

        public bool SynthesisFinished { get; set; } = false;

        public ProducerConsumerStream Stream { get; }

        public RawSourceWaveStream Reader { get; }
    }
}
