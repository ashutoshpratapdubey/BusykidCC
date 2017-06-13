mjcApp.factory('subscriptionService', ['$http', '$q', 'settings', function ($http, $q, settings) {

    var subscriptionService = {};

    subscriptionService.subscriptionTypes = [
     {
         Id: 1,
         Name: 'Annual',
         PlaneName: 'Annual'
     },
     {
         Id: 2,
         Name: 'OneMonthTrial',
         PlaneName: 'One month free trial'
     },
      {
          Id: 3,
          Name: 'PromoPlan',
          PlaneName: 'Promo plan'
      },
        {
            Id: 4,
            Name: 'Pendingaccount',
            PlaneName: 'Pending microtransaction account'
        }

    ];

    subscriptionService.getFamilySubscription = function () {
        return $http.get(settings.apiBaseUri + 'api/subscription/getsubscription');
    }

    subscriptionService.GetSubscriptionStatus = function () {
        return $http.get(settings.apiBaseUri + 'api/subscription/getsubscriptionstatus');
    }

    subscriptionService.ValidatePromoCode = function (promoCode) {
        return $http.get(settings.apiBaseUri + 'api/subscription/validatepromocode/' + promoCode);
    }

    subscriptionService.Subscribe = function (subscription) {
        return $http.post(settings.apiBaseUri + 'api/subscription/subscribe', subscription);
    }
    //subscriptionService.authorizeSubscribePlan = function (subscription) {
    //    return $http.post(settings.apiBaseUri + 'api/subscription/AuthorizeSubscribePlan', subscription);
    //}

    subscriptionService.CancelSubscription = function () {
        return $http.put(settings.apiBaseUri + 'api/subscription/cancelsubscription');
    }

    return subscriptionService;
}]);
