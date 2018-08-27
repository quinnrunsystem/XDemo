using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Common;

namespace XDemo.Core.BusinessServices.Implementations.Common
{
    public class SecurityService : ISecurityService
    {
        public Task<LoginResultDto> Login(string userName, string password)
        {
            _currentUser = null;
            return Task.FromResult(new LoginResultDto
            {
                IsValid = true,
                UserName = userName,
                DisplayName = "Nguyen Van A"
            });
        }

        public void Logout()
        {
            _currentUser = null;
        }

        private LoginResultDto _currentUser;
        public LoginResultDto CurrentUser()
        {
            //example for storing data within a service
            return _currentUser;
        }
    }
}
