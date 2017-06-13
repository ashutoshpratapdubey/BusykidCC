mjcApp.controller('profilePictureController', ['$scope', '$state', '$uibModal', '$timeout', 'Upload', 'localStorageService', 'profilePictureService', function ($scope, $state, $uibModal, $timeout, Upload, localStorageService, profilePictureService) {

    $scope.hasImage = false;
    $scope.isUploading = false;
    $scope.imageSource = '../../../images/Avatar.png';

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.userName = authData.Firstname;
        $scope.imageSource = authData.ProfileUrl;
        $scope.hasImage = (authData.ProfileUrl !== undefined && authData.ProfileUrl !== null && authData.ProfileUrl !== '');
        $scope.memberType = authData.MemberType;
    }
    
    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.isUploading = true;
        Upload.base64DataUrl(file).then(function (url) {
            var profileImage = {
                Base64ImageUrl: url,
                FileName: file.name,
                ContentType: file.type,
                FamilyMemberId: authData.FamilyMemberId
            };

            profilePictureService.uploadPhoto(profileImage).success(function (response) {
                authData.ProfileUrl = response;
                localStorageService.remove('authorizationData');
                localStorageService.set('authorizationData', authData);
                Upload.base64DataUrl(file).then(function (url) {
                    $scope.imageSource = url;
                    $scope.hasImage = true;
                    $scope.isUploading = false;
                });
            }).error(function (err) {
                DisplayAlert(err, 'danger');
                $scope.isUploading = false;
                $scope.hasImage = false;
            });
        });
    };

    // Shows the picker modal
    $scope.showProfilePicker = function () {
        var modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/shared/directives/profilePicker/addPhotoView.html',
            controller: 'addPhotoController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    }
}]);
