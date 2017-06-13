mjcApp.controller('stockController', ['$scope', '$uibModal', '$state', 'saveService', function ($scope, $uibModal, $state, saveService) {

    // ***** Variables *****
    $scope.isStockGiftCardsLoading = false;
    $scope.stockGiftCards = [];
    $scope.selectedStockGiftCard = {};
    $scope.query = '';

    // ***** Variables End *****

    // *** Methods *****

    // Loads the stock gift cards
    $scope.getStockGiftCards = function () {
        $scope.isStockGiftCardsLoading = true;
        saveService.getStockGiftCards().success(function (response) {
            angular.forEach(response, function (giftCard) {
                giftCard.IsStock = true;
            });

            $scope.stockGiftCards = response;
            $scope.isStockGiftCardsLoading = false;
        }).error(function (err) {
            $scope.isStockGiftCardsLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.purchase = function (stockGiftCard) {
        $scope.selectedStock = stockGiftCard;

        // Remove it and get child from local
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/save/purchaseStockView.html',
            controller: 'purchaseStockController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    $scope.toggleHomeTab = function () {
        saveService.showHome = !saveService.showHome;
        saveService.showActivity = false;
        $state.go('childDashboard');
    };

    $scope.toggleActivityTab = function () {
        saveService.showHome = false;
        saveService.showActivity = !saveService.showActivity;
        $state.go('childDashboard');
    };

    // Displays the stock pile modal
    $scope.showStockModal = function ($event, stockGiftCard) {
        $event.stopPropagation();
        if (stockGiftCard == undefined || stockGiftCard == null || stockGiftCard.ItemCode == undefined || stockGiftCard.ItemCode == null || stockGiftCard.ItemCode == '')
            return;

        $scope.selectedStock = stockGiftCard;
        Stockpile.open(stockGiftCard.ItemCode);
    };

    if (Stockpile != undefined) {
        Stockpile.onClose(function (giftItem) {
            if (giftItem)
                $scope.purchase($scope.selectedStock);
        });
    }

    // *** Methods End *****

    // ***** View Loading *****

    saveService.isBuyStock = true;

    $scope.getStockGiftCards();

    // ***** View Loading End *****
}]);



