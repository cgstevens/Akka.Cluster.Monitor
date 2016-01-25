
appRoot
    .controller('ReportApiController', ['$scope', '$filter', '$location', '$resource', '$templateCache', '$timeout', 'ReportApiService', 'watchCountService', function ($scope, $filter, $location, $resource, $templateCache, $timeout, reportApiService, watchCountService) {
        var vm = this,
            getClusterState = function (clusterAddress) {
                reportApiService.getClusterState(clusterAddress).then(
                    function (cluster) {
                        var clusterStatusModel = { currentClusterAddress: cluster.data.CurrentClusterAddress, clusterState: cluster.data.ClusterState, timeStamp: new Date() };
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
                                for (var y = 0; y < clusterStatusModel.clusterState.Members.length; y++) {
                                    clusterStatusModel.clusterState.Members[i].RoleLeader = "False";
                                }
                                for (var y = 0; y < cluster.data.ClusterRoleLeaders.length; y++) {
                                    if (cluster.data.ClusterRoleLeaders[y].Address != null && cluster.data.ClusterRoleLeaders[y].Address.Host === clusterStatusModel.clusterState.Members[i].Address.Host && cluster.data.ClusterRoleLeaders[y].Address.Port === clusterStatusModel.clusterState.Members[i].Address.Port) {
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
                                if (cluster.data.CurrentClusterAddress.Roles !== undefined) {
                                    vm.clusterList[x].currentClusterAddress.Name = cluster.data.CurrentClusterAddress.Roles[0] + ' - ' + cluster.data.CurrentClusterAddress.Host + ':' + cluster.data.CurrentClusterAddress.Port;
                                } else {
                                    vm.clusterList[x].currentClusterAddress.Name = 'Unknown - ' + vm.clusterList[x].currentClusterAddress.Host + ':' + vm.clusterList[x].currentClusterAddress.Port;
                                }
                                vm.clusterList[x].clusterState = clusterStatusModel.clusterState;
                                vm.clusterList[x].timeStamp = clusterStatusModel.timeStamp;
                                exists = true;
                            }
                        }


                        if (!exists) {
                            if (cluster.data.CurrentClusterAddress.Roles !== undefined) {
                                clusterStatusModel.currentClusterAddress.Name = cluster.data.CurrentClusterAddress.Roles[0] + ' - ' + cluster.data.CurrentClusterAddress.Host + ':' + cluster.data.CurrentClusterAddress.Port;
                            } else {
                                clusterStatusModel.currentClusterAddress.Name = 'Unknown - ' + cluster.data.CurrentClusterAddress.Host + ':' + cluster.data.CurrentClusterAddress.Port;
                            }

                            vm.clusterList.push(clusterStatusModel);
                        }

                        if (vm.selectedCluster === '') {
                            vm.selectedCluster = vm.clusterList[0];
                        }


                        $scope.$apply();
                    },
                    function (error) {
                        //notificationService.error("Could not start broadcast messaging!");
                    }
                );
            },
            memberLeaveCluster = function (member) {
                alert('MemberLeaveCluster not fully implemented yet.');
                //reportApiService.memberLeaveCluster(member).then(
                //    function (response) {
                //    },
                //    function (error) {
                //        //notificationService.error("Could not start broadcast messaging!");
                //    }
                //);
            },
            downMember = function (member) {
                alert('DownMember not fully implemented yet.');
                //reportApiService.downMember(member).then(
                //    function (response) {
                //    },
                //    function (error) {
                //        //notificationService.error("Could not start broadcast messaging!");
                //    }
                //);
            },
            initialize = function () {
                vm.clusterList = [];
                vm.selectedCluster = '';
                vm.clusterAddress = 'http://localhost:8080/api/ClusterStatus/1';
                vm.timerRunning = false;
                vm.timer = null;
                vm.countDown = 2;

            };

        vm.getClusterState = function () {
            getClusterState(vm.clusterAddress);
        };

        vm.memberLeaveCluster = function (member) {
            memberLeaveCluster(member);
        };

        vm.downMember = function (member) {
            downMember(member);
        };

        vm.startTimer = function () {
            vm.timerRunning = true;
            vm.countDown = 2;

            vm.timer = setInterval(function () {
                if (vm.countDown > 0 && vm.timerRunning === true) {
                    vm.countDown--;
                } else {
                    vm.getClusterState();
                    vm.countDown = 2;
                }

                $scope.$apply();
            }, 1000);

        };

        vm.stopTimer = function () {
            vm.timerRunning = false;            
            clearInterval(vm.timer);
            vm.timer = 0;
        }


        //// Initialize App
        initialize();
    }]);