using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eu.EDelivery.AS4.Common;
using NLog;

namespace Eu.EDelivery.AS4.ServiceHandler.ConsoleHost
{
    public class Program
    {
        public static void Main()
        {
            //Console.SetWindowSize(Console.LargestWindowWidth, Console.WindowHeight);

            // https://stackoverflow.com/questions/10345240/c-sharp-set-probing-privatepath-without-app-config
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            ShowHelp();

            Kernel kernel = CreateKernel();
            if (kernel == null)
            {
                Console.ReadLine();
                return;
            }

            try
            {
                var lifecycle = new AS4ComponentLifecycle(kernel);
                lifecycle.Start();

                ConsoleKeyInfo key;

                do
                {
                    key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.C:
                            Console.Clear();
                            ShowHelp();
                            break;
                        case ConsoleKey.R:
                            Console.WriteLine();
                            Console.WriteLine("Restarting...");
                            lifecycle.Stop();

                            kernel.Dispose();
                            Config.Instance.Dispose();

                            kernel = CreateKernel();
                            lifecycle = new AS4ComponentLifecycle(kernel);
                            lifecycle.Start();
                            Console.WriteLine("Restarted.");
                            break;
                    }
                } while (key.Key != ConsoleKey.Q);

                Console.WriteLine();
                Console.WriteLine("Stopping...");
                Task task = lifecycle.Stop();

                Console.WriteLine($@"Stopped: {task.Status}");

                if (task.IsFaulted && task.Exception != null)
                {
                    Console.WriteLine(task.Exception.ToString());
                    Console.WriteLine("Press enter to terminate ...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Fatal(ex);
            }
            finally
            {
                kernel?.Dispose();
                Config.Instance.Dispose();
            }

            Console.ReadLine();
        }


        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //Console.WriteLine("Current dir: {0}", Directory.GetCurrentDirectory());

            var probingPath = Directory.GetCurrentDirectory(); //"bin";
            var assyName = new AssemblyName(args.Name);

            var newPath = Path.Combine(probingPath, assyName.Name);
            if (!newPath.EndsWith(".dll"))
            {
                newPath += ".dll";
            }

            Console.WriteLine("Resolving: {0} ({1})", newPath, File.Exists(newPath));

            return File.Exists(newPath)
                ? Assembly.LoadFile(newPath)
                : null;
        }


        private static void ShowHelp()
        {
            WriteLine("\nAS4.NET v" + Assembly.GetExecutingAssembly().GetName().Version + "\n"
                      + "\nThe following commands are available while the AS4.NET MSH is running:"
                      + "\n c\tClears the screen"
                      + "\n q\tQuits the application"
                      + "\n r\tRestarts the application"
                      + "\n");
        }

        private static void WriteLine(string msg)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
            Console.ForegroundColor = temp;
        }

        private static Kernel CreateKernel()
        {
            try
            {
                return Kernel.CreateFromSettings(Path.Combine("config", "settings.xml"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private sealed class AS4ComponentLifecycle
        {
            private readonly CancellationTokenSource _cancellation;

            private readonly Kernel _kernel;
            private Task _kernelTask, _frontendTask, _payloadServiceTask;


            /// <summary>
            /// Initializes a new instance of the <see cref="AS4ComponentLifecycle" /> class.
            /// </summary>
            /// <param name="kernel">The kernel.</param>
            public AS4ComponentLifecycle(Kernel kernel)
            {
                _kernel = kernel;
                _cancellation = new CancellationTokenSource();

            }

            /// <summary>
            /// Starts this instance.
            /// </summary>
            public void Start()
            {
                _kernelTask = StartKernel();
                _frontendTask = StartFeInProcess(_cancellation.Token);
                _payloadServiceTask = StartPayloadServiceInProcess(_cancellation.Token);
            }

            private Task StartKernel()
            {
                Task task = _kernel.StartAsync(_cancellation.Token);

                task.ContinueWith(
                    x =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        LogManager.GetCurrentClassLogger().Fatal(x.Exception?.ToString());
                    },
                    TaskContinuationOptions.OnlyOnFaulted);

                return task;
            }

            private static Task StartFeInProcess(CancellationToken cancellationToken)
            {
                if (!Config.Instance.FeInProcess)
                {
                    return Task.CompletedTask;
                }

                Task task = Task.Factory
                    .StartNew(() => Fe.Program.StartInProcess(cancellationToken), cancellationToken);
                task.ContinueWith(LogExceptions, TaskContinuationOptions.OnlyOnFaulted);

                return task;
            }

            private static Task StartPayloadServiceInProcess(CancellationToken cancellationToken)
            {
                if (!Config.Instance.PayloadServiceInProcess)
                {
                    return Task.CompletedTask;
                }

                Task task = Task.Factory.StartNew(() => PayloadService.Program.Start(cancellationToken), cancellationToken);
                task.ContinueWith(LogExceptions, TaskContinuationOptions.OnlyOnFaulted);

                return task;
            }

            private static void LogExceptions(Task task)
            {
                if (task.Exception?.InnerExceptions == null)
                {
                    return;
                }

                foreach (Exception ex in task.Exception?.InnerExceptions)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(ex);
                }
            }

            /// <summary>
            /// Stops this instance.
            /// </summary>
            /// <returns></returns>
            public Task Stop()
            {
                _cancellation.Cancel();

                StopTask(_kernelTask);
                StopTask(_frontendTask);
                StopTask(_payloadServiceTask);

                return _kernelTask;
            }

            private static void StopTask(Task task)
            {
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
}
