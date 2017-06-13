mjcApp.filter('indexOf', function () {
    return function (array, itemToFind) {

        var index = !itemToFind ? -1 : 0;
        if (!itemToFind)
            return index;

        angular.forEach(array, function (item) {
            if (item.Id === itemToFind.Id) {
                return index;
            }

            index++;
        });

        return index >= 0 ? index : -1;
    };
});