mjcApp.controller('phoneEntryController', ['$scope', '$state', '$filter', 'localStorageService', 'phoneEntryService', function ($scope, $state, $filter, localStorageService, phoneEntryService) {
    $scope.phoneNumber = '';
    $scope.VerificationCode;
    $scope.btnContinue = 'Continue';
    $scope.isContinue = false;

    $scope.btnConfirm = 'Confirm';
    $scope.isConfirm = false;

    if ($state.current.name === 'phoneNumberConfirmation')
        $scope.phoneNumber = localStorageService.get('phoneNumber');

    // Sends the verification code to user.
    $scope.goToNext = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.phoneEntry.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            localStorageService.remove('phoneNumber');
            localStorageService.set('phoneNumber', $scope.phoneNumber);
            phoneEntryService.getVerificationCode($scope.phoneNumber).success(function () {
                $state.go('phoneNumberConfirmation');
            }).error(function (err, data) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    $scope.goBack = function () {
        localStorageService.remove('phoneNumber');
        $state.go('phoneEntry');
    };


    $scope.gotoNextPage = function () {
        $state.go('profilePicture');
    };

    // Verifies the phone number
    $scope.confirmOTP = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.phoneConfirmation.$valid) {
            $scope.btnConfirm = 'Confirming...';
            $scope.isConfirm = true;
            
             phoneEntryService.verifyCode($scope.VerificationCode).success(function (response) {
                if (response !== 'Verified')
                    return;
                   
                phoneEntryService.updatePhoneNumber($scope.phoneNumber).success(function (response) {
                    localStorageService.remove('phoneNumber');
                    $scope.gotoNextPage();
                }).error(function (err, data) {
                    DisplayAlert(err, 'danger');
                    $scope.btnConfirm = 'Confirm';
                    $scope.isConfirm = false;
                });
            }).error(function (err, data) {
                DisplayAlert(err, 'danger');
                $scope.btnConfirm = 'Confirm';
                $scope.isConfirm = false;
            });
        }
    };
}]);
