var app = angular.module('NeroApp', ['ngSanitize', 'ngCookies', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter', 'angularUtils.directives.dirPagination', 'ui.select']);

app.config(['$provide', function ($provide) {
    $provide.decorator('$locale', ['$delegate', function ($delegate) {
        $delegate.NUMBER_FORMATS.DECIMAL_SEP = ',';
        $delegate.NUMBER_FORMATS.GROUP_SEP = '.';
        return $delegate;
    }]);
}]);

app.filter('propsFilter', function () {
    return function (items, props) {
        var out = [];

        if (angular.isArray(items)) {
            var keys = Object.keys(props);

            items.forEach(function (item) {
                var itemMatches = false;

                for (var i = 0; i < keys.length; i++) {
                    var prop = keys[i];
                    var text = props[prop].toLowerCase();
                    if ((item[prop] || '').toString().toLowerCase().indexOf(text) !== -1) {
                        itemMatches = true;
                        break;
                    }
                }

                if (itemMatches) {
                    out.push(item);
                }
            });
        } else {
            // Let the output be the input untouched
            out = items;
        }

        return out;
    };
});
 
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

app.factory("userPersistenceService", [
    "$cookies", function ($cookies) {
        //var userName = "";
        return {
            setCookieData: function (key, value) {
                $cookies.put(key, value);
            },
            getCookieData: function (key) {
                return $cookies.get(key);
            },
            clearCookieData: function (key) {
                $cookies.remove(key);
            }
        }
    }
]);

function NeroCtrl($scope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder, userPersistenceService) {

    $scope.SetCookies = function () {
        userPersistenceService.setCookieData("filtertu", JSON.stringify($scope.FilterFormModel))
    };
    $scope.GetCookies = function () {
        var filterjson = userPersistenceService.getCookieData('filtertu');
        if (filterjson != null)
            $scope.FilterFormModel = JSON.parse(filterjson);
    };
    $scope.ClearCookies = function () {
        userPersistenceService.clearCookieData('filtertu');
    };

    $scope.FilterFormModel = {};
    $scope.FilterFormModel.selectedFirms = null;
    $scope.FilterFormModel.ProductSearchText = null;

    /* ui-select*/
    $scope.multipleDemo = {};
    $scope.multipleDemo.selectedFirms = null;
    $scope.multipleDemo.selectedFirmsGrdAdd = null;
    //$scope.FilterFormModel = {};

    $scope.UploadExcelForm = {};
    $scope.alert = false;
    $scope.checkall = true;

    //toastr.options = {
    //    "closeButton": true,
    //    "debug": false,
    //    "newestOnTop": false,
    //    "progressBar": false,
    //    "positionClass": "toast-top-full-width",
    //    "preventDuplicates": true,
    //    "showDuration": "300",
    //    "hideDuration": "1000",
    //    "timeOut": 0,
    //    "extendedTimeOut": 0,
    //    "showEasing": "swing",
    //    "hideEasing": "linear",
    //    "showMethod": "fadeIn",
    //    "hideMethod": "fadeOut",
    //    "tapToDismiss": false
    //};


    $scope.isDisabled = true;
    //$scope.emailFormat = /^[a-z]+[a-z0-9._]+@[a-z0-9]+\.[a-z.]{2,5}$/;
    $scope.emailFormat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
   
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

    //var today = new Date();
    //var today7 = new Date();
    //today7.setDate(today.getDate() - 7);

    //var beginDate = formattedDate(today7); //dateFilter(today7, 'dd.MM.yyyy'); // dateFilter(new Date, 'dd/MM/yyyy');
    //var endDate = formattedDate(today); // dateFilter(today, 'dd.MM.yyyy');

    //$scope.selected = [{ NodeGroupID: "", BeginDate: beginDate, EndDate: endDate }];
    //$scope.selected.BeginDate = beginDate;
    //$scope.selected.EndDate = endDate;


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
     

    $scope.FormModel = {};

     
    $scope.cmbChanged = function () {
        $scope.isDisabled = ($scope.FormModel.BUKOD === "" || $scope.FormModel.BUKOD === null || $scope.FormModel.BUKOD === undefined ||
            $scope.FormModel.NAME === "" || $scope.FormModel.NAME === null || $scope.FormModel.NAME === undefined);
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
                    angular.lowercase($scope.exceldata[key].NAME || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1 ||
                    angular.lowercase($scope.exceldata[key].NAME_EN || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1
                ) {
                    $scope.exceldata[key].isSelected = true;
                }
                //}
            });
        }

        $scope.checkall = c;

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


     


    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        
        $.LoadingOverlay("show");

        $http({
            url: "/api/TProducts/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.CurrencyCode = $scope.Currencies[$scope.Currencies.findIndex(x => x.ID === $scope.FormModel.CURRENCY)].Code;
            $scope.FormModel.FIRMA_ADI = $scope.Tedarikciler[$scope.Tedarikciler.findIndex(x => x.ID === $scope.FormModel.MusteriID)].FIRMA_ADI;

            var table = $('#entry-grid').DataTable();

            if ($scope.FormModel.TPID == null) {
                $scope.FormModel.TPID = response.data.NewID;
                $scope.FormModel.CreatedOn = formattedDate(response.data.TRNDate);
                table.rows.add($scope.FormModel).draw('page');
            } else {
                //var idx = $scope.TUList.findIndex(x => x.TPID == $scope.FormModel.TPID);
                //$scope.TUList[idx] = angular.copy($scope.FormModel);

                $scope.FormModel.CreatedOn = formattedDate($scope.FormModel.CreatedOn);
                $scope.FormModel.UPDATED = formattedDate(response.data.TRNDate);

                var rowindex = -1;
                table
                    .rows(function (idx, data, node) {
                        if (data.TPID === $scope.FormModel.TPID) {
                            rowindex = idx;
                            return;
                        }
                    });

                table
                    .row(rowindex)
                    .data($scope.FormModel)
                    .draw('page');
            }

            $window.setTimeout(function () {

                $.magnificPopup.close();

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);

                $scope.$apply();

            }, 500);


        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });

    };

    $scope.AddItem = function () {

        $scope.Saved = false;

        $scope.SavedMessage = "";
        $scope.FormModel = { "TPID": null };
        $scope.TuTitle = "Yeni kayıt";

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $.magnificPopup.open({
            items: {
                src: $('#modalTProductEditForm')
            },
            type: 'inline',
            modal: true,
            showCloseBtn: true
        });

        $.LoadingOverlay("hide");

        // $scope.ShowingDiv = "detail";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();

        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/TProducts/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.TProductDetail;

                    $scope.TuTitle = ($scope.FormModel.BUKOD || "") + " " + ($scope.FormModel.NAME || "");


                    $.magnificPopup.open({
                        items: {
                            src: $('#modalTProductEditForm')
                        },
                        type: 'inline',
                        closeOnContentClick: false,
                        modal: true,
                        showCloseBtn: true
                    });

                    //$scope.ShowingDiv = "detail";

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

    };

    $scope.View = function (id) {

        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/TProducts/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.TProductDetail;

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalTProductViewForm')
                        },
                        type: 'inline'
                    });

                    //$scope.ShowingDiv = "detail";

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

    $scope.ShowDeletePopup = function (id) {

        var table = $('#entry-grid').DataTable();

        $scope.DeletingRow = table
            .rows(function (idx, data, node) {
                return data.TPID === id ? true : false;
            })
            .data();

        //var idx = $scope.TUList.findIndex(x => x.TPID == id);
        //$scope.TUList[idx] = angular.copy($scope.FormModel);

        $scope.DeletingTProductName = ($scope.DeletingRow[0].BUKOD || "");  //($scope.TUList[idx].BUKOD || "");
        $scope.DeleteTProductFormModel = { "ID": id };

        $.magnificPopup.open({
            items: {
                src: $('#modalTProductDeleteForm')
            },
            type: 'inline'
        });

    }
      
    $scope.deleteTProduct = function () {

        $("#modalTProductDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/TProducts/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteTProductFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data.Message);

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.TPID === $scope.DeleteTProductFormModel.ID) {
                            rowindex = idx;
                            return;
                        }
                    })
                    .data();

                var d = table.row(rowindex).remove().draw('page');

                //var idx = $scope.TUList.findIndex(x => x.TPID == $scope.DeleteTProductFormModel.ID);
                //$scope.TUList.splice(idx, 1);

                $("#modalTProductDeleteForm").LoadingOverlay("hide");
                $('modalTProductDeleteForm').magnificPopup('close');

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalTProductDeleteForm").LoadingOverlay("hide");
            }, 1000);
        });
    }



    $scope.DeleteRowGridAdd = function (idx) {
        $scope.GridAddModel.splice(idx, 1);
    }

    $scope.UpdateGridEdit = function () {

        var isvalid = true;
        angular.forEach($scope.GridEditModel, function (value, key) {

            if (($scope.GridEditModel[key].XPSNO || "") == "" && ($scope.GridEditModel[key].BUKOD || "") == "") {
                isvalid = false;
                return;
            }

            if (($scope.GridEditModel[key].NAME || "") == "" || ($scope.GridEditModel[key].PRICE || "") == "" || ($scope.GridEditModel[key].CURRENCY || "") == "") {
                isvalid = false;
                return;
            }
        });

        if (isvalid == false) {
            toastr.warning("Ürün bilgileri geçersiz veya eksik!");
            return;
        }

        $("#modalTUGridEditPopup").LoadingOverlay("show");

        $http({
            url: "/api/TProducts/SaveGridEdit/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: { "Items": $scope.GridEditModel }
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                $scope.TotalCount = response.data.TotalCount;
                $scope.SuccededCount = response.data.SuccededCount;
                $scope.ReturnObj = response.data.StrObj;

                $scope.alert = true;

                const today = moment();

                angular.forEach($scope.GridEditModel, function (value, key) {

                    var table = $('#entry-grid').DataTable();
                    var rowindex = -1;
                    var row = table.rows(function (idx, data, node) {

                        if ($scope.GridEditModel[key].TPID == data.TPID) {
                            rowindex = idx;
                            return
                        }

                    }).data();

                    var d = table.row(rowindex).data();
                    $scope.GridEditModel[key].UPDATED = today.format('YYYY-MM-DD HH:mm:ss');
                    d = $scope.GridEditModel[key];

                    table
                        .row(rowindex)
                        .data(d)
                        .draw('page');

                    //var idx = $scope.TUList.findIndex(x => x.TPID === $scope.GridEditModel[key].TPID);
                    //$scope.GridEditModel[key].UPDATED = today.format('YYYY-MM-DD HH:mm:ss');
                    //$scope.TUList[idx] = angular.copy($scope.GridEditModel[key]);

                });

                $("#modalTUGridEditPopup").LoadingOverlay("hide");

                // $.magnificPopup.close();

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalTUGridEditPopup").LoadingOverlay("hide");
            }, 1000);

        });
    }

    $scope.CurrencyChanged = function (tpid, currency) {
        var idx = $scope.GridEditModel.findIndex(x => x.TPID == tpid);
        $scope.GridEditModel[idx].CurrencyCode = $scope.Currencies[$scope.Currencies.findIndex(x => x.ID === currency)].Code;
    }

    $scope.CurrencyChangedGridADD = function (tpid, currency) {
        var idx = $scope.GridAddModel.findIndex(x => x.TPID == tpid);
        $scope.GridAddModel[idx].CurrencyCode = $scope.Currencies[$scope.Currencies.findIndex(x => x.ID === currency)].Code;
    }

    $scope.ClosePopup = function () {
        $scope.alert = false;
        $.magnificPopup.close();
    }

    $scope.BrowseBUKOD = function (mode, idx) {

        $scope.SelProduct = null;
        $scope.SelMode = mode;
        $scope.SelIdx = idx;

        //$scope.Products = angular.copy($scope.ProductsTemp);
        angular.forEach($scope.Products, function (value, key) {

            if ($scope.Products[key].chk == true)
                $scope.Products[key].chk = false;
        });     


        $.magnificPopup.open({
            items: {
                src: $('#modalBrowseBUKODPopup')
            },
            type: 'inline',
            modal: true
        });
    }
    $scope.CloseBrowseBUKOD = function () {

        var inspectorname = "";
        if ($scope.SelMode == 's' || $scope.SelMode == 'm') {
            inspectorname = "#modalTUGridAddPopup";
        }
        else if ($scope.SelMode == 'e') {
            inspectorname = "#modalTProductEditForm";
        } else {
            return;
        }

        $.magnificPopup.open({
            items: {
                src: $(inspectorname)
            },
            type: 'inline',
            closeOnContentClick: false,
            modal: true,
            showCloseBtn: true
        });
    }

    $scope.AddSelectedBUKOD = function () {

        if ($scope.SelProduct == null) {
            toastr.warning("Ürün seçmelisiniz!");
            return;
        }

        var prodID = $scope.SelProduct;
        var product = $scope.Products.filter(item => item.PRODID == prodID);
        if (product.length == 0) {
            toastr.warning("Ürün kaydına ulaşılamadı!");
            return;
        }

        // check selected product in other rows
        var returnfunc = false;
        angular.forEach($scope.GridAddModel, function (value, key) {
            if (key != $scope.SelIdx && $scope.GridAddModel[key].ProductID == product[0].PRODID) {
                toastr.warning("Seçtiğiniz ürün zaten listede var!");
                returnfunc = true;
                return;
            }
        });

        if (returnfunc == true) {
            return;
        }

        var idx = $scope.SelIdx;
        $scope.GridAddModel[idx].BUKOD = product[0].BUKOD;
        $scope.GridAddModel[idx].ProductID = product[0].PRODID;
        $scope.GridAddModel[idx].NAME = product[0].NAME;

        $.magnificPopup.open({
            items: {
                src: $('#modalTUGridAddPopup')
            },
            type: 'inline',
            closeOnContentClick: false,
            modal: true
        });
    }

    $scope.AddBUKODToEditPage = function () {

        if ($scope.SelProduct == null) {
            toastr.warning("Ürün seçmelisiniz!");
            return;
        }

        var prodID = $scope.SelProduct;
        var product = $scope.Products.filter(item => item.PRODID == prodID);
        if (product.length == 0) {
            toastr.warning("Ürün kaydına ulaşılamadı!");
            return;
        }

        $scope.FormModel.BUKOD = product[0].BUKOD;
        $scope.FormModel.ProductID = product[0].PRODID;
        $scope.FormModel.ProductName = product[0].NAME;

        $.magnificPopup.open({
            items: {
                src: $('#modalTProductEditForm')
            },
            type: 'inline',
            closeOnContentClick: false,
            modal: true
        });
    }

    $scope.AddMultiBUKODToAddGrid = function () {

        /* for Multiple selecting */

        // alert($scope.SelProduct); // tekli seçim için lazım

        var selectedrows = $scope.Products.filter(item => item.chk === true);

        if (selectedrows.length === 0) {
            toastr.error("En az bir ürün seçmelisiniz!");
            return;
        }

        var addingcount = 0;

        angular.forEach(selectedrows, function (value, key) {

            var grdrow = $scope.GridAddModel.filter(item => item.ProductID == selectedrows[key].PRODID);

            if (grdrow.length == 0) {

                $scope.gridaddrowcount += -1;

                $scope.GridAddModel.push({
                    "TPID": $scope.gridaddrowcount,
                    "BUKOD": selectedrows[key].BUKOD,
                    "ProductID": selectedrows[key].PRODID,
                    "FIRMA_ADI": null,
                    "NAME": selectedrows[key].NAME,
                    "CreatedOn": null,
                    "UPDATED": null,
                    "PRICE": null,
                    "CURRENCY": null,
                    "CurrencyCode": null
                });

                addingcount++;
            }
        });

        toastr.info(addingcount + " adet ürün listeye eklendi.");

        $.magnificPopup.open({
            items: {
                src: $('#modalTUGridAddPopup')
            },
            type: 'inline',
            closeOnContentClick: false,
            modal: true
        });

    }

    $scope.AddRowTUGridAdd = function () {

        $scope.gridaddrowcount += -1;

        $scope.GridAddModel.push({
            "TPID": $scope.gridaddrowcount,
            "BUKOD": null,
            "ProductID": null,
            "FIRMA_ADI": null,
            "NAME": null,
            "CreatedOn": null,
            "UPDATED": null,
            "PRICE": null,
            "CURRENCY": null,
            "CurrencyCode": null
        });
    }

    $scope.InsertRowsToTUListGridAdd = function () {

        /* check firm exists */
        if ($scope.multipleDemo.selectedFirmsGrdAdd == null) {
            toastr.warning("Tedarikçi seçmelisiniz!");
            return;
        }
        else {
            if ($scope.multipleDemo.selectedFirmsGrdAdd.length == 0) {
                toastr.warning("Tedarikçi seçmelisiniz!");
                return;
            }
        }

        /* check product exists */
        if ($scope.GridAddModel.length == 0) {
            toastr.warning("Ürün eklemelisiniz!");
            return;
        }

        var isvalid = true;
        angular.forEach($scope.GridAddModel, function (value, key) {

            if (($scope.GridAddModel[key].XPSNO || "") == "" && ($scope.GridAddModel[key].BUKOD || "") == "") {
                isvalid = false;
                return;
            }

            if (($scope.GridAddModel[key].NAME || "") == "" || ($scope.GridAddModel[key].PRICE || "") == "" || ($scope.GridAddModel[key].CURRENCY || "") == "") {
                isvalid = false;
                return;
            }
        });

        if (isvalid == false) {
            toastr.warning("Ürün bilgileri geçersiz veya eksik!");
            return;
        }

        $("#modalTUGridAddPopup").LoadingOverlay("show");

        var musterilerRQ = [];
        musterilerRQ.push($scope.multipleDemo.selectedFirmsGrdAdd);

        var jsondata = {
            Products: $scope.GridAddModel,
            Tedarikciler: musterilerRQ // $scope.multipleDemo.selectedFirms
        };

        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/TProducts/SaveGridAdd/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                $scope.TotalCount = response.data.TotalCount;
                $scope.SuccededCount = response.data.SuccededCount;
                $scope.ReturnObj = response.data.StrObj;

                $scope.alert = true;

                $scope.Filtrele();

                //const today = moment();

                //angular.forEach($scope.GridAddModel, function (value, key) {
                //    var idx = $scope.ReturnObj.findIndex(x => x.ID == $scope.GridAddModel[key].MPID);
                //    if (idx == -1) { // sonuç listesinde yok ise  başarılı olanlar listeye eklenir.
                //        $scope.GridAddModel[key].CreatedOn = today.format('YYYY-MM-DD');
                //        $scope.GridAddModel[key].FIRMA_ADI = musterilerRQ[0].FIRMA_ADI;
                //        $scope.MUList.push($scope.GridAddModel[key]);
                //    }
                //});

                $("#modalTUGridAddPopup").LoadingOverlay("hide");

                // $.magnificPopup.close();

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalTUGridAddPopup").LoadingOverlay("hide");
            }, 1000);

        });




    }



    $scope.ShowTUGridAddPopup = function () {

        $scope.gridaddrowcount = -1;
        $scope.multipleDemo.selectedFirmsGrdAdd = null;
        $scope.alert = false;

        $scope.GridAddModel = [
            {
                "TPID": $scope.gridaddrowcount,
                "BUKOD": null,
                "ProductID": null,
                "FIRMA_ADI": null,
                "NAME": null,
                "CreatedOn": null,
                "UPDATED": null,
                "PRICE": null,
                "CURRENCY": null,
                "CurrencyCode": null
            }];

        $.magnificPopup.open({
            items: {
                src: $('#modalTUGridAddPopup')
            },
            type: 'inline',
            closeOnContentClick: false,
            modal: true
        });

    }

    $scope.ShowTUGridEditPopup = function () {

        // $scope.GridEditModel = angular.copy($scope.TUList.filter(item => item == $scope.q ));

        $scope.alert = false;
        $scope.GridEditModel = [];
        var table = $('#entry-grid').DataTable();
        table.rows(function (idx, data, node) {
            // return node.childNodes[0].firstChild.checked == true;
            if (node.childNodes[0].firstChild.checked == true)
                $scope.GridEditModel.push(data);
        });

        if ($scope.GridEditModel.length == 0) {
            toastr.error("Seçim yapmalısınız.");
            return;
        }

        //$scope.GridEditModel = angular.copy($scope.tufiltered.filter(item => item.chk === true));

        $.magnificPopup.open({
            items: {
                src: $('#modalTUGridEditPopup')
            },
            type: 'inline',
            modal: true
        });

    }


    //$scope.currentPage = 1;
    $scope.currentPage2 = 1;
    //$scope.pageSize = 10;
    $scope.pageSize2 = 10;
    $scope.pagesSizes = [10, 25, 50, 100];

    //$scope.sort = function (keyname) {
    //    $scope.sortBy = keyname;   //set the sortBy to the param passed
    //    $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    //}
    $scope.sort2 = function (keyname) {
        $scope.sortBy2 = keyname;   //set the sortBy to the param passed
        $scope.reverse2 = !$scope.reverse2; //if true make it false and vice versa
    }

    $scope.Search = function (isvalid) {

        if (!isvalid) {
            toastr.warning("Arama kriterleri hatalı!");
            return;
        }

        //$scope.Listele();
        //$scope.dtInstance.rerender();
    }

    function renderDate(data) {
        if (data === null)
            return "";

        return formattedDate(data);
    }

    $scope.Filtrele = function () {

        $.LoadingOverlay("show");

        $scope.SetCookies();

        var table = $('#entry-grid').DataTable();

        table
            .clear()
            .draw();

        $("#chkAll").prop("checked", false);
        $scope.selectAll = false;

        //$scope.dtInstance.rerender();
    }

    $scope.selectAll = false;

    var titleHtml = '<input type="checkbox" id="chkAll" onclick="angular.element(this).scope().toggleAll()">';

    $scope.toggleAll = function () {
        var table = $('#entry-grid').DataTable();
        $scope.selectAll = !$scope.selectAll;
        table
            .rows(function (idx, data, node) {
                node.childNodes[0].firstChild.checked = $scope.selectAll;
            });
    }
    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye text-white\"></i></a>";
    }
    function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt text-white\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' class='sil-btn-round' onclick='angular.element(this).scope().ShowDeletePopup(" + data + ")'><i class=\"fas fa-trash-alt text-white\"></i></a>";
    }

    $scope['ListeleDT'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn(null).withOption('width', '30px').withTitle(titleHtml).notSortable()
                .renderWith(function (data, type, full, meta) {
                    return '<input type="checkbox" />';
                }),
            DTColumnBuilder.newColumn("BUKOD", "BU Numarası").withOption('width', '200px'),
            DTColumnBuilder.newColumn("FIRMA_ADI", "Tedarikçi Ünvan").withOption('width', '200px'),
            DTColumnBuilder.newColumn("XPSNO", "Tedarikçi No").withOption('width', '200px'),
            DTColumnBuilder.newColumn("NAME", "Ürün Adı").withOption('width', '200px'),
            DTColumnBuilder.newColumn("CreatedOn", "Eklenme Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("UPDATED", "GÜncelleme Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("PRICE", "Fiyat").withClass('text-right').renderWith(function (data, type, full) { return $filter('currency')(data, '', 2); }),
            DTColumnBuilder.newColumn("CurrencyCode", "P.B.").withOption('width', '30px')
        ];

        //    DTColumnBuilder.newColumn("ENABLED", "Durum").withOption('width', '10px').renderWith(renderStatus).withClass('text-center')];
        //DTColumnBuilder.newColumn("ProductName", "Ürün Adı").withOption('width', '200px'),

        if ($scope.Permission.View == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TPID", "Görüntüle").withOption('width', '10px').renderWith(renderView).withClass('text-center').notSortable());
        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TPID", "Düzenle").withOption('width', '10px').renderWith(renderEdit).withClass('text-center').notSortable());
        if ($scope.Permission.Delete == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TPID", "Sil").withOption('width', '10px').renderWith(renderDelete).withClass('text-center').notSortable());

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/TProducts/DTList/",
            type: "POST",
            dataType: "json",
            dataSrc: "data",
            data: function (d) {
                //d.FirmCode = $scope.SelectedFirm;// $('.daterangepicker_start_input input').val();
                //d.PeriodCode = $scope.SelectedPeriod; ////$('.daterangepicker_end_input input').val();
                //d.SalesmanRef = $scope.SelectedSalesManRef;
                //d.BeginDate = $scope.start;
                //d.EndDate = $scope.end;

                d.TedarikciID = $scope.FilterFormModel.selectedFirms == null ? null : $scope.FilterFormModel.selectedFirms.ID,
                    d.BUKOD = $scope.FilterFormModel.ProductSearchText
            },
            complete: function (xhr, textStatus) {

                $("#chkAll").prop("checked", false);
                $scope.selectAll = false;

                $.LoadingOverlay("hide");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status == 400)
                    toastr.error(xhr.responseJSON.Message);
                else
                    toastr.error("Listeleme işlemi yapılırken bir hata oluştu!");

                $.LoadingOverlay("hide");
            }
        })
            .withDOM('lBfrtip')
            .withOption('order', [1, 'asc'])
            .withOption('processing', true)
            .withOption('serverSide', true)
            .withOption('paging', true)
            .withPaginationType('full_numbers')
            .withDisplayLength(10)
            //.withOption('responsive', true)
            .withOption('scrollX', 'auto')
            .withOption('scrollY', '500px')

            .withOption('scrollCollapse', true)
            .withOption('autoWidth', false)
            .withButtons([
                'print',
                'excel'
            ])
            .withLightColumnFilter({
                '1': { type: 'text' },
                '2': { type: 'text' },
                '3': { type: 'text' },
                '4': { type: 'text' },
                '5': { type: 'text' },
                '6': { type: 'text' },
                '7': { type: 'text' },
                '8': { type: 'select', values: [{ value: 'EURO', label: 'EURO' }, { value: 'USD', label: 'USD' }, { value: 'TL', label: 'TL' }] }
            })
            .withLanguage({
                "sEmptyTable": "Tabloda veri yok.",
                "sInfo": " _TOTAL_ kayıttan _START_ - _END_ arası kayıtlar",
                "sInfoEmpty": "Kayıt yok",
                "sInfoFiltered": "( _MAX_ Kayıt İçerisinden Bulunan)",
                "sInfoPostFix": "",
                "sInfoThousands": ",",
                "sLengthMenu": "Sayfada _MENU_ Kayıt Göster",
                "sLoadingRecords": "Yükleniyor...",
                "sProcessing": "İşlem yapılıyor...",
                "sSearch": "Bul:",
                "sZeroRecords": "Eşleşen Kayıt Bulunmadı",
                "oPaginate": {
                    "sFirst": "İlk",
                    "sLast": "Son",
                    "sNext": "Sonraki",
                    "sPrevious": "Önceki"
                },
                "oAria": {
                    "sSortAscending": ": Artan",
                    "sSortDescending": ": Azalan"
                }
            });

        //$scope.dtInstance = {};
    }


    //$scope.Listele = function () {

    //    $.LoadingOverlay("show");

    //    var jsondata = {
    //        "TedarikciID": ($scope.multipleDemo.selectedFirms == null ? null : $scope.multipleDemo.selectedFirms.ID),
    //        "ProductSearchText": $scope.FilterFormModel.ProductSearchText
    //    };
    //    var dt = JSON.stringify(jsondata);

    //    $http({
    //        url: "/api/TProducts/ListDT/"
    //        , method: "POST"
    //        , params: {}
    //        , contentType: "application/json"
    //        , data: dt
    //        , dataType: "json"
    //    })
    //        .then(function (response) {

    //            $scope.TUList = response.data.TUlist;

    //            $scope.sort('BUKOD');
    //            $scope.reverse = false;

    //            //$scope.ShowingDiv = 'Liste';

    //            $.LoadingOverlay("hide");
    //        })
    //        , function (error) {
    //            $.LoadingOverlay("hide");
    //            toastr.error(error.data.Message);
    //        };

    //}

    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/TProducts/ListPageFillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {
                // $scope.Oemsupliers = response.data.Oemsupliers;
                $scope.Tedarikciler = response.data.Tedarikciler;
                // $scope.Tedarikciler = response.data.Tedarikciler;
                $scope.Currencies = response.data.Currencies;
                //$scope.TUList = response.data.TUlist;

                // $scope.ProductsTemp = response.data.Products;
                // $scope.Products = response.data.Products;

                $scope.LoadProducts();

                //$scope.sort('BUKOD')
                //$scope.reverse = false;

                $scope.ShowingDiv = 'Liste';

                $.LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };
    }

    $scope.LoadProducts = function () {
        
        $("#modalBrowseBUKODPopup").LoadingOverlay("show");

        $http({
            url: "/api/Product/BrowseList/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {
                $scope.Products = response.data.Products;
                $scope.sort2('BUKOD')
                $scope.reverse2 = false;
                $("#modalBrowseBUKODPopup").LoadingOverlay("hide");
            }, function (error) {

                $("#modalBrowseBUKODPopup").LoadingOverlay("hide");

                toastr.warning(error.data.Message);    
            });
    }

    $scope.Init = function (v, a, e, d) {

        $scope.GetCookies();
        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d }; 

        $.LoadingOverlay("show");

        $scope.ListeleDT();
        $window.setTimeout(function () {

            $scope.dtInstance = {};
            $scope.$apply();

        }, 200);

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


//function OtherController($scope) {
//    $scope.pageChangeHandler = function (num) {
//        console.log('going to page ' + num);
//    };
//}
//app.controller('OtherController', OtherController);


