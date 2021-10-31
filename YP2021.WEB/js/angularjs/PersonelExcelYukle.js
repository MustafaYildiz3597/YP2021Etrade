var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.light-columnfilter', 'ngFileUpload']);

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
    $scope.isDisabled = true;
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



    $scope.FormModel = { TCKN: null, Ad: null, Soyad: null, KepAdresi:null };

    $scope.SelectFile = function (file) {
        $scope.SelectedFile = file;
    };
    $scope.Upload = function () {

        if ($scope.SelectedFile === null || $scope.SelectedFile === undefined) {
            toastr.error("Dosya seçmelisiniz.");
            return;
        }

        $.LoadingOverlay("show");

        var regex = /^.*\.(xls|xlsx|csv)$/; // // /^([a-zA-Z0-9_ğüşıöçĞÜŞIİÖÇ\s_\\.\-:])+(.xls|.xlsx)$/;
        if (regex.test($scope.SelectedFile.name.toLowerCase())) {
            if (typeof (FileReader) !== "undefined") {
                var reader = new FileReader();
                //For Browsers other than IE.
                if (reader.readAsBinaryString) {
                    reader.onload = function (e) {
                        $scope.ProcessExcel(e.target.result);
                    };
                    reader.readAsBinaryString($scope.SelectedFile);
                } else {
                    //For IE Browser.
                    reader.onload = function (e) {
                        var data = "";
                        var bytes = new Uint8Array(e.target.result);
                        for (var i = 0; i < bytes.byteLength; i++) {
                            data += String.fromCharCode(bytes[i]);
                        }
                        $scope.ProcessExcel(data);
                    };
                    reader.readAsArrayBuffer($scope.SelectedFile);
                }
            } else {
                toastr.error("Tarayınızın HTML5 desteği bulunmadığından yükleme işlemi yapılamıyor.");
                // $window.alert("This browser does not support HTML5.");
            }
        } else {
            toastr.error("Yüklemek istediğiniz dosya geçerli bir Excel dosyası değil.");
            // $window.alert("Please upload a valid Excel file.");
        }

        $.LoadingOverlay("hide");

    };

    $scope.ProcessExcel = function (data) {
        //Read the Excel File data.
        //$scope.Workbook = XLSX.read(data, {
        //    type: 'binary'
        //});

        //Fetch the name of First Sheet.
        //var firstSheet = workbook.SheetNames[0];

        //Read all rows from First Sheet into an JSON array.
        // var excelRows = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[firstSheet]);

        //Display the data from Excel file in Table.
        $scope.$apply(function () {

            //var workbook = XLSX.readFile('100264.xlsx', { type: 'binary', cellDates: true, dateNF: 'mm/dd/yyyy;@' });

            $scope.Workbook = XLSX.read(data, {
                type: 'binary',
                cellDates: true,
                dateNF: 'dd/mm/yyyy'
            });

            $scope.SheetNames = $scope.Workbook.SheetNames; 
            // $scope.Customers = excelRows;
            // $scope.IsVisible = true;
        });
    };

    $scope.GetSheetData = function () {
        var sheetname = $scope.SelectedSheet;
        $scope.excelRows = XLSX.utils.sheet_to_row_object_array($scope.Workbook.Sheets[sheetname]);

        $scope.ColumnNames = [];
        $scope.FormModel.Ad = null;
        $scope.FormModel.Soyad = null;
        $scope.FormModel.TCKN = null;
        $scope.FormModel.KepAdresi = null;
        $scope.isDisabled = true;

        angular.forEach($scope.excelRows, function (value, key) {
            if (Object.keys($scope.excelRows[key]).length > $scope.ColumnNames.length)
                $scope.ColumnNames = Object.keys($scope.excelRows[key]);
        });
    }

    $scope.StartTransferFromExcel = function () {

        // check controls

        if ($scope.FirmID === null) {
            toastr.error("Firma seçmelisiniz.");
            return false;
        }

        if ($scope.FormModel.Ad === $scope.FormModel.Soyad || $scope.FormModel.Ad === $scope.FormModel.TCKN || $scope.FormModel.Ad === $scope.FormModel.GSMNumber || $scope.FormModel.Ad === $scope.FormModel.Birim || $scope.FormModel.Ad === $scope.FormModel.EMailAddress || $scope.FormModel.Ad === $scope.FormModel.BeginDate  ) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        if ($scope.FormModel.Soyad === $scope.FormModel.TCKN || $scope.FormModel.Soyad === $scope.FormModel.GSMNumber || $scope.FormModel.Soyad === $scope.FormModel.Birim || $scope.FormModel.Soyad === $scope.FormModel.EMailAddress || $scope.FormModel.Soyad === $scope.FormModel.BeginDate ) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        if ($scope.FormModel.TCKN === $scope.FormModel.GSMNumber || $scope.FormModel.TCKN === $scope.FormModel.Birim || $scope.FormModel.TCKN === $scope.FormModel.EMailAddress || $scope.FormModel.TCKN === $scope.FormModel.BeginDate ) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        if ($scope.FormModel.GSMNumber === $scope.FormModel.Birim || $scope.FormModel.GSMNumber === $scope.FormModel.EMailAddress || $scope.FormModel.GSMNumber === $scope.FormModel.BeginDate) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        if ($scope.FormModel.Birim === $scope.FormModel.EMailAddress || $scope.FormModel.Birim === $scope.FormModel.BeginDate) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        if ($scope.FormModel.EMailAddress === $scope.FormModel.BeginDate) {
            toastr.error("Mükerrer seçim hatası.");
            return false;
        }
        // check controls end

        $.LoadingOverlay("show");

        var dt = {
            "ExcelRows": [],
            "FirmID": $scope.FirmID,
            "FileName": $scope.SelectedFile.name.toLowerCase()
        };  

        angular.forEach($scope.excelRows, function (row) {
            dt.ExcelRows.push({
                "FirstName": row[$scope.FormModel.Ad],
                "LastName": row[$scope.FormModel.Soyad],
                "TCIdentityNo": row[$scope.FormModel.TCKN],
                //"KepAddress": row[$scope.FormModel.KepAdresi],
                "GSMNumber": row[$scope.FormModel.GSMNumber],
                "Birim": row[$scope.FormModel.Birim],
                "EMailAddress": row[$scope.FormModel.EMailAddress],
                "BeginDate": row[$scope.FormModel.BeginDate]
            });
        });     

        $http({
            url: "/api/Employee/TransferFromExcel/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            toastr.success(response.data);
            $.LoadingOverlay("hide");

            $scope.Saved = true;

            //$scope.Listele();

            //debugger;
            //$scope.dtInstance.rerender();


        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });
         
    }

    $scope.cmbChanged = function () {
        $scope.isDisabled = ($scope.FirmID === "" || $scope.FirmID === null || $scope.FirmID === undefined ||  $scope.FormModel.TCKN === undefined || $scope.FormModel.Soyad === undefined || $scope.FormModel.Ad === undefined ||  $scope.FormModel.TCKN === null || $scope.FormModel.Soyad === null || $scope.FormModel.Ad === null);
    }
     

    $scope.Saved = false;

    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $.LoadingOverlay("show");

        $http({
            url: "/api/Employee/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

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
        $scope.FormModel = { "ID": "", "FirmID": $scope.FirmID};

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
                "ID": id
            };
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/Employee/Detail/",
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
            DTColumnBuilder.newColumn("FirstName", "Adı"),
            DTColumnBuilder.newColumn("LastName", "Soyadı"),
            DTColumnBuilder.newColumn("TCIdentityNo", "TCKN"),
            DTColumnBuilder.newColumn("KEPAddress", "KEP E-Posta"), //.renderWith(renderNum).withClass('numericCol'),
            DTColumnBuilder.newColumn("SentPayrollsCount", "Top.Gönd.Bordro#"), //.renderWith(renderNum).withClass('numericCol'),
            DTColumnBuilder.newColumn("KEPStatus", "KEP Durumu"), //.renderWith(renderNum).withClass('numericCol'),
            DTColumnBuilder.newColumn("CreateDT", "Kayıt Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("CreateFullName", "Kaydeden"),
            DTColumnBuilder.newColumn("ID", "Detay").renderWith(renderEdit)
        ];

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Employee/DTList/",
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
            .then(function (response) {

                $scope.SelectedPeriod = "01";

                $scope.CmbFirms = response.data.Firms;

                if (response.data.Firms.length > 0) {

                    $scope.Periods = response.data.Periods;
                    $scope.CmbPeriods = $scope.Periods.filter(function (item) {
                        return (item.FIRMNR === $scope.SelectedFirm);
                    });

                    //if ($scope.CmbPeriods.length > 0)
                    //    $scope.SelectedPeriod = $scope.CmbPeriods[0].NR;

                    $scope.SalesMans = response.data.SalesMans;
                    $scope.CmbSalesMans = $scope.SalesMans.filter(function (item) {
                        return (item.FIRMNR === $scope.SelectedFirm);
                    });

                    $scope.SelectedSalesManRef = "";

                    //if ($scope.CmbSalesMans.length > 0)
                    //    $scope.SelectedSalesMan = $scope.CmbSalesMans[0].CODE;

                    // $scope.Listele();

                    $.LoadingOverlay("hide");
                }
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };

    }

    $scope.FillControls = function () {

        $.LoadingOverlay("show");

        setTimeout(function () {

            $scope.SavedMessage = "";

            $scope.FormModel = {};

            var jsondata = {};
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/Firm/CmbList/",
                method: "POST",
                params: {},
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        //$scope.Firms = [{ID:"", "Title": "Seçiniz"}];
                        //$scope.Firms = $scope.Firms.concat(response.data);

                        $scope.Firms = response.data;

                        $.LoadingOverlay("hide");

                        $scope.$apply();
                    }, 2000);
                }
                    , function (error) {

                        $window.setTimeout(function () {

                            toastr.error(error.data.Message);
                            $.LoadingOverlay("hide");

                            $scope.$apply();

                        }, 1000);
                    });
        }, 500);

    }

    $scope.Init = function (firmID) {

        //$.LoadingOverlay("show");
        //$scope.FillAllCmbs();

        if (firmID === "" || firmID === null) {
            $scope.FirmID = null;
            $scope.FillControls();
        }
        else {
            $scope.FirmID = firmID;
        }

        // $scope.Listele();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

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
