
namespace XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications
{
    public class LocalAuthResult
    {
        public LocalAuthResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
