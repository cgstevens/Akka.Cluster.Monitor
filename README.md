# My Akka.Net Examples
You can find examples of what else is within the solution here - https://github.com/cgstevens/Akka.Cluster.Monitor/wiki


I needed a way to monitor the current and unreachable members as well as who the member is seenby and this is what I came up with.  
By subscribing to the ClusterEvent.IMemberEvent you can find the status of a cluster pretty easily.

>Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });    
http://getakka.net/docs/clustering/cluster-extension        

I originally created a Windows Form which allowed me to see what members are part of the cluster as I felt like I never knew what was going on.  Are they up, down, joining or not even showing up.
I also had no way to tell a member to leave or be considered down by the cluster so that a joining member can join the cluster.   

Due to network issues connecting the WindowsForm app member it would always became unreachable by the other members which then caused the cluster to go weird. I came up with the a way to monitor the cluster state without having to connect to that network and so I created a web page in our app that would be hosted on the same server or at least on the same network segment.

The website demonstrates how the following frameworks give you a real-time application; AkkaNet, TopShelf, MVC, WebApi, SignalR and AngularJS. 
You can control what messages are being sent to the web client which have subscribed to the web server. These messages can be sent to or from windows services running Akka and TopShelf to your webpage using Angular and SignalR. 
To have full control of message passing to do whatever I want, such as start a job on any of my services on any of my servers from a webpage and get realtime stats back is awesome!

The following links will help you along with your Akka adventure!  
<b>Main Site:</b> http://getakka.net/  
<b>Documentation:</b> http://getakka.net/docs/  
<b>The Code (includes basic examples):</b> https://github.com/akkadotnet/getakka.net  
<b>Need to ask a question:</b> https://gitter.im/akkadotnet/akka.net  
<b>Where do you begin:</b> https://github.com/petabridge/akka-bootcamp  
<b>Where do you begin Part2:</b> https://github.com/petabridge/akkadotnet-code-samples

##Projects  

####WinForms  
	Shows the state of the cluster from its point of view.  

####Website  
	This project has two endpoints: 
	http://localhost:8088/#/akka/report 
		Reports the state of the cluster from its point of view.  
		The state is scheduled to be sent from the SignalRClusterStatusActor -> SignalR.ClusterStateHub -> Webpage 
	http://localhost:8088/#/akka/reportapi  
		Get the cluster state from a specific cluster member.  
		Currently the only member that you can query is the WorkerWebApi.
		This demonstrates that you don't need to be part of the cluster to get the information as well as send commands.
		Currently the Leave and Down button are not fully wired up and will hopefully be soon.  
		
####Worker  
		Plain cluster member.  Demonstrates clean exit when itself is removed from the cluster.  

####WorkerWithWebApi  
		You can only run one instance of this as the website port is hardcoded.
		You must also run it as an adminstrator.
		Creating your services similar to this will allow you to view that members current state.
		This will allow you to access the ClusterEvent.CurrentClusterState by hitting the endpoint  
		http://localhost:8080/api/ClusterStatus/1
		Opens the door to be able to send a Leave/Down message to that member through that api.  
		With having this access to the member my ClusterMonitor has no need to actually be part of  
		the cluster to view just its state but instead can just request the state from a specific member.  
		This will disconnect this client to avoid any interuptions with in the cluster perhaps.  
		Start all of the projects up, then open up your favorite browser and navigate to  
		http://localhost:8080/api/ClusterStatus/1
		This will give you the JSON results of the worker member.  
		
####Lighthouse
		Added for easy setup of a cluster.   You should always run 2 or more.  

####SharedLibrary  
		Contains the shared paths and actors used in the above projects.  



NOTE: I am using the Address as the key for the members instead of the UniqueAddress.
When dealing with the Leader and Role changes those objects do not have the UniqueAddress so looking that up made it not work as expected...

Example:
Build -> Start up Lighthouse -> then start up the rest of the projects.  
Start another ClusterMonitor and watch it join the cluster.
Click on a member then click Down or Leave button and watch that member leave and exit.
You can also close out the one of the ClusterMembers and watch the status change.

<b>Windows app</b>
<img src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/ClusterMonitor.jpg"/>

<br/>
<b>Website Report</b><br/>
<img style="margin-left: 50px;" src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/SelfWebClusterMonitor.jpg"/>

<br/>
<b>Website Report WebApi Request</b><br/>
<img style="margin-left: 50px;" src="https://github.com/cgstevens/Akka.Cluster.Monitor/blob/master/WebApiClusterMonitor.jpg"/>

Remember to start all of the projects up to see the full effect!

Also comments and criticism is always welcome :)
