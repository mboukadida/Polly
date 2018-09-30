using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static int retryCount = 3;
        private static readonly TimeSpan delay = TimeSpan.FromSeconds(5);

        static void Main(string[] args)
        {
            OperationWithBasicRetryAsync();
        }

        public static Task OperationWithBasicRetryAsync()
        {
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
                    var result = client.GetAsync("api/users").Result;
                    break;
                }
                catch (AggregateException ex)
                {
                    Trace.TraceError("Operation Exception");

                    currentRetry++;
                    if (currentRetry > retryCount)
                    {
                        throw;
                    }
                }

                Task.Delay(delay);
            }

            return Task.FromResult(0);
        }
    }
}
