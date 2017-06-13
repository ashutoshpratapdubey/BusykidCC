
mjcApp.controller('giftCardController', ['$scope', '$uibModal', '$state', '$sce', 'saveService', 'spendService', function ($scope, $uibModal, $state, $sce, saveService, spendService) {

    // ***** Variables *****
    $scope.isGiftCardsLoading = true;
    $scope.giftCardPreviews = [];
    $scope.query = '';

    // ***** Variables End *****

    // *** Methods *****

    // Loads the gift cards
    spendService.GetGiftCardPreview().success(function (response) {
        $scope.giftCardPreviews = response;
        angular.forEach($scope.giftCardPreviews, function (giftCard) {
            giftCard.Disclosure = $sce.trustAsHtml(giftCard.Disclosure);
        });

        $scope.isGiftCardsLoading = false;
    }).error(function (err) {
        $scope.isGiftCardsLoading = false;
        DisplayAlert(err, 'danger');
    });

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

    // *** Methods End *****

    // ***** View Loading *****

    saveService.isBuyStock = true;

    // ***** View Loading End *****
}]);




