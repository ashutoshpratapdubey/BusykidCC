mjcApp.controller('cashOutController', ['$scope', '$uibModalInstance', '$filter', '$state', 'localStorageService', 'spendService', function ($scope, $uibModalInstance, $filter, $state, localStorageService, spendService) {

    // *** Variables*** //
    $scope.isLoading = false;
    $scope.isProcessing = false;
    $scope.minAmount = 1;
    $scope.maxAmount = 100;

    $scope.profileUrl = '../../../images/Avatar.png';

    $scope.cashOut = {
        Amount: 10,
        Note: '',
    };

    // *** Variables End*** //

    // *** Methods ***//

    // close the bonus modal
    $scope.hideModal = function () {
        if ($scope.isProcessing)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    // cas out to child
    $scope.cashOutRequest = function () {
        if ($scope.isProcessing)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.getCashForm.$valid) {

            var amount = parseFloat($scope.cashOut.Amount);
            if (amount <= 0) {
                DisplayAlert("Please enter valid amount", 'danger');
                return;
            }

            $scope.isProcessing = true;
            spendService.cashOut($scope.cashOut).success(function (response) {
                $scope.isProcessing = false;
                DisplayAlert('Your cash out request has been sent to your parent for approval!', 'success');
                $scope.hideModal();
                $scope.$emit("updateEarnings");
            }).error(function (err) {
                $scope.isProcessing = false;
                DisplayAlert(err, 'danger');
            });
        }
    }

    // *** Methods End***//

    // *** View Loading ** //

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.childName = authData.Firstname;
        $scope.childId = authData.FamilyMemberId;
        if (authData.ProfileUrl !== undefined && authData.ProfileUrl !== null && authData.ProfileUrl !== '')
            $scope.profileUrl = authData.ProfileUrl;
    }

    // *** View Loading Ends ** //
}]);



