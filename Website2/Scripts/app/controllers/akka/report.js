
appRoot
    .controller('ReportController', ['$scope', '$filter', '$location', '$resource', '$templateCache', 'ReportService', 'signalrService', 'watchCountService', function ($scope, $filter, $location, $resource, $templateCache, reportService, signalrService, watchCountService) {
        var vm = this,
            signalrReady = false,
            addBroadcastListeners = function () {
                $scope.$on("signalrClusterState", function (event, data) {
                    var clusterStatusModel = { currentClusterAddress: data.currentClusterAddress, clusterState: data.clusterState, timeStamp: new Date() };
                    vm.timestamp = $filter('date')(new Date(), 'MM/dd/yyyy hh:mm:ss.sss');

                    if (clusterStatusModel.clusterState.Members.length > 0) {
                        // Would like to display some meaningful data.
                        for (var i = 0; i < clusterStatusModel.clusterState.Members.length; i++) {
                            switch (clusterStatusModel.clusterState.Members[i].Status) {
                                case 0:
                                    clusterStatusModel.clusterState.Members[i].Status = "Joining";
                                    break;
                                case 1:
                                    clusterStatusModel.clusterState.Members[i].Status = "Up";
                                    break;
                                case 2:
                                    clusterStatusModel.clusterState.Members[i].Status = "Leaving";
                                    break;
                                case 3:
                                    clusterStatusModel.clusterState.Members[i].Status = "Exiting";
                                    break;
                                case 4:
                                    clusterStatusModel.clusterState.Members[i].Status = "Down";
                                    break;
                                case 5:
                                    clusterStatusModel.clusterState.Members[i].Status = "Removed";
                                    break;
                            }

                            // Set Leader
                            for (var y = 0; y < data.clusterRoleLeaders.length; y++) {
                                clusterStatusModel.clusterState.Members[i].RoleLeader = "False";
                            }
                            for (var y = 0; y < data.clusterRoleLeaders.length; y++) {
                                if (data.clusterRoleLeaders[y].Address != null && data.clusterRoleLeaders[y].Address.Host === clusterStatusModel.clusterState.Members[i].Address.Host && data.clusterRoleLeaders[y].Address.Port === clusterStatusModel.clusterState.Members[i].Address.Port) {
                                    clusterStatusModel.clusterState.Members[i].RoleLeader = "True";
                                }
                            }
                        }

                        for (var i = 0; i < clusterStatusModel.clusterState.SeenBy.length; i++) {
                            for (var x = 0; x < clusterStatusModel.clusterState.Members.length; x++) {
                                if (clusterStatusModel.clusterState.SeenBy[i].Host === clusterStatusModel.clusterState.Members[x].Address.Host && clusterStatusModel.clusterState.SeenBy[i].Port === clusterStatusModel.clusterState.Members[x].Address.Port) {
                                    clusterStatusModel.clusterState.SeenBy[i].Roles = clusterStatusModel.clusterState.Members[x].Roles;
                                }
                            }
                        }

                        for (var x = 0; x < clusterStatusModel.clusterState.Members.length; x++) {
                            if (clusterStatusModel.currentClusterAddress.Host === clusterStatusModel.clusterState.Members[x].Address.Host && clusterStatusModel.currentClusterAddress.Port === clusterStatusModel.clusterState.Members[x].Address.Port) {
                                clusterStatusModel.currentClusterAddress.Roles = clusterStatusModel.clusterState.Members[x].Roles;
                            }
                        }

                        for (var x = 0; x < clusterStatusModel.clusterState.Members.length; x++) {
                            if (clusterStatusModel.clusterState.Leader.Host === clusterStatusModel.clusterState.Members[x].Address.Host && clusterStatusModel.clusterState.Leader.Port === clusterStatusModel.clusterState.Members[x].Address.Port) {
                                clusterStatusModel.clusterState.Leader.Roles = clusterStatusModel.clusterState.Members[x].Roles;
                            }
                        }
                    }

                    // Add/Update
                    var exists = false;
                    for (var x = 0; x < vm.clusterList.length; x++) {
                        if (vm.clusterList[x].currentClusterAddress.Host === clusterStatusModel.currentClusterAddress.Host && vm.clusterList[x].currentClusterAddress.Port === clusterStatusModel.currentClusterAddress.Port) {
                            if (data.currentClusterAddress.Roles !== undefined) {
                                vm.clusterList[x].currentClusterAddress.Name = data.currentClusterAddress.Roles[0] + ' - ' + data.currentClusterAddress.Host + ':' + data.currentClusterAddress.Port;
                            } else {
                                vm.clusterList[x].currentClusterAddress.Name = 'Unknown - ' + vm.clusterList[x].currentClusterAddress.Host + ':' + vm.clusterList[x].currentClusterAddress.Port;
                            }
                            vm.clusterList[x].clusterState = clusterStatusModel.clusterState;
                            vm.clusterList[x].timeStamp = clusterStatusModel.timeStamp;
                            exists = true;
                        }
                    }


                    if (!exists) {
                        if (data.currentClusterAddress.Roles !== undefined) {
                            clusterStatusModel.currentClusterAddress.Name = data.currentClusterAddress.Roles[0] + ' - ' + data.currentClusterAddress.Host + ':' + data.currentClusterAddress.Port;
                        } else {
                            clusterStatusModel.currentClusterAddress.Name = 'Unknown - ' + data.currentClusterAddress.Host + ':' + data.currentClusterAddress.Port;
                        }

                        vm.clusterList.push(clusterStatusModel);
                    }

                    if (vm.selectedCluster === '') {
                        vm.selectedCluster = vm.clusterList[0];
                    }


                    $scope.$apply();
                });

            },
            memberLeaveCluster = function (member) {
                reportService.memberLeaveCluster(member).then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            downMember = function (member) {
                reportService.downMember(member).then(
                    function (response) {
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            initialize = function () {
                vm.clusterList = [];
                vm.selectedCluster = '';

                addBroadcastListeners();
                
                signalrService.startListener().then(function (response) {
                    signalrReady = true;

                    signalrService.subscribe('clusterStateHub', 'broadcastClusterState');
                });
            };
        


        vm.memberLeaveCluster = function (member) {
            memberLeaveCluster(member);
        };

        vm.downMember = function (member) {
            downMember(member);
        };

        //// Public Variables
        

        //// Initialize App
        initialize();
    }]);