appRoot
    .controller('ItemStatusController', ['$scope', '$filter', '$location', '$resource', '$templateCache', 'ItemStatusService', 'signalrService', 'watchCountService', function ($scope, $filter, $location, $resource, $templateCache, itemStatusService, signalrService, watchCountService) {
        var vm = this,
            signalrReady = false,
            addBroadcastListeners = function () {
                
                $scope.$on("signalrSubscriptionStatus", function (event, data) {
                    //vm.messages.push(data);
                    data.timestamp = new Date();
                    vm.subscriptionStatus = data.subscriptionStatus;
                    $scope.$apply();
                });

                $scope.$on("signalrClusterRoutes", function (event, data) {
                    vm.clusterRoutes = data.clusterRoutes;
                    vm.clusterRoutes.timestamp = $filter('date')(new Date(), 'MM/dd/yyyy hh:mm:ss.sss');
                    $scope.$apply();
                });

                $scope.$on("signalrJobWorkerInfo", function (event, data) {
                    var jobWorkerInfo = data;

                    var today = $filter('date')(new Date(), 'MM/dd/yyyy hh:mm:ss.sss');
                    jobWorkerInfo.timestamp = today;

                    if (jobWorkerInfo.job.JobInfo.Id === "No Job Found") {
                        vm.jobWorkerInfoList = [];
                    }

                    // Find out if workerInfo already exists
                    var exists = false;
                    var removeNotFound = -1;
                    for (var i = 0; i < vm.jobWorkerInfoList.length; i++) {
                        if (jobWorkerInfo.job.JobInfo.Id === vm.jobWorkerInfoList[i].job.JobInfo.Id) {
                            vm.jobWorkerInfoList[i] = jobWorkerInfo;
                            exists = true;
                        }

                        if (vm.jobWorkerInfoList.length > 1 && "No Job Found" === vm.jobWorkerInfoList[i].job.JobInfo.Id) {
                            removeNotFound = i;
                        }
                    }
                    if (!exists) {
                        vm.jobWorkerInfoList.push(jobWorkerInfo);
                    }
                    if (removeNotFound > -1) {
                        vm.jobWorkerInfoList.splice(removeNotFound, 1);
                    }
                    
                    $scope.$apply();
                });
            },
            subscribeToWorkers = function () {
                itemStatusService.subscribeToWorkers().then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            unsubscribeToWorkers = function () {
                itemStatusService.unsubscribeToWorkers().then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            initialize = function () {
                vm.subscriptionStatus = {};
                vm.clusterRoutes = '';
                vm.jobWorkerInfoList = [];
                vm.timestamp = 'Please wait while I get your data.';

                addBroadcastListeners();

                signalrService.startListener().then(function (response) {
                    signalrReady = true;

                    signalrService.subscribe('itemStatusHub', 'broadcastSubscriptionStatus');
                    signalrService.subscribe('itemStatusHub', 'broadcastClusterRoutes');
                    signalrService.subscribe('itemStatusHub', 'broadcastJobWorkerInfo');
                });
            };
            
        vm.subscribeToWorkers = function() {
            subscribeToWorkers();
        };

        vm.unsubscribeToWorkers = function () {
            unsubscribeToWorkers();
        };
        
        initialize();
    }]);
