mjcApp.filter('dateInput', function () {
    return function (dateInput) {
        if (!dateInput) { return ''; }

        dateInput = String(dateInput);

        var formattedDate = dateInput;

        var month = dateInput.substring(0, 2);
        var date = dateInput.substring(2, 4);
        var year = dateInput.substring(4, 8);

        if (month) {
            var monthInDigit = parseInt(month);
            if (monthInDigit > 12)
                month = "01";
            if (formattedDate.length == 2)
                formattedDate = month;

        }
        if (date) {
            var dateInDigit = parseInt(date);
            if (dateInDigit > 31)
                date = "01";
        }

        if (date)
            formattedDate = (month + "/" + date);

        if (year)
            formattedDate += ("/" + year);

        if (!year && (formattedDate.length == 2 || formattedDate.length == 5))
            formattedDate += "/";

        return formattedDate;
    };
});

mjcApp.filter('fullYearDateInput', function () {
    return function (dateInput) {
        if (!dateInput) { return ''; }

        dateInput = String(dateInput);

        var formattedDate = dateInput;

        var month = dateInput.substring(0, 2);
        var date = dateInput.substring(2, 4);
        var year = dateInput.substring(4, 8);

        if (month) {
            var monthInDigit = parseInt(month);
            if (monthInDigit > 12)
                month = "01";
            if (formattedDate.length == 2)
                formattedDate = month;

        }
        if (date) {
            var dateInDigit = parseInt(date);
            if (dateInDigit > 31)
                date = "01";
        }

        if (date)
            formattedDate = (month + "/" + date);

        if (year) {
            var todayDate = new Date();
            var currentYear = todayDate.getFullYear();

            var fullYear = parseInt(currentYear.toString().substring(0, 2) + year);
            if (fullYear > currentYear || year.length != 2)
                formattedDate += ("/" + year);
            else
                formattedDate += ("/" + fullYear);
        }

        if (!year && (formattedDate.length == 2 || formattedDate.length == 5))
            formattedDate += "/";

        return formattedDate;
    };
});