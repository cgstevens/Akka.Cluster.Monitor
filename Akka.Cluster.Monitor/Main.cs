using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Cluster.Monitor.Actors;
using Shared;

namespace Akka.Cluster.Monitor
{
    public partial class Main : Form
    {
        private IActorRef _myClusterActor;

        public Main()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _myClusterActor.Tell(new Messages.Messages.AppLeaveCluster());

            Thread.Sleep(3000); // Give time to leave before we close out everything.
            base.OnClosing(e);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            InitializeActors();

            clusterListView.View = View.Details;
            clusterListView.GridLines = true;
            clusterListView.FullRowSelect = true;
            clusterListView.Columns.Add("Roles", 100, HorizontalAlignment.Left);
            clusterListView.Columns.Add("Status", 50, HorizontalAlignment.Left);
            clusterListView.Columns.Add("Service Address", 300, HorizontalAlignment.Left);
            clusterListView.Columns.Add("DateStamp", 125, HorizontalAlignment.Left);
            clusterListView.Columns.Add("IsClusterLeader", 75, HorizontalAlignment.Left);
            clusterListView.Columns.Add("IsRoleLeader", 75, HorizontalAlignment.Left);


            unreachableListView.View = View.Details;
            unreachableListView.GridLines = true;
            unreachableListView.FullRowSelect = true;
            unreachableListView.Columns.Add("Roles", 100, HorizontalAlignment.Left);
            unreachableListView.Columns.Add("Status", 50, HorizontalAlignment.Left);
            unreachableListView.Columns.Add("Service Address", 300, HorizontalAlignment.Left);
            unreachableListView.Columns.Add("DateStamp", 125, HorizontalAlignment.Left);


            seenByListView.View = View.Details;
            seenByListView.GridLines = true;
            seenByListView.FullRowSelect = true;
            seenByListView.Columns.Add("Roles", 100, HorizontalAlignment.Left);
            seenByListView.Columns.Add("Status", 50, HorizontalAlignment.Left);
            seenByListView.Columns.Add("Service Address", 300, HorizontalAlignment.Left);
            seenByListView.Columns.Add("DateStamp", 125, HorizontalAlignment.Left);
        }

        private void InitializeActors()
        {
            Program.MyActorSystem = ActorSystem.Create(ActorPaths.ActorSystem);

            _myClusterActor = Program.MyActorSystem.ActorOf(Props.Create(() => new ClusterAppHelper(clusterListBox, clusterListView, unreachableListView, seenByListView)), ActorPaths.ClusterHelperActor.Name);
        }

        private void LeaveClusterButton_Click(object sender, EventArgs e)
        {
            var selectedItem = clusterListView.SelectedItems;
            if (selectedItem.Count > 0)
            {
                _myClusterActor.Tell(new Messages.Messages.AskMemberToLeaveCluster(selectedItem[0].Name));
            }
        }

        private void DownClusterButton_Click(object sender, EventArgs e)
        {
            var selectedItem = clusterListView.SelectedItems;
            if (selectedItem.Count > 0)
            {
                _myClusterActor.Tell(new Messages.Messages.DownMember(selectedItem[0].Name));
            }
        }

    }
}
