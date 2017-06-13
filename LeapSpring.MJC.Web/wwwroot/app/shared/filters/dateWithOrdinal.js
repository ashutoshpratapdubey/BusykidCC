mjcApp.filter('dateWithOrdinal', function ($filter) {
    var suffixes = ["th", "st", "nd", "rd"];
    return function (input, format) {
        if (!input)
            return;
        var date = $filter('date')(new Date(input), format);

        var day = parseInt($filter('date')(input, 'dd'));
        var relevantDigits = (day < 30) ? day % 20 : day % 30;
        var suffix = (relevantDigits <= 3) ? suffixes[relevantDigits] : suffixes[0];
        if (day < 10) {
            date = date.replace('0' + day, day);
        }
        return date.replace('oo', suffix);
    };
});