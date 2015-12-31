// Copyright 2014-2015 Aaron Stannard, Petabridge LLC
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Akka.Actor;
using Topshelf;

namespace Lighthouse2
{
    class Program
    {
        static bool exitSystem = false;
        public static ActorSystem ClusterSystem { get; set; }
        public static IActorRef ClusterHelper;
        public static IActorRef ClusterStatus;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");
            
            ClusterSystem.Shutdown();
            Console.WriteLine("Cleanup complete");

            Thread.Sleep(5000); // Give the Remove time to actually remove...

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion
        static int Main(string[] args)
        {
            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            if (Environment.UserInteractive)
            {
                _handler += new EventHandler(Handler);
                SetConsoleCtrlHandler(_handler, true);
            }
            
            return (int) HostFactory.Run(x =>
            {
                x.SetServiceName("Lighthouse");
                x.SetDisplayName("Lighthouse Service Discovery");
                x.SetDescription("Lighthouse Service Discovery for Akka.NET Clusters");
                
                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                //x.StartAutomatically();
                x.DependsOnEventLog();
                //x.UseLog4Net();
                x.Service<LighthouseService>();
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
