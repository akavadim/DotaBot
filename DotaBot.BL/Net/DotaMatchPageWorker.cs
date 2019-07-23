using HtmlAgilityPack;
using System.Linq;
using DotaBot.BL.Struct;

namespace DotaBot.BL.Net
{
    sealed class DotaMatchPageWorker:PageWorker
    {
        /// <summary>
        /// Домен с которым рабоатет класс
        /// </summary>
        private readonly string Site = "https://www.cybersport.ru";

        /// <summary>
        /// Создает экземпляр класса для работы с ссылкой
        /// </summary>
        /// <param name="URL">ссылка на сайт</param>
        public DotaMatchPageWorker(string URL) : base(URL) { }

        /// <summary>
        /// Счет матча
        /// </summary>
        /// <returns>Счет левой и правой команд</returns>
        public (int Left, int Right) Score()
        {
            if (IsInternational())
                return ScoreForInternational();
            HtmlNode[] score = Page.DocumentNode.SelectSingleNode("//p[@class='duel__count-score']").SelectNodes("./span").ToArray();
            int scoresLeft = int.Parse(score[0].InnerText);
            int scoresRight = int.Parse(score[1].InnerText);
            return (scoresLeft, scoresRight);
        }
        
        /// <summary>
        /// Получение игроков для левой и правой команды
        /// </summary>
        /// <returns>Игроки левой и правой команд</returns>
        public (Gamer[] Left, Gamer[] Right) GetGamers()
        {
            if (IsInternational())
                return GetGamersForInternational();
            var teams = Page.DocumentNode.SelectNodes("//ul[@class='list-unstyled']");
            HtmlNode leftNode = teams[0];
            HtmlNode rightNode = teams[1];
            Gamer[] left = GetGamersByTeamNode(leftNode);
            Gamer[] right = GetGamersByTeamNode(rightNode);

            return (left, right);

        }
        
        /// <summary>
        /// Получение названий команд
        /// </summary>
        /// <returns>Названия левой и правой команд</returns>
        public (string Left, string Right) TeamsNames() //TODO: доделать получение названий команд
        {
            if (IsInternational())
                return TeamsNamesForInternational();
            var nodeNames = Page.DocumentNode.SelectNodes("//h2[@class='duel__title']");
            string left = nodeNames[0].InnerText;
            string right = nodeNames[1].InnerText;
            return (left, right);
        }

        /// <summary>
        /// Получает названия команд для матча-интернешионала
        /// </summary>
        /// <returns>Названия левой и правой команд</returns>
        private (string Left, string Right) TeamsNamesForInternational()
        {
            var nodeNames = Page.DocumentNode.SelectNodes("//header[@class='d-flex align-items-center']");
            string left = nodeNames[0].SelectSingleNode("a").ChildNodes["h2"].InnerText;
            string right = nodeNames[1].SelectSingleNode("a").ChildNodes["h2"].InnerText;
            return (left, right);
        }

        /// <summary>
        /// Получает счет для матча-интернешинола
        /// </summary>
        /// <returns>Счет для левой и правой команд</returns>
        private (int Left, int Right) ScoreForInternational()
        {
            HtmlNode[] score = Page.DocumentNode.SelectNodes("//header[@class='d-flex align-items-center']").ToArray();
            int scoresLeft = int.Parse(score[0].SelectSingleNode("./strong").InnerText);
            int scoresRight = int.Parse(score[1].SelectSingleNode("./strong").InnerText);
            return (scoresLeft, scoresRight);
        }
        
        /// <summary>
        /// Получение игроков для матча-интернегинола
        /// </summary>
        /// <returns>Игроки левой и правой команд</returns>
        private (Gamer[] Left, Gamer[] Right) GetGamersForInternational()
        {
            var teams = Page.DocumentNode.SelectNodes("//ul[@class='list-unstyled int-about-match__teams']");
            HtmlNode leftNode = teams[0];
            HtmlNode rightNode = teams[1];
            Gamer[] left = GetGamersByTeamNodeForInternational(leftNode);
            Gamer[] right = GetGamersByTeamNodeForInternational(rightNode);

            return (left, right);
        }
        
        /// <summary>
        /// Получает игроков для опряделенной команды
        /// </summary>
        /// <param name="teamNode">Подходящий узел с командой</param>
        /// <returns>Игроки команды</returns>
        private Gamer[] GetGamersByTeamNode(HtmlNode teamNode)
        {
            var gamerNodes = teamNode.SelectNodes("./li")?.ToArray();
            Gamer[] gamers = new Gamer[gamerNodes.Length];
            for (int i = 0; i < gamerNodes.Length; i++)
            {
                string url = Site + gamerNodes[i].ChildNodes["a"].Attributes["href"].Value;
                string name = gamerNodes[i].ChildNodes["a"].ChildNodes[2].InnerText.Replace("\n", "").Trim();
                gamers[i] = new Gamer(name, url);
            }

            return gamers;
        }

        /// <summary>
        /// Получат игроков для определенной команды матча-интернешионола
        /// </summary>
        /// <param name="teamNode">Подъодящий узел с командой</param>
        /// <returns>Игроки</returns>
        private Gamer[] GetGamersByTeamNodeForInternational(HtmlNode teamNode)
        {
            var gamerNodes = teamNode.SelectNodes("./li")?.ToArray();
            Gamer[] gamers = new Gamer[gamerNodes.Length];
            for (int i = 0; i < gamerNodes.Length; i++)
            {
                string url = Site + gamerNodes[i].ChildNodes["a"].Attributes["href"].Value;
                string name = gamerNodes[i].SelectSingleNode(".//strong").SelectSingleNode("./span").InnerText;
                gamers[i] = new Gamer(name, url);
            }

            return gamers;
        }

        /// <summary>
        /// Проверяет, поддерживает ли этот класс работу с URL
        /// </summary>
        /// <returns>True - поддерживает, False - не поддерживает</returns>
        public override bool IsSupported()
        {
            bool supported = false;
            bool workedURL = URL.Contains("cybersport.ru") && URL.Contains("/base/match/group/");
            bool neededComponentsForInternational = (Page.DocumentNode.SelectSingleNode("//ul[@class='list-unstyled list-team-status']") != null) &&
                (Page.DocumentNode.SelectSingleNode("//header[@class='d-flex align-items-center']") != null)
                && (Page.DocumentNode.SelectNodes("//li[@class='d-flex alig-items-center team']")?.Count > 9);
            bool neededComponents = (Page.DocumentNode.SelectNodes("//h2[@class='duel__title']").Count > 1)
                && (Page.DocumentNode.SelectNodes("li").Count > 9);

            if (base.IsSupported() && workedURL && (neededComponents || neededComponentsForInternational))
                supported = true;

            return supported;
        }

        /// <summary>
        /// Проверяет является ли эта страница страницей с интернешионалом
        /// </summary>
        /// <returns>True - Является, False - не является</returns>
        public bool IsInternational()
        {
            if (!IsSupported())
                throw new PageNotSupportedException("Страница не поддерживается");
            bool neededComponentsForInternational = (Page.DocumentNode.SelectSingleNode("//ul[@class='list-unstyled list-team-status']") != null) &&
               (Page.DocumentNode.SelectSingleNode("//header[@class='d-flex align-items-center']") != null)
               && (Page.DocumentNode.SelectNodes("//li[@class='d-flex alig-items-center team']")?.Count > 9);
            return neededComponentsForInternational;
        }
    }
}
