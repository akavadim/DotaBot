using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.PS.IViews
{
    /// <summary>
    /// *Iview* Выводит данные
    /// </summary>
    public interface IMainWindow
    {
        /// <summary>
        /// Статус базы данных
        /// </summary>
        string DatabaseStatus { get; set; }
        /// <summary>
        /// Ссылки на игроков левой команды
        /// </summary>
        string[] LeftGamersUrls { get; }
        /// <summary>
        /// Статус несортированной базы данных
        /// </summary>
        string NotSortedDataStatus { get; set; }
        /// <summary>
        /// Путь к базе данных
        /// </summary>
        string PathDatabase { get; }
        /// <summary>
        /// Путь к несортированным данным
        /// </summary>
        string PathNotSortedData { get; }
        /// <summary>
        /// Ссылки на игроков правой команды
        /// </summary>
        string[] RightGamersUrls { get; }

        /// <summary>
        /// Запрос на анадиз несортированных данных
        /// </summary>
        event EventHandler AnalizDataClick;
        /// <summary>
        /// Запрос на загрузку базы Данных
        /// </summary>
        event EventHandler LoadFileDatabaseClick;
        /// <summary>
        /// Запрос на загрузку данных
        /// </summary>
        event EventHandler LoadFileNotSortedDataClick;
        /// <summary>
        /// Запрос на сохранение несортированных данных
        /// </summary>
        event EventHandler SaveFileNotSortedDataClick;
        /// <summary>
        /// Запрос на сохранение базы данных
        /// </summary>
        event EventHandler SaveFileDatabaseClick;
        /// <summary>
        /// Запрос однопоточного парсинга
        /// </summary>
        event ParseEventHandler ParseClick;
        /// <summary>
        /// Запрос многопоточного парсинга
        /// </summary>
        event ParseEventHandler ParseMultyThreadClick;
        /// <summary>
        /// Запрос парсинга одной страницы
        /// </summary>
        event ParseEventHandler ParsePageClick;
        /// <summary>
        /// Запрос на рассчет результатов матча
        /// </summary>
        event EventHandler ResultMatchClick;
        /// <summary>
        /// Запрос на обновление несортированных данных
        /// </summary>
        event ParseEventHandler UpdateNotSortedDataClick;
    }
}
