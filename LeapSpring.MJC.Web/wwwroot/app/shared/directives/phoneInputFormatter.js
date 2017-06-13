//** Phone number formatting starts
mjcApp.directive('phoneInputFormatter', ['$filter', '$browser', function ($filter, $browser) {
    return {
        require: 'ngModel',
        link: function ($scope, $element, $attrs, ngModelCtrl) {
            var cursorPosition = 0;
            var isDeleteKey = false;
            var hasCountryCode = false;

            var listener = function () {

                var value = $element.val().replace(/[^0-9]/g, '');
                var isEntered = (value.length >= 10);
                cursorPosition = $element[0].selectionStart;

                if (isEntered) {
                    hasCountryCode = ($element.val()[0] === '+' || $element.val()[0] === '1' || $element.val().substring(0, 2) === '91');
                    cursorPosition = $element[0].selectionStart;
                    if (hasCountryCode && (cursorPosition === 3 || cursorPosition === 7 || cursorPosition === 11 || cursorPosition === 14 || cursorPosition === 16)) {
                        cursorPosition += (cursorPosition === 14 || cursorPosition === 16) ? 2 : 1;
                    } else if (cursorPosition === 4 || cursorPosition === 8) {
                        cursorPosition += 1;
                    }
                }

                if (!isDeleteKey)
                    $element.val($filter('phoneNumber')(value, false));

                if (isEntered) {
                    $element[0].setSelectionRange(cursorPosition, cursorPosition);
                }
            };

            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function (viewValue) {
                //var maxLength = ($element.val().substring(0, 2) == '+1') ? 11 : ($element.val().substring(0, 3) == '+91') ? 12 : 10;
                var maxLength = ($element.val().substring(0, 1) == '1') ? 11 : ($element.val().substring(0, 2) == '91') ? 12 : 10;
                return viewValue.replace(/[^0-9]/g, '').slice(0, maxLength);
            });

            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function () {
                if (!isDeleteKey)
                    $element.val($filter('phoneNumber')(ngModelCtrl.$viewValue, false));
            };

            $element.bind('change', listener);
            $element.bind('keyup', function (event) {
                var key = event.keyCode;
                isDeleteKey = (key === 8);

                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, also alphabets do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40)) {
                    return;
                }

                //// Fillters the values when delete is clicked
                //var value = $element.val().replace(/[^0-9]/g, '');
                //$element.val($filter('phoneNumber')(value, false));

                $browser.defer(listener); // Have to do this or changes don't get picked up properly
            });

            $element.bind('paste cut', function () {
                $browser.defer(listener);
            });
        }

    };
}]);



//** Phone number formatting end