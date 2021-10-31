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


var compareTo = function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
};
app.directive("compareTo", compareTo);

function NeroCtrl($scope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {


    var tabClasses;
    //$scope.UploadForm = {};

    function initTabs() {
        tabClasses = ["", "", "",""];
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
       // $scope.$apply();
    };

     
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

    var ajaxCallback = function (data, callback, settings) {

        $.LoadingOverlay("hide");

        $scope.ShowingDiv = "Liste";
        $scope.Saved = false;
    }

    $scope.Save = function (isValid) {

        if (!isValid) {
            toastr.error("Form alanlarını kontrol ediniz.", "Hata");
            return;
        }

        $scope.FormModel.StartDateF = $scope.FormModel.StartDate == undefined ? null : $scope.FormModel.StartDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.StartDate);
        $scope.FormModel.BirthDateF = $scope.FormModel.BirthDate == undefined ? null : $scope.FormModel.BirthDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.EndDate);

        /* image */
        var file = document.getElementById('attachment').files[0];

        if (file !== undefined) {

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

            reader.onload = function (e) {
                var image = e.target.result;
                image = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

                $scope.FormModel.FileName = file.name;
                $scope.FormModel.imgData = image;

                $scope.SavePost();
            }

        } else {
            $scope.FormModel.FileName = null;
            $scope.FormModel.imgData = null;

            $scope.SavePost();
        }
    };

    $scope.SavePost = function () {

        $.LoadingOverlay("show");

        $http({
            url: "/api/Member/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.ID = response.data.ID;

            //$scope.FormModel.StartDate = moment($scope.FormModel.StartDate).format('DD.MM.YYYY');
            //$scope.FormModel.EndDate = $scope.FormModel.EndDate == null ? "" : moment($scope.FormModel.EndDate).format('DD.MM.YYYY');

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
    }

    $scope.AddItem = function () {

        $scope.setActiveTab(1);

        $scope.Saved = false;
      
        $scope.SavedMessage = "";
        $scope.FormModel = { "ID": "", "FirmID": $scope.FirmID };
        $scope.FormModel.user = { password: "", newpassword: "", confirmPassword: "" };

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $("#qimg")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        // .height(200);

        $("#qimgspan").text("Dosya Seç");
        $("#attachment").val("");
        $("#file-name").val("");
        $scope.silImgBtnGoster = false;

        var jsondata = {
            "ID": ""
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Member/Detail/",
            method: "POST",
            params: {
            },
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $scope.FormModel.UserLevels = response.data.UserLevels;
                $scope.ShowingDiv = "detail";
        
            }
            , function (error) {

                $window.setTimeout(function () {

                    toastr.error(error.data.Message);
                    $.LoadingOverlay("hide");

                    $scope.$apply();

                }, 2000);
            });
       
    }

    $scope.View = function (id) {

        $.LoadingOverlay("show");

        $scope.silImgBtnGoster = false;

      
        $scope.setActiveTab(1);
        $scope.FormModel = {};
 
        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Member/Detail/",
            method: "POST",
            params: {
            },
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.Member;
                    //$scope.FormModel.user = { password: "", newpassword: "", confirmPassword: "" };

                    $scope.FormModel.StartDate = $scope.FormModel.StartDate == null ? "" : moment($scope.FormModel.StartDate).format('DD.MM.YYYY');
                    // $scope.FormModel.EndDate = $scope.FormModel.EndDate == null ? "" : moment($scope.FormModel.EndDate).format('DD.MM.YYYY');
                    $scope.FormModel.BirthDate = $scope.FormModel.BirthDate == null ? "" : moment($scope.FormModel.BirthDate).format('DD.MM.YYYY');

                    //$("#attachment").val("");
                    if ($scope.FormModel.PhotoPath) {
                        $("#qimgview")
                            .attr("src", $scope.FormModel.PhotoPath)
                            .width(400);
                        // .height(200)
                        $scope.silImgBtnGoster = true;
                    } else {
                        $("#qimgview")
                            .attr("src", "/content/images/noimage.png")
                         .width(200);
                        // .height(200);

                        //$("#qimgspan").text("Dosya Seç");
                        //$("#attachment").val(""); 
                        //$scope.silImgBtnGoster = false;
                    }


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


    $scope.Edit = function (id) {

        $.LoadingOverlay("show");

        $scope.silImgBtnGoster = false;

        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
         
        //$scope.UploadForm.$setPristine();
        //$scope.UploadForm.$setUntouched();

        //$scope.UploadFormModel = {};

        //$scope.changePassword = {};
        //$scope.changePassword.user = { password: "", newpassword: "", confirmPassword: "" };


        //$scope.changePasswordForm.$setPristine();
        //$scope.changePasswordForm.$setUntouched();

        //setTimeout(function () {

        $scope.Saved = false;
        $scope.SavedMessage = "";
        $scope.setActiveTab(1);
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
            url: "/api/Member/Detail/",
            method: "POST",
            params: {
            },
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.Member;
                    $scope.FormModel.user = { password: "", newpassword: "", confirmPassword: "" };
                    $scope.FormModel.UserLevels = response.data.UserLevels;


                    $scope.FormModel.StartDate = $scope.FormModel.StartDate == null ? "" : moment($scope.FormModel.StartDate).format('DD.MM.YYYY');
                    // $scope.FormModel.EndDate = $scope.FormModel.EndDate == null ? "" : moment($scope.FormModel.EndDate).format('DD.MM.YYYY');
                    $scope.FormModel.BirthDate = $scope.FormModel.BirthDate == null ? "" : moment($scope.FormModel.BirthDate).format('DD.MM.YYYY');


                    $("#attachment").val("");
                    $("#file-name").val("");
                    if ($scope.FormModel.PhotoPath) {
                        $("#qimg")
                            .attr("src", $scope.FormModel.PhotoPath)
                            .width(150);
                        // .height(200)
                        $scope.silImgBtnGoster = true;
                    } else {
                        $("#qimg")
                            .attr("src", "/content/images/noimage.png")
                            .width(150);
                        // .height(200);

                        $("#qimgspan").text("Dosya Seç");
                        $("#attachment").val("");
                        $("#file-name").val("");
                        $scope.silImgBtnGoster = false;
                    }


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
     
    };

    $scope.readURLQImg = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("#qimg")
                    .attr("src", e.target.result)
                    .width(150);
                //.height(200)
            };

            reader.readAsDataURL(input.files[0]);
            $scope.silImgBtnGoster = true;
            scope.FormModel.DeletedResim = false;
        } else {
            $("#qimg")
                .attr("src", "/images/noimage.png")
                .width(150);
            //.height(50);
            $scope.silImgBtnGoster = false;
        }
    };
    $scope.deleteFilesImg = function () {
        $scope.silImgBtnGoster = false;

        $("#qimg")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        //.height(50);

        $("#qimgspan").text("Dosya Seç");
        $("#attachment").val("");
        $("#file-name").val("");

        if ($scope.tutorial != null)
            $scope.tutorial.attachment = null;

        $scope.FormModel.DeletedResim = true;
        //$scope.UploadFormModel.File = "";
    };

    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye btn btn-primary\"></i></a>";
    }
    function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt btn btn-success\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' class='sil-btn-round' onclick='angular.element(this).scope().showDeletingPopup(\"" + data + "\")'><i class=\"fas fa-trash-alt btn btn-danger\"></i></a>";
    }
    function renderStatus(data) {
        return data == "Aktif" ? "<button type='button' class='btn btn-circle btn-success btn-xs'>" + data + "</button>" : "<button type='button' class='btn btn-circle btn-danger btn-xs'>" + data + "</button>";
    }

    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.ID === id ? true : false;
            })
            .data();

        $scope.DeletingMemberFullName = ($scope.DeletingFile[0].FirstName || "") + " " + ($scope.DeletingFile[0].LastName || "");
        $scope.DeleteMemberFormModel = { "ID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalMemberDeleteForm')
            },
            type: 'inline'
        });

    }
    $scope.deleteMember = function () {

        $("#modalMemberDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/Member/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteMemberFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                $scope.dtInstance.rerender();

                $("#modalMemberDeleteForm").LoadingOverlay("hide");
                $('modalMemberDeleteForm').magnificPopup('close');

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalMemberDeleteForm").LoadingOverlay("hide");
            }, 1000);

        });

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

    $scope.changePassword = function (isValid) {

        if (!isValid)
            return;

        $.LoadingOverlay("show");

        $scope.changePassword.user.ID = $scope.FormModel.ID,

            $http({
                url: "/api/Member/ChangePassword"
                , method: "POST"
                , params: {}
                , contentType: "application/json"
                , data: $scope.changePassword.user
                , dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.changePassword.user = { password: "", newpassword: "", confirmPassword: "" };

                        //$scope.ShowingDiv = "tebrikler";
                        $.LoadingOverlay("hide");
                        $scope.$apply();

                    }, 2000);

                }, function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        $.LoadingOverlay("hide");

                        $scope.$apply();

                    }, 2000);
                });
    };

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("PhotoPath", "Resim").withOption('width', '10px').withClass('text-center').notSortable(),
           // DTColumnBuilder.newColumn("Role", "Yetki"),
            DTColumnBuilder.newColumn("JobTitle", "Görev"),
            DTColumnBuilder.newColumn("FirstName", "Ad"),
            DTColumnBuilder.newColumn("LastName", "Soyad"),
            DTColumnBuilder.newColumn("Email", "Mail"),
            DTColumnBuilder.newColumn("GSMNumber", "Cep Tel. No"),
            //DTColumnBuilder.newColumn("StartDate", "Başlangıç Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("IsActive", "Durum").withOption('width', '10px').renderWith(renderStatus).withClass('text-center')];

        if ($scope.Permission.View == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("ID", "Görüntüle").withOption('width', '10px').renderWith(renderView).withClass('text-center').notSortable());
        if ($scope.Permission.Edit == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("ID", "Düzenle").withOption('width', '10px').renderWith(renderEdit).withClass('text-center').notSortable());
        if ($scope.Permission.Delete == 1)
            $scope.dtColumns.push(DTColumnBuilder.newColumn("ID", "Sil").withOption('width', '10px').renderWith(renderDelete).withClass('text-center').notSortable());

            //DTColumnBuilder.newColumn("ID", "Görüntüle").renderWith(renderView).withClass('text-center'),
            //DTColumnBuilder.newColumn("ID", "Düzenle").renderWith(renderEdit).withClass('text-center'),
            //DTColumnBuilder.newColumn("ID", "Sil").renderWith(renderDelete).withClass('text-center')

        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Member/DTList/",
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
            .withOption('order', [2, 'asc'])
            .withPaginationType('full_numbers')
            .withDisplayLength(10)
            .withOption('responsive', true)
            .withOption('scrollX', 'auto')
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
                '6': { type: 'select', values: [{ value: 'Aktif', label: 'Aktif' }, { value: 'Pasif', label: 'Pasif' }] },

               // '8': { type: 'text' }
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
            url: "/api/Member/FillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {

                $scope.CmbRoles = response.data.Roles;
                $scope.CmbJobTitles = response.data.JobTitles;

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
