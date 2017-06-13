mjcApp.controller('TestViewController', ['$scope', '$state', 'accountService', function ($scope, $state, accountService) {
   
    $scope.SubmitValue = function (OneValue, TwoValue) {
       
        accountService.getMobileResponse(OneValue, TwoValue).success(function (response) {

        }).error(function (err) {
            DisplayAlert(err, 'danger');
        });
    };

    $scope.RunProcess = function () {
       
        accountService.getCoreProResponse().success(function (response) {

        }).error(function (err) {
            DisplayAlert(err, 'danger');
        });
    };
}]);
