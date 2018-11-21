using Polly.CircuitBreaker;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Polly
{
    public static class Wrap_WaitAndRetry_CircuitBreaker
    {
        private static readonly Policy waitAndRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2)); //TimeSpan.FromSeconds(5)

        private static readonly Policy circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking: 3, 
                durationOfBreak: TimeSpan.FromSeconds(5));

        private static PolicyWrap policy = Policy.WrapAsync(waitAndRetryPolicy, circuitBreakerPolicy);

        public static async Task OperationWithWrappedPolicyAsync()
        {
            try
            {
                await policy.ExecuteAsync(() => TransientOperationAsync());
            }
            catch (BrokenCircuitException exception)
            {
                ConsoleHelper.WriteLineInColor($"A circuit breaker exception occured" + exception, ConsoleColor.Green);
                Console.ReadLine();
            }
        }

        public static async Task TransientOperationAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = await client.GetAsync("api/users");
        }
    }
}
