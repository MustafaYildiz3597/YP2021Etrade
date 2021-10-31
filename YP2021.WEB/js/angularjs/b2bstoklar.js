var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter']);

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

function NeroCtrl($scope, $rootScope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {

    $scope.Permission = [];

    $scope.FilterFormModel = {};
    $scope.FilterFormModel.ProductSectionID = "";
    $scope.FilterFormModel.ProductMCatID = "";
    $scope.FilterFormModel.ProductCatID = "";

    $scope.FormModel = {};

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

    function renderNum0(data) {
        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        var parts = data.toFixed(0).toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }
    

    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $.LoadingOverlay("show");

        $http({
            url: "/api/B2B/StoklarSave/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var table = $('#entry-grid').DataTable();

                if ($scope.FormModel.PRODID == null) {

                    table.rows.add(
                        [{
                            "B2BBasePrice": response.data.Data[0].B2BBasePrice,
                            "CurrencyCode": response.data.Data[0].CurrencyCode,
                            "B2BDiscountedPrice": response.data.Data[0].B2BDiscountedPrice,
                            "B2BIsNewProduct": response.data.Data[0].B2BIsNewProduct,
                            "B2BIsOnSale": response.data.Data[0].B2BIsOnSale,
                            "B2BIsVisibleOnCategoryHomepage": response.data.Data[0].B2BIsVisibleOnCategoryHomepage,
                            "B2BIsVisibleOnHomepage": response.data.Data[0].B2BIsVisibleOnHomepage,
                            "B2BStockAmount": response.data.B2BStockAmount
                        }]).draw('page');
                    //.nodes()
                    //.to$()
                    //.addClass('bg-primary');

                } else {

                    var rowindex = -1;
                    var row = table
                        .rows(function (idx, data, node) {
                            if (data.PRODID === $scope.FormModel.PRODID) {
                                rowindex = idx;
                                return;
                            }
                        })
                        .data();

                    var d = table.row(rowindex).data();

                    d.B2BBasePrice = response.data.Data[0].B2BBasePrice;
                    d.CurrencyCode = response.data.Data[0].CurrencyCode;
                    d.B2BDiscountedPrice = response.data.Data[0].B2BDiscountedPrice;
                    d.B2BIsNewProduct = response.data.Data[0].B2BIsNewProduct;
                    d.B2BIsOnSale = response.data.Data[0].B2BIsOnSale;
                    d.B2BIsVisibleOnCategoryHomepage = response.data.Data[0].B2BIsVisibleOnCategoryHomepage;
                    d.B2BIsVisibleOnHomepage = response.data.Data[0].B2BIsVisibleOnHomepage;
                    d.B2BStockAmount = response.data.Data[0].B2BStockAmount;

                    table
                        .row(rowindex)
                        .data(d)
                        .draw('page');

                }

                $scope.FormModel.PRODID = response.data.ID;

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);
                //$scope.ShowingDiv = "Liste";

                $.magnificPopup.close();
                 
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
        $scope.FormModel = { "MPID": null };
        $scope.MuTitle = "Yeni kayıt";

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $.magnificPopup.open({
            items: {
                src: $('#modalMProductEditForm')
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
            url: "/api/B2B/StockDetail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.ProductDetail;

                    $scope.PopupTitle = "Ürün: " + ($scope.FormModel.BUKOD || "") + " " + ($scope.FormModel.NAME || "");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductEditForm')
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
            url: "/api/B2B/StockDetail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.ProductDetail;

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductViewForm')
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

   
    $scope.deleteMProduct = function () {

        $("#modalMProductDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/MProducts/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteMProductFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data.Message);
                //$scope.dtInstance.rerender();

                var idx = $scope.MUList.findIndex(x => x.MPID == $scope.DeleteMProductFormModel.ID);
                $scope.MUList.splice(idx, 1);

                $("#modalMProductDeleteForm").LoadingOverlay("hide");
                $('modalMProductDeleteForm').magnificPopup('close');

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalMProductDeleteForm").LoadingOverlay("hide");
            }, 1000);
        });
    }
    

      
    $scope.ClosePopup = function () {
        $.magnificPopup.close();
    }
      

    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye text-white\"></i></a>";
    }
    function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt text-white\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' class='sil-btn-round' onclick='angular.element(this).scope().showDeletingPopup(" + data + ")'><i class=\"fas fa-trash-alt text-white\"></i></a>";
    }
    
    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.DeletingProductName = ($scope.DeletingFile[0].NAME || "");
        $scope.DeleteProductFormModel = { "ID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalProductDeleteForm')
            },
            type: 'inline'
        });

    }
    $scope.deleteProduct = function () {

        $("#modalProductDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/B2B/StoklarDelete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteProductFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data.Message);
                // $scope.dtInstance.rerender();

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.PRODID === $scope.DeleteProductFormModel.ID) {
                            rowindex = idx;
                            return;
                        }
                    })
                    .data();

                var d = table.row(rowindex).remove().draw('page');

                $("#modalProductDeleteForm").LoadingOverlay("hide");
                $('modalProductDeleteForm').magnificPopup('close');

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductDeleteForm").LoadingOverlay("hide");
            }, 1000);

        });

    }

    $scope.Filtrele = function () {

        var table = $('#entry-grid').DataTable();

        table
            .clear()
            .draw();

        $scope.dtInstance.rerender();
    }

    function renderBool(data) {
        return data == true ? "<i class=\"fas fa-check fa-2x text-success\"></i>" : "<i class=\"fas fa-times fa-2x text-danger\"></i>";
    }
     

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("NAME", "Ürün Adı"),
            DTColumnBuilder.newColumn("BUKOD", "BU Numarası"),
            DTColumnBuilder.newColumn("B2BBasePrice", "Baz Fiyat").withClass('text-right'),
            DTColumnBuilder.newColumn("B2BDiscountedPrice", "İndirimli Fiyat").withClass('text-right'),
            DTColumnBuilder.newColumn("CurrencyCode", "P.B.").withClass('text-center'),
            DTColumnBuilder.newColumn("B2BIsNewProduct", "Yeni").renderWith(renderBool).withClass('text-center'),
            DTColumnBuilder.newColumn("B2BIsOnSale", "İndirimde").renderWith(renderBool).withClass('text-center'),
            DTColumnBuilder.newColumn("B2BIsVisibleOnCategoryHomepage", "Kat.Anasayfa").renderWith(renderBool).withClass('text-center'),
            DTColumnBuilder.newColumn("B2BIsVisibleOnHomepage", "Anasayfa").renderWith(renderBool).withClass('text-center'),
            DTColumnBuilder.newColumn("B2BStockAmount", "Stok Adet").withClass('text-center')
        ];

        if ($scope.Permission.View == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Görüntüle").renderWith(renderView).withClass('text-center'));
        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Düzenle").renderWith(renderEdit).withClass('text-center'));
        if ($scope.Permission.Delete == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Sil").renderWith(renderDelete).withClass('text-center'));

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/B2B/GetStocks/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                //d.SecID = $scope.FilterFormModel.ProductSectionID,
                //    d.MCatID = $scope.FilterFormModel.ProductMCatID,
                //    d.CatID = $scope.FilterFormModel.ProductCatID,
                //    d.OEMNo = $scope.FilterFormModel.txtFilterOEMNo
            },
            complete: function (xhr, textStatus) {
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
            .withPaginationType('full_numbers')
            .withDisplayLength(10)
            .withOption('responsive', true)
            .withOption('scrollX', 'auto')
            .withOption('scrollY', '500px')

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
                '9': { type: 'text' }
                //'5': { type: 'text' },
                //'6': { type: 'select', values: [{ value: 'Aktif', label: 'Aktif' }, { value: 'Pasif', label: 'Pasif' }] }
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

        //.withLanguage({
        //    "sEmptyTable": "No data available in table",
        //    "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
        //    "sInfoEmpty": "Showing 0 to 0 of 0 entries",
        //    "sInfoFiltered": "(filtered from _MAX_ total entries)",
        //    "sInfoPostFix": "",
        //    "sInfoThousands": ",",
        //    "sLengthMenu": "Show _MENU_ entries",
        //    "sLoadingRecords": "Yükleniyor...",
        //    "sProcessing": "Processing...",
        //    "sSearch": "Search:",
        //    "sZeroRecords": "No matching records found",
        //    "oPaginate": {
        //        "sFirst": "First",
        //        "sLast": "Last",
        //        "sNext": "Next",
        //        "sPrevious": "Previous"
        //    },
        //    "oAria": {
        //        "sSortAscending": ": activate to sort column ascending",
        //        "sSortDescending": ": activate to sort column descending"
        //    }
        //});
        //.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/Turkish.json');

        $scope.dtInstance = {};
        //$scope.dtInstance.rerender();
    }

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

    $scope.criFilterMatchMaincat = function () {
        return function (psecitem) {
            return psecitem.SECID === $scope.FilterFormModel.ProductSectionID; //criteriaMcat.SECID;
        };
    };
    $scope.criFilterMatchCat = function () {
        return function (pmcatitem) {
            return pmcatitem.MCATID === $scope.FilterFormModel.ProductMCatID; //criteriaMcat.SECID;
        };
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

    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/B2B/StoklarFillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {

                $scope.Currencies = response.data.Currencies;

            }, function (error) {
                //$.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            });

    }

    $scope.Init = function (v, a, e, d) {

        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d };

        $.LoadingOverlay("show");
        //$scope.FirmID = firmID;
        $scope.Listele();
        $scope.FillAllCmbs();
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
