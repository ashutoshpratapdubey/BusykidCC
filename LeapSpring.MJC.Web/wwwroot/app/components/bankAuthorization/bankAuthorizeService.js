mjcApp.factory('bankAuthorizeService', ['$http', 'settings', function ($http, settings) {

    var bankAuthorizeService = {};

    bankAuthorizeService.GetFinancialAccount = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getfinancialaccount');
    };

    bankAuthorizeService.GetBankDocuments = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getbankdocuments');
    };

    bankAuthorizeService.createCustomer = function () {
        return $http.put(settings.apiBaseUri + 'api/bankauthorization/createcustomer');
    };

    bankAuthorizeService.linkBankAccount = function (publicToken, institutionName, selectedAccountId, isMicroDeposit) {
        return $http.put(settings.apiBaseUri + 'api/bankauthorization/linkbankaccount?publicToken=' + publicToken + '&institutionName=' + institutionName + '&selectedAccountId=' + selectedAccountId);
    };

    bankAuthorizeService.linkMicroDepositAccount = function (accountNumber, routingNumber, accountType) {
        return $http.put(settings.apiBaseUri + 'api/bankauthorization/linkmicrodepositaccount?accountNumber=' + accountNumber + '&routingNumber=' + routingNumber + '&accountType=' + accountType);
    };

    bankAuthorizeService.verifyBankAccount = function (firstAmount, secondAmount) {
        return $http.put(settings.apiBaseUri + 'api/bankauthorization/verify?firstAmount=' + parseFloat(firstAmount).toFixed(2) + '&secondAmount=' + parseFloat(secondAmount).toFixed(2));
    };

    bankAuthorizeService.disconnectAccount = function () {
        return $http.delete(settings.apiBaseUri + 'api/bankauthorization/removebank');
    };

    bankAuthorizeService.IsBankLinked = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/isbanklinked');
    };

    bankAuthorizeService.GetLinkedBankStatus = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getlinkedbankstatus');
    };

    bankAuthorizeService.GetPromocode = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/PrePromocode');
    };
    bankAuthorizeService.GetCreditCard = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getcreditcard');
    }

    bankAuthorizeService.GetCreditCardChildDashboard = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getcreditcardinfo');
    }

    bankAuthorizeService.ChangeCreditCardStatus = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/changecreditcardstatus');
    }

    bankAuthorizeService.ChangeCreditCardStatus = function () {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/changecreditcardstatus');
    }

    bankAuthorizeService.ValidateCreditCard = function () {

        return $http.get(settings.apiBaseUri + 'api/bankauthorization/getcreditcarddetails');

    }

    bankAuthorizeService.ValidateThreeStepApiUrl = function (tokenID, cardtype) {
        return $http.get(settings.apiBaseUri + 'api/bankauthorization/validatetokenid/' + tokenID +'/'+ cardtype);
    }

    return bankAuthorizeService;
}]);