
mjcApp.controller('updateMemberInfoController', ['$scope', '$state', '$stateParams', '$filter', 'familyMemberService', function ($scope, $state, $stateParams, $filter, familyMemberService) {
    $scope.isLoading = true;
    $scope.member = {};
    $scope.btnContinue = 'Continue';
    $scope.isUpdating = false;
    $scope.isBelow18Age = false;
    $scope.dobPlaceHolder = new Date();
    $scope.dobPlaceHolder.setFullYear(1985);
    $scope.states = [];
    $scope.isShowHint = false;

    $scope.isSignUpProgress = {};
    $scope.isSignUpProgress.response = true;

    $scope.toggleSsnHint = function () {
        $scope.isShowHint = !$scope.isShowHint;
    }

    $(document).click(function (event) {
        if (!$(event.target).parents('#hintssn').length) {
            if ($scope.isShowHint) {
                $scope.isShowHint = false;
                $scope.$apply();
            }
        }
    })

    // Check is this action update
    $scope.isUpdateProgress = $stateParams.action != null && $stateParams.action == 'update';

    familyMemberService.getStates().success(function (response) {
        $scope.states = response;
        $scope.member.State = $filter('filter')($scope.states, { Name: 'Arizona' }, true)[0];
        $scope.isLoading = false;
    }).error(function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.updateMemberInfo = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.memberInfoForm.$valid && !$scope.isBelow18Age) {
            var member = $scope.member;
            $scope.isUpdating = false;
            $scope.btnContinue = 'Updating...';
            member.DateOfBirth = (new Date(Date.parse($filter('dateInput')(member.Dob)))).toDateString();
            familyMemberService.updateMemberInfo(member.DateOfBirth, member.Address, member.City, member.State.Id, member.SSN).success(function (response) {
                $scope.gotoNextPage();
            }).error(function (err) {
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    $scope.validateDOB = function () {
        if ($scope.member.Dob == undefined)
            return;

        var memberDOB = new Date(Date.parse($filter('dateInput')($scope.member.Dob)));
        var date = new Date();
        var yearOfUnder18 = date.getFullYear() - 18;
        date.setFullYear(yearOfUnder18);
        if (date >= memberDOB)
            $scope.isBelow18Age = false;
        else
            $scope.isBelow18Age = true;
    }

    $scope.gotoNextPage = function () {
        if ($scope.isUpdateProgress)
            $state.go('linkbank', { action: 'update' });
        else
            $state.go('linkbank');
    }

    $scope.changeState = function (state) {
        $scope.member.State = state;
    }

}]);
