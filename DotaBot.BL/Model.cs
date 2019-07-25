using DotaBot.BL.Data;
using DotaBot.BL.Parsing;
using DotaBot.BL.Struct;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DotaBot.BL
{
    public class Model
    {
        /// <summary>
        /// Событие срабатывает при каких либо проблемах при загрузке страницы. 
        /// Возврат True - попробовать загрузить еще раз.
        /// False - пропустить
        /// </summary>
        public event ErrorLoadPageEventHandler LoadPageError;
        /// <summary>
        /// Событие срабатывает при обновлении статуса парсинга
        /// </summary>
        public event ParserStatusUpdatedEventHandler ParserStatusUpdated;

        private SortedDatabase _database;
        private Match[] _notSortedMatches;

        /// <summary>
        /// Датабаза, если она загружена
        /// </summary>
        public SortedDatabase Database
        {
            get
            {
                if (_database == null)
                    throw new NullReferenceException("База данных не загружена, сначала загрузите базу данных");
                return _database;
            }
        }
        /// <summary>
        /// Не сортированные матчи
        /// </summary>
        public Match[] NotSortedMatches
        {
            get
            {
                if (_notSortedMatches == null)
                    throw new NullReferenceException("Не сортированные матчи не загружены, сначала загрузите матчи");
                return _notSortedMatches;
            }
        }

        /// <summary>
        /// Парсит одну страницу и заносит в NotSortedMatches
        /// </summary>
        public void ParsePage(CancellationToken token)
        {
            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError(sender, args);
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = new Task<Match[]>(() => parser.GetMatchesPage(token));
            task.Start();

            while (!task.IsCompletedSuccessfully)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                    throw task.Exception;
                Thread.Sleep(100);
            }

            _notSortedMatches = task.Result;
        }

        /// <summary>
        /// Парсит все страницы и заносит в NotSortedMatches
        /// </summary>
        /// <param name="token">Токен отмены</param>
        public void Parse(CancellationToken token)
        {
            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError(sender, args);
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = new Task<Match[]>(()=>parser.GetMatches(token));
            task.Start();

            while (!task.IsCompletedSuccessfully)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                    throw task.Exception;
                Thread.Sleep(100);
            }

            _notSortedMatches = task.Result;
        }
    }
}
