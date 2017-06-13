mjcApp.controller('promoCodeSubscriptionController', ['$scope', '$uibModalInstance', '$filter', '$state', 'localStorageService', 'subscriptionService', function ($scope, $uibModalInstance, $filter, $state, localStorageService, subscriptionService) {

    // *** Variables*** //

    $scope.isValidatingPromoCode = false;
    $scope.promoCode = '';

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        if ($scope.isValidatingPromoCode)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    $scope.validatePromoCode = function () {
        if ($scope.isValidatingPromoCode)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.promoCodeForm.$valid) {
            $scope.isValidatingPromoCode = true;
            subscriptionService.ValidatePromoCode($scope.promoCode).success(function (response) {
                $scope.isValidatingPromoCode = false;
                $scope.hideModal();
                $scope.updatePromoCode(response);
            }).error(function (err, data) {
                $scope.isValidatingPromoCode = false;
                DisplayAlert(err, 'danger');
            });
        }
    }

    // *** Methods End***//

    // *** View Loading ** //

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.adminName = authData.Firstname;
        if (authData.ProfileUrl !== undefined && authData.ProfileUrl !== null && authData.ProfileUrl !== '')
            $scope.profileUrl = authData.ProfileUrl;
    }

    // *** View Loading Ends ** //
}]);