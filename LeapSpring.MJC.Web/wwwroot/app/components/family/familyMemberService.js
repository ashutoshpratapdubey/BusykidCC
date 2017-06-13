
mjcApp.factory('familyMemberService', ['$http', 'settings', function ($http, settings) {

    var familyMemberService = {};

    familyMemberService.Add = function (familyMember) {
        return $http.post(settings.apiBaseUri + 'api/family/add', familyMember);
    }

    familyMemberService.Update = function (familyMember) {
        return $http.put(settings.apiBaseUri + 'api/family/update', familyMember);
    }

    familyMemberService.UpdateChild = function (familyMember) {
        return $http.put(settings.apiBaseUri + 'api/family/updatechild', familyMember);
    }

    familyMemberService.UpdateChildName = function (firstName, memberId) {
        return $http.put(settings.apiBaseUri + 'api/family/updatechildname?firstname=' + firstName + '&memberId=' + memberId);
    }


    familyMemberService.UpdateMemberPin = function (pin, memberId) {
        return $http.put(settings.apiBaseUri + 'api/family/updatememberpin?pin=' + pin + '&memberId=' + memberId);
    }

    familyMemberService.UpdateChildPin = function (pin, memberId) {
        return $http.put(settings.apiBaseUri + 'api/family/updatechildpin?pin=' + pin + '&memberId=' + memberId);
    }

    familyMemberService.UpdatePhoneNumber = function (phonenumber) {
        return $http.put(settings.apiBaseUri + 'api/family/updateadminphone?phonenumber=' + phonenumber);
    }

    familyMemberService.UpdateChildPhone = function (phonenumber, memberId, isparent) {
        return $http.put(settings.apiBaseUri + 'api/family/updatechildphone?phonenumber=' + phonenumber + '&memberId=' + memberId + '&isparent=' + isparent);
    }
   

    familyMemberService.Delete = function (memberId) {
        return $http.delete(settings.apiBaseUri + 'api/family/delete/' + memberId);
    }

    familyMemberService.GetCurrentMember = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getmember');
    }
    familyMemberService.GetChildMember = function (memberId) {
        return $http.get(settings.apiBaseUri + 'api/family/getchildmember?memberId=' + memberId);
        //return $http.get(settings.apiBaseUri + 'api/family/getchildmember' + memberId);
    }

    familyMemberService.GetById = function (familyMemberId) {
        return $http.get(settings.apiBaseUri + 'api/family/getbyid/' + familyMemberId);
    }

    familyMemberService.GetChildrens = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getchildrens');
    }

    familyMemberService.GetChildCount = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getchildcount');
    }

    familyMemberService.getsignUpProgress = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getsignupprogress');
    }

    familyMemberService.getfamilybyname = function (familyName) {
        return $http.get(settings.apiBaseUri + 'api/family/getfamilybyname?familyName=' + familyName);
    }

    familyMemberService.updateMemberInfo = function (dateOfBirth, address, city, stateId, ssn) {
        address = encodeURIComponent(address);
        city = encodeURIComponent(city);
        return $http.put(settings.apiBaseUri + 'api/family/updatememberinfo?dateOfBirth=' + dateOfBirth + '&address=' + address + '&city=' + city + '&stateId=' + stateId + '&ssn=' + ssn);
    }

    familyMemberService.getStates = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getstates');
    }

    familyMemberService.GetFamilyById = function (familyId) {
        return $http.get(settings.apiBaseUri + 'api/family/getfamilybyid/' + familyId);
    }

    familyMemberService.getAllMembers = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getallmembers');
    };

    familyMemberService.toggleEmailSubscription = function () {
        return $http.put(settings.apiBaseUri + 'api/family/toggleemailsubscription');
    };

    familyMemberService.hasTrial = function () {
        return $http.get(settings.apiBaseUri + 'api/family/hastrial');
    };

    familyMemberService.markTrialAsUsed = function () {
        return $http.put(settings.apiBaseUri + 'api/family/marktrialasused');
    };

    // Upload photo
    familyMemberService.uploadPhoto = function (profileImage) {
        return $http.put(settings.apiBaseUri + 'api/family/uploadimage/', profileImage);
    };

    familyMemberService.GetCreditCard = function () {
        return $http.get(settings.apiBaseUri + 'api/family/getcreditcard');
    }

    return familyMemberService;
}]);