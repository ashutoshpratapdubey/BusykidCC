mjcApp.controller('parentController', ['$scope', '$state', '$filter', '$stateParams', 'Upload', 'localStorageService', 'parentService', 'invitationService', function ($scope, $state, $filter, $stateParams, Upload, localStorageService, parentService, invitationService) {
    $scope.btnContinue = 'Continue';
    $scope.isContinue = false;
    $scope.hasImage = false;
    $scope.isLoading = true;
    $scope.parentOwnPhone = 0;
    $scope.parent = {};
    $scope.imageFile;
    $scope.invitation = {};

    // parent sign up
    $scope.inputType = 'password';
    $scope.btnSignUp = 'Accept & Sign up';
    $scope.isSignUp = false;
    $scope.member = {};
    $scope.parentForm = {};

    $scope.AuthData = localStorageService.get('authorizationData');

    $scope.addParentPhone = function () {
        if (parseInt($scope.parentOwnPhone) === 1) {
            $state.go('addParent');
            return;
        }

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.parentPhoneForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            $scope.invitation.PhoneNumber = $scope.member.PhoneNumber;
            $scope.invitation.MemberType = 1;
            invitationService.invite($scope.invitation).success(function (response) {
                DisplayAlert('Invitation sent to the parent successfully.', 'success');
                $state.go('adminDashboard');
            }).error(function (err) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    }

    if ($state.current.name === 'parentSignUp') {
        $scope.showPassword = function () {
            if ($scope.member !== undefined && $scope.member.Password !== undefined && $scope.member.Password.length > 0) {
                if ($scope.inputType === 'password')
                    $scope.inputType = 'text';
                else
                    $scope.inputType = 'password';
            }
        }

        invitationService.getInvitationByToken($stateParams.invitationtoken).success(function (response) {
            $scope.invitation = response;
            $scope.isLoading = false;
        }).error(function (err) {
            $scope.isLoading = false;
            $scope.invitationAccepted = err.Message;
        });

        $scope.parentSignUp = function () {
            $scope.$broadcast('show-errors-check-validity');
            if ($scope.parentForm.parentSignupForm.$valid) {
                $scope.isSignUp = true;
                $scope.btnSignUp = 'Signing Up...';
                $scope.member.PhoneNumber = $scope.invitation.PhoneNumber;
                $scope.member.MemberType = 1;
                $scope.member.FamilyID = $scope.invitation.FamilyID;
                parentService.signUp($scope.member).success(function (response) {
                    localStorageService.remove('authorizationData');
                    localStorageService.set('authorizationData', response);
                    invitationService.UpdateInvitationStatus($scope.invitation.Id, 1).success(function (response) {
                        $state.go('addParent');
                    });
                }).error(function (err) {
                    $scope.isSignUp = false;
                    $scope.btnSignUp = 'Accept & Sign up';
                    DisplayAlert(err, 'danger');
                });
            }
        };
    }

    $scope.UploadParentImage = function () {
        if ($scope.AuthData.MemberType === 'Parent' && ($scope.imageFile === undefined || $scope.imageFile === null)) {
            $state.go('addParentPin');
            return;
        }

        $scope.isContinue = true;
        $scope.btnContinue = 'Continuing...';
        $scope.UploadProfileImage();
    }

    $scope.addParent = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.parentForm.$valid) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            $scope.member.MemberType = 1;
            parentService.addParent($scope.member).success(function (response) {
                $scope.parent = response;
                $scope.UploadProfileImage();
                $state.go('addParentPin', { id: $scope.parent.Id, name: $scope.parent.Firstname });
            }).error(function (err, data) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    // Child Image Upload
    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.imageFile = file;

        Upload.base64DataUrl($scope.imageFile).then(function (url) {
            $scope.imageSource = url;
            $scope.hasImage = true;
        });
    };

    $scope.UploadProfileImage = function () {
        if ($scope.imageFile === undefined || $scope.imageFile === null)
            return;

        var profileImage = {
            Base64ImageUrl: $scope.imageSource,
            FileName: $scope.imageFile.name,
            ContentType: $scope.imageFile.type,
            FamilyMemberId: $scope.parent.Id != undefined ? $scope.parent.Id : $scope.AuthData.FamilyMemberId
        };

        parentService.uploadPhoto(profileImage).success(function (response) {
            $scope.imageSource = response;
            $scope.hasImage = true;

            if ($scope.AuthData.MemberType === 'Parent')
                $state.go('addParentPin');
        }).error(function (err) {
            DisplayAlert(err, 'danger');
            $scope.hasImage = false;
            $scope.isContinue = false;
            $scope.btnContinue = 'Continue';
        });
    }
}]);
