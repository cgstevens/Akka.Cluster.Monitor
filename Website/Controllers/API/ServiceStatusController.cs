using System;
using System.Linq;
using System.ServiceProcess;
using System.Web.Http;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Microsoft.Web.Administration;
using Shared;
using Website.Actors;
using Website.Hubs;

namespace Website.Controllers.API
{
    public class ServiceStatusController : ApiController
    {
        IHubContext _hubContext;

        public ServiceStatusController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ClusterStateHub>();
        }

        [HttpPost]
        public void StartService(string type, string serviceName, string machineName)
        {
            if (type == "Service")
            {
                try
                {
                    using (var sc = new ServiceController(serviceName, "."))
                    {
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
                    }
                }
                catch (Exception ex)
                {
                    var test = ex.Message;
                    throw;
                }
            }
            else if (type == "AppPool")
            {
                StartApplicationPool(machineName, serviceName);
            }
            else if (type == "Website")
            {
                StartApplication(machineName, serviceName);
            }
        }

        [HttpPost]
        public void StopService(string type, string serviceName, string machineName)
        {
            if (type == "Service")
            {
                try
                {
                    using (var sc = new ServiceController(serviceName, "."))
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                    }
                }
                catch (Exception ex)
                {
                    var test = ex.Message;
                    throw;
                }
            }
            else if (type == "AppPool")
            {
                StopApplicationPool(machineName, serviceName);
            }
            else if (type == "Website")
            {
                StopApplication(machineName, serviceName);
            }



        }

        public static void StopApplicationPool(string serverName, string appPoolName)
        {
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(appPoolName))
            {
                try
                {
                    using (ServerManager manager = ServerManager.OpenRemote(serverName))
                    {
                        ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == appPoolName);

                        //Don't bother trying to recycle if we don't have an app pool
                        if (appPool != null)
                        {
                            //Get the current state of the app pool
                            bool appPoolRunning = appPool.State == ObjectState.Started || appPool.State == ObjectState.Starting;

                            //Only try restart the app pool if it was running in the first place, because there may be a reason it was not started.
                            if (appPoolRunning)
                            {
                                //Wait for the app to finish before trying to start
                                while (appPool.State == ObjectState.Starting) { System.Threading.Thread.Sleep(1000); }

                                //Start the app
                                appPool.Stop();
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to restart the application pools for {0}.{1}", serverName, appPoolName), ex.InnerException);
                }
            }
        }

        public static void StartApplicationPool(string serverName, string appPoolName)
        {
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(appPoolName))
            {
                try
                {
                    using (ServerManager manager = ServerManager.OpenRemote(serverName))
                    {
                        ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == appPoolName);

                        //Don't bother trying to recycle if we don't have an app pool
                        if (appPool != null)
                        {
                            //Get the current state of the app pool
                            bool appPoolStopped = appPool.State == ObjectState.Stopped || appPool.State == ObjectState.Stopping;

                            if (appPoolStopped)
                            {
                                //Wait for the app to finish before trying to start
                                while (appPool.State == ObjectState.Stopping) { System.Threading.Thread.Sleep(1000); }

                                //Start the app
                                appPool.Start();
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to restart the application pools for {0}.{1}", serverName, appPoolName), ex.InnerException);
                }
            }
        }

        public static void StopApplication(string serverName, string websiteName)
        {
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(websiteName))
            {
                try
                {
                    using (ServerManager manager = ServerManager.OpenRemote(serverName))
                    {
                        var website = manager.Sites.FirstOrDefault(ap => ap.Name == websiteName);

                        //Don't bother trying to recycle if we don't have an app pool
                        if (website != null)
                        {
                            //Get the current state of the app pool
                            bool websiteRunning = website.State == ObjectState.Started || website.State == ObjectState.Starting;

                            if (websiteRunning)
                            {
                                //Wait for the app to finish before trying to start
                                while (website.State == ObjectState.Starting) { System.Threading.Thread.Sleep(1000); }

                                //Start the app
                                website.Stop();
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("An Application does not exist with the name {0}.{1}", serverName, websiteName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to restart the application for {0}.{1}", serverName, websiteName), ex.InnerException);
                }
            }
        }
        public static void StartApplication(string serverName, string websiteName)
        {
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(websiteName))
            {
                try
                {
                    using (ServerManager manager = ServerManager.OpenRemote(serverName))
                    {
                        var website = manager.Sites.FirstOrDefault(ap => ap.Name == websiteName);

                        //Don't bother trying to recycle if we don't have an app pool
                        if (website != null)
                        {
                            //Get the current state of the app pool
                            bool websiteStopped = website.State == ObjectState.Stopped || website.State == ObjectState.Stopping;

                            if (websiteStopped)
                            {
                                //Wait for the app to finish before trying to start
                                while (website.State == ObjectState.Stopping) { System.Threading.Thread.Sleep(1000); }

                                //Start the app
                                website.Start();
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("An Application does not exist with the name {0}.{1}", serverName, websiteName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to restart the application for {0}.{1}", serverName, websiteName), ex.InnerException);
                }
            }
        }

    }
}
