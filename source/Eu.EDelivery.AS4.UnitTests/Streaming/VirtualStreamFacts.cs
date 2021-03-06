using System.IO;
using Eu.EDelivery.AS4.Streaming;
using Xunit;

namespace Eu.EDelivery.AS4.UnitTests.Streaming
{
    public class VirtualStreamFacts
    {
        [Fact]
        public void IsFileStreamWhenInitialCapacityIsLargerThenDefaultThreshold()
        {
            using (VirtualStream stream = VirtualStream.Create(VirtualStream.ThresholdMax + 1))
            {
                Assert.True(stream.UnderlyingStream is FileStream);
            }
        }

        [Fact]
        public void IsFileStreamWhenInitialCapacityIsLargerThenSpecifiedThreshold()
        {
            using (VirtualStream stream = VirtualStream.Create(10, 7))
            {
                Assert.True(stream.UnderlyingStream is FileStream);
            }
        }

        [Fact]
        public void CorrectlyOverflowsToDiskWhenThresholdIsReached()
        {
            byte[] bytesToWrite = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            using (VirtualStream stream = VirtualStream.Create(1, 10))
            {
                Assert.True(stream.UnderlyingStream is MemoryStream);

                stream.Write(bytesToWrite, 0, bytesToWrite.Length);

                Assert.True(stream.UnderlyingStream is MemoryStream);

                stream.Write(bytesToWrite, 0, bytesToWrite.Length);

                Assert.True(stream.UnderlyingStream is MemoryStream);

                stream.Write(bytesToWrite, 0, bytesToWrite.Length);

                Assert.True(stream.UnderlyingStream is FileStream);
                Assert.Equal(bytesToWrite.Length * 3, stream.Length);
            }
        }

        [Fact]
        public void UnderlyingFileIsDeletedAfterDispose()
        {
            string fileName = string.Empty;

            using (VirtualStream stream = new VirtualStream(VirtualStream.MemoryFlag.OnlyToDisk))
            {
                FileStream fs = stream.UnderlyingStream as FileStream;

                Assert.NotNull(fs);

                fileName = fs.Name;
            }

            Assert.False(File.Exists(fileName));
        }
    }
}
