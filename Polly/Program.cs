using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static Policy waitAndRetryPolicy = Policy
            .Handle<AggregateException>()
            .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        private static Policy circuitBreakerPolicy = Policy
            .Handle<AggregateException>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(5));

        private static PolicyWrap myStrategy = Policy.Wrap(waitAndRetryPolicy, circuitBreakerPolicy);

        static void Main(string[] args)
        {
            OperationWithBasicRetryAsync();
        }

        public static Task OperationWithBasicRetryAsync()
        {
            try
            {
                HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
                var response = myStrategy.Execute(() => client.GetAsync(("api/users")).Result);
            }
            catch (BrokenCircuitException exception)
            {
                ConsoleHelper.WriteLineInColor($"A circuit breaker exception occured" + exception, ConsoleColor.Green);
                Console.ReadLine();
            }

            return Task.FromResult(0);
        }
    }
}