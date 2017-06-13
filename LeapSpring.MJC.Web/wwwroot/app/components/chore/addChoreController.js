mjcApp.controller('addChoreController', ['$scope', '$uibModalInstance', '$filter', '$state', '$stateParams', 'localStorageService', 'choreService', '$timeout', function ($scope, $uibModalInstance, $filter, $state, $stateParams, localStorageService, choreService, $timeout) {
    // *** Variables*** //
    $scope.isContinue = false;
    $scope.btnContinue = 'Continue';
    $scope.dueDate;
    $scope.hasDueDate = true;
    $scope.chore = {};
    $scope.children = {};
    $scope.childrens = $scope.childrens;
    $scope.frequencyRange = '';
    $scope.currentChildId = $stateParams.id;

    $scope.frequencies = choreService.getFrequencies();
    $scope.weekDays = choreService.getWeekDays();
    $scope.chore.FrequencyName = 'Once';
    $scope.chore.FrequencyType = 0;
    $scope.isVisible = true;

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        $scope.isVisible = false;
        $uibModalInstance.dismiss('cancel');
    };

    $scope.resetWeekDays = function () {
        angular.forEach($scope.weekDays, function (day) {
            day.IsSelected = ($scope.chore.FrequencyType == 1);
        });
    };

    $scope.OnChoreSelected = function (chore) {
        chore.Value = $filter('number')(chore.Value, 2);
        $scope.chore = chore;
        $scope.getSelectedFrequency($scope.chore.FrequencyType);
    };

    $scope.getSelectedFrequency = function (frequencyId) {
        angular.forEach($scope.frequencies, function (frequency) {
            if (frequency.Name === frequencyId) {
                $scope.chore.FrequencyName = frequency.Name;
                $scope.chore.FrequencyType = frequency.Id;
                $scope.resetWeekDays();
            }
        });
    };

    $scope.toggleWeekDay = function (weekDay) {
        angular.forEach($scope.weekDays, function (day) {
            if (day.Id === weekDay.Id)
                day.IsSelected = !day.IsSelected;
        });
    };

    $scope.changeDueDate = function (due) {
        angular.forEach($scope.dueDates, function (dueDate) {
            dueDate.IsSelected = false;
            if (dueDate.DueDate === due.DueDate) {
                dueDate.IsSelected = true;
                $scope.dueDate = new Date(due.DueDate);
            }
        });
        $scope.hasDueDate = $scope.HasDueDate();
    };

    $scope.getDueDate = function (due) {
        var dueDate;
        angular.forEach($scope.dueDates, function (due) {
            if (due.IsSelected) {
                dueDate = new Date(due.DueDate);
            }
        });
        return dueDate;
    };

    $scope.prepareFrequencyRange = function () {
        $scope.frequencyRange = '';
        if ($scope.chore.FrequencyType === 0)
            return;

        angular.forEach($scope.weekDays, function (weekDay) {
            if (weekDay.IsSelected)
                $scope.frequencyRange += ($scope.frequencyRange === undefined || $scope.frequencyRange === '') ? weekDay.Name : ',' + weekDay.Name;
        });
    };

    $scope.getChores = function (keyword) {
        if (!keyword)
            return;

        var memberId = ($scope.childrens.length > 1) ? $scope.childrens[$("#scrolling ul").getActiveIndex()].Id : $scope.children.Id;

        return choreService.searchChores(memberId, keyword).then(function (response) {
            return response.data.map(function (item) {
                return item;
            });
        });
    };

    $scope.addCustomChore = function () {
        $scope.$broadcast('show-errors-check-validity');
        $scope.prepareFrequencyRange();
        $scope.hasDueDate = $scope.HasDueDate();
        if (!$scope.chore || ($scope.chore.FrequencyType == 0 && !$scope.hasDueDate)
            || ($scope.chore.FrequencyType != 0 && !$scope.frequencyRange)
            || $scope.chore.NameofChore === undefined || $scope.chore.NameofChore.replace(/ /g, '') === ''
            || $scope.chore.Value === '' || $scope.chore.Value <= 0)
            return;

        if ($scope.addChoreForm.$valid) {
            var currentChild = ($scope.childrens.length > 1) ? $scope.childrens[$("#scrolling ul").getActiveIndex()] : $scope.children;
            var chore = {
                Name: $scope.chore.NameofChore,
                Value: $filter('number')($scope.chore.Value, 2),
                ImageUrl: $scope.chore.ImageUrl,
                DueDate: null,
                SystemChoreID: $scope.chore.SystemChoreId,
                FrequencyType: $scope.chore.FrequencyType,
                FrequencyRange: $scope.frequencyRange,
                ChoreStatus: 0,
                FamilyMemberID: currentChild.Id
            }

            var choreCreatedDay = ($scope.chore.FrequencyType == 0) ? $filter('date')($scope.getDueDate(), 'EEEE') : $filter('date')(new Date(), 'EEEE');
            $scope.isContinue = true;
            $scope.btnContinue = 'Continuing...';
            choreService.addChore(chore, choreCreatedDay).success(function (response) {
                $scope.chore = response;
                $scope.addChores(response);
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
                choreService.childId = 0;
                $scope.hideModal();
            }).error(function () {
                $scope.isContinue = false;
                $scope.btnContinue = 'Continue';
            });
        }
    };

    $scope.frequecyChanged = function (frequencyId, frequencyName) {
        $scope.chore.FrequencyName = frequencyName;
        $scope.chore.FrequencyType = frequencyId;
        $scope.resetWeekDays();
        if ($scope.chore.FrequencyType === 2)
            $scope.weekDays[1].IsSelected = true;
    };

    $scope.HasDueDate = function () {
        var isDueDateSelected = false;
        angular.forEach($scope.dueDates, function (dueDate) {
            if (dueDate.IsSelected)
                isDueDateSelected = true;
        });
        return isDueDateSelected;
    };

    $scope.viewChildProfile = function () {
        choreService.childId = $scope.currentChildId;
        $state.go('adminDashboard');
    };

    // *** Methods End***//


    // *** View Loading ** //

    // Gets the due date from today
    $scope.dueDates = choreService.getDueDates();

    // Loads the childrens
    $timeout(function () {
        if ($scope.childrens.length <= 1)
            return;

        $("#scrolling ul").itemslide({ one_item: true }); //initialize itemslide

        $(window).resize(function () {
            if (!$scope.isVisible)
                return;

            $("#scrolling ul").reload();
        }); //Recalculate width and center positions and sizes when window is resized

        var childIndex = $scope.childrens.indexOf($filter('filter')($scope.childrens, { Id: parseInt($stateParams.id) }, true)[0]);
        $("#scrolling ul").gotoSlide(childIndex);

    }, 100);


    if ($scope.childrens.length > 1) {
        angular.forEach($scope.childrens, function (child) {
            if (child.ProfileImageUrl === undefined || child.ProfileImageUrl === '' || child.ProfileImageUrl === null) {
                child.ProfileImageUrl = '../../../images/Avatar.png';
            }
        });
    } else {
        $scope.children = $scope.childrens[0];
        if ($scope.children.ProfileImageUrl === undefined || $scope.children.ProfileImageUrl === '' || $scope.children.ProfileImageUrl === null) {
            $scope.children.ProfileImageUrl = '../../../images/Avatar.png';
        }
    }

    // *** View Loading Ends ** //
}]);
