mjcApp.factory('earningsService', ['$http', 'settings', function ($http, settings) {

    var earningsService = {};

    earningsService.GetBucketTypes = function () {
        return $http.get(settings.apiBaseUri + 'api/earnings/getbuckettypes');
    }

    earningsService.GetByMemberId = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/earnings/getbymemberid/' + familyMemberId);
    }

    earningsService.MoveMoney = function (sourceBucket, destinationBucket, amount) {
        return $http.get(settings.apiBaseUri + 'api/earnings/movemoney?sourceBucket=' + sourceBucket + '&destinationBucket=' + destinationBucket + '&amount=' + amount);
    }

    return earningsService;
}]);