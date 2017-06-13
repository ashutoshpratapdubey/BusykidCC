
mjcApp.factory('spendService', ['$http', 'settings', function ($http, settings) {

    var spendService = {};

    spendService.GetGiftCards = function () {
        return $http.get(settings.apiBaseUri + 'api/spend/getgiftcards/');
    }

    spendService.GetGiftCardPreview = function (isFeature) {
        return $http.get(settings.apiBaseUri + 'api/spend/getgiftcardpreviews/' + isFeature);
    }

    spendService.purchaseGiftCard = function (giftCardId) {
        return $http.get(settings.apiBaseUri + 'api/spend/purchasegiftcard/'+ giftCardId);
    }

    spendService.getPurchasedGiftCards = function () {
        return $http.get(settings.apiBaseUri + 'api/spend/getpurchasedgiftcards');
    }

    spendService.deletepurchaseGiftCard = function (purchasedGiftCardId) {
        return $http.delete(settings.apiBaseUri + 'api/spend/deletepurchasedgiftcard/' + purchasedGiftCardId);
    }

    spendService.cashOut = function (cashOut) {
        return $http.put(settings.apiBaseUri + 'api/spend/cashout', cashOut);
    }

    return spendService;
}]);