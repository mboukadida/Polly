using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        static void Main(string[] args)
        {
            WaitAndRetry.OperationWithBasicRetryAsync().Wait();
        }
    }
}
