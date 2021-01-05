using Grpc.Core;
using System.Collections.Generic;
using System.Threading;

namespace HomeSpeaker.Mobile.ViewModels
{
    public static class Extensions
    {

        public static SongViewModel ToSongViewModel(this Server.gRPC.SongMessage song)
        {
            return new SongViewModel
            {
                SongId = song?.SongId ?? -1,
                Name = song?.Name ?? "[ Null Song Response ??? ]",
                Album = song?.Album,
                Artist = song?.Artist,
                Path = song?.Path
            };
        }
        public async static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
        {
            if (streamReader == null)
            {
                throw new System.ArgumentNullException(nameof(streamReader));
            }

            while (await streamReader.MoveNext(cancellationToken))
            {
                yield return streamReader.Current;
            }
        }
    }
}
