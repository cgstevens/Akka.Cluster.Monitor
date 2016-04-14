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

using System.Threading;
using Akka.Actor;
using Shared;
using Shared.Actors;
using Topshelf;
using Akka.Cluster.Tools.Client;

namespace Lighthouse2
{
    public class LighthouseService : ServiceControl
    {
        private readonly string _ipAddress;
        private readonly int? _port;

        private ActorSystem _lighthouseSystem;
        private HostControl _hostControl;

        public LighthouseService() : this(null, null)
        {
        }

        public LighthouseService(string ipAddress, int? port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public bool Start(HostControl hostControl)
        {
            _hostControl = hostControl;
            InitializeCluster();
            return true;
        }

        public bool Restart()
        {
            InitializeCluster();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Program.ClusterHelper.Tell(new ClusterHelper.RemoveMember());

            Thread.Sleep(10000); // Give the Remove time to actually remove before totally shutting down system...

            _lighthouseSystem.Terminate();

            return true;
        }

        public void InitializeCluster()
        {
            _lighthouseSystem = LighthouseHostFactory.LaunchLighthouse(_ipAddress, _port);
            Program.ClusterSystem = _lighthouseSystem;
            Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus(_hostControl)),
                ActorPaths.ClusterStatusActor.Name);
            Program.ClusterHelper = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterHelper()),
                ActorPaths.ClusterHelperActor.Name);

            //var receptionist = ClusterClientReceptionist.Get(_lighthouseSystem);
            //receptionist.RegisterService(_lighthouseSystem.ActorOf(Props.Create<ClusterManager>(), "clustermanager"));
        }
    }
}
