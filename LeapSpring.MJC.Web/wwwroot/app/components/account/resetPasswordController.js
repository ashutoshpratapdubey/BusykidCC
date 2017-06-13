mjcApp.controller('resetPasswordController', ['$scope', '$state', '$filter', '$stateParams', '$timeout', 'accountService', function ($scope, $state, $filter, $stateParams, $timeout, accountService) {
    $scope.btnReset = 'Reset';
    $scope.isContinue = false;
    $scope.NewPassword = '';
    $scope.ConfirmPassword = '';
    $scope.newPasswordInputType = 'password';
    $scope.confirmPasswordInputType = 'password';

    $scope.showNewPassword = function () {
        if ($scope.NewPassword != undefined && $scope.NewPassword.length > 0) {
            if ($scope.newPasswordInputType == 'password')
                $scope.newPasswordInputType = 'text';
            else
                $scope.newPasswordInputType = 'password';
        }
    }

    $scope.showConfirmPassword = function () {
        if ($scope.ConfirmPassword != undefined && $scope.ConfirmPassword.length > 0) {
            if ($scope.confirmPasswordInputType == 'password')
                $scope.confirmPasswordInputType = 'text';
            else
                $scope.confirmPasswordInputType = 'password';
        }
    }

    $scope.resetPassword = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.resetPasswordForm.$valid) {
            $scope.isContinue = true;
            $scope.btnReset = 'Resetting...';
            accountService.resetPassword($stateParams.token, $scope.NewPassword).success(function (response) {
                $scope.btnReset = 'Reset';
                DisplayAlert('Password updated successfully.', 'success', true);
                $timeout(function () {
                    $state.go('login');
                }, 1000);
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnReset = 'Reset';
                DisplayAlert(err, 'danger');
            });
        }
    };
}]);