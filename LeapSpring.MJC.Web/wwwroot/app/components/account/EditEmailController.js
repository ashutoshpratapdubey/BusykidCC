mjcApp.controller('EditEmailController', ['$rootScope', '$scope', '$state', '$stateParams', '$timeout', '$q', '$uibModalInstance', 'localStorageService', 'familyMemberService', 'dashboardService', 'ModalService', function ($rootScope, $scope, $state, $stateParams, $timeout, $q, $uibModalInstance, localStorageService, familyMemberService, dashboardService, ModalService) {


    $scope.familyMember = {};
    $scope.familyMemberCopy = {};
    $scope.canShowAllErr = false;
    $scope.isUpdating = false;
    $scope.EmailId = '';
    $scope.Password = '';

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.memberType = authData.MemberType;
        $scope.familyId = authData.FamilyId;
    }
    $scope.UpdateEmail = function () {
        
        $scope.isUpdating = true;
        $scope.canShowAllErr = true;
        $scope.$broadcast('show-errors-check-validity');
        if (!$scope.Editemailform.$valid) return;

        familyMemberService.UpdateEmail($scope.familyMember.User.Email, $scope.Password, $scope.EmailId).success(function (response) {
            $scope.familyMemberCopy = angular.copy($scope.familyMember.User);
            $scope.isEditEmail = false;
            //DisplayAlert('Updated successfully', 'success');
            $scope.isVisible = false;
            $uibModalInstance.dismiss('cancel');
            $state.reload();
            //$state.go('login');
        }).error(function (err, data) {
            $scope.familyMember = angular.copy($scope.familyMemberCopy);
            $scope.isUpdating = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.hideModal = function () {       
        $scope.isVisible = false;
        $uibModalInstance.dismiss('cancel');
    };

    var getCurrentMemberReq = familyMemberService.GetCurrentMember();
    var getFamilyByName = familyMemberService.GetFamilyById($scope.familyId);


    $q.all([getCurrentMemberReq, getFamilyByName]).then(function (response) {
        $scope.familyMember = response[0].data;
        $scope.familyMemberCopy = angular.copy($scope.familyMember);
        $scope.familybyname = response[1].data;
        $scope.isCancelling = false;
    }, function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });


}]);