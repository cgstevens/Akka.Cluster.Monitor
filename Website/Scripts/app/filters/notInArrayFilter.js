(function () {
    var filter = function ($filter) {
        return function (list, arrayFilter, element) {
            if (arrayFilter) {
                return $filter("filter")(list, function (listItem) {
                    var value = listItem[element];
                    for (var i = 0; i < arrayFilter.length; i++)
                        if (arrayFilter[i][element] === value)
                            return false;
                    return true;
                });
            }
        };
    };

    filter.$inject = ['$filter'];
    angular.module('main')
        .filter('notInArray', filter);
})()