mjcApp
    .directive('digitsOnly', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var re = RegExp(attrs.digitsOnly);
                var exclude = /Backspace|Enter|Tab|Delete|Del|ArrowUp|Up|ArrowDown|Down|ArrowLeft|Left|ArrowRight|Right/;

                element[0].addEventListener('keydown', function (event) {
                    if (!exclude.test(event.key) && !re.test(event.key)) {
                        event.preventDefault();
                    }
                });
            }
        }
    })

    // Only Number Restriction Directive ends here
.controller('childAccountController', ['$rootScope', '$scope', '$state', '$stateParams', '$timeout', '$q', '$uibModal', 'localStorageService', 'Upload', 'familyMemberService', 'dashboardService', 'ModalService','childService', function ($rootScope, $scope, $state, $stateParams, $timeout, $q, $uibModal, localStorageService, Upload, familyMemberService, dashboardService, ModalService, childService) {

    $scope.isLoading = true;
    $scope.isUpdating = false;
    $scope.familyMember = {};
    $scope.isEditPhone = false;
    $scope.isEditName = false;
    $scope.isEditPin = false;
    $scope.saveBtnTxt = 'Save Settings';
    $scope.defaultProfileImage = '../../../images/Avatar.png';
    //$scope.ChangeProfileImage = '../../../images/AvatarChange.png';
    $scope.isCancelling = false;
    $scope.familyMemberCopy = {};
    $scope.canShowAllErr = false;
    $scope.id = $stateParams.id;

    // alert($scope.subscribe);


    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.memberType = authData.MemberType;
        $scope.familyId = authData.FamilyId;
    }


    if ($scope.memberType == "Admin") {

        var getCurrentMemberReq = familyMemberService.GetChildMember($scope.id);
        var getFamilyByName = familyMemberService.GetFamilyById($scope.familyId);
    }
    else {
        var getCurrentMemberReq = familyMemberService.GetCurrentMember();
        var getFamilyByName = familyMemberService.GetFamilyById($scope.familyId);
    }

    //var getCurrentMemberReq = familyMemberService.GetCurrentMember();
    //var getFamilyByName = familyMemberService.GetFamilyById($scope.familyId);


    $q.all([getCurrentMemberReq, getFamilyByName]).then(function (response) {

        $scope.familyMember = response[0].data;
        $scope.familyMemberCopy = angular.copy($scope.familyMember);
        $scope.familybyname = response[1].data;


        $scope.familyMember.isUploading = false;


        if ($scope.familyMember != null && ($scope.familyMember.ProfileImageUrl == undefined || $scope.familyMember.ProfileImageUrl == null || $scope.familyMember.ProfileImageUrl == ''))
            $scope.familyMember.ProfileImageUrl = $scope.defaultProfileImage;



        $scope.isLoading = false;
    }, function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.editFormInput = function (type) {
        $scope.canShowAllErr = false;
        var inputName = '';
        switch (type) {
            case 'phone':
                if ($scope.isEditPhone && !$scope.validateForm() &&
                    ($scope.familyMember.PhoneNumber == null || $scope.familyMember.PhoneNumber == '' || $scope.familyMember.PhoneNumber == undefined)) {
                    inputName = 'memberPhone';
                    break;
                }

                $scope.isEditPhone = !$scope.isEditPhone;
                if ($scope.isEditPhone)
                    inputName = 'memberPhone';
                break;
            case 'name':
                if ($scope.isEditName && !$scope.isEditName() &&
                    ($scope.familyMember.Firstname == null || $scope.familyMember.Firstname == '' || $scope.familyMember.Firstname == undefined)) {
                    inputName = 'memberName';
                    break;
                }
                $scope.isEditName = !$scope.isEditName;
                if ($scope.isEditName) 
                    inputName = 'memberName';
              
                break;
            case 'pin':
                if ($scope.isEditPin && !$scope.isEditPin() &&
                    ($scope.familyMember.User.PIN == null || $scope.familyMember.User.PIN == '' || $scope.familyMember.User.PIN == undefined)) {
                    inputName = 'memberPin';
                    break;
                }
                $scope.isEditPin = !$scope.isEditLasisEditPintName;
                if ($scope.isEditPin) {
                    $timeout(function () {
                        $scope.familyMember.User.PIN = '';
                    }, 10);
                    inputName = 'memberPin';
                }
                break;
        }

        if (inputName != '')
            $timeout(function () {
                $('#' + inputName)[0].focus();
            }, 50);
    }

    $scope.UpdatechildName = function () {       
        if ($scope.familyMember.Firstname == undefined || $scope.familyMember.Firstname == null || $scope.familyMember.Firstname == "") {
            var err = "Firstname is required";
            DisplayAlert(err, 'danger');
            return;
        }
        if ($scope.id != "" && $scope.id != null) {
            $scope.familyMember.Id = $scope.id;
        }
        // $scope.saveBtnTxt = 'Saving...';
        familyMemberService.UpdateChildName($scope.familyMember.Firstname, $scope.familyMember.Id).success(function (response) {
            $scope.familyMemberCopy = angular.copy($scope.familyMember);
            if (authData.MemberType == "Child") {
                authData.Firstname = $scope.familyMember.Firstname;
                localStorageService.remove('authorizationData');
                localStorageService.set('authorizationData', authData);
                $rootScope.$broadcast('childnamechange', { Firstname: $scope.familyMember.Firstname });
            }
            $scope.isEditName = false;
            DisplayAlert('Updated successfully', 'success');
            $state.reload();
        }).error(function (err, data) {
            $scope.familyMember = angular.copy($scope.familyMemberCopy);
            //$scope.saveBtnTxt = 'Save Settings';
            $scope.isUpdating = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.UpdateChildPin = function () {     
        if ($scope.familyMember.User.PIN == undefined || $scope.familyMember.User.PIN == null || $scope.familyMember.User.PIN == "") {
            var err = "Pin is required";
            DisplayAlert(err, 'danger');
            return;
        }
        // $scope.saveBtnTxt = 'Saving...';
        familyMemberService.UpdateChildPin($scope.familyMember.User.PIN, $scope.familyMember.Id).success(function (response) {
            $scope.familyMemberCopy = angular.copy($scope.familyMember);
            $scope.isEditPin = false;
            DisplayAlert('Updated successfully', 'success');
        }).error(function (err, data) {
            $scope.familyMember = angular.copy($scope.familyMemberCopy);
            //$scope.saveBtnTxt = 'Save Settings';
            $scope.isUpdating = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.UpdateChildPhone = function () {       
        if ($scope.familyMember.PhoneNumber == undefined || $scope.familyMember.PhoneNumber == null || $scope.familyMember.PhoneNumber == "") {
            var err = "Phone number is required";
            DisplayAlert(err, 'danger');
            return;
        }
        // $scope.saveBtnTxt = 'Saving...';
        if ($scope.id == "" || $scope.id == null) {
            familyMemberService.UpdateChildPhone($scope.familyMember.PhoneNumber, $scope.familyMember.Id, 0).success(function (response) {
                $scope.familyMemberCopy = angular.copy($scope.familyMember);
                $scope.isEditPhone = false;
                DisplayAlert('Updated successfully', 'success');
            }).error(function (err, data) {
                $scope.familyMember = angular.copy($scope.familyMemberCopy);
                //$scope.saveBtnTxt = 'Save Settings';
                $scope.isUpdating = false;
                DisplayAlert(err, 'danger');
            });
        }
        else {
            familyMemberService.UpdateChildPhone($scope.familyMember.PhoneNumber, $scope.familyMember.Id, 1).success(function (response) {
                $scope.familyMemberCopy = angular.copy($scope.familyMember);
                $scope.isEditPhone = false;
                DisplayAlert('Updated successfully', 'success');
            }).error(function (err, data) {
                $scope.familyMember = angular.copy($scope.familyMemberCopy);
                //$scope.saveBtnTxt = 'Save Settings';
                $scope.isUpdating = false;
                DisplayAlert(err, 'danger');
            });
        }
        
    }

    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.familyMember.isUploading = true;
        Upload.base64DataUrl(file).then(function (url) {

            var profileImage = {
                Base64ImageUrl: url,
                FileName: file.name,
                ContentType: file.type,
                FamilyMemberId: $scope.familyMember.Id
            };

            dashboardService.uploadPhoto(profileImage).success(function (response) {               
                if ($scope.id == "" || $scope.id == null) {
                    $scope.familyMember.ProfileImageUrl = response;
                    authData.ProfileUrl = response;
                    localStorageService.remove('authorizationData');
                    localStorageService.set('authorizationData', authData);
                    $rootScope.$broadcast('profileImageChanged', { profileImageUrl: response });
                }
                else {
                    $scope.familyMember.ProfileImageUrl = response;
                    authData.ProfileUrl = response;
                }

                $scope.familyMember.isUploading = false;
            }).error(function (err) {
                $scope.familyMember.isUploading = false;
            });
        });
    };

    $scope.updateChildMember = function () {      
       // $scope.canShowAllErr = true;
       // $scope.$broadcast('show-errors-check-validity');
        if ($scope.familyMember.Firstname == undefined || $scope.familyMember.Firstname == null || $scope.familyMember.Firstname == ""||$scope.familyMember.User.PIN == undefined || $scope.familyMember.User.PIN == null || $scope.familyMember.User.PIN == "") return;

        $scope.isUpdating = true;
        $scope.isEditPhone = false;
        $scope.isEditPin = false;
        $scope.isEditName = false;
        $scope.saveBtnTxt = 'Saving...';
        familyMemberService.UpdateChild($scope.familyMember).success(function (response) {
            $scope.familyMemberCopy = angular.copy($scope.familyMember);
            $scope.isUpdating = false;
            $scope.saveBtnTxt = 'Save Settings';
            DisplayAlert('Updated successfully', 'success');
        }).error(function (err, data) {
            $scope.familyMember = angular.copy($scope.familyMemberCopy);
            $scope.saveBtnTxt = 'Save Settings';
            $scope.isUpdating = false;
            DisplayAlert(err, 'danger');
        });
    }
}]);