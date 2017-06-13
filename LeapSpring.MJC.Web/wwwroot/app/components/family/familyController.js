mjcApp.controller('familyController', ['$scope', '$state', '$stateParams', '$filter', 'familyMemberService', 'accountService', function ($scope, $state, $stateParams, $filter, familyMemberService, accountService) {
    $scope.isLoading = true;
    $scope.Members = [];
    $scope.defaultProfileImage = '../../../images/Avatar.png';
    $scope.btnContinue = 'Add Member';

    familyMemberService.getfamilybyname($stateParams.name).success(function (response) {
        angular.forEach(response, function (member) {
            if (member.ProfileImageUrl === undefined || member.ProfileImageUrl === null || member.ProfileImageUrl === '')
                member.ProfileImageUrl = $scope.defaultProfileImage;
            member.btnForgorPasswordContent = 'Forgot Pin';
            $scope.Members.push(member);
        });
        $scope.isLoading = false;
    }).error(function (err, data) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.switchAccount = function (memberId, childname) {
        $state.go('switchPin', { id: memberId, name: childname });
    }

    $scope.retrievePin = function (member) {
        member.isRetrievingPin = true;
        member.btnForgorPasswordContent = 'Sending PIN....';
        accountService.retrievePin(member.Id).success(function (response) {
            DisplayAlert('Your pin has been sent to your mobile!', 'success');
            member.btnForgorPasswordContent = 'Forgot Pin';
            var foundedMember = $filter('filter')($scope.Members, { Id: member.Id }, true)[0];
            foundedMember.isRetrievingPin = false;

        }).error(function (err) {
            member.btnForgorPasswordContent = 'Forgot Pin';
            var foundedMember = $filter('filter')($scope.Members, { Id: member.Id }, true)[0];
            foundedMember.isRetrievingPin = false;
            DisplayAlert(err, 'danger');
        });
    };

}]);