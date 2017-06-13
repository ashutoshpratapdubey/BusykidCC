mjcApp.controller('linkBankController', ['$scope', '$state', '$stateParams', '$filter', 'localStorageService', 'bankAuthorizeService', '$timeout', '$rootScope', '$uibModal', '$q', 'subscriptionService', 'familyMemberService', function ($scope, $state, $stateParams, $filter, localStorageService, bankAuthorizeService, $timeout, $rootScope, $uibModal, $q, subscriptionService, familyMemberService) {

    $scope.isLoading = true;
    $scope.currentUser = {};
    $scope.isConnectionSuccess = false;
    $scope.isMicroDepositAccount = false;
    $scope.comeBackText = 'I’ll come back to this, let’s assign some chores!';
    $scope.linkedBankDetails = {};
    $scope.bankDocuments = [];
    $scope.selectedAccountType = 0;
    $scope.isExpaired = false;
    $scope.isSignUpProgress = {};
    $scope.isSignUpProgress.response = true;
    $stateParams.PromoCodevalue = null;
    $rootScope.promocode = null;
    // To move subscribe functionality here
    $scope.isFromTrial = false;
    $scope.isLinkedBank = false;
    $scope.isPendingAccount = false;
    // To move subscribe functionality here
    // Check is this action update
    $scope.isUpdateProgress = $stateParams.action != null && $stateParams.action == 'update';

    $scope.AuthData = localStorageService.get('authorizationData');


    // Creates the plaid instance handler
    $scope.linkHandler = Plaid.create({
        selectAccount: true,
        env: 'tartan',
        //  env: 'production',
        clientName: 'Busykid',
        key: '428e241fd1b4c6a53b6378050afdeb', // plaid public key
        product: 'auth',
        // To use Link with longtail institutions on Connect, set the
        // 'longtail' option to true:
        // longtail: true,
        onLoad: function () {
            // The Link module finished loading.
        },
        onSuccess: function (public_token, metadata) {
            // Send the public_token to your app server here.
            // The metadata object contains info about the institution the
            // user selected and the account ID, if selectAccount is enabled.
            bankAuthorizeService.linkBankAccount(public_token, metadata.institution.name, metadata.account_id, false).success(function (response) {
                if ($stateParams.action == 'subscription') {
                    $scope.subscribe();
                }
                else {
                    //$scope.isLoading = false;
                    $scope.subscribe();
                    $scope.gotoConnectionSuccess();
                }
            }).error(function (err) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
                $state.go('linkbankinfo');
            });
        },
        onExit: function (err, metadata) {

            if ($stateParams.action == 'update') { $state.go('myAccount'); }
            else {
                $rootScope.modalInstance = $uibModal.open({
                    scope: $rootScope,
                    templateUrl: '/app/components/addAccount/AddExternalAccount.html',
                    controller: 'AddExternalAccountController',
                    resolve: {
                        chkstatetype: function () {
                            return 0;
                        }
                    },
                    size: 'sm',
                    windowClass: 'box-popup'
                });
            }

        }
    });
   
    if ($state.includes('linkbankinfo')) {
        var createCustomer = bankAuthorizeService.createCustomer();
        //var getBankDocuments = bankAuthorizeService.GetBankDocuments();

        $q.all([createCustomer]).then(function (response) {
            //$scope.bankDocuments = response[0].data;
            $scope.isLoading = false;
        }, function (err) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.gotoConnectionSuccess = function () {
        if ($scope.isUpdateProgress)
            $state.go('connectionSuccess', { action: $stateParams.action });
        else
            $state.go('connectionSuccess');
    }

    $scope.moveToNextPage = function () {

        if ($stateParams.PromoCodevalue != null)
        {
            $rootScope.promocode = $stateParams.PromoCodevalue;
        }

        if ($scope.isUpdateProgress) {
            if ($stateParams.PromoCodevalue != null) {         
                $state.go('adminDashboard', { promoCode: $stateParams.PromoCodevalue });
            }
            else {
                $state.go('adminDashboard');
            }
        }
        else
            $state.go('signupprogress');
    }

    $scope.gotoLinkBankPage = function () {
        if (parseInt($scope.selectedAccountType) === 0) {
            //if ($scope.isUpdateProgress)
            //    $state.go('linkbank', { action: 'update' });
            //else
            //    $state.go('linkbank');
            $scope.linkHandler.open();
        }
        else if (parseInt($scope.selectedAccountType) === 1)
            $state.go('addmicrodepositaccount');
    }

    $scope.selectVerificationType = function () {
        //$scope.modalInstance = $uibModal.open({
        //    scope: $scope,
        //    templateUrl: '/app/components/bankAuthorization/chooseBankTypeView.html',
        //    controller: 'linkBankController',
        //    size: 'sm',
        //    windowClass: 'box-popup'
        //});

        $scope.selectedAccountType = 0;
        //$state.go('linkbank');
        $scope.linkHandler.open();
        $scope.isLoading = true;
    };

    $scope.subscribe = function () {
        var subscription = {
            SubscriptionType: subscriptionService.subscriptionTypes[0].Name,
            PromoCode: ($stateParams.promoCode === undefined || $stateParams.promoCode === null) ? null : $stateParams.promoCode
        };

        subscriptionService.Subscribe(subscription).success(function (response) {
            $scope.gotoConnectionSuccess();
        }).error(function (err, data) {
            DisplayAlert(err, 'danger');
        });
    }

    if ($state.includes('connectionSuccess')) {
        bankAuthorizeService.GetLinkedBankStatus().success(function (response) {
            $scope.isLoading = false;
            $scope.linkedBankDetails = response;
            $scope.isMicroDepositAccount = (response.BankStatus == "Unverified" || response.BankStatus == "VerifyLocked" || response.BankStatus == "Expired" || response.BankStatus == "Denied");
        }).error(function (err) {
            DisplayAlert(err, 'danger');
        });
    }

    // Getting promo code for script

    $scope.loadPromocodePlaid = function () {

        bankAuthorizeService.GetPromocode().success(function (response) {
            if (response != null && response.PromoCode != null) {
                $stateParams.PromoCodevalue = response.PromoCode;
                $stateParams.promoCode = response.PromoCode;

            }
        });
    }

    $scope.loadPromocodePlaid();
    /***To Show  Vebriage *****/

    $scope.showVerbiageValue = function () {
        
        bankAuthorizeService.GetPromocode().success(function (response) {

            var hasTrial = response.HasTrial;
            if (response != null && response.PromoCode != null) {
                subscriptionService.ValidatePromoCode(response.PromoCode).success(function (responseNew) {

                    $scope.trial = hasTrial;
                    $scope.promocode = response.PromoCode;
                    $stateParams.promoCode = response.PromoCode;
                    $scope.isValidatingPromoCode = false;
                    $scope.updatePromoCode(responseNew);
                }).error(function (err, data) {
                    $scope.isValidatingPromoCode = false;
                    DisplayAlert(err, 'danger');
                });

            }
        });
    };

    $scope.showVerbiageValue();

    $scope.updatePromoCode = function (subscriptionPromoCode) {
        
        if (!subscriptionPromoCode) return;
        $scope.selectedPromoCode = subscriptionPromoCode;
        if ($scope.selectedPromoCode.Duration !== 0) {
            $scope.subscriptionDuration = $scope.selectedPromoCode.Duration + ' ' + $filter('lowercase')($scope.selectedPromoCode.DurationType) + (($scope.selectedPromoCode.Duration > 1) ? 's' : '');
            $scope.isExpaired = false;
        }
        $scope.hasPromoCode = $scope.selectedPromoCode.Duration !== 0;
    }


    /*********/

    $scope.enterCreditCardDetails = function () {
        $state.go('addCreditCard');
    }



}]);

