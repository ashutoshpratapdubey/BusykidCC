mjcApp.factory('charityService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var charityService = {};

    charityService.getCharities = function () {
        return $http.get(settings.apiBaseUri + 'api/charity/getcharities');
    };

    charityService.donate = function (donation) {
        return $http.post(settings.apiBaseUri + 'api/charity/donate', donation);
    };

    return charityService;
}]);

