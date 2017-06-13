mjcApp.factory('goToDashboardService', ['$http', '$q', 'localStorageService', 'settings', '$state', '$location', '$window', function ($http, $q, localStorageService, settings, $state, $location, $window) {

    var goToDashboardService = {};
    goToDashboardService.LoginforDashboard = function (FamilyMemberId, memberType, FamilyUrl) {
        return $http.put(settings.apiBaseUri + 'api/account/LoginforDashboard/' + FamilyMemberId + '/' + memberType + '/' + FamilyUrl);
    }
    
   return goToDashboardService;
}]);