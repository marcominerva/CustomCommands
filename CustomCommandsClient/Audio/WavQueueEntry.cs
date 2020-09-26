using NAudio.Wave;

namespace CustomCommandsClient.Audio
{
    public class WavQueueEntry
    {
        public ProducerConsumerStream Stream { get; }

        public RawSourceWaveStream Reader { get; }

        public WavQueueEntry(ProducerConsumerStream stream, RawSourceWaveStream reader) =>
            (Stream, Reader) = (stream, reader);
    }
}
