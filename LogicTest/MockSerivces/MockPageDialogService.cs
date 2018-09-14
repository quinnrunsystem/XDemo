using System;
using System.Threading.Tasks;
using Prism.Services;
using System.Linq;
namespace LogicTest.MockSerivces
{
    public class MockPageDialogService : IPageDialogService
    {
        /* ==================================================================================================
         * todo: handle expected reaction
         * ================================================================================================*/
        public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            throw new NotImplementedException();
        }

        public Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
        {
            throw new NotImplementedException();
        }

        public Task DisplayAlertAsync(string title, string message, string cancelButton)
        {
            throw new NotImplementedException();
        }
    }
}
