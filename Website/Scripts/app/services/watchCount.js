(function () {
    var watchCount = function() {

// I return the count of watchers on the current page.
        function getWatchCount() {
            var total = 0;
            angular.element(".ng-scope").each(
                function ngScopeIterator() {
                    var scope = $(this).scope();
                    total += scope.$$watchers
                        ? scope.$$watchers.length
                        : 0;
                }
            );

            return (total);
        }

        // For convenience, let's serialize the above method and convert it to
        // a bookmarklet that can easily be run on ANY AngularJS page.
        getWatchCount.bookmarklet = (
            "javascript:alert('Watchers:'+(" +
                getWatchCount.toString()
                .replace(/\/\/.*/g, " ")
                .replace(/\s+/g, " ") +
                ")());void(0);"
        );

        return {
            getWatchCount: getWatchCount
        };
    };

    angular.module('report.services')
        .factory('watchCountService', watchCount);
})()
