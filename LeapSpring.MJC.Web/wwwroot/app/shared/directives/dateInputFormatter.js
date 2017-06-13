//** Date formatting starts

mjcApp.directive('dateInputFormatter', ['$filter', '$browser', function ($filter, $browser) {
    return {
        require: 'ngModel',
        link: function ($scope, $element, $attrs, ngModelCtrl) {
            var cursorPosition = 0;
            var isDeleteKey = false;

            var listener = function () {

                var dateInuput = $element.val().split('/');
                var month = dateInuput[0];
                var date = dateInuput[1];
                var year = dateInuput[2];

                var value = $element.val().replace(/[^0-9]/g, '');
                var isEntered = (value.length >= 6) ||(value.length >= 8);
                cursorPosition = $element[0].selectionStart;

                if (isEntered && !isDeleteKey) {
                    cursorPosition = $element[0].selectionStart;
                    if (cursorPosition === 3 || cursorPosition === 8)
                        cursorPosition += 1;
                }

                if (!isDeleteKey) {
                    if (month && month.length == 2 && year && (year.length == 2 || year.length == 4) && date && date.length == 1) {
                        isEntered = true;
                        if (cursorPosition === 3 || cursorPosition === 8)
                            cursorPosition += 1;
                    }
                        
                    if (date && date.length == 2 && year && (year.length == 2 || year.length == 4) && month && month.length == 1) {
                        isEntered = true;
                        if (cursorPosition === 3 || cursorPosition === 8)
                            cursorPosition += 1;
                    }
                       
                    $element.val($filter('dateInput')(value, false));
                }

                if (isEntered) {
                    $element[0].setSelectionRange(cursorPosition, cursorPosition);
                }
            };

            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function (viewValue) {
                return viewValue.replace(/[^0-9]/g, '').substring(0, 8);
            });

            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function () {
                if (!isDeleteKey)
                    $element.val($filter('dateInput')(ngModelCtrl.$viewValue, false));
            };

            $element.bind('change', listener);
            $element.bind('keyup', function (event) {
                var key = event.keyCode;
                isDeleteKey = (key == 8 || key == 46);
                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, also alphabets do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40)) {
                    return;
                }
                $browser.defer(listener); // Have to do this or changes don't get picked up properly
            });

            $element.bind('paste cut', function () {
                $browser.defer(listener);
            });
        }

    };
}]);

//** Date formatting end