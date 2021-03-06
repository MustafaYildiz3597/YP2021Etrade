var app = angular.module('NeroApp', ['ngSanitize', 'scrollable-table', 'angular.filter', 'ngFileUpload']);

function NeroCtrl($scope, $http, $timeout, $window, $filter) {

    $scope.UploadExcelForm = {};
    $scope.alert = false;
    $scope.checkall = true;

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-full-width",
        "preventDuplicates": true,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": 0,
        "extendedTimeOut": 0,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": true
    };


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


    $scope.listFilter = function (row) {

        //if (row.BUKOD == undefined)
        //    return false;

        return (
            //(row.BUKOD != undefined && row.BUKOD.toString().indexOf($scope.oemfiltertext || '') !== -1) ||
            //(row.NAME != undefined && angular.lowercase(row.NAME).toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1) ||
            //(row.NAME_EN != undefined && angular.lowercase(row.NAME_EN).toString().indexOf(angular.lowercase($scope.oemfiltertext) || '')!== -1)
            ((row.BUKOD || '').toString().indexOf($scope.oemfiltertext || '') !== -1) ||
            (angular.lowercase(row.NAME || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1) ||
            (angular.lowercase(row.NAME_EN || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1)
        );

        //return $scope.oemfiltertext == undefined || $scope.oemfiltertext == "" ||
        //    (item.OEMID != undefined && item.OEMID.indexOf($scope.oemfiltertext) !== -1) ||
        //    (item.OEMNR != undefined && item.OEMNR.indexOf($scope.oemfiltertext) !== -1) ||
        //    item.SUPNAME.indexOf($scope.oemfiltertext) !== -1; 
    }


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

    $scope.ShowingDiv = "SelectExcel";

    function renderNum(data) {
        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        var parts = data.toFixed(2).toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }



    $scope.FormModel = { TCKN: null, Ad: null, Soyad: null, KepAdresi: null };

    $scope.SelectFile = function (file) {
        $scope.SelectedFile = file;
        $scope.Upload();
    };
    $scope.Upload = function () {

        $scope.SelectedSheet = null;
        $scope.FormModel.BUKOD = null;
        $scope.FormModel.NAME = null;
        $scope.FormModel.NAME_EN = null;
        $scope.FormModel.NAME_DE = null;
        $scope.FormModel.NAME_FR = null;


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
        $scope.FormModel.NAME = null;
        $scope.FormModel.NAME_EN = null;
        $scope.FormModel.NAME_DE = null;
        $scope.FormModel.NAME_FR = null;
        $scope.isDisabled = true;

        angular.forEach($scope.excelRows, function (value, key) {
            if (Object.keys($scope.excelRows[key]).length > $scope.ColumnNames.length)
                $scope.ColumnNames = Object.keys($scope.excelRows[key]);
        });
    }

    $scope.StartTransferFromExcel = function () {

        $scope.alert = false;
        $scope.checkall = false;
        //$("#checkAll").attr("checked", false);
        //$("#checkAll").prop("checked", false);

        //$.LoadingOverlay("show");

        var dt = {
            "ExcelRows": [],
            "FirmID": $scope.FirmID,
            "FileName": $scope.SelectedFile.name.toLowerCase()
        };

        angular.forEach($scope.excelRows, function (row) {
            dt.ExcelRows.push({
                // "isSelected": false,
                "BUKOD": row[$scope.FormModel.BUKOD],
                "OEMNR": row[$scope.FormModel.OEMNR],
                "SUPID": row[$scope.FormModel.SUPID]
            });
        });

        $scope.exceldata = dt.ExcelRows;


        $scope.ShowingDiv = "ExcelData";

        //$.LoadingOverlay("hide");
    }


    $scope.cmbChanged = function () {
        $scope.isDisabled = ($scope.FormModel.BUKOD === "" || $scope.FormModel.BUKOD === null || $scope.FormModel.BUKOD === undefined ||
            $scope.FormModel.OEMNR === "" || $scope.FormModel.OEMNR === null || $scope.FormModel.OEMNR === undefined ||
            $scope.FormModel.SUPID === "" || $scope.FormModel.SUPID === null || $scope.FormModel.SUPID === undefined 
            );
    }


    $scope.selectorhideall = function (c) {

       // if $scope.checkall  false ise ve c de false ise aslında c true dan dönüyordur.
        // ayar çekilecek.

        if (c == false) {
            angular.forEach($scope.exceldata, function (value, key) {
                $scope.exceldata[key].isSelected = false;
            });
        } else {

            /*firstly clear all */
            angular.forEach($scope.exceldata, function (value, key) {
                $scope.exceldata[key].isSelected = false;
            });

            angular.forEach($scope.exceldata, function (value, key) {

                //if ($scope.exceldata[key].BUKOD != undefined) {
                if (
                    ($scope.exceldata[key].BUKOD || '').toString().indexOf($scope.oemfiltertext || '') !== -1 ||
                    angular.lowercase($scope.exceldata[key].OEMNR || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1 ||
                    angular.lowercase($scope.exceldata[key].SUPID || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1
                ) {
                    $scope.exceldata[key].isSelected = true;
                }
                //}
            });
        }
        
        $scope.checkall = c;

    }

    $scope.ShowProductImportPopup = function () {
       
        $scope.filtereddata = $scope.exceldata.filter(function (item) {
            return (item.isSelected == true);
        });

        if ($scope.filtereddata.length === 0) {
            toastr.error("Yükleme işlemi için listeden veri seçmelisiniz.");
            return;
        }


        $window.setTimeout(function () {

            $scope.UploadExcelForm.$setPristine();
            $scope.UploadExcelForm.$setUntouched();

            $scope.FormModel = {};

            $.magnificPopup.open({
                items: {
                    src: $('#modalImportPopup')
                },
                type: 'inline'
            });

            $scope.$apply();
        }, 500);
    }

    $scope.Saved = false;


    // error callback
    $scope.errorcallback = function (data, status, headers, config) {

        $.LoadingOverlay("hide");

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

    $scope.criMatchMaincat = function () {
        return function (psecitemD) {
            return psecitemD.SECID === $scope.FormModel.SECID; //criteriaMcat.SECID;
        };
    };
    $scope.criMatchCat = function () {
        return function (pmcatitemD) {
            return pmcatitemD.MCATID === $scope.FormModel.MCATID; //criteriaMcat.SECID;
        };
    };

    $scope.UploadSelectedRows = function (isValid) {

        if (isValid == undefined)
            return;

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        if ($scope.filtereddata.length === 0) {
            toastr.error("Yükleme işlemi için listeden veri seçmelisiniz.");
            return;
        }

        var jsondata = {
            "Data": $scope.filtereddata,
            "Cats": $scope.FormModel
        };
        var dt = JSON.stringify(jsondata);

        $("#modalImportPopup").LoadingOverlay("show");

        $http({
            url: "/api/OEM/ImportExcelUploadData/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                if (response.data.IsSuccess) 
                    toastr.success(response.data.Message);
                else 
                    toastr.error(response.data.Message);

                $scope.TotalCount = response.data.TotalCount;
                $scope.SuccededCount = response.data.SuccededCount;
                $scope.DuplicatedProducts = response.data.StrList;

                // $('.alert').alert();
                $scope.alert = true;

                $("#modalImportPopup").LoadingOverlay("hide");

                $.magnificPopup.close();

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {

                debugger;

                toastr.error(error.data.Message);
                $("#modalImportPopup").LoadingOverlay("hide");
            }, 1000);

        });


    }


    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/Product/ExcelImportFillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {

                $scope.ProductSections = response.data.ProductSections;
                $scope.ProductMainCategories = response.data.ProductMainCategories;
                $scope.ProductCategories = response.data.ProductCategories;

                $.LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };
    }

    $scope.Init = function (firmID) {

        $.LoadingOverlay("show");
        $scope.FillAllCmbs();

        //if (firmID === "" || firmID === null) {
        //    $scope.FirmID = null;
        //    $scope.FillControls();
        //}
        //else {
        //    $scope.FirmID = firmID;
        //}

        //// $scope.Listele();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

    $scope.BackToList = function () {
        //$.LoadingOverlay("show");
        $scope.ShowingDiv = "SelectExcel";

        $scope.alert = false;
        $scope.checkall = true;

        //if ($scope.Saved === true)
        //    $scope.dtInstance.rerender();

        //$scope.Saved = false;

        //$scope.Listele();
    };

    $scope.ClosePopup = function () {
        $.magnificPopup.close();
    }

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
