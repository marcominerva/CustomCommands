using NAudio.Wave;
using System.IO;

namespace CustomCommandsClient.Audio
{
    public class WavQueueEntry
    {
        public Stream Stream { get; }

        public RawSourceWaveStream Reader { get; }

        public WavQueueEntry(Stream stream, RawSourceWaveStream reader) =>
            (Stream, Reader) = (stream, reader);
    }
}
