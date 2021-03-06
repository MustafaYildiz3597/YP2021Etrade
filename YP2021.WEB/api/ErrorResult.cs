using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Nero2021.api
{
    public class ErrorResult : IHttpActionResult
    {
        Error _error;
        HttpRequestMessage _request;

        public ErrorResult(Error error, HttpRequestMessage request)
        {
            _error = error;
            _request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<Error>(_error, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }

    public class Error
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ErrorData> data { get; set; }
    }

    public class ErrorData
    {
        public string Text { get; set; }
        public string Description { get; set; }
    }

}