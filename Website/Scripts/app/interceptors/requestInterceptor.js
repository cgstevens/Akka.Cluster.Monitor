(function () {
    var requestInterceptor = function ($q, $rootScope, $injector) {
        $rootScope.submissionInProgress = false;
        $rootScope.showLoading = false;
        $rootScope.pendingRequests = 0; // with using $templateCache, the $http.pendingRequests seemed unreliable.  So, I've added an additional counter to compare against
        $rootScope.http = null;

        var turnOffModal = function (error) {
            $rootScope.pendingRequests--;
            $rootScope.http = $rootScope.http || $injector.get('$http');
            //if ($rootScope.http.pendingRequests.length < 1 || $rootScope.pendingRequests <= 0) {
            if ($rootScope.pendingRequests <= 0) {
                if ($rootScope.showLoading) {
                    $('body').spin("modal");
                }
                $rootScope.showLoading = false;
                if (error) {
                    $rootScope.$broadcast(error);
                }
                $rootScope.$broadcast('loading-complete');
            }
        };

        return {
            'request': function (config) {
                if (config.method !== 'GET') {
                    $rootScope.submissionInProgress = true;
                }
                $rootScope.pendingRequests++;
                if (!$rootScope.showLoading) {
                    $rootScope.showLoading = true;
                    $rootScope.$broadcast('loading-started');
                    $('body').spin("modal");
                }
                $rootScope.showLoading = true;
                return config || $q.when(config);
            },

            'requestError': function (rejection) {
                turnOffModal('request-error');
                return $q.reject(rejection);
            },

            'response': function (response) {
                if (response.config.method !== 'GET') {
                    $rootScope.submissionInProgress = false;
                }
                turnOffModal();
                return $q.when(response);
            },

            'responseError': function (rejection) {
                if (rejection.config.method !== 'GET') {
                    $rootScope.submissionInProgress = false;
                }
                turnOffModal('response-error');
                return $q.reject(rejection);
            }
        };
    };

    requestInterceptor.$inject = ['$q', '$rootScope', '$injector'];
    angular.module('website.infrastructure')
        .factory('requestInterceptor', requestInterceptor);
})()
