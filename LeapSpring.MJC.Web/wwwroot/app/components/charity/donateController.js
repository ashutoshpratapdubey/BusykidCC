mjcApp.controller('donateController', ['$scope', '$uibModalInstance', '$filter', '$state', '$timeout', 'charityService', function ($scope, $uibModalInstance, $filter, $state, $timeout, charityService) {

    // *** Variables*** //

    $scope.isDonating = false;
    $scope.minAmount = 10;
    $scope.maxAmount = 100;
    $scope.isVisible = true;
    $scope.charities = $scope.charities;
    $scope.charity = ($scope.selectedCharity !== undefined && $scope.selectedCharity !== null) ? $scope.selectedCharity : $scope.charities[0];
    $scope.donation = {
        CharityID: ($scope.charity !== undefined && $scope.charity !== null) ? $scope.charity.Id : 0,
        Amount: $scope.minAmount
    };
    $scope.carousel;

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        if ($scope.isDonating)
            return;

        $scope.isVisible = false;
        $uibModalInstance.dismiss('cancel');
    };

    $scope.sendDonation = function () {
        if (!$scope.donation || $scope.isDonating)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.donateForm.$valid) {
            $scope.isDonating = true;
            $scope.DonateButtonContent = 'Donating...';
            charityService.donate($scope.donation).success(function (response) {
                $scope.isDonating = false;
                $scope.DonateButtonContent = 'Donate';
                $scope.hideModal();
                DisplayAlert('Your donation request has been sent to your parent for approval!', 'success');
                $scope.$emit("updateEarnings");
            }).error(function (err) {
                $scope.isDonating = false;
                $scope.DonateButtonContent = 'Donate';
                DisplayAlert(err, 'danger');
            });
        }
    };

    // *** Methods End***//

    // *** View Loading ** //

    // Loads the charities
    $timeout(function () {
        if ($scope.charities === undefined || $scope.charities === null || $scope.charities.length <= 1) {
            $scope.isCharitiesLoading = false;
            return;
        }

        $scope.carousel = angular.element(document.querySelector("#scrolling ul"));
        $scope.carousel.itemslide({ one_item: true }); //initialize itemslide

        $(window).resize(function () {
            if (!$scope.isVisible)
                return;

            $scope.carousel.reload();
        }); //Recalculate width and center positions and sizes when window is resized

        var charityIndex = $scope.charities.indexOf($filter('filter')($scope.charities, { Id: $scope.charity.Id }, true)[0]);
        $scope.carousel.gotoSlide(charityIndex);

        $scope.carousel.on('changeActiveIndex', function (e) {
            var currentIndex = $scope.carousel.getActiveIndex();
            if ($scope.charity.Id === $scope.charities[currentIndex].Id)
                return;

            $scope.$apply(function () {
                $scope.charity = $scope.charities[currentIndex];
                $scope.donation.CharityID = $scope.charity.Id;
            });
        });

        $scope.isCharitiesLoading = false;
    }, 50);

    // *** View Loading Ends ** //
}]);



