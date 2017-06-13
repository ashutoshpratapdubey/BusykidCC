mjcApp
    .directive('digitsOnly', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var re = RegExp(attrs.digitsOnly);
                var exclude = /Backspace|Enter|Tab|Delete|Del|ArrowUp|Up|ArrowDown|Down|ArrowLeft|Left|ArrowRight|Right/;

                element[0].addEventListener('keydown', function (event) {
                    if (!exclude.test(event.key) && !re.test(event.key)) {
                        event.preventDefault();
                    }
                });
            }
        }
    })

    // Only Number Restriction Directive ends here
.controller('myAccountController', ['$rootScope', '$scope', '$state', '$stateParams', '$timeout', '$q', '$uibModal', 'localStorageService', 'Upload', 'familyMemberService', 'bankAuthorizeService', 'subscriptionService', 'dashboardService', 'ModalService', 'NonplaidbankAuthorizeService', function ($rootScope, $scope, $state, $stateParams, $timeout, $q, $uibModal, localStorageService, Upload, familyMemberService, bankAuthorizeService, subscriptionService, dashboardService, ModalService, NonplaidbankAuthorizeService) {

    $scope.isLoading = true;
    $scope.isUpdating = false;
    $scope.familyMember = {};
    $scope.isEditPassword = false;
    $scope.isEditPhone = false;
    $scope.isEditEmail = false;


    $scope.saveBtnTxt = 'Save Settings';
    $scope.bankLinkText = 'Link Bank Account';
    $scope.removeBankText = 'Remove Connected Bank';
    $scope.bankDetails = {};
    $scope.defaultProfileImage = '../../../images/Avatar.png';
    $scope.isSubscribed = true;
    $scope.isCancelling = false;
    $scope.familyMemberCopy = {};
    $scope.canShowAllErr = false;
    $scope.isRemovingBank = false;
    $scope.isSubscribing = false;
    $scope.verifyDepositAmountsNonPlaid = {};
    $scope.IsPlaidAccount = false;



    $scope.emailSubscriptionContent = 'Unsubscribe';

    var authData = localStorageService.get('authorizationData');
    if (authData != null) {
        $scope.memberType = authData.MemberType;
        $scope.familyId = authData.FamilyId;
    }
    var getCurrentMemberReq = familyMemberService.GetCurrentMember();
    var getFamilySubscription = subscriptionService.getFamilySubscription();
    var getLinkedBankReq = NonplaidbankAuthorizeService.GetLinkedBankStatus();
    var getFamilyByName = familyMemberService.GetFamilyById($scope.familyId);
    var getcreditcarddetail = bankAuthorizeService.GetCreditCard();


    $q.all([getCurrentMemberReq, getLinkedBankReq, getFamilySubscription, getFamilyByName, getcreditcarddetail]).then(function (response) {

        $scope.familyMember = response[0].data;
        $scope.familyMemberCopy = angular.copy($scope.familyMember);
        $scope.family = response[3].data;
        $scope.familySubscription = response[2].data;
        $scope.getLinkedBankReq = response[1].data;
        $scope.creditinformation = response[4].data;

        if ($scope.creditinformation.Tokenid != "" && $scope.creditinformation.Tokenid != null && $scope.creditinformation.CardStatus == "Verified") {
            $scope.CreditCard = 1;
        }
        else {
            $scope.CreditCard = 0;
        }
        //if (getbankinfo.AccountId != null || getbankinfo.AccountId != undefined) {
        //    $scope.CreditCard = 0;
        //}
        //else {
        //    $scope.CreditCard = 1;
        //}

        $scope.familyMember.isUploading = false;
        if ($scope.memberType == 'Admin') {
            $scope.familySubscription = response[2].data;
            $scope.isSubscribed = ($scope.familySubscription != undefined && $scope.familySubscription != null && $scope.familySubscription.Status == 'Active');

            $scope.bankDetails = response[1].data;
            $scope.isLinkedBank = $scope.bankDetails.IsLinkedBank;
            $scope.isShowVerifiedBank = $scope.bankDetails.BankStatus == 'Unverified';
            if ($scope.isLinkedBank)
                $scope.prepareBankAccountStatus($scope.bankDetails);

            $scope.emailSubscriptionContent = $scope.familyMember.IsUnSubscribed ? 'Subscribe' : 'Unsubscribe';

            if ($scope.isLinkedBank == "1") {
                IsPlaidAccount = true;
            }
            else {
                IsPlaidAccount = false;
            }


        }

        if ($scope.familyMember != null && ($scope.familyMember.ProfileImageUrl == undefined || $scope.familyMember.ProfileImageUrl == null || $scope.familyMember.ProfileImageUrl == ''))
            $scope.familyMember.ProfileImageUrl = $scope.defaultProfileImage;

        if ($scope.isLinkedBank)
            $scope.bankLinkText = 'Connect a different bank';

        $scope.isLoading = false;
    }, function (err) {
        $scope.isLoading = false;
        DisplayAlert(err, 'danger');
    });

    $scope.prepareBankAccountStatus = function (bankDetails) {
        if (bankDetails.Status == 'Unverified' || bankDetails.Status == 'VerifyLocked' || bankDetails.Status == 'Expired' || bankDetails.Status == 'Denied')
            $scope.bankDetails.AccountStatus = 'Pending Verification';
        else if (bankDetails.Status == 'Verified')
            $scope.bankDetails.AccountStatus = 'Verified';
    }

    $scope.updateMember = function () {
        $scope.canShowAllErr = true;
        $scope.$broadcast('show-errors-check-validity');
        if (!$scope.familyMemeberForm.$valid) return;

        $scope.isUpdating = true;
        $scope.isEditPassword = false;

        $scope.saveBtnTxt = 'Saving...';
        familyMemberService.Update($scope.familyMember).success(function (response) {
            $scope.familyMemberCopy = angular.copy($scope.familyMember);
            $scope.isUpdating = false;
            $scope.saveBtnTxt = 'Save Settings';
            DisplayAlert('Updated successfully', 'success');
        }).error(function (err, data) {
            $scope.familyMember = angular.copy($scope.familyMemberCopy);
            $scope.saveBtnTxt = 'Save Settings';
            $scope.isUpdating = false;
            DisplayAlert(err, 'danger');
        });
    }
    $scope.editFormInput = function (type) {
        $scope.canShowAllErr = false;
        var inputName = '';
        switch (type) {
            case 'password':
                if ($scope.isEditPassword && !$scope.validateForm() &&
                    ($scope.familyMember.User.Password == null || $scope.familyMember.User.Password == '' || $scope.familyMember.User.Password == undefined)) {
                    inputName = 'memberPassword';
                    break;
                }
                $scope.isEditPassword = !$scope.isEditPassword;
                if ($scope.isEditPassword) {
                    $timeout(function () {
                        $scope.familyMember.User.Password = '';
                    }, 10);
                    inputName = 'memberPassword';
                }
                break;
            case 'phone':
                if ($scope.isEditPhone && !$scope.validateForm() &&
                    ($scope.familyMember.PhoneNumber == null || $scope.familyMember.PhoneNumber == '' || $scope.familyMember.PhoneNumber == undefined)) {
                    inputName = 'memberPhone';
                    break;
                }

                $scope.isEditPhone = !$scope.isEditPhone;
                if ($scope.isEditPhone)
                    inputName = 'memberPhone';
                break;

        }

        if (inputName != '')
            $timeout(function () {
                $('#' + inputName)[0].focus();
            }, 50);
    }

    $scope.validateForm = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.familyMemeberForm.$valid)
            return true;
        else
            return false;
    }

    $scope.removeFundingSource = function () {
        if ($scope.isRemovingBank) return;

        ModalService.showModal({
            templateUrl: '/app/shared/common/confirmdialog.html',
            controller: 'confirmDialogController',
            inputs: {
                type: 'alert',
                yesText: 'Remove',
                noText: 'Cancel',
                message: 'Are you sure to remove the bank account?',
                listMessage: null
            }
        }).then(function (modal) {
            modal.element.modal();
            modal.close.then(function (result) {
                if (result) {
                    $scope.isRemovingBank = true;
                    $scope.removeBankText = 'Removing...';
                    bankAuthorizeService.disconnectAccount().success(function (response) {
                        $scope.bankDetails = response;
                        $scope.bankLinkText = 'Link Bank Account';
                        $scope.isRemovingBank = false;
                        $scope.isLinkedBank = false;
                        DisplayAlert("Removed Connected bank successfully", 'success');
                    }).error(function (err) {
                        $scope.removeBankText = 'Remove Connected Bank';
                        $scope.isRemovingBank = false;
                        DisplayAlert(err, 'success');
                    });
                }
            });
        });
    }

    $scope.gotoSubscription = function () {
        if ($scope.memberType === 'Admin')
            $state.go('subscription');
        else
            DisplayAlert('Your family does not have an active subscription. To continue contact your parent to enroll into subscription.', 'danger');
    };

    $scope.cancelSubscription = function () {

        $scope.modalInstance = $uibModal.open({
            scope: $scope,
            templateUrl: '/app/components/subscription/subscriptionCancellationView.html',
            controller: 'subscriptionCancellationController',
            size: 'sm',
            windowClass: 'box-popup'
        });
    };


    $scope.subscribe = function () {
        var subscription = {
            SubscriptionType: subscriptionService.subscriptionTypes[0].Name,
            PromoCode: ($stateParams.promoCode === undefined || $stateParams.promoCode === null) ? null : $stateParams.promoCode
        };

        subscriptionService.Subscribe(subscription).success(function (response) {
            $scope.gotoConnectionSuccess();
        }).error(function (err, data) {
            DisplayAlert(err, 'danger');
        });
    }

    $scope.updateSubscription = function () {
        $scope.isSubscribed = false;
        $scope.isCancelling = false;
        $scope.isLinkedBank = false;
        $scope.isShowVerifiedBank = false;
        $scope.bankLinkText = 'Link Bank Account';
    };

    $scope.uploadImage = function (file) {
        if (!file)
            return;

        $scope.familyMember.isUploading = true;
        Upload.base64DataUrl(file).then(function (url) {

            var profileImage = {
                Base64ImageUrl: url,
                FileName: file.name,
                ContentType: file.type,
                FamilyMemberId: $scope.familyMember.Id
            };

            dashboardService.uploadPhoto(profileImage).success(function (response) {
                $scope.familyMember.ProfileImageUrl = response;
                authData.ProfileUrl = response;
                localStorageService.remove('authorizationData');
                localStorageService.set('authorizationData', authData);
                $rootScope.$broadcast('profileImageChanged', { profileImageUrl: response });
                $scope.familyMember.isUploading = false;
            }).error(function (err) {
                $scope.familyMember.isUploading = false;
            });
        });
    };

    $scope.toggleEmailSubscription = function () {
        if ($scope.isSubscribing)
            return;

        $scope.isSubscribing = true;
        $scope.emailSubscriptionContent = $scope.familyMember.IsUnSubscribed ? 'Subscribing...' : 'Unsubscribing...';
        familyMemberService.toggleEmailSubscription().success(function (response) {
            $scope.familyMember.IsUnSubscribed = !$scope.familyMember.IsUnSubscribed;
            $scope.emailSubscriptionContent = $scope.familyMember.IsUnSubscribed ? 'Subscribe' : 'Unsubscribe';
            $scope.isSubscribing = false;
        }).error(function (err) {
            $scope.isSubscribing = false;
            $scope.emailSubscriptionContent = $scope.familyMember.IsUnSubscribed ? 'Subscribe' : 'Unsubscribe';
            DisplayAlert(err, 'danger');
        });
    };

    $scope.ChangeVerifyAmount = function () {
        if ($scope.verifyDepositAmountsNonPlaid.AmountOne == null)
            $scope.isShowAmountsError = true;
    }


    $scope.verifyNonPlaidExternalAccount = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.familyMemeberForm.$valid) {
            $scope.isVerifying = true;

            var amountOne = ($scope.verifyDepositAmountsNonPlaid.AmountOne) / 100;

            if ($scope.verifyDepositAmountsNonPlaid.AmountTwo == undefined || $scope.verifyDepositAmountsNonPlaid.AmountTwo == null || $scope.verifyDepositAmountsNonPlaid.AmountTwo == NaN) {
                var amountTwo = 0;
            }
            else {
                var amountTwo = ($scope.verifyDepositAmountsNonPlaid.AmountTwo) / 100;
            }
            //   verify   
            NonplaidbankAuthorizeService.verifyBankAccount(amountOne, amountTwo).then(function (response) {
                $scope.isVerifying = false;
                $state.reload();
                $scope.gotoNextPage();

            }, function (err) {

                $scope.isVerifying = false;
                DisplayAlert(err.data, 'danger');
            });
        } else
            $scope.isShowAmountsError = true;
    }
    $scope.linkHandler = Plaid.create({
        selectAccount: true,
        env: 'tartan',
        //  env: 'production',
        clientName: 'Busykid',
        key: '428e241fd1b4c6a53b6378050afdeb', // plaid public key
        product: 'auth',
        // To use Link with longtail institutions on Connect, set the
        // 'longtail' option to true:
        // longtail: true,
        onLoad: function () {
            // The Link module finished loading.
        },
        onSuccess: function (public_token, metadata) {
            // Send the public_token to your app server here.
            // The metadata object contains info about the institution the
            // user selected and the account ID, if selectAccount is enabled.
            bankAuthorizeService.linkBankAccount(public_token, metadata.institution.name, metadata.account_id, false).success(function (response) {
                debugger;
                if ($stateParams.action == 'subscription') {
                    bankAuthorizeService.ChangeCreditCardStatus().success(function (response) {
                    }).error(function (err, data) {
                        $scope.familyMember = angular.copy($scope.familyMemberCopy);
                        //$scope.saveBtnTxt = 'Save Settings';
                        $scope.isUpdating = false;
                        DisplayAlert(err, 'danger');
                    });
                    //$scope.isLoading = false;
                    //$scope.subscribe();
                    $scope.gotoConnectionSuccess();
                    // $scope.subscribe();
                }
                else {
                    bankAuthorizeService.ChangeCreditCardStatus().success(function (response) {
                    }).error(function (err, data) {
                        $scope.familyMember = angular.copy($scope.familyMemberCopy);
                        //$scope.saveBtnTxt = 'Save Settings';
                        $scope.isUpdating = false;
                        DisplayAlert(err, 'danger');
                    });
                    //$scope.isLoading = false;
                    //$scope.subscribe();
                    $scope.gotoConnectionSuccess();
                }
            }).error(function (err) {
                $scope.isLoading = false;
                DisplayAlert(err, 'danger');
                $state.go('linkbankinfo');
            });
        },
        onExit: function (err, metadata) {
            if ($stateParams.action == 'update') { $state.go('myAccount'); }
            else {
                $rootScope.modalInstance = $uibModal.open({
                    scope: $rootScope,
                    templateUrl: '/app/components/addAccount/AddExternalAccount.html',
                    controller: 'AddExternalAccountController',
                    resolve: {
                        chkstatetype: function () {
                            return 1;
                        }
                    },
                    size: 'sm',
                    windowClass: 'box-popup',

                });
            }

        }
    });
   
    $scope.gotoConnectionSuccess = function () {
        if ($scope.isUpdateProgress)
            $state.go('connectionSuccess', { action: $stateParams.action });
        else
            $state.go('connectionSuccess');
    }

    $scope.moveToNextPage = function () {
        $state.go('adminDashboard');
    }

    $scope.gotoLinkBankPage = function () {
        if (parseInt($scope.selectedAccountType) === 0) {
            //if ($scope.isUpdateProgress)
            //    $state.go('linkbank', { action: 'update' });
            //else
            //    $state.go('linkbank');
            $scope.linkHandler.open();
        }
        else if (parseInt($scope.selectedAccountType) === 1)
            $state.go('addmicrodepositaccount');
    }
    $scope.openplaid = function () {
        $scope.selectedAccountType = 0;
        //$state.go('linkbank');
        $scope.linkHandler.open();
        $scope.isLoading = true;
    }

    $scope.OpenCreditCard = function (cardreplace) {
        $rootScope.replacecard = "replacecard";
        $state.go("addCreditCard");
    }





}]);

