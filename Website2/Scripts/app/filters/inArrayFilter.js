(function () {
    var simpleInArray = function ($filter) {
        return function (list, arrayFilter, element) {
            if (arrayFilter) {
                return $filter("filter")(list, function (listItem) {
                    var isFound = arrayFilter.indexOf(listItem[element]) != -1;
                    return isFound;
                });
            }
        };
    };

    simpleInArray.$inject = ['$filter'];
    angular.module('main')
        .filter('simpleInArray', simpleInArray);

    var complexInArray = function ($filter) {
        return function (list, arrayFilter, element) {
            if (arrayFilter) {
                return $filter("filter")(list, function (listItem) {
                    var value = listItem[element];
                    for (var i = 0; i < arrayFilter.length; i++)
                        if (arrayFilter[i][element] === value)
                            return true;
                    return false;
                });
            }
        };
    };

    complexInArray.$inject = ['$filter'];
    angular.module('main')
        .filter('complexInArray', complexInArray);
})()
