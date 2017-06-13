mjcApp.controller('createPinController', ['$scope', '$state', '$stateParams', 'localStorageService', 'createPinService', function ($scope, $state, $stateParams, localStorageService, createPinService) {

    $scope.PinOne;
    $scope.PinTwo;
    $scope.PinThree;
    $scope.PinFour;
    $scope.btnContinue = 'Continue';
    $scope.isContinue = false;

    $scope.isSignUpProgress = {};
    $scope.isSignUpProgress.response = true;
    
    if ($state.current.name === 'createPin') {
        $scope.MemberType = 'Admin';
    } else if ($state.current.name === 'addParentPin') {
        $scope.AuthData = localStorageService.get('authorizationData');
        $scope.name = ($stateParams.name != null || $stateParams.name != undefined) ? $stateParams.name : $scope.AuthData.Lastname;
        $scope.MemberType = 'Parent';
    } else if ($state.current.name === 'addChildPin') {
        $scope.name = $stateParams.name;
        $scope.MemberType = 'Child';
    }

    $scope.checkPinValidation = function () {
        if ($scope.pinForm.$valid || $scope.parentPinForm.$valid || $scope.childPinForm.$valid)
            $scope.isPinEmpty = false;
    }

    $scope.createAdminPin = function () {
        
        $scope.$broadcast('show-errors-check-validity');
        $scope.isPinEmpty = !$scope.pinForm.$valid;
        if ($scope.pinForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            var pin = $scope.PinOne.toString() + $scope.PinTwo.toString() + $scope.PinThree.toString() + $scope.PinFour.toString();
            createPinService.updateAdminPIN(pin).success(function (response) {
               
                $state.go('phoneEntry');
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    $scope.createChildPin = function () {
        $scope.$broadcast('show-errors-check-validity');
        $scope.isPinEmpty = !$scope.childPinForm.$valid;
        if ($scope.childPinForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            var pin = $scope.PinOne.toString() + $scope.PinTwo.toString() + $scope.PinThree.toString() + $scope.PinFour.toString();
            createPinService.updateMemberPIN(pin, $stateParams.id).success(function (response) {
               
                $state.go('suggestedChore', { id: $stateParams.id });
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    $scope.createParentPin = function () {       
        $scope.$broadcast('show-errors-check-validity');
        $scope.isPinEmpty = !$scope.parentPinForm.$valid;
        if ($scope.parentPinForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            var pin = $scope.PinOne.toString() + $scope.PinTwo.toString() + $scope.PinThree.toString() + $scope.PinFour.toString();
            createPinService.updateMemberPIN(pin, ($stateParams.id != null || $stateParams.id != undefined) ? $stateParams.id : $scope.AuthData.FamilyMemberId).success(function (response) {               
                $state.go('adminDashboard');
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };
}]);
