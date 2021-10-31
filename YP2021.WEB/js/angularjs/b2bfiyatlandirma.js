var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter']);

app.config(['$provide', function ($provide) {
    $provide.decorator('$locale', ['$delegate', function ($delegate) {
        $delegate.NUMBER_FORMATS.DECIMAL_SEP = ',';
        $delegate.NUMBER_FORMATS.GROUP_SEP = '.';
        return $delegate;
    }]);
}]);

app.directive('yrInteger', function yrInteger() {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            element.on('keypress', function (event) {

                if (!isIntegerChar())
                    event.preventDefault();

                function isIntegerChar() {
                    return /[0-9]|,/.test(
                        String.fromCharCode(event.which))
                }

            })

        }
    }
});

function NeroCtrl($scope, $http, $window) {

    
    $scope.error = false;
    $scope.success = false;
    $scope.Saved = false;

    $scope.ShowingDiv = "Liste";

    $scope.newitemcount = -1;
    $scope.AddItem = function () {

        $scope.newitemcount += -1;

        $scope.B2BDealerTypeList.push({
            "ID": $scope.newitemcount,
            "Name": null,
            "SalesRate": null,
            "IsDeleted": null,
        });

    }

    $scope.DeleteItem = function (id) {
        var idx = $scope.B2BDealerTypeList.findIndex(x => x.ID === id);
        $scope.B2BDealerTypeList[idx].IsDeleted = true;
    }
    $scope.CancelDeletedItem = function (id) {
        var idx = $scope.B2BDealerTypeList.findIndex(x => x.ID === id);
        $scope.B2BDealerTypeList[idx].IsDeleted = false;
    }

    $scope.SaveList = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $.LoadingOverlay("show");

        $http({
            url: "/api/B2B/SaveDealerTypeList/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.B2BDealerTypeList
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {
                toastr.success(response.data.Message);
                $scope.B2BDealerTypeList = response.data.List;
                $.LoadingOverlay("hide");
                $scope.$apply();
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $.LoadingOverlay("hide");
            }, 1000);

        });
    }

    function renderNum(data) {
        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        var parts = data.toFixed(2).toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }

     
    $scope.FormModel = {};
     
    function renderDate(data) {

        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        //var parts = data.toFixed(2).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        //return parts.join(".");

        if (data === null)
            return "";

        return formattedDate(data);
    }

 
    $scope['Listele'] = function () {

        //var jsondata = {
        //    "ID": id
        //};
        var dt = {}; //JSON.stringify(jsondata);

        $http({
            url: "/api/b2b/DealerTypeList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.B2BDealerTypeList = response.data;
                     
                    $scope.ShowingDiv = "detail";
                    $.LoadingOverlay("hide");
                    $scope.$apply();

                }, 500);
            }
                , function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        $.LoadingOverlay("hide");

                        $scope.$apply();

                    }, 2000);
                });
    }

    // error callback
    $scope.errorcallback = function (data, status, headers, config) {

        // alert("! Bir hata oluştu : " + data);
        toastr.error("Bir hata oluştu : " + data, "Hata");

        // $('#myModal').modal('show');
        // $scope.PopupMessage = data;
    };

    // finally callback
    $scope.finallycallback = function () {
        $.LoadingOverlay("hide");
        $('html,body').scrollTop(0, 0);
    };

    $scope.Init = function (firmID, v, a, e, d) {

        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d }; 

        $.LoadingOverlay("show");
        $scope.FirmID = firmID;
        $scope.Listele();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

    $scope.BackToList = function () {

        //$.LoadingOverlay("show");

        //$scope.Saved = false;

        $scope.ShowingDiv = "Liste";


        $window.setTimeout(function () {

        //    //if ($scope.Saved === true)
        //    //    $scope.dtInstance.rerender();
            

        //    //$scope.apply();
        }, 500);

        
        //$scope.Saved = false;
        //$scope.Listele();
    };


    function formattedDate(date) {
        var d = new Date(date || Date.now()),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [year, month, day].join('-');
        //return [day, month, year].join('.');
    }

}
//])
app.controller('NeroCtrl', NeroCtrl);
