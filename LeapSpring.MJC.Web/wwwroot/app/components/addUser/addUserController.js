mjcApp.controller('addUserController', ['$scope', '$state', '$filter', '$stateParams', 'Upload', 'localStorageService', function ($scope, $state, $filter, $stateParams, Upload, localStorageService) {
    $scope.btnContinue = 'Continue';
    $scope.selectedAccountType = 0;
    $scope.isShowHint = false;

    $scope.toggleAccountType = function () {
        $scope.isShowHint = !$scope.isShowHint;
    }

    $(document).click(function (event) {
        if (!$(event.target).parents('#hintaccounttype').length) {
            if ($scope.isShowHint) {
                $scope.isShowHint = false;
                $scope.$apply();
            }
        }
    })

    $scope.selectAccountType = function () {
        // Click continue when showing hint
        if ($scope.isShowHint) {
            $scope.isShowHint = false;
            return;
        }

        if (parseInt($scope.selectedAccountType) === 0)
            $state.go('addChild');
        else if (parseInt($scope.selectedAccountType) === 1)
            $state.go('addParentPhone');
    }
}]);