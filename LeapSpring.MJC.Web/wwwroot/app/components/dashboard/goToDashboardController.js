mjcApp.controller('goToDashboardController', ['$scope', '$state', '$filter', 'localStorageService', '$rootScope', 'goToDashboardService', 'subscriptionService', 'familyMemberService', function ($scope, $state, $filter, localStorageService, $rootScope, goToDashboardService, subscriptionService, familyMemberService) {
    $scope.goToDashboard = function ()
    {
          var authData = localStorageService.get('authorizationData');
        if (authData != null) {
            $scope.memberType = authData.MemberType;
            $scope.FamilyMemberId = authData.FamilyMemberId;
            $scope.FamilyUrl = authData.FamilyUrl;
        }
       
         goToDashboardService.LoginforDashboard($scope.FamilyMemberId, $scope.memberType, $scope.FamilyUrl).success(function (response) {
            // start watching when the app runs. also starts the Keepalive service by default.
            //Idle.watch();

            localStorageService.remove('authorizationData');
            localStorageService.set('authorizationData', response);


            var memberType = response.MemberType;
            familyMemberService.getsignUpProgress().success(function (response) {
                if (response === 'Completed') {
                    // Checks for subscription
                    subscriptionService.getFamilySubscription().success(function (response) {
                        if (response != null && response.Status == 'Active') {
                            if (memberType !== 'Child')
                                $state.go('adminDashboard');
                            else
                                $state.go('childDashboard');
                            return;
                        }
                        memberType === 'Admin' ? $state.go('subscription') : $state.go('signupprogress');
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
        }).error(function (err) {
            $scope.isContinue = false;
            $scope.btnContinue = 'Continue';
            DisplayAlert(err, 'danger');
        });





    };

    $scope.goToDashboard();
   
}]);