
mjcApp.controller('allocationSettingsController', ['$scope', '$state', '$filter', 'localStorageService', 'settingsService', 'familyMemberService', 'choreService', function ($scope, $state, $filter, localStorageService, settingsService, familyMemberService, choreService) {

    $scope.isLoading = true;
    $scope.isSettingsLoading = true;
    $scope.allocationSettings = {};
    $scope.isSettingsSaving = false;
    $scope.showNotEqualErr = false;
    $scope.selectedMember = {};
    $scope.familyMembers = [];
    $scope.btnTxt = 'Save Settings';
    $scope.defaultProfileImage = '../../../images/Avatar.png';

    $scope.getAllocationSettings = function () {
        $scope.isSettingsLoading = true;
        settingsService.getAllocationSettings($scope.selectedMember.Id).success(function (response) {
            if (response != null)
                $scope.allocationSettings = response;
            else
                $scope.resetToDefault();

            $scope.isLoading = false;
            $scope.isSettingsLoading = false;
        }).error(function (err, data) {
            $scope.resetToDefault();
            $scope.isLoading = false;
            $scope.isSettingsLoading = false;
        });
    }

    familyMemberService.GetChildrens().success(function (response) {
        if (response == null) return;
        $scope.familyMembers = response;

        angular.forEach($scope.familyMembers, function (familyMember) {
            if (familyMember.ProfileImageUrl === undefined || familyMember.ProfileImageUrl === null || familyMember.ProfileImageUrl === '')
                familyMember.ProfileImageUrl = $scope.defaultProfileImage;

            // Prepare child age
            familyMember.Age = new Date().getFullYear() - parseInt($filter('date')(familyMember.DateOfBirth, "yyyy"));
        });

        var childIndex = $scope.familyMembers.indexOf($filter('filter')($scope.familyMembers, { Id: parseInt(choreService.childId) }, true)[0]);
        $scope.selectedMember = $scope.familyMembers[childIndex < 0 ? 0 : childIndex];
        $scope.getAllocationSettings();
    }).error(function (err, data) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.saveSettings = function () {
        $scope.$broadcast('show-errors-check-validity');
        if (!$scope.settingsForm.$valid) {
            $scope.showNotEqualErr = false;
            return
        };

        var totalAllocate = parseInt($scope.allocationSettings.Save) + parseInt($scope.allocationSettings.Share) + parseInt($scope.allocationSettings.Spend);
        if (totalAllocate != 100) {
            $scope.showNotEqualErr = true;
            return;
        }
        else
            $scope.showNotEqualErr = false;

        $scope.isSettingsSaving = true;
        $scope.btnTxt = 'Saving...';
        settingsService.updateAllocationSettings($scope.allocationSettings).success(function (response) {
            $scope.btnTxt = 'Save Settings';
            $state.go('adminDashboard');
        }).error(function (err, data) {
            $scope.btnTxt = 'Save Settings';
            $scope.isSettingsSaving = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.changeMember = function (member) {
        if ($scope.selectedMember.Id == member.Id) return;

        $scope.selectedMember = member;
        $scope.getAllocationSettings();
    }

    $scope.resetToDefault = function () {
        $scope.isSettingsLoading = true;
        settingsService.getAllocationByAge($scope.selectedMember.Age).success(function (response) {
            $scope.isSettingsLoading = false;
            $scope.allocationSettings.Save = response.Save;
            $scope.allocationSettings.Share = response.Share;
            $scope.allocationSettings.Spend = response.Spend;
        }).error(function (err, data) {
            $scope.isSettingsLoading = false;
            $scope.allocationSettings.Save = 40;
            $scope.allocationSettings.Share = 10;
            $scope.allocationSettings.Spend = 50;
        });
    }

}]);
