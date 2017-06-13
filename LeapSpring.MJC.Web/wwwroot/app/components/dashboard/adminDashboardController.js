mjcApp.controller('adminDashboardController', ['$scope', '$state', '$filter', '$uibModal', 'Upload', 'localStorageService', '$timeout', '$q', 'dashboardService', 'familyMemberService', 'earningsService', 'choreService', 'subscriptionService', 'NonplaidbankAuthorizeService', '$rootScope', 'accountService', 'GoogleAdWordsService', function ($scope, $state, $filter, $uibModal, Upload, localStorageService, $timeout, $q, dashboardService, familyMemberService, earningsService, choreService, subscriptionService, NonplaidbankAuthorizeService, $rootScope, accountService, GoogleAdWordsService) {
    // ***** Variables *****
   
    $scope.isLoading = true;
    $scope.childrens = [];
    $scope.children = {};
    $scope.selectedWeekDay = {};
    $scope.weekDays = [];
    $scope.isChoresLoading = false;
    $scope.chores = [];
    $scope.isProfileSelected = false;
    $scope.childEarnings = {};
    $scope.defaultProfileImage = '../../../images/Avatar.png';
    $scope.selectedDate = new Date();
    $scope.selectedChore = {};
    $scope.isEarningsLoading = false;
    $scope.isFinancialOverviewLoading = false;
    $scope.showActivity = false;
    $scope.isSelectionChanged = true;
    $scope.isSelectedChild = false;
    $scope.isDesktop = false;

    $scope.carousel;
    $scope.currentChildIndex = 0;
    $scope.Child = {};

    // ***** Variables Ends *****

    // ***** Methods *****

    // Get week days
    $scope.getWeekDays = function () {
        var weekDays = choreService.getWeekDays();
        var today = $filter('date')(new Date(), 'EEEE');
        angular.forEach(weekDays, function (day) {
            day.IsDaySelected = (today == day.Name);
            day.IsToday = (today == day.Name);
        });

        $scope.weekDays = weekDays;
    };

    // Changes week day
    $scope.toggleWeekDay = function (weekDay) {
        var index = 0;
        angular.forEach($scope.weekDays, function (day) {
            day.IsDaySelected = false;
            index++;
            if (day.Id === weekDay.Id) {
                day.IsDaySelected = true;
                $scope.selectedWeekDay = day;

                $scope.getDate($scope.selectedWeekDay);
                $scope.getChoresByDate($scope.selectedDate, $scope.selectedChild.Id);
            }
        });
    };

    // Get date of selected day
    $scope.getDate = function (weekDay) {
        var index = 0;
        angular.forEach($scope.weekDays, function (day) {
            index++;
            if (day.Id === weekDay.Id) {
                // Get date of selected day
                var todaysDay = $filter('date')(new Date(), 'EEEE');
                var todayIndex = $scope.weekDays.indexOf($filter('filter')($scope.weekDays, { Name: todaysDay }, true)[0]);
                var diff = ((index - 1)) - todayIndex;
                $scope.selectedDate = new Date(new Date().setDate(new Date().getDate() + diff));
            }
        });
    };

    $scope.checkSigupProgress = function () {
        familyMemberService.getsignUpProgress().success(function (response) {
            if (response != 'Completed') {
                $state.go('signupprogress');
                $scope.isLoading = false;
                return;
            }
            $scope.checkSubscription();
        });
    };

    // Checks for subscription
    $scope.checkSubscription = function () {

        subscriptionService.getFamilySubscription().success(function (response) {
            if (response != null && response.Status == 'Active') {
                $scope.loadChildData($scope.selectedChild.Id, new Date());
                return;
            }
            $scope.memberType === 'Admin' ? $state.go('subscription') : $state.go('incompleteSignUp');
        }).error(function (err, data) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    }

    // Get chores by day
    $scope.getChoresByDate = function (date, familyMemberId) {
        if (!$scope.isLoading)
            $scope.isChoresLoading = true;

        choreService.getChoresByDate($filter('date')(date, 'MM-dd-yyyy hh:mm:ss'), familyMemberId).success(function (response) {
            angular.forEach(response, function (chore) {
                if (chore.FamilyMember.ProfileImageUrl === undefined || chore.FamilyMember.ProfileImageUrl === null || chore.FamilyMember.ProfileImageUrl === '')
                    chore.FamilyMember.ProfileImageUrl = $scope.defaultProfileImage;
                chore.Frequency = chore.FrequencyType;
            });
            $scope.chores = response;
            $scope.isChoresLoading = false;
            $scope.isLoading = false;
        }).error(function (err) {
            $scope.isChoresLoading = false;
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    // Edit the selected chore
    $scope.editChore = function (chore) {
        $scope.selectedChore = chore;
        $scope.selectedChore.NameofChore = chore.Name;
        $scope.selectedChore.Value = $filter('number')(chore.Value, 2);

        // Remove it and get child from local
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/chore/editChore.html',
            controller: 'editChoreController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    $scope.updateChildId = function () {
        choreService.childId = $scope.selectedChild.Id;
        dashboardService.selectedChildId = $scope.selectedChild.Id;
    };

    //$scope.sendBonus = function () {
    //    $scope.currentChild = $scope.selectedChild;

    //    // Remove it and get child from local
    //    $scope.modalInstance = $uibModal.open({
    //        scope: $scope,
    //        templateUrl: '/app/components/bonus/sendBonus.html',
    //        controller: 'bonusController',
    //        size: 'sm',
    //        windowClass: 'box-popup'
    //    });
    //};
    $scope.sendBonus = function () {
        NonplaidbankAuthorizeService.GetLinkedBankStatus().success(function (response) {
            if (response.BankStatus == 'Verified') {
                //$scope.buttondisbaled = true;
                $scope.currentChild = $scope.selectedChild;

                // Remove it and get child from local
                $scope.modalInstance = $uibModal.open({
                    scope: $scope,
                    templateUrl: '/app/components/bonus/sendBonus.html',
                    controller: 'bonusController',
                    size: 'sm',
                    windowClass: 'box-popup'
                });
            }
            else {
                DisplayAlert("Please verify your account", 'danger');
                return false;
            }

        }).error(function (err, data) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    // Go to previous slide
    $scope.gotoPrevious = function () {
        $scope.carousel.previous();
    };

    // Go to next slide
    $scope.gotoNext = function () {
        $scope.carousel.next();
    };

    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.imageFile = file;

        Upload.base64DataUrl($scope.imageFile).then(function (url) {
            $scope.imageSource = url;
            $scope.uploadChildImage($scope.Child);
        });
    };

    $scope.uploadChildImage = function (child) {
        if ($scope.imageFile === undefined || $scope.imageFile === null)
            return;

        child.isUploading = true;
        var profileImage = {
            Base64ImageUrl: $scope.imageSource,
            FileName: $scope.imageFile.name,
            ContentType: $scope.imageFile.type,
            FamilyMemberId: child.Id
        };

        dashboardService.cc(profileImage).success(function (response) {
            var foundedChild = $filter('filter')($scope.childrens, { Id: child.Id }, true)[0];
            foundedChild.ProfileImageUrl = response;
            foundedChild.isUploading = false;
        }).error(function (err) {
            var foundedChild = $filter('filter')($scope.childrens, { Id: child.Id }, true)[0];
            foundedChild.isUploading = false;
        });
    }

    $scope.getAllTransactions = function () {
        if ($scope.selectedChild !== undefined) {
            $scope.showActivity = false;
            $scope.$broadcast('loadAllTransactions', { childId: $scope.selectedChild.Id });
        }
    }

    $scope.changeChildActivity = function () {
        $scope.$broadcast('onChildChanged');
    }

    // Loads the page data for selected child.
    $scope.loadChildData = function (familyMemberId, date) {
       
        var getChildEarnings = earningsService.GetByMemberId(familyMemberId);
        var getChoresByDate = choreService.getChoresByDate($filter('date')(date, 'MM-dd-yyyy hh:mm:ss'), familyMemberId);

        var weekDayName = $filter('date')(new Date(), 'EEEE');
        var getChildFinacialOverview = dashboardService.getChildFinacialOverview(weekDayName, familyMemberId);

        if (!$scope.isLoading) {
            $scope.isChoresLoading = true;
            $scope.isEarningsLoading = true;
            $scope.isFinancialOverviewLoading = true;
        }

        // This queue will execute one by one
        $q.all([getChildEarnings, getChoresByDate, getChildFinacialOverview]).then(function (response) {

            $scope.childEarnings = response[0].data;

            angular.forEach(response[1].data, function (chore) {
                if (chore.FamilyMember.ProfileImageUrl === undefined || chore.FamilyMember.ProfileImageUrl === null || chore.FamilyMember.ProfileImageUrl === '')
                    chore.FamilyMember.ProfileImageUrl = $scope.defaultProfileImage;
                chore.Frequency = chore.FrequencyType;
            });
            $scope.chores = response[1].data;

            $scope.FinancialOverview = response[2].data;
            $scope.daysRemain = $scope.FinancialOverview.RemainingDays;

            $scope.isFinancialOverviewLoading = false;
            $scope.isChoresLoading = false;
            $scope.isEarningsLoading = false;
            $scope.isLoading = false;
        }, function (err) {
            $scope.isFinancialOverviewLoading = false;
            $scope.isChoresLoading = false;
            $scope.isEarningsLoading = false;
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    }

    $scope.selectFile = function (child) {
        $scope.Child = child;
        $("#imageUploader").trigger("click");
    };
    // ***** Methods Ends *****

    // ***** View Loading *****

    $scope.getWeekDays();

    familyMemberService.GetChildrens().success(function (response) {
        $scope.isDesktop = (window.innerWidth >= 768);

        $scope.childrens = response;
        if ($scope.childrens === undefined || $scope.childrens.length === 0) {
            $state.go('signupprogress');
            return;
        }

        // Prepares the child profiles
        angular.forEach($scope.childrens, function (child) {
            if (child.ProfileImageUrl === undefined || child.ProfileImageUrl === '' || child.ProfileImageUrl === null) {
                child.ProfileImageUrl = '../../../images/Avatar.png';
            }
        });

        if ($scope.childrens.length > 1) {
            // Loads the childrens
            $timeout(function () {
                $scope.carousel = angular.element(document.querySelector("#scrolling ul"));
                //angular.element(document.querySelector("#scrolling ul")).itemslide({ one_item: true }); //initialize itemslide

                $scope.carousel.itemslide({ one_item: true }); //initialize itemslide

                angular.element(window).resize(function () {
                    if ($state.current.name !== 'adminDashboard')
                        return;

                    angular.element(document.querySelector("#scrolling ul")).reload();
                }); //Recalculate width and center positions and sizes when window is resized

                $scope.carousel.on('changeActiveIndex', function (e) {
                    var currentIndex = $scope.carousel.getActiveIndex();
                    if ($scope.selectedChild.Id === $scope.childrens[currentIndex].Id)
                        return;

                    $scope.isSelectionChanged = false;

                    $scope.currentChildIndex = currentIndex;
                    $scope.selectedChild = $scope.childrens[currentIndex];

                    dashboardService.selectedChildId = $scope.selectedChild.Id;
                    $scope.loadChildData($scope.selectedChild.Id, $scope.selectedDate);
                });
            }, 100);
        } else {
            $scope.Child = $scope.children = $scope.childrens[0];
        }

        $timeout(function () {
            $scope.selectedChild = ($scope.childrens.length > 1) ? $scope.childrens[angular.element(document.querySelector("#scrolling ul")).getActiveIndex()] : $scope.children;
            $scope.pageLoad();
        }, 100);

    }).error(function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.memberType = authData.MemberType;
    }


    // Loads the page data.
    $scope.pageLoad = function () {
       
        if (choreService.childId != 0 && $scope.childrens.length > 1) {
            $scope.isSelectedChild = true;
            var childIndex = $scope.childrens.indexOf($filter('filter')($scope.childrens, { Id: parseInt(choreService.childId) }, true)[0]);
            angular.element(document.querySelector("#scrolling ul")).gotoSlide(childIndex);
        }

        dashboardService.selectedChildId = $scope.selectedChild.Id;

        // Change Here
        $scope.checkSigupProgress();

        if ($rootScope.promocode != null) {
            subscriptionService.ValidatePromoCode($rootScope.promocode).success(function (response) {
                var Pixelscript = angular.element(document.querySelector('#divpixel'));
                if (response != null && response.PixelScript != null) {
                    Pixelscript.append(response.PixelScript);
                }
                else {
                    Pixelscript.append("");
                }

            }).error(function (err, data) {

            });
        }

    };
    //Gyan sir updated code
    $scope.RunAdwardScript = function () {
        GoogleAdWordsService.checkgoogleAdscript();
    }
    $scope.RunAdwardScript();

    $scope.gotoChildDashboard = function () {
        $scope.loadChildData($scope.selectedChild.Id, $scope.selectedDate, $scope.selectedDate);
    }



}]);