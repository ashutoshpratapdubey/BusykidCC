mjcApp.controller('privacyPolicyController', ['$scope', '$location', '$anchorScroll', function ($scope, $location, $anchorScroll) {
    $scope.scrollTo = function (id) {
        $location.hash(id);
        $anchorScroll();
    }
}]);