mjcApp.controller('childController', ['$scope', '$state', '$filter', '$stateParams', 'Upload', 'localStorageService', 'childService', function ($scope, $state, $filter, $stateParams, Upload, localStorageService, childService) {
    $scope.btnContinue = 'Continue';
    $scope.isContinue = false;
    $scope.hasImage = false;

    $scope.dobPlaceHolder = new Date();
    $scope.child = {};
    $scope.imageFile;
    $scope.gender = 0;
    $scope.childOwnPhone = 0;
    $scope.isLoading = true;
    $scope.isFeatureDate = false;
    $scope.isShowHint = false;
    $scope.isSignUpProgress = {};
    $scope.isSignUpProgress.response = true;

    $scope.toggleDobHint = function () {
        $scope.isShowHint = !$scope.isShowHint;
    }

    $(document).click(function (event) {
        if (!$(event.target).parents('#hintdob').length) {
            if ($scope.isShowHint) {
                $scope.isShowHint = false;
                $scope.$apply();
            }
        }
    })

    if ($state.current.name !== 'addChild') {
        childService.getMemberById($stateParams.id).success(function (response) {
            $scope.child = response;
            $scope.imageSource = $scope.child.ProfileImageUrl;
            $scope.hasImage = !($scope.child.ProfileImageUrl === undefined || $scope.child.ProfileImageUrl === null || $scope.child.ProfileImageUrl === '');
            $scope.isLoading = false;
        });
    }

    $scope.addChild = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.childForm.$valid && !$scope.isFeatureDate) {
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            $scope.member.MemberType = 2;
            $scope.member.DateOfBirth = new Date(Date.parse($filter('fullYearDateInput')($scope.member.Dob)));
            $scope.member.Gender = $scope.gender;
            childService.addChild($scope.member).success(function (response) {
                $scope.child = response;
                if (!$scope.imageSource) {
                    $state.go('addChildPhone', { id: $scope.child.Id });
                    return;
                }

                $scope.UploadProfileImage();
            }).error(function (err, data) {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                DisplayAlert(err, 'danger');
            });
        }
    };

    $scope.addChildPhone = function () {
        if ($scope.child === undefined)
            return;

        if ($scope.childOwnPhone === 0) {
            $scope.$broadcast('show-errors-check-validity');
            if ($scope.childPhoneForm.$valid) {
                $scope.isContinue = true;
                $scope.btnContinue = 'Continuing...';
                childService.updatePhoneNumber($scope.member.PhoneNumber, $scope.child.Id).success(function (response) {
                    $state.go('addChildPin', { id: $scope.child.Id, name: $scope.child.Firstname });
                }).error(function (err, data) {
                    $scope.isContinue = false;
                    $scope.btnContinue = 'Continue';
                    DisplayAlert(err, 'danger');
                });
            }
        } else {
            $state.go('addChildPin', { id: $scope.child.Id, name: $scope.child.Firstname });
        }
    };


    // Child Image Upload
    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.imageFile = file;

        Upload.base64DataUrl($scope.imageFile).then(function (url) {
            $scope.imageSource = url;
            if ($state.current.name === 'addChildPhone') {
                $scope.UploadProfileImage();
                return;
            }
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
            FamilyMemberId: $scope.child.Id
        };

        childService.uploadPhoto(profileImage).success(function (response) {
            $scope.imageSource = response;
            if ($state.current.name === 'addChild')
                $state.go('addChildPhone', { id: $scope.child.Id });
            $scope.hasImage = true;

        }).error(function (err) {
            DisplayAlert(err, 'danger');
            $scope.hasImage = false;
            $scope.isContinue = false;
            $scope.btnContinue = 'Continue';
        });
    }

    // Check Validation for child age.
    $scope.validateDOB = function () {
        if ($scope.member.Dob == undefined || $scope.member.Dob == null) return;
        var memberDOB = new Date(Date.parse($filter('dateInput')($scope.member.Dob)));

        // Check given date is feature
        if (memberDOB > (new Date()))
            $scope.isFeatureDate = true;
        else
            $scope.isFeatureDate = false;
    }
}]);