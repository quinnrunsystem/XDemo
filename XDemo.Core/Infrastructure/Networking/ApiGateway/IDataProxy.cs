using System.Threading.Tasks;
using XDemo.Core.Infrastructure.Networking.Base;
using System.Threading;

namespace XDemo.Core.Infrastructure.Networking.ApiGateway
{
    public interface IDataProxy
    {
        Task<T> Post<T>(string url, object parameter, CancellationToken token = default(CancellationToken))
              where T : DtoBase, new();

        Task<T> Get<T>(string url, object parameter, CancellationToken token = default(CancellationToken))
            where T : DtoBase, new();

        Task<T> GetByParaName<T>(string url, string paraName, object val, CancellationToken token = default(CancellationToken))
            where T : DtoBase, new();

        void SetEndpoint(string endpoint);

        void SetToken(string token);
    }
}
