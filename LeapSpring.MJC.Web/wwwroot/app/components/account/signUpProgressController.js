mjcApp.controller('signUpProgressController', ['$scope', '$state', '$stateParams', 'localStorageService', 'accountService', 'familyMemberService', function ($scope, $state, $stateParams, localStorageService, accountService, familyMemberService) {

    $scope.title = 'You’re Almost There!';
    $scope.isLoading = true;
    $scope.signUpProgress = {};
    $scope.nextStateName = 'addChild';
    // This property eliminate jerk from navigate connection success page to admin dashboard directly.
    // If this property false, then load signup progress.
    $scope.isSignUpCompleted = true;
    $scope.bankLinkText = "Linked a Bank Account";
    $scope.linkBankStateName = 'linkbankinfo';
    $scope.isCompletedBankProcess = false;

    var authData = localStorageService.get('authorizationData');

    accountService.getDetailedSignUpProgress().success(function (response) {
        if (response.IsAccountCreated && response.IsAddedChild && response.IsLinkedToBank && response.BankStatus === 'Verified') {
            if (authData.MemberType === 'Child')
                $state.go('childDashboard');
            else
                $state.go('adminDashboard');
        } else if (authData.MemberType != 'Admin')
            $state.go('incompleteSignUp');

        $scope.prepareBankLink(response);
        $scope.isSignUpCompleted = false;
        $scope.signUpProgress = response;
        $scope.isLoading = false;
    }).error(function (err, data) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.prepareBankLink = function (signUpProgress) {
        switch (signUpProgress.BankStatus) {
            case null:
            case '':
            case 'NotLinked':
                $scope.bankLinkText = "Link My Bank Account";
                $scope.linkBankStateName = 'linkbankinfo';
                break;
            case 'Unverified':
                $scope.bankLinkText = "Pending bank verification";
                $scope.linkBankStateName = 'verifymicrodeposit';
                break;
            case 'Expired':
                $scope.bankLinkText = "Verify Micro Deposits";
                $scope.linkBankStateName = 'verifymicrodeposit';
                break;
            case 'Verified':
                $scope.bankLinkText = "Linked a Bank Account";
                $scope.isCompletedBankProcess = true;
                $scope.linkBankStateName = '';
                break;
        }
    }

    $scope.completeSignup = function () {
        if (!$scope.signUpProgress.HasPin)
            $state.go('createPin');

        if ($scope.signUpProgress.HasPin && !$scope.signUpProgress.HasPhoneNumber)
            $state.go('phoneEntry');
    }

    $scope.gotoAddChild = function () {
        if ($scope.signUpProgress.HasPin && $scope.signUpProgress.HasPhoneNumber)
            $state.go('addChild');
    }

    $scope.gotoAssignedChores = function () {
        if ($scope.signUpProgress.IsAddedChild)
            $state.go('suggestedChore', { id: $scope.signUpProgress.LastChildId });
    }

    $scope.gotoLinkBank = function () {
        if ($scope.signUpProgress.IsAssignedSomeChores && !$scope.isCompletedBankProcess && $scope.linkBankStateName != '')
            $state.go($scope.linkBankStateName);
    }

}]);