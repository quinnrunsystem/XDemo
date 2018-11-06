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
#endif
            var timeoutTcs = new CancellationTokenSource(TimeSpan.FromSeconds(10)); //10s
            var linkedTcs = CancellationTokenSource.CreateLinkedTokenSource(timeoutTcs.Token, cancellationToken);
            try
            {
                /* ==================================================================================================
                 * todo: provide authorization bearer token if needed
                 * ================================================================================================*/
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await base.SendAsync(request, linkedTcs.Token).ConfigureAwait(false);
                return response;
            }
            catch (OperationCanceledException ex)
            {
                if (timeoutTcs.IsCancellationRequested)
                    /* ==================================================================================================
                     * in case of the server connection timeout!
                     * ================================================================================================*/
                    throw new TimeoutException("Connection was timeout!");
                throw ex;
            }
            finally
            {
                timeoutTcs.Dispose();
                linkedTcs.Dispose();
#if DEBUG
                stopWatch.Stop();
                LogCommon.Info($"Durations for resource '{request.RequestUri.AbsolutePath ?? "---"}': {stopWatch.ElapsedMilliseconds:n0} ms");
#endif
            }
        }
    }
}
