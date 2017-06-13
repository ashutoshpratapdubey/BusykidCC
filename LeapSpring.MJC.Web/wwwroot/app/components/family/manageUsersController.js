
mjcApp.controller('manageUsersController', ['$scope', '$state', 'localStorageService', '$filter', 'familyMemberService', 'ModalService', 'Upload',
    function ($scope, $state, localStorageService, $filter, familyMemberService, ModalService, Upload) {
        $scope.isLoading = true;
        $scope.Members = [];
        $scope.Admin = {};
        $scope.Parents = [];
        $scope.Childrens = [];
        $scope.defaultProfileImage = '../../../images/Avatar.png';
        $scope.AuthData = localStorageService.get('authorizationData');

        $scope.getMembers = function () {
            familyMemberService.getAllMembers().success(function (response) {
                $scope.Admin = response.Admin;
                $scope.Parents = response.Parent;
                $scope.Childrens = response.Child;

                angular.forEach($scope.Admin, function (admin) {
                    if (admin.ProfileImageUrl === undefined || admin.ProfileImageUrl === null || admin.ProfileImageUrl === '')
                        admin.ProfileImageUrl = $scope.defaultProfileImage;
                    admin.isUploading = false;
                    $scope.Members.push(admin);
                });

                angular.forEach($scope.Parents, function (parent) {
                    if (parent.ProfileImageUrl === undefined || parent.ProfileImageUrl === null || parent.ProfileImageUrl === '')
                        parent.ProfileImageUrl = $scope.defaultProfileImage;
                    parent.isUploading = false;
                    $scope.Members.push(parent);
                });

                angular.forEach($scope.Childrens, function (child) {
                    if (child.ProfileImageUrl === undefined || child.ProfileImageUrl === null || child.ProfileImageUrl === '')
                        child.ProfileImageUrl = $scope.defaultProfileImage;
                    child.isUploading = false;
                    $scope.Members.push(child);
                });

                $scope.isLoading = false;
                $scope.isShowedUserProfile = true;
            }).error(function (err, data) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
            });
        }

        if ($scope.AuthData.MemberType === 'Child')
            $state.go('childDashboard');
        else
            $scope.getMembers();

        $scope.deleteMember = function (member) {
            var conformationMsg = "Are you sure to delete? Funds in " + member.Firstname + "'s account will be transferred back.";
            ModalService.showModal({
                templateUrl: '/app/shared/common/confirmdialog.html',
                controller: 'confirmDialogController',
                inputs: {
                    type: 'alert',
                    yesText: 'Delete',
                    noText: 'Cancel',
                    message: conformationMsg,
                    listMessage: null
                }
            }).then(function (modal) {
                modal.element.modal();
                modal.close.then(function (result) {
                    if (result) {
                        member.IsDeleting = true;
                        var memberIndex = $scope.Members.indexOf(member);
                        familyMemberService.Delete(member.Id).success(function (response) {
                            $scope.Members.splice(memberIndex, 1);
                            DisplayAlert(response, 'success');
                        }).error(function (err, data) {
                            var foundedMember = $filter('filter')($scope.Members, { Id: member.Id }, true)[0];
                            foundedMember.IsDeleting = false;
                            DisplayAlert(err, 'danger');
                        });
                    }
                });
            });
        }

        $scope.switchAccount = function (member) {
            if (member.IsDeleting) return;

            $state.go('switchPin', { id: member.Id, name: member.Firstname });
        }

        $scope.completeProfile = function (member) {
            if (member.MemberType === 'Parent') {
                $state.go('addParentPin', { id: member.Id, name: member.Firstname });
            }
            else if (member.MemberType === 'Child') {
                if (member.PhoneNumber === null)
                    $state.go('addChildPhone', { id: member.Id });
                else
                    $state.go('addChildPin', { id: member.Id, name: member.Firstname });
            }
        }

        $scope.uploadImage = function (member, file) {
            if (!file)
                return;

            member.isUploading = true;
            Upload.base64DataUrl(file).then(function (url) {

                var profileImage = {
                    Base64ImageUrl: url,
                    FileName: file.name,
                    ContentType: file.type,
                    FamilyMemberId: member.Id
                };

                familyMemberService.uploadPhoto(profileImage).success(function (response) {
                    member.ProfileImageUrl = response;
                    member.isUploading = false;
                }).error(function (err) {
                    member.isUploading = false;
                });
            });
        };

        $scope.EditChild = function (member) {           
            $state.go("childAccount", { id: member.Id});
        }
        

    }]);

