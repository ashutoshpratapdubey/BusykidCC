mjcApp.controller('switchPinController', ['$scope', '$state', '$stateParams', 'accountService', 'localStorageService', 'choreService', 'Idle', 'familyMemberService', 'subscriptionService','$rootScope',
    function ($scope, $state, $stateParams, accountService, localStorageService, choreService, Idle, familyMemberService, subscriptionService, $rootScope) {

    $scope.PinOne;
    $scope.PinTwo;
    $scope.PinThree;
    $scope.PinFour;
    $scope.btnContinue = 'Continue';
    $scope.isContinue = false;
    $scope.ChildName = $stateParams.name;

    $scope.btnForgorPinContent = 'Forgot PIN?';
    $scope.isRetrievingPin = false;

    $scope.checkPinValidation = function () {
        if ($scope.switchPinForm.$valid)
            $scope.isPinEmpty = false;
    }

    $scope.login = function () {
        $scope.$broadcast('show-errors-check-validity');
        $scope.isPinEmpty = !$scope.switchPinForm.$valid;
        if ($scope.switchPinForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            var pin = $scope.PinOne.toString() + $scope.PinTwo.toString() + $scope.PinThree.toString() + $scope.PinFour.toString();
            accountService.loginWithPin($stateParams.id, pin).success(function (response) {
                // start watching when the app runs. also starts the Keepalive service by default.
                Idle.watch();
                $rootScope.autoLogin = 0;
                localStorageService.remove('authorizationData');
                localStorageService.set('authorizationData', response);
                choreService.childId = 0;

                var memberType = response.MemberType;
                familyMemberService.getsignUpProgress().success(function (response) {
                    if (response === 'Completed') {
                        // Checks for subscription
                    
                        subscriptionService.getFamilySubscription().success(function (response) {
                            if (response != null && response.Status == 'Active') {
                                if (memberType !== 'Child')
                                    $state.go('adminDashboard');
                                else
                                    $state.go('childDashboard');
                                return;
                            }
                            memberType === 'Admin' ? $state.go('subscription') : $state.go('signupprogress');
                        }).error(function (err, data) {
                            DisplayAlert(err, 'danger');
                        });
                    }
                    else
                        $state.go('signupprogress');
                }).error(function (err, data) {
                    $scope.isLogging = false;
                    $scope.btnLoginTxt = 'Sign In';
                    DisplayAlert(err, 'danger');
                });
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };


    $scope.retrievePin = function () {
        $scope.isRetrievingPin = true;
        $scope.btnForgorPinContent = 'Sending PIN to your phone...';
        accountService.retrievePin($stateParams.id).success(function (response) {
            DisplayAlert('Your pin has been sent to your mobile!', 'success');
            $scope.btnForgorPinContent = 'Forgot PIN?';
            $scope.isRetrievingPin = false;
        }).error(function (err) {
            $scope.btnForgorPinContent = 'Forgot PIN?';
            $scope.isRetrievingPin = false;
            DisplayAlert(err, 'danger');
        });
    };
}]);