using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernHttpClient;
#if DEBUG
using System.Diagnostics;
using XDemo.Core.Infrastructure.Logging;
#endif

namespace XDemo.Core.Infrastructure.Networking.Base
{
    public class ExtendedNativeMessageHandler : NativeMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
#if DEBUG
            var stopWatch = Stopwatch.StartNew();
            LogCommon.Info($"Begin call api. Method: {request.Method.ToString()} - Resource: '{request.RequestUri.AbsolutePath ?? "---"}' - Host: '{request.RequestUri.Host ?? "---"}'");
            try
            {
#endif
                //default: charset utf-8; content-type: applicetion/json
                //todo: provide authorization bear token if needed
                request.Headers.Add("Accept", "application/json");
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                return response;
#if DEBUG
            }
            finally
            {
                stopWatch.Stop();
                LogCommon.Info($"Durations for resource '{request.RequestUri.AbsolutePath ?? "---"}': {stopWatch.ElapsedMilliseconds:n0} ms");
            }
#endif
        }
    }
}
