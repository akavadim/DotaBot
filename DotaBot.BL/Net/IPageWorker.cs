﻿using System;

namespace DotaBot.BL.Net
{
    interface IPageWorker
    {
        /// <summary>
        /// URL страницы, с которой работает класс
        /// </summary>
        string URL { get; set; }
        /// <summary>
        /// Текущая страница 
        /// </summary>
        HtmlAgilityPack.HtmlDocument Page { get; }

        /// <summary>
        /// Событие перед LoadPage
        /// </summary>
        event LoadPageEventHandler BeforeLoadPage;
        /// <summary>
        /// Событие после LoadPage
        /// </summary>
        event LoadPageEventHandler AfterLoadPage;

        /// <summary>
        /// Загружает страницу
        /// </summary>
        void LoadPage();
        /// <summary>
        /// Проверяет, возможно ли загрузить страницу
        /// </summary>
        /// <returns>True - возможно, False - не возможно</returns>
        bool IsExists();
        /// <summary>
        /// Проверяет, поддерживает ли этот класс работу с этой страницей
        /// </summary>
        /// <returns>True - поддерживает, False - не поддерживает</returns>
        bool IsSupported();
    }
}
