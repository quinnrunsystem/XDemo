using System.Threading.Tasks;

namespace XDemo.Core.BusinessServices.Interfaces.Common
{
    public interface ISecurityService
    {
        Task<LoginResultDto> Login(string userName, string password);
        void Logout();
        LoginResultDto CurrentUser { get; }
    }
}