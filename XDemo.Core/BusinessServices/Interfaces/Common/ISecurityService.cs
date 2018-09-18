using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Dtos.Common;

namespace XDemo.Core.BusinessServices.Interfaces.Common
{
    public interface ISecurityService
    {
        Task<LoginResultDto> Login(string userName, string password);
        void Logout();
        LoginResultDto CurrentUser { get; }
    }
}