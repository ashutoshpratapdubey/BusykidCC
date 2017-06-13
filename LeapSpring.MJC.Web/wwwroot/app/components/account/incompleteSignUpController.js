mjcApp.controller('incompleteSignUpController', ['$scope', 'localStorageService', function ($scope, localStorageService) {
    $scope.authData = localStorageService.get('authorizationData');
}]);