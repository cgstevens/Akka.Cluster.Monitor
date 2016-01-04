## My Akka.Net Examples that I have created within this solution.

* [How to monitor your Akka cluster.](https://github.com/cgstevens/Akka.Cluster.Monitor/wiki/Cluster-Monitor)
* [How to control windows services.](https://github.com/cgstevens/Akka.Cluster.Monitor/wiki/AkkaServicesControl)
* [How to create a job, subscribe to that job and report the status.](https://github.com/cgstevens/Akka.Cluster.Monitor/wiki/Job-Workers)
* [Why you need more than 1 lighthouse to keep cluster intact.](https://github.com/cgstevens/Akka.Cluster.Monitor/wiki/Seed-Nodes)
* MORE EXAMPLES TO COME!

##Projects  
Remember to start all of the projects up to see the full effect!  

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
		Demonstrates that you don't need to be part of the cluster to get the information as well as send commands.
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





The following links will help you along with your Akka adventure!  
<b>Main Site:</b> http://getakka.net/  
<b>Documentation:</b> http://getakka.net/docs/  
<b>The Code (includes basic examples):</b> https://github.com/akkadotnet/getakka.net  
<b>Need to ask a question:</b> https://gitter.im/akkadotnet/akka.net  
<b>Where do you begin:</b> https://github.com/petabridge/akka-bootcamp  
<b>Where do you begin Part2:</b> https://github.com/petabridge/akkadotnet-code-samples
