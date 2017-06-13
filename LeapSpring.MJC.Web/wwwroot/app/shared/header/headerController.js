mjcApp.controller('headerController', ['$scope', '$state', '$stateParams', '$q', '$window', '$timeout', 'localStorageService', 'headerService', 'accountService', 'familyMemberService', '$location', 'Idle', 'ModalService', '$rootScope',
    function ($scope, $state, $stateParams, $q, $window, $timeout, localStorageService, headerService, accountService, familyMemberService, $location, Idle, ModalService, $rootScope) {
        $scope.isLoggedIn = false;
        $scope.isLoading = false;
        $scope.isLoginPage = false;
        $scope.Members = [];
        $scope.defaultProfileImage = '../../../images/Avatar.png';
        $scope.isShowedUserProfile = false;
        $scope.IsMemberDeleting = false;
        hideAlert();

        if ($state.current.name === 'login')
            $scope.isLoginPage = true;

        $scope.AuthData = localStorageService.get('authorizationData');
        if ($scope.AuthData != null) {
            if ($state.current.name === 'welcome' || $state.current.name === 'login' || $state.current.name === 'signup' || $state.current.name === 'forgotPassword' || $state.current.name === 'resetPassword' || $state.current.name === 'parentSignUp' || $state.current.name === 'subscriptionDetail')
                $scope.isLoggedIn = false;
            else
                $scope.isLoggedIn = true;

            if ($rootScope.autoLogin == 1) {
                $scope.isLoggedIn = false;
            }
            else {
                $scope.isLoggedIn = $scope.isLoggedIn;
            }
        
            if ($scope.AuthData.ProfileUrl === undefined || $scope.AuthData.ProfileUrl === null || $scope.AuthData.ProfileUrl === '')
                $scope.AuthData.ProfileUrl = $scope.defaultProfileImage;
        } else if ($state.current.name === 'welcome') {
            $scope.isLoggedIn = false;
            $state.go('welcome');
        } else if ($state.current.name === 'login') {
            $scope.isLoggedIn = false;
            $state.go('login');
        } else if ($state.current.name === 'signup') {
            $scope.isLoggedIn = false;
            $state.go('signup', { offer: $stateParams.offer });
        } else if ($state.current.name === 'forgotPassword') {
            $scope.isLoggedIn = false;
            $state.go('forgotPassword');
        } else if ($state.current.name === 'resetPassword') {
            $scope.isLoggedIn = false;
            $state.go('resetPassword');
        } else if ($state.current.name === 'switchPin') {
            $scope.isLoggedIn = false;
            $state.go('switchPin');
        } else if ($state.current.name === 'parentSignUp') {
            $scope.isLoggedIn = false;
            $state.go('parentSignUp');
        } else if ($state.current.name === 'termsandcondition') {
            $scope.isLoggedIn = false;
            $state.go('termsandcondition');
        } else if ($state.current.name === 'privacypolicy') {
            $scope.isLoggedIn = false;
            $state.go('privacypolicy');
        } else if ($state.current.name === 'coppa') {
            $scope.isLoggedIn = false;
            $state.go('coppa');
        } else if ($state.current.name === 'subscriptionDetail') {
            $scope.isLoggedIn = false;
            $state.go('subscriptionDetail');
        } else if ($state.current.name === 'myAccount') {
            localStorageService.set('isEmailSubscription', ($stateParams.subscribe != undefined && $stateParams.subscribe != null && $stateParams.subscribe != ''));
            $scope.isLoggedIn = false;
        } else {
            if ($state.current.name != 'familyPage')
                accountService.logout();
        }

        $scope.getAllMembers = function () {
            if ($scope.AuthData == null || $scope.isLoading || $scope.isShowedUserProfile) return;

            $scope.isLoading = true;
            headerService.getAllMembers().success(function (response) {
                $scope.Admin = response.Admin;
                $scope.Parents = response.Parent;
                $scope.Childrens = response.Child;
                angular.forEach($scope.Admin, function (admin) {
                    admin.IsAuthenticate = false;
                    if ($scope.AuthData.FamilyMemberId === admin.Id) {
                        admin.IsAuthenticate = true;
                    }
                    if (admin.ProfileImageUrl === undefined || admin.ProfileImageUrl === null || admin.ProfileImageUrl === '')
                        admin.ProfileImageUrl = $scope.defaultProfileImage;

                    $scope.Members.push(admin);
                });

                angular.forEach($scope.Parents, function (parent) {
                    parent.IsAuthenticate = false;
                    if ($scope.AuthData.FamilyMemberId === parent.Id) {
                        parent.IsAuthenticate = true;
                    }
                    if (parent.ProfileImageUrl === undefined || parent.ProfileImageUrl === null || parent.ProfileImageUrl === '')
                        parent.ProfileImageUrl = $scope.defaultProfileImage;

                    $scope.Members.push(parent);
                });

                angular.forEach($scope.Childrens, function (child) {
                    child.IsAuthenticate = false;
                    if ($scope.AuthData.FamilyMemberId === child.Id) {
                        child.IsAuthenticate = true;
                    }
                    if (child.ProfileImageUrl === undefined || child.ProfileImageUrl === null || child.ProfileImageUrl === '')
                        child.ProfileImageUrl = $scope.defaultProfileImage;

                    $scope.Members.push(child);
                });

                $scope.isLoading = false;
                $scope.isShowedUserProfile = true;
            }).error(function (err, data) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
            });
        }

        $scope.toggleUserProfile = function () {
            $scope.isShowUserProfile = !$scope.isShowUserProfile;
            $scope.getAllMembers();
        }

        $(document).on('click touchstart', function (event) {
            if (!$(event.target).parents('#profilemenu').length && !$(event.target).parents('#modalPopup').length) {
                if ($scope.isShowUserProfile) {
                    $scope.isShowUserProfile = false;
                    $scope.$apply();
                }
            }
        })

        $scope.switchAccount = function (memberId, childname) {
            if ($scope.IsMemberDeleting) return;

            $scope.isShowUserProfile = !$scope.isShowUserProfile;
            $state.go('switchPin', { id: memberId, name: childname });
        }

        $scope.viewProfile = function (memberId, memberType) {
            if (memberType === 'Admin' || memberType === 'Parent')
                return;

            $scope.isShowUserProfile = !$scope.isShowUserProfile;
            $state.go('childProfile', { familyMemberId: memberId });
        }

        $scope.signOut = function () {
            localStorageService.remove('isEmailSubscription');
            accountService.logout();
        }

        $scope.goToDashboard = function (memberType) {
            if ($scope.AuthData === null)
                $window.location.href = "http://busykid.com/";
            if ($scope.AuthData != null && $scope.AuthData.MemberType === 'Admin')
                $state.go('adminDashboard');
            else if ($scope.AuthData != null && $scope.AuthData.MemberType === 'Child')
                $state.go('childDashboard');
        }


        $scope.test = function () {
            $("#home2").show();
            $("#home1").hide();
            $state.go('login');
        }
        $scope.test2 = function () {
            $("#home1").show();
            $("#home2").hide();
            $state.go('login');

        }

        $scope.$on('profileImageChanged', function (event, args) {
            $scope.AuthData.ProfileUrl = args.profileImageUrl;
        });

        $scope.$on('childnamechange', function (event, args) {
            $scope.AuthData.Firstname = args.Firstname;
        });

        $scope.$on('Adminnamechange', function (event, args) {
            $scope.AuthData.Firstname = args.Firstname;
        });

        $('html,body').animate({
            scrollTop: 0
        }, 'fast');

        $scope.$on('IdleTimeout', function () {
            var authData = localStorageService.get('authorizationData');
            if (authData != null) {
                var familyUrl = authData.FamilyUrl;
                localStorageService.remove('authorizationData');
                // $state.go('familyPage', { name: familyUrl });
                // $location.path('/familyPage').replace();
                $state.go('login');
                $location.path('/login').replace();
            }
            Idle.unwatch();
        });

        $timeout(function () {
            $("#firstTxtInput").focus();
        }, 10);

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

    }]);
