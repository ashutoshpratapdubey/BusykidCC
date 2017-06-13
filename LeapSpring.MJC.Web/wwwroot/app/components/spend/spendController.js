
mjcApp.controller('spendController', ['$scope', '$state', '$filter', '$window', '$uibModal', '$sce', 'spendService', function ($scope, $state, $filter, $window, $uibModal, $sce, spendService) {

    $scope.isPurchasing = false;
    $scope.isGettingGiftCards = false;
    $scope.isGettingPurchased = false;
    $scope.isGettingAllGiftCards = false;
    $scope.isShowBuyGiftCards = true;
    $scope.purchasedGiftCards = [];
    $scope.featuredGiftCards = [];
    $scope.giftCardPreviews = [];

    $scope.$on('getGiftCardsData', function () {
        if ($scope.featuredGiftCards.length == 0 && !$scope.isGettingGiftCards)
            $scope.GetFeaturedGiftCards();

        if ($scope.purchasedGiftCards.length == 0 && !$scope.isGettingPurchased)
            $scope.getPurchasedGiftCards();
    });

    $scope.GetFeaturedGiftCards = function () {
        $scope.isGettingGiftCards = true;
        spendService.GetGiftCardPreview(true).success(function (response) {
            $scope.featuredGiftCards = response;

            angular.forEach($scope.featuredGiftCards, function (giftCard) {
                giftCard.Disclosure = $sce.trustAsHtml(giftCard.Disclosure);
            });
            $scope.isGettingGiftCards = false;
        }).error(function (err, data) {
            $scope.isGettingGiftCards = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.GetGiftCardPreviews = function () {
        $scope.isGettingAllGiftCards = true;
        spendService.GetGiftCardPreview(false).success(function (response) {
            $scope.GiftCardPreviews = response;

            angular.forEach($scope.GiftCardPreviews, function (giftCard) {
                giftCard.Disclosure = $sce.trustAsHtml(giftCard.Disclosure);
            });
            $scope.isGettingAllGiftCards = false;
        }).error(function (err, data) {
            $scope.isGettingAllGiftCards = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.getPurchasedGiftCards = function () {
        if ($scope.purchasedGiftCards.length != 0 && $scope.isGettingPurchased) return;

        $scope.isGettingPurchased = true;
        spendService.getPurchasedGiftCards().success(function (response) {
            $scope.purchasedGiftCards = response;
            $scope.isGettingPurchased = false;
        }).error(function (err, data) {
            $scope.isGettingPurchased = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.deletepurchaseGiftCard = function (purchaseGiftCard, index) {
        purchaseGiftCard.isDeleting = true;
        spendService.deletepurchaseGiftCard(purchaseGiftCard.Id).success(function (response) {
            DisplayAlert(response, "success");
            $scope.purchasedGiftCards.splice(index, 1);
        }).error(function (err, data) {
            $scope.purchasedGiftCards[index].isDeleting = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.openGiftCardLink = function (giftCardUrl) {
        $window.open(giftCardUrl, '_blank');
    }

    $scope.toggleShowGiftCard = function () {
        $scope.isShowBuyGiftCards = !$scope.isShowBuyGiftCards;
    }

    $scope.purchase = function (giftCardPreview) {
        $scope.selectedGiftPreview = giftCardPreview;

        // Remove it and get child from local
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/spend/purchaseGiftCardView.html',
            controller: 'purchaseGiftCardController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

}]);
