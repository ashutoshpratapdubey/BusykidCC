mjcApp.factory('phoneEntryService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var phoneEntryService = {};

    // Update Phone Number
    phoneEntryService.updatePhoneNumber = function (phoneNumber) {
        return $http.put(settings.apiBaseUri + 'api/family/updateadminphone?phonenumber=' + phoneNumber);
    };

    // Get OTP
    phoneEntryService.getVerificationCode = function (phoneNumber) {
        return $http.get(settings.apiBaseUri + 'api/phoneverification/getverificationcode/' + phoneNumber);
    };

    // Verify OTP
    phoneEntryService.verifyCode = function (verficationCode) {
        return $http.put(settings.apiBaseUri + 'api/phoneverification/verifyCode/', verficationCode);
    };

    return phoneEntryService;
}]);