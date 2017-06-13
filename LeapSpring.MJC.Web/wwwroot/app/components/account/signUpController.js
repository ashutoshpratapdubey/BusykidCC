mjcApp.controller('signUpController', ['$scope', 'accountService', '$state', '$stateParams', '$rootScope', '$rememberPromoCode', '$timeout', function ($scope, accountService, $state, $stateParams, $rootScope, $rememberPromoCode, $timeout) {

    $scope.inputType = 'password';
    $scope.btnSignUp = 'Accept & Sign up';
    $scope.isSignUp = false;
    $scope.isAcceptTerms = false;
    $scope.passwordValidationError = 'Password is required.';
    $scope.isWrongPassword = false;
    var Isproceed = 1;
    var IsDisplay = 1;
    $scope.showPassword = function () {
        if ($scope.member != undefined && $scope.member.Password != undefined && $scope.member.Password.length > 0) {
            if ($scope.inputType == 'password')
                $scope.inputType = 'text';
            else
                $scope.inputType = 'password';
        }
    }

    $scope.getPromoCode = function () {

        if ($stateParams.offer != null && $stateParams.offer != '') {
            var storePromocode = $stateParams.offer

            accountService.ValidatePromoCode($stateParams.offer).then(function (response) {
                $stateParams.offer = storePromocode;

                $rememberPromoCode('Promocode', $stateParams.offer);

                IsDisplay = 0;
            }, function (err) {
                if ($stateParams.offer == 'trial') {
                    $rememberPromoCode('Promocode', null);
                    $stateParams.offer = 'Organic'
                    IsDisplay = 1;
                }
                else {
                    //  alert("unvalidated code");
                    $rememberPromoCode('Promocode', "");
                    $stateParams.offer = 'Organic'
                    IsDisplay = 1;
                }
            });
        }


        if ($rememberPromoCode('Promocode') != null && $rememberPromoCode('Promocode') != '') {
            $stateParams.offer = $rememberPromoCode('Promocode');
            IsDisplay = 0;

        }


    }

    $scope.getPromoCode();

    $scope.ShowPromocode = function () {
        if (IsDisplay == 1) {
            return true;
        }
        else {
            return false;
        }
    }


    $scope.signUp = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.signupForm.$valid) {
            if (!$scope.isAcceptTerms) {
                DisplayAlert("Please agree to the Terms & Privacy Policy.", 'danger');
                return;
                $state.reload();
            }
            $scope.member.HasTrial = false;

            if ($stateParams.offer != undefined && $stateParams.offer != null && $stateParams.offer != '') {
                if ($stateParams.offer == 'trial' || $stateParams.offer == 'TRIAL') {
                    $scope.member.HasTrial = true;
                    if ($scope.member.PromoCodeValue != undefined && $scope.member.PromoCodeValue != null) {
                        $scope.member.promoCode = $scope.member.PromoCodeValue;
                    }
                    else {
                        $scope.member.promoCode = 'Organic';
                    }

                }
                else {
                    $scope.member.promoCode = $stateParams.offer;
                }
            }

            if ($stateParams.offer == null) {
                if ($scope.member.PromoCodeValue != undefined && $scope.member.PromoCodeValue != null && $scope.member.PromoCodeValue != '') {
                    $scope.member.promoCode = $scope.member.PromoCodeValue;
                }
                else {
                    $scope.member.promoCode = 'Organic';
                }
            }

            if ($stateParams.offer == 'Organic') {
                if ($scope.member.PromoCodeValue != undefined && $scope.member.PromoCodeValue != null && $scope.member.PromoCodeValue != '') {
                    $scope.member.promoCode = $scope.member.PromoCodeValue;
                }
                else {
                    $scope.member.promoCode = 'Organic';
                }
            }

            $scope.isSignUp = true;
            $scope.btnSignUp = 'Signing Up...';
            $scope.member.MemberType = 0;
            $rootScope.autoLogin = 0;
            $rootScope.Zip = $scope.member.ZipCode;
            accountService.signUp($scope.member).then(function (response) {
                $state.go('createPin');
            }, function (err) {
                $scope.isSignUp = false;
                $scope.btnSignUp = 'Accept & Sign up';
                DisplayAlert(err, 'danger');

            });

        }
    };



    //function ValidatePromocode() {
    //    $.ajax({
    //        type: "POST",
    //        url: "AccountController/ValidatePromo",
    //        data: JSON.stringify({ issue_1: Issue1, SubIssue_1: SubIssue1, IssueDesc_1: IssueDesc1, atmId: atmid, value: value }),
    //        contentType: "application/json; charset=utf-8",
    //        async: false,
    //        success: function (data) {
    //            var output = $("<div />").html(data.d).text();
    //            var subdata = output.split('╥');
    //            $('#bindissuedivid').html(subdata[0]);
    //            $('#chkvalue').text(subdata[1]);
    //            $('#additonalid').show();
    //            $('#CcYourid').hide();
    //            $('#divshowphtoDiv').hide();
    //            $('#additonalidthird').hide();
    //        },
    //        failure: function (msg) {
    //        }
    //    });
    //}

    //$scope.validatePassword = function () {
    //    if ($scope.member != undefined && $scope.member.Password != undefined && $scope.member.Password.length == 0) {
    //        $scope.passwordValidationError = "Password is required.";
    //        $scope.isWrongPassword = true;
    //    } else if ($scope.member.Password.length < 9) {
    //        $scope.passwordValidationError = "Password contain at least 9 digits.";
    //        $scope.isWrongPassword = true;
    //    } else if ($scope.member.Password.search(/\d/) == -1) {
    //        $scope.passwordValidationError = "Password contain at least one number.";
    //        $scope.isWrongPassword = true;
    //    } else if ($scope.member.Password.search(/[a-zA-Z]/) == -1) {
    //        $scope.passwordValidationError = "Password contain at least one alphabet.";
    //        $scope.isWrongPassword = true;
    //    } else if ($scope.member.Password.search(/[!@#$%^&*]/) == -1) {
    //        $scope.passwordValidationError = "Password contain at least one special character";
    //        $scope.isWrongPassword = true;
    //    } else {
    //        $scope.isWrongPassword = false;
    //    }
    //    return $scope.passwordValidationError;
    //}

}]);
