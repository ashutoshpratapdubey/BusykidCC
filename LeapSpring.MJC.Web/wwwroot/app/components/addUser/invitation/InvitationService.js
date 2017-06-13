mjcApp.factory('invitationService', ['$http', '$q', 'localStorageService', 'settings', function ($http, $q, localStorageService, settings) {

    var invitationService = {};

    invitationService.invite = function (invitation) {
        return $http.post(settings.apiBaseUri + 'api/invitation/add', invitation);
    }

    invitationService.getInvitationByToken = function (token) {
        return $http.get(settings.apiBaseUri + 'api/invitation/getinvitationbytoken/' + token);
    }

    invitationService.UpdateInvitationStatus = function (id, statusId) {
        return $http.put(settings.apiBaseUri + 'api/invitation/updateinvitationstatus?invitationId=' + id + '&invitationStatus=' + statusId);
    }

    return invitationService;
}]);