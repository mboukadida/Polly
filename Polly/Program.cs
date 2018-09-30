using Polly.Fallback;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static Policy<HttpResponseMessage> waitAndRetryPolicy = Policy<HttpResponseMessage>
            .Handle<AggregateException>()
            .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        private static Policy<HttpResponseMessage> circuitBreakerPolicy = Policy<HttpResponseMessage>
            .Handle<AggregateException>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(5));

        private static FallbackPolicy<HttpResponseMessage> fallBackPolicy = FallbackPolicy<HttpResponseMessage>
            .Handle<Exception>()
            .Fallback(
                fallbackAction: () =>
                {
                    Console.WriteLine("Please try again later [Fallback for any exception]");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed);
                },
                onFallback: b =>
                {
                    ConsoleHelper.WriteLineInColor("Fallback catches failed with: " + b.Exception, ConsoleColor.Red);
                    Console.ReadLine();
                });

        private static PolicyWrap<HttpResponseMessage> myStrategy = fallBackPolicy.Wrap(circuitBreakerPolicy.Wrap(waitAndRetryPolicy));

        static void Main(string[] args)
        {
            OperationWithBasicRetryAsync();
        }

        public static Task OperationWithBasicRetryAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = myStrategy.Execute(() => client.GetAsync(("api/users")).Result);
            return Task.FromResult(0);
        }
    }
}