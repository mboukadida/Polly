using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static Policy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(10));

        static void Main(string[] args)
        {
            OperationWithBasicRetryAsync().Wait();
        }

        public static async Task OperationWithBasicRetryAsync()
		{
            try
            {
                await policy.ExecuteAsync(() => TransientOperationAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task TransientOperationAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = await client.GetAsync("api/users");
        }
    }
}
