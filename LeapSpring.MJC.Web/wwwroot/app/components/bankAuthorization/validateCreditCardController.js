mjcApp.controller('validateCreditCardController', ['$scope', '$http', '$state', '$stateParams', '$filter', 'localStorageService', 'bankAuthorizeService', '$timeout', '$rootScope', '$uibModal', '$q', 'subscriptionService', 'familyMemberService', function ($scope, $http, $state, $stateParams, $filter, localStorageService, bankAuthorizeService, $timeout, $rootScope, $uibModal, $q, subscriptionService, familyMemberService) {

    $scope.isLoading = true;
    $scope.btnContinue = 'Continue';
    $scope.btnBack = 'Back';   
    $scope.PageReload = function () { if ($rootScope.CardDigit == null || $rootScope.CardDigit == 'undefined' || $rootScope.CardDigit == '') { $state.go('addCreditCard'); } }
    $scope.PageReload();
    $scope.goToNext = function () {

        $scope.CardValidity();

        if ($scope.isShowAllError != true && $scope.isShowValidityError != true && $scope.isShowRegExpError != true) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';


            bankAuthorizeService.ValidateCreditCard().then(function (response) {
                if (response != null) {
                    $rootScope.apiurl = response.data;
                    var splitUrl = $rootScope.apiurl.split('/');
                    var tokenID = splitUrl[6];
                    var vrExp = $scope.Month + '' + $scope.Year;
                    var cardtype = $rootScope.CardType;                    
                    $.post($rootScope.apiurl,
                    {
                        cc_number: $rootScope.lastDigit,
                        cc_exp: vrExp
                    },
                    function (data, status) {
                        alert("Data: " + data + "\nStatus: " + status);
                    });

                    bankAuthorizeService.ValidateThreeStepApiUrl(tokenID, cardtype).then(function (response) {                        
                        if ($rootScope.replacecard != "replacecard") {
                            $scope.subscribeFamily();
                            if ($rootScope.promoCodeValueAuth != null) {
                                $state.go('adminDashboard', { promoCode: $rootScope.promoCodeValueAuth });
                            }
                            else {
                                $state.go('adminDashboard');
                            }
                        } else {
                            $state.go('adminDashboard');
                        }
                    }, function (err) {
                       DisplayAlert(err.data, 'danger');
                        $state.go('addCreditCard');

                    });

                }
            }, function (err) {
                $scope.isProcessing = false;
                DisplayAlert(err.data, 'danger');
            });
        }
    }
    $scope.change = function (e) {
        var numChars = $scope.MonthYear.length;
        if (numChars === 2) {
            var thisVal = $scope.MonthYear;
            thisVal += '/';
            $scope.MonthYear = thisVal;
        }
    };

    //Subscribe Flow

    $scope.subscribeFamily = function () {
        if ($rootScope.checkpagestate == 1) {
            bankAuthorizeService.ChangeCreditCardStatus().success(function (response) {
            }).error(function (err, data) {
                $scope.familyMember = angular.copy($scope.familyMemberCopy);
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
    }

    $scope.CardValidity = function () {

        if ($scope.MonthYear != null) {
            var val = $scope.MonthYear;

            var CardDate = val.split('/')
            $scope.Month = CardDate[0];
            $scope.Year = CardDate[1];
        }
        var numbers = new RegExp(/^[0-9]+$/);
        var code = $scope.CVV;

        if ($scope.MonthYear == null || $scope.CVV == null) { $scope.isShowAllError = true; $scope.isShowValidityError = false; $scope.isShowRegExpError = false; }
        else if ($scope.Month > 12 || $scope.Month < 01 || $scope.Year < 17 || $scope.Year > 25) { $scope.isShowValidityError = true; $scope.isShowAllError = false; $scope.isShowRegExpError = false; }
        else if (numbers.test($scope.CVV) && numbers.test($scope.Month) && numbers.test($scope.Year)) { $scope.isShowRegExpError = false; $scope.isShowAllError = false; $scope.isShowValidityError = false; }
        else { $scope.isShowAllError = false; $scope.isShowValidityError = false; $scope.isShowRegExpError = true; }
    }


    $scope.goBack = function () {
        $state.go('addCreditCard');
    }


}]);

