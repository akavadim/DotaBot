using System;
using System.Collections.Generic;
using System.Text;
using DotaBot.BL.Net;
using DotaBot.BL.Struct;
using System.Threading;

namespace DotaBot.BL.Parsing
{
    class Parser
    {
        /// <summary>
        /// Событие срабатвает, когда матч успешно загружен и записан
        /// </summary>
        public event LoadPageEventHandler MatchLoaded;
        /// <summary>
        /// Событие срабатывает при окончании загрузки страницы одного матча
        /// </summary>
        public event LoadPageEventHandler MatchPageLoaded;
        /// <summary>
        /// Событие срабатывает при окончании загрзуки страницы с матчами
        /// </summary>
        public event LoadPageEventHandler PageOfMatchesLoaded;
        /// <summary>
        /// Событие срабатывает при каких либо проблемах при загрузке страницы. 
        /// Возврат True - попробовать загрузить еще раз.
        /// False - пропустить
        /// </summary>
        public event ErrorLoadPageEventHandler LoadPageError;

        /// <summary>
        /// Метод парсит с сайта cybersport.ru все матчи Dota2
        /// </summary>
        /// <returns>Матчи</returns>
        public Match[] GetMatches()
        {
            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            List<Match> matches = new List<Match>();

            while (true)
            {
                matches.AddRange(GetMatches(dotaPageWorker, new CancellationToken()));
                if (!TryLoadPage(dotaPageWorker, "Следующая страница с матчами не загружается, если ее не загрузить, то парсинг прервется", new CancellationToken()))
                    break;
                if (dotaPageWorker.NextPage == null)
                    break;

                dotaPageWorker = new DotaPageWorker(dotaPageWorker.NextPage);
            }

            return matches.ToArray();
        }

        /// <summary>
        /// Метод парсит с сайта cybersport.ru все матчи Dota2
        /// </summary>
        /// <param name="token">Токен отмены</param>
        /// <returns>Матчи</returns>
        public Match[] GetMatches(CancellationToken token)
        {
            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            List<Match> matches = new List<Match>();

            while (true)
            {
                matches.AddRange(GetMatches(dotaPageWorker, token));
                if (!TryLoadPage(dotaPageWorker, "Следующая страница с матчами не загружается, если ее не загрузить, то парсинг прервется", token))
                    break;
                if (dotaPageWorker.NextPage == null)
                    break;

                dotaPageWorker = new DotaPageWorker(dotaPageWorker.NextPage);
            }

            return matches.ToArray();
        }

        /// <summary>
        /// Метод парсит с сайта cybersport.ru первую страницу с мачами Dota2;
        /// </summary>
        /// <returns>Матчи</returns>
        public Match[] GetMatchesPage()
        {
            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            return GetMatches(dotaPageWorker, new CancellationToken());
        }

        /// <summary>
        /// Метод парсит с сайта cybersport.ru первую страницу с мачами Dota2;
        /// </summary>
        /// <param name="token">Токен отмены</param>
        /// <returns>Матчи</returns>
        public Match[] GetMatchesPage(CancellationToken token)
        {
            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            return GetMatches(dotaPageWorker, token);
        }

        /// <summary>
        /// Метод парсит с сайта cybersport.ru все матчи Dota2 многопоточно
        /// </summary>
        public void GetMatchesMultyThread()
        {
            throw new System.NotImplementedException();
        }   //TODO: Доделать сногопоточное получение матчей

        /// <summary>
        /// Получение матчей по определенной страницу заданной dotaPageWorker
        /// </summary>
        /// <param name="dotaPageWorker">Страница с матчами</param>
        /// <param name="token">Токен отмены</param>
        /// <returns></returns>
        private Match[] GetMatches(DotaPageWorker dotaPageWorker, CancellationToken token)
        {
            if (dotaPageWorker == null)
                throw new ArgumentNullException("dotaPageWorker был пуст");

            List<Match> matches = new List<Match>();
            if (!TryLoadPage(dotaPageWorker, "Страница с матчами не загружается, если ее не загрузить, то парсинг прервется", token))
                return matches.ToArray();

            string[] matchURLs = dotaPageWorker.GetMatchesURLs();
            foreach (var matchURL in matchURLs)
            {
                DotaMatchPageWorker matchPageWorker = new DotaMatchPageWorker(matchURL);

                matchPageWorker.AfterLoadPage += (sender, URL) =>
                {
                    MatchPageLoaded?.Invoke(this, URL);
                };

                if (!TryLoadPage(matchPageWorker, "Страница с матчем не загружается", token))
                    continue;

                matches.Add(GetMatch(matchPageWorker));
                MatchLoaded?.Invoke(this, matchPageWorker.URL);
            }

            PageOfMatchesLoaded?.Invoke(this, dotaPageWorker.URL);
            return matches.ToArray();
        }

        /// <summary>
        /// Попробовать загрузить страницу
        /// </summary>
        /// <param name="pageWorker">Класс с страницей, которю нужно загрузить</param>
        /// <param name="message">Сообщение в случае ошибки загрузки</param>
        /// <param name="token">Токен отмены</param>
        /// <returns>True - загружена, False - нет</returns>
        private bool TryLoadPage(IPageWorker pageWorker, string message, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new OperationCanceledException("Операция отменена", token);
            while (!pageWorker.IsSupported())
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException("Операция отменена", token);

                bool? res = LoadPageError?.Invoke(this, new PageInfoArgs(message, pageWorker.URL));
                if (res == null||res==false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Получает матч с страницы
        /// </summary>
        /// <param name="matchPageWorker">Страница с матчем</param>
        /// <returns>Матч</returns>
        private Match GetMatch(DotaMatchPageWorker matchPageWorker)
        {
            var teamGamers = matchPageWorker.GetGamers();
            var teamsNames = matchPageWorker.TeamsNames();
            var teamsScore = matchPageWorker.Score();

            Team left = new Team(teamsNames.Left, teamGamers.Left);
            Team right = new Team(teamsNames.Right, teamGamers.Right);
            Match match = new Match(left, right, teamsScore.Left, teamsScore.Right, matchPageWorker.URL);

            return match;
        }
    }
}
