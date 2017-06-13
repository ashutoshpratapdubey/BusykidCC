
mjcApp.controller('moveMoneyController', ['$scope', '$uibModalInstance', 'earningsService', '$state', function ($scope, $uibModalInstance, earningsService, $state) {

    $scope.isBucketLoading = true;
    $scope.isMoving = false;
    $scope.btnSumbitTxt = 'Submit';
    $scope.data = { };
    $scope.bucketTypes = [];
    $scope.sourceBucket = {};
    $scope.desitinationBucket = {};

    earningsService.GetBucketTypes().success(function (response) {
        $scope.bucketTypes = response;
        $scope.sourceBucket = response[0];
        $scope.desitinationBucket = response[1];
        $scope.isBucketLoading = false;
    }).error(function (err) {
        DisplayAlert(err, 'danger');
    });

    $scope.moveMoney = function () {
        $scope.$broadcast('show-errors-check-validity');
        if (!$scope.moveMoneyForm.$valid) return;

        if ($scope.sourceBucket.Id == $scope.desitinationBucket.Id) {
            DisplayAlert("You can't move to same bucket", 'danger');
            return;
        }
        
        var amount = parseFloat($scope.data.Amount);
        if (amount <= 0) {
            DisplayAlert("Please enter valid amount", 'danger');
            return;
        }

        $scope.btnSumbitTxt = 'Submiting...';
        $scope.isMoving = true;
        earningsService.MoveMoney($scope.sourceBucket.Id, $scope.desitinationBucket.Id, amount).success(function (response) {
            $('#audioMoveMoney')[0].play(); // Play send bonus audio
            angular.copy(response.Earnings, $scope.FinancialOverview.Earnings);
            DisplayAlert(response.Message, 'success');
            $scope.isMoving = false;
            $scope.hideModal();
        }).error(function (err) {
            $scope.isMoving = false;
            $scope.btnSumbitTxt = 'Submit';
            DisplayAlert(err, 'danger');
        });
    }

    $scope.hideModal = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.changeBucket = function (type, bucketType) {
        switch (type) {
            case 'From':
                $scope.sourceBucket = bucketType;
                break;
            case 'To':
                $scope.desitinationBucket = bucketType;
                break;
        }
    }

}]);
