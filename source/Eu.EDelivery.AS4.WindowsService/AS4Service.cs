using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Eu.EDelivery.AS4.Common;
using Eu.EDelivery.AS4.ServiceHandler;


namespace Eu.EDelivery.AS4.WindowsService
{
    public class AS4Service
    {
        private readonly ILogger<AS4Service> _logger;

        private Kernel _kernel;
        private Task _rootTask, _feTask, _payloadServiceTask;
        private CancellationTokenSource _cancellation;


        /// <summary>
        /// Initializes a new instance of the <see cref="AS4Service"/> class.
        /// </summary>
        public AS4Service(ILogger<AS4Service> logger)
        {
            _logger = logger;
        }


        public void Start()
        {
            _logger.LogInformation("Starting AS4.NET Component Service");

            try
            {
                string assemblyLocationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (string.Compare(Environment.CurrentDirectory, assemblyLocationFolder, StringComparison.OrdinalIgnoreCase) != 0
                    && assemblyLocationFolder != null)
                {
                    Environment.CurrentDirectory = assemblyLocationFolder;
                }

                try
                {
                    _kernel = Kernel.CreateFromSettings(@"config\settings-service.xml");
                }
                catch (Exception ex)
                {
                    _logger.LogError("AS4.NET Component cannot be initialized");
                    _logger.LogError(ex.ToString());

                    Stop();

                    return;
                }

                _cancellation = new CancellationTokenSource();
                _rootTask = _kernel.StartAsync(_cancellation.Token);

                _feTask = StartFeInProcess(_cancellation.Token);
                _payloadServiceTask = StartPayloadServiceInProcess(_cancellation.Token);

                _logger.LogInformation("AS4.NET Component Service is started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }


        private Task StartFeInProcess(CancellationToken cancellation)
        {
            if (!Config.Instance.FeInProcess)
            {
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(() => Fe.Program.StartInProcess(cancellation), cancellation);
            task.ContinueWith(LogExceptions, TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }


        private Task StartPayloadServiceInProcess(CancellationToken cancellation)
        {
            if (!Config.Instance.PayloadServiceInProcess)
            {
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(() => PayloadService.Program.Start(cancellation), cancellation);
            task.ContinueWith(LogExceptions, TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }


        private void LogExceptions(Task t)
        {
            if (t.Exception?.InnerExceptions == null)
            {
                return;
            }

            foreach (Exception ex in t.Exception.InnerExceptions)
            {
                _logger.LogError(ex.Message);
            }
        }


        public void Stop()
        {
            _logger.LogInformation("Stopping AS4.NET Component Service");

            try
            {
                _cancellation?.Cancel();

                StopTask(_rootTask);
                StopTask(_feTask);
                StopTask(_payloadServiceTask);

                _kernel?.Dispose();
                Config.Instance.Dispose();

                _logger.LogInformation("AS4.NET Component Service is stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }


        private static void StopTask(Task task)
        {
            if (task == null) { return; }

            try
            {
                task.GetAwaiter().GetResult();
            }
            catch (AggregateException exception)
            {
                exception.Handle(e => e is TaskCanceledException);
            }
            finally
            {
                task.Dispose();
            }
        }

    }
}
