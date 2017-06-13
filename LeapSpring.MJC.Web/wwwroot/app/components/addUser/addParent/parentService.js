mjcApp.factory('parentService', ['$http', '$q', 'localStorageService', 'settings', function ($http, $q, localStorageService, settings) {

    var parentService = {};

    // Add parent
    parentService.addParent = function (member) {
        return $http.post(settings.apiBaseUri + 'api/family/add/', member);
    };

    // Upload photo
    parentService.uploadPhoto = function (profileImage) {
        return $http.put(settings.apiBaseUri + 'api/family/uploadimage/', profileImage);
    };

    // Parent signup
    parentService.signUp = function (signup) {
        return $http.post(settings.apiBaseUri + 'api/account/signup', signup);
    }

    return parentService;
}]);