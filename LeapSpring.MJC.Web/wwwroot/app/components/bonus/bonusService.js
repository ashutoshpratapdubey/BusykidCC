mjcApp.factory('bonusService', ['$http', '$q', '$filter', 'settings', function ($http, $q, $filter, settings) {

    var bonusService = {};

    bonusService.sendBonus = function (bonus) {
        return $http.put(settings.apiBaseUri + 'api/earnings/sendbonus', bonus);
    }

    return bonusService;
}]);
