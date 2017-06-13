mjcApp.factory('dashboardService', ['$http', 'settings', '$state', function ($http, settings, $state) {

    var dashboardService = {};

    dashboardService.getChildById = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/family/getbyid/' + memberId);
    };

    // Upload photo
    dashboardService.uploadPhoto = function (profileImage) {
        return $http.put(settings.apiBaseUri + 'api/family/uploadimage/', profileImage);
    };

    // Gets the financial overview of a child
    dashboardService.getChildFinacialOverview = function (weekDayName, familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/earnings/getchildfinancialoverview/' + weekDayName + '/' + familyMemberId);
    };

    /// ************************** Activity Services  **************************

    // Gets all the transactions done by a child
    dashboardService.getAllTransactions = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/transactionhistory/getalltransactions/' + familyMemberId);
    };

    // Gets only the incoming transactions
    dashboardService.getAllowanceIn = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/transactionhistory/getallowancein/' + familyMemberId);
    };

    // Gets only the outgoing transactions
    dashboardService.getAllowanceOut = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/transactionhistory/getallowanceout/' + familyMemberId);
    };

    // Gets only the outgoing transactions
    dashboardService.removeAprovalService = function (choreId) {
        return $http.get(settings.apiBaseUri + 'api/earnings/removeapproval/' + choreId);
    };

    dashboardService.ApproveForPayedayService = function (choreId) {
        return $http.get(settings.apiBaseUri + 'api/earnings/approveforpayeday/' + choreId);
    };
 
    /// ************************** Activity Services End **************************
    return dashboardService;
}]);