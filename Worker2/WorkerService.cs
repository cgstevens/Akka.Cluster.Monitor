using System.Threading;
using Akka.Actor;
using Shared;
using Shared.Actors;
using Topshelf;
using ClusterHelper = Shared.Actors.ClusterHelper;

namespace Worker2
{
    internal class WorkerService : ServiceControl
    {
        public HostControl _hostControl;
        
        public bool Start(HostControl hostControl)
        {
            _hostControl = hostControl;
            InitializeCluster();
            return true;
        }
        
        public bool Stop(HostControl hostControl)
        {
            //do your cleanup here
            Program.ClusterHelper.Tell(new ClusterHelper.RemoveMember());

            Thread.Sleep(5000); // Give the Remove time to actually remove before totally shutting down system...

            Program.ClusterSystem.Terminate();
            
            return true;
        }

        public void InitializeCluster()
        {
            Program.ClusterSystem = ActorSystem.Create(ActorPaths.ActorSystem);
            Program.ClusterHelper = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterHelper()),
                ActorPaths.ClusterHelperActor.Name);
            Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus(_hostControl)),
                ActorPaths.ClusterStatusActor.Name);
        }
    }
}