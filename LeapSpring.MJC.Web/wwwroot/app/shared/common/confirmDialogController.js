mjcApp.controller('confirmDialogController', function ($scope, type, yesText, noText, message, close, listMessage) {
    $scope.isAlert = type == 'alert';
    $scope.isInfo = type == 'info';
    $scope.yesText = yesText;
    $scope.noText = noText;
    $scope.message = message;
    $scope.listMessage = listMessage;
    $scope.close = function (result) {
        close(result, 500); // close, but give 500ms for bootstrap to animate
        $('.modal-backdrop').remove();
    };
});