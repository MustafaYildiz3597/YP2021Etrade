var app = angular.module('NeroApp', ['ngSanitize', 'textAngular', 'scrollable-table', 'ui.mask', 'angular.filter', 'angularUtils.directives.dirPagination', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter']);

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

app.directive('validFile', function () {
    return {
        require: 'ngModel',
        link: function (scope, el, attrs, ngModel) {
            ngModel.$render = function () {
                ngModel.$setViewValue(el.val());
            };

            el.bind('change', function () {
                scope.$apply(function () {
                    ngModel.$render();
                });
            });
        }
    };
});
 
function NeroCtrl($scope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {

        /* ui-select*/
    $scope.multipleDemo = {};
    $scope.multipleDemo.selectedFirms = null;

    $scope.currentPage = 1;
    $scope.currentPage2 = 1;
    $scope.pageSize = 10;
    $scope.pageSize2 = 10;
    $scope.pagesSizes = [10, 25, 50, 100];

    $scope.sort = function (keyname) {
        $scope.sortBy = keyname;   //set the sortBy to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }
    $scope.sort2 = function (keyname) {
        $scope.sortBy2 = keyname;   //set the sortBy to the param passed
        $scope.reverse2 = !$scope.reverse2; //if true make it false and vice versa
    }


    $scope.FilterFormModel = {};
    $scope.FilterFormModel.TeklifTipiID = "";
    $scope.FilterFormModel.FirmID = "";

    $scope.FormModel = {};

    $scope.TeklifTipiList = [{ "ID": 1, "Title": "Alış Teklifi" }, { "ID": 2, "Title": "Satış Teklifi" }];
    $scope.TeklifDurumuList = [{ "ID": 1, "Title": "Açık" }, { "ID": 2, "Title": "Beklemede" }, { "ID": 3, "Title": "Kapalı" }];

    var tabClasses;
    $scope.UploadForm = {};

    function initTabs() {
        tabClasses = ["", "", ""];
    }


    $scope.getTabClass = function (tabNum) {
        return tabClasses[tabNum];
    };

    $scope.getTabPaneClass = function (tabNum) {
        return "tab-pane " + tabClasses[tabNum];
    }

    $scope.setActiveTab = function (tabNum) {
        initTabs();
        tabClasses[tabNum] = "active";
    };

    initTabs();
    $scope.setActiveTab(1);


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

        if ($scope.linemode != "")
            return;

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        //$scope.FormModel.StartDateF = $scope.FormModel.StartDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.StartDate);
        //$scope.FormModel.BirthDateF = $scope.FormModel.BirthDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.EndDate);

        $.LoadingOverlay("show");

        $http({
            url: "/api/Offer/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.TID = response.data.TID;

            $scope.ShowingDiv = "Liste";

            $window.setTimeout(function () {

                $scope.dtInstance.rerender();
                //$scope.dtInstance.reloadData(null, true);

                //var table = $('#entry-grid').DataTable();

                //// Sort by column 1 and then re-draw
                //table
                //    .order([[1, 'asc']])
                //    .draw(true);

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);

                $scope.Saved = true;
                $scope.$apply();

            }, 500);


            //$scope.Listele();

            //debugger;
            //$scope.dtInstance.rerender();


        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });

    };

    $scope.AddItem = function () {

        $scope.newline = -1;

        $scope.Saved = false;
        //$scope.setActiveTab(1);
        $scope.SavedMessage = "";
        $scope.FormModel = { "TID": "", "FirmID": $scope.FirmID };
        $scope.FormModel.TeklifItems = [];

        //$scope.DetailForm = [];

        $scope.OfferHeaderText = "Yeni"

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}


        $scope.ShowingDiv = "edit";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $scope.newline = -1;

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();

        $.LoadingOverlay("show");

        setTimeout(function () {

            $scope.SavedMessage = "";

            $scope.FormModel = {};

            var jsondata = {
                "ID": id
            };
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/OFfer/Detail/",
                method: "POST",
                params: {},
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.FormModel = response.data.OfferDetail;

                        $scope.OfferHeaderText = " No: " + ($scope.FormModel.TEKLIFNO || ' - ') + " / " + (response.data.OfferDetail.TITLE || '');

                        $scope.TDID = 0;

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

                        }, 500);
                    });
        }, 500);

    };

    $scope.View = function (id) {

        $.LoadingOverlay("show");

        setTimeout(function () {

            $scope.SavedMessage = "";

            $scope.FormModel = {};

            var jsondata = {
                "ID": id
            };
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/OFfer/Detail/",
                method: "POST",
                params: {},
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.FormModel = response.data.OfferDetail;

                        $scope.OfferHeaderText = " No: " + ($scope.FormModel.TEKLIFNO || ' - ') + " / " + (response.data.OfferDetail.TITLE || '');

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

                        }, 500);
                    });
        }, 500);
    }

    $scope.readURLQImg = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {

                image = e.target.result;
                $scope.FormModel.RESIMDATA = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

                $("#qimg")
                    .attr("src", e.target.result);
                //.width(150);
                //.height(200)
            };

            reader.readAsDataURL(input.files[0]);
            $scope.silImgBtnGoster = true;
        } else {
            $("#qimg")
                .attr("src", "/images/noimage.png")
            //.width(150);
            //.height(50);
            $scope.silImgBtnGoster = false;
        }
    };

    $scope.deleteFilesImg = function () {
        $scope.silImgBtnGoster = false;

        $("#qimg")
            .attr("src", "/content/images/noimage.png")
        //.width(150);
        //.height(50);

        $("#qimgspan").text("Dosya Seç");
        $("#attachment").val("");

        if ($scope.tutorial != null)
            $scope.tutorial.attachment = null;
        $scope.UploadFormModel.File = "";
    };

     function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt text-white\"></i></a>";
    }
    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye text-white\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' class='sil-btn-round' onclick='angular.element(this).scope().showDeletingPopup(" + data + ")'><i class=\"fas fa-trash-alt text-white\"></i></a>";
    }
    function renderStatus(data) {
        return data == true ? "<button type='button' class='btn btn-circle btn-success btn-xs'>Aktif</button>" : "<button type='button' class='btn btn-circle btn-danger btn-xs'>Pasif</button>";
    }
    function renderWebSite(data) {
        return "<button type=\"button\" onclick=\"window.open('http://demo3.buinteractive.com/prod_details.asp?prodid=" + data + "')\" class=\"btn red\"><i class=\"fa fa-link\"></i></button>";
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

    function renderPic(data) {
        return "<img style=\"border-radius: 50 % !important; \" width=\"70\" height=\"70\" src=\"http://nero.buautomotive.net/V.2019.01/upload/Pimages/" + (data == null || data == "" ? "/images/bulogo.jpg" : data) + "\" />"
    }

    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.TID === id ? true : false;
            })
            .data();

        $scope.DeletingTitle = ($scope.DeletingFile[0].Konu || "");
        $scope.DeleteOfferFormModel = { "TID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalOfferDeleteForm')
            },
            type: 'inline'
        });

    }


    $scope.deleteOffer = function () {

        $("#modalOfferDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/Offer/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteOfferFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                $scope.dtInstance.rerender();

                $("#modalOfferDeleteForm").LoadingOverlay("hide");
                $('modalOfferDeleteForm').magnificPopup('close');

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalOfferDeleteForm").LoadingOverlay("hide");
            }, 1000);

        });

    }

    $scope.Filtrele = function () {
        $scope.dtInstance.rerender();
    }


    $scope.ExportOfferToExcel = function () {
        $window.open('/Teklif/GoruntuleXls?id=' + ($scope.FormModel.TID || 0), '_blank');
        toastr.error("Teklif kaydı excele aktarıldı!");
    }

    $scope.ExportOfferToPDF = function () {
        $window.open('/Teklif/GoruntulePdf?id=' + ($scope.FormModel.TID || 0), '_blank');
        toastr.error("Teklif kaydı PDF dosyasına aktarıldı!");
    }

    $scope.UploadImage = function (isvalid) {

        if (!isvalid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }


        var file = document.getElementById('attachment').files[0];

        if (file.size > 5242880) {
            toastr.error("Yüklemiş olduğunuz dosya boyutu çok yüksektir, lütfen kontrol edip tekrar deneyiniz.", "", {
                "closeButton": true,
                "positionClass": "toast-top-full-width",
                "tapToDismiss": true
            });
            // toastr.error("Yüklemiş olduğunuz dosya boyutu çok yüksektir, lütfen kontrol edip tekrar deneyiniz.");
            return;
        }

        if (file.type != "image/jpg" && file.type != "image/jpeg" && file.type != "image/png") {
            toastr.error("Yüklemiş olduğunuz dosya jpeg ya da png formatında olmalıdır, lütfen kontrol edip tekrar deneyiniz.", "", {
                "closeButton": true,
                "positionClass": "toast-top-full-width",
                "tapToDismiss": true
            });
            return;
        }

        var reader = new FileReader();
        reader.readAsDataURL(file);
        var image = '';
        reader.onload = function (e) {
            image = e.target.result;
            image = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

            //$scope.OCRimage = image;

            setTimeout(function () {

                var jsondata = {
                    ID: $scope.FormModel.ID,
                    FileName: file.name,
                    imgData: image
                }

                var dt = JSON.stringify(jsondata);

                $.LoadingOverlay("show");

                $http({
                    url: "/api/Member/UploadProfileImage/"
                    //, crossDomain: true
                    , method: "POST"
                    //, headers: { "accept": "application/json", "content-type": "application/json", "cache-control": "no-cache" }               
                    , params: {}
                    , contentType: "application/json"
                    , dataType: "json"
                    , data: dt
                })
                    .then(function (response) {

                        toastr.success(response.data);
                        $scope.dtInstance.rerender();

                        $.LoadingOverlay("hide");
                        //$("#loading-wrapper").fadeOut();

                    }, function (error) {

                        $.LoadingOverlay("hide");

                        toastr.error(error.data.Message);

                        //if (error.data != null) {
                        //    if (error.data.Message != null)
                        //        $scope.maploginErrText = error.data.Message;
                        //    else
                        //        $scope.maploginErrText = "Görseliniz yüklenirken bir hata oluştu. Lütfen tekrar deneyiniz."; //error.data.Message;
                        //}
                        //else {
                        //    $scope.maploginErrText = "Görseliniz yüklenirken bir hata oluştu. Lütfen tekrar deneyiniz."; //error.data.Message;
                        //}

                        //$scope.VerifyFormReset();
                        //$scope.ShowErrorPage('dogrulama');
                    });

            }, 500);
        };


    }

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            //DTColumnBuilder.newColumn("RESIM", "Resim").renderWith(renderPic).withClass('text-center'),
            DTColumnBuilder.newColumn("TEKLIFNO", "Teklif No").withOption('width', '10%'),
            DTColumnBuilder.newColumn("TeklifTipi", "Teklif Tipi").withOption('width', '10%'),
            DTColumnBuilder.newColumn("FirmaAdi", "Firma Adı").withOption('width', '15%'),
            DTColumnBuilder.newColumn("Konu", "Konu").withOption('width', '15%'),
            DTColumnBuilder.newColumn("Hazirlayan", "Hazırlayan").withOption('width', '15%'),
            DTColumnBuilder.newColumn("EklenmeTarihi", "Eklenme Tarihi").renderWith(renderDate).withOption('width', '10%'),
            DTColumnBuilder.newColumn("TeklifDurumu", "Teklif Durumu").withOption('width', '10%') //.renderWith(renderStatus).withClass('text-center'),
        ];

        if ($scope.Permission.View == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Görüntüle").renderWith(renderView).withClass('text-center').withOption('width', '5%').notSortable());
        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Detay").renderWith(renderEdit).withClass('text-center').withOption('width', '5%').notSortable());
        if ($scope.Permission.Delete == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Sil").renderWith(renderDelete).withClass('text-center').withOption('width', '5%').notSortable());


        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Offer/DTList/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                //d.FirmCode = $scope.SelectedFirm;// $('.daterangepicker_start_input input').val();
                //d.PeriodCode = $scope.SelectedPeriod; ////$('.daterangepicker_end_input input').val();
                //d.SalesmanRef = $scope.SelectedSalesManRef;
                //d.BeginDate = $scope.start;
                //d.EndDate = $scope.end;

                d.TeklifTipiID = $scope.FilterFormModel.TeklifTipiID,
                    d.FirmID = $scope.FilterFormModel.FirmID
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
                '5': { type: 'text' },
                '6': {
                    type: 'select', values: [{
                        value: 'Açık', label: 'Açık'
                    }, {
                        value: 'Beklemede', label: 'Beklemede'
                    }, {
                        value: 'Kapalı', label: 'Kapalı'
                    }]
                }
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

        $scope.dtOptions.order = [[5, "desc"]];

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



    $scope.criFilterFirmalar = function () {
        return function (item) {
            return item.FIRMA_TIPI === $scope.FormModel.TTIPI || item.FIRMA_TIPI == 5;
        };
    };

    $scope.criFilterYetkiliKisiler = function () {
        return function (item) {
            return item.FIRMA_ID === $scope.FormModel.MusteriID;
        };
    };

  $scope.MusteriIDChanged = function (newval, oldval) {

      if ($scope.FormModel.TeklifItems.length > 0) {

            $scope.FormModel.MusteriID = oldval;
            $scope.MusteriIDNewVal = newval;

            $.magnificPopup.open({
                items: {
                    src: $('#modalConfirmMessageForm')
                },
                type: 'inline',
                modal: true,
                closeOnContentClick: false,
            });
        }
    }
    $scope.ApproveConfirm = function (val) {
        $scope.FormModel.MusteriID = $scope.MusteriIDNewVal;
        $scope.FormModel.TeklifItems = [];

        if ($scope.FormModel.DeletedItems == null)
            $scope.FormModel.DeletedItems = [];

        $scope.FormModel.TeklifItems.forEach(function (item) {
            if (item.ORDID > 0) { // newlines hariçler silinecek listesine eklenir.
                var pushObj = {
                    "ORDID": item.ORDID
                };
                $scope.FormModel.DeletedItems.push(pushObj);
            }
        });

        $('modalConfirmMessageForm').magnificPopup('close');
    }
    $scope.RejectConfirm = function (val) {
        $('modalConfirmMessageForm').magnificPopup('close');
    }
    
    //$scope.ConfirmMusteriIDChanged = function (val) {

    //    if (val == 0) {
    //        $scope.FormModel.MusteriID = $scope.MusteriIDOldVal;
    //    }

    //}

    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/Offer/FillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {
                $scope.Firmalar = response.data.Firmalar;
                $scope.Currencies = response.data.Currencies;
                $scope.YetkiliKisiler = response.data.YetkiliKisiler;

                $.LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };

    }

    $scope.Init = function (firmID, v, a, e, d) {

        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d }; 

        $.LoadingOverlay("show");
        $scope.FirmID = firmID;
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

    $scope.newline = 0;
    $scope.linemode = "";
    $scope.TDInlineAdd = function () {

        $scope.linemode = "add";
        $scope.newline = $scope.newline - 1;

        var pushObj = {
            "TDID": $scope.newline
        };
        $scope.FormModel.TeklifItems.push(pushObj);
        var itemID = $scope.newline;


        $scope.TDID = itemID;

        var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);

        $scope.FormModel.TeklifItems[idx].error = false;
        $scope.FormModel.TeklifItems[idx].errortext = "";

        $scope.TmpItem = angular.copy($scope.FormModel.TeklifItems[idx]);

        document.getElementById('bottomOfDiv').scrollIntoView(true);
    }

    $scope.TDInlineDelete = function (itemID) {

        if ($scope.FormModel.DeletedItems == null)
            $scope.FormModel.DeletedItems = [];

        var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);

        if (itemID < 0) { // newlines listeye eklenmez.
            var pushObj = {
                "TDID": itemID
            };
            $scope.FormModel.DeletedItems.push(pushObj);
            $scope.linemode = "";
        }

        $scope.FormModel.TeklifItems.splice(idx, 1);
    }

    $scope.TDInlineEdit = function (itemID) {

        $scope.TDID = itemID;

        var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);

        $scope.FormModel.TeklifItems[idx].error = false;
        $scope.FormModel.TeklifItems[idx].errortext = "";

        $scope.TmpItem = angular.copy($scope.FormModel.TeklifItems[idx]);

        $scope.linemode = "edit";

        //if ($scope.newline == itemID)
        //    $scope.newline = $scope.newline - 1;
    }

    $scope.TDInlineCancel = function (itemID) {

        if (itemID == $scope.newline) { //if (itemID < 0) {
            var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);
            $scope.FormModel.TeklifItems.splice(idx, 1);
        } else {
            var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);
            $scope.FormModel.TeklifItems[idx] = angular.copy($scope.TmpItem);

            $scope.FormModel.TeklifItems[idx].error = false;
            $scope.FormModel.TeklifItems[idx].errortext = "";
        }

        $scope.TDID = 0;
        $scope.linemode = "";
    }

    $scope.TDInlineUpdate = function (itemID) {

        var idx = $scope.FormModel.TeklifItems.findIndex(x => x.TDID === itemID);

        var item = $scope.FormModel.TeklifItems[idx];

        $scope.FormModel.TeklifItems[idx].error = false;
        $scope.FormModel.TeklifItems[idx].errortext = "";

        if (
            (item.CustomerCode == "" || item.CustomerCode == undefined)
            && (item.BuCode == "" || item.BuCode == undefined)
        ) {
            $scope.FormModel.TeklifItems[idx].error = true;
            $scope.FormModel.TeklifItems[idx].errortext = "Müşteri kodu ya da BUKOD girmelisiniz!";
            return;
        } else if (item.Quantity == "" || item.Quantity == undefined) {
            $scope.FormModel.TeklifItems[idx].error = true;
            $scope.FormModel.TeklifItems[idx].errortext = "Miktar girmelisiniz!";
            return;
        } else if (item.UnitPrice == "" || item.UnitPrice == undefined) {
            $scope.FormModel.TeklifItems[idx].error = true;
            $scope.FormModel.TeklifItems[idx].errortext = "Birim Fiyatı girmelisiniz!";
            return;
        } else if (item.CURRENCY == "" || item.CURRENCY == undefined) {
            $scope.FormModel.TeklifItems[idx].error = true;
            $scope.FormModel.TeklifItems[idx].errortext = "Para Birimi seçmelisiniz!";
            return;
        }

        var currencyCode = $scope.Currencies[$scope.Currencies.findIndex(x => x.ID === item.CURRENCY)].Code;

        $scope.FormModel.TeklifItems[idx].CurrencyCode = currencyCode;

        $scope.TDID = 0;
        $scope.linemode = "";
    }

   $scope.BrowseBUKOD = function (idx) {

        $scope.SelProduct = null;
        $scope.SelIdx = idx;

        ////$scope.Products = angular.copy($scope.ProductsTemp);
        //angular.forEach($scope.Products, function (value, key) {

        //    if ($scope.Products[key].chk == true)
        //        $scope.Products[key].chk = false;
        //});

        $("#modalBrowseBUKODPopup").LoadingOverlay("show");

        $.magnificPopup.open({
            items: {
                src: $('#modalBrowseBUKODPopup')
            },
            type: 'inline',
            modal: true
        });

        var jsondata = {
            "MusteriID": $scope.FormModel.MusteriID
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/MProducts/BrowseList/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        })
            .then(function (response) {
                $scope.MProducts = response.data.MProducts;

                $scope.sort('BUKOD');
                $scope.reverse = false;

                //$scope.SearchForm.$setPristine();
                //$scope.SearchForm.$setUntouched();

                $("#modalBrowseBUKODPopup").LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };

       
    }
    $scope.CloseBrowseBUKOD = function () {
        $.magnificPopup.close();
    }
    $scope.AddSelectedBUKOD = function () {

        if ($scope.SelProduct == null) {
            toastr.warning("Ürün seçmelisiniz!");
            return;
        }

        var mpID = $scope.SelProduct;
        var mproduct = $scope.MProducts.filter(item => item.MPID == mpID);
        if (mproduct.length == 0) {
            toastr.warning("Ürün kaydına ulaşılamadı!");
            return;
        }

        // check selected product in other rows
        //var returnfunc = false;
        //angular.forEach($scope.GridAddModel, function (value, key) {
        //    if (key != $scope.SelIdx && $scope.GridAddModel[key].ProductID == product[0].PRODID) {
        //        toastr.warning("Seçtiğiniz ürün zaten listede var!");
        //        returnfunc = true;
        //        return;
        //    }
        //});

        //if (returnfunc == true) {
        //    return;
        //}

        //var idx = $scope.SelIdx;
        //$scope.GridAddModel[idx].BUKOD = product[0].BUKOD;
        //$scope.GridAddModel[idx].ProductID = product[0].PRODID;
        //$scope.GridAddModel[idx].NAME = product[0].NAME;

        $scope.FormModel.TeklifItems[$scope.SelIdx].BuCode = mproduct[0].BUKOD;
        $scope.FormModel.TeklifItems[$scope.SelIdx].CustomerCode = mproduct[0].XPSNO;
        $scope.FormModel.TeklifItems[$scope.SelIdx].NAME_EN = mproduct[0].NAME;
        $scope.FormModel.TeklifItems[$scope.SelIdx].UnitPrice = mproduct[0].PRICE;
        $scope.FormModel.TeklifItems[$scope.SelIdx].CurrencyID = mproduct[0].CURRENCY;

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
