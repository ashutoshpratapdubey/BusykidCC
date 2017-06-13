mjcApp.controller('subscriptionController', ['$scope', '$filter', '$state', '$uibModal', '$q', 'localStorageService', 'subscriptionService', 'bankAuthorizeService', 'familyMemberService', function ($scope, $filter, $state, $uibModal, $q, localStorageService, subscriptionService, bankAuthorizeService, familyMemberService) {

    // ******* Variables *********   
    $scope.promocode = '';
    $scope.trial = false;
    $scope.isLoading = false;
    $scope.hasPromoCode = false;
    $scope.isSubscribing = false;
    $scope.subscriptionDuration = '30 days';
    $scope.promoCodeButtonText = 'Have a promo code?';
    $scope.SubscriptionType = '';
    $scope.isExpaired = false;
    $scope.signUpStatus = {};
    $scope.subscription;
    $scope.selectedPromoCode = null;

    $scope.isSignUpProgress = {};
    $scope.isSignUpProgress.response = true;

    $scope.isFromTrial = false;
    $scope.isLinkedBank = false;
    $scope.isPendingAccount = false;

    // ******* Variables End *********   

    // ******* Methods *********   
    $scope.subscribe = function () {

        if ($scope.isSubscribing)
            return;

        if (!$scope.isFromTrial && !$scope.hasPromoCode && !$scope.isLinkedBank && !$scope.isPendingAccount) {
            $state.go('linkbankinfo', { action: 'subscription', promoCode: ($scope.selectedPromoCode === undefined || $scope.selectedPromoCode === null) ? null : $scope.selectedPromoCode.PromoCode });
            return;
        }

        var subscriptionName = $scope.SubscriptionType === 'NoSubscription' && $scope.isFromTrial
                ? subscriptionService.subscriptionTypes[1].Name : subscriptionService.subscriptionTypes[0].Name;

        if ($scope.subscription !== undefined) {
            var subscriptionType = $filter('filter')(subscriptionService.subscriptionTypes, { PlaneName: $scope.subscription.SubscriptionPlan.PlanName }, true)[0];
            subscriptionName = subscriptionType.Name;
        }

        if ($scope.hasPromoCode && $scope.selectedPromoCode.Duration !== 0) {
            subscriptionName = subscriptionService.subscriptionTypes[2].Name;
        }

        var subscription = {
            SubscriptionType: subscriptionName,
            PromoCode: ($scope.selectedPromoCode === undefined || $scope.selectedPromoCode === null) ? null : $scope.selectedPromoCode.PromoCode
        };

        $scope.isSubscribing = true;
        subscriptionService.Subscribe(subscription).success(function (response) {
            if (!$scope.isFromTrial) {
                $scope.gotoNextPage();
                return;
            }

            familyMemberService.markTrialAsUsed().success(function (response) {
                $scope.gotoNextPage();
            }).error(function (err, data) {
                $scope.isSubscribing = false;
                DisplayAlert(err, 'danger');
            });
        }).error(function (err, data) {
            $scope.isSubscribing = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.updatePromoCode = function (subscriptionPromoCode) {
       
        if (!subscriptionPromoCode) return;
        $scope.selectedPromoCode = subscriptionPromoCode;
        if ($scope.selectedPromoCode.Duration !== 0) {
            $scope.subscriptionDuration = $scope.selectedPromoCode.Duration + ' ' + $filter('lowercase')($scope.selectedPromoCode.DurationType) + (($scope.selectedPromoCode.Duration > 1) ? 's' : '');
            $scope.isExpaired = false;
        }
        $scope.hasPromoCode = $scope.selectedPromoCode.Duration !== 0;
    }

    $scope.GetSubscriptionStatus = function () {
        $scope.isLoading = true;

        var hasFreeTrial = familyMemberService.hasTrial();
        var getSubscriptionStatus = subscriptionService.GetSubscriptionStatus();

        $q.all([hasFreeTrial, getSubscriptionStatus]).then(function (response) {
            $scope.isFromTrial = response[0].data;
            $scope.SubscriptionType = response[1].data;

            bankAuthorizeService.IsBankLinked().success(function (response) {
                $scope.isLinkedBank = response;
            });

            $scope.isExpaired = ($scope.SubscriptionType == 'PendingCancellation' || $scope.SubscriptionType == 'Cancelled' || $scope.SubscriptionType == 'Expired' || $scope.SubscriptionType == 'TrialExpired');
            if ($scope.SubscriptionType === 'NoSubscription') {
                $scope.isLoading = false;
                return;
            }

            if ($scope.isExpaired) {
                subscriptionService.getFamilySubscription().success(function (response) {
                    if ($scope.SubscriptionType == 'PendingCancellation' || $scope.SubscriptionType == 'Cancelled')
                        $scope.subscription = response;

                    $scope.planName = '';
                    switch (response.SubscriptionPlan.PlanName) {
                        case 'One month free trial':
                            $scope.planName = 'trial';
                            break
                        case 'Promo plan':
                            $scope.planName = 'promo period';
                            break
                        case 'Annual':
                            $scope.planName = 'annual subscription';
                            break
                    }
                    $scope.isLoading = false;
                });
            } else {
                if ($scope.isChild)
                    $state.go('childDashboard');
                else
                    $state.go('adminDashboard');
                $scope.isLoading = false;
            }

        }, function (err) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.gotoNextPage = function () {
        bankAuthorizeService.IsBankLinked().success(function (response) {
            if ($scope.signUpStatus.data !== 'Completed') {
                (!response) ? $state.go('linkbankinfo') : $state.go('signupprogress');
            } else {
                $state.go('adminDashboard');
            }
        }).error(function (err, data) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.subscribePromoCode = function () {
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/subscription/promoCodeSubscriptionView.html',
            controller: 'promoCodeSubscriptionController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    // ******* Methods End *********  

    // ******* View Loading *********   

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.isAdmin = (authData.MemberType === 'Admin');
        $scope.isChild = (authData.MemberType === 'Child')
    }

    $scope.GetSubscriptionStatus();


    $scope.loadpromocodeURL = function () {
       
        bankAuthorizeService.GetPromocode().success(function (response) {
          
            var hasTrial = response.HasTrial;
            if (response != null && response.PromoCode != null) {
                subscriptionService.ValidatePromoCode(response.PromoCode).success(function (responseNew) {
                 
                    $scope.trial = hasTrial;
                    $scope.promocode = response.PromoCode;
                    $scope.isValidatingPromoCode = false;
                    $scope.updatePromoCode(responseNew);
                }).error(function (err, data) {
                    $scope.isValidatingPromoCode = false;
                    DisplayAlert(err, 'danger');
                });

            }
        });
    };

    // ******* View Loading End *********   
}]);