using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Cluster.Monitor.Actors;
using Shared;
using Shared.Actors;

namespace Akka.Cluster.Monitor
{
    public partial class Main : Form
    {
        private IActorRef _clusterManagerActor;

        public Main()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_clusterManagerActor != null)
            {
                _clusterManagerActor.Tell(new ClusterManager.UnSubscribeFromManager());
            }
            Thread.Sleep(3000); // Give time to leave before we close out everything.
            base.OnClosing(e);
        }

        private void Main_Load(object sender, EventArgs e)
        {
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

        public void InitializeCluster()
        {
            Program.MyActorSystem = ActorSystemFactory.LaunchClusterManager(systemNameTB.Text, ipAddressTB.Text, Convert.ToInt32(portTB.Text));
            loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Actor System Started"));
            InitializeActors();
        }

        private void InitializeActors()
        {
            _clusterManagerActor = Program.MyActorSystem.ActorOf(Props.Create(() => new ClusterManagerActor(loggerBox, clusterListView, unreachableListView, seenByListView, subscribeBtn, unSubscribeBtn)), "clustermanager");
            loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Cluster Manager Actor Started"));
        }

        private void LeaveClusterButton_Click(object sender, EventArgs e)
        {
            var selectedItem = clusterListView.SelectedItems;
            if (selectedItem.Count > 0)
            {
                _clusterManagerActor.Tell(new Messages.Messages.MemberLeave(selectedItem[0].Name));
            }
            else
            {
                loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Select a member to have it leave the cluster."));
            }
        }

        private void DownClusterButton_Click(object sender, EventArgs e)
        {
            var selectedItem = clusterListView.SelectedItems;
            if (selectedItem.Count > 0)
            {
                _clusterManagerActor.Tell(new Messages.Messages.MemberDown(selectedItem[0].Name));
            }
            else
            {
                loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Select a member to have it be forced down."));
            }
        }

        private void subscribeBtn_Click(object sender, EventArgs e)
        {
            _clusterManagerActor.Tell(new ClusterManager.SubscribeToManager());
            clusterListView.Items.Clear();
            seenByListView.Items.Clear();
            unreachableListView.Items.Clear();
        }

        private void unSubscribeBtn_Click(object sender, EventArgs e)
        {
            _clusterManagerActor.Tell(new ClusterManager.UnSubscribeFromManager());
            clusterListView.Items.Clear();
            seenByListView.Items.Clear();
            unreachableListView.Items.Clear();
        }

        private void clusterBtn_Click(object sender, EventArgs e)
        {
            if (clusterBtn.Text == "Start System")
            {
                loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Actor System Starting"));
                InitializeCluster();
                clusterBtn.Text = "Stop System";

                systemNameTB.Enabled = false;
                ipAddressTB.Enabled = false;
                portTB.Enabled = false;

                subscribeBtn.Enabled = true;
                unSubscribeBtn.Enabled = true;
            }
            else
            {
                loggerBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "Actor System Terminating"));
                clusterListView.Items.Clear();
                seenByListView.Items.Clear();
                unreachableListView.Items.Clear();
                Program.MyActorSystem.Terminate();
                Program.MyActorSystem.Dispose();
                clusterBtn.Text = "Start System";

                systemNameTB.Enabled = true;
                ipAddressTB.Enabled = true;
                portTB.Enabled = true;

                subscribeBtn.Enabled = false;
                unSubscribeBtn.Enabled = false;
            }

        }

        private void startSchedule_Click(object sender, EventArgs e)
        {
            if (startSchedule.Text == "Start Schedule")
            {
                startSchedule.Text = "Stop Schedule";
                _clusterManagerActor.Tell(new Messages.Messages.StartSchedule(Convert.ToInt32(secondsTB.Text)));
            }
            else
            {
                startSchedule.Text = "Start Schedule";
                _clusterManagerActor.Tell(new Messages.Messages.StopSchedule());
            }


        }
    }
}
