mjcApp.filter('searchStock', function () {
    return function (arr, searchString) {
        if (!searchString) {
            return arr;
        }
        var result = [];
        searchString = searchString.toLowerCase();
        angular.forEach(arr, function (item) {
            if (item.CompanyPopularName.toLowerCase().indexOf(searchString) !== -1 || item.StockSymbol.toLowerCase().indexOf(searchString) !== -1
                || (item.BrandName !== undefined && item.BrandName !== null && item.BrandName.toLowerCase().indexOf(searchString) !== -1)) {
                result.push(item);
            }
        });
        return result;
    };
});