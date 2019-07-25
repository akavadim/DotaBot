using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Linq;

namespace DotaBot.BL.Net
{
    /// <summary>
    /// Класс для работы с страницами игр по доте cybersport.ru 
    /// </summary>
    sealed class DotaPageWorker : PageWorker
    {
        /// <summary>
        /// Сайт с которым работает данный класс
        /// </summary>
        private readonly string Site = "https://www.cybersport.ru";
        /// <summary>
        /// Текущая страница
        /// </summary>
        private int _pageNumber;
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int PageNumber
        {
            get
            {
                if (!IsSupported())
                    throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");

                string res = Regex.Match(URL, @"page=(\d+)").Value.Replace("page=", "");    //TODO: проверить работоспособность регулярного выражения
                int number = int.Parse(res);
                return number;
            }
            set
            {
                if (!IsSupported())
                    throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");

                _pageNumber = value;
                URL = Site + "/base/match?disciplines=21&status=past&page=" + _pageNumber;

                if (!IsExists())
                    throw new PageNotExistException("Страница \"" + URL + "\" не существует");
                if (!IsSupported())
                    throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");
            }
        }
        /// <summary>
        /// Следующая страница
        /// </summary>
        public string NextPage
        {
            get
            {
                if (!IsSupported())
                    throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");

                string urlNextPage = Page.DocumentNode.SelectSingleNode("//a[@class='pagination__item pagination__item--next']")?.Attributes["href"].Value;
                if (urlNextPage == null)
                    return null;
                return Site + urlNextPage;
            }
        }
        /// <summary>
        /// Предыдущая страница
        /// </summary>
        public string PreviousPage
        {
            get
            {
                if (!IsSupported())
                    throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");

                string urlPrevPage = Page.DocumentNode.SelectSingleNode("//a[@class='pagination__item pagination__item--prev']")?.Attributes["href"].Value;
                if (urlPrevPage == null)
                    return null;
                return Site + urlPrevPage;
            }
        }

        /// <summary>
        /// Создать класс для работы с ссылкой
        /// </summary>
        /// <param name="URL">ссылка на сайт</param>
        public DotaPageWorker(string URL) : base(URL) { }

        /// <summary>
        /// Создает экземпляр класса для работы с страницей cybersport.ru/base/match?disciplines=21status=pastpage=1
        /// </summary>
        public DotaPageWorker() : base("https://www.cybersport.ru/base/match?disciplines=21&status=past&page=1") { }


        /// <summary>
        /// Получить ссылки на матчи 
        /// </summary>
        /// <returns>Ссылки на все матчи на странице</returns>
        public string[] GetMatchesURLs()
        {
            if (!IsSupported())
                throw new PageNotSupportedException("Страница \"" + URL + "\" не поддердживается");

            List<string> results = new List<string>();
            var nodes = Page.DocumentNode.SelectNodes("//div[@class='matche__score']").ToArray();
            foreach (var node in nodes)
                results.Add(Site + node.ChildNodes["a"].Attributes["href"].Value);
            return results.ToArray();
        }

        /// <summary>
        /// Проверяет, поддерживает ли этот класс работу с URL
        /// </summary>
        /// <returns>True - поддерживает, False - не поддерживает</returns>
        public override bool IsSupported()
        {
            bool supported = false;
            bool workedURL = Regex.IsMatch(URL, @"page=(\d+)") && URL.Contains("disciplines=21") && URL.Contains("status=past");
            bool neededComponents = (Page.DocumentNode.SelectNodes("//a[@class='matches__list list-unstyled']") != null)
                && (Page.DocumentNode.SelectNodes("//a[@class='matches__item']") != null);

            if (base.IsSupported() && workedURL && neededComponents)
                supported = true;

            return supported;
        }
    }
}
