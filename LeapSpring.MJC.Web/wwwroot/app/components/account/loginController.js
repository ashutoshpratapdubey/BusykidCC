mjcApp.controller('loginController', ['$scope', '$state', '$location', '$remember', 'localStorageService', 'accountService', 'familyMemberService', 'subscriptionService', 'Idle', '$rootScope', function ($scope, $state, $location, $remember, localStorageService, accountService, familyMemberService, subscriptionService, Idle, $rootScope) {

    $scope.loginCredential = {};
    $scope.isLogging = false;
    $scope.btnLoginTxt = 'Sign In';
    $scope.inputType = 'password';
    $location.replace();
    $scope.remember = true;


    $scope.encryptData = function () {

        $scope.source_string = $scope.loginCredential.Password;

        var encrypted = CryptoJS.AES.encrypt(
                  $scope.source_string,
                  $rootScope.base64Key,
                  { iv: $rootScope.iv });
        //  console.log('encrypted = ' + encrypted);


        $scope.ciphertext = encrypted.ciphertext.toString(CryptoJS.enc.Base64);

        var cipherParams = CryptoJS.lib.CipherParams.create({
            ciphertext: CryptoJS.enc.Base64.parse($scope.ciphertext)
        });

        var decrypted = CryptoJS.AES.decrypt(
                      cipherParams,
                      $rootScope.base64Key,
                      { iv: $rootScope.iv });
        $scope.descrString = decrypted.toString(CryptoJS.enc.Utf8);
    }

    $scope.showPassword = function () {
        if ($scope.loginCredential != undefined && $scope.loginCredential.Password != undefined) {
            if ($scope.inputType == 'password')
                $scope.inputType = 'text';
            else
                $scope.inputType = 'password';

            $('#txtPassword').focus();
        }
    }


    $scope.rememberMe = function () {

        if ($scope.remember) {
            $remember('username', $scope.loginCredential.Email);
            $remember('password', $scope.ciphertext);
        } else {
            $remember('username', '');
            $remember('password', '');
        }
    };


    $scope.login = function () {

        $scope.$broadcast('show-errors-check-validity');

        if ($scope.loginForm.$valid) {
            $scope.isLogging = true;
            $scope.btnLoginTxt = 'Signing in...';

            // Checking if cookie is disabled
            if (!$scope.cookie()) {
                DisplayAlert("Cookies is diabled in your browser", 'danger');
            }

            accountService.login($scope.loginCredential).then(function (response) {
                
                var authData = response;
                $scope.encryptData();
                $scope.rememberMe();
                //  $scope.savecookies();
                familyMemberService.getsignUpProgress().success(function (response) {
                    
                    // start watching when the app runs. also starts the Keepalive service by default.
                    Idle.watch();
                    if (response === 'Completed') {
                        $rootScope.autoLogin = 0;
                        // Checks for subscription
                        subscriptionService.getFamilySubscription().success(function (response) {
                            if (response != null && response.Status == 'Active') {
                                if (localStorageService.get('isEmailSubscription')) {
                                    localStorageService.remove('isEmailSubscription');
                                    $state.go('myAccount', { subscribe: 'emailSubscription' });
                                } else {
                                    $state.go('adminDashboard');
                                }
                                return;
                            }
                            authData.MemberType === 'Admin' ? $state.go('subscription') : $state.go('signupprogress');
                        }).error(function (err, data) {
                            DisplayAlert(err, 'danger');
                        });
                    }
                    else
                        $state.go('signupprogress');
                }).error(function (err, data) {
                    $scope.isLogging = false;
                    $scope.btnLoginTxt = 'Sign In';
                    DisplayAlert(err, 'danger');
                });
            }, function (err) {
                $scope.isLogging = false;
                $scope.btnLoginTxt = 'Sign In';
                DisplayAlert(err, 'danger');
            });
        }
    };

    if ($remember('username') && $remember('password')) {
        $scope.loginCredential = {
            Email: $remember('username'),
            Password: $scope.descrString
        };
        $scope.remember = ($scope.loginCredential.Email != undefined && $scope.loginCredential.Password != undefined);
    }

    $scope.cookie = function () {
        var cookieEnable = navigator.cookieEnabled;

        if (typeof navigator.cookieEnabled === 'undefined' && !cookieEnable) {
            document.cookie = 'cookie-test';
            cookieEnable = (document.cookie.indexOf('cookie-test') !== -1);
        }

        return cookieEnable;
    }



}]);
