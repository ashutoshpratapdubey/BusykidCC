mjcApp.controller('subscriptionCancellationController', ['$scope', '$filter', '$state', '$stateParams', '$uibModalInstance', 'subscriptionService', function ($scope, $filter, $state, $stateParams, $uibModalInstance, subscriptionService) {

    // ******* Variables *********   
    $scope.isLoading = false;
    $scope.isCancelling = false;
    //$scope.listMessage = ['Funds in kids accounts will be transferred back.',
    //                'Bank account will be unlinked.',
    //                'Subscription will be cancelled (no refund).'];

    $scope.listMessage = ['Funds in kids accounts will be transferred back.',
                    'Subscription will be cancelled (no refund).'];

    // ******* Variables End *********   

    // ******* Methods *********   

    $scope.hideModal = function () {
        if ($scope.isCancelling)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    $scope.cancelSubscription = function () {
        if ($scope.isCancelling)
            return;
        $scope.isCancelling = true;
        subscriptionService.CancelSubscription().success(function (response) {
            //DisplayAlert('Instructions has been sent to your email to complete the cancellation!', 'success');
            DisplayAlert('Your subscription has been cancelled successfully!', 'success');
            $scope.isCancelling = false;
            $scope.updateSubscription();
            $uibModalInstance.dismiss('cancel');
        }).error(function (err, data) {
            $scope.isCancelling = false;
            DisplayAlert(err, 'danger');
        });
    };

    // ******* Methods End *********  

    // ******* View Loading *********   

    // ******* View Loading End *********   
}]);
