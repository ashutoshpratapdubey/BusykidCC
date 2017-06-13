mjcApp.controller('bonusController', ['$scope', '$uibModalInstance', '$filter', '$state', 'localStorageService', 'bonusService', function ($scope, $uibModalInstance, $filter, $state, localStorageService, bonusService) {

    // *** Variables*** //
    $scope.children = $scope.currentChild;
    $scope.isLoading = false;
    $scope.showSuccessMessage = false;
    $scope.isSending = false;
    $scope.btnSendBonus = 'Send Bonus';
    $scope.btnCancel = 'Cancel';
    $scope.parentName = '';
    $scope.profileImage = '../../../images/Avatar.png';

    $scope.bonus = {
        Amount: '',
        Note: '',
        childId: $scope.children.Id,
    };

    // *** Variables End*** //

    // *** Methods ***//

    // close the bonus modal
    $scope.hideModal = function () {
        if ($scope.isSending)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    // send bonus to child
    $scope.send = function () {
        //debugger;
        if ($scope.isSending)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.sendBonusForm.$valid) {

            var amount = parseFloat($scope.bonus.Amount);
            if (amount <= 0) {
                DisplayAlert("Please enter valid amount", 'danger');
                return;
            }

            $scope.isSending = true;
            $scope.btnSendBonus = 'Sending...';
            bonusService.sendBonus($scope.bonus).success(function (response) {
                //if ($state.current.name === 'adminDashboard') {
                //    $scope.getEarnings($scope.children.Id);
                //}
                $('#audioSendBonus')[0].play(); // Play send bonus audio
                $scope.isSending = false;
                $scope.btnSendBonus = 'Send Bonus';
                $scope.showSuccessMessage = true;
            }).error(function (err) {
                $scope.isSending = false;
                $scope.btnSendBonus = 'Send Bonus';
                DisplayAlert(err, 'danger');
            });
        }
    }

    // *** Methods End***//

    // *** View Loading ** //

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.parentName = authData.Firstname;
        if (authData.ProfileUrl !== undefined && authData.ProfileUrl !== null && authData.ProfileUrl !== '')
            $scope.profileImage = authData.ProfileUrl;
    }

    // *** View Loading Ends ** //
}]);


