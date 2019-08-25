using DotaBot.BL.Data;
using DotaBot.BL.Parsing;
using DotaBot.BL.Struct;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DotaBot.BL
{
    public class Model:DotaBot.BL.IModels.IModel
    {
        private SortedDatabase _database;
        private Match[] _notSortedData;

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
            private set
            {
                if ((_database == null && value != null) || (_database != null && value == null))
                {
                    _database = value;
                    StatusDatabaseChanged?.Invoke(this, EventArgs.Empty);
                }
                else _database = value;
            }
        }
        /// <summary>
        /// Несортированные матчи
        /// </summary>
        public Match[] NotSortedData
        {
            get
            {
                if (_notSortedData == null)
                    throw new NullReferenceException("Не сортированные матчи не загружены, сначала загрузите матчи");
                return _notSortedData;
            }
            set
            {
                if ((_notSortedData == null && value != null) || (_notSortedData != null && value == null))
                {
                    _notSortedData = value;
                    StatusNotSortedDataChanged?.Invoke(this, EventArgs.Empty);
                }
                else _notSortedData = value;
            }
        }
        /// <summary>
        /// Статус базы данных: True-Загружена
        /// </summary>
        public bool StatusDatabase { get => _database != null; }
        /// <summary>
        /// Статус несортированных данных: True-Загружены
        /// </summary>
        public bool StatusNotSortedData { get => _notSortedData != null; }

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
        /// <summary>
        /// Срабатывает при изменении статуса базы данных
        /// </summary>
        public event EventHandler StatusDatabaseChanged;
        /// <summary>
        /// Срабатывает при изменении статуса несортированных данных 
        /// </summary>
        public event EventHandler StatusNotSortedDataChanged;

        public void UppdateNotSortedData(CancellationToken token)
        {
            if (!StatusNotSortedData)
                throw new NullReferenceException("Несортиованные данные не загружены");

            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError?.Invoke(sender, args) == true ? true : false;
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = new Task<Match[]>(() => parser.UpdateMatches(_notSortedData,token));
            task.Start();

            while (task.Status != TaskStatus.RanToCompletion)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                {
                    if (!(task.Exception.InnerException is OperationCanceledException))
                        throw task.Exception;
                    return;
                }
                Thread.Sleep(100);
            }

            NotSortedData = task.Result;
        }

        /// <summary>
        /// Парсит одну страницу и заносит в NotSortedMatches
        /// </summary>
        public void ParsePage(CancellationToken token)
        {
            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError?.Invoke(sender, args) == true ? true : false;
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = Task.Run(() => parser.GetMatchesPage(token));

            while (task.Status!=TaskStatus.RanToCompletion)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                {
                    if (!(task.Exception.InnerException is OperationCanceledException))
                        throw task.Exception;
                    return;
                }
                Thread.Sleep(100);
            }

            NotSortedData = task.Result;
        }

        /// <summary>
        /// Парсит все страницы и заносит в NotSortedMatches
        /// </summary>
        /// <param name="token">Токен отмены</param>
        public void Parse(CancellationToken token)
        {
            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError?.Invoke(sender, args)==true?true:false;
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = new Task<Match[]>(()=>parser.GetMatches(token));
            task.Start();

            while (task.Status != TaskStatus.RanToCompletion)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                {
                    if (!(task.Exception.InnerException is OperationCanceledException))
                        throw task.Exception;
                    return;
                }
                Thread.Sleep(100);
            }

            NotSortedData = task.Result;
        }

        /// <summary>
        /// Парсит все страницы многопоточно
        /// </summary>
        /// <param name="token">Токен отмены</param>
        public void ParseMultyThread(CancellationToken token)
        {
            Parser parser = new Parser();
            parser.LoadPageError += (sender, args) => LoadPageError?.Invoke(sender, args) == true ? true : false;
            int pageOfMatchesLoaded = 0;
            int matchesLoaded = 0;

            parser.MatchLoaded += (sender, args) => matchesLoaded++;
            parser.PageOfMatchesLoaded += (sender, args) => pageOfMatchesLoaded++;

            Task<Match[]> task = new Task<Match[]>(() => parser.GetMatchesMultyThread(token));
            task.Start();

            while (task.Status != TaskStatus.RanToCompletion)
            {
                ParserStatusUpdated?.Invoke(this, new ParserStatusArgs(matchesLoaded, pageOfMatchesLoaded));
                if (task.Status == TaskStatus.Faulted)
                {
                    if (!(task.Exception.InnerException is OperationCanceledException))
                        throw task.Exception;
                    return;
                }
                Thread.Sleep(100);
            }

            NotSortedData = task.Result;
        }

        /// <summary>
        /// Загрузить базу данных из файла
        /// </summary>
        /// <param name="path">Путь к файлу база данных</param>
        public void LoadDatabase(string path)
        {
            BinarySerializer binarySerializer = new BinarySerializer(path);
            Database = binarySerializer.DeserializeSortedDatabase();

        }

        /// <summary>
        /// Загрузить несортированные данные из файла
        /// </summary>
        /// <param name="path">Путь к файлу NotSortedData</param>
        public void LoadNotSortedData(string path)
        {
            BinarySerializer binarySerializer = new BinarySerializer(path);
            NotSortedData = binarySerializer.DeserializeMatches();
        }

        /// <summary>
        /// Сохраняет базу даных
        /// </summary>
        /// <param name="path">Расположение, в котором нужно сохранить данные</param>
        public void SaveDatabase(string path)
        {
            BinarySerializer binarySerializer = new BinarySerializer(path);
            binarySerializer.Serialize(Database);
        }

        /// <summary>
        /// Сохраняет несортированные данные
        /// </summary>
        /// <param name="path">Расположение, в котором нужно сохранить файл</param>
        public void SaveNotSortedData(string path)
        {
            BinarySerializer binarySerializer = new BinarySerializer(path);
            binarySerializer.Serialize(NotSortedData);
        }

        /// <summary>
        /// Аназил несортированных даанных
        /// </summary>
        public void AnalysisNotSortedData()
        {
            if (!StatusNotSortedData)
                throw new ArgumentNullException("Несортированные данные не загружены");
            SortedDatabase database = new SortedDatabase();
            foreach (var match in NotSortedData)
                database.AddNotSortedMatch(match);
            Database = database;
        }

        /// <summary>
        /// Получить результаты матчей
        /// </summary>
        /// <param name="urlsLeftGamers">Ссылки на игроков левой команды</param>
        /// <param name="urlsRightGamers">Ссылки на игроков правой команды</param>
        /// <returns>Шанс для левой команды и шанс ддля правой команды</returns>
        public (double Left, double Right) GetResultMatch(string[] urlsLeftGamers, string[] urlsRightGamers) //TODO: доделать поолучение результата
        {
            if (Database == null)
                throw new NullReferenceException("Сначала нужно загрузить датабазу");

            return Database.GetResultMatch(urlsLeftGamers, urlsRightGamers);
        }
    }
}
