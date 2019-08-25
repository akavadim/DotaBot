using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DotaBot.BL.IModels
{
    public interface IModel
    {
        /// <summary>
        /// Статус базы данных: True-Загружена
        /// </summary>
        bool StatusDatabase { get; }
        /// <summary>
        /// Статус несортированных данных: True-Загружены
        /// </summary>
        bool StatusNotSortedData { get; }

        /// <summary>
        /// Событие срабатывает при каких либо проблемах при загрузке страницы. 
        /// Возврат True - попробовать загрузить еще раз.
        /// False - пропустить
        /// </summary>
        event ErrorLoadPageEventHandler LoadPageError;
        /// <summary>
        /// Событие срабатывает при обновлении статуса парсинга
        /// </summary>
        event ParserStatusUpdatedEventHandler ParserStatusUpdated;
        /// <summary>
        /// Срабатывает при изменении статуса базы данных
        /// </summary>
        event EventHandler StatusDatabaseChanged;
        /// <summary>
        /// Срабатывает при изменении статуса несортированных данных 
        /// </summary>
        event EventHandler StatusNotSortedDataChanged;

        /// <summary>
        /// Обновить несортированные данные
        /// </summary>
        /// <param name="token">Токен отмены</param>
        void UppdateNotSortedData(CancellationToken token);

        /// <summary>
        /// Парсит одну страницу и заносит в NotSortedMatches
        /// </summary>
        void ParsePage(CancellationToken token);

        /// <summary>
        /// Парсит все страницы и заносит в NotSortedMatches
        /// </summary>
        /// <param name="token">Токен отмены</param>
        void Parse(CancellationToken token);

        /// <summary>
        /// Парсит все страницы многопоточно
        /// </summary>
        /// <param name="token">Токен отмены</param>
        void ParseMultyThread(CancellationToken token);

        /// <summary>
        /// Загрузить базу данных из файла
        /// </summary>
        /// <param name="path">Путь к файлу база данных</param>
        void LoadDatabase(string path);

        /// <summary>
        /// Загрузить не сортированные данные из файла
        /// </summary>
        /// <param name="path">Путь к файлу NotSortedData</param>
        void LoadNotSortedData(string path);

        /// <summary>
        /// Сохраняет несортированные данные
        /// </summary>
        /// <param name="path">Расположение, в котором нужно сохранить файл</param>
        void SaveNotSortedData(string path);

        /// <summary>
        /// Сохраняет базу даных
        /// </summary>
        /// <param name="path">Расположение, в котором нужно сохранить данные</param>
        void SaveDatabase(string path);

        /// <summary>
        /// Аназил несортированных даанных
        /// </summary>
        void AnalysisNotSortedData();

        /// <summary>
        /// Получить результаты матчей
        /// </summary>
        /// <param name="urlsLeftGamers">Ссылки на игроков левой команды</param>
        /// <param name="urlsRightGamers">Ссылки на игроков правой команды</param>
        /// <returns>Шанс для левой команды и шанс ддля правой команды</returns>
        (double Left, double Right) GetResultMatch(string[] urlsLeftGamers, string[] urlsRightGamers);
    }
}
