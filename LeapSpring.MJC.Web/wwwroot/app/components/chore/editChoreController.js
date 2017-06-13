mjcApp.controller('editChoreController', ['$scope', '$uibModalInstance', '$filter', '$state', 'localStorageService', 'choreService', function ($scope, $uibModalInstance, $filter, $state, localStorageService, choreService) {
    // *** Variables*** //
    $scope.dueDate;
    $scope.hasDueDate = true;
    $scope.chore = $scope.selectedChore;
    $scope.children = $scope.selectedChild;
    $scope.frequencyRange = '';
    $scope.showSuccessMessage = false;
    $scope.isChoreLoading = true;

    $scope.isProcessing = false;
    $scope.btnSaveContent = 'Save Edits';
    $scope.btnDeleteContent = 'Delete Chore';

    $scope.frequencies = choreService.getFrequencies();
    $scope.weekDays = choreService.getWeekDays();

    // *** Variables End*** //

    // *** Methods ***//

    $scope.hideModal = function () {
        if ($scope.isProcessing)
            return;

        $uibModalInstance.dismiss('cancel');
    };

    // Gets the recurring chore
    $scope.getRecurringChore = function (recurringChoreId) {
        choreService.getChoreById(recurringChoreId).success(function (response) {
            response.NameofChore = response.Name;
            response.FrequencyName = response.FrequencyType;
            response.FrequencyType = $filter('filter')($scope.frequencies, { Name: response.FrequencyType }, true)[0].Id;
            response.Value = $filter('number')(response.Value, 2);

            $scope.chore = response;

            if (response.FrequencyType === 'Once' || response.FrequencyType == 0) {
                $scope.isChoreLoading = false;
                return;
            }
            var frequencyRanges = $scope.chore.FrequencyRange.split(',');
            $scope.setFrequency(frequencyRanges);
            $scope.isChoreLoading = false;
        }).error(function (err) {
            $scope.isChoreLoading = false;
            DisplayAlert(err, 'danger');
        });
    };

    // reset the week days
    $scope.resetWeekDays = function () {
        angular.forEach($scope.weekDays, function (day) {
            day.IsSelected = ($scope.chore.FrequencyType == 1);
        });
    };

    $scope.OnChoreSelected = function (chore) {
        $scope.chore.Name = chore.NameofChore;
        $scope.chore.Value = $filter('number')(chore.Value, 2);
        $scope.chore.DueDate = (chore.FrequencyType == 'Once') ? $scope.getDueDate() : null;
        $scope.chore.SystemChoreID = chore.SystemChoreId;
        $scope.chore.FrequencyType = chore.FrequencyType;

        $scope.getSelectedFrequency($scope.chore.FrequencyType);
    };

    $scope.setFrequency = function (frequencyRange) {
        angular.forEach($scope.weekDays, function (weekDay) {
            weekDay.IsSelected = false;
        });
        angular.forEach(frequencyRange, function (frequencyName) {
            $filter('filter')($scope.weekDays, { Name: frequencyName }, true)[0].IsSelected = true;
        });
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

    $scope.setDueDate = function (date) {
        var selectedDueDate = new Date(choreService.parseDate(date));
        angular.forEach($scope.dueDates, function (dueDate) {
            dueDate.IsSelected = false;
            if ($filter('date')(dueDate.DueDate, 'dd/MM/yyyy') === $filter('date')(selectedDueDate, 'dd/MM/yyyy')) {
                dueDate.IsSelected = true;
                $scope.dueDate = selectedDueDate;
            }
        });
        $scope.hasDueDate = $scope.HasDueDate();
    };

    $scope.changeDueDate = function (due) {
        angular.forEach($scope.dueDates, function (dueDate) {
            dueDate.IsSelected = false;
            if (dueDate.DueDate === due.DueDate) {
                dueDate.IsSelected = true;
                $scope.dueDate = due.DueDate;
            }
        });
        $scope.hasDueDate = $scope.HasDueDate();
    };

    $scope.getDueDate = function (due) {
        var dueDate;
        angular.forEach($scope.dueDates, function (due) {
            if (due.IsSelected) {
                dueDate = due.DueDate;
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

        return choreService.searchChores($scope.children.Id, keyword).then(function (response) {
            return response.data.map(function (item) {
                return item;
            });
        });
    };

    // Update chore
    $scope.updateChore = function () {
        if ($scope.isProcessing)
            return;

        $scope.prepareFrequencyRange();
        $scope.hasDueDate = $scope.HasDueDate();
        if (!$scope.chore || ($scope.chore.FrequencyType == 0 && !$scope.hasDueDate) || ($scope.chore.FrequencyType != 0 && !$scope.frequencyRange))
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.editChoreForm.$valid) {

            $scope.isProcessing = true;
            $scope.btnSaveContent = 'Saving...';

            if ($scope.chore.ImageUrl === '../../../images/DefaultChore.png')
                $scope.chore.ImageUrl = null;

            $scope.chore.Name = $scope.chore.NameofChore;
            $scope.chore.Value = $filter('number')($scope.chore.Value, 2);
            $scope.chore.DueDate = ($scope.chore.FrequencyType == 0) ? $scope.getDueDate() : null;
            $scope.chore.SystemChoreID = $scope.chore.SystemChoreID;
            $scope.chore.FrequencyType = $scope.chore.FrequencyType;
            $scope.chore.FrequencyRange = $scope.frequencyRange;
            $scope.chore.ChoreStatus = 0;

            var choreEditedDay = ($scope.chore.FrequencyType == 0) ? $filter('date')($scope.getDueDate(), 'EEEE') : $filter('date')(new Date(), 'EEEE');
            choreService.updateChore($scope.chore, choreEditedDay).success(function (response) {
                $scope.isProcessing = false;
                $scope.btnSaveContent = 'Save Edits';
                $scope.hideModal();
                if ($state.current.name === 'suggestedChore') {
                    $scope.updateChores(response);
                    return;
                }
                $scope.getChoresByDate($scope.selectedDate, $scope.children.Id);
            }).error(function (err) {
                $scope.isProcessing = false;
                $scope.btnSaveContent = 'Save Edits';
                DisplayAlert(err, 'danger');
            });
        }
    };

    // Delete Chore
    $scope.deleteChore = function () {
        if ($scope.isProcessing)
            return;

        $scope.$broadcast('show-errors-check-validity');
        if ($scope.editChoreForm.$valid) {
            $scope.isProcessing = true;
            $scope.btnDeleteContent = 'Deleting...';

            choreService.deleteChore($scope.chore.Id).success(function (response) {
                $scope.isProcessing = false;
                $scope.btnDeleteContent = 'Delete Chore';
                $scope.hideModal();
                if ($state.current.name === 'suggestedChore') {
                    $scope.deleteChores($scope.chore.Id);
                    return;
                }
                $scope.getChoresByDate($scope.selectedDate, $scope.children.Id);
            }).error(function (err) {
                $scope.isProcessing = false;
                $scope.btnDeleteContent = 'Delete Chore';
                DisplayAlert(err, 'danger');
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

    // *** Methods End***//


    // *** View Loading ** //

    // Gets the due date from today
    $scope.dueDates = choreService.getDueDates($scope.selectedChore.DueDate);

    if ($scope.children.ProfileImageUrl === undefined || $scope.children.ProfileImageUrl === null || $scope.children.ProfileImageUrl === '')
        $scope.children.ProfileImageUrl = '../../../images/Avatar.png';

    $scope.getSelectedFrequency($scope.chore.FrequencyType)

    if ($scope.selectedChore.Frequency === 'Once') {
        $scope.getRecurringChore($scope.selectedChore.Id);
        $scope.setDueDate($scope.chore.DueDate);
        $scope.isChoreLoading = false;
    } else {
        var recurringChoreID = $scope.selectedChore.RecurringChoreID === null ? $scope.selectedChore.Id : $scope.selectedChore.RecurringChoreID;
        $scope.getRecurringChore(recurringChoreID);
    }

    // *** View Loading Ends ** //
}]);

