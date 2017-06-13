mjcApp.controller('addMicroDepositAccountController', ['$scope', '$state', 'bankAuthorizeService', function ($scope, $state, bankAuthorizeService) {

    $scope.isLoading = false;
    $scope.isProcessing = false;
    $scope.account = {
        SelectedAccountType: 'Checking'
    };
    $scope.accountTypes = ['Checking', 'Savings']

    $scope.createMicroDepositAccount = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.linkMicroDepositForm.$valid) {
            $scope.isProcessing = true;
            bankAuthorizeService.linkMicroDepositAccount($scope.account.AccountNumber, $scope.account.RoutingNumber, $scope.account.SelectedAccountType).then(function (response) {
                $scope.isProcessing = false;
                $scope.gotoNextPage();
            }, function (err) {
                $scope.isProcessing = false;
                DisplayAlert(err.data, 'danger');
            });
        }
    }

    $scope.gotoNextPage = function () {
        $state.go('connectionSuccess');
    }

    $scope.changeAccountType = function (accountType) {
        $scope.account.SelectedAccountType = accountType;
    }
}]);



