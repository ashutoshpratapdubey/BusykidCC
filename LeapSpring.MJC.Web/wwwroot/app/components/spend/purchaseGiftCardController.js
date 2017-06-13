mjcApp.controller('purchaseGiftCardController', ['$scope', '$uibModalInstance', '$filter', '$state', 'spendService', function ($scope, $uibModalInstance, $filter, $state, spendService) {

    // *** Variables*** //
    $scope.isPurchasing = false;
    $scope.selectedIndex = 0;
    $scope.maxIndex = 0;

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        if ($scope.isPurchasing)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    $scope.giftCardPreview = $scope.selectedGiftPreview;
    $scope.selectedGiftCard = $scope.selectedGiftPreview.GiftCards[$scope.selectedIndex];
    $scope.maxIndex = $scope.selectedGiftPreview.GiftCards.length - 1;

    $scope.purchaseGiftCard = function () {
        if ($scope.FinancialOverview != null && $scope.selectedGiftCard.Amount > $scope.FinancialOverview.Earnings.Spend) {
            DisplayAlert('Insufficient balance in spend bucket!', 'danger');
            return;
        }

        $scope.isPurchasing = true;
        spendService.purchaseGiftCard($scope.selectedGiftCard.CardId).success(function (response) {
            if ($scope.purchasedGiftCards != null || $scope.purchasedGiftCards != undefined)
                $scope.purchasedGiftCards.push(response);
            $scope.isPurchasing = false;
            DisplayAlert('Your gift card purchase request has been sent to your parent for approval!', 'success');
            $scope.hideModal();
            if ($state.current.name === 'childDashboard') {
                $scope.$emit("updateEarnings");
            }
        }).error(function (err, data) {
            $scope.isPurchasing = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.nextGiftCard = function () {
        $scope.selectedIndex += 1;
        $scope.selectedGiftCard = $scope.selectedGiftPreview.GiftCards[$scope.selectedIndex];
    }

    $scope.previousGiftCard = function () {
        $scope.selectedIndex -= 1;
        $scope.selectedGiftCard = $scope.selectedGiftPreview.GiftCards[$scope.selectedIndex];
    }

    // *** Methods End***//

}]);





