mjcApp.controller('saveController', ['$scope', '$uibModal', '$q', '$filter', '$window', 'saveService', function ($scope, $uibModal, $q, $filter, $window, saveService) {

    // ***** Variables *****
    $scope.isStockGiftCardsLoading = false;
    $scope.isShowPurchaseStockGiftCards = false;
    $scope.stockGiftCards = [];
    $scope.purchasedStockGiftCards = [];
    $scope.pendingStockGiftCards = [];
    $scope.query = '';

    // ***** Variables End *****

    // *** Methods *****

    // Loads the stock gift cards
    $scope.getStockGiftCards = function () {
        $scope.isStockGiftCardsLoading = true;
        saveService.getStockGiftCards(true).success(function (response) {
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

    // Loads the purchased stock gift cards
    $scope.getPurchasedStockGiftCards = function () {
        $scope.isStockGiftCardsLoading = true;

        saveService.getPurchasedStockGiftCards().success(function (response) {
            $scope.purchasedStockGiftCards = $filter('filter')(response, { Status: 'Completed' }, true);
            $scope.pendingStockGiftCards = $filter('filter')(response, { Status: 'PendingApproval' }, true);
            $scope.isStockGiftCardsLoading = false;
        }).error(function (err, data) {
            $scope.isStockGiftCardsLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.purchase = function (stockGiftCard) {
        $scope.selectedStock = stockGiftCard;

        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/save/purchaseStockView.html',
            controller: 'purchaseStockController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    // Inserts the purchased stock giftcard
    $scope.addPurchasedStockGiftCard = function (purchasedStockGiftCard) {
        $scope.pendingStockGiftCards.push(purchasedStockGiftCard);
    };

    // toggle the stock gift card layout
    $scope.toggleShowStockGiftCard = function () {
        $scope.isShowPurchaseStockGiftCards = !$scope.isShowPurchaseStockGiftCards;
        if ($scope.isShowPurchaseStockGiftCards)
            $scope.getPurchasedStockGiftCards();
    };

    // Navigates to the stockpile site.
    $scope.gotoStockPile = function () {
        $window.open('https://www.stockpile.com/', '_blank');
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

    $scope.$on('loadStockGiftCards', function () {
        if ($scope.stockGiftCards.length === 0 && !$scope.isStockGiftCardsLoading)
            $scope.getStockGiftCards();
    });

    if (saveService.isBuyStock && saveService.showActivity) {
        saveService.isBuyStock = false;
        saveService.showActivity = false;
        $scope.$emit('showActivityTab', null);
    };

    // ***** View Loading End *****
}]);


