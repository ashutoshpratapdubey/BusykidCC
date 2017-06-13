//mjcApp.directive("moveToNext", ['$window', function ($window) {
//    return {
//        restrict: "A",
//        link: function (scope, element, attr, form) {
//           
//            var tabindex = parseInt(attr.tabindex);
//            var maxLength = parseInt(attr.maxlength);
//            //element.on('input, keyup', function (e) {

//            //    if (element.val().length > maxLength - 1) {
//            //        var next = angular.element(document.body).find('[tabindex=' + (tabindex + 1) + ']');
//            //        if (next.length > 0) {
//            //            next.focus();
//            //            next.select();
//            //            if (!$window.getSelection().toString() && next.val()) {
//            //                //Required for mobile Safari
//            //                next.setSelectionRange(0, next.val().length)
//            //                this.setSelectionRange(0, this.value.length)
//            //            }
//            //        }
//            //        else {
//            //            return false;
//            //        }
//            //    }
          
//                //var inputLength = (element.val().length === 0) ? element.val().length : (element.val().length - 1);
//                //if (element.val().length === 0)
//                //    return false;
//                //if (inputLength === maxLength - 1) {
//                //    if (tabindex === 4)
//                //        return true;

//                //    var next = angular.element(document.body).find('[tabindex=' + (tabindex + 1) + ']');
//                //    if (next.length > 0) {
//                //        next.focus();
//                //        next.select();
//                //        if (!$window.getSelection().toString() && next.val()) {
//                //            //Required for mobile Safari
//                //            next.setSelectionRange(0, next.val().length)
//                //            this.setSelectionRange(0, this.value.length)
//                //        }
//                //        return true;
//                //    }
//                //    else {
//                //        return false;
//                //    }
//                //}
//                //return true;
//            });

//            //element.on('keypress', function () {
//            //    element.select();
//            //    if (!$window.getSelection().toString()) {
//            //        //Required for mobile Safari
//            //        this.setSelectionRange(0, this.value.length)
//            //    }
//            //});

//            //element.on('click', function () {
//            //    element.select();
//            //    if (!$window.getSelection().toString()) {
//            //        // Required for mobile Safari
//            //        this.setSelectionRange(0, this.value.length)
//            //    }
//            //});
//        }
//    }
//}]);



mjcApp.directive("moveToNext", ['$window', function ($window) {
    return {
        restrict: "A",
        link: function (scope, element, attr, form) {
           
            var tabindex = parseInt(attr.tabindex);
            var maxLength = parseInt(attr.maxlength);
          
            element.on("input", function (e) {               
                if (element.val().length == element.attr("maxlength")) {
                    var next = angular.element(document.body).find('[tabindex=' + (tabindex + 1) + ']');
                    if (next.length > 0) {
                        next.focus();
                        next.select();
                        if (!$window.getSelection().toString() && next.val()) {
                            //Required for mobile Safari
                            next.setSelectionRange(0, next.val().length)
                            this.setSelectionRange(0, this.value.length)
                        }
                    }
                }
              
            });
        
        }
    }
}]);
