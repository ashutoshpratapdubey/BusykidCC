// Angular modules 
// Custom modules 
// 3rd Party Modules
var mjcApp = angular.module('mjcApp', ['LocalStorageModule', 'ui.router', 'ui.bootstrap', 'ngFileUpload', 'ui.bootstrap.datetimepicker', 'ngAnimate', 'monospaced.elastic', 'ngIdle', 'angularMoment', 'angularModalService', 'angularjs-crypto', 'CreditCard']);

mjcApp.config(function ($stateProvider, $urlRouterProvider) {
    var IsCookie = document.cookie;
    var CookieUser = '';
    var CookiePassword = '';

    if (IsCookie) {
        try {
            IsCookie = IsCookie.split(';');
            CookieUser = IsCookie[0].split('=')[1];
            CookiePassword = IsCookie[1].split('=')[1];
        }
        catch (err) {
            CookieUser = '';
            CookiePassword = '';
        }
    }



    $stateProvider
        .state('login', {
            url: '/login',
            views: {
                "login": {
                    templateUrl: '/app/components/account/loginView.html',
                    controller: 'loginController'
                },
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {}
            },
            data: { pageTitle: 'BusyKid - Login' }
        })

        .state('signup', {
            url: '/signup/:offer',
            views: {
                "login": {},
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/account/signUpView.html',
                    controller: 'signUpController'
                }
            },
            params: {
                offer: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Sign Up' }
        })

        .state('signupprogress', {
            url: '/signupprogress',
            views: {
                "login": {},
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/account/signUpProgressView.html',
                    controller: 'signUpProgressController'
                }
            },
            data: { pageTitle: 'BusyKid - Sign Up Progress' }
        })

        .state('additionalmemberinfo', {
            url: '/additionalmemberinfo/:action',
            views: {
                "login": {},
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/family/updateMemberInfoView.html',
                    controller: 'updateMemberInfoController'
                }
            },
            params: {
                action: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Member Information' }
        })

        .state('welcome', {
            url: '/welcome',
            views: {
                "login": {},
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/welcome/welcomeView.html'
                }
            },
            data: { pageTitle: 'BusyKid - Welcome' }
        })

        .state('createPin', {
            url: '/createPin',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/createPin/createPinView.html',
                    controller: 'createPinController'
                }
            },
            data: { pageTitle: 'BusyKid - Create Pin' }
        })

        .state('phoneEntry', {
            url: '/phoneEntry',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/phoneEntry/phoneEntryView.html',
                    controller: 'phoneEntryController'
                }
            },
            data: { pageTitle: 'BusyKid - Phone Entry' }
        })

        .state('phoneNumberConfirmation', {
            url: '/phoneNumberConfirmation',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/phoneEntry/phoneNumberConfirmationView.html',
                    controller: 'phoneEntryController'
                }
            },
            data: { pageTitle: 'BusyKid - Phone Number Confirmation' }
        })

        .state('profilePicture', {
            url: '/profilePicture',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/profilePicture/profilePictureView.html',
                    controller: 'profilePictureController'
                }
            },
            data: { pageTitle: 'BusyKid - Profile Picture' }
        })

        .state('addChild', {
            url: '/addChild',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addChild/addChildView.html',
                    controller: 'childController'
                }
            },
            data: { pageTitle: 'BusyKid - Add Child' }
        })

        .state('addChildPhone', {
            url: '/addChildPhone/:id',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addChild/addChildPhoneView.html',
                    controller: 'childController'
                }
            },
            data: { pageTitle: 'BusyKid - Add Child Phone' }
        })

        .state('addChildPin', {
            url: '/addChildPin/:id/:name',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/createPin/createPinView.html',
                    controller: 'createPinController'
                }
            },
            data: { pageTitle: 'BusyKid - Add Child Pin' }
        })

        .state('suggestedChore', {
            url: '/suggestedChore/:id',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/chore/suggestedChores.html',
                    controller: 'choreController'
                }
            },
            data: { pageTitle: 'BusyKid - Suggested Chores' }
        })

        .state('linkbankinfo', {
            url: '/linkbankinfo/:action/:promocode',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/bankauthorization/linkBankInfoView.html',
                    controller: 'linkBankController'
                }
            },
            params: {
                action: { squash: true, value: null },
                promoCode: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Link Bank Information' }
        })

        .state('linkbank', {
            url: '/linkbank/:action',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/bankauthorization/linkbankview.html',
                    controller: 'linkBankController'
                }
            },
            params: {
                action: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Link Bank' }
        })

        .state('addmicrodepositaccount', {
            url: '/addmicrodepositaccount',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/bankauthorization/addMicroDepositAccountView.html',
                    controller: 'addMicroDepositAccountController'
                }
            },
            params: {
                action: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Link Bank' }
        })

        .state('verifymicrodeposit', {
            url: '/verifymicrodeposit',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/bankauthorization/verifymicrodepositpage.html',
                    controller: 'verifyMicroDepositController'
                }
            },
            data: { pageTitle: 'BusyKid - Verify Micro Deposit' }
        })

        .state('connectionSuccess', {
            url: '/connectionsuccess/:action',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/bankauthorization/connectionSuccessView.html',
                    controller: 'linkBankController'
                }
            },
            params: {
                action: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Connection Success' }
        })

        .state('adminDashboard', {
            url: '/adminDashboard/:promoCode',
            params: { promoCode: { squash: true, value: null } },
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/dashboard/admindashboard.html',
                    controller: 'adminDashboardController'
                }
            },
            data: { pageTitle: 'BusyKid - Dashboard' }
        })


        .state('childDashboard', {
            url: '/childDashboard',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/dashboard/childdashboard.html',
                    controller: 'childDashboardController'
                }
            },
            data: { pageTitle: 'BusyKid - Dashboard' }
        })

        .state('manageusers', {
            url: '/manageusers',
            views: {
                "login": {},
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/family/manageUsersView.html',
                    controller: 'manageUsersController'
                }
            },
            data: { pageTitle: 'BusyKid - Manage Users' }
        })

        .state('allocationSettings', {
            url: '/allocationsettings',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/settings/allocationsettingsview.html',
                    controller: 'allocationSettingsController'
                }
            },
            data: { pageTitle: 'BusyKid - Child Profile' }
        })

        .state('switchPin', {
            url: '/switchPin/:id/:name',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/account/switchPinView.html',
                    controller: 'switchPinController'
                }
            },
            data: { pageTitle: 'BusyKid - Switch Pin' }
        })

        .state('myAccount', {
            url: '/myaccount/:subscribe',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/account/myAccountView.html',
                    controller: 'myAccountController'
                }
            },
            params: {
                subscribe: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - My Account' }
        })

         .state('childAccount', {
             url: '/childAccount/:id',
             views: {
                 "header": {
                     templateUrl: '/app/shared/header/headerView.html',
                     controller: 'headerController'
                 },
                 "content": {
                     templateUrl: '/app/components/account/childAccountView.html',
                     controller: 'childAccountController'
                 }
             },
             params: {
                 id: { squash: true, value: null }
             },
             data: { pageTitle: 'BusyKid -  My Account' }
         })

        .state('addUser', {
            url: '/addUser',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addUser/addUserView.html',
                    controller: 'addUserController'
                }
            },
            data: { pageTitle: 'BusyKid - Add New User' }
        })

        .state('addParentPhone', {
            url: '/parent/phone',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addUser/addParent/hasPhoneView.html',
                    controller: 'parentController'
                }
            },
            data: { pageTitle: 'BusyKid - Add New Parent' }
        })

        .state('parentSignUp', {
            url: '/invite/:invitationtoken',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addUser/addParent/parentSignUpView.html',
                    controller: 'parentController'
                }
            },
            data: { pageTitle: 'BusyKid - Parent' }
        })

        .state('addParent', {
            url: '/parent/add',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addUser/addParent/addParentView.html',
                    controller: 'parentController'
                }
            },
            data: { pageTitle: 'BusyKid - Add New Parent' }
        })

        .state('addParentPin', {
            url: '/parent/pin/:id/:name',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/createPin/createPinView.html',
                    controller: 'createPinController'
                }
            },
            params: {
                id: { squash: true, value: null },
                name: { squash: true, value: null }
            },
            data: { pageTitle: 'BusyKid - Add New Parent' }
        })

        .state('inviteContributor', {
            url: '/inviteContributor',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/addUser/invitation/invitationView.html',
                    controller: 'invitationController'
                }
            },
            data: { pageTitle: 'BusyKid - Invite a Contributor' }
        })

        .state('buystock', {
            url: '/buystock',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/save/stockView.html',
                    controller: 'stockController'
                }
            },
            data: { pageTitle: 'BusyKid - Stock' }
        })

        .state('buygiftcard', {
            url: '/buygiftcard',
            views: {
                "header": {
                    templateUrl: '/app/shared/header/headerView.html',
                    controller: 'headerController'
                },
                "content": {
                    templateUrl: '/app/components/spend/giftcardView.html',
                    controller: 'giftCardController'
                }
            },
            data: { pageTitle: 'BusyKid - Gift Cards' }
        })

    .state('familyPage', {
        url: '/family/:name',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/family/familyView.html',
                controller: 'familyController'
            }
        },
        data: { pageTitle: 'BusyKid - Family' }
    })

    .state('forgotPassword', {
        url: '/forgotPassword',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/account/forgotPasswordView.html',
                controller: 'forgotPasswordController'
            }
        },
        data: { pageTitle: 'BusyKid - Forgot Password' }
    })

    .state('resetPassword', {
        url: '/reset/:token',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/account/resetPasswordView.html',
                controller: 'resetPasswordController'
            }
        },
        data: { pageTitle: 'BusyKid - Reset Password' }
    })

    .state('subscription', {
        url: '/subscribe',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/subscription/subscriptionView.html',
                //templateUrl: '/app/components/subscription/linkbankview.html',
                controller: 'subscriptionController'
            }
        },
        data: { pageTitle: 'BusyKid - Subscription' }
    })

    .state('incompleteSignUp', {
        url: '/incompletesignup',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/account/incompleteSignUpView.html',
                controller: 'incompleteSignUpController'
            }
        },
        data: { pageTitle: 'BusyKid - Incomplete Signup' }
    })

    .state('termsandcondition', {
        url: '/termsandcondition',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/termsAndPrivacy/termsAndConditionsView.html'
            }
        },
        data: { pageTitle: 'BusyKid - Terms And Conditions' }
    })

    .state('privacypolicy', {
        url: '/privacypolicy',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/termsAndPrivacy/privacyPolicyView.html',
                controller: 'privacyPolicyController'
            }
        },
        data: { pageTitle: 'BusyKid - Privacy Policy' }
    })

    .state('coppa', {
        url: '/coppa',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/termsAndPrivacy/coppaView.html'
            }
        },
        data: { pageTitle: 'BusyKid - Coppa' }
    })

    .state('subscriptionDetail', {
        url: '/subscriptiondetail',
        views: {
            "header": {
                templateUrl: '/app/shared/header/headerView.html',
                controller: 'headerController'
            },
            "content": {
                templateUrl: '/app/components/termsAndPrivacy/subscriptionDetailView.html'
            }
        },
        data: { pageTitle: 'BusyKid - Subscription Detail' }
    })

   .state('AddExternalAccount', {
       url: '/AddExternalAccount',
       views: {
           "header": {
               templateUrl: '/app/shared/header/headerView.html',
               controller: 'headerController'
           },
           "content": {
               templateUrl: '/app/components/addAccount/AddExternalAccount.html',
               controller: 'AddExternalAccountController'
           }
       },
       params: {
           action: { squash: true, value: null }
       },
       data: { pageTitle: 'BusyKid - Link Bank' }
   })

     .state('AuthorizeExternalAccount', {
         url: '/AuthorizeExternalAccount',
         views: {
             "header": {
                 templateUrl: '/app/shared/header/headerView.html',
                 controller: 'headerController'
             },
             "content": {
                 templateUrl: '/app/components/addAccount/AuthorizeExternalAccount.html',
                 controller: 'AuthorizeExternalAccountController'
             }
         },
         params: {
             action: { squash: true, value: null }
         },
         data: { pageTitle: 'BusyKid - Link Bank' }
     })


        // for the home in app
           .state('goToDashboard', {
               url: '/goToDashboard',
               views: {
                   "header": {
                       templateUrl: '/app/shared/header/headerView.html',
                       controller: 'headerController'
                   },
                   "content": {
                       //  templateUrl: '/app/components/dashboard/goToDashboard.html',
                       templateUrl: null,
                       controller: 'goToDashboardController'
                   }
               }
           })

      .state('test', {
          url: '/test',
          views: {
              "header": {
                  templateUrl: '/app/shared/header/headerView.html',
                  controller: 'headerController'
              },
              "content": {
                  templateUrl: '/app/components/bankAuthorization/test.html',
                  controller: 'testController'
              }
          },
          data: { pageTitle: 'BusyKid - test ' }
      })


           .state('addCreditCard', {
               url: '/addCreditCard',
               views: {
                   "header": {
                       templateUrl: '/app/shared/header/headerView.html',
                       controller: 'headerController'
                   },
                   "content": {
                       templateUrl: '/app/components/bankAuthorization/addCreditCard.html',
                       controller: 'addCreditCardController'
                   }
               },
               data: { pageTitle: 'BusyKid - Add a Card ' }
           })
           .state('validateCreditCard', {
               url: '/validateCreditCard',
               views: {
                   "header": {
                       templateUrl: '/app/shared/header/headerView.html',
                       controller: 'headerController'
                   },
                   "content": {
                       templateUrl: '/app/components/bankAuthorization/validateCreditCard.html',
                       controller: 'validateCreditCardController'
                   }
               },
               data: { pageTitle: 'BusyKid - Validate the Card ' }
           })

    //.state('maintenance', {
    //    url: '/maintenance',
    //    views: {
    //        "login": {
    //            templateUrl: '/app/components/account/maintenance.html',
    //        },

    //        "content": {}
    //    },
    //    data: { pageTitle: 'BusyKid - Login' }
    //})

});

mjcApp.config(['$httpProvider', 'msdElasticConfig', 'IdleProvider', function ($httpProvider, msdElasticConfig, IdleProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
    msdElasticConfig.append = '\n';

    IdleProvider.idle(1800); // in seconds
    IdleProvider.timeout(1); // in seconds
}]);

mjcApp.constant('settings', {
    apiBaseUri: 'http://localhost:60749/', // local api
    // apiBaseUri: 'http://34.197.230.102/', // local api
    //apiBaseUri: 'http://bkstageapi.azurewebsites.net/' // stage api
    //Amazon RDS
    //apiBaseUri: 'https://api.busykid.com/' // stage api
});

mjcApp.run(['$rootScope', '$state', '$stateParams', '$remember', 'accountService', '$timeout', function ($rootScope, $state, $stateParams, $remember, accountService, $timeout) {
    $rootScope.base64Key = CryptoJS.enc.Hex.parse('0123456789abcdef0123456789abcdef')
    $rootScope.iv = CryptoJS.enc.Hex.parse('abcdef9876543210abcdef9876543210');

    $rootScope.$state = $state;
    $rootScope.$stateParams = $stateParams;



    if ($remember('password') != '' && $remember('password') != null) {

        var newParams = CryptoJS.lib.CipherParams.create({
            ciphertext: CryptoJS.enc.Base64.parse($remember('password'))
        });

        var decrypted = CryptoJS.AES.decrypt(
                      newParams,
                      $rootScope.base64Key,
                      { iv: $rootScope.iv });
        $rootScope.descrString = decrypted.toString(CryptoJS.enc.Utf8)

        $rootScope.loginCredential =
            {
                Email: $remember('username'),
                Password: $rootScope.descrString
            };

        //STPL Code Change  Dated 20-Apr-2017 Refresh Issue
        var url = $(location).attr('href');


        //    if (url == "http://dev.busykid.com/#/Login/" || url == "http://dev.busykid.com/") {

        if (url == "http://localhost:60763/#/login/" || url == "http://localhost:60763/") {
            //  if (url == "https://app.busykid.com/#/Login/" || url == "https://app.busykid.com/") {
            $rootScope.userCookies = function () {
                accountService.login($rootScope.loginCredential).then(function (response) {
                    var authData = response;
                    $rootScope.autoLogin = 1;
                    $state.go('familyPage', { name: response.FamilyUrl });
                   // $location.path('/familyPage').replace();

                }),
                function (err) {
                    $timeout(function () { $state.go('login') });
                };
            }
            $rootScope.userCookies();
        }
    }
    else {
        $timeout(function () { $state.go('login') });
    }


    // Initialize stockpile modal popup
    Stockpile.init('qa', 'pk_qa_0b1fb5e42d1789b962735d87406c3154');
    //  Stockpile.init('production', 'pk_production_7d7cdc06f5df1fdcf974e04f4e8e46b2');
}]);

