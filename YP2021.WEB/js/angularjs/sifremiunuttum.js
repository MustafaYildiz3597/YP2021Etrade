var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask']);


var compareTo = function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
};
app.directive("compareTo", compareTo);

function NeroCtrl($scope, $window, $http, $timeout) {
    
    $scope.PageBreadCrumb = "Şifremi Unuttum";
    $scope.PageHeaderText = "Kullanıcı";

    $scope.changePassword = {};
    $scope.changePassword.user = { password: "", newpassword: "", confirmPassword: ""};


    $scope.showingDiv = "1";

    $scope.counter = 180;
    var stopped;
    $scope.countdown = function () {
        stopped = $timeout(function () {
            //console.log($scope.counter);
            $scope.counter--;
            if ($scope.counter == 0) {

                $scope.otp = 2;

                $scope.stop();

                $scope.modalMsg = "Lütfen cep telefonunuza gönderilen şifreyi zamanında giriniz. ";
                $("#modal1").fadeIn(300);
                $("body").addClass("overflow");

                return;
            }
            $scope.countdown();
        }, 1000);
    };
    $scope.stop = function () {
        $timeout.cancel(stopped);
    }

    $scope.KayitOlModel = { "KartNo": "", "Sifre": "", "IdentifierNumber": "", "Otp": "", "FullName": "" };

    $scope.OtpCancelled = function () {
        $scope.stop();
        $scope.showingDiv = "1";
    }

    // kullanılmıyor...
    // spinner eklenmedi... !!!!
    $scope.SendOTP = function () {

        $scope.ResetFrmOTP();

        var jsondata = { "MobileNumber": $scope.KayitOlModel.IdentifierNumber };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/BPUye/SendOTP/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        })
            .then(function (response) {

                $scope.showingDiv = "2";
                $scope.counter = 180;
                $scope.otp = 1;
                $scope.countdown();

            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };

            //.error(function (data, status, headers, config) {

            //    $scope.modalMsg = data.Message;

            //    $("#modal1").fadeIn(300);
            //    $("body").addClass("overflow");

            //    //alert("İşlem yapılırken hata oluştu.");

            //});
    }

    $scope.ResetFrmOTP = function () {
        $scope.frmOTP.$setPristine();
        $scope.frmOTP.$setUntouched();

        $scope.KayitOlModel.Otp = "";
    }

    $scope.Showcounter = function () {
        return $scope.right("00" + parseInt($scope.counter / 60), 2) + ':' + $scope.right("00" + ($scope.counter % 60), 2);
    }

    $scope.right = function (str, chr) {
        return str.slice(str.length - chr, str.length);
    }

    $scope.Step1 = function (isvalid) {

        if (!isvalid)
            return;

        $.LoadingOverlay("show");

        // var jsondata = { "MobileNumber": $scope.KayitOlModel.IdentifierNumber };
        //var jsondata = {
        //    "KartNo": $scope.KayitOlModel.KartNo,
        //    "Telefon": $scope.KayitOlModel.IdentifierNumber,
        //    "SendOTP": true
        //};
        //var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Member/RememberPassOTPCheckPhoneNumber/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.KayitOlModel // dt
            , dataType: "json"
        })
            .then(function (response) {

                //$scope.ResponseOTP = data.OTP;

                //SERVER Tarafında SendOTP yapılmaktadır.
                //$scope.SendOTP();
                //return;

                $window.setTimeout(function () {

                    $.LoadingOverlay("hide");

                    $scope.OTPVerifyID = response.data.OTPVerifyID;
                    $scope.Otpkey = response.data.Otpkey;

                    $scope.isDisabled = true;

                    $scope.showingDiv = "2";
                    $scope.counter = 180;
                    $scope.otp = 1;
                    $scope.countdown();

                }, 1000);

            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };

            //.error(function (data, status, headers, config) {

            //    toastr.error(data.Message);

            //    $window.setTimeout(function () {
            //        $.LoadingOverlay("hide");
            //    }, 1000);
            //});

    }

    // post register
    $scope.Step2 = function (isvalid) {

        if (!isvalid)
            return;

        $scope.stop();

        $.LoadingOverlay("show");


        var jsondata = {
            VerifyID: $scope.OTPVerifyID,
            OTPCode: $scope.VerifyFormModel.Code
        }

        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Member/RememberPassConfirmOTP/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.changePassword.user = {
                        "newpassword": "",
                        "confirmPassword": "",
                        "VerifiedID": response.data.VerifiedID,
                        "GSMNumber": $scope.KayitOlModel.GSMNumber
                        //"IdentifierNumber": $scope.KayitOlModel.IdentifierNumber,
                        //"Otp": $scope.KayitOlModel.Otp
                    };

                    $window.setTimeout(function () {

                        $.LoadingOverlay("hide");

                        $scope.showingDiv = "3";
                        $scope.$apply();
                    }, 1000);
                }, 2000);
            }
                , function (error) {
                    $window.setTimeout(function () {
                        toastr.error(error.data.Message);
                        $.LoadingOverlay("hide");
                        $scope.$apply();
                    }, 2000);
                });
    }

    // şifre yenileme
    $scope.Step3 = function (isvalid) {

        if (!isvalid)
            return;

        $.LoadingOverlay("show");

        $http({
            url: "/api/Member/RememberPassChangePass/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.changePassword.user
            , dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.changePassword.user = { password: "", newpassword: "", confirmPassword: "" };

                    $scope.showingDiv = "tebrikler";
                    $.LoadingOverlay("hide");
                    $scope.$apply();

                }, 2000);

            }, function (error) {

                $window.setTimeout(function () {

                    toastr.error(error.data.Message);
                    $.LoadingOverlay("hide");

                    $scope.$apply();

                }, 2000);
            });
    }

    $scope.submit = function (isValid) {

        if (!isValid)
            return;

        if (isValid) {

            $.LoadingOverlay("show");

            $http({
                url: "/api/Member/PostChangePassword"
                , method: "POST"
                , params: {}
                , contentType: "application/json"
                , data: $scope.changePassword.user
                , dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.changePassword.user = { password: "", newpassword: "", confirmPassword: "" };

                        $scope.ShowingDiv = "tebrikler";
                        $.LoadingOverlay("hide");
                        $scope.$apply();

                    }, 2000);

                }, function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        $.LoadingOverlay("hide");

                        $scope.$apply();

                    }, 2000);
                });
            

        } else {
            model.message = "Geçersiz İşlem"; //  There are still invalid fields below
        }
    };

    $scope.ResendOTP = function () {
        $scope.Step1(true);
    }
}
 
app.controller('NeroCtrl', NeroCtrl);
