using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static Policy policy = Policy
            .Handle<AggregateException>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        static void Main(string[] args)
        {
            try
            {
                OperationWithBasicRetryAsync();
            }
            catch (AggregateException exception)
            {
                ConsoleHelper.WriteLineInColor($"An aggregate exception occured {exception}", ConsoleColor.Blue);
                Console.ReadLine();
            }
        }

        public static Task OperationWithBasicRetryAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = policy.Execute(() => client.GetAsync(("api/users")).Result);
            return Task.FromResult(0);
        }
    }
}
