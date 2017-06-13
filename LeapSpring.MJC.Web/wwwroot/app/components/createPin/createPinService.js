mjcApp.factory('createPinService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var pinService = {};

    // Update PIN
    pinService.updateAdminPIN = function (pin) {
        return $http.put(settings.apiBaseUri + 'api/family/updateadminpin?pin=' + pin);
    };

    // Update PIN
    pinService.updateMemberPIN = function (pin, memberId) {
        return $http.put(settings.apiBaseUri + 'api/family/updatememberpin?pin=' + pin + '&memberId=' + memberId);
    };

    return pinService;
}]);