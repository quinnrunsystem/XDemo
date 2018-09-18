using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Dtos.Common;
using XDemo.Core.BusinessServices.Interfaces.Common;

namespace XDemo.Core.BusinessServices.Implementations.Common
{
    public class SecurityService : ISecurityService
    {
        public Task<LoginResultDto> Login(string userName, string password)
        {
            CurrentUser = null;
            return Task.FromResult(new LoginResultDto
            {
                IsValid = true,
                UserName = userName,
                DisplayName = "Nguyen Van A"
            });
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        /* ==================================================================================================
         * example for storing data within a service
         * ================================================================================================*/
        public LoginResultDto CurrentUser { get; private set; }
    }
}
