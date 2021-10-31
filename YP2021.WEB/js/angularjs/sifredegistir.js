var app = angular.module('NeroApp', ['ngSanitize']);


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

function NeroCtrl($scope, $window, $http) {

    $scope.ShowingDiv = "form";
    $scope.PageBreadCrumb = "Şifre Değiştirme";
    $scope.PageHeaderText = "Kullanıcı";

    $scope.changePassword = {};
    $scope.changePassword.user = { password: "", newpassword: "", confirmPassword: "" };

    $scope.submit = function (isValid) {

        if (!isValid)
            return;

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
    };
}

app.controller('NeroCtrl', NeroCtrl);
