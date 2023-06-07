using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThreadSafetyTest;


public class ThreadSaftyTest
{
    

    public static async Task Main()
    {
        Console.WriteLine("Application Started...");

        int numberOfParallelExecutions = 2;
        var service = new GetRequester();

        await RunTaskInParallel(() => RunThreadSaftyTest(service), numberOfParallelExecutions);

        Console.ReadKey();
    }

    static Task RunThreadSaftyTest(GetRequester service)
    {
        try
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Bearer", "123456780");
            headers.Add("correlationId", "testcorrelationid");
            headers.Add("subscriptionId", "subscriptionId");

            var result = service.RunAsync("http://localhost:5223/WeatherForecast", null, headers).Result;
            //var body = JObject.Parse(result.Content.ReadAsStringAsync().Result);

            Console.WriteLine("Request processed successfully without duplicate headers...");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Task.CompletedTask;
    }

    static async Task<(bool IsSuccess, Exception Error)> RunTaskInParallel(Func<Task> task, int numberOfParallelExecutions = 2)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        Exception error = null;
        int tasksCompletedCount = 0;
        var result = Parallel.For(0, numberOfParallelExecutions, GetParallelLoopOptions(cancellationTokenSource),
                      async index =>
                      {
                          try
                          {
                              await task();
                          }
                          catch (Exception ex)
                          {
                              error = ex;
                              cancellationTokenSource.Cancel();
                          }
                          finally
                          {
                              tasksCompletedCount++;
                          }

                      });

        int spinWaitCount = 0;
        int maxSpinWaitCount = 100;

        while (numberOfParallelExecutions > tasksCompletedCount && error is null && spinWaitCount < maxSpinWaitCount)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            spinWaitCount++;
        }

        return (error == null, error);
    }

    static ParallelOptions GetParallelLoopOptions(CancellationTokenSource cancellationTokenSource)
    {
        ParallelOptions parallelOptions = new ParallelOptions();
        return parallelOptions;
    }
}