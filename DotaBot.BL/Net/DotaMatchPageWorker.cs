using System;
using System.Collections.Generic;
using System.Text;

namespace DotaBot.BL.Net
{
    class DotaMatchPageWorker:PageWorker
    {
        public DotaMatchPageWorker(string URL) : base(URL) { }

        public (int Left, int Right) Score()    //TODO: доделать получение результата матча
        {
            throw new System.NotImplementedException();
        }

        public (string[] Left, string[] Right) GamersNames()    //TODO: доделать получение игроков команд
        {
            throw new System.NotImplementedException();
        }

        public (string Left, string Right) TeamsNames() //TODO: доделать получение названий команд
        {
            throw new System.NotImplementedException();
        }

        public override bool IsSupported()  //TODO: доделать проверку страницы на рабуту с классом
        {
            return base.IsSupported();
        }
    }
}
