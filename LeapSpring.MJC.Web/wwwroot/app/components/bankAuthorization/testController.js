mjcApp.controller('testController', ['$scope', function ($scope) {
    alert('1');
   
    bankAuthorizeService.ValidateCreditCard().then(function (response) {
        alert("success");
    }, function (err) {
        alert("error");
    });

    //$scope.test = function () { alert($rootScope.urltest); }
    $scope.test();
}]);
