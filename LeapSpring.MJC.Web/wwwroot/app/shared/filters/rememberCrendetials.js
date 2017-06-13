mjcApp.factory('$remember', [function () {
    function fetchValue(name) {
        var cookieValues = document.cookie.split("; ");
        for (var i = 0; i < cookieValues.length; i++) {
            // a name/value pair (a crumb) is separated by an equal sign
            var gCrumb = cookieValues[i].split("=");
            if (name === gCrumb[0]) {
                var value = '';
                try {
                    value = angular.fromJson(gCrumb[1]);
                } catch (e) {
                    value = unescape(gCrumb[1]);
                }
                return value;
            }
        }
        // a cookie with the requested name does not exist
        return null;
    }
    return function (name, values) {
        if (arguments.length === 1) return fetchValue(name);
        var cookie = name + '=';
        if (typeof values === 'object') {
            var expires = '';
            cookie += (typeof values.value === 'object') ? angular.toJson(values.value) + ';' : values.value + ';';
            if (values.expires) {
                var date = new Date();
                date.setTime(date.getTime() + (values.expires * 24 * 60 * 60 * 1000));
                expires = date.toGMTString();
            }
            cookie += (!values.session) ? 'expires=' + expires + ';' : '';
            cookie += (values.path) ? 'path=' + values.path + ';' : '';
            cookie += (values.secure) ? 'secure;' : '';
        } else {
            var date = new Date();
            date.setTime(date.getTime() + (90 * 24 * 60 * 60 * 1000));
            expires = date.toGMTString();

            cookie += values + ';';
            cookie += 'expires=' + expires + ';';
        }


        document.cookie = cookie;
    }
}]);

mjcApp.factory('GoogleAdWordsService', function ($window) {

    // Conversion labels 
    ////var google_conversion_label = {
    ////    'busykid_tracker': "M_bQCMrsxW4QrfTNmwM"
    ////};

    // Basic settings for AdWords Conversion
    var googleTrackConversion = function (conversion_label) {
        $window.google_trackConversion({
            google_conversion_id: 863205933,
            google_conversion_language: "en",
            google_conversion_format: "3",
            google_conversion_color: "ffffff",
            google_conversion_label: "M_bQCMrsxW4QrfTNmwM",
            google_remarketing_only: false
        });
    };

    return {
        checkgoogleAdscript: function () {
            // Trigger register-customer conversion 
            googleTrackConversion();
        }
    };
});


mjcApp.factory('$rememberPromoCode', [function () {
    function fetchValue(name) {
        var cookieValues = document.cookie.split("; ");
        for (var i = 0; i < cookieValues.length; i++) {
            // a name/value pair (a crumb) is separated by an equal sign
            var gCrumb = cookieValues[i].split("=");
            if (name === gCrumb[0]) {
                var value = '';
                try {
                    value = angular.fromJson(gCrumb[1]);
                } catch (e) {
                    value = unescape(gCrumb[1]);
                }
                return value;
            }
        }
        // a cookie with the requested name does not exist
        return null;
    }
    return function (name, values) {
        if (arguments.length === 1) return fetchValue(name);
        var cookie = name + '=';
        if (typeof values === 'object') {
            var expires = '';
            cookie += (typeof values.value === 'object') ? angular.toJson(values.value) + ';' : values.value + ';';
            if (values.expires) {
                var date = new Date();
                date.setTime(date.getTime() + (values.expires * 24 * 60 * 60 * 1000));
                expires = date.toGMTString();
            }
            cookie += (!values.session) ? 'expires=' + expires + ';' : '';
            cookie += (values.path) ? 'path=' + values.path + ';' : '';
            cookie += (values.secure) ? 'secure;' : '';
        } else {
            var date = new Date();
            date.setTime(date.getTime() + (30 * 24 * 60 * 60 * 1000));
            expires = date.toGMTString();

            cookie += values + ';';
            cookie += 'expires=' + expires + ';';
        }


        document.cookie = cookie;
    }
}]);
