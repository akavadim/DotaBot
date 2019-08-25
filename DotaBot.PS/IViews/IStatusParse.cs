using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.PS.IViews
{
    /// <summary>
    /// Интерфейс для отображения статуса парсинга
    /// </summary>
    public interface IStatusParse
    {
        /// <summary>
        /// Количество загруженных и обработанных матчей
        /// </summary>
        string MatchesLoaded { get; set; }
        /// <summary>
        /// Количество загруженных и обработанных страниц с матчами
        /// </summary>
        string PagesOfMatchesLoaded { get; set; }
        /// <summary>
        /// Всего загружено и обработано страниц
        /// </summary>
        string TotalLoaded { get; set; }
        /// <summary>
        /// Обязательно! Установить в True после завершения парсинга
        /// </summary>
        bool ParseEnded { get; set; }

        /// <summary>
        /// Событие запрашивающее отмену парсинга
        /// </summary>
        event EventHandler CancelParseClick;
    }
}
