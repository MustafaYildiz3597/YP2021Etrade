var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.light-columnfilter']);

app.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            $(element).datepicker({
                format: 'dd.mm.yyyy',
                dateFormat: 'dd.mm.yy',
                //dateFormat: 'dd.mm.yy',
                language: 'tr',
                //showButtonPanel: true,
                showOtherMonths: true,
                prevText: "<",
                nextText: ">",
                //changeMonth: true,
                changeYear: true,
                yearRange: "1900:+0",
                monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
                dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
                firstDay: 1,
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

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("TEKLIFNO", "Teklif Nr."),
            DTColumnBuilder.newColumn("FIRMA_ADI", "Müşteri Adı"),
            DTColumnBuilder.newColumn("ProductName", "Ürün Adı"),
            DTColumnBuilder.newColumn("CustomerCode", "Müşteri Nr."),
            DTColumnBuilder.newColumn("BUKOD", "BUKOD"),
            //DTColumnBuilder.newColumn("BUESKI", "BU Eski Nr."),
            DTColumnBuilder.newColumn("Quantity", "Adet"),
            DTColumnBuilder.newColumn("UnitPrice", "Birim Fiyat"),
            DTColumnBuilder.newColumn("CurrencyCode", "Para Birimi"),
            DTColumnBuilder.newColumn("ADD_DATE", "Teklif Tarihi")

            //DTColumnBuilder.newColumn("StartDate", "Başlangıç Tarihi").renderWith(renderDate),
            //DTColumnBuilder.newColumn("IsActive", "Durum").renderWith(renderStatus).withClass('text-center'),
            //DTColumnBuilder.newColumn("ID", "Düzenle").renderWith(renderEdit).withClass('text-center'),
            //DTColumnBuilder.newColumn("ID", "Sil").renderWith(renderDelete).withClass('text-center')
            //DTColumnBuilder.newColumn("TCIdentityNo", "TCKN"),
            //DTColumnBuilder.newColumn("KEPAddress", "KEP E-Posta"), //.renderWith(renderNum).withClass('numericCol'),
            //DTColumnBuilder.newColumn("SentPayrollsCount", "Top.Gönd.Bordro#"), //.renderWith(renderNum).withClass('numericCol'),
            //DTColumnBuilder.newColumn("KEPStatus", "KEP Durumu"), //.renderWith(renderNum).withClass('numericCol'),
            //DTColumnBuilder.newColumn("ApprovalStatus", "Onay Durumu"), 
            //DTColumnBuilder.newColumn("CreateDT", "Kayıt Tarihi").renderWith(renderDate),
            //DTColumnBuilder.newColumn("CreateFullName", "Kaydeden"),
            //DTColumnBuilder.newColumn("ID", "Detay").renderWith(renderEdit)
        ];


        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Offer/SorguPageDTList/", // "/api/Siparis/SorguDT/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                //d.FirmCode = $scope.SelectedFirm;// $('.daterangepicker_start_input input').val();
                //d.PeriodCode = $scope.SelectedPeriod; ////$('.daterangepicker_end_input input').val();
                //d.SalesmanRef = $scope.SelectedSalesManRef;
                //d.BeginDate = $scope.start;
                //d.EndDate = $scope.end;
            }, 
            complete: function (xhr, textStatus) {
                $.LoadingOverlay("hide");

                //$('#entry-grid').dataTable({
                //    "dom": "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
                //        "<'row'<'col-sm-12'tr>>" +
                //        "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
                //});
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status == 400)
                    toastr.error(xhr.responseJSON.Message);
                else
                    toastr.error("Listeleme işlemi yapılırken bir hata oluştu!");
            }
        })
            .withDOM('lBfrtip')
            //.withDOM('&lt;\'row\'&lt;\'col-xs-6\'l&gt;&lt;\'col-xs-6\'f&gt;r&gt;t&lt;\'row\'&lt;\'col-xs-6\'i&gt;&lt;\'col-xs-6\'p&gt;&gt;')
            //.withBootstrap()
            //.withBootstrapOptions({
            //    TableTools: {
            //        classes: {
            //            container: 'btn-group',
            //            buttons: {
            //                normal: 'btn btn-danger'
            //            }
            //        }
            //    },
            //    pagination: {
            //        classes: {
            //            ul: 'pagination pagination-sm'
            //        }
            //    }
            //})
            .withPaginationType('full_numbers')
            .withOption('order', [1, 'asc'])
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
                '5': { type: 'text' },
                '6': { type: 'text' },
                '7': { type: 'text' },
                '8': { type: 'text' },
                '9': { type: 'text' }
            })
            .withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/Turkish.json');

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

    $scope.Init = function (firmID) {
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
