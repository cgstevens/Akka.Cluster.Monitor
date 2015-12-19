(function () {
    var signalrService = function ($rootScope, $q, $log, $location, $timeout) {
        var baseUrl = $location.protocol() + '://' + $location.host() + ($location.port() !== 80 && $location.port() !== 443 ? ':' + $location.port() : '');
        $.connection.hub.url = baseUrl + '/signalr/hubs';
        var
            messageHubs = [
                {
                    hub: $.connection.clusterStateHub, name: 'clusterStateHub', reconnectTimer: undefined,
                    broadcastMessages: [
                        {
                            serverMethodName: 'broadcastClusterState', subscribed: false, self: this,
                            handler: function (clusterState, clusterRoleLeaders, currentClusterAddress) {
                                var data = { clusterState: clusterState, clusterRoleLeaders: clusterRoleLeaders, currentClusterAddress: currentClusterAddress };
                                broadcastHandler('signalrClusterState', data);
                            }
                        }
                    ]
                }
            ],
            signalrTransport = { transport: ['longPolling', 'webSockets'] },
            findHubByName = function (name) {
                var criteriaFunction = function (c) {
                    return c.name === name;
                };
                var results = messageHubs.filter(criteriaFunction);
                return results && results.length > 0 ? results[0] : null;
            },
            findBroadcastMessage = function (messageHub, serverMethodName) {
                var criteriaFunction = function (c) {
                    return c.serverMethodName == serverMethodName;
                };
                var results = messageHub.broadcastMessages.filter(criteriaFunction);
                return results && results.length > 0 ? results[0] : null;
            };

        // Must attach an empty handler initially (limitation of signalr)
        for (var i = 0; i < messageHubs.length; i++) {
            var messageHub = messageHubs[i];
            for (var j = 0; j < messageHub.broadcastMessages.length; j++) {
                var broadcastMessage = messageHub.broadcastMessages[j];
                var serverMethodName = broadcastMessage.serverMethodName;
                messageHub.hub.client[serverMethodName] = function () { };
            }
        }

        var broadcastHandler = function (eventName, data) {
            $log.log('SignalR message received: ' + eventName);
            $rootScope.$broadcast(eventName, data);
        };

        var attachHandlers = function (messageHub) {
            messageHub.hub.connection.stateChanged(function (change) {
                if (change.newState === $.signalR.connectionState.reconnecting) {
                    $log.log('SignalR connection lost to hub ' + messageHub.name);
                } else if (change.newState === $.signalR.connectionState.connected) {
                    $log.log('SignalR connected. Hub: ' + messageHub.name);
                } else if (change.newState === $.signalR.connectionState.disconnected) {
                    $log.log('SignalR disconnected. Hub: ' + messageHub.name);
                };
            });

            messageHub.hub.connection.error(function (error) {
                if (error) {
                    $log.log('Error on hub ' + messageHub.name + ':' + error.message);
                }

                if (messageHub.reconnectTimer) {
                    $timeout.cancel(messageHub.reconnectTimer);
                }

                // Try to restart the connection
                messageHub.reconnectTimer = $timeout(function () { $.connection.hub.start(signalrTransport); }, 2000);
            });

            messageHub.hub.connection.reconnected(function (error) {
                $log.log('SignalR reconnected.  Hub: ' + messageHub.name);
                if (messageHub.reconnectTimer) {
                    $timeout.cancel(messageHub.reconnectTimer);
                }
            });
        }

        var attachConnectionEventHandlers = function () {
            for (var i = 0; i < messageHubs.length; i++) {
                var messageHub = messageHubs[i];
                attachHandlers(messageHub);
            }
        };

        var subscribe = function (name, event) {
            var messageHub = findHubByName(name);
            var broadcastMessage = findBroadcastMessage(messageHub, event);
            if (!broadcastMessage.subscribed) {
                messageHub.hub.on(event, broadcastMessage.handler);
                broadcastMessage.subscribed = true;
                $log.log('SignalR Subscribed. Hub: ' + messageHub.name + ', Event: ' + event);
            }
        };

        var unsubscribe = function (name, event) {
            var messageHub = findHubByName(name);
            var broadcastMessage = findBroadcastMessage(messageHub, event);
            if (broadcastMessage.subscribed) {
                messageHub.hub.off(event, broadcastHandler.handler);
                broadcastMessage.subscribed = false;
                $log.log('SignalR Unsubscribed. Hub: ' + messageHub.name + ', Event: ' + event);
            }
        };

        var sendMessage = function () {
            messageHubs[0].hub.server.send("Hello!", "I'm a button click and you received a response from SignalR!");
        };

        var startListener = function () {
            var defer = $q.defer();
            var isAnyHubNull = false;
            for (var i = 0; i < messageHubs.length; i++) {
                if (messageHubs[i].hub == null) {
                    isAnyHubNull = true;
                    break;
                }
            }
            if (isAnyHubNull) {
                defer.reject();
            } else {
                attachConnectionEventHandlers();
                $.connection.hub
                    .start(signalrTransport)
                    .done(function () {
                        defer.resolve();
                    })
                    .fail(function (error) {
                        if (error) {
                            $log.log(error.message);
                        }
                        else {
                            $log('Error - disconnected.');
                        }
                        defer.reject();
                    });
            }
            return defer.promise;
        };

        return {
            startListener: startListener,
            sendMessage: sendMessage,
            subscribe: subscribe,
            unsubscribe: unsubscribe
        };
    };

    signalrService.$inject = ['$rootScope', '$q', '$log', '$location', '$timeout'];
    angular.module('report.services')
        .factory('signalrService', signalrService);
})()
