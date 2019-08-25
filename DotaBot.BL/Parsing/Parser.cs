using System;
using System.Collections.Generic;
using System.Text;
using DotaBot.BL.Net;
using DotaBot.BL.Struct;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace DotaBot.BL.Parsing
{
    class Parser
    {
        private static Semaphore sem = new Semaphore(10, 10);

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

        public Match[] UpdateMatches(Match[] notSortedMatches, CancellationToken token)
        {
            if (notSortedMatches == null)
                throw new ArgumentNullException("notSortedMatches был null");

            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            List<Match> matchesNew = new List<Match>(); 
            Match LastoldMatch = notSortedMatches[0];
            bool lastMatchloaded = false;

            while (!lastMatchloaded)
            {
                List<Match> matches = new List<Match>();
                if (!TryLoadPage(dotaPageWorker, "Страница с матчами не загружается, если ее не загрузить, то парсинг прервется", token))
                    return matches.ToArray();

                string[] matchURLs = dotaPageWorker.GetMatchesURLs();
                foreach (var matchURL in matchURLs)
                {
                    if (matchURL == LastoldMatch.URL)
                    {
                        lastMatchloaded = true;
                        break;
                    }
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
                

                matchesNew.AddRange(matches);
                if (dotaPageWorker.NextPage == null)
                    break;

                dotaPageWorker = new DotaPageWorker(dotaPageWorker.NextPage);
            }
            matchesNew.AddRange(notSortedMatches);
            return matchesNew.ToArray();
        }

        /// <summary>
        /// Метод парсит с сайта cybersport.ru все матчи Dota2
        /// </summary>
        /// <returns>Матчи</returns>
        public Match[] GetMatches()
        {
            return GetMatches(new CancellationToken());
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
        public Match[] GetMatchesMultyThread()
        {
            return GetMatchesMultyThread(new CancellationToken());
        }   //TODO: оделать многопоточное получение матчей

        /// <summary>
        /// Метод парсит с сайта cybersport.ru все матчи многопоточно с возможностью отмены
        /// </summary>
        /// <param name="token">Токен отмены</param>
        public Match[] GetMatchesMultyThread(CancellationToken token)
        {
            DotaPageWorker dotaPageWorker = new DotaPageWorker();
            List<Match> matches = new List<Match>();
            List<Task<Match[]>> tasks = new List<Task<Match[]>>();

            while (true)
            {
                if (token.IsCancellationRequested)
                    throw new System.OperationCanceledException("Операция отменена", token);

                tasks.Add(Task.Run(()=>GetMatches(dotaPageWorker, token)));
                while (dotaPageWorker.Page == null)
                    Thread.Sleep(100);
                if (dotaPageWorker.NextPage == null)
                    break;

                dotaPageWorker = new DotaPageWorker(dotaPageWorker.NextPage);
            }

            while((from task in tasks
                  where task.Status==TaskStatus.RanToCompletion
                  select task).Count() == tasks.Count)
            {
                if (token.IsCancellationRequested)
                    throw new System.OperationCanceledException("Операция отменена", token);
                foreach (var t in tasks)
                    if (t.IsFaulted)
                        throw t.Exception.InnerException;

                Thread.Sleep(100);
            }

            foreach (var t in tasks)
                matches.AddRange(t.Result);

            return matches.ToArray();
        }

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
            do
            {
                if (token.IsCancellationRequested)
                    throw new OperationCanceledException("Операция отменена", token);

                if (pageWorker.Page == null)
                    try
                    {
                        sem.WaitOne();
                        pageWorker.LoadPage();
                        break; }
                    catch (Exception ex)
                    {
                        bool? res = LoadPageError?.Invoke(this, 
                            new PageInfoArgs(message + "\nОшибка: " + ex.Message + "\nСайт: "+pageWorker.URL, pageWorker.URL));
                        if (res == null || res == false)
                            return false;
                    }
                    finally { sem.Release(); }
                else break;
            } while (true);

            if (!pageWorker.IsSupported())
                return false;
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
