mjcApp.controller('verifyMicroDepositController', ['$scope', '$state', '$stateParams', 'bankAuthorizeService', function ($scope, $state, $stateParams, bankAuthorizeService) {

    $scope.isLoading = true;
    $scope.financialAccount = {};
    $scope.isVerifying = false;
    $scope.verificationType = "InstantAccount";
    $scope.verifyDepositAmounts = {};
    $scope.isShowAmountsError = false;

    // Check is this action update
    $scope.isUpdateProgress = $stateParams.action != null && $stateParams.action == 'update';

    bankAuthorizeService.GetFinancialAccount().success(function (response) {
        if (response == null) {
            DisplayAlert('Please link your bank', 'danger');
            $state.go('adminDashboard');
        }

        $scope.financialAccount = response;
        $scope.isLoading = false;
    }).error(function (err, data) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.verifyExternalAccount = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.verifyMicroDepositForm.$valid) {
            $scope.isVerifying = true;
            var amountOne = '0.' + $scope.verifyDepositAmounts.AmountOne;
            var amountTwo = '0.' + $scope.verifyDepositAmounts.AmountTwo;

         //   verify   
            bankAuthorizeService.verifyBankAccount(amountOne, amountTwo).then(function (response) {
                $scope.isVerifying = false;
                $scope.gotoNextPage();
            }, function (err) {
                $scope.isVerifying = false;
                DisplayAlert(err.data, 'danger');
            });
        } else
            $scope.isShowAmountsError = true;
    }

    $scope.ChangeAmount = function () {
        if ($scope.verifyDepositAmounts.AmountOne == null && $scope.verifyDepositAmounts.AmountOne == null)
            $scope.isShowAmountsError = true;
    }

    $scope.gotoNextPage = function () {
        $state.go('connectionSuccess', { action: 'update' });
    }

}]);


