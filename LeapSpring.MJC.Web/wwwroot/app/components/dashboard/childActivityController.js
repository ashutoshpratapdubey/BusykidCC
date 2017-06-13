mjcApp.controller('childActivityController', ['$scope', '$q', '$filter', '$state', '$timeout', '$uibModal', 'localStorageService', 'dashboardService', 'choreService', function ($scope, $q, $filter, $state, $timeout, $uibModal, localStorageService, dashboardService, choreService) {

    // ***** Variables *****
    $scope.childId = undefined;
    $scope.switchChild = false;

    $scope.AllTransactions = [];
    $scope.AllowanceIn = [];
    $scope.AllowanceOut = [];

    $scope.isAllTransactionsLoading = true;
    $scope.isAllowanceInLoading = false;
    $scope.isAllowanceOutLoading = false;

    $scope.hasAllTransactions = false;
    $scope.hasAllowanceIn = false;
    $scope.hasAllowanceOut = false;

    $scope.showAllTransactions = true;
    $scope.showAllowanceIn = false;
    $scope.showAllowanceOut = false;
    $scope.choreID = undefined;
    // ***** Variables *****

    // *** Methods *****


    // Get all transactions of a child
    $scope.getAllTransactions = function () {
        var element = angular.element(document.querySelector('#allowanceInTab'))
        element.addClass('remove-navbar-border-left');
        element.removeClass('remove-navbar-border-right');

        $scope.showAllTransactions = true;
        $scope.showAllowanceIn = false;
        $scope.showAllowanceOut = false;

        $scope.isAllTransactionsLoading = true;
        dashboardService.getAllTransactions($scope.childId).success(function (response) {
            $scope.hasAllTransactions = false;
            angular.forEach(response, function (transactions) {
                $scope.hasAllTransactions = true;
                angular.forEach(transactions, function (transaction) {
                    transaction.ImageUrl = '../../../images/success-small.png';
                    if (transaction.TransactionHistoryType !== 'AllowanceIn')
                        transaction.ImageUrl = '../../../images/star.png';
                });
            });
            $scope.AllTransactions = response;
            $scope.isAllTransactionsLoading = false;
        }).error(function (err) {
            $scope.AllTransactions = [];
            $scope.hasAllTransactions = false;
            $scope.isAllTransactionsLoading = false;
        });
    };

    //Remove Approval from chore

    $scope.authData = localStorageService.get('authorizationData');
    //if (authData != null) {
    //   
    //    $scope.isAdmin = (authData.MemberType === 'Admin');
    //    $scope.isChild = (authData.MemberType === 'Child')
    //}

    $scope.RemoveAproval = function (choreID) {

        $scope.choreID = choreID;
        dashboardService.removeAprovalService($scope.choreID).success(function (response) {

            $scope.getAllTransactions();
        }).error(function (err) {
            $scope.AllTransactions = [];
            $scope.hasAllTransactions = false;
            $scope.isAllTransactionsLoading = false;
        });
    };

    $scope.ApproveForPayeday = function (choreID) {

        $scope.choreID = choreID;
        dashboardService.ApproveForPayedayService($scope.choreID).success(function (response) {

            $scope.getAllTransactions();
        }).error(function (err) {
            $scope.AllTransactions = [];
            $scope.hasAllTransactions = false;
            $scope.isAllTransactionsLoading = false;
        });
    };

    // Get allowance in
    $scope.getAllowanceIn = function () {
        var childId = undefined;

        $scope.showAllTransactions = false;
        $scope.showAllowanceIn = true;
        $scope.showAllowanceOut = false;

        $scope.isAllowanceInLoading = true;
        dashboardService.getAllowanceIn($scope.childId).success(function (response) {
            $scope.hasAllowanceIn = false;
            angular.forEach(response, function (transactions) {
                $scope.hasAllowanceIn = true;
                angular.forEach(transactions, function (transaction) {
                    transaction.ImageUrl = '../../../images/success-small.png';
                });
            });
            $scope.AllowanceIn = response;
            $scope.isAllowanceInLoading = false;
        }).error(function (err) {
            $scope.AllowanceIn = [];
            $scope.isAllowanceInLoading = false;
            $scope.hasAllowanceIn = false;
        });
    };

    // Get allowance out
    $scope.getAllowanceOut = function () {
        var element = angular.element(document.querySelector('#allowanceInTab'))
        element.removeClass('remove-navbar-border-left');
        element.addClass('remove-navbar-border-right');

        $scope.showAllTransactions = false;
        $scope.showAllowanceIn = false;
        $scope.showAllowanceOut = true;

        $scope.isAllowanceOutLoading = true;
        dashboardService.getAllowanceOut($scope.childId).success(function (response) {
            $scope.hasAllowanceOut = false;
            angular.forEach(response, function (transactions) {
                $scope.hasAllowanceOut = true;
                angular.forEach(transactions, function (transaction) {
                    transaction.ImageUrl = '../../../images/star.png';
                });
            });

            $scope.AllowanceOut = response;
            $scope.isAllowanceOutLoading = false;
        }).error(function (err) {
            $scope.AllowanceOut = [];
            $scope.hasAllowanceOut = false;
            $scope.isAllowanceOutLoading = false;
        });
    };

    // ******** Activity Tab Methods Ends ************

    // *** Methods *****

    // ***** View Loading *****

    $scope.$on('onChildChanged', function () {
        $scope.switchChild = true;
    });

    $scope.$on('loadAllTransactions', function (event, args) {
        $scope.childId = (args !== undefined) ? args.childId : undefined;
        $scope.getAllTransactions();
    });

    // ***** View Loading *****
}]);
