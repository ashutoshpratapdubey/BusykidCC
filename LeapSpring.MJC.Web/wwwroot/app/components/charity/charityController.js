mjcApp.controller('charityController', ['$scope', '$uibModal', 'Upload', 'charityService', function ($scope, $uibModal, Upload, charityService) {

    // ***** Variables *****

    $scope.isCharitiesLoading = false;
    $scope.charities = [];
    $scope.selectedCharity = {};
    // ***** Variables End *****

    // *** Methods *****

    // Loads the charities
    $scope.getCharities = function () {
        $scope.isCharitiesLoading = true;
        charityService.getCharities().success(function (response) {
            $scope.charities = response;
            $scope.isCharitiesLoading = false;
        }).error(function (err) {
            $scope.isCharitiesLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.donate = function (charity) {
        $scope.selectedCharity = charity;

        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/charity/donateView.html',
            controller: 'donateController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    // *** Methods End *****

    // ***** View Loading *****

    $scope.$on('loadCharities', function () {
        if ($scope.charities.length == 0 && !$scope.isCharitiesLoading)
            $scope.getCharities();
    });

    // ***** View Loading End *****
}]);
