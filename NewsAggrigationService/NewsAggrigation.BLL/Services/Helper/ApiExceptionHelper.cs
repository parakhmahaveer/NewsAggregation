using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggrigation.BLL.Services.Helper
{
    public class ApiExceptionHelper : Exception
    {
        public int StatusCode { get; }

        /// <summary>
        /// Creates a new instance of ApiException with a message and optional HTTP status code (defaults to 400).
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">HTTP status code to return (default is 400 - Bad Request).</param>
        public ApiExceptionHelper(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Creates a new instance of ApiException with a formatted message and status code.
        /// </summary>
        /// <param name="format">Message format string.</param>
        /// <param name="statusCode">HTTP status code to return.</param>
        /// <param name="args">Format arguments.</param>
        public ApiExceptionHelper(string format, int statusCode, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, format, args))
        {
            StatusCode = statusCode;
        }
    }
}
