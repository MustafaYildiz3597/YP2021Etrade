var app = angular.module('NeroApp', ['ngSanitize', 'ngCookies', 'textAngular', 'scrollable-table', 'ui.mask', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter', 'ngAnimate', 'ngTouch', 'ckeditor', 'angularUtils.directives.dirPagination']);

app.config(['$provide', function ($provide) {
    $provide.decorator('$locale', ['$delegate', function ($delegate) {
        $delegate.NUMBER_FORMATS.DECIMAL_SEP = ',';
        $delegate.NUMBER_FORMATS.GROUP_SEP = '.';
        return $delegate;
    }]);
}]);

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

app.directive('customimage', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            attrs.$observe("ng-src", function (value) {

            })
        }
    }
});

app.animation('.slide-animation', function () {
    return {
        beforeAddClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                var finishPoint = element.parent().width();
                if (scope.direction !== 'right') {
                    finishPoint = -finishPoint;
                }
                TweenMax.to(element, 0.5, { left: finishPoint, onComplete: done });
            }
            else {
                done();
            }
        },
        removeClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                element.removeClass('ng-hide');

                var startPoint = element.parent().width();
                if (scope.direction === 'right') {
                    startPoint = -startPoint;
                }

                TweenMax.fromTo(element, 0.5, { left: startPoint }, { left: 0, onComplete: done });
            }
            else {
                done();
            }
        }
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

function NeroCtrl($scope, $rootScope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder, userPersistenceService) {

    $scope.SetCookies = function () {
        userPersistenceService.setCookieData("filterurunler", JSON.stringify($scope.FilterFormModel))
    };
    $scope.GetCookies = function () {
        var filterjson = userPersistenceService.getCookieData('filterurunler');
        if (filterjson!=null) 
            $scope.FilterFormModel = JSON.parse(filterjson);
    };
    $scope.ClearCookies = function () {
        userPersistenceService.clearCookieData('filterurunler');
    };

    $scope.data = {
        textInput: 'pretext',
        options: {
            language: 'tr',
            allowedContent: true,
            entities: false
        }
    };

    $scope.Permission = [];

    $scope.lastMillis = new Date().getTime();
    $scope.getRandom = function () {
        var curMillis = new Date().getTime();
        if (curMillis - $scope.lastMillis > 5000) {
            $scope.lastMillis = curMillis;
        }
        return "?ran=" + $scope.lastMillis;
    }

    $scope.FilterFormModel = {};
    $scope.FilterFormModel.ProductSectionID = "";
    $scope.FilterFormModel.ProductMCatID = "";
    $scope.FilterFormModel.ProductCatID = "";

    $scope.FormModel = {};

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

    $scope.BundleUnbundleProduct = function () {
         
        $scope.FormModel.IsBundled = !($scope.FormModel.IsBundled || false);

        if ($scope.FormModel.IsBundled == true) {
            $scope.setActiveTab(8);
        } else {
            $scope.setActiveTab(1);
        }
         
    }
   

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

    //$scope.CmbFirms = [];
    //$scope.CmbPeriods = [];
    //$scope.CmbSalesMans = [];
    //$scope.SelectedFirm = "119";
    //$scope.SelectedPeriod = "01";

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


    $scope.CloseProductImagePopup = function () {
        $.magnificPopup.close();
    }
    // $scope.ProductImageNewRowID = 0;
    $scope.NewProductImage = function (itemtype) {

        //$scope.ProductImageNewRowID = $scope.ProductImageNewRowID - 1;

        //$scope.RESIMTMPDATA = null;

        $scope.ProductImageTitle = "";

        $scope.ProductImageForm.$setPristine();
        $scope.ProductImageForm.$setUntouched();

        $scope.ProductImageHeaderText = "BU Numarası " + $scope.FormModel.BUKOD + " - " + ((itemtype == 1) ? "Yeni Resim" : "Yeni Teknik Resim");
        $scope.ProductImageFormData = { "ID": null, "ItemType": itemtype, "ProductImageData": null, "Description": null, "Title": null, "RankNumber": null };
        $scope.ProductImagePopup = "form";

        $("#qimgx")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        //.height(50);
        $("#attachmentX").val("");
        $("#file-name").val("");

        $scope.silImgXBtnGoster = false;

        $.magnificPopup.open({
            items: {
                src: $('#modalProductImageForm')
            },
            type: 'inline',
            modal: true,
            closeOnContentClick: false,
        });
    }
    $scope.EditProductImage = function (itemtype, productimageid) {

        var idx = 0;

        $scope.ProductImageForm.$setPristine();
        $scope.ProductImageForm.$setUntouched();
        $scope.ProductImageHeaderText = "BU Numarası " +  $scope.FormModel.BUKOD + " - " + ((itemtype == 1) ? "Resim" : "Teknik Resim");

        if (itemtype == 1) {
            idx = $scope.ProductImageList.findIndex(x => x.ID === productimageid);
            $scope.ProductImageFormData = $scope.ProductImageList[idx];

        } else {
            idx = $scope.ProductTImageList.findIndex(x => x.ID === productimageid);
            $scope.ProductImageFormData = $scope.ProductTImageList[idx];
        }

        if ($scope.ProductImageFormData.FileName != null && $scope.ProductImageFormData.FileName != undefined) 
            $scope.ProductImageHeaderText += " " + $scope.ProductImageFormData.FileName.substring($scope.ProductImageFormData.FileName.lastIndexOf('/') + 1);

        $("#attachmentX").val("");
        $("#file-name").val("");
        
        $("#qimgx")
            .attr("src", $scope.ProductImageFormData.FilePath)
            .width(450);
        // .height(200);
         
        $scope.silImgXBtnGoster = true;

        $.magnificPopup.open({
            items: {
                src: $('#modalProductImageForm')
            },
            type: 'inline',
            modal: true,
            closeOnContentClick: false,
        });

        $scope.ProductImagePopup = "form";
    }
    $scope.SaveProductImageForm = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        //if (ProductImageFormData.ID == null || (ProductImageFormData.ID != null && silImgXBtnGoster == true)) {

        //    $scope.imageErr = true;

        //    toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
        //    return;
        //}


        if ($scope.FormModel.PRODID == null) {
            toastr.error("Öncelikle Ürün kaydını yapmalısınız!", "Hata");
            return;
        }

        $scope.ProductImageFormData.ProductID = $scope.FormModel.PRODID;

        var file = document.getElementById('attachmentX').files[0];

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

            $("#modalProductImageForm").LoadingOverlay("show");

            var reader = new FileReader();
            reader.readAsDataURL(file);
            var image = '';
            reader.onload = function (e) {
                image = e.target.result;
                image = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

                $scope.ProductImageFormData.FileName = file.name;  /***  bukod a göre olduğu için gerek yok ***/
                $scope.ProductImageFormData.ProductImageData = image;

                $scope.SavePost();
            };

        } else {
            $scope.ProductImageFormData.FileName = null;
            $scope.ProductImageFormData.ProductImageData = null;

            $scope.SavePost();
        }

    }
    $scope.SavePost = function () {

        $http({
            url: "/api/Product/SaveImage/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.ProductImageFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                $scope.ProductImageList = response.data.ProductImageList;
                $scope.ProductTImageList = response.data.ProductTImageList;

                toastr.success(response.data.Message);

                $("#modalProductImageForm").LoadingOverlay("hide");
                $.magnificPopup.close();

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductImageForm").LoadingOverlay("hide");
                $scope.$apply();
            }, 1000);

        });
    }
    $scope.ShowDeleteImagePopup = function (itemtype, id) {

        if (itemtype == 1) {
            idx = $scope.ProductImageList.findIndex(x => x.ID === id);
            $scope.ProductImageFormData = $scope.ProductImageList[idx];
            $scope.DeleteProductImageHeaderText = $scope.ProductImageFormData.Title;
        } else {
            idx = $scope.ProductTImageList.findIndex(x => x.ID === id);
            $scope.ProductImageFormData = $scope.ProductTImageList[idx];
            $scope.DeleteProductImageHeaderText = $scope.ProductImageFormData.Title;
        }

        $.magnificPopup.open({
            items: {
                src: $('#modalProductImageForm')
            },
            type: 'inline',
            modal: true,
            closeOnContentClick: false,
        });

        $scope.ProductImagePopup = "delete";
    }
    $scope.DeleteProductImage = function () {
        var jsondata = {
            "ID": $scope.ProductImageFormData.ID
        };
        var dt = JSON.stringify(jsondata);

        $("#modalProductImageForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/DeleteImage/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                if ($scope.ProductImageFormData.ItemType == 1) {
                    var idx = $scope.ProductImageList.findIndex(x => x.ID === $scope.ProductImageFormData.ID);
                    $scope.ProductImageList.splice(idx, 1);
                } else if ($scope.ProductImageFormData.ItemType == 2) {
                    var idx = $scope.ProductTImageList.findIndex(x => x.ID === $scope.ProductImageFormData.ID);
                    $scope.ProductTImageList.splice(idx, 1);
                }

                toastr.success(response.data.Message);

                $("#modalProductImageForm").LoadingOverlay("hide");

                $scope.CloseProductImagePopup();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductImageForm").LoadingOverlay("hide");
            }, 1000);

        });
    }


    function renderOEM(data, type, row) {
        //var parts = data.toFixed(0).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showProductOEMList(" + row.PRODID + ")'><i class=\"no-btn\">" + data + "</i></a>";
    }
    $scope.showProductOEMList = function (id) {

        $scope.OEMPopup = "oemlist";

        var table = $('#entry-grid').DataTable();
        var OEMRow = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.OEMPopupTitle = "BUKOD: " + OEMRow[0].BUKOD + " - " + OEMRow[0].ProductName + " (ID:" + OEMRow[0].PRODID + ")";
        $scope.OEMFormModel = { "PRODID": OEMRow[0].PRODID, "BUKOD": OEMRow[0].BUKOD };

        $scope.PopupPRODID = OEMRow[0].PRODID;

        $.LoadingOverlay("show");

        var jsondata = { "PRODID": OEMRow[0].PRODID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Product/OEMList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.ProductOEMList = response.data.OEMList;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductOEMForm')
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
    $scope.CloseProductOEMPopup = function () {

        var table = $('#entry-grid').DataTable();
        var rowindex = -1;
        var row = table
            .rows(function (idx, data, node) {
                // return data.PRODID === $scope.PopupPRODID ? true : false;
                if (data.PRODID === $scope.PopupPRODID) {
                    rowindex = idx;
                    return;
                }
            })
            .data();

        var d = table.row(rowindex).data();

        d.OEMCount = $scope.ProductOEMList.length;

        table
            .row(rowindex)
            .data(d)
            .draw('page');

        $.magnificPopup.close();
    }
    $scope.NewProductOEM = function () {

        $scope.OEMForm.$setPristine();
        $scope.OEMForm.$setUntouched();
        $scope.OEMHeaderText = "Yeni OEM";
        $scope.OEMFormData = { "OEMID": null, "PRODID": $scope.OEMFormModel.PRODID, "BUKOD": $scope.OEMFormModel.BUKOD, "OEMNR": "", "SUPID": null, "SUPNAME": "" };
        $scope.OEMPopup = "oemform";
    }
    $scope.EditProductOEM = function (OEMID) {

        $scope.OEMForm.$setPristine();
        $scope.OEMForm.$setUntouched();

        var idx = $scope.ProductOEMList.findIndex(x => x.OEMID === OEMID);

        $scope.OEMHeaderText = $scope.ProductOEMList[idx].OEMID + " - " + $scope.ProductOEMList[idx].OEMNR + " - " + $scope.ProductOEMList[idx].SUPNAME;
        $scope.OEMFormData = angular.copy($scope.ProductOEMList[idx]);
        $scope.OEMPopup = "oemform";
    }
    $scope.BackToProductOEMList = function (OEMID) {
        $scope.OEMPopup = "oemlist";
    }
    $scope.OEMSuplierChanged = function (id) {
        var idx = $scope.OEMSupliers.findIndex(x => x.SUPID === id);
        $scope.OEMFormData.SUPNAME = $scope.OEMSupliers[idx].SUPNAME;
    }
    function ConcatStr(item, index, arr) {
        arr[index] = item;
    }
    $scope.SaveProductOEM = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $("#modalProductOEMForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/SaveOEM/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.OEMFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                if ($scope.OEMFormData.OEMID == null) {
                    $scope.OEMFormData.OEMID = response.data.ID;
                    $scope.OEMFormData.ADDED = response.data.Time;
                    $scope.ProductOEMList.push($scope.OEMFormData);
                }
                else {
                    var idx = $scope.ProductOEMList.findIndex(x => x.OEMID === $scope.OEMFormData.OEMID);
                    $scope.OEMFormData.UPDATED = response.data.Time;
                    $scope.ProductOEMList[idx] = angular.copy($scope.OEMFormData);
                }

                toastr.success(response.data.Message);

                $("#modalProductOEMForm").LoadingOverlay("hide");
                $scope.OEMPopup = "oemlist";

                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductOEMForm").LoadingOverlay("hide");
                $scope.$apply();
            }, 1000);

        });
    }
    $scope.ShowDeleteOEMPopup = function (OEMID) {

        var idx = $scope.ProductOEMList.findIndex(x => x.OEMID === OEMID);
        $scope.OEMHeaderText = $scope.ProductOEMList[idx].OEMID + " - " + $scope.ProductOEMList[idx].OEMNR + " - " + $scope.ProductOEMList[idx].SUPNAME;
        $scope.OEMFormData = angular.copy($scope.ProductOEMList[idx]);
        $scope.OEMPopup = "oemdelete";
    }
    $scope.DeleteProductOEM = function () {

        var OEMID = $scope.OEMFormData.OEMID;

        var jsondata = {
            "OEMID": OEMID
        };
        var dt = JSON.stringify(jsondata);

        $("#modalProductOEMForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/DeleteOEM/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var idx = $scope.ProductOEMList.findIndex(x => x.OEMID === OEMID);
                $scope.ProductOEMList.splice(idx, 1);

                toastr.success(response.data.Message);

                $("#modalProductOEMForm").LoadingOverlay("hide");
                $scope.OEMPopup = "oemlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductOEMForm").LoadingOverlay("hide");
            }, 1000);

        });
    }
    $scope.oemlistFilter = function (row) {

        return (row.OEMID.toString().indexOf($scope.oemfiltertext || '') !== -1 ||
            angular.lowercase(row.OEMNR || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1 ||
            angular.lowercase(row.SUPNAME || '').toString().indexOf(angular.lowercase($scope.oemfiltertext) || '') !== -1
            //    angular.lowercase(row.OEMNR).contains(angular.lowercase($scope.oemfiltertext) || '') !== -1
        );

        //return $scope.oemfiltertext == undefined || $scope.oemfiltertext == "" ||
        //    (item.OEMID != undefined && item.OEMID.indexOf($scope.oemfiltertext) !== -1) ||
        //    (item.OEMNR != undefined && item.OEMNR.indexOf($scope.oemfiltertext) !== -1) ||
        //    item.SUPNAME.indexOf($scope.oemfiltertext) !== -1; 
    }

    function renderMU(data, type, row) {
        //var parts = data.toFixed(0).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showProductMUList(" + row.PRODID + ")'><i class=\"no-btn\">" + data + "</i></a>";
    }
    $scope.showProductMUList = function (id) {

        $scope.MUPopup = "MUlist";

        var table = $('#entry-grid').DataTable();
        var MURow = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.MUPopupTitle = "BUKOD: " + MURow[0].BUKOD + " - " + MURow[0].ProductName + " (ID:" + MURow[0].PRODID + ")";
        $scope.MUFormModel = { "PRODID": MURow[0].PRODID, "BUKOD": MURow[0].BUKOD };

        $scope.PopupPRODID = MURow[0].PRODID;

        $.LoadingOverlay("show");

        var jsondata = { "PRODID": MURow[0].PRODID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Product/MUList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.ProductMUList = response.data.MUList;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductMUForm')
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
    $scope.CloseProductMUPopup = function () {
        var table = $('#entry-grid').DataTable();
        var rowindex = -1;
        var row = table
            .rows(function (idx, data, node) {
                if (data.PRODID === $scope.PopupPRODID) {
                    rowindex = idx;
                    return;
                }
            })
            .data();

        var d = table.row(rowindex).data();

        d.MUCount = $scope.ProductMUList.length;

        table
            .row(rowindex)
            .data(d)
            .draw('page');

        $.magnificPopup.close();
    }
    $scope.ViewProductMU = function (MPID) {
        var idx = $scope.ProductMUList.findIndex(x => x.MPID === MPID);

        $scope.MUHeaderText = $scope.ProductMUList[idx].XPSNO + " - " + $scope.ProductMUList[idx].FIRMA_ADI;
        $scope.MUFormData = angular.copy($scope.ProductMUList[idx]);
        $scope.MUPopup = "MUformview";
    }
    $scope.NewProductMU = function () {

        $scope.MUForm.$setPristine();
        $scope.MUForm.$setUntouched();
        $scope.MUHeaderText = "Yeni Müşteri";
        $scope.MUFormData = { "MPID": null, "ProductID": $scope.MUFormModel.PRODID, "BUKOD": $scope.MUFormModel.BUKOD, "MUNR": "", "SUPID": null, "SUPNAME": "" };
        $scope.MUPopup = "MUform";
    }
    $scope.EditProductMU = function (MPID) {

        var idx = $scope.ProductMUList.findIndex(x => x.MPID === MPID);

        $scope.MUHeaderText = $scope.ProductMUList[idx].XPSNO + " - " + $scope.ProductMUList[idx].FIRMA_ADI;
        $scope.MUFormData = angular.copy($scope.ProductMUList[idx]);
        $scope.MUPopup = "MUform";

        $scope.MUForm.$setPristine();
        $scope.MUForm.$setUntouched();
    }
    $scope.BackToProductMUList = function () {
        $scope.MUPopup = "MUlist";
    }
    $scope.MPMusteriChanged = function (id) {
        var idx = $scope.Musteriler.findIndex(x => x.ID === id);
        $scope.MUFormData.FIRMA_ADI = $scope.Musteriler[idx].FIRMA_ADI;
    }
    $scope.MPCurencyChanged = function (id) {
        var idx = $scope.Currencies.findIndex(x => x.ID === id);
        $scope.MUFormData.CurrencyCode = $scope.Currencies[idx].Code;
    }
    $scope.SaveProductMU = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $("#modalProductMUForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/SaveMU/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.MUFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                if ($scope.MUFormData.MPID == null) {
                    $scope.MUFormData.MPID = response.data.ID;
                    $scope.MUFormData.ADDED = response.data.Time;
                    $scope.MUFormData.ADDEDBY = response.data.SavedBy;
                    $scope.ProductMUList.push($scope.MUFormData);
                }
                else {
                    var idx = $scope.ProductMUList.findIndex(x => x.MPID === $scope.MUFormData.MPID);
                    $scope.MUFormData.UPDATED = response.data.Time;
                    $scope.MUFormData.UPDATEDBY = response.data.SavedBy;
                    $scope.ProductMUList[idx] = angular.copy($scope.MUFormData);
                }
                toastr.success(response.data.Message);

                $("#modalProductMUForm").LoadingOverlay("hide");
                $scope.MUPopup = "MUlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductMUForm").LoadingOverlay("hide");
            }, 1000);

        });
    }
    $scope.ShowDeleteMUPopup = function (MPID) {
        var idx = $scope.ProductMUList.findIndex(x => x.MPID === MPID);
        $scope.MUHeaderText = $scope.ProductMUList[idx].XPSNO + " - " + $scope.ProductMUList[idx].FIRMA_ADI;
        $scope.MUFormData = angular.copy($scope.ProductMUList[idx]);
        $scope.MUPopup = "MUdelete";
    }
    $scope.DeleteProductMU = function () {

        var jsondata = {
            "MPID": $scope.MUFormData.MPID
        };
        var dt = JSON.stringify(jsondata);

        $("#modalProductMUForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/DeleteMU/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var idx = $scope.ProductMUList.findIndex(x => x.MPID === $scope.MUFormData.MPID);
                $scope.ProductMUList.splice(idx, 1);

                toastr.success(response.data.Message);

                $("#modalProductMUForm").LoadingOverlay("hide");
                $scope.MUPopup = "MUlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductMUForm").LoadingOverlay("hide");
            }, 1000);

        });
    }

    function renderTU(data, type, row) {
        //var parts = data.toFixed(0).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showProductTUList(" + row.PRODID + ")'><i class=\"no-btn\">" + data + "</i></a>";
    }
    $scope.showProductTUList = function (id) {

        $scope.TUPopup = "TUlist";

        var table = $('#entry-grid').DataTable();
        var TURow = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.TUPopupTitle = "BUKOD: " + TURow[0].BUKOD + " - " + TURow[0].ProductName + " (ID:" + TURow[0].PRODID + ")";
        $scope.TUFormModel = { "PRODID": TURow[0].PRODID, "BUKOD": TURow[0].BUKOD };

        $scope.PopupPRODID = TURow[0].PRODID;

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
    $scope.CloseProductTUPopup = function () {
        var table = $('#entry-grid').DataTable();
        var rowindex = -1;
        var row = table
            .rows(function (idx, data, node) {
                // return data.PRODID === $scope.PopupPRODID ? true : false;
                if (data.PRODID === $scope.PopupPRODID) {
                    rowindex = idx;
                    return;
                }
            })
            .data();

        var d = table.row(rowindex).data();

        d.TUCount = $scope.ProductTUList.length;

        table
            .row(rowindex)
            .data(d)
            .draw('page');

        $.magnificPopup.close();

    }
    $scope.ViewProductTU = function (TPID) {

        var idx = $scope.ProductTUList.findIndex(x => x.TPID === TPID);

        $scope.TUHeaderText = $scope.ProductTUList[idx].XPSNO + " - " + $scope.ProductTUList[idx].FIRMA_ADI + " - " + $scope.ProductTUList[idx].XPSNO;
        $scope.TUFormData = angular.copy($scope.ProductTUList[idx]);
        $scope.TUPopup = "TUFormview";
    }
    $scope.NewProductTU = function () {

        $scope.TUForm.$setPristine();
        $scope.TUForm.$setUntouched();

        $scope.TUHeaderText = "Yeni Tedarikçi";

        $scope.TUFormData = { "TPID": null, "ProductID": $scope.TUFormModel.PRODID, "BUKOD": $scope.TUFormModel.BUKOD };
        $scope.TUPopup = "TUForm";
    }
    $scope.EditProductTU = function (TPID) {

        $scope.TUForm.$setPristine();
        $scope.TUForm.$setUntouched();

        var idx = $scope.ProductTUList.findIndex(x => x.TPID === TPID);

        $scope.TUHeaderText = $scope.ProductTUList[idx].XPSNO + " - " + $scope.ProductTUList[idx].FIRMA_ADI + " - " + $scope.ProductTUList[idx].XPSNO;
        $scope.TUFormData = angular.copy($scope.ProductTUList[idx]);
        $scope.TUPopup = "TUForm";
    }
    $scope.BackToProductTUList = function (TUID) {
        $scope.TUPopup = "TUlist";
    }
    $scope.TPMusteriChanged = function (id) {
        var idx = $scope.Tedarikciler.findIndex(x => x.ID === id);
        $scope.TUFormData.FIRMA_ADI = $scope.Tedarikciler[idx].FIRMA_ADI;
    }
    $scope.TPCurencyChanged = function (id) {
        var idx = $scope.Currencies.findIndex(x => x.ID === id);
        $scope.TUFormData.CurrencyCode = $scope.Currencies[idx].Code;
    }
    $scope.SaveProductTU = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $("#modalProductTUForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/SaveTU/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.TUFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                if ($scope.TUFormData.TPID == null) {
                    $scope.TUFormData.TPID = response.data.ID;
                    $scope.TUFormData.ADDED = response.data.Time;
                    $scope.TUFormData.ADDEDBY = response.data.SavedBy;
                    $scope.ProductTUList.push($scope.TUFormData);
                }
                else {
                    var idx = $scope.ProductTUList.findIndex(x => x.TPID === $scope.TUFormData.TPID);
                    $scope.TUFormData.UPDATED = response.data.Time;
                    $scope.TUFormData.UPDATEDBY = response.data.SavedBy;
                    $scope.ProductTUList[idx] = angular.copy($scope.TUFormData);
                }
                toastr.success(response.data.Message);

                $("#modalProductTUForm").LoadingOverlay("hide");
                $scope.TUPopup = "TUlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductTUForm").LoadingOverlay("hide");
            }, 1000);

        });
    }
    $scope.ShowDeleteTUPopup = function (TPID) {

        var idx = $scope.ProductTUList.findIndex(x => x.TPID === TPID);
        $scope.TUHeaderText = $scope.ProductTUList[idx].XPSNO + " - " + $scope.ProductTUList[idx].FIRMA_ADI + " - " + $scope.ProductTUList[idx].XPSNO;
        $scope.TUFormData = angular.copy($scope.ProductTUList[idx]);
        $scope.TUPopup = "TUdelete";
    }
    $scope.DeleteProductTU = function () {

        var id = $scope.TUFormData.TPID;

        var jsondata = {
            "TPID": id
        };
        var dt = JSON.stringify(jsondata);

        $("#modalProductTUForm").LoadingOverlay("show");

        $http({
            url: "/api/Product/DeleteTU/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var idx = $scope.ProductTUList.findIndex(x => x.TPID === TPID);
                $scope.ProductTUList.splice(idx, 1);

                toastr.success(response.data.Message);

                $("#modalProductTUForm").LoadingOverlay("hide");
                $scope.TUPopup = "TUlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalProductTUForm").LoadingOverlay("hide");
            }, 1000);

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

        $scope.FormModel.RelatedProducts = $scope.RelatedProducts;
        $scope.FormModel.BundledProducts = $scope.BundledProducts;


        $http({
            url: "/api/Product/Save/"
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
                            "RESIM": response.data.Data[0].RESIM,
                            "BUKOD": response.data.Data[0].BUKOD,
                            "SectionName": response.data.Data[0].SectionName,
                            "MCatName": response.data.Data[0].MCatName,
                            "CatName": response.data.Data[0].CatName,
                            "ProductName": response.data.Data[0].ProductName,
                            "ADDED": response.data.Data[0].ADDED,
                            "ENABLED": response.data.Data[0].ENABLED,
                            "PRODID": response.data.PRODID,
                            "OEMCount": 0,
                            "MUCount": 0,
                            "TUCount": 0
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

                    d.RESIM = response.data.Data[0].RESIM;
                    d.BUKOD = response.data.Data[0].BUKOD;
                    d.SectionName = response.data.Data[0].SectionName;
                    d.MCatName = response.data.Data[0].MCatName;
                    d.CatName = response.data.Data[0].CatName;
                    d.ProductName = response.data.Data[0].ProductName;
                    d.ADDED = response.data.Data[0].ADDED;
                    d.ENABLED = response.data.Data[0].ENABLED;

                    table
                        .row(rowindex)
                        .data(d)
                        .draw('page');

                }

                $scope.FormModel.PRODID = response.data.ID;

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);
                $scope.ShowingDiv = "Liste";

                $scope.Saved = true;
                $scope.$apply();

            }, 500);
        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });
    };

    $scope.AddItem = function () {

        $scope.Saved = false;
        $scope.setActiveTab(1);
        $scope.SavedMessage = "";
        $scope.FormModel = { "ID": "", "FirmID": $scope.FirmID, "PRODID": null };


        //$scope.FormModel.EDITOR_TABLE = {
        //    textInput: 'pretext',
        //    options: {
        //        language: 'en',
        //        allowedContent: true,
        //        entities: false
        //    }
        //};

        $scope.MProducts = [];
        $scope.TProducts = [];
        $scope.ProductImageList = [];
        $scope.ProductTImageList = [];
        $scope.RelatedProducts = [];
        $scope.BundledProducts = [];

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
        $scope.silImgBtnGoster = false;

        $("#qimg2")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        // .height(200);

        $("#qimg2span").text("Dosya Seç");
        $("#attachment2").val("");
        $scope.silImg2BtnGoster = false;

        $scope.ShowingDiv = "detail";
        // $('#myModal').modal('show')
    }

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
        $scope.FormModel.DeletedResim = false;
        $scope.NewRelatedProductId = 0;
        $scope.NewBundledProductId = 0;

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
            url: "/api/Product/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.FormModel = response.data.ProductDetail;
                    $scope.ProductToOEMList = response.data.OemList;
                    $scope.MProducts = response.data.MProductlist;
                    $scope.TProducts = response.data.TProductlist;
                    $scope.ProductImageList = response.data.ProductImageList;
                    $scope.ProductTImageList = response.data.ProductTImageList;
                    $scope.B2BPriceList = response.data.B2BPriceList;
                    $scope.RelatedProducts = response.data.RelatedProducts;
                    $scope.BundledProducts = response.data.BundledProducts;

                    $("#attachment").val("");
                    if ($scope.FormModel.RESIM) {
                        $("#qimg")
                            .attr("src", '/upload/pimages/' + $scope.FormModel.RESIM)
                            .width(450);
                        // .height(200)
                        $scope.silImgBtnGoster = true;
                    } else {
                        $("#qimg")
                            .attr("src", "/content/images/noimage.png")
                            .width(150);
                        // .height(200);

                        $("#qimgspan").text("Dosya Seç");
                        $("#attachment").val("");
                        $scope.silImgBtnGoster = false;
                    }

                    $("#attachment2").val("");
                    if ($scope.FormModel.TRESIM) {
                        $("#qimg2")
                            .attr("src", '/upload/trimages/' + $scope.FormModel.TRESIM)
                            .width(150);
                        // .height(200)
                        $scope.silImg2BtnGoster = true;
                    } else {
                        $("#qimg2")
                            .attr("src", "/content/images/noimage.png")
                            .width(150);
                        // .height(200);

                        $("#qimg2span").text("Dosya Seç");
                        $("#attachment2").val("");
                        $scope.silImg2BtnGoster = false;
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
        //.success(function (data, status, headers, config) {

        //    $scope.FormModel = data;


        //    $scope.ShowingDiv = "detail";
        //})
        //.error($scope.errorcallback)
        //.finally($scope.finallycallback);

        //}, 500);
    }

    $scope.ViewPrev = function () {

        var idIndex = -1;

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {

                if ((data.PRODID + "") == ($scope.ViewID + "")) {
                    idIndex = idx;
                    return true;
                } else {
                    return false;
                }
            })
            .data();

        if (idIndex == 0) {
            toastr.warning("İlk ürün kaydındasınız!");
        } else if (idIndex > 0) {
            DTRow = table
                .rows(function (idx, data, node) {
                    return idx === (idIndex - 1) ? true : false;
                })
                .data();

            $scope.View(DTRow[0].PRODID);
        }
    }

    $scope.ViewNext = function () {

        var idIndex = -1;

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {

                if ((data.PRODID + "") == ($scope.ViewID + "")) {
                    idIndex = idx;
                    return true;
                } else {
                    return false;
                }
            })
            .data();

        if (idIndex + 1 == table.rows().count() ) {
            toastr.warning("Son ürün kaydındasınız!");
        } else  if (idIndex > -1) {
            DTRow = table
                .rows(function (idx, data, node) {
                    return idx === (idIndex + 1) ? true : false;
                })
                .data();

            $scope.View(DTRow[0].PRODID);
        }
    }

    $scope.Test = function () {

        var table = $('#entry-grid').DataTable();
        var TURow = table
            .rows(function (idx, data, node) {
                return data.PRODID === 18923 ? true : false;
            })
            .data();

        var d = table.row(TURow[0]).data();

        d.OEMCount = 999;

        table
            .row(TURow[0])
            .data(d)
            .draw();

    }

    $scope.View = function (id) {

        $scope.ViewID = id;
        $("#modalProductViewForm").LoadingOverlay("show");
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

                    //$scope.ProductImageList = response.data.ProductImageList;
                    //$scope.ProductTImageList = response.data.ProductTImageList;

                    $scope.slides = [];
                    angular.forEach(response.data.ProductImageList, function (value, key) {
                        $scope.slides.push({ image: "http://nero2021.buautomotive.net" + response.data.ProductImageList[key].FilePath + '', description: response.data.ProductImageList[key].Title });
                    });     
                    
                    

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

                    //$scope.slidesTR = [
                    //    { image: '/angular-photo-slider-master/images/img00.jpg', description: 'Image 00' },
                    //    { image: '/angular-photo-slider-master/images/img01.jpg', description: 'Image 01' },
                    //    { image: '/angular-photo-slider-master/images/img02.jpg', description: 'Image 02' },
                    //    { image: '/angular-photo-slider-master/images/img03.jpg', description: 'Image 03' },
                    //    { image: '/angular-photo-slider-master/images/img04.jpg', description: 'Image 04' }
                    //];
                    $scope.slidesTR = [];
                    angular.forEach(response.data.ProductTImageList, function (value, key) {
                        $scope.slidesTR.push({ "image": "http://nero2021.buautomotive.net" + response.data.ProductTImageList[key].FilePath, "description": response.data.ProductTImageList[key].Title });
                    });   

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
                    $scope.B2BPriceList = response.data.B2BPriceList;
                    $scope.RelatedProducts = response.data.RelatedProducts;
                    $scope.BundledProducts = response.data.BundledProducts;

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalProductViewForm')
                        },
                        type: 'inline',
                        modal: true,
                        closeOnContentClick: false,
                    });

                    //$scope.ShowingDiv = "detail";

                    //$.LoadingOverlay("hide");
                    $("#modalProductViewForm").LoadingOverlay("hide");
                    $scope.$apply();

                }, 500);
            }
                , function (error) {

                    $window.setTimeout(function () {

                        toastr.error(error.data.Message);
                        // $.LoadingOverlay("hide");
                        $("#modalProductViewForm").LoadingOverlay("hide");

                        $scope.$apply();

                    }, 2000);
                });
    }

    $scope.ClosePopup = function () {
        $.magnificPopup.close();
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
            $scope.FormModel.DeletedResim = false;

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

        if ($scope.tutorial != null)
            $scope.tutorial.attachment = null;
        // $scope.UploadFormModel.File = "";
        $scope.FormModel.DeletedResim = true;
    };
    $scope.readURLQImg2 = function (input) {
        if (input.files && input.files[1]) {
            var reader = new FileReader();

            reader.onload = function (e) {

                image = e.target.result;
                $scope.FormModel.TRESIMDATA = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

                $("#qimg2")
                    .attr("src", e.target.result);
                //.width(150);
                //.height(200)
            };

            reader.readAsDataURL(input.files[1]);
            $scope.silImg2BtnGoster = true;
            $scope.FormModel.DeletedTResim = false;

        } else {
            $("#qimg2")
                .attr("src", "/images/noimage.png")
                .width(150);
            //.height(50);
            $scope.silImg2BtnGoster = false;
        }
    };
    $scope.deleteFilesImg2 = function () {
        $scope.silImg2BtnGoster = false;

        $("#qimg2")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        //.height(50);

        $("#qimg2span").text("Dosya Seç");
        $("#attachment2").val("");

        if ($scope.tutorial != null)
            $scope.tutorial.attachment2 = null;
        // $scope.UploadFormModel.File = "";
        $scope.FormModel.DeletedTResim = true;
    };

    $scope.readURLQImgX = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {

                image = e.target.result;
                $scope.ProductImageFormData.ProductImageData = image.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');

                $("#qimgx")
                    .attr("src", e.target.result)
                    .width(450);
                //.height(200)
            };

            reader.readAsDataURL(input.files[0]);
            $scope.silImgXBtnGoster = true;

            $scope.imageErr = false;

            //$scope.FormModel.DeletedResim = false;

        } else {
            $("#qimgx")
                .attr("src", "/images/noimage.png")
                .width(150);
            //.height(50);
            $scope.silImgXBtnGoster = false;
            $scope.imageErr = true;
        }
    };
    $scope.deleteFilesImgX = function () {
        $scope.silImgXBtnGoster = false;
        $scope.ProductImageFormData.ProductImageData = null;
        $("#qimgx")
            .attr("src", "/content/images/noimage.png")
            .width(150);
        //.height(50);

        //$("#qimgspan").text("Dosya Seç");
        $("#attachmentX").val("");


        //if ($scope.tutorial != null)
        //    $scope.tutorial.attachment = null;
        // $scope.UploadFormModel.File = "";
        //$scope.FormModel.DeletedResim = true;
    };
    

    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye text-white\"></i></a>";
    }
    function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt text-white\"></i></a>";
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
        if (data == null || data == "")
            return "<img style='border-radius: 50 % !important;'  width='100' src='/images/bulogo.png" + "' />";
        else
            return "<a href='" + data + "' class='fancybox' data-fancybox='urun'><img style='border-radius: 50 % !important;'  width='100' src='" + data + "' /></a>";
    }

    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.PRODID === id ? true : false;
            })
            .data();

        $scope.DeletingProductName = ($scope.DeletingFile[0].ProductName || "");
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
            url: "/api/Product/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteProductFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                // $scope.dtInstance.rerender();

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.PRODID === $scope.DeleteProductFormModel.ID ) {
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

        $.LoadingOverlay("show");

        $scope.SetCookies();

        var table = $('#entry-grid').DataTable();

        table
            .clear()
            .draw();

        //$scope.dtInstance.rerender();
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
            DTColumnBuilder.newColumn("RESIM", "Resim").withOption('width', '100px').renderWith(renderPic).withClass('text-center').notSortable(),
            DTColumnBuilder.newColumn("BUKOD", "BU Numarası").withOption('width', '200px'),
            DTColumnBuilder.newColumn("SectionName", "Bölüm").withOption('width', '200px'),
            DTColumnBuilder.newColumn("MCatName", "Ana Kategori").withOption('width', '200px'),
            DTColumnBuilder.newColumn("CatName", "Alt Kategori").withOption('width', '200px'),
            DTColumnBuilder.newColumn("ProductName", "Ürün Adı").withOption('width', '200px'),
            DTColumnBuilder.newColumn("ADDED", "Eklenme Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("ENABLED", "Durum").withOption('width', '10px').renderWith(renderStatus).withClass('text-center')];

        if ($scope.Permission_urunler.ViewPermission == 1) //($scope.Permission.View == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Görüntüle").withOption('width', '10px').renderWith(renderView).withClass('text-center').notSortable());
        if ($scope.Permission_urunler.UpdatePermission == 1)  //($scope.Permission.Edit == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Düzenle").withOption('width', '10px').renderWith(renderEdit).withClass('text-center').notSortable());
        if ($scope.Permission_urunler.DeletePermission == 1)  //($scope.Permission.Delete == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("PRODID", "Sil").withOption('width', '10px').renderWith(renderDelete).withClass('text-center').notSortable());

        $scope.dtColumns.push(DTColumnBuilder.newColumn("OEMCount", "OEM").withOption('width', '10px').renderWith(renderOEM).withClass('text-center').notSortable());

        if ($scope.Permission_musteriurun.ViewPermission == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("MUCount", "Müşteri").withOption('width', '10px').renderWith(renderMU).withClass('text-center').notSortable());

        if ($scope.Permission_tedarikciurun.ViewPermission == 1) 
            $scope.dtColumns.push(DTColumnBuilder.newColumn("TUCount", "Tedarikçi").withOption('width', '10px').renderWith(renderTU).withClass('text-center').notSortable());
            
          
        //$scope.dtColumns = [
        //    DTColumnBuilder.newColumn("RESIM", "Resim").renderWith(renderPic).withClass('text-center'),
        //    DTColumnBuilder.newColumn("BUKOD", "BU Numarası"),
        //    DTColumnBuilder.newColumn("MCatName", "Ana Kategori"),
        //    DTColumnBuilder.newColumn("CatName", "Alt Kategori"),
        //    DTColumnBuilder.newColumn("ProductName", "Ürün Adı"),
        //    DTColumnBuilder.newColumn("ADDED", "Eklenme Tarihi").renderWith(renderDate),
        //    DTColumnBuilder.newColumn("ENABLED", "Durum").renderWith(renderStatus).withClass('text-center'),
        //    DTColumnBuilder.newColumn("PRODID", "Görüntüle").renderWith(renderView).withClass('text-center'),
        //    DTColumnBuilder.newColumn("PRODID", "Düzenle").renderWith(renderEdit).withClass('text-center'),
        //    DTColumnBuilder.newColumn("PRODID", "Sil").renderWith(renderDelete).withClass('text-center'),
        //    DTColumnBuilder.newColumn("OEMCount", "OEM").renderWith(renderOEM).withClass('text-center'),
        //    //DTColumnBuilder.newColumn("oem", "OEM").withClass('text-center'),
        //    DTColumnBuilder.newColumn("MUCount", "Müşteri").renderWith(renderMU).withClass('text-center'),
        //    DTColumnBuilder.newColumn("TUCount", "Tedarikçi").renderWith(renderTU).withClass('text-center')
        //];
        

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Product/DTList/",
            type: "POST",
            dataType: "json",
            dataSrc: "data",
            data: function (d) {
                //d.FirmCode = $scope.SelectedFirm;// $('.daterangepicker_start_input input').val();
                //d.PeriodCode = $scope.SelectedPeriod; ////$('.daterangepicker_end_input input').val();
                //d.SalesmanRef = $scope.SelectedSalesManRef;
                //d.BeginDate = $scope.start;
                //d.EndDate = $scope.end;
                
                d.SecID = $scope.FilterFormModel.ProductSectionID,
                d.MCatID = $scope.FilterFormModel.ProductMCatID,
                d.CatID = $scope.FilterFormModel.ProductCatID,
                d.OEMNo = $scope.FilterFormModel.txtFilterOEMNo
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
                '0': { type: 'select', values: [{ value: '0', label: 'Resim Yok' }, { value: '1', label: 'Resim Var' }] },
                '1': { type: 'text' },
                '2': { type: 'text' },
                '3': { type: 'text' },
                '4': { type: 'text' },
                '5': { type: 'text' },
                '6': { type: 'text' },
                '7': { type: 'select', values: [{ value: 1, label: 'Aktif' }, { value: 0, label: 'Pasif' }] } 
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

    $scope.CmbSegmentChanged = function () {
        $scope.FilterFormModel.ProductMCatID = null;
        $scope.FilterFormModel.ProductCatID = null;
    }
    $scope.CmbMCatChanged = function () {
        $scope.FilterFormModel.ProductCatID = null;
    }

    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/Product/FillAllCmb/"
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

                //$.LoadingOverlay("hide");
                
            }, function (error) {
                //$.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            });
    }

    $scope.Init = function (firmID, p) { //, v, a, e, d

        $scope.GetCookies();
        $scope.Permissions = p; //{ "View": v, "Add": a, "Edit": e, "Delete": d }; 

        angular.forEach(p, function (value, key) {
            if (p[key].Key == "urunler")
                $scope.Permission_urunler = p[key];
            else if (p[key].Key == "musteriurun")
                $scope.Permission_musteriurun = p[key];
            else if (p[key].Key == "tedarikciurun")
                $scope.Permission_tedarikciurun = p[key];
            else if (p[key].Key == "urunexceldenyukle")
                $scope.Permission_urunexceldenyukle = p[key];
        });             

        $.LoadingOverlay("show");
        $scope.FirmID = firmID;
        $scope.Listele();
        $window.setTimeout(function () {

            $scope.dtInstance = {};
            $scope.$apply();

        }, 200);
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

    //BrowseBUKOD
    $scope.NewRelatedProductId = 0;
    $scope.NewBundledProductId = 0;
    $scope.BrowseProducts = null;
    $scope.AddBrowsedProduct = function (mode) {

        $scope.browsemode = mode;

        $scope.SelProduct = null;
        $scope.currentPage = 1;
        $scope.currentPage2 = 1;
        $scope.q2 = "";


        $.magnificPopup.open({
            items: {
                src: $('#modalBrowseBUKODPopup')
            },
            type: 'inline',
            modal: true
        });

        if ($scope.BrowseProducts != null) {

            $scope.sort2('BUKOD');
            $scope.reverse2 = false;

            return;
        }

        $("#modalBrowseBUKODPopup").LoadingOverlay("show");

        var jsondata = {
            //"MusteriID": $scope.FormModel.MusteriID
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Product/BrowseList/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        })
            .then(function (response) {
                $scope.BrowseProducts = response.data.Products;

                $scope.sort2('BUKOD');
                $scope.reverse2 = false;

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

        var product = $scope.BrowseProducts.filter(item => item.PRODID == $scope.SelProduct);
        if (product.length == 0) {
            toastr.warning("Ürün kaydına ulaşılamadı!");
            return;
        }

        if ($scope.browsemode == "related") {

            $scope.NewRelatedProductId = $scope.NewRelatedProductId - 1;

            $scope.RelatedProducts.push({
                "ID": $scope.NewRelatedProductId,
                "ProductID": $scope.FormModel.PRODID,
                "RelatedBUKod": product[0].BUKOD,
                "RelatedID": product[0].PRODID,
                "Quantity": 1,
                "RankNumber": null,
                "RelatedProductName": product[0].NAME == null ? product[0].NAME_EN : product[0].NAME
            });

            toastr.warning("İlgili ürün listeye eklendi. Kaydet butonuna basınız!");
        }
        else {

            $scope.NewBundledProductId = $scope.NewBundledProductId - 1;

            $scope.BundledProducts.push({
                "ID": $scope.NewBundledProductId,
                "ProductID": $scope.FormModel.PRODID,
                "BundledBUKod": product[0].BUKOD,
                "RelatedID": product[0].PRODID,
                "Quantity": 1,
                "RankNumber": null,
                "BundledProductName": product[0].NAME == null ? product[0].NAME_EN : product[0].NAME
            });

            toastr.warning("Paket ürünü listeye eklendi. Kaydet butonuna basınız!");
        }

        $.magnificPopup.close();
    }
    $scope.DeleteRelatedProduct = function (id) {
        var idx = $scope.RelatedProducts.findIndex(x => x.ID === id);
        $scope.RelatedProducts.splice(idx, 1);
        toastr.warning("İlgili ürün listeden çıkartıldı. Kaydet butonuna basınız!");
    }

    $scope.DeleteBundledProduct = function (id) {

        var idx = $scope.BundledProducts.findIndex(x => x.ID === id);
        $scope.BundledProducts.splice(idx, 1);
        toastr.warning("Paket ürünü listeden çıkartıldı. Kaydet butonuna basınız!");
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
