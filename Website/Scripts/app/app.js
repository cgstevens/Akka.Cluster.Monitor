
angular.module("website.infrastructure", []);
angular.module("report.values", []);
var appRoot = angular.module('main', ['ngRoute', 'ngGrid', 'ngResource', 'report.values', 'website.infrastructure', 'fsm', 'report.services', 'pasvaz.bindonce', 'infinite-scroll', 'report.directives', 'ngAnimate']);


appRoot.config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
    $httpProvider.interceptors.push('requestInterceptor');

    //Setup routes to load partial templates from server. TemplateUrl is the location for the server view (Razor .cshtml view)
    $routeProvider
            .when('/home', { templateUrl: 'home/main', controller: 'MainController as vm' })
            .when('/akka/report', { templateUrl: 'home/report', controller: 'ReportController as vm' })
            .when('/akka/reportapi', { templateUrl: 'home/reportapi', controller: 'ReportApiController as vm' })
            .when('/akka/servicestatus', { templateUrl: 'home/servicestatus', controller: 'ServiceStatusController as vm' })
            .when('/akka/itemstatus', { templateUrl: 'home/itemstatus', controller: 'ItemStatusController as vm' })
            .otherwise({ redirectTo: '/akka/itemstatus' });
}])
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location', function ($scope, $route, $routeParams, $location) {
        var vm = this;

        $scope.$on('$routeChangeSuccess', function (e, current, previous) {
            $scope.activeViewPath = $location.path();
            $scope.format = 'M/d/y';
            vm.format = $scope.format;
            if (console) {
                console.log($location.path());
            }
        });
    }]);





