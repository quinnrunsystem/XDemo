using System;
namespace XDemo.Core.Storage.Settings
{
    public class LoginSettings
    {
        public bool IsUserIdSaved { get; set; }
        public string SavedUserId { get; set; }
        public bool IsPasswordSaved { get; set; }
        public string SavedPassword { get; set; }
        public bool AutoLogin { get; set; }
    }
}
