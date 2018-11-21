using Polly.Fallback;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    public class Wrap_Fallback_WaitAndRetry_CircuitBreaker
    {
        private static readonly Policy waitAndRetryPolicy = Policy
           .Handle<Exception>()
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2)); //TimeSpan.FromSeconds(5)

        private static readonly Policy circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(5));

        private static readonly FallbackPolicy<HttpResponseMessage> fallBackPolicy = Policy<HttpResponseMessage>
            .Handle<Exception>()
            .FallbackAsync(
               fallbackAction: async ct =>
               {
                   Console.WriteLine("Please try again later [Fallback for any exception]");
                   return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed);

               },
               onFallbackAsync: async e =>
               {
                   ConsoleHelper.WriteInColor("Fallback catches eventually failed with: " + e.Exception.Message, ConsoleColor.Red);
                   Console.ReadLine();
               }
            );

        private static PolicyWrap<HttpResponseMessage> policy = fallBackPolicy
            .WrapAsync(circuitBreakerPolicy.WrapAsync(waitAndRetryPolicy));

        public static async Task OperationWithWrappedPolicyAsync()
        {
            var result = await policy.ExecuteAsync(() => TransientOperationAsync());
        }

        public static async Task<HttpResponseMessage> TransientOperationAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            return await client.GetAsync("api/users");
        }
    }
}
