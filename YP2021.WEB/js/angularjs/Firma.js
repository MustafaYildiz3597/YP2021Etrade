var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.light-columnfilter']);

app.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            $(element).datepicker({
                //format: 'dd.mm.yyyy',
                //dateFormat: 'dd.mm.yy',
                dateFormat: 'dd.mm.yy',
                language: 'tr',
                //showButtonPanel: true,
                showOtherMonths: true,
                prevText: "<",
                nextText: ">",
                //changeMonth: true,
                changeYear: true,
                //yearRange: "-10:+10",
                onSelect: function (date) {
                    ctrl.$setViewValue(date);
                    ctrl.$render();
                    scope.$apply();
                }
            });
        }
    };
});

function NeroCtrl($scope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {

    //$scope.emailFormat = /^[a-z]+[a-z0-9._]+@[a-z0-9]+\.[a-z.]{2,5}$/;
    $scope.emailFormat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    $scope.start = "2019-01-01";
    $scope.end = "2019-12-31";
    $scope.firsttime = true;

    $scope.filterdatabydate = function (start, end) {

        if ($scope.firsttime === true) {
            $scope.firsttime = false;
            return;
        }

        $scope.start = start;
        $scope.end = end;

        $scope.dtInstance.rerender();

        // $scope.Listele(start.format('YYYY-MM-DD'), end.format('YYYY-MM-DD'));
        //alert("start:" + start.format('YYYY-MM-DD') + "; end:" + end);
    };

    $scope.CmbFirms = [];
    $scope.CmbPeriods = [];
    $scope.CmbSalesMans = [];
    $scope.SelectedFirm = "119";
    $scope.SelectedPeriod = "01";

    var today = new Date();
    var today7 = new Date();
    today7.setDate(today.getDate() - 7);

    var beginDate = formattedDate(today7); //dateFilter(today7, 'dd.MM.yyyy'); // dateFilter(new Date, 'dd/MM/yyyy');
    var endDate = formattedDate(today); // dateFilter(today, 'dd.MM.yyyy');

    $scope.selected = [{ NodeGroupID: "", BeginDate: beginDate, EndDate: endDate }];
    $scope.selected.BeginDate = beginDate;
    $scope.selected.EndDate = endDate;


    $scope.error = false;
    $scope.success = false;
    $scope.Saved = false;

    $scope.ShowingDiv = "Liste";

    function renderNum(data) {
        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        var parts = data.toFixed(2).toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }



    $scope.FormModel = {};

    $scope.Saved = false;

    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        //$scope.FormModel.KEPExpirationDT2 = moment($('#KEPExpirationDT').val(), "DD/MM/YYYY").toDate();
        //var from = $("#KEPExpirationDT").val().split(".");

        $scope.FormModel.KEPExpirationDT2 = moment($("#KEPExpirationDT").val(), ['DD.MM.YYYY', 'YYYY-MM-DD']).format();

        //$scope.FormModel.KEPExpirationDT2 = new Date(from[2], from[1], from[0]);


             
        $.LoadingOverlay("show");

        $http({
            url: "/api/Firm/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.ID = response.data.ID;
            toastr.success(response.data.Message);
            $.LoadingOverlay("hide");

            $scope.Saved = true;

            //$scope.Listele();

            //debugger;
            //$scope.dtInstance.rerender();
            

        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");
            
        });

    };

    $scope.AddItem = function () {

        $scope.SavedMessage = "";
        $scope.FormModel = { "ID": "" };

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $scope.ShowingDiv = "detail";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $.LoadingOverlay("show");

        setTimeout(function () {

            $scope.SavedMessage = "";

            $scope.FormModel = {};

            //if (id == 0) {
            //    $scope.FormModel = {};
            //    $scope.FormModel.ID = 0;
            //    $scope.FormModel.VideoURL = "";
            //    $scope.FormModel.ImageURL = "";
            //    $scope.FormModel.ImageURL2 = "";
            //}

            var jsondata = {
                ID: id
            };
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/Firm/Detail/",
                method: "POST",
                params: {
                },
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.FormModel = response.data;

                        // $scope.FormModel.KEPExpirationDT = moment().format('dd.mm.YYYY');
                        // debugger;
                        //var dt = new Date($scope.FormModel.KEPExpirationDT);
                        //$scope.FormModel.KEPExpirationDT = new Date("2000-01-01"); //moment($scope.FormModel.KEPExpirationDT, ['YYYY-MM-DD', 'dd.mm.yyyy']).format();
                        $scope.FormModel.KEPExpirationDT = moment($scope.FormModel.KEPExpirationDT).format('DD.MM.YYYY');

                        //moment($("#KEPExpirationDT").val(), ['DD.MM.YYYY', 'YYYY-MM-DD']).format();

                        //var expdt = moment($scope.FormModel.KEPExpirationDT, ['YYYY-MM-DD', 'DD.MM.YYYY']).format();
                        //$scope.FormModel.KEPExpirationDT = expdt;
                        //$('#KEPExpirationDT').val(expdt);
                        //$("#KEPExpirationDT").datepicker("setDate", $scope.FormModel.KEPExpirationDT); 
                        //$('#KEPExpirationDT').datepicker().trigger('change');

                        //https://stackoverflow.com/questions/11507508/how-to-dynamically-set-bootstrap-datepickers-date-value
                        //$("#datepicker").datepicker("setDate", new Date); 
                        $scope.ShowingDiv = "detail";
                        $.LoadingOverlay("hide");
                        $scope.$apply();

                    }, 2000);
                }
                    , function (error) {

                        $window.setTimeout(function () {

                            toastr.error(error.data.Message);
                            $.LoadingOverlay("hide");

                            $scope.$apply();

                        }, 2000);
                    });
            //.success(function (data, status, headers, config) {

            //    $scope.FormModel = data;


            //    $scope.ShowingDiv = "detail";
            //})
            //.error($scope.errorcallback)
            //.finally($scope.finallycallback);

        }, 500);
    };

    function renderEdit(data) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt\"></i></a>";
    }

    function renderDate(data) {

        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        //var parts = data.toFixed(2).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        //return parts.join(".");

        if (data === null)
            return "";

        return formattedDate(data);
    }


    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("Title", "Firma Ünvanı"),
            DTColumnBuilder.newColumn("ContactName", "Kontak Kişi"),
            DTColumnBuilder.newColumn("ContactGSMNo", "Kontak GSM No"),
            DTColumnBuilder.newColumn("ContactEmail", "Kontak E-Posta"), //.renderWith(renderNum).withClass('numericCol'),
            DTColumnBuilder.newColumn("KEPStatus", "KEP Durumu"), //.renderWith(renderNum).withClass('numericCol'),
            DTColumnBuilder.newColumn("KEPExpirationDT", "Son Kul.Tar.").renderWith(renderDate),
            DTColumnBuilder.newColumn("KEPMemberMaxLimit", "KEP Limit"),
            DTColumnBuilder.newColumn("CreateDT", "Kayıt Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("CreateFullName", "Kaydeden"),
            DTColumnBuilder.newColumn("ID", "Detay").renderWith(renderEdit)
        ];

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Firm/DTList/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                //d.FirmCode = $scope.SelectedFirm;// $('.daterangepicker_start_input input').val();
                //d.PeriodCode = $scope.SelectedPeriod; ////$('.daterangepicker_end_input input').val();
                //d.SalesmanRef = $scope.SelectedSalesManRef;
                //d.BeginDate = $scope.start;
                //d.EndDate = $scope.end;
            }
        })
            .withDOM('lBfrtip')
            .withPaginationType('full_numbers')
            .withDisplayLength(10)
            .withOption('responsive', true)
            .withOption('scrollX', 'auto')
            .withOption('scrollCollapse', true)
            .withOption('autoWidth', true)
            .withButtons([
                'print',
                'excel'
            ])
            .withLightColumnFilter({
                '0': { type: 'text' },
                '1': { type: 'text' },
                '2': { type: 'text' },
                '3': { type: 'text' },
                '4': { type: 'text' },
                '5': { type: 'text' }

                });

        $scope.dtInstance = {};
        //$scope.dtInstance.rerender();
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

    $scope.FirmChanged = function (f) {

        $scope.CmbPeriods = $scope.Periods.filter(function (item) {
            return (item.FIRMNR === f);
        });

        if ($scope.CmbPeriods.length > 0) {
            $scope.SelectedPeriod = $scope.CmbPeriods[0].NR;
            $scope.start = moment($scope.CmbPeriods[0].BEGDATE).format(); //"2019-01-01";
            $scope.end = moment($scope.CmbPeriods[0].ENDDATE).format();  //"2019-12-31";
        }

        $scope.CmbSalesMans = $scope.SalesMans.filter(function (item) {
            return (item.FIRMNR === f);
        });

        $scope.SelectedSalesManRef = "";

    }

    $scope.ReRenderList = function () {

        $("#recent-orders .date-range-report span").html(
            moment($scope.start).format("ll") + " - " + moment($scope.end).format("ll")
        );

        $scope.dtInstance.rerender();
    };

    $scope.FillAllCmbs = function () {


        $scope.SelectedPeriod = "01";

        $http({
            url: "/api/Rapor/FillAllCombo/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .success(function (data, status, headers, config) {

                $scope.SelectedPeriod = "01";

                $scope.CmbFirms = data.Firms;

                if (data.Firms.length > 0) {

                    $scope.Periods = data.Periods;
                    $scope.CmbPeriods = $scope.Periods.filter(function (item) {
                        return (item.FIRMNR === $scope.SelectedFirm);
                    });

                    //if ($scope.CmbPeriods.length > 0)
                    //    $scope.SelectedPeriod = $scope.CmbPeriods[0].NR;

                    $scope.SalesMans = data.SalesMans;
                    $scope.CmbSalesMans = $scope.SalesMans.filter(function (item) {
                        return (item.FIRMNR === $scope.SelectedFirm);
                    });

                    $scope.SelectedSalesManRef = "";

                    //if ($scope.CmbSalesMans.length > 0)
                    //    $scope.SelectedSalesMan = $scope.CmbSalesMans[0].CODE;


                    // $scope.Listele();
                }
            })
            .error($scope.errorcallback).finally($scope.finallycallback);

    }

    $scope.Init = function () {
        //$.LoadingOverlay("show");
        //$scope.FillAllCmbs();
        $scope.Listele();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    $scope.Init();

    $scope.BackToList = function () {
        //$.LoadingOverlay("show");
        $scope.ShowingDiv = "Liste";

        if ($scope.Saved === true)
            $scope.dtInstance.rerender();

        $scope.Saved = false;

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
