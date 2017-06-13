mjcApp.factory('settingsService', ['$http', 'settings', function ($http, settings) {

    var settingsService = {};

    settingsService.getAllocationSettings = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/settings/getallocationsettings/' + familyMemberId);
    }

    settingsService.getAllocationByAge = function (age) {
        return $http.get(settings.apiBaseUri + 'api/settings/getallocationbyage/' + age);
    }

    settingsService.updateAllocationSettings = function (allocationSettings) {
        return $http.put(settings.apiBaseUri + 'api/settings/updateallocationsettings', allocationSettings);
    }

    return settingsService;
}]);