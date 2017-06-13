mjcApp.controller('childDashboardController', ['$scope', '$state', '$q', '$filter', '$timeout', '$uibModal', 'moment', 'localStorageService', 'dashboardService', 'choreService', 'subscriptionService', 'familyMemberService', 'charityService', 'bankAuthorizeService', '$rootScope', function ($scope, $state, $q, $filter, $timeout, $uibModal, moment, localStorageService, dashboardService, choreService, subscriptionService, familyMemberService, charityService, bankAuthorizeService, $rootScope) {

    // ***** Variables *****
    $scope.showActivity = false;
    $scope.isLoading = true;
    $scope.profileUrl = '../../../images/Avatar.png';
    $scope.defaultChoreImage = '../../../images/BusyKidLogo.png';
    $scope.selectedDate = new Date();
    $scope.charities = [];

    $scope.isChoresLoading = true;
    $scope.selectedBucket = 'Chores & Activities';

    $scope.Chores = [];
    $scope.selectedWeekDay = {};
    $scope.weekDays = [];
    var getcreditcarddetail = bankAuthorizeService.GetCreditCardChildDashboard();
    $scope.dropdownHeaders = [
     {
         Id: 0,
         Name: 'Chores & Activities',
         IsSelected: false
     },
     {
         Id: 1,
         Name: 'Save',
         IsSelected: false
     },
      {
          Id: 2,
          Name: 'Share',
          IsSelected: false
      },
      {
          Id: 3,
          Name: 'Spend',
          IsSelected: false
      }];

    // ***** Variables *****

    // *** Methods *****

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
                $scope.getChoresByDate($scope.selectedDate);
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

    // get child financial overview
    $scope.getChildFinacialOverview = function () {
        $scope.isLoading = true;
        var weekDayName = $filter('date')(new Date(), 'EEEE');
        dashboardService.getChildFinacialOverview(weekDayName).success(function (response) {
            $scope.FinancialOverview = response;
            $scope.daysRemain = $scope.FinancialOverview.RemainingDays;
            $scope.isLoading = false;
        }).error(function (err) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    $scope.loadCharities = function () {
        charityService.getCharities().success(function (response) {
            $scope.charities = response;
        }).error(function (err) {
            DisplayAlert(err, 'danger');
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
                $scope.loadCharities();
                $scope.getChildFinacialOverview();
                $scope.getChoresByDate($scope.selectedDate);
                return;
            }
            $state.go('incompleteSignUp');
        }).error(function (err, data) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    }

    // Get chores by day
    $scope.getChoresByDate = function (date) {
        if (!$scope.isLoading)
            $scope.isChoresLoading = true;

        choreService.getChoresByDate($filter('date')(date, 'MM-dd-yyyy hh:mm:ss')).success(function (response) {
            angular.forEach(response, function (chore) {
                chore.IsProcessing = false;
                chore.IsChoreCompleted = false;
                if (chore.ImageUrl === undefined || chore.ImageUrl === null || chore.ImageUrl === '')
                    chore.ImageUrl = $scope.defaultChoreImage;

                if (chore.FamilyMember.ProfileImageUrl === undefined || chore.FamilyMember.ProfileImageUrl === null || chore.FamilyMember.ProfileImageUrl === '')
                    chore.FamilyMember.ProfileImageUrl = $scope.defaultProfileImage;
            });
            $scope.Chores = response;
            $scope.isChoresLoading = false;
            $scope.isLoading = false;
        }).error(function (err) {
            $scope.isChoresLoading = false;
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    // Complete the chore
    $scope.completeChore = function (chore) {
        if (!chore)
            return;

        if (chore.ImageUrl === '../../../images/DefaultChore.png')
            chore.ImageUrl = null;

        chore.IsProcessing = true;
        chore.IsCompleted = !chore.IsCompleted;
        chore.ChoreStatus = 'Completed';
        choreService.updateChore(chore).success(function (response) {
            var weekDayName = $filter('date')(new Date(), 'EEEE');
            dashboardService.getChildFinacialOverview(weekDayName).success(function (response) {
                $scope.FinancialOverview = response;
                $('#audioChoreCompleted')[0].play(); // Play audio chore completed
                chore.IsChoreCompleted = true;
                chore.IsProcessing = false;
                $timeout(function () {
                    var choreIndex = $scope.Chores.indexOf($filter('filter')($scope.Chores, { Id: chore.Id }, true)[0]);
                    $scope.Chores.splice(choreIndex, 1);
                }, 1000);

            }).error(function (err) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
            });
        }).error(function (err) {
            chore.IsCompleted = !chore.IsCompleted;
            chore.IsProcessing = false;
            DisplayAlert(err, 'danger');
        });
    };

    // checks the allocated setting tab, adds the boder to allocated setting tabs

    $scope.toggleBucket = function (bucketName) {

        $scope.selectedBucket = $scope.selectedBucket == bucketName ? '' : bucketName;

        switch (bucketName) {
            case 'Save':
                if (angular.element(document.querySelector('#spend')).hasClass('in'))
                    angular.element(document.querySelector('#spend')).removeClass('in')

                if (angular.element(document.querySelector('#share')).hasClass('in'))
                    angular.element(document.querySelector('#share')).removeClass('in')

                $("#save").collapse('toggle');

                $scope.getSaveData();
                break;
            case 'Share':
                if (angular.element(document.querySelector('#save')).hasClass('in'))
                    angular.element(document.querySelector('#save')).removeClass('in');

                if (angular.element(document.querySelector('#spend')).hasClass('in'))
                    angular.element(document.querySelector('#spend')).removeClass('in');

                $("#share").collapse('toggle');

                $scope.getCharities();
                break;
            case 'Spend':
                if (angular.element(document.querySelector('#save')).hasClass('in'))
                    angular.element(document.querySelector('#save')).removeClass('in');

                if (angular.element(document.querySelector('#share')).hasClass('in'))
                    angular.element(document.querySelector('#share')).removeClass('in');

                $("#spend").collapse('toggle');

                $scope.getSpendData();
                break;
        }
    };

    $scope.getSaveData = function () {
        $scope.$broadcast('loadStockGiftCards');
    }

    $scope.getSpendData = function () {
        $scope.$broadcast('getGiftCardsData');
    }

    $scope.getCharities = function () {
        $scope.$broadcast('loadCharities');
    }

    $scope.getAllTransactions = function () {
        $scope.$broadcast('loadAllTransactions', $scope.selectedChild);
    }

    $scope.donate = function () {

        // Remove it and get child from local
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/charity/donateView.html',
            controller: 'donateController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    $scope.cashOut = function () {
        $scope.currentChild = $scope.selectedChild;

        // Remove it and get child from local
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/spend/cashOutView.html',
            controller: 'cashOutController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    // Toggle the corresponding tab when clicking save, share or spend tabs
    $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
        $('.collapse').on('show.bs.collapse', function () {
            $('.collapse.in').collapse('hide');
        });
    });

    // Shows the move money modal
    $scope.showMoveMoneyModal = function () {
        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/earnings/_moveMoneyView.html',
            controller: 'moveMoneyController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };

    // Updates the child earnings.
    $scope.$on("updateEarnings", function () {
        $scope.getChildFinacialOverview();
    });

    // *** Methods *****

    // ***** View Loading *****

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.username = authData.Firstname;
        if (authData.ProfileUrl !== undefined && authData.ProfileUrl !== null && authData.ProfileUrl !== '')
            $scope.profileUrl = authData.ProfileUrl;
    }

    $scope.getWeekDays();

    $scope.checkSigupProgress();

    $scope.$on('showActivityTab', function () {
        $scope.showActivity = true;
        $scope.getAllTransactions();
    });

    $scope.scrollToContent = function (id) {
        //$('html,body').animate({
        //    scrollTop: $("#test" + id).offset().top
        //}, 'normal');
    };

    $scope.gotoChildDashboard = function () {
        $scope.getChildFinacialOverview();
    }
    $q.all([getcreditcarddetail]).then(function (response) {
       
        $scope.carddetail = response[0].data;
        var CardInfo = response[0].data;

        if (CardInfo.Tokenid != null && CardInfo.Tokenid != "" && $scope.carddetail.CardStatus == "Verified") {
            $scope.creditcard = 1;
        }
        else {
            $scope.creditcard = 0;
        }

        $scope.isLoading = false;
    }, function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });


    // ***** View Loading *****
}]);