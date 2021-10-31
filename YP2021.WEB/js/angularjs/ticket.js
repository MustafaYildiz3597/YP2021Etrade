var app = angular.module('NeroApp', ['ngSanitize', 'angular.filter', 'angularUtils.directives.dirPagination', 'ui.select', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter', 'ngAnimate', 'ngTouch', 'ckeditor']);

app.config(['$provide', function ($provide) {
    $provide.decorator('$locale', ['$delegate', function ($delegate) {
        $delegate.NUMBER_FORMATS.DECIMAL_SEP = ',';
        $delegate.NUMBER_FORMATS.GROUP_SEP = '.';
        return $delegate;
    }]);
}]);

//Bootstrap dropbox directive
app.directive('bsSelectbox', function ($parse) {
    return {
        restrict: 'A',
        priority: 100,
        transclude: true,
        scope: {
            themodel: '=ngModel',
            thearray: '@ngOptions',
            thechange: '&ngChange',
            defaultval: '@bsSelectbox'
        },
        template:
            '<div class="bs-selectbox btn-group">' +
            '<button class="btn dropdown-toggle" data-toggle="dropdown" type="button">' +
            '{{display}} ' +
            '<span class="caret"></span>' +
            '</button>' +
            '<ul class="dropdown-menu">' +
            '<li ng-show="defaultval">' +
            '<a href="javascript:" ng-click="change(false)"> <span>{{defaultval}}</span> </a>' +
            '</li>' +
            '<li ng-show="defaultval" class="divider"></li>' +
            '<li ng-repeat="itm in elements" ng-class="{active:itm.value==themodel}">' +
            '<a href="javascript:" ng-click="change(itm)">' +
            '<span>{{itm.label}}</span>' +
            '</a>' +
            '</li>' +
            '</ul>' +
            '<div style="display:none;" class="bs-selectbox-transclude" ng-transclude></div>' +
            '</div>',
        link: function (scope, element, attrs) {
            scope.display = '--';
            scope.elements = [];
            attrs.$observe('bsSelectbox', function (value) {
                if (value) scope.display = value;
            });
            attrs.$observe('ngOptions', function (value, element) {
                if (angular.isDefined(value)) {
                    var match, loc = {};
                    var NG_OPTIONS_REGEXP = /^\s*(.*?)(?:\s+as\s+(.*?))?(?:\s+group\s+by\s+(.*))?\s+for\s+(?:([\$\w][\$\w\d]*)|(?:\(\s*([\$\w][\$\w\d]*)\s*,\s*([\$\w][\$\w\d]*)\s*\)))\s+in\s+(.*)$/;
                    if (match = value.match(NG_OPTIONS_REGEXP)) {
                        var displayFn = $parse(match[2] || match[1]),
                            valueName = match[4] || match[6],
                            valueFn = $parse(match[2] ? match[1] : valueName),
                            valuesFn = $parse(match[7]);
                        scope.$watch(function () { return valuesFn(scope.$parent); }, function (newVal) {
                            var collection = newVal || [];
                            scope.elements = [];
                            angular.forEach(collection, function (value, key) {
                                loc[valueName] = collection[key];
                                scope.elements.push({
                                    'label': displayFn(scope.$parent, loc),
                                    'value': valueFn(scope.$parent, loc)
                                });
                            });
                            scope.setdefault();
                        });
                    }
                }
            });
            scope.$watch('themodel', function (newval, oldval) {
                scope.setdefault();
                if (angular.isFunction(scope.thechange) && (newval != oldval)) {
                    scope.thechange();
                }
            });
            scope.setdefault = function () {
                angular.forEach(scope.elements, function (value, key) {
                    if (value.value == scope.themodel) scope.display = value.label;
                });
            }
            scope.change = function (itm) {
                if (!itm) {
                    scope.display = scope.defaultval;
                    scope.themodel = "";
                } else {
                    scope.display = itm.label;
                    scope.themodel = itm.value;
                }
            }
            var elements = element.find(".bs-selectbox-transclude").children();
            if (angular.isObject(elements) && elements.length) {
                angular.forEach(elements, function (value, key) {
                    scope.elements.push({
                        'label': value.innerText,
                        'value': value.value
                    });
                });
                scope.setdefault();
            }
        },
        replace: true
    };
});

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

//app.directive('myKeyDot', function () {
//    return function (scope, element, attrs) {
//        element.bind("keydown keypress", function (event) {
//            if (event.which === 46) {
//                //scope.$apply(function () {
//                //    scope.$eval(attrs.myEnter);
//                //});

//                event.preventDefault();
//            }
//        });
//    };
//});

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

    $scope.data = {
        textInput: 'pretext',
        options: {
            language: 'tr',
            allowedContent: true,
            entities: false
        },
        required: true
    };

    /* ui-select*/
    $scope.multipleDemo = {};
    $scope.multipleDemo.selectedFirms = null;
    $scope.multipleDemo.selectedFirmsGrdAdd = null;
    $scope.FilterFormModel = {};
    $scope.FilterFormModel.TicketType = null;
    $scope.FilterFormModel.Status = null;
    $scope.FilterFormModel.Priority = null;
    $scope.FilterFormModel.Direction = "1";
    $scope.FilterFormModel.IsArchived = "false";

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
    function renderDate(data) {

        // return data + " ** " + data.toFixed(2); //data === true ? 'red' : 'green';
        //var parts = data.toFixed(2).toString().split(".");
        //parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        //return parts.join(".");

        if (data === null)
            return "";

        return formattedDate(data);
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
            url: "/api/Ticket/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

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
        $scope.FormModel = { "ID": null, "FirmID": $scope.FirmID };

        $scope.TicketHeaderText = "Yeni Ticket";

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $scope.ShowingDiv = "new";
    }

    $scope.OpenReplyForm = function () {

        $("#attachmentX").val("");
        $("#file-name").val("");

        $.magnificPopup.open({
            items: {
                src: $('#modalTicketReplyForm')
            },
            type: 'inline',
            modal: true,
            closeOnContentClick: false,
        });

    }

    $scope.SubmitTicketReplyForm = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

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

            if (file.type != "image/jpg" && file.type != "image/gif" && file.type != "image/jpeg" && file.type != "image/png" && file.type != "application/pdf"
                && file.type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && file.type != "application/vnd.ms-excel"
                && file.type != "application/msword" && file.type != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            ) {
                toastr.error("Yüklemiş olduğunuz dosya tipi jpg, jpeg, png, xls, xlsx, doc, docx, pdf formatında olmalıdır. Lütfen kontrol edip tekrar deneyiniz.", "", {
                    "closeButton": true,
                    "positionClass": "toast-top-full-width",
                    "tapToDismiss": true
                });
                return;
            }


            var reader = new FileReader();
            reader.readAsDataURL(file);
            var filedata = '';
            reader.onload = function (e) {
                filedata = e.target.result;
                var idx = filedata.indexOf(";base64,");
                if (idx == -1) {
                    toastr.error("Hatalı Dosya!");
                    return;
                }

                $.LoadingOverlay("show");

                filedata = filedata.substring(idx + 8);

                //filedata = filedata.replace('data:image/png;base64,', '').replace('data:image/jpeg;base64,', '');
                //filedata = filedata.replace('data:application/pdf;base64,', '');

                $scope.TicketReplyFormModel.DocumentType = file.type;
                $scope.TicketReplyFormModel.FileName = file.name;  /***  bukod a göre olduğu için gerek yok ***/
                $scope.TicketReplyFormModel.DocumentData = filedata;

                $scope.TicketReplyFormPost();
            };

        } else {

            $.LoadingOverlay("show");

            $scope.TicketReplyFormModel.DocumentType = null;
            $scope.TicketReplyFormModel.FileName = null;
            $scope.TicketReplyFormModel.DocumentData = null;

            $scope.TicketReplyFormPost();
        }
    }

    $scope.TicketReplyFormPost = function () {
         
        $http({
            url: "/api/Ticket/SaveReply/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.TicketReplyFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                //$scope.MusteriDocumentList = response.data.MusteriDocumentList;

                toastr.success(response.data.Message);
               
                $scope.TicketReplyFormModel = { "TicketID": $scope.TicketModel.ID };
                
                $scope.TicketReplyForm.$setPristine();
                $scope.TicketReplyForm.$setUntouched();

                $("#attachmentX").val("");
                $("#file-name").val("");

                $scope.TicketReplies = response.data.TicketReplies

                $.LoadingOverlay("hide");
                //$.magnificPopup.close();
                $scope.$apply();

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $.LoadingOverlay("hide");
            }, 1000);
        });
    }
    
    $scope.View = function (id) {

        var jsondata = {
            "ID": id
        };
        var dt = JSON.stringify(jsondata);

        $.LoadingOverlay("show");

        $http({
            url: "/api/Ticket/Detail/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.TicketModel = response.data.Ticket;
                    $scope.TicketReplies = response.data.Ticket.TicketReplies;

                    $scope.TicketReplyFormModel = { "TicketID": $scope.TicketModel.ID};
                    $scope.TicketReplyForm.$setPristine();
                    $scope.TicketReplyForm.$setUntouched();

                    $("#attachmentX").val("");
                    $("#file-name").val("");

                    var result = [];
                    angular.forEach($scope.TicketModel.Members, function (value, key) {
                        result.push(value.FullName);
                    });
                    
                    $scope.TicketModel.Members = result;

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

                    }, 200);
                });
    }

    function json2array(json) {
        var result = [];
        var keys = Object.keys(json);
        keys.forEach(function (key) {
            result.push(json[key]);
        });
        return result;
    }

    $scope.ShowDeletePopup = function (id) {

        var idx = $scope.MUList.findIndex(x => x.MPID == id);
        //$scope.MUList[idx] = angular.copy($scope.FormModel);

        $scope.DeletingMProductName = ($scope.MUList[idx].BUKOD || "");
        $scope.DeleteMProductFormModel = { "ID": id };


        $.magnificPopup.open({
            items: {
                src: $('#modalMProductDeleteForm')
            },
            type: 'inline'
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
    $scope.SendToArchive = function (id) {

        var jsondata = {
            "TicketID": id
        };
        var dt = JSON.stringify(jsondata);

        $.LoadingOverlay("show");

        $http({
            url: "/api/Ticket/SendToArchive/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.ID === id) {
                            rowindex = idx;
                            return;
                        }
                    })
                    .data();

                var d = table.row(rowindex).remove().draw('page');

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);

                $scope.$apply();

            }, 500);

        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });

    }
    $scope.RemoveFromArchive = function (id) {

        var jsondata = {
            "TicketID": id
        };
        var dt = JSON.stringify(jsondata);

        $.LoadingOverlay("show");

        $http({
            url: "/api/Ticket/TakeBackTicketMemberFromArchive/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var table = $('#entry-grid').DataTable();
                var rowindex = -1;
                var row = table
                    .rows(function (idx, data, node) {
                        if (data.ID === id) {
                            rowindex = idx;
                            return;
                        }
                    })
                    .data();

                var d = table.row(rowindex).remove().draw('page');

                $.LoadingOverlay("hide");
                toastr.success(response.data.Message);

                $scope.$apply();

            }, 500);

        }, function (error) {

            toastr.error(error.data.Message);
            $.LoadingOverlay("hide");

        });

    }


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

    $scope.Search = function (isvalid) {

        if (!isvalid) {
            toastr.warning("Arama kriterleri hatalı!");
            return;
        }

        $scope.Listele();
        //$scope.dtInstance.rerender();
    }


    function renderStatus(data) {
        return data == true ? "<button type='button' class='btn btn-circle btn-success btn-xs'>Aktif</button>" : "<button type='button' class='btn btn-circle btn-danger btn-xs'>Pasif</button>";
    }
    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye text-white\"></i></a>";
    }
    function renderArchive(data, type, row) {
        if (row.IsArchived)
            return "<span>Arşivde</span> <a href='javascript:void(0);' class='goruntule-btn-round pull-right' title=\"Arşivden Çıkar\" onclick='angular.element(this).scope().RemoveFromArchive(" + row.ID + ")' ><i class=\"fas fa-arrow-left text-white\"></i></a>";
        else
            return "<span>Güncel</span> <a href='javascript:void(0);' class='goruntule-btn-round pull-right'  title=\"Arşive Gönder!\"  onclick='angular.element(this).scope().SendToArchive(" + row.ID + ")'><i class=\"fas fa-archive text-white\"></i></a>";
    }

    $scope.Filtrele = function () {
        // $scope.dtInstance = {};
        $scope.isUserLogged = true;
        $scope.dtInstance.rerender();
    }

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            //DTColumnBuilder.newColumn("RESIM", "Resim").renderWith(renderPic).withClass('text-center'),
            DTColumnBuilder.newColumn("CreatedBy", "Oluşturan").withOption('width', '150px'),
            DTColumnBuilder.newColumn("Type", "Ticket Tipi").withOption('width', '120px'),
            DTColumnBuilder.newColumn("Priority", "Öncelik").withOption('width', '60px'),
            DTColumnBuilder.newColumn("FIRMA_ADI", "Müşteri Adı"),
            DTColumnBuilder.newColumn("Title", "Başlık"),
            DTColumnBuilder.newColumn("CreatedOn", "Eklenme Tarihi").renderWith(renderDate),
            DTColumnBuilder.newColumn("Status", "Durum").withOption('width', '80px').withClass('text-center'), //.renderWith(renderStatus).withClass('text-center'),
            DTColumnBuilder.newColumn("UnreadCount", "Okunmamış").withOption('width', '40px'),
            DTColumnBuilder.newColumn("ID", "Detay").withOption('width', '10px').renderWith(renderView).withClass('text-center'),
            DTColumnBuilder.newColumn("IsArchived", "Güncel/Arşiv").withOption('Width', '70px;').renderWith(renderArchive).withClass('text-center'),
        ];

        //if ($scope.Permission.View == 1)
        //    $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Görüntüle").renderWith(renderView).withClass('text-center').withOption('width', '5%').notSortable());
        //if ($scope.Permission.Edit == 1)
        //    $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Detay").renderWith(renderEdit).withClass('text-center').withOption('width', '5%').notSortable());
        //if ($scope.Permission.Delete == 1)
        //    $scope.dtColumns.push(DTColumnBuilder.newColumn("TID", "Sil").renderWith(renderDelete).withClass('text-center').withOption('width', '5%').notSortable());


        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');


        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Ticket/ListDT/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                d.Direction = $scope.FilterFormModel.Direction,
                    d.IsArchived = $scope.FilterFormModel.IsArchived,
                    d.TypeID = $scope.FilterFormModel.TicketType,
                    d.StatusID = $scope.FilterFormModel.Status,
                    d.PriorityID = $scope.FilterFormModel.Priority
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
            .withOption('order', [0, 'asc'])
            .withDisplayLength(10)
            .withOption('responsive', true)
            .withOption('scrollX', 'auto')
            .withOption('scrollY', '500px')

            .withOption('scrollCollapse', true)
            //.withOption('autoWidth', true)
            .withButtons([
                'print',
                'excel'
            ])
            .withLightColumnFilter({
                '0': { type: 'text' },
                '1': { type: 'select', values: $scope.tict },
                '2': { type: 'select', values: [{ value: 'Düşük', label: 'Düşük' }, { value: 'Normal', label: 'Normal' }, { value: 'Acil', label: 'Acil' }, { value: 'Çok Acil', label: 'Çok Acil' }] },
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

        //$scope.dtOptions.order = [[1, 'asc']] ;

        $scope.dtInstance = {};
        //$scope.dtInstance.rerender();
    }

    $scope.ShowingDiv = "Liste";

    $scope.FillAllCmbs = function () {

        $http({
            url: "/api/Ticket/FillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {

                $scope.TicketTypes = response.data.TicketTypes;
                $scope.TicketStatus = response.data.TicketStatus;
                $scope.TicketPriorities = response.data.TicketPriorities;
                $scope.Firmalar = response.data.Firmalar;
                $scope.Members = response.data.Members;

                //$.('#entry-grid_wrapper')
                //$scope.TicketTypes.splice(0, 0, { 'value': null, 'label': 'Hepsi' });

                $scope.isUserLogged = true;

                //$scope.Listele();

                //$scope.Firmalar = response.data.Firmalar;
                //$scope.Currencies = response.data.Currencies;
                //$scope.YetkiliKisiler = response.data.YetkiliKisiler;

                $.LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };
    }

    $scope.Init = function (v, a, e, d, tict) {
        $scope.isUserLogged = false;
        //$scope.dtInstance = null;
        //$scope.dtColumns = null;
        //$scope.dtOptions = null;

        $scope.Permission = { "View": v, "Add": a, "Edit": e, "Delete": d };

        $.LoadingOverlay("show");
        $scope.FillAllCmbs();

        $scope.tict = tict;



        $scope.Listele();
    }

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    // $scope.Init();

    $scope.BackToList = function () {
        $scope.ShowingDiv = "Liste";
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


