mjcApp.factory('accountService', ['$http', '$q', 'localStorageService', 'settings', '$state', '$location', '$window', '$remember', '$rootScope', function ($http, $q, localStorageService, settings, $state, $location, $window, $remember,$rootScope) {

    var accountService = {};

    var authentication = {
        isAuth: false,
        userName: ""
    };

    accountService.login = function (loginCredential) {
        var deferred = $q.defer();
        localStorageService.remove('authorizationData');
           $http.put(settings.apiBaseUri + 'api/account/signin', loginCredential).success(function (response) {
            localStorageService.set('authorizationData', response);
            deferred.resolve(response);
        }).error(function (err, status) {
            accountService.logout();
            DisplayAlert(err, 'danger');
            deferred.reject(err);
        });

        return deferred.promise;
    };

    accountService.logout = function () {
        var authData = localStorageService.get('authorizationData');
        if (authData != null) {
                   var familyUrl = authData.FamilyUrl;
            if ($remember('username') == null || $remember('username') == '')
            {
                localStorageService.remove('authorizationData');
                $state.go('login');
                $location.path('/login').replace();
            }
            else
            {
                $rootScope.autoLogin = 1;
               $state.go('familyPage', { name: familyUrl });
            }
         
        } else {
            localStorageService.remove('authorizationData');
            $state.go('login');
            $location.path('/login').replace();
        }
        
    };

    accountService.authentication = authentication;


    accountService.signUp = function (signup) {
        var deferred = $q.defer();
        localStorageService.remove('authorizationData');
        $http.post(settings.apiBaseUri + 'api/account/signup', signup).success(function (response) {
            localStorageService.set('authorizationData', response);
            deferred.resolve(response);
        }).error(function (err, status) {
            DisplayAlert(err, 'danger');
            deferred.reject(err);
        });

        return deferred.promise;
    }

    accountService.signupafterpromocode = function (signup) {
        var deferred = $q.defer();
        localStorageService.remove('authorizationData');
        $http.post(settings.apiBaseUri + 'api/account/signupafterpromocode', signup).success(function (response) {
            localStorageService.set('authorizationData', response);
            deferred.resolve(response);
        }).error(function (err, status) {
            DisplayAlert(err, 'danger');
            deferred.reject(err);
        });

        return deferred.promise;
    }

    accountService.getDetailedSignUpProgress = function () {
        return $http.get(settings.apiBaseUri + 'api/account/getdetailedsignupprogress');
    }

    accountService.loginWithPin = function (memberId, pin) {
        return $http.put(settings.apiBaseUri + 'api/account/loginwithpin/' + memberId + '/' + pin);
    }

    accountService.retrievePin = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/account/retrievepin/' + memberId);
    }

    accountService.passwordResetRequest = function (emailId) {
        return $http.put(settings.apiBaseUri + 'api/account/passwordresetrequest?emailId=' + emailId);
    }

    accountService.resetPassword = function (token, password) {
        return $http.put(settings.apiBaseUri + 'api/account/resetpassword/' + token + '/' + password);
    }

    accountService.ValidatePromoCode = function (promocode) {
        return $http.get(settings.apiBaseUri + 'api/account/validatepromocode/' + promocode);
    }

    accountService.getMobileResponse = function (OneValue, TwoValue) {
        return $http.get(settings.apiBaseUri + 'api/account/getmobileresponse/' + OneValue + '/' + TwoValue);
    }
    accountService.getCoreProResponse = function () {
        return $http.get(settings.apiBaseUri + 'api/account/getcoreproresponse');
    }

    accountService.getCoreProResponse = function () {
        return $http.get(settings.apiBaseUri + 'api/account/getcoreproresponse');
    }

    return accountService;

}]);