using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Web.Administration;
using Website2.Hubs;

namespace Website2.Actors
{
    /// <summary>
    /// Actor used to wrap a signalr hub
    /// </summary>
    public class ClusterServiceStatusActor : ReceiveActor
    {
        #region Messages

        public class GetServiceStatus
        {
        }

        public class Cancel
        {
        }

        public class Finished
        {
            public Finished(IReadOnlyList<ServiceModel> services, Exception exception)
            {
                Services = services;
                Exception = exception;
            }

            public IReadOnlyList<ServiceModel> Services { get; private set; }
            public Exception Exception { get; private set; }
        }

        public class ServiceModel
        {
            public ServiceModel(string type, string serviceName, string machineName, string status)
            {
                Type = type;
                ServiceName = serviceName;
                MachineName = machineName;
                Status = status;
            }

            public string Type { get; private set; }
            public string ServiceName { get; private set; }
            public string MachineName { get; private set; }
            public string Status { get; set; }
        }

        #endregion

        private ServiceStatusHub _hub;
        protected ICancelable ClusterServiceStatusTeller;
        protected Cluster Cluster = Cluster.Get(Context.System);

        private CancellationTokenSource _cancel;
        public ClusterServiceStatusActor()
        {
            _cancel = new CancellationTokenSource();
            ClusterServiceStatusTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(1), Self, new GetServiceStatus(), Self);

            Ready();
        }

        public void Ready()
        {
            Receive<GetServiceStatus>(ic =>
            {
                var self = Self;
                Task.Run(() =>
                {
                    List<ServiceModel> services = new List<ServiceModel>();
                    services.Add(new ServiceModel("Service", "Lighthouse", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Service", "Lighthouse2", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Service", "Worker", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Service", "Worker2", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Service", "WorkerWithWebApi", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Service", "Tasker", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("Website", "Default", Dns.GetHostName(), ""));
                    services.Add(new ServiceModel("AppPool", "Default", Dns.GetHostName(), ""));

                    foreach (var serviceModel in services.Where(x => x.Type == "Service"))
                    {
                        try
                        {
                            using (var sc = new ServiceController(serviceModel.ServiceName, serviceModel.MachineName))
                            {
                                serviceModel.Status = sc.Status.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            serviceModel.Status = "Error: " + ex.Message;
                        }

                    }

                    foreach (var serviceModel in services.Where(x => x.Type == "Website"))
                    {
                        try
                        {
                            using (ServerManager manager = ServerManager.OpenRemote(serviceModel.MachineName))
                            {
                                var website = manager.Sites.FirstOrDefault(ap => ap.Name == serviceModel.ServiceName);

                                //Don't bother trying to recycle if we don't have an app pool
                                if (website != null)
                                {
                                    //Get the current state of the app pool
                                    serviceModel.Status = website.State.ToString();
                                }
                                else
                                {
                                    serviceModel.Status = string.Format("An Application does not exist with the name {0}", serviceModel.ServiceName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            serviceModel.Status = "Error: " + ex.Message;
                        }
                    }

                    foreach (var serviceModel in services.Where(x => x.Type == "AppPool"))
                    {
                        try
                        {
                            using (ServerManager manager = ServerManager.OpenRemote(serviceModel.MachineName))
                            {
                                ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == serviceModel.ServiceName);

                                //Don't bother trying to recycle if we don't have an app pool
                                if (appPool != null)
                                {
                                    //Get the current state of the app pool
                                    serviceModel.Status = appPool.State.ToString();
                                }
                                else
                                {
                                    serviceModel.Status = string.Format("An Application Pool does not exist with the name {0}.{1}", serviceModel.MachineName, serviceModel.ServiceName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            serviceModel.Status = "Error: " + ex.Message;
                        }
                    }

                    return services;

                }, _cancel.Token).ContinueWith((x) =>
                {
                    if (x.IsCanceled || x.IsFaulted)
                        return new Finished(x.Result.ToList(), x.Exception);
                    return new Finished(x.Result.ToList(), null);
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(self);

                // switch behavior
                Become(Working);
            });
        }

        private void Working()
        {
            Receive<Cancel>(cancel =>
            {
                _cancel.Cancel(); // cancel work
                BecomeReady();
            });
            Receive<Finished>(f =>
            {
                var ip = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();

                if (ip.Length > ip.LastIndexOf("."))
                    _hub.PushServiceClusterStatus(f.Services.ToList(), "Website1:" + ip.Substring(ip.LastIndexOf(".")+1, ip.Length - ip.LastIndexOf(".")-1));

                BecomeReady();
            });
        }

        private void BecomeReady()
        {
            _cancel = new CancellationTokenSource();
            Become(Ready);
        }

        protected override void PostStop()
        {
            ClusterServiceStatusTeller.Cancel();
            base.PostStop();
        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("serviceStatusHub") as ServiceStatusHub;
        }


    }
}