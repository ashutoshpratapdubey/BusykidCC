mjcApp.factory('NonplaidbankAuthorizeService', ['$http', 'settings', function ($http, settings) {

    var NonplaidbankAuthorizeService = {};

    NonplaidbankAuthorizeService.GetFinancialAccount = function () {
        return $http.get(settings.apiBaseUri + 'api/nonplaidbankauthorization/getfinancialaccount');
    };

    NonplaidbankAuthorizeService.GetBankDocuments = function () {
        return $http.get(settings.apiBaseUri + 'api/nonplaidbankauthorization/getbankdocuments');
    };

    NonplaidbankAuthorizeService.createCustomer = function () {
        return $http.put(settings.apiBaseUri + 'api/nonplaidbankauthorization/createcustomer');
    };

    NonplaidbankAuthorizeService.linkBankAccount = function (publicToken, institutionName, selectedAccountId, isMicroDeposit) {
        return $http.put(settings.apiBaseUri + 'api/nonplaidbankauthorization/linkbankaccount?publicToken=' + publicToken + '&institutionName=' + institutionName + '&selectedAccountId=' + selectedAccountId);
    };

    NonplaidbankAuthorizeService.linkMicroDepositAccount = function (accountNumber, routingNumber, accountType) {
        return $http.put(settings.apiBaseUri + 'api/nonplaidbankauthorization/linkmicrodepositaccount?accountNumber=' + accountNumber + '&routingNumber=' + routingNumber + '&accountType=' + accountType);
    };

    NonplaidbankAuthorizeService.verifyBankAccount = function (firstAmount, secondAmount) {
        return $http.put(settings.apiBaseUri + 'api/nonplaidbankauthorization/verify?firstAmount=' + parseFloat(firstAmount).toFixed(2) + '&secondAmount=' + parseFloat(secondAmount).toFixed(2));
    };

    NonplaidbankAuthorizeService.disconnectAccount = function () {
        return $http.delete(settings.apiBaseUri + 'api/nonplaidbankauthorization/removebank');
    };

    NonplaidbankAuthorizeService.IsBankLinked = function () {
        return $http.get(settings.apiBaseUri + 'api/nonplaidbankauthorization/isbanklinked');
    };

    NonplaidbankAuthorizeService.GetLinkedBankStatus = function () {
        return $http.get(settings.apiBaseUri + 'api/nonplaidbankauthorization/getlinkedbankstatus');
    };

    NonplaidbankAuthorizeService.GetPromocode = function () {
       return $http.get(settings.apiBaseUri + 'api/nonplaidbankauthorization/PrePromocode');
    };

    return NonplaidbankAuthorizeService;
}]);