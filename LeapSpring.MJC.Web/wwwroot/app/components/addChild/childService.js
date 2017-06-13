mjcApp.factory('childService', ['$http', '$q', 'localStorageService', 'settings', function ($http, $q, localStorageService, settings) {

    var childService = {};

    // Add child
    childService.addChild = function (member) {
        return $http.post(settings.apiBaseUri + 'api/family/add/', member);
    };

    // Get child by id
    childService.getMemberById = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/family/getbyid/' + memberId);
    };

    // Update Phone Number
    childService.updatePhoneNumber = function (phoneNumber, memberId) {
        return $http.put(settings.apiBaseUri + 'api/family/updatememberphone?phonenumber=' + phoneNumber + '&memberId=' + memberId);
    };

    // Uploa photo
    childService.uploadPhoto = function (profileImage) {
        return $http.put(settings.apiBaseUri + 'api/family/uploadimage/', profileImage);
    };
    return childService;
}]);