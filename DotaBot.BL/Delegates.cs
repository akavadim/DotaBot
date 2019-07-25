using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.BL
{
    /// <summary>
    /// В этом файле все делегаты из решения и класс EventArgs для них
    /// </summary>

    /// <summary>
    /// Делегат обработчика события возвращающий True или False
    /// </summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="e">EventArgs</param>
    /// <returns>True или False</returns>
    public delegate bool ErrorLoadPageEventHandler(object sender, PageInfoArgs e);
    /// <summary>
    /// Делегат обработчика события обновления статуса парсера
    /// </summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="args">Аргумент ParserStatus</param>
    public delegate void ParserStatusUpdatedEventHandler(object sender, ParserStatusArgs args);
    /// <summary>
    /// Делегат обработчика события LoadPage
    /// </summary>
    /// <param name="sender">Отправитель</param>
    /// <param name="url">Страница, с которой ведется работа</param>
    delegate void LoadPageEventHandler(object sender, string url);

    /// <summary>
    /// Класс предоставляет информацию о статусе парсера
    /// </summary>
    public class ParserStatusArgs : EventArgs
    {
        /// <summary>
        /// Количество загруженных и обработанных страниц с матчами
        /// </summary>
        public int PagesOfMatchesLoaded { get; set; }
        /// <summary>
        /// Количество загруженных и обработанных матчей
        /// </summary>
        public int MatchesLoaded { get; set; }
        /// <summary>
        /// Всего загружено и обработано страниц (Сумма MatchesLoaded и PagesOfMatchesLoaded)
        /// </summary>
        public int TotalLoaded { get => PagesOfMatchesLoaded + MatchesLoaded; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="matchesLoaded">Количество загруженных и обработанных матчей</param>
        /// <param name="pagesOfMatchesLoaded">Количество загруженныых и обработанных страниц с матчами</param>
        public ParserStatusArgs(int matchesLoaded, int pagesOfMatchesLoaded)
        {
            PagesOfMatchesLoaded = pagesOfMatchesLoaded;
            MatchesLoaded = matchesLoaded;
        }
        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public ParserStatusArgs() { }
    }

    /// <summary>
    /// Класс для информации о странице
    /// </summary>
    public class PageInfoArgs : EventArgs
    {
        public string URL { get; set; }
        public string Message { get; set; }

        public PageInfoArgs(string message, string URL)
        {
            Message = message;
            this.URL = URL;
        }
        public PageInfoArgs(string message) : this(message, null) { }
        public PageInfoArgs() { }
    }
}
