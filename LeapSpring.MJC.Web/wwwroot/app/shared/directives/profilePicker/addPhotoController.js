mjcApp.controller('addPhotoController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {

    $scope.closeModal = function () {
        $uibModalInstance.dismiss('cancel');
    };

}]);
