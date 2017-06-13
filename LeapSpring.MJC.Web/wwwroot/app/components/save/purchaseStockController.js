mjcApp.controller('purchaseStockController', ['$scope', '$uibModalInstance', '$filter', '$state', 'saveService', function ($scope, $uibModalInstance, $filter, $state, saveService) {

    // *** Variables*** //
    $scope.isPurchasing = false;
    $scope.minAmount = 10;
    $scope.maxAmount = 1000;

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        if ($scope.isPurchasing)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    $scope.purchaseStock = function () {
        if (!$scope.stock || $scope.isPurchasing)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.purchaseStockForm.$valid) {
            var stockPurchaseRequest = {
                StockItemID: $scope.stock.Id,
                Amount: $scope.stock.Amount,
                StockPrice: $scope.stock.StockPrice
            };

            $scope.isPurchasing = true;

            saveService.initiateStockPurchase(stockPurchaseRequest).success(function (response) {
                $scope.isPurchasing = false;
                $scope.hideModal();
                DisplayAlert('Your stock purchase request has been sent to your parent for approval!', 'success');
                if ($state.current.name === 'childDashboard') {
                    $scope.addPurchasedStockGiftCard(response);
                    $scope.$emit("updateEarnings");
                }

            }).error(function (err) {
                $scope.isPurchasing = false;
                DisplayAlert(err, 'danger');
            });
        }
    };

    // *** Methods End***//

    // *** View Loading ** //
    if ($scope.selectedStock !== undefined)
        $scope.selectedStock.Amount = $scope.minAmount;

    $scope.stock = $scope.selectedStock;

    // *** View Loading Ends ** //
}]);




