using System;
using System.Threading.Tasks;
using XDemo.Core.Infrastructure.Networking.Base;
using System.Threading;

namespace XDemo.Core.Infrastructure.Networking.ApiGateway
{
    public class RestApi : IDataProxy
    {
        public Task<T> Get<T>(string url, object parameter, CancellationToken token = default(CancellationToken)) where T : DtoBase, new()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByParaName<T>(string url, string paraName, object val, CancellationToken token = default(CancellationToken)) where T : DtoBase, new()
        {
            throw new NotImplementedException();
        }

        public Task<T> Post<T>(string url, object parameter, CancellationToken token = default(CancellationToken)) where T : DtoBase, new()
        {
            throw new NotImplementedException();
        }

        public void SetEndpoint(string endpoint)
        {
            throw new NotImplementedException();
        }

        public void SetToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
