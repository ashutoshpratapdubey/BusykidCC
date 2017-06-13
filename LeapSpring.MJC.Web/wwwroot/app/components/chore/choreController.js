mjcApp.controller('choreController', ['$scope', 'choreService', '$state', '$filter', '$stateParams', '$uibModal', 'localStorageService', '$timeout', '$q', 'familyMemberService', 'bankAuthorizeService','$window',
    function ($scope, choreService, $state, $filter, $stateParams, $uibModal, localStorageService, $timeout, $q, familyMemberService, bankAuthorizeService, $window) {

        $scope.isLoading = true;
        $scope.Chores = [];
        $scope.childrens = [];
        $scope.frequencyRange = '';
        $scope.frequencyName = 'Once';
        $scope.hasDueDate = true;
        $scope.signUpStatus = {};
        $scope.suggestedChores = [];
        $scope.totalCount = 0;
        $scope.isLoadMore = false;
        $scope.lblLoadMore = 'Load more';

        $scope.isSignUpProgress = {};
        $scope.isSignUpProgress.response = true;

        var getChildrens = familyMemberService.GetChildrens();
        var getMemberById = choreService.getMemberById($stateParams.id);
        var getSuggestedChores = choreService.getSuggestedChoresByDOB($stateParams.id, 0);

        // This queue will execute one by one
        $q.all([getMemberById, getSuggestedChores, getChildrens]).then(function (response) {
            $scope.child = response[0].data;
            $scope.suggestedChores = response[1].data.SystemChores;
            $scope.staticSuggestedChores = angular.copy($scope.suggestedChores);
            $scope.totalCount = response[1].data.TotalCount;
            $scope.childrens = response[2].data;

            if ($scope.totalCount > $scope.suggestedChores.length)
                $scope.isLoadMore = true;

            angular.forEach($scope.suggestedChores, function (suggestedChore) {
                suggestedChore.Value = $filter('number')(suggestedChore.Value, 2);

                suggestedChore.WeekDays = choreService.getWeekDays();
                suggestedChore.Frequencies = choreService.getFrequencies();
                suggestedChore.DueDates = choreService.getDueDates();
            });
            $scope.isLoading = false;
        }, function (err) {
            $scope.isLoading = false;
            DisplayAlert(err, 'danger');
        });

        $scope.addSuggestedChore = function (suggestedChore) {
            $scope.prepareFrequencyRange(suggestedChore.FrequencyType, suggestedChore);
            $scope.hasDueDate = $scope.HasDueDate(suggestedChore);
            if (!suggestedChore || (suggestedChore.FrequencyType === 'Once' && !$scope.hasDueDate) || (suggestedChore.FrequencyType !== 'Once' && !$scope.frequencyRange))
                return;

            $scope.CheckSystemChoreUpdated(suggestedChore);

            var chore = {
                Name: suggestedChore.Name,
                Value: suggestedChore.Value,
                ImageUrl: suggestedChore.ImageUrl,
                DueDate: null,
                SystemChoreID: suggestedChore.Id,
                FrequencyType: suggestedChore.FrequencyType,
                FrequencyRange: $scope.frequencyRange,
                ChoreStatus: 0,
                FamilyMemberID: $scope.child.Id,
                IsSystemChoreUpdated: $scope.IsSystemChoreUpdated
            }

            var choreCreatedDay = (suggestedChore.FrequencyType == 'Once') ? $filter('date')($scope.getDueDate(suggestedChore), 'EEEE') : $filter('date')(new Date(), 'EEEE');
            suggestedChore.isAdding = true;
            choreService.addChore(chore, choreCreatedDay).success(function (response) {
                var foundedSuggestedChore = $filter('filter')($scope.suggestedChores, { Id: suggestedChore.Id }, true)[0];
                foundedSuggestedChore.isAddSuccess = true;
                var indexOfSuggestedChore = $scope.suggestedChores.indexOf(foundedSuggestedChore);

                $timeout(function () {
                    $('#audioAddChore')[0].play(); // Play add chore audio
                    $scope.suggestedChores.splice(indexOfSuggestedChore, 1);
                    $scope.staticSuggestedChores.splice(indexOfSuggestedChore, 1);
                    foundedSuggestedChore.isAdding = false;
                    response.Value = $filter('number')(chore.Value, 2);
                    response.Frequency = chore.FrequencyType;
                    $scope.Chores.push(response);
                }, 500);
            }).error(function () {
                var foundedSuggestedChore = $filter('filter')($scope.suggestedChores, { Id: suggestedChore.Id }, true)[0];
                foundedSuggestedChore.isAdding = false;
            });
        };

        $scope.blockSuggestedChore = function (suggestedChore) {
            if (suggestedChore === null)
                return;

            suggestedChore.isBlocking = true;
            var foundedSuggestedChore = $filter('filter')($scope.suggestedChores, { Id: suggestedChore.Id }, true)[0];
            foundedSuggestedChore.isBlockSuccess = true;
            var indexOfSuggestedChore = $scope.suggestedChores.indexOf(foundedSuggestedChore);

            $timeout(function () {
                $scope.suggestedChores.splice(indexOfSuggestedChore, 1);
                $scope.staticSuggestedChores.splice(indexOfSuggestedChore, 1);
                foundedSuggestedChore.isBlocking = false;
            }, 500);
        };

        $scope.addChores = function (customChore) {
            customChore.Frequency = customChore.FrequencyType;
            $scope.Chores.push(customChore);
        };

        $scope.updateChores = function (customChore) {
            customChore.Frequency = customChore.FrequencyType;
            var index = $scope.Chores.indexOf($filter('filter')($scope.Chores, { Id: customChore.Id }, true)[0]);
            $scope.Chores.splice(index, 1, customChore);
        };

        $scope.deleteChores = function (choreId) {
            var index = $scope.Chores.indexOf($filter('filter')($scope.Chores, { Id: choreId }, true)[0]);
            $scope.Chores.splice(index, 1);
        };

        $scope.getUpdatedChores = function () {
            // This queue will execute one by one
            choreService.getSuggestedChoresByDOB($stateParams.id, $scope.suggestedChores.length).success(function (response) {
                $scope.suggestedChores = response.SystemChores;

                angular.forEach($scope.suggestedChores, function (suggestedChore) {
                    suggestedChore.Value = $filter('number')(suggestedChore.Value, 2);

                    suggestedChore.WeekDays = choreService.getWeekDays();
                    suggestedChore.Frequencies = choreService.getFrequencies();
                    suggestedChore.DueDates = choreService.getDueDates();
                });
            }).error(function (err) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
            });
        };

        // Edit the selected chore
        $scope.editChore = function (chore) {
            $scope.selectedChild = $scope.child;
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

        // Shows the add chore modal
        $scope.showAddChoreModal = function () {
            // Remove it and get child from local
            $scope.modalInstance = $uibModal.open({
                scope: $scope,
                templateUrl: '/app/components/chore/addChore.html',
                controller: 'addChoreController',
                size: 'sm',
                windowClass: 'box-popup'
            });
        };

        $scope.prepareFrequencyRange = function (frequencyName, chore) {
            $scope.frequencyRange = '';
            if (frequencyName === 'Once')
                return;

            angular.forEach(chore.WeekDays, function (weekDay) {
                if (weekDay.IsSelected)
                    $scope.frequencyRange += ($scope.frequencyRange === undefined || $scope.frequencyRange === '') ? weekDay.Name : ',' + weekDay.Name;
            });
        };

        $scope.resetWeekDays = function (frequencyName, chore) {
            angular.forEach(chore.WeekDays, function (day) {
                day.IsSelected = (frequencyName === 'Daily');
            });
        };

        $scope.frequecyChanged = function (frequencyName, chore) {
            chore.FrequencyType = frequencyName;
            $scope.resetWeekDays(chore.FrequencyType, chore);
            if (chore.FrequencyType === 'Weekly')
                chore.WeekDays[1].IsSelected = true;
        };

        $scope.toggleWeekDay = function (weekDay, chore) {
            angular.forEach(chore.WeekDays, function (day) {
                if (day.Id === weekDay.Id)
                    day.IsSelected = !day.IsSelected;
            });
        };

        $scope.changeDueDate = function (due, chore) {
            angular.forEach(chore.DueDates, function (dueDate) {
                dueDate.IsSelected = false;
                if (dueDate.DueDate === due.DueDate) {
                    dueDate.IsSelected = true;
                    chore.dueDate = new Date(due.DueDate);
                }
            });
            $scope.hasDueDate = $scope.HasDueDate(chore);
        };

        $scope.getDueDate = function (chore) {
            var dueDate;
            angular.forEach(chore.DueDates, function (due) {
                if (due.IsSelected) {
                    dueDate = new Date(due.DueDate);
                }
            });
            return dueDate;
        };

        $scope.HasDueDate = function (chore) {
            var isDueDateSelected = false;
            angular.forEach(chore.DueDates, function (dueDate) {
                if (dueDate.IsSelected)
                    isDueDateSelected = true;
            });
            return isDueDateSelected;
        };

        $scope.gotoNextPage = function () {           
            //if ($scope.Chores.length > 0) {
                if ($scope.signUpStatus.data !== 'Completed') {
                    //   $state.go('subscription'); // Sending directly to link bank account
                    $state.go('linkbankinfo');
                } else {
                    choreService.childId = $stateParams.id;
                    $state.go('adminDashboard');
                }
            //}
            //else {
            //    if ($window.confirm("Do you want to continue?")) {
            //        if ($scope.signUpStatus.data !== 'Completed') {
            //            $state.go('linkbankinfo');
            //        } else {
            //            choreService.childId = $stateParams.id;
            //            $state.go('adminDashboard');
            //        }
            //    }              
            //}
          
        };

        $scope.CheckSystemChoreUpdated = function (suggestedChore) {
            var chore = $filter('filter')($scope.staticSuggestedChores, { Id: suggestedChore.Id }, true)[0];
            if (chore.Name !== suggestedChore.Name || chore.Value !== suggestedChore.Value || chore.FrequencyType !== suggestedChore.FrequencyType)
                $scope.IsSystemChoreUpdated = true;
            else
                $scope.IsSystemChoreUpdated = false;
        }

        $scope.calculateAge = function (dob) {
            var ageDifferentMilliseconds = Date.now() - new Date(dob);
            var ageDate = new Date(ageDifferentMilliseconds);
            return Math.abs(ageDate.getUTCFullYear() - 1970);
        }

        $scope.loadMore = function () {
            $scope.lblLoadMore = "Loading...";
            $scope.isLoadMoreProgress = true;
            choreService.getSuggestedChoresByDOB($stateParams.id, $scope.suggestedChores.length).success(function (response) {
                $scope.totalCount = response.TotalCount;
                angular.forEach(response.SystemChores, function (suggestedChore) {
                    suggestedChore.Value = $filter('number')(suggestedChore.Value, 2);
                    suggestedChore.WeekDays = choreService.getWeekDays();
                    suggestedChore.Frequencies = choreService.getFrequencies();
                    suggestedChore.DueDates = choreService.getDueDates();

                    $scope.suggestedChores.push(suggestedChore);
                    $scope.staticSuggestedChores.push(suggestedChore);
                });

                $scope.isLoadMore = ($scope.totalCount > $scope.suggestedChores.length)
                $scope.lblLoadMore = "Load more";
                $scope.isLoadMoreProgress = false;
            }).error(function (err) {
                $scope.lblLoadMore = "Load more";
                $scope.isLoading = false;
                $scope.isLoadMoreProgress = false;
                DisplayAlert(err, 'danger');
            });
        }

    }]);