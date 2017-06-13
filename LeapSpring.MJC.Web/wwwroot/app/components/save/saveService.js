mjcApp.factory('saveService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var saveService = {};
    saveService.isBuyStock = false;
    saveService.showHome = true;
    saveService.showActivity = false;

    saveService.getStockGiftCards = function (isFeaturedStock) {
        return $http.get(settings.apiBaseUri + 'api/save/getstockgiftcards/' + isFeaturedStock);
    };

    saveService.getPurchasedStockGiftCards = function () {
        return $http.get(settings.apiBaseUri + 'api/save/getpurchasedstockgiftcards');
    };

    saveService.getDisapprovedStockGiftCards = function () {
        return $http.get(settings.apiBaseUri + 'api/save/getdisapprovedstockgiftcards');
    };

    saveService.initiateStockPurchase = function (stockGiftCard) {
        return $http.post(settings.apiBaseUri + 'api/save/initiatestockpurchase', stockGiftCard);
    };

    return saveService;
}]);
