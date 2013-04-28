using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

using frontend.Models;
using CompilerRunner;

namespace compiler_worker
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        private const string QueueName = "ProcessingQueue";
        private ICompiler Compiler;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private QueueClient Client;
        private bool IsStopped;
        private CodeContext db = new CodeContext();

        public override void Run()
        {
            while (!IsStopped)
            {
                try
                {
                    // Receive the message
                    BrokeredMessage receivedMessage = null;
                    receivedMessage = Client.Receive();

                    if (receivedMessage != null)
                    {
                        // Process the message
                        try
                        {
                            Trace.WriteLine("**** Processing: " + receivedMessage.SequenceNumber.ToString(), "WORKER");

                            CodeSnippet code = receivedMessage.GetBody<CodeSnippet>();

                            if (Compiler != null)
                            {
                                Thread.Sleep(10000);
                                Task<CompileResult> ResultTask =
                                    Compiler.Compile(new CompileRequest
                                    {
                                        Code = code.Code
                                    });
                                ResultTask.Wait();
                                CompileResult result = ResultTask.Result;

                                CompilerOutput output = new CompilerOutput
                                {
                                    Successful = result.Successful,
                                    ExitCode = result.ExitCode,
                                    StdOut = result.StdOut,
                                    StdErr = result.StdErr,
                                    CompileTime = result.CompileTime,
                                    MessagesJson = JsonConvert.SerializeObject(result.Messages)
                                };

                                db.CompilerOutputs.Add(output);
                                code.Result = output;

                                db.Entry(code).State = EntityState.Modified;
                                db.SaveChanges();

                                Trace.WriteLine("  <<<< Result: " + result.Successful.ToString(), "WORKER");
                            }

                            receivedMessage.Complete();
                        }
                        catch (Exception ex)
                        {
                            if (receivedMessage.DeliveryCount > 1)
                            {
                                var properties = new Dictionary<string, object>();
                                properties.Add("Reason for deadlettering", "Uncaught Exception");
                                properties.Add("Exception", ex.ToString());
                                receivedMessage.DeadLetter(properties);
                            }
                        }
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }

                    Thread.Sleep(10000);
                }
                catch (OperationCanceledException e)
                {
                    if (!IsStopped)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                }
            }
        }

        public override bool OnStart()
        {
            if (RoleEnvironment.IsEmulated)
                AppDomain.CurrentDomain.SetData("DataDirectory", @"C:\Users\Mikael\DEV\compiler\frontend\App_Data");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //
            // setup compilers
            //

            // initialize the local cache for the Azure drive
            LocalResource cache = RoleEnvironment.GetLocalResource("LocalDriveCache");
            CloudDrive.InitializeCache(cache.RootPath, cache.MaximumSizeInMegabytes);

            // retrieve storage account
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                configSetter(CloudConfigurationManager.GetSetting(configName)));
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("Microsoft.Storage.ConnectionString");
            string driveBlobUri = "compilers/mingw64-1.vhd";

            // unmount any previously mounted drive.
            foreach (var drive in CloudDrive.GetMountedDrives())
            {
                var mountedDrive = account.CreateCloudDrive(drive.Value.PathAndQuery);
                mountedDrive.Unmount();
            }

            // create the Windows Azure drive and its associated page blob
            CloudDrive Drive = account.CreateCloudDrive(driveBlobUri);
            Drive.CreateIfNotExist(20);
            string path = Drive.Mount(cache.MaximumSizeInMegabytes, DriveMountOptions.Force);

            Compiler = GccCompilerFinder.DetectAtPath(Path.Combine(path, "bin\\x86_64-w64-mingw32-g++.exe")).Result;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            IsStopped = false;
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
            Client.Close();
            base.OnStop();
        }
    }
}
