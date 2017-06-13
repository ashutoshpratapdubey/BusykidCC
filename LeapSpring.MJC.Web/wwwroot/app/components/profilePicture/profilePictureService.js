mjcApp.factory('profilePictureService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var profilePictureService = {};

    // Update Phone Number
    profilePictureService.uploadPhoto = function (profileImage) {
        return $http.put(settings.apiBaseUri + 'api/family/uploadimage/', profileImage);
    };


    ///******* Chore Service Start *****/////

    profilePictureService.getSystemChores = function (familyMemberId, keyWord) {
        return $http.get(settings.apiBaseUri + 'api/chore/searchchores?familyMemberId=' + familyMemberId + '&keyword=' + keyWord);
    };

    profilePictureService.addChore = function (chore) {
        return $http.post(settings.apiBaseUri + 'api/chore/addchore', chore);
    };

    ///******* Chore Service End *****/////
    return profilePictureService;
}]);