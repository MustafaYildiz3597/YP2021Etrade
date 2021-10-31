var app = angular.module('NeroApp', ['ngSanitize', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter']);

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

function NeroCtrl($scope, $http, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {

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

    $scope.ClosePermissionPopup = function () {
        $.magnificPopup.close();
    }

    function renderPermission(data, type, row) {
        if (data > 0)
            return "<a href='javascript:void(0);' onclick='angular.element(this).scope().ShowPermissionPopup(" + row.UserLevelID + ")'><i class=\"fas fa-key btn btn-warning\"></i></a>";
        else
            return "";
    }

    $scope.ShowPermissionPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {
                return data.UserLevelID === id ? true : false;
            })
            .data();

        $scope.UserLevelPopupTitle = "Yetki: " + DTRow[0].UserLevelName;

        $scope.UserLevelID = DTRow[0].UserLevelID;

        $.LoadingOverlay("show");

        var jsondata = { "UserLevelID": DTRow[0].UserLevelID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/UserLevels/GetPermissions/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.PermissionList = response.data;
                    $scope.PermissionPopup = "list";

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalPermissionForm')
                        },
                        type: 'inline',
                        modal: true,
                        closeOnContentClick: false,
                    });

                    $scope.$apply();

                }, 500);
            }
                , function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        $.LoadingOverlay("hide");

                        $scope.$apply();

                    }, 500);
                });
    }

    $scope.SaveUserLevelPermissions = function () {

        $('#modalPermissionForm').LoadingOverlay("show");

        var jsondata = {
            "UserLevelID": $scope.UserLevelID,
            "UserLevelPermissions": $scope.PermissionList
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/UserLevels/SetPermissions/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $('#modalPermissionForm').LoadingOverlay("hide");

                    toastr.success(response.data.Message);

                    $.magnificPopup.close();

                    $scope.$apply();

                }, 500);
            }
                , function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        $('#modalPermissionForm').LoadingOverlay("hide");

                        $scope.$apply();

                    }, 500);
                });

    }

    var ajaxCallback = function (data, callback, settings) {

        $.LoadingOverlay("hide");

        $scope.ShowingDiv = "Liste";
        $scope.Saved = false;
    }

    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $.LoadingOverlay("show");

        $http({
            url: "/api/Ulke/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            var table = $('#entry-grid').DataTable();

            if ($scope.FormModel.UID == null) {

                $scope.FormModel.UID = response.data.ID;
                table.rows.add([{
                    "UID": response.data.ID,
                    "UNAME": $scope.FormModel.UNAME
                }]).draw();

            } else {

                var rowindex = -1;
                table
                    .rows(function (idx, data, node) {
                        if (data.UID === $scope.FormModel.UID) {
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

                // $scope.dtInstance.rerender();
                $scope.breadcrumbs = "Liste";
                $scope.ShowingDiv = "Liste";

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);

                // $scope.Saved = true;
                $scope.$apply();

            }, 500);

        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });

    };

    $scope.AddItem = function () {

        $scope.breadcrumbs = "Ekle";

        $scope.Saved = false;

        $scope.SavedMessage = "";
        $scope.FormModel = { "ID": null };

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $scope.ShowingDiv = "edit";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $scope.breadcrumbs = "Düzenle";

        $.LoadingOverlay("show");

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();

        $scope.Saved = false;
        $scope.SavedMessage = "";

        $scope.FormModel = {};

        var jsondata = {
            "UID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Ulke/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.Ulke;

                    $scope.ShowingDiv = "edit";
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

        $scope.breadcrumbs = "Detay/İncele";

        $.LoadingOverlay("show");

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();

        $scope.Saved = false;
        $scope.SavedMessage = "";

        $scope.FormModel = {};

        var jsondata = {
            "UID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Ulke/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.Ulke;

                    $scope.ShowingDiv = "view";
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

    function renderView(data) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye btn btn-primary\"></i></a>";
    }
    function renderEdit(data) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt btn btn-success\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showDeletingPopup(" + data + ")'><i class=\"fas fa-trash-alt btn btn-danger\"></i></a>";
    }
    function renderStatus(data) {
        return data == true ? "<button type='button' class='btn btn-circle btn-success btn-xs'>Aktif</button>" : "<button type='button' class='btn btn-circle btn-danger btn-xs'>Pasif</button>";
    }

    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.UID === id ? true : false;
            })
            .data();

        $scope.DeletingName = ($scope.DeletingFile[0].UNAME || "");
        $scope.DeleteFormModel = { "UID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalDeletePopupForm')
            },
            type: 'inline'
        });
    }
    $scope.delete = function () {

        $("#modalDeletePopupForm").LoadingOverlay("show");

        $http({
            url: "/api/Ulke/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                //$scope.dtInstance.rerender();

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.UID === $scope.DeleteFormModel.UID) {
                            rowindex = idx;
                            return;
                        }
                    })
                    .data();

                var d = table.row(rowindex).remove().draw('page');

                $("#modalDeletePopupForm").LoadingOverlay("hide");
                $('modalDeletePopupForm').magnificPopup('close');

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalDeletePopupForm").LoadingOverlay("hide");
                $scope.$apply();
            }, 1000);

        });

    }


    //function renderSehirler(data, type, row) {
    //    return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showSehirlerList(" + row.UID + ")'><i class=\"no-btn\">" + data + "</i></a>";
    //}
    //$scope.showSehirlerList = function (id) {

    //    $scope.OEMPopup = "oemlist";

    //    var table = $('#entry-grid').DataTable();
    //    var OEMRow = table
    //        .rows(function (idx, data, node) {
    //            return data.PRODID === id ? true : false;
    //        })
    //        .data();

    //    $scope.OEMPopupTitle = "BUKOD: " + OEMRow[0].BUKOD + " - " + OEMRow[0].ProductName + " (ID:" + OEMRow[0].PRODID + ")";
    //    $scope.OEMFormModel = { "PRODID": OEMRow[0].PRODID, "BUKOD": OEMRow[0].BUKOD };

    //    $scope.PopupPRODID = OEMRow[0].PRODID;

    //    $.LoadingOverlay("show");

    //    var jsondata = { "PRODID": OEMRow[0].PRODID };
    //    var dt = JSON.stringify(jsondata);

    //    $http({
    //        url: "/api/Product/OEMList/",
    //        method: "POST",
    //        params: {},
    //        contentType: "application/json",
    //        data: dt,
    //        dataType: "json"
    //    })
    //        .then(function (response) {

    //            $window.setTimeout(function () {

    //                $scope.ProductOEMList = response.data.OEMList;

    //                $.LoadingOverlay("hide");

    //                $.magnificPopup.open({
    //                    items: {
    //                        src: $('#modalProductOEMForm')
    //                    },
    //                    type: 'inline',
    //                    modal: true,
    //                    closeOnContentClick: false,
    //                });

    //                $scope.$apply();

    //            }, 500);
    //        }
    //            , function (error) {

    //                $window.setTimeout(function () {

    //                    toastr.error(error.data.Message);
    //                    $.LoadingOverlay("hide");

    //                    $scope.$apply();

    //                }, 500);
    //            });
    //}


    $scope.Filtrele = function () {
        $scope.dtInstance.rerender();
    }

    $scope.breadcrumbs = "Liste";
    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            // DTColumnBuilder.newColumn("RankNumber", "Sıra No"),
            DTColumnBuilder.newColumn("UNAME", "Ad")//,           
            // DTColumnBuilder.newColumn("IsActive", "Durum").renderWith(renderStatus).withClass('text-center')
        ];

        if ($scope.Permission.View == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("UID", "Görüntüle").renderWith(renderView).withClass('text-center').notSortable());
        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("UID", "Düzenle").renderWith(renderEdit).withClass('text-center').notSortable());
        if ($scope.Permission.Delete == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("UID", "Sil").renderWith(renderDelete).withClass('text-center').notSortable());

        //$scope.dtColumns.push(DTColumnBuilder.newColumn("UID", "Şehirler").withOption('width', '10px').renderWith(renderSehirler).withClass('text-center').notSortable());

        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Ulke/DTList/",
            type: "POST",
            dataType: "json",
            data: function (d) {

            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status == 400)
                    toastr.error(xhr.responseJSON.Message);
                else
                    toastr.error("Listeleme işlemi yapılırken bir hata oluştu!");
            }
        })
            .withDOM('lBfrtip')
            .withPaginationType('full_numbers')
            .withDisplayLength(50)
            .withOption('responsive', true)
            .withOption('scrollX', 'auto')
            .withOption('scrollY', '500px')
            .withOption('order', [0, 'asc'])
            .withOption('scrollCollapse', true)
            .withOption('autoWidth', true)
            .withButtons([
                'print',
                'excel'
            ])
            .withLightColumnFilter({
                '0': { type: 'text' },
                '1': { type: 'text' },
                '2': { type: 'text' }
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

        $scope.dtInstance = {};
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


    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/UserLevels/FillAllCmb/"
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
                $scope.OEMSupliers = response.data.OEMSupliers;
                $scope.Musteriler = response.data.Musteriler;
                $scope.Tedarikciler = response.data.Tedarikciler;
                $scope.Currencies = response.data.Currencies;

                $.LoadingOverlay("hide");

            }, function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            });

    }

    $scope.Init = function (firmID, v, a, e, d) {

        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d };

        // $.LoadingOverlay("show");
        $scope.FirmID = firmID;
        $scope.Listele();
        //$scope.FillAllCmbs();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

    $scope.BackToList = function () {

        $scope.ShowingDiv = "Liste";
        $scope.breadcrumbs = "Liste";
        $window.setTimeout(function () {
            //    //if ($scope.Saved === true)
            //    //    $scope.dtInstance.rerender();
            //    //$scope.apply();
        }, 500);
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
