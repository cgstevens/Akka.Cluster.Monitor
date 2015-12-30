
appRoot
    .controller('ServiceStatusController', ['$scope', '$filter', '$location', '$resource', '$templateCache', 'ServiceStatusService', 'signalrService', 'watchCountService', function ($scope, $filter, $location, $resource, $templateCache, serviceStatusService, signalrService, watchCountService) {
        var vm = this,
            signalrReady = false,
            addBroadcastListeners = function () {
                $scope.$on("signalrServiceStatus", function (event, data) {
                    var serverInfo = { hostName: data.hostName, serviceList: data.services, timeStamp: $filter('date')(new Date(), 'MM/dd/yyyy hh:mm:ss.sss') };
                    vm.timestamp = $filter('date')(new Date(), 'MM/dd/yyyy hh:mm:ss.sss');

                    // Add/Update
                    var exists = false;
                    for (var x = 0; x < vm.serverList.length; x++) {
                        if (vm.serverList[x].hostName === serverInfo.hostName) {
                            vm.serverList[x].serviceList = serverInfo.serviceList;
                            vm.serverList[x].timeStamp = serverInfo.timeStamp;
                            exists = true;
                        }
                    }

                    if (!exists) {
                        vm.serverList.push(serverInfo);
                    }

                    if (vm.selectedServer === '') {
                        vm.selectedServer = vm.serverList[0];
                    }

                    $scope.$apply();
                });

            },
            startService = function (service) {
                serviceStatusService.startService(service).then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            stopService = function (service) {
                serviceStatusService.stopService(service).then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            initialize = function () {
                vm.serverList = [];
                vm.selectedServer = '';
                addBroadcastListeners();
                vm.timestamp = 'Please wait while I get your data.';
                
                signalrService.startListener().then(function (response) {
                    signalrReady = true;

                    signalrService.subscribe('serviceStatusHub', 'broadcastServiceStatus');
                });
            };


        vm.stopService = function (service) {
            stopService(service);
        };

        vm.startService = function (service) {
            startService(service);
        };

        initialize();
    }]);