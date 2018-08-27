namespace XDemo.Core.BusinessServices.Interfaces.Common
{
    public class LoginResultDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool IsValid { get; set; }
        public string[] Roles { get; set; }
    }
}