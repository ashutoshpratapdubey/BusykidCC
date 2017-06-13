mjcApp.controller('invitationController', ['$scope', '$state', '$filter', '$stateParams', 'localStorageService', 'invitationService', function ($scope, $state, $filter, $stateParams, localStorageService, invitationService) {
    $scope.btnSendInvite = 'Send Invite';
    $scope.isContinue = false;

    $scope.SendInvite = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.invitationForm.$valid) {
            $scope.isContinue = true;
            $scope.btnSendInvite = 'Inviting...';
            $scope.invitation.MemberType = 3;
            invitationService.invite($scope.invitation).success(function (response) {
                DisplayAlert('Your invite has been sent to ' + response.Firstname, 'success');
                $state.go('adminDashboard');
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnSendInvite = 'Send Invite';
                DisplayAlert(err, 'danger');
            });
        }
    };
}]);

