using System.Threading.Tasks;

namespace XDemo.Core.BusinessServices.Interfaces.Common
{
    public interface IClipboardService
    {
        bool HasText { get; }
        Task<string> GetTextAsync();
        void SetText(string text);
    }
}
