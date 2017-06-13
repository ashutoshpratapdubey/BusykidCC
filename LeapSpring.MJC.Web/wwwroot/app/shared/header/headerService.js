mjcApp.factory('headerService', ['$http', 'settings', '$state', function ($http, settings, $state) {

    var headerService = {};

    headerService.getAllMembers = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getallmembers');
    };

    return headerService;
}]);