using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace DotaBot.BL.Net
{
    abstract class PageWorker : IPageWorker
    {
        /// <summary>
        /// Текущий URL
        /// </summary>
        private string _url;
        /// <summary>
        /// Страница, если использовался LoadPage()
        /// </summary>
        private HtmlDocument _page;
        /// <summary>
        /// URL страницы, с которой работает класс
        /// </summary>
        public string URL
        {
            get { return _url; }
            set { _page = null; _url = value; }
        }
        /// <summary>
        /// Текущая страница, если оона была загружена с помощью LoadPage
        /// </summary>
        public HtmlDocument Page
        {
            get => _page;
        }

        /// <summary>
        /// Событие перед LoadPage
        /// </summary>
        public event LoadPageEventHandler BeforeLoadPage;
        /// <summary>
        /// Событие после LoadPage
        /// </summary>
        public event LoadPageEventHandler AfterLoadPage;

        /// <summary>
        /// Создать класс для работы с ссылкой
        /// </summary>
        /// <param name="URL">ссылка на сайт</param>
        public PageWorker(string URL) => this.URL = URL;

        /// <summary>
        /// Загружает страницу
        /// </summary>
        /// <param name="url">Ссылка на страницу</param>
        public virtual void LoadPage()
        {
            string url = URL;

            BeforeLoadPage?.Invoke(this, url);

            if (!IsExists())
                throw new WebException("Страница не существует");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            if (response.CharacterSet == null)
            {
                readStream = new StreamReader(receiveStream);
            }
            else
            {
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(readStream);

            response.Close();
            readStream.Close();

            AfterLoadPage?.Invoke(this, url);

            _page=htmlDocument;
        }

        /// <summary>
        /// Проверяет, возможно ли загрузить URL
        /// </summary>
        /// <returns>True - возможно, False - не возможно</returns>
        public virtual bool IsExists()
        {
            if (_page != null)
                return true;

            WebRequest webRequest = HttpWebRequest.Create(URL);
            webRequest.Method = "HEAD";
            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }   //TODO: Загрузка с парметром null, не ссылкой, нужно проверить.

        /// <summary>
        /// Проверяет, поддерживает ли этот класс работу с URL
        /// </summary>
        /// <returns>True - поддерживает, False - не поддерживает</returns>
        public virtual bool IsSupported()
        {
            return IsExists();
        }
    }
}
