using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.PS.IViews
{
    public interface IMessageService
    {
        void ShowError(string error);
        void ShowExclamation(string exclamation);
        void ShowMessage(string message);

        bool GetAnswer(string question);
    }
}
