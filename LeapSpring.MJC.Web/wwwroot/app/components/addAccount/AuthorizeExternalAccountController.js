mjcApp.controller('AuthorizeExternalAccountController', ['$scope', '$state', '$uibModal', 'NonplaidbankAuthorizeService', '$rootScope', 'subscriptionService', '$stateParams','bankAuthorizeService', function ($scope, $state, $uibModal, NonplaidbankAuthorizeService, $rootScope, subscriptionService, $stateParams,bankAuthorizeService) {
    $scope.isLoading = false;
    $scope.isProcessing = false;
    $rootScope.promoCodeValueAuth = null;

    $scope.loadPromocodeaddAccount = function () {

        NonplaidbankAuthorizeService.GetPromocode().success(function (response) {

            if (response != null && response.PromoCode != null) {
                $rootScope.promoCodeValueAuth = response.PromoCode;

            }
        });
    }

    $scope.loadPromocodeaddAccount();

    $scope.authorizeAccount = function () {       
        $scope.subscribeFamily();
        $scope.isProcessing = true;
        $rootScope.modalInstance.close();

        if ($rootScope.promoCodeValueAuth != null) {
            $state.go('adminDashboard', { promoCode: $rootScope.promoCodeValueAuth });
        }
        else {
            $state.go('adminDashboard');
        }
    }


    //$scope.createMicroDepositAccount = function () {
    //    alert(11);
    //    
    //    //
    //    $rootScope.modalInstance.close();
    //    $scope.$broadcast('show-errors-check-validity');
    //    if ($scope.AddExternalAccountForm.$valid) {
    //        $scope.isProcessing = true;
    //        $state.go('adminDashboard');
    //        //NonplaidbankAuthorizeService.linkMicroDepositAccount($scope.account.AccountNumber, $scope.account.RoutingNumber, $scope.account.SelectedAccountType).then(function (response) {
    //        //   $scope.isProcessing = false;
    //        //    //$scope.gotoNextPage();
    //        //   $state.go('connectionSuccess');
    //        //}, function (err) {
    //        //  DisplayAlert(err.data, 'danger');
    //        //});
    //    }
    //}


    $scope.cancelBankAuthorzation = function () {
        $scope.isProcessing = true;
        $rootScope.modalInstance.close();
        window.location.href = "#/adminDashboard";
    }


    $scope.gotoNextPage = function () {
        $state.go('connectionSuccess');
    }
    $scope.changeAccountType = function (accountType) {
        $scope.account.SelectedAccountType = accountType;
    }


    $scope.subscribeFamily = function () {
       
        if ($rootScope.checkpagestate == 1) {
            bankAuthorizeService.ChangeCreditCardStatus().success(function (response) {
            }).error(function (err, data) {
                $scope.familyMember = angular.copy($scope.familyMemberCopy);
                //$scope.saveBtnTxt = 'Save Settings';
                $scope.isUpdating = false;
                DisplayAlert(err, 'danger');
            });
           

        } else {
            var subscription = {
                SubscriptionType: subscriptionService.subscriptionTypes[3].Name,
                PromoCode: ($rootScope.promoCodeValueAuth === undefined || $rootScope.promoCodeValueAuth === null) ? null : $rootScope.promoCodeValueAuth
            };

            subscriptionService.Subscribe(subscription).success(function (response) {
                $scope.isPendingAccount = true;

            }).error(function (err, data) {
                //   DisplayAlert(err, 'danger');
            });

        }


        //}
    }




}]);



