using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace DotaBot.BL.Net
{
    /// <summary>
    /// Класс для работы с страницами игр по доте cybersport.ru 
    /// </summary>
    class DotaPageWorker : PageWorker
    {
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Следующая страница
        /// </summary>
        public DotaPageWorker NextPage { get; }   //TODO: Доделать следующую страаницу
        /// <summary>
        /// Предыдущая страница
        /// </summary>
        public DotaPageWorker PreviousPage { get; }   //TODO: Доделать предыдущую страницу

        /// <summary>
        /// Создать класс для работы с ссылкой
        /// </summary>
        /// <param name="URL">ссылка на сайт</param>
        public DotaPageWorker(string URL) : base(URL) { }

        /// <summary>
        /// Получить ссылки на матчи 
        /// </summary>
        /// <returns>Ссылки на все матчи на странице</returns>
        public string[] GetMatchesURLs() { throw new NotImplementedException(); }    //TODO: Доделать получение ссылок на матчи, проверить поддерживается ли эта страница

        /// <summary>
        /// Проверяет, поддерживает ли этот класс работу с URL
        /// </summary>
        /// <returns>True - поддерживает, False - не поддерживает</returns>
        public override bool IsSupported()  //TODO: Сделать проверку поддерживается ли эта страница
        {
            return base.IsSupported();
        }
    }
}
