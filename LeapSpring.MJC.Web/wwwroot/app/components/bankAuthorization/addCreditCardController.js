mjcApp.controller('addCreditCardController', ['$scope', '$state', '$stateParams', '$filter', 'localStorageService', 'bankAuthorizeService', '$timeout', '$rootScope', '$uibModal', '$q', 'subscriptionService', 'familyMemberService', function ($scope, $state, $stateParams, $filter, localStorageService, bankAuthorizeService, $timeout, $rootScope, $uibModal, $q, subscriptionService, familyMemberService) {

    $scope.isLoading = true;
    $scope.btnContinue = 'Continue';

    $scope.goToNext = function () {
        $scope.$broadcast('show-errors-check-validity');

        $rootScope.CardType = $filter('validate')($scope.cardNumber);
        if ($scope.creditCardEntry.$valid && $rootScope.CardType != 'undefined' && $rootScope.CardType != 'null') {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            $rootScope.lastDigit = $scope.cardNumber;
            var cardnumberdata = $scope.cardNumber;
            $rootScope.CardDigit = cardnumberdata.substr(-4);
            $state.go('validateCreditCard');
        }
    }
}]);

