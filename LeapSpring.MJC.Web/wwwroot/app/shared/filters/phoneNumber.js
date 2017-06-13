mjcApp.filter('phoneNumber', function () {
    return function (number) {
        /* 
        @param {Number | String} number - Number that will be formatted as telephone number
        Returns formatted number: (###) ###-####
            if number.length < 4: ###
            else if number.length < 7: (###) ###

        Does not handle country codes that are not '1' (USA)
        */
        if (!number) { return ''; }

        number = String(number);

        // Will return formattedNumber. 
        // If phonenumber isn't longer than an area code, just show number
        var formattedNumber = number;

        // if the first character is '1', strip it out and add it back
        var c = (number[0] == '1') ? '+1-' : ((number.substring(0, 2) == '91') && number.substring(2).length >= 10) ? '+91-' : '';
        number = number[0] == '1' ? number.slice(1) : ((number.substring(0, 2) == '91') && number.substring(2).length >= 10) ? number.slice(2) : number;


        // # (###) ###-#### as c (area) front-end
        // #-###-###-#### as c-area-front-end
        var area = number.substring(0, 3);
        var front = number.substring(3, 6);
        var end = ((number.substring(0, 2) == '91') && number.substring(2).length < 10) ? number.substring(6) : number.substring(6, 10);

        if (front) {
            // With Country Code
            formattedNumber = (c + area + "-" + front);

            //// Without Country Code
            //formattedNumber = (area + "-" + front);
        }
        if (end) {
            formattedNumber += ("-" + end);
        }
        return formattedNumber;
    };
});