var app = angular.module('NeroApp', ['ngSanitize', 'scrollable-table', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter']);

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

        //$scope.FormModel.StartDateF = $scope.FormModel.StartDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.StartDate);
        //$scope.FormModel.BirthDateF = $scope.FormModel.BirthDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.EndDate);

        $.LoadingOverlay("show");

        $http({
            url: "/api/UserLevels/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.UserLevelID = response.data.ID;

            $scope.ShowingDiv = "Liste";


            $window.setTimeout(function () {

                $scope.dtInstance.rerender();
              
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

        $scope.Saved = false;
        
        $scope.SavedMessage = "";
        $scope.FormModel = { "UserLevelID": null };

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}
         
        $scope.ShowingDiv = "edit";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $.LoadingOverlay("show");

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();

        $scope.Saved = false;
        $scope.SavedMessage = "";

        $scope.FormModel = {};

        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/UserLevels/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.UserLevelDetail;
                     
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

        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Product/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.slides = [
                        { image: '/angular-photo-slider-master/images/img00.jpg', description: 'Image 00' },
                        { image: '/angular-photo-slider-master/images/img01.jpg', description: 'Image 01' },
                        { image: '/angular-photo-slider-master/images/img02.jpg', description: 'Image 02' },
                        { image: '/angular-photo-slider-master/images/img03.jpg', description: 'Image 03' },
                        { image: '/angular-photo-slider-master/images/img04.jpg', description: 'Image 04' }
                    ];

                    $scope.direction = 'left';
                    $scope.currentIndex = 0;

                    $scope.setCurrentSlideIndex = function (index) {
                        $scope.direction = (index > $scope.currentIndex) ? 'left' : 'right';
                        $scope.currentIndex = index;
                    };

                    $scope.isCurrentSlideIndex = function (index) {
                        return $scope.currentIndex === index;
                    };

                    $scope.prevSlide = function () {
                        $scope.direction = 'left';
                        $scope.currentIndex = ($scope.currentIndex < $scope.slides.length - 1) ? ++$scope.currentIndex : 0;
                    };

                    $scope.nextSlide = function () {
                        $scope.direction = 'right';
                        $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.slides.length - 1;
                    };


                    $scope.slidesTR = [
                        { image: '/angular-photo-slider-master/images/img00.jpg', description: 'Image 00' },
                        { image: '/angular-photo-slider-master/images/img01.jpg', description: 'Image 01' },
                        { image: '/angular-photo-slider-master/images/img02.jpg', description: 'Image 02' },
                        { image: '/angular-photo-slider-master/images/img03.jpg', description: 'Image 03' },
                        { image: '/angular-photo-slider-master/images/img04.jpg', description: 'Image 04' }
                    ];

                    // teknik resim
                    $scope.directionTR = 'left';
                    $scope.currentIndexTR = 0;

                    $scope.setCurrentSlideIndexTR = function (index) {
                        $scope.directionTR = (index > $scope.currentIndexTR) ? 'left' : 'right';
                        $scope.currentIndexTR = index;
                    };

                    $scope.isCurrentSlideIndexTR = function (index) {
                        return $scope.currentIndexTR === index;
                    };

                    $scope.prevSlideTR = function () {
                        $scope.directionTR = 'left';
                        $scope.currentIndexTR = ($scope.currentIndexTR < $scope.slidesTR.length - 1) ? ++$scope.currentIndexTR : 0;
                    };

                    $scope.nextSlideTR = function () {
                        $scope.directionTR = 'right';
                        $scope.currentIndexTR = ($scope.currentIndexTR > 0) ? --$scope.currentIndexTR : $scope.slidesTR.length - 1;
                    };

                    $scope.FormModel = response.data.ProductDetail;
                    $scope.ProductToOEMList = response.data.OemList;
                    $scope.MProducts = response.data.MProductlist;
                    $scope.TProducts = response.data.TProductlist;


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

        return;


        $scope.TUPopup = "TUlist";

        var table = $('#entry-grid').DataTable();
        var TURow = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.TUPopupTitle = "BUKOD: " + TURow[0].BUKOD + " - " + TURow[0].ProductName + " (ID:" + TURow[0].PRODID + ")";
        $scope.TUFormModel = { "PRODID": TURow[0].PRODID, "BUKOD": TURow[0].BUKOD };

        $.LoadingOverlay("show");

        var jsondata = { "PRODID": TURow[0].PRODID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Product/TUList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.ProductTUList = response.data.TUList;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductTUForm')
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

     


    function renderView(data) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye btn btn-primary\"></i></a>";
    }
    function renderEdit(data) {
        if (data > 0)
            return "<a href='javascript:void(0);' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt btn btn-success\"></i></a>";
        else
            return "";
    }
    function renderDelete(data) {
        if (data > 0)
            return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showDeletingPopup(" + data + ")'><i class=\"fas fa-trash-alt btn btn-danger\"></i></a>";
        else
            return "";
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
        if (data == null || data == "")
            return "<img style=\"border-radius: 50 % !important; \" width=\"70\" height=\"70\" src=\"/images/bulogo.jpg" + "\" />";
        else
            return "<img style=\"border-radius: 50 % !important; \" width=\"70\" height=\"70\" src=\"/upload/pimages/" + data + "\" />";
    }

    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.UserLevelID === id ? true : false;
            })
            .data();
        
        $scope.DeletingUserLevelName = ($scope.DeletingFile[0].UserLevelName || "");
        $scope.DeleteUserLevelFormModel = { "ID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalUserLevelDeletePopupForm')
            },
            type: 'inline'
        });
    }
    $scope.deleteUserLevel = function () {

        $("#modalUserLevelDeletePopupForm").LoadingOverlay("show");

        $http({
            url: "/api/UserLevels/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteUserLevelFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                $scope.dtInstance.rerender();

                $("#modalUserLevelDeletePopupForm").LoadingOverlay("hide");
                $('modalUserLevelDeletePopupForm').magnificPopup('close');

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalUserLevelDeletePopupForm").LoadingOverlay("hide");
            }, 1000);

        });

    }




    $scope.Filtrele = function () {
        $scope.dtInstance.rerender();
    }

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("UserLevelName", "Yetki/Rol"),
            DTColumnBuilder.newColumn("IsActive", "Durum").renderWith(renderStatus).withClass('text-center'),
            DTColumnBuilder.newColumn("UserLevelID", "İzinler").renderWith(renderPermission).withClass('text-center').notSortable()
        ];

        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("UserLevelID", "Düzenle").renderWith(renderEdit).withClass('text-center').notSortable());
        if ($scope.Permission.Delete == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("UserLevelID", "Sil").renderWith(renderDelete).withClass('text-center').notSortable());

        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/UserLevels/DTList/",
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
                '1': { type: 'text' }
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
        $scope.FillAllCmbs();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

    $scope.BackToList = function () {

        $scope.ShowingDiv = "Liste";
         
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
