mjcApp.controller('signUpProgressBarController', ['$scope', '$state', '$location', 'familyMemberService', function ($scope, $state, $location, familyMemberService) {

    $scope.isProgressShow = true;
    $scope.isProgressCount = 0;

    familyMemberService.getsignUpProgress().success(function (response) {
        if (response === 'SingedUp')
            $scope.isProgressCount = 1;

        if (response === 'AddedChild')
            $scope.isProgressCount = 2;

        if (response === 'AddedChore')
            $scope.isProgressCount = 3;

        if (response === 'Completed')
            $scope.isProgressShow = false;

        // This is used to loading when fetching signup progress
        if ($scope.isSignUpProgress != 'undefined' && $scope.isSignUpProgress != null) {
            $scope.isSignUpProgress.response = false;
        }

        // This is used to navigate which page from suggested chore page
        if ($scope.signUpStatus != null)
            $scope.signUpStatus.data = angular.copy(response);
    });
}]);