mjcApp.factory('choreService', ['$http', '$q', '$filter', 'localStorageService', 'settings', '$state', function ($http, $q, $filter, localStorageService, settings, $state) {

    var choreService = {};
    choreService.dueDates = [];

    choreService.childId = 0;

    choreService.getDueDates = function (dueDate) {
        var todayDate = (!dueDate) ? new Date().getDate() : new Date(choreService.parseDate(dueDate)).getDate();
        choreService.dueDates = [];
        for (var i = 0; i < 7; i++) {
            var dueDate = new Date().setDate(todayDate + i);
            choreService.dueDates.push({ DueDate: new Date(dueDate), Name: $filter('date')(dueDate, 'EEEE'), IsSelected: false, date: $filter('date')(dueDate, 'dd/MM/yyyy') })
        }
        choreService.dueDates[0].IsSelected = true;
        return choreService.dueDates;
    };

    choreService.parseDate = function (input) {
        var parts = input.match(/(\d+)/g);
        return new Date(parts[0], parts[1] - 1, parts[2]);
    }

    // Get child by id
    choreService.getMemberById = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/family/getbyid/' + memberId);
    };

    // Get chore by id
    choreService.getChoreById = function (choreId) {
        return $http.get(settings.apiBaseUri + 'api/chore/getbyid/' + choreId);
    };


    // Get suggested chores
    choreService.getSuggestedChoresByDOB = function (memberId, skipCount) {
        return $http.get(settings.apiBaseUri + 'api/chore/getsystemchoresbyagerange?memberId=' + memberId + '&skipCount=' + skipCount);
    };

    // Get chores by family member identifier
    choreService.getChoresByFamilyMemberId = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/chore/getchoresbyfamilymember?memberId=' + memberId);
    };

    // Add chore
    choreService.addChore = function (chore, weekDay) {
        return $http.put(settings.apiBaseUri + 'api/chore/addchore/' + weekDay, chore);
    };

    choreService.searchChores = function (familyMemberId, keyWord) {
        return $http.get(settings.apiBaseUri + 'api/chore/searchchores?familyMemberId=' + familyMemberId + '&keyword=' + keyWord);
    };

    choreService.getChoresByDate = function (date, familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/chore/getchoresbydate?date=' + date + '&familyMemberId=' + familyMemberId);
    };

    // Update the chore
    choreService.updateChore = function (chore, weekDay) {
        return $http.put(settings.apiBaseUri + 'api/chore/update/' + weekDay, chore);
    };

    // Delete the chore
    choreService.deleteChore = function (choreId) {
        return $http.delete(settings.apiBaseUri + 'api/chore/delete/' + choreId);
    };

    choreService.getFrequencies = function () {
        return [{
            Id: 0,
            Name: 'Once'
        }, {
            Id: 1,
            Name: 'Daily'
        }, {
            Id: 2,
            Name: 'Weekly'
        }];
    };

    choreService.getWeekDays = function () {
        return [{
            Id: 0,
            Name: 'Sunday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 1,
            Name: 'Monday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 2,
            Name: 'Tuesday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 3,
            Name: 'Wednesday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 4,
            Name: 'Thursday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 5,
            Name: 'Friday',
            IsSelected: true,
            IsToday: false
        }, {
            Id: 6,
            Name: 'Saturday',
            IsSelected: true,
            IsToday: false
        }];
    };

    return choreService;
}]);