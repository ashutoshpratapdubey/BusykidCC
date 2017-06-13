/// <reference path="../account/myaccountview.html" />
mjcApp.controller('AddExternalAccountController', ['$scope', '$state', '$uibModal', 'NonplaidbankAuthorizeService', '$rootScope', 'chkstatetype', function ($scope, $state, $uibModal, NonplaidbankAuthorizeService, $rootScope, chkstatetype) {
    $scope.isLoading = false;
    $scope.isProcessing = false;
    $scope.account = {
        SelectedAccountType: 'Checking',
        AccountNumber: '',
        RoutingNumber: ''
    };
     
    $scope.accountTypesselect = ['Checking', 'Savings']

    //  $scope.authorizeAccount = function () { $state.go('adminDashboard'); }
    $scope.checkstate = chkstatetype;
    $rootScope.checkpagestate = chkstatetype;
    $scope.createMicroDepositAccount = function () {

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.AddExternalAccountForm.$valid) {
            $scope.isProcessing = true;
            NonplaidbankAuthorizeService.linkMicroDepositAccount($scope.account.AccountNumber, $scope.account.RoutingNumber, $scope.account.SelectedAccountType).then(function (response) {
                $scope.isProcessing = true;
                $rootScope.modalInstance.close();

                var str = $scope.account.AccountNumber;
                $rootScope.Lastdigit = str.substr(-4);

                $rootScope.modalInstance = $uibModal.open({
                    scope: $rootScope,
                    templateUrl: '/app/components/addAccount/AuthorizeExternalAccount.html',
                    controller: 'AuthorizeExternalAccountController',
                    size: 'sm',
                    windowClass: 'box-popup'
                });
            }, function (err) {
                DisplayAlert(err.data, 'danger');
            });
        }

    }
    $scope.cancelBankAuthorzation = function () {      
        if ($scope.checkstate == 1) {
            $scope.isProcessing = true;
            $state.reload();
            $rootScope.modalInstance.close();
        } else {
            $scope.isProcessing = true;
            $state.go("adminDashboard");
            $rootScope.modalInstance.close();
        }
      
        //var path = "#/adminDashboard";
        //window.location.href = path;
    }

    $scope.gotoNextPage = function () {
        $state.go('connectionSuccess');
    }
    $scope.changeAccountType = function (accountType) {
        $scope.account.SelectedAccountType = accountType;
    }


}]);



