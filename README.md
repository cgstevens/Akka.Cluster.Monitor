# Akka.Cluster.Monitor
Windows Form and Website that displays the current and unreachable members as well as who the member is seenby.
Added a Lighthouse for easy setup of a cluster.

Build -> Start up Lighthouse -> start up ClusterMonitor.  
Start another ClusterMonitor and watch it join the cluster.
Click on a member then click Down or Leave button and watch that member leave and exit.
You can also close out the one of the ClusterMembers and watch the status change.

NOTE: I am using the Address as the key for the members instead of the UniqueAddress.
When dealing with the Leader and Role changes those objects do not have the UniqueAddress so looking that up made it not work as expected...


<b>What the windows app looks like.</b>
<img src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/ClusterMonitor.jpg"/>


<b>UPDATE: 12/17/2015</b>
Added a new project called Worker which has an WebApiEndpoint. Creating your services similar to this will allow you to view that members current state.
This will allow you to access the ClusterEvent.CurrentClusterState by hitting the endpoint http://localhost:8080/api/ClusterStatus/1
Opens the door to be able to send a Leave/Down message to that member through that api.
With having this access to the member my ClusterMonitor has no need to actually be part of the cluster to view just its state but instead can just request the state from a specific member.  This will disconnect this client to avoid any interuptions with in the cluster perhaps.  
Start all of the projects up, then open up your favorite browser and navigate to http://localhost:8080/api/ClusterStatus/1
This will give you the JSON results of the worker member. 

{"clusterState":{"Members":[{"UniqueAddress":{"Address":{"Host":"127.0.0.1","Port":4053,"System":"myservice","Protocol":"akka.tcp"},"Uid":1171493071},"Status":1,"Roles":["Lighthouse"],"Address":{"Host":"127.0.0.1","Port":4053,"System":"myservice","Protocol":"akka.tcp"}},{"UniqueAddress":{"Address":{"Host":"127.0.0.1","Port":54953,"System":"myservice","Protocol":"akka.tcp"},"Uid":1052005463},"Status":1,"Roles":["ClusterMonitor"],"Address":{"Host":"127.0.0.1","Port":54953,"System":"myservice","Protocol":"akka.tcp"}},{"UniqueAddress":{"Address":{"Host":"127.0.0.1","Port":61111,"System":"myservice","Protocol":"akka.tcp"},"Uid":782767697},"Status":1,"Roles":["ServiceWorker"],"Address":{"Host":"127.0.0.1","Port":61111,"System":"myservice","Protocol":"akka.tcp"}}],"Unreachable":[],"SeenBy":[{"Host":"127.0.0.1","Port":54953,"System":"myservice","Protocol":"akka.tcp"},{"Host":"127.0.0.1","Port":4053,"System":"myservice","Protocol":"akka.tcp"},{"Host":"127.0.0.1","Port":61111,"System":"myservice","Protocol":"akka.tcp"}],"Leader":{"Host":"127.0.0.1","Port":4053,"System":"myservice","Protocol":"akka.tcp"},"AllRoles":["ClusterMonitor","ServiceWorker","Lighthouse"]}}

<b>UPDATE: 12/17/2015</b>
I forgot to add that I will be added a website project as well.
This project will be AngularJS, MVC, SignalR which basically replaces the WinForms App.
It will allow you to join a cluster to view the state as part of the cluster or enter the endpoint such as the "Worker" and ask for the status which then you will no need to be part of the cluster for that request.


<b>UPDATE: 12/18/2015</b>
I have added the website project.  There are two endpoints: 

Report Self Awareness of the cluster.  This page will who you the cluster state from the websites cluster state.
The state is scheduled to be sent from the SignalRClusterStatusActor -> SignalR.ClusterStateHub -> Webpage 
<b>http://localhost:8088/#/akka/report</b>

<img style="margin-left: 50px;" src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/SelfWebClusterMonitor.jpg"/>

Get the cluster state from a specific cluster member.  Currently the only member that you can query is the WorkerWebApi.
This demonstrates that you don't need to be part of the cluster to get the information as well as send commands.
Currently the Leave and Down button are not fully wired up and will hopefully be soon.
<b>http://localhost:8088/#/akka/reportapi</b>

<img style="margin-left: 50px;" src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/WebApiClusterMonitor.jpg"/>


Remember to start all of the projects up to see the full effect!

Also comments and criticism is always welcome :)
