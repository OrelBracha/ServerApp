using System;
using System.Threading.Tasks;
using ServerApp.Services;

namespace BatchServiceTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var batchService = new UserBatchService();

            
            Console.WriteLine("Starting batch processing...");
            await batchService.ProcessBatch();
            Console.WriteLine("Batch processing completed.");

            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
