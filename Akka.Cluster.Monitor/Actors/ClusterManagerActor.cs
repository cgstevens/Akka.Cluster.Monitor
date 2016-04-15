﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using Akka.Cluster.Tools.Client;
using Shared.Actors;
using Shared;

namespace Akka.Cluster.Monitor.Actors
{
    /// <summary>
    /// Actor responsible for processing commands
    /// </summary>
    public class ClusterManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private ListBox _clusterListBox;
        private ListView _clusterListView;
        private ListView _unreachableListView;
        private ListView _seenByListView;
        private Dictionary<string, Member> Members;
        protected ICancelable CurrentClusterStateTeller;
        private IActorRef clusterClient;

        public ClusterManagerActor(ListBox clusterListBox, ListView clusterListView, ListView unreachableListView, ListView seenByListView)
        {
            Members = new Dictionary<string, Member>();
            _clusterListBox = clusterListBox;
            _clusterListView = clusterListView;
            _seenByListView = seenByListView;
            _unreachableListView = unreachableListView;

            CurrentClusterStateTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMilliseconds(20),
                    TimeSpan.FromSeconds(2), Self, new Messages.Messages.CurrentClusterState(), Self);

            clusterClient = null;
            Receives();
        }

        protected override void PostStop()
        {
        }

        private void UpdateClusterListView(Member member)
        {
            var key = member.Address.ToString();

            if (!Members.ContainsKey(key))
            {
                Members.Add(key, member);
            }
            else
            {
                Members[key] = member;
            }
            
            if (!_clusterListView.Items.ContainsKey(key))
            {
                string[] arr = new string[6];
                arr[0] = member.Roles.Join(",");
                arr[1] = member.Status.ToString();
                arr[2] = key;
                arr[3] = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
                arr[4] = "False";
                arr[5] = "False";
                ListViewItem item = new ListViewItem(arr);
                item.Name = key;
                _clusterListView.Items.Add(item);
            }
            else
            {
                _clusterListView.Items[key].SubItems[0].Text = member.Roles.Join(",");
                _clusterListView.Items[key].SubItems[1].Text = member.Status.ToString();
                _clusterListView.Items[key].SubItems[2].Text = key;
                _clusterListView.Items[key].SubItems[3].Text = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
                _clusterListView.Items[key].SubItems[4].Text = "False";
                _clusterListView.Items[key].SubItems[5].Text = "False";
            }
            
        }

        private void UpdateUnreachableListView(Member member)
        {
            var key = member.Address.ToString();

            if (!_unreachableListView.Items.ContainsKey(key))
            {
                string[] arr = new string[6];
                arr[0] = member.Roles.Join(",");
                arr[1] = member.Status.ToString();
                arr[2] = key;
                arr[3] = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
                ListViewItem item = new ListViewItem(arr);
                item.Name = key;
                _unreachableListView.Items.Add(item);
            }
            else
            {
                _unreachableListView.Items[key].SubItems[0].Text = member.Roles.Join(",");
                _unreachableListView.Items[key].SubItems[1].Text = member.Status.ToString();
                _unreachableListView.Items[key].SubItems[2].Text = key;
                _unreachableListView.Items[key].SubItems[3].Text = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
            }
        }

        private void UpdateSeenByListView(Member member)
        {
            var key = member.Address.ToString();

            if (!_seenByListView.Items.ContainsKey(key))
            {
                string[] arr = new string[6];
                arr[0] = member.Roles.Join(",");
                arr[1] = member.Status.ToString();
                arr[2] = key;
                arr[3] = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
                ListViewItem item = new ListViewItem(arr);
                item.Name = key;
                _seenByListView.Items.Add(item);
            }
            else
            {
                _seenByListView.Items[key].SubItems[0].Text = member.Roles.Join(",");
                _seenByListView.Items[key].SubItems[1].Text = member.Status.ToString();
                _seenByListView.Items[key].SubItems[2].Text = key;
                _seenByListView.Items[key].SubItems[3].Text = DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff");
            }
        }

        private void Receives()
        {
            Receive<ClusterEvent.CurrentClusterState>(state =>
            {
                if (state.Members.Count == 0)
                {
                    _clusterListBox.Items.Insert(0, "No members have been found.");
                    return;
                }

                // Cluster List
                foreach (var member in state.Members)
                {
                    UpdateClusterListView(member);
                }
                
                // Unreachable
                var removeMembers = new List<ListViewItem>();
                foreach (ListViewItem item in _unreachableListView.Items)
                {
                    var exists = state.Unreachable.FirstOrDefault(x => x.Address.ToString() == item.Name);
                    if (exists == null)
                    {
                        removeMembers.Add(item);
                    }
                }
                foreach (var removeMember in removeMembers)
                {
                    _unreachableListView.Items.RemoveByKey(removeMember.Name.ToString());
                }
                foreach (var member in state.Unreachable)
                {
                    UpdateUnreachableListView(member);
                }


                // Seenby
                var removeSeenByMembers = new List<ListViewItem>();
                foreach (ListViewItem item in _seenByListView.Items)
                {
                    var exists = state.SeenBy.FirstOrDefault(x => x.ToString() == item.Name);
                    if (exists == null)
                    {
                        removeSeenByMembers.Add(item);
                    }
                }
                foreach (var removeMember in removeMembers)
                {
                    _seenByListView.Items.RemoveByKey(removeMember.Name.ToString());
                }
                foreach (var address in state.SeenBy)
                {
                    var member = state.Members.FirstOrDefault(x => x.Address == address);
                    UpdateSeenByListView(member);
                }

                // Set Cluster Leader
                foreach (ListViewItem item in _clusterListView.Items)
                {
                    item.SubItems[4].Text = "False";
                }
                if (state.Leader != null && _clusterListView.Items.ContainsKey(state.Leader.ToString()))
                {
                    _clusterListView.Items[state.Leader.ToString()].SubItems[4].Text = "True";
                }
                
                // Set RoleLeader
                var roles = Members.Select(x => x.Value.Roles.First()).Distinct().ToList();
                foreach (var role in roles)
                {
                    var address = state.RoleLeader(role);
                    foreach (ListViewItem item in _clusterListView.Items)
                    {
                        if (item.SubItems[0].Text == role)
                        {
                            item.SubItems[5].Text = "False";
                        }
                    }
                    if (address != null && _clusterListView.Items.ContainsKey(address.ToString()))
                    {
                        _clusterListView.Items[address.ToString()].SubItems[5].Text = "True";
                    }
                }
            });
            
            Receive<ClusterEvent.MemberUp>(mem =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  MemberUp: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), mem.Member, mem.Member.Roles.Join(",")));
                UpdateClusterListView(mem.Member);
            });
            
            Receive<ClusterEvent.UnreachableMember>(mem =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  UnreachableMember: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), mem.Member, mem.Member.Roles.Join(",")));
                UpdateClusterListView(mem.Member);
                UpdateUnreachableListView(mem.Member);
            });

            Receive<ClusterEvent.ReachableMember>(mem =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  ReachableMember: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), mem.Member, mem.Member.Roles.Join(",")));
                UpdateClusterListView(mem.Member);
                _unreachableListView.Items.RemoveByKey(mem.Member.Address.ToString());
            });

            Receive<ClusterEvent.MemberRemoved>(mem =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  MemberRemoved: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), mem.Member, mem.Member.Roles.Join(",")));
                var key = mem.Member.Address.ToString();
                if (Members.ContainsKey(key))
                {
                    _clusterListView.Items.RemoveByKey(key);
                }
            });

            Receive<ClusterEvent.IMemberEvent>(mem =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  IMemberEvent: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), mem.Member, mem.Member.Roles.Join(",")));
                UpdateClusterListView(mem.Member);
            });

            Receive<ClusterManager.SubscribeToManager>(ic =>
            {
                Program.MyActorSystem.Settings.InjectTopLevelFallback(ClusterClientReceptionist.DefaultConfig());
                clusterClient = Program.MyActorSystem.ActorOf(ClusterClient.Props(ClusterClientSettings.Create(Program.MyActorSystem)));
                Context.Watch(clusterClient);
                clusterClient.Tell(new ClusterClient.Send(ActorPaths.ClusterManagerActor.Path, new ClusterManager.SubscribeToManager()));
            });

            Receive<Terminated>(ic =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0} Address Terminated:  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), ic.AddressTerminated.ToString()));
            });

            Receive<ClusterManager.ClusterMessage>(ic =>
            {                
                _clusterListBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), ic.Message));
            });

            Receive<ClusterManager.UnSubscribeFromManager>(ic =>
            {
                Context.Unwatch(clusterClient);
                clusterClient.Tell(new ClusterClient.Send(ActorPaths.ClusterManagerActor.Path, new ClusterManager.UnSubscribeFromManager()));
                clusterClient = null;
            });

			Receive<Messages.Messages.MemberDown>(key =>
            {
                if (clusterClient == null)
                {
                    _clusterListBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "You must subscribe first before forcing a member down."));
                    return;
                }

                if (Members.ContainsKey(key.Address))
                {
                    var member = Members[key.Address];
                    clusterClient.Tell(new ClusterClient.Send(ActorPaths.ClusterManagerActor.Path, new ClusterManager.MemberDown(Self.ToString(), member.Address)));
                }                
            });

			Receive<Messages.Messages.MemberLeave>(key =>
            {
                if (clusterClient == null)
                {
                    _clusterListBox.Items.Insert(0, string.Format("{0}  {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), "You must subscribe first before asking a member to leave."));
                    return;
                }

                if (Members.ContainsKey(key.Address))
                {
                    var member = Members[key.Address];
                    clusterClient.Tell(new ClusterClient.Send(ActorPaths.ClusterManagerActor.Path, new ClusterManager.MemberLeave(Self.ToString(), member.Address)));

                }
            });
            
            

            

            

            Receive<ClusterEvent.LeaderChanged>(leader =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  LeaderChanged: {1}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), leader.Leader != null ? leader.Leader.ToString() : "Missing Leader Value"));

                foreach (ListViewItem item in _clusterListView.Items)
                {
                    item.SubItems[4].Text = "False";
                }

                if (leader.Leader != null && _clusterListView.Items.ContainsKey(leader.Leader.ToString()))
                {
                    _clusterListView.Items[leader.Leader.ToString()].SubItems[4].Text = "True";
                }
            });

            Receive<ClusterEvent.RoleLeaderChanged>(leader =>
            {
                _clusterListBox.Items.Insert(0, string.Format("{0}  RoleLeaderChanged: {1}, Role: {2}", DateTime.Now.ToString("MM-dd-yy hh:mm:ss.fff"), leader.Leader != null ? leader.Leader.ToString() : "Missing Leader Value", leader.Role.ToString()));
                
                foreach (ListViewItem item in _clusterListView.Items)
                {
                    if (item.SubItems[0].Text == leader.Role)
                    {
                        item.SubItems[5].Text = "False";
                    }

                }

                if (leader.Leader != null && _clusterListView.Items.ContainsKey(leader.Leader.ToString()))
                {
                    _clusterListView.Items[leader.Leader.ToString()].SubItems[5].Text = "True";
                }
            });
        }
    }
}