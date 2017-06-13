mjcApp.controller('forgotPasswordController', ['$scope', '$state', '$timeout', 'accountService', function ($scope, $state, $timeout, accountService) {
    $scope.btnForgotPassword = 'Request reset email';
    $scope.isForgotPassword = false;
    $scope.EmailId = '';

    $scope.forgotPassword = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.forgotPasswordForm.$valid) {
            $scope.isForgotPassword = true;
            $scope.btnForgotPassword = 'Requesting reset email...';
            accountService.passwordResetRequest($scope.EmailId).success(function (response) {
                $scope.btnForgotPassword = 'Request reset email';
                DisplayAlert('Instructions to reset your password has been sent your email.', 'success', true);
                $timeout(function () {
                    $state.go('login');
                }, 2500);
            }).error(function (err) {
                $scope.isForgotPassword = false;
                $scope.btnForgotPassword = 'Request reset email';
                DisplayAlert(err, 'danger');
            });
        }
    };
}]);