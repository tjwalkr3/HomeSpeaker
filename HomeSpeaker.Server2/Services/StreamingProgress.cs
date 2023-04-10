using Grpc.Core;
using HomeSpeaker.Shared;

namespace HomeSpeaker.Server
{
    internal class StreamingProgress : IProgress<double>
    {
        private IServerStreamWriter<CacheVideoReply> responseStream;
        private string title;
        private readonly ILogger logger;
        private double lastProgress = 0;

        public StreamingProgress(IServerStreamWriter<CacheVideoReply> responseStream, string title, ILogger logger)
        {
            this.responseStream = responseStream;
            this.title = title;
            this.logger = logger;
        }

        public async void Report(double value)
        {
            logger.LogInformation("Progress of {title} is {value}", title, value);
            if (value > lastProgress + .01)
            {
                await responseStream.WriteAsync(new CacheVideoReply { PercentComplete = value, Title = title });
                lastProgress = value;
            }
        }
    }
}