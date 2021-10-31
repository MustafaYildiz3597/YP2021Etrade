var app = angular.module('NeroApp', ['ngSanitize', 'textAngular', 'scrollable-table', 'ui.mask', 'ui.select', 'angular.filter', 'datatables', 'datatables.buttons', 'datatables.options', 'datatables.light-columnfilter']);

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


function NeroCtrl($scope, $http, $timeout, $window, $filter, DTOptionsBuilder, DTColumnBuilder) {

    /* ui-select*/
    $scope.multipleDemo = {};
    $scope.multipleDemo.selectedCountry = null;


    $scope.FilterFormModel = {};
    //$scope.FilterFormModel.TeklifTipiID = "";
    //$scope.FilterFormModel.FirmID = "";

    $scope.FormModel = {};
    $scope.AddressFormData = {};
    

    //$scope.emailFormat = /^[a-z]+[a-z0-9._]+@[a-z0-9]+\.[a-z.]{2,5}$/;
    $scope.emailFormat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    $scope.dateFormat = /^(31[\/.](0[13578]|1[02])[\/.](18|19|20)[0-9]{2})|((29|30)[\/.](01|0[3-9]|1[1-2])[\/.](18|19|20)[0-9]{2})|((0[1-9]|1[0-9]|2[0-8])[\/.](0[1-9]|1[0-2])[\/.](18|19|20)[0-9]{2})|(29[\/.](02)[\/.](((18|19|20)(04|08|[2468][048]|[13579][26]))|2000))$/;


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

        //$scope.FormModel.StartDateF = $scope.FormModel.StartDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.StartDate);
        //$scope.FormModel.BirthDateF = $scope.FormModel.BirthDate.split(".").reverse().join("-"); //formattedDate($scope.FormModel.EndDate);

        $.LoadingOverlay("show");

        $http({
            url: "/api/Musteri/Save/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.FormModel
            , dataType: "json"
        }).then(function (response) {

            $scope.FormModel.OrderID = response.data.OrderID;

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
        $scope.FormModel = { "ID": null };
        $scope.FormModel.OrderItems = [];

        //$scope.DetailForm = [];

        //if ($scope.DetailForm) {
        $scope.DetailForm.$setPristine();
        $scope.DetailForm.$setUntouched();
        //}

        $scope.MusteriHeaderText = "Yeni Kayıt";


        $scope.ShowingDiv = "edit";
        // $('#myModal').modal('show')
    }

    $scope.Edit = function (id) {

        $scope.newline = -1;

       
        $.LoadingOverlay("show");

        setTimeout(function () {

            $scope.SavedMessage = "";

            $scope.FormModel = {};

            var jsondata = {
                "ID": id
            };
            var dt = JSON.stringify(jsondata);

            $http({
                url: "/api/Musteri/Detail/",
                method: "POST",
                params: {},
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.FormModel = response.data.MusteriDetail;
                        //$scope.FormModel.OrderDate = $scope.FormModel.OrderDate == null ? "" : moment($scope.FormModel.OrderDate).format('DD/MM/YYYY');

                        $scope.DetailForm.$setPristine();
                        $scope.DetailForm.$setUntouched();


                        $scope.MusteriHeaderText = " No: " + id.toString() + (" - " + response.data.MusteriDetail.FIRMA_ADI || '');

                        $scope.ID = 0;

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
                url: "/api/Musteri/Detail/",
                method: "POST",
                params: {},
                contentType: "application/json",
                data: dt,
                dataType: "json"
            })
                .then(function (response) {

                    $window.setTimeout(function () {

                        $scope.FormModel = response.data.MusteriDetail;

                        $scope.MusteriHeaderText = " No: " + id.toString() + (" - " + response.data.MusteriDetail.FIRMA_ADI || '');

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

    function renderEdit(data) {
        return "<a href='javascript:void(0);' class='duzenle-btn-round' onclick='angular.element(this).scope().Edit(\"" + data + "\")'><i class=\"fas fa-pencil-alt btn btn-success\"></i></a>";
    }
    function renderView(data) {
        return "<a href='javascript:void(0);' class='goruntule-btn-round' onclick='angular.element(this).scope().View(\"" + data + "\")'><i class=\"fas fa-eye btn btn-primary\"></i></a>";
    }
    function renderDelete(data) {
        return "<a href='javascript:void(0);' class='sil-btn-round' onclick='angular.element(this).scope().showDeletingPopup(" + data + ")'><i class=\"fas fa-trash-alt btn btn-danger\"></i></a>";
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



    function renderContactsCol(data, type, row) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showContacts(" + row.ID + ")'><i class=\"data-btn\">" + data + "</i></a>";
    }
    function renderAddressesCol(data, type, row) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showAddresses(" + row.ID + ")'><i class=\"data-btn\">" + data + "</i></a>";
    }
    function renderMeetingsCol(data, type, row) {
        return "<a href='javascript:void(0);' onclick='angular.element(this).scope().showMeetings(" + row.ID + ")'><i class=\"data-btn\">" + data + "</i></a>";
    }
    $scope.ClosePopup = function () {

        $.LoadingOverlay("show");
        $.magnificPopup.close();

        $window.setTimeout(function () {
            $scope.dtInstance.rerender();
        }, 100);
    }

    $scope.showContacts = function (id) {
        $scope.ContactPopup = "contactlist";

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {
                return data.ID === id ? true : false;
            })
            .data();

        $scope.CariContactPopupTitle = "Cari: " + DTRow[0].FIRMA_ADI;
        $scope.CariContactFormModel = { "ID": DTRow[0].ID };

        $.LoadingOverlay("show");

        var jsondata = { "ID": DTRow[0].ID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Musteri/ContactList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.CariContactList = response.data.YetkiliKisiler;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalCariContactForm')
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
    $scope.ViewCariContact = function (id) {
        var idx = $scope.CariContactList.findIndex(x => x.ID === id);
        $scope.ContactHeaderText = $scope.CariContactList[idx].ID + " - " + $scope.CariContactList[idx].FullName;
        $scope.ContactFormData = angular.copy($scope.CariContactList[idx]);
        $scope.ContactPopup = "formview";
    }
    $scope.BackToContactList = function () {
        $scope.ContactPopup = "contactlist";
    }
    $scope.AddCariContact = function () {
        $scope.ContactForm.$setPristine();
        $scope.ContactForm.$setUntouched();
        $scope.ContactHeaderText = "Yeni Yetkili Kişi";
        $scope.ContactFormData = { "ID": null, "FIRMA_ID": $scope.CariContactFormModel.ID };
        $scope.ContactPopup = "formedit";
    }
    $scope.EditCariContact = function (id) {
        var idx = $scope.CariContactList.findIndex(x => x.ID === id);
        $scope.ContactHeaderText = $scope.CariContactList[idx].ID + " - " + $scope.CariContactList[idx].FullName;
        $scope.ContactFormData = angular.copy($scope.CariContactList[idx]);
        $scope.ContactPopup = "formedit";

        $scope.ContactForm.$setPristine();
        $scope.ContactForm.$setUntouched();
    }
    $scope.SaveCariContact = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $("#modalCariContactForm").LoadingOverlay("show");

        $http({
            url: "/api/Musteri/SaveContact/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.ContactFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                $scope.ContactFormData.FullName = $scope.ContactFormData.ADI + " " + $scope.ContactFormData.SOYADI; 
                $scope.ContactFormData.ContactTitle = $scope.ContactTitles[$scope.ContactTitles.findIndex(x => x.ID === $scope.ContactFormData.ContactTitleID)].Title;

                if ($scope.ContactFormData.ID == null) {
                    $scope.ContactFormData.ID = response.data.ID;
                    $scope.ContactFormData.TARIH = response.data.Time;
                    $scope.CariContactList.push($scope.ContactFormData);
                }
                else {
                    var idx = $scope.CariContactList.findIndex(x => x.ID === $scope.ContactFormData.ID);
                    $scope.ContactFormData.UPDATED = response.data.Time;
                    $scope.CariContactList[idx] = angular.copy($scope.ContactFormData);
                }
                toastr.success(response.data.Message);

                $("#modalCariContactForm").LoadingOverlay("hide");
                $scope.ContactPopup = "contactlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalCariContactForm").LoadingOverlay("hide");
            }, 1000);

        });
    }
    $scope.ShowDeleteCariContactPopup = function (id) {
        var idx = $scope.CariContactList.findIndex(x => x.ID === id);
        $scope.ContactHeaderText = $scope.CariContactList[idx].ID + " - " + $scope.CariContactList[idx].FullName;
        $scope.ContactFormData = angular.copy($scope.CariContactList[idx]);
        $scope.ContactPopup = "formdelete";
    }
    $scope.DeleteCariContact = function () {

        var jsondata = {
            "ID": $scope.ContactFormData.ID
        };
        var dt = JSON.stringify(jsondata);

        $("#modalCariContactForm").LoadingOverlay("show");

        $http({
            url: "/api/Musteri/DeleteContact/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var idx = $scope.CariContactList.findIndex(x => x.ID === $scope.ContactFormData.ID);
                $scope.CariContactList.splice(idx, 1);

                toastr.success(response.data.Message);

                $("#modalCariContactForm").LoadingOverlay("hide");
                $scope.ContactPopup = "contactlist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalCariContactForm").LoadingOverlay("hide");
            }, 1000);

        });
    }

    $scope.showAddresses = function (id) {
        $scope.AddressPopup = "addresslist";

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {
                return data.ID === id ? true : false;
            })
            .data();

        $scope.CariAddressPopupTitle = "Cari: " + DTRow[0].FIRMA_ADI;
        $scope.CariAddressFormModel = { "ID": DTRow[0].ID };

        $.LoadingOverlay("show");

        var jsondata = { "ID": DTRow[0].ID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Musteri/AddressList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.CariAddressList = response.data.Addresses;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalCariAddressForm')
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
    $scope.ViewCariAddress = function (id) {
        var idx = $scope.CariAddressList.findIndex(x => x.ID === id);
        $scope.AddressHeaderText = $scope.CariAddressList[idx].ID + " - " + $scope.CariAddressList[idx].ADRES;
        $scope.AddressFormData = angular.copy($scope.CariAddressList[idx]);
        $scope.AddressPopup = "formview";
    }
    $scope.BackToAddressList = function () {
        $scope.AddressPopup = "addresslist";
    }
    $scope.AddCariAddress = function () {
        $scope.AddressForm.$setPristine();
        $scope.AddressForm.$setUntouched();
        $scope.AddressHeaderText = "Yeni Adres";
        $scope.AddressFormData = { "ID": null, "FIRMA_ID": $scope.CariAddressFormModel.ID  };
        $scope.AddressPopup = "formedit";
    }
    $scope.EditCariAddress = function (id) {
        var idx = $scope.CariAddressList.findIndex(x => x.ID === id);
        $scope.AddressHeaderText = $scope.CariAddressList[idx].ID + " - " + $scope.CariAddressList[idx].ADRES;
        $scope.AddressFormData = angular.copy($scope.CariAddressList[idx]);
        $scope.AddressPopup = "formedit";

        $scope.ContactForm.$setPristine();
        $scope.ContactForm.$setUntouched();
    }
    $scope.SaveCariAddress = function (isValid) {

        if (!isValid) {
            toastr.error("Lütfen Zorunlu Alanaları Doldurun", "Hata");
            return;
        }

        $("#modalCariAddressForm").LoadingOverlay("show");

        $http({
            url: "/api/Musteri/SaveAddress/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.AddressFormData
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                $scope.AddressFormData.AdresTipi = $scope.AdresTipleri[$scope.AdresTipleri.findIndex(x => x.ID === $scope.AddressFormData.TIP)].Name;
                $scope.AddressFormData.UlkeAd = $scope.Ulkeler[$scope.Ulkeler.findIndex(x => x.UID === $scope.AddressFormData.Ulke)].UNAME;
                $scope.AddressFormData.SehirAd = $scope.Sehirler[$scope.Sehirler.findIndex(x => x.ID === $scope.AddressFormData.Sehir)].NAME;

                if ($scope.AddressFormData.ID == null) {
                    $scope.AddressFormData.ID = response.data.ID;
                    $scope.AddressFormData.TARIH = response.data.Time;
                    $scope.CariAddressList.push($scope.AddressFormData);
                }
                else {
                    var idx = $scope.CariAddressList.findIndex(x => x.ID === $scope.AddressFormData.ID);
                    $scope.AddressFormData.UPDATED = response.data.Time;
                    $scope.CariAddressList[idx] = angular.copy($scope.AddressFormData);
                }
                toastr.success(response.data.Message);

                $("#modalCariAddressForm").LoadingOverlay("hide");
                $scope.AddressPopup = "addresslist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalCariAddressForm").LoadingOverlay("hide");
            }, 1000);

        });
    }
    $scope.ShowDeleteCariAddressPopup = function (id) {
        var idx = $scope.CariAddressList.findIndex(x => x.ID === id);
        $scope.AddressHeaderText = $scope.CariAddressList[idx].ID + " - " + $scope.CariAddressList[idx].ADRES;
        $scope.AddressFormData = angular.copy($scope.CariAddressList[idx]);
        $scope.AddressPopup = "formdelete";
    }
    $scope.DeleteCariAddress = function () {

        var jsondata = {
            "ID": $scope.AddressFormData.ID
        };
        var dt = JSON.stringify(jsondata);

        $("#modalCariAddressForm").LoadingOverlay("show");

        $http({
            url: "/api/Musteri/DeleteAddress/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                var idx = $scope.CariAddressList.findIndex(x => x.ID === $scope.AddressFormData.ID);
                $scope.CariAddressList.splice(idx, 1);

                toastr.success(response.data.Message);

                $("#modalCariAddressForm").LoadingOverlay("hide");
                $scope.AddressPopup = "addresslist";
            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalCariAddressForm").LoadingOverlay("hide");
            }, 1000);

        });
    }


    $scope.showMeetings = function (id) {
        $scope.MeetingPopup = "meetinglist";

        var table = $('#entry-grid').DataTable();
        var DTRow = table
            .rows(function (idx, data, node) {
                return data.ID === id ? true : false;
            })
            .data();

        $scope.CariMeetingPopupTitle = "Cari: " + DTRow[0].FIRMA_ADI;
        $scope.CariMeetingFormModel = { "ID": DTRow[0].ID };

        $.LoadingOverlay("show");

        var jsondata = { "ID": DTRow[0].ID };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Musteri/MeetingList/",
            method: "POST",
            params: {},
            contentType: "application/json",
            data: dt,
            dataType: "json"
        })
            .then(function (response) {

                $window.setTimeout(function () {

                    $scope.CariMeetingList = response.data.Meetings;

                    $.LoadingOverlay("hide");

                    $.magnificPopup.open({
                        items: {
                            src: $('#modalCariMeetingForm')
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
    $scope.ViewCariMeeting = function (id) {
        var idx = $scope.CariMeetingList.findIndex(x => x.ID === id);
        $scope.MeetingHeaderText = $scope.CariMeetingList[idx].ID + " - " + $scope.CariMeetingList[idx].TITLE;
        $scope.MeetingFormData = angular.copy($scope.CariMeetingList[idx]);
        $scope.MeetingPopup = "formview";
    }
    $scope.BackToMeetingList = function () {
        $scope.MeetingPopup = "meetinglist";
    }





    $scope.showDeletingPopup = function (id) {

        var table = $('#entry-grid').DataTable();
        $scope.DeletingFile = table
            .rows(function (idx, data, node) {
                return data.ID === id ? true : false;
            })
            .data();

        $scope.DeletingTitle = ($scope.DeletingFile[0].FIRMA_ADI || "");
        $scope.DeleteMusteriFormModel = { "ID": id };
        $scope.$apply();

        $.magnificPopup.open({
            items: {
                src: $('#modalMusteriDeleteForm')
            },
            type: 'inline'
        });

    }
    $scope.deleteMusteri = function () {

        $("#modalMusteriDeleteForm").LoadingOverlay("show");

        $http({
            url: "/api/Musteri/Delete/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.DeleteMusteriFormModel
            , dataType: "json"
        }).then(function (response) {

            $window.setTimeout(function () {

                toastr.success(response.data);
                $scope.dtInstance.rerender();

                $("#modalMusteriDeleteForm").LoadingOverlay("hide");
                $('modalMusteriDeleteForm').magnificPopup('close');

            }, 1000);

        }, function (error) {
            $window.setTimeout(function () {
                toastr.error(error.data.Message);
                $("#modalMusteriDeleteForm").LoadingOverlay("hide");
            }, 1000);

        });

    }

    $scope.Filtrele = function () {
        $scope.dtInstance.rerender();
    }

    $scope.Search = function (isvalid) {

        if (!isvalid) {
            toastr.warning("Arama kriterleri hatalı!");
            return;
        }

        $scope.dtInstance.rerender();
    }

    $scope['Listele'] = function (start, end) {

        $scope.dtColumns = [
            DTColumnBuilder.newColumn("ID", "ID"),
            DTColumnBuilder.newColumn("FirmaTipi", "FIRMA TIPI"),
            DTColumnBuilder.newColumn("FIRMA_ADI", "FIRMA_ADI"),
            DTColumnBuilder.newColumn("Ulke", "ÜLKE"),
            DTColumnBuilder.newColumn("Sehir", "SEHIR"),
            DTColumnBuilder.newColumn("WEB_STESI", "WEB SİTESI"),
            DTColumnBuilder.newColumn("FIRMNICK", "FIRMNICK"),
            DTColumnBuilder.newColumn("YetkiliKisiCount", "Yet.Kişiler").renderWith(renderContactsCol).withClass('text-center'),
            DTColumnBuilder.newColumn("AdresCount", "Adresler").renderWith(renderAddressesCol).withClass('text-center'),
            DTColumnBuilder.newColumn("ToplantiCount", "Toplantılar").renderWith(renderMeetingsCol).withClass('text-center'),
            DTColumnBuilder.newColumn("ID", "Görüntüle").renderWith(renderView).withClass('text-center'),
            DTColumnBuilder.newColumn("ID", "Detay").renderWith(renderEdit).withClass('text-center'),
            DTColumnBuilder.newColumn("ID", "Sil").renderWith(renderDelete).withClass('text-center'),
        ];

        //$scope.dtColumns[6].visible = false;

        // $scope.dtOptions.withLanguageSource('//cdn.datatables.net/plug-ins/1.10.9/i18n/French.json');

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/api/Musteri/CarilerlerPageDTList/",
            type: "POST",
            dataType: "json",
            data: function (d) {
                d.FirmaTipiID = ($scope.FilterFormModel.FirmTypeID || null),
                    d.UlkeID = ($scope.multipleDemo.selectedCountry == null ? null : $scope.multipleDemo.selectedCountry.UID)
            },

            complete: function (xhr, textStatus) {
                $.LoadingOverlay("hide");
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
                '6': { type: 'text' },
                '7': { type: 'text' },
                '8': { type: 'text' },
                '9': { type: 'text' }
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
            })
            ;

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

        //$scope.dtColumns[6].display = false;

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

    $scope.criFilterUlkeler = function () {
        return function (item) {
            return item.UID === $scope.AddressFormData.Ulke;
        };
    };
    $scope.criFilterCariUlkeler = function () {
        return function (item) {
            return item.UID === $scope.FormModel.ULKE;
        };
    };


       $scope.FillAllCmbs = function () {

        $http({
            url: "/api/Musteri/CarilerPageFillAllCmb/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
            .then(function (response) {
                $scope.FirmaTipleri = response.data.FirmaTipleri;
                $scope.Ulkeler = response.data.Ulkeler;
                $scope.Sehirler = response.data.Sehirler;
                $scope.ContactTitles = response.data.ContactTitles;
                $scope.AdresTipleri = response.data.AdresTipleri;
                $scope.CariVadeAltRakamList = response.data.CariVadeAltRakamList;
                $scope.CariVadeList = response.data.CariVadeList;
                $scope.CariOdemeSekliList = response.data.CariOdemeSekliList;
                $scope.CariTeslimatSekliList = response.data.CariTeslimatSekliList;
                $scope.CariNakliyeOdemesiList = response.data.CariNakliyeOdemesiList;

                $scope.SearchForm.$setPristine();
                $scope.SearchForm.$setUntouched();

                $.LoadingOverlay("hide");
            })
            , function (error) {
                $.LoadingOverlay("hide");
                toastr.error(error.data.Message);
            };
    }

    $scope.Init = function (firmID) {
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

    $scope.newline = -1;
    $scope.TDInlineAdd = function () {

        var pushObj = {
            "ORDID": $scope.newline
        };
        $scope.FormModel.OrderItems.push(pushObj);
        var itemID = $scope.newline;


        $scope.ORDID = itemID;

        var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);

        $scope.FormModel.OrderItems[idx].error = false;
        $scope.FormModel.OrderItems[idx].errortext = "";

        $scope.TmpItem = angular.copy($scope.FormModel.OrderItems[idx]);
    }

    $scope.TDInlineDelete = function (itemID) {

        if ($scope.FormModel.DeletedItems == null)
            $scope.FormModel.DeletedItems = [];

        var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);

        if (itemID < 0) { // newlines listeye eklenmez.
            var pushObj = {
                "ORDID": itemID
            };
            $scope.FormModel.DeletedItems.push(pushObj);
        }

        $scope.FormModel.OrderItems.splice(idx, 1);
    }

    $scope.TDInlineEdit = function (itemID) {

        $scope.ORDID = itemID;

        var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);

        $scope.FormModel.OrderItems[idx].error = false;
        $scope.FormModel.OrderItems[idx].errortext = "";

        $scope.TmpItem = angular.copy($scope.FormModel.OrderItems[idx]);

        if ($scope.newline == itemID)
            $scope.newline = $scope.newline - 1;
    }

    $scope.TDInlineCancel = function (itemID) {

        if (itemID < 0) {
            var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);
            $scope.FormModel.OrderItems.splice(idx, 1);
        } else {
            var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);
            $scope.FormModel.OrderItems[idx] = angular.copy($scope.TmpItem);

            $scope.FormModel.OrderItems[idx].error = false;
            $scope.FormModel.OrderItems[idx].errortext = "";
        }

        $scope.ORDID = 0;

    }

    $scope.TDInlineUpdate = function (itemID) {

        var idx = $scope.FormModel.OrderItems.findIndex(x => x.ORDID === itemID);

        var item = $scope.FormModel.OrderItems[idx];

        $scope.FormModel.OrderItems[idx].error = false;
        $scope.FormModel.OrderItems[idx].errortext = "";

        if (
            (item.CustomerCode == "" || item.CustomerCode == undefined)
            && (item.BuCode == "" || item.BuCode == undefined)
        ) {
            $scope.FormModel.OrderItems[idx].error = true;
            $scope.FormModel.OrderItems[idx].errortext = "Müşteri kodu ya da BUKOD girmelisiniz!";
            return;
        } else if (item.Quantity == "" || item.Quantity == undefined) {
            $scope.FormModel.OrderItems[idx].error = true;
            $scope.FormModel.OrderItems[idx].errortext = "Miktar girmelisiniz!";
            return;
        } else if (item.UnitPrice == "" || item.UnitPrice == undefined) {
            $scope.FormModel.OrderItems[idx].error = true;
            $scope.FormModel.OrderItems[idx].errortext = "Birim Fiyatı girmelisiniz!";
            return;
        } else if (item.CurrencyID == "" || item.CurrencyID == undefined) {
            $scope.FormModel.OrderItems[idx].error = true;
            $scope.FormModel.OrderItems[idx].errortext = "Para Birimi seçmelisiniz!";
            return;
        }

        var currencyCode = $scope.Currencies[$scope.Currencies.findIndex(x => x.ID === item.CurrencyID)].Code;

        $scope.FormModel.OrderItems[idx].CurrencyCode = currencyCode;

        $scope.ORDID = 0;
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
