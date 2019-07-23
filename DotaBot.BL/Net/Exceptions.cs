using System;
using System.Collections.Generic;
using System.Text;

namespace DotaBot.BL.Net
{
    /// <summary>
    /// Страница не подддерживается классом
    /// </summary>
    class PageNotSupportedException : ApplicationException
    {
        public PageNotSupportedException() : base() { }
        public PageNotSupportedException(string message) : base(message) { }
    }
    /// <summary>
    /// Страница не существует
    /// </summary>
    class PageNotExistException : ApplicationException
    {
        public PageNotExistException() : base() { }
        public PageNotExistException(string message) : base(message) { }
    }
}
