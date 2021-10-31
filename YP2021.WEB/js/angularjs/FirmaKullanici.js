var app = angular.module('KullaniciApp', ['ngSanitize']);

app.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            $(element).datepicker({
                dateFormat: 'dd.mm.yy',
                showButtonPanel: true,
                showOtherMonths: true,
                prevText: "<",
                nextText: ">",
                //changeMonth: true,
                changeYear: true,
                onSelect: function (date) {
                    ctrl.$setViewValue(date);
                    ctrl.$render();
                    scope.$apply();
                }
            });
        }
    };
});


function KullaniciListCtrl($scope, $http) {

    $scope.dTable = $('#dt_basic').dataTable({
        "scrollX": true,
        "scrollY": "500px",
        "aoColumnDefs": [

                {
                    "aTargets": [4],
                    "render": function (data, type, row) {
                        // return Date.parse(data).toString('dd-MMM-yyyy'); //
                        var df = data === null ? "" : moment(new Date(data)).format('DD-MM-YYYY');
                        // return $.datepicker.formatDate('dd-mm-yy h', new Date(data)); // data.toFixed(2).replace(".", ",");
                        return df;
                    } //,

                    //"sClass": "text-align-right",
                    //"sStyle": "font-size:8px;",
                },
                //{ width: 150, targets: 2 },
                //{ width: 200, targets: 3 },
                //{ "aTargets": [9], "sClass": "text-align-center" },
                //{ width: 70, targets: 11 },
        ],

        //"sScrollX": "100%",
        //"bScrollCollapse": true,

        dom: "<'row'<'col-sm-4'l><'col-sm-4 text-align-center'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        //buttons: [
        //    {
        //        extend: 'copy',
        //        text: 'Kopyala'
        //    },
        //    {
        //        extend: 'excel',
        //        footer: true
        //    },
        //    {
        //        extend: 'pdf',
        //        footer: true
        //    },
        //    'csv'
        //    , {
        //        extend: 'print',
        //        text: 'Yazdır',
        //        footer: true
        //    }
        //],
        "language": {
            "info": "_PAGE_/_PAGES_ gösteriliyor",
            "lengthMenu": "_MENU_ kayıt göster",
            "emptyTable": "Gösterilecek kayıt bulunamadı.",
            "infoEmpty": "0 kayıttan 0 gösteriliyor.",
            "infoFiltered": "(maksimum _MAX_ kayda göre filtrelendi)",
            "decimal": ",",
            "thousands": ".",
            "loadingRecords": "Yükleniyor...",
            "processing": "İşleniyor...",
            "search": "Ara:",
            "zeroRecords": "Uygun kayıt bulunamadı",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            },
            "aria": {
                "sortAscending": ": artan şekilde sıralamak için tıklayın",
                "sortDescending": ": azalan şekilde sıralamak için tıklayın"
            }
        }
    });

    $scope.error = false;
    $scope.success = false;


    $scope.ListEmpty = true;
    $scope.selected = [{ FirmaID: "" }];

    $scope['Listele'] = function () {

        var criFirmaID = ($scope.selected.FirmaID === undefined || $scope.selected.FirmaID === null) ? "" : $scope.selected.FirmaID;

        $scope.LoadingMessage = "Liste hazırlanıyor...";
        $("#LoadingBox").fadeIn("slow", function () { });

        $scope.ResetMessages();

        var jsondata = {
            FirmaID: criFirmaID
        };
        var dt = JSON.stringify(jsondata);

        $http({
            url: "/api/Kullanici/List/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: dt
            , dataType: "json"
        })
        .success(function (data, status, headers, config) {
            $scope.dTable.dataTable().fnClearTable();
            angular.forEach(data, function (value, key) {
                $scope.dTable.dataTable().fnAddData([

                    value.UserName,
                    value.FirmaAck,
                    value.FirstName,
                    value.LastName,
                    //value.AktiflikDurumuAck,
                    value.KayitTarihi,
                    value.KayitKullaniciAck,
                    "<a class=\"btn btn-success btn-sm\" href=\"Edit/" + value.Id + "\"><i class=\"fa fa-pencil-square-o\" aria-hidden=\"true\"></i></a>",
                    "<a href=\"javascript:void(0);\" class=\"btn btn-danger btn-sm\" onclick=\"angular.element(this).scope().Delete('" + value.Id + "', '" + value.UserName + "')\"><i class=\"fa fa-trash-o fa-fw\"></i></a>",
                ]);
            });
        })
        .error($scope.errorcallback).finally($scope.finallycallback);
    }

    $scope['FillSearchControls'] = function () {

        $scope.FirmaList = [];

        ListItems($http, "/api/Firma/List", {}, "POST")
            .success(function (data, status, headers, config) {
                $scope.FirmaList = data;
            })
             .error(function (data, status, headers, config) { });
    }

    // error callback
    $scope.errorcallback = function (data, status, headers, config) {
        alert("! Bir hata oluştu : " + data);
        $('#myModal').modal('show');
        $scope.PopupMessage = data;
    };

    // finally callback
    $scope.finallycallback = function () {

        $scope.isLoad = $scope.isLoad + 1;

        if ($scope.isLoad === 1) {
            $scope.isLoad = 0;
            $("#LoadingBox").fadeOut("slow", function () { });
            $('html,body').scrollTop(0, 0);
            //$scope.$apply();
        };
    };

    $scope.Delete = function (id, username) {

        var DltMsgTitle = 'Kullanıcı silinecek, devam etmek istiyor musunuz?';
        var DltMsg = 'Kullanıcı Adı : ' + username + ' | Lojistik Reportwan';
        $.SmartMessageBox(
                            {
                                title: DltMsgTitle,
                                content: DltMsg,
                                buttons: '[Hayır][Evet]'
                            }
                        ,
                        function (ButtonPressed) {
                            if (ButtonPressed === "Evet") {

                                var table = $('#dt_basic').DataTable();
                                table.row('.selected').remove().draw(false);

                                ListItems($http, "/api/Kullanici/Delete/" + id, {}, "DELETE")
                                    .success(function (data, status, headers, config) {

                                        var table = $('#dt_basic').DataTable();
                                        table.row('.selected').remove().draw(false);

                                        $.SmartMessageBox(
                                            {
                                                title: 'Lojistik Reportwan',
                                                content: "Kullanıcı silindi." + id,
                                                buttons: "[Tamam]"
                                            }
                                        );
                                    })
                                    .error(function (data, status, headers, config) {

                                        $.SmartMessageBox(
                                          {
                                              title: 'Lojistik Reportwan',
                                              content: (data.Message != null) ? data.Message : data,
                                              buttons: "[Tamam]"
                                          }
                                      );
                                    })
                                    .catch(function (response) { console.error('Gists error', response.status, response.data); })
                                    .finally(function () { console.log("finally finished gists"); });

                            }
                            else {
                            }

                        });

    };


    $scope.Init = function () {

        $scope.isLoad = 0;
        $scope.LoadingMessage = "Sayfa Yükleniyor...";
        $("#LoadingBox").fadeIn("slow", function () { });

        $scope.FillSearchControls();
        $scope.Listele();
    }

    $scope.error = false;
    $scope.success = false;

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    $scope.Success = function () {
        return $scope.success;
    };
    $scope.Error = function () {
        return $scope.error;
    };

    $scope.Init();

}
app.controller('KullaniciListCtrl', KullaniciListCtrl);

function KullaniciEditCtrl($scope, $http) {

    // error callback
    $scope.errorcallback = function (data, status, headers, config) {
        alert("! Bir hata oluştu : " + data);
        $('#myModal').modal('show');
        $scope.PopupMessage = data;
    };

    // finally callback
    $scope.finallycallback = function () {

        $scope.isLoad = $scope.isLoad + 1;

        if ($scope.isLoad === 1) {
            $scope.isLoad = 0;
            $("#LoadingBox").fadeOut("slow", function () { });
            $('html,body').scrollTop(0, 0);
            //$scope.$apply();
        };
    };

    $scope.Init = function (id) {

        $scope.isLoad = 0;
        $scope.LoadingMessage = "Sayfa Yükleniyor...";
        $("#LoadingBox").fadeIn("slow", function () { });

        $scope.FillDropdownlistCtrls();
        $scope.BindKullaniciForm(id);
    }

    $scope['FillDropdownlistCtrls'] = function () {

        $scope.FirmaList = [];

        ListItems($http, "/api/Firma/List", {}, "POST")
            .success(function (data, status, headers, config) {
                $scope.FirmaList = data;
            })
             .error(function (data, status, headers, config) { });




    }

    $scope.error = false;
    $scope.success = false;

    $scope.ResetMessages = function () {
        $scope.error = false;
        $scope.success = false;
    };

    $scope.Success = function () {
        return $scope.success;
    };
    $scope.Error = function () {
        return $scope.error;
    };

    $scope.BindKullaniciForm = function (kullaniciID) {

        $scope.RowExists = true;

        if (kullaniciID === "") {

            //$scope.DogrulamaYapildimi = 0;

            //$scope.AnaUrGrpAck = "";
            //$scope.UrunAck = "";
            //$scope.selected.GB_KontTip = "";

            //var dtNow = dateFilter(new Date, 'dd.MM.yyyy');
            //$scope.selected.MuracaatTarihi = dtNow;
            //$scope.selected.TeslimTarihi = "";
            //$scope.selected.SatinAlmaTarihi = "";




            $scope.KullaniciDataRow = {
                "Id": ""

                //, "Firma": "SDERA"
                //, "MuracaatTarihi": dtNow
                //, "MusteriAdi": ""
                //, "GSM": ""
                //, "PN": ""
                //, "UretimHaftasi": ""
                //, "SeriNo": ""
                //, "SeriNoGirilemedi": false
                //, "GSMNoVerilmedi": false
                //, "IscilikToplam": 0
                //, "Toplam": 0
                //, "AktarimDurumuID": 1
                //, "AktarimDurumuAciklama": "Taslak"
                , "KayitTarihi": null
                , "KayitKullaniciID": null
                , "GuncellemeTarihi": null
                , "GuncellemeKullaniciID": null
                //, "IslemDurumuID": 0
            };
           
        } else {

            $scope.user = { username: "", password: "", confirmPassword: "" };


            $scope.KullaniciDataRow = [];

            ListItems($http, "/api/Kullanici/Get/" + kullaniciID, null, "GET")
                    .success(function (data, status, headers, config) {

                        $scope.RowExists = (data.length === 0 ? false : true);

                        $scope.KullaniciDataRow = data;

                        $scope.user = { username: $scope.KullaniciDataRow.UserName, password: "", confirmPassword: "" };
                    })
                    .error(function (data, status, headers, config) {
                        $scope.RowExists = false;

                        if (status == 404)
                            $scope.ErrorMsg = "Aradığınız kayıt bulunamadı.";
                        else
                            $scope.ErrorMsg = "Sistem Hatası!";
                    });
        }
         

        ListItems($http, "/api/Kullanici/GetNewUserForFirmAdmin", {}, "POST")
           .success(function (data, status, headers, config) {
               // $scope.FirmaList = data;
               if (data != null) {
                   $scope.KullaniciLimitAndAdet = data.KullaniciLimit;
                   $scope.AktifKullaniciAdet = data.AktifKullanici;
                   $scope.KalanKullaniciAdet = data.KalanAdet;
                   $scope.KullaniciDataRow.FirmaID = data.FirmaID;
                   $scope.FirmaAdminFirmaAck = data.FirmaAck;
               }

           })
            .error(function (data, status, headers, config) { });


    }

    $scope.Save = function (isValid) {

        $scope.submitted = true;

        if (!isValid) {
            return;
        }

        var yenikayitID = $scope.KullaniciDataRow.Id


        $http({
            url: "/api/AccountRegister/Save"
           , method: "POST"
           , params: {}
           , contentType: "application/json"
           , data: $scope.KullaniciDataRow
           , dataType: "json"
        })
       .success(function (data, status, headers, config) {

           // PopupMessage("Lojistik ReportWan - Mesaj ", (data != null) ? data : "İşleminiz yapılırken bilinmeyen bir hata oluştu.");
            $('#myModal').modal('show');

           var kullaniciID = data.NewID;

           var SMBMsgTitle = '';
           var SMBMsg = 'Kayıt işlemi tamamlandı.';
           $.SmartMessageBox(
                               {
                                   title: SMBMsgTitle,
                                   content: SMBMsg,
                                   buttons: '[Tamam]'
                               }
                           ,
                           function (ButtonPressed) {
                               if (yenikayitID == "")
                                   window.location.href = '/Kullanici/Edit/' + kullaniciID;
                           });

          

       })
       .error(function (data, status, headers, config) {
           PopupMessage("Lojistik ReportWan - Mesaj ", (data.Message != null) ? data.Message : "İşleminiz yapılırken bilinmeyen bir hata oluştu.");
       })
       .finally($scope.finallycallback);



    }

    $scope.ChangePassword = function (model) {

        if (!model.$valid) {
            return;
        }

        $scope.accountChangePassword = {};
        $scope.accountChangePassword.userID = $scope.KullaniciDataRow.Id;
        $scope.accountChangePassword.newpassword = $scope.user.password;
        $scope.accountChangePassword.confirmpassword = $scope.user.password;

        // $("#LoadingBox").fadeIn();

        $http({
            url: "/api/AccountChangePass/PostChangePassword"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: $scope.accountChangePassword
            , dataType: "json"
        })
        .success(function (data, status, headers, config) {
            $scope.user.password = "";
            $scope.user.confirmPassword = "";
            model.$setPristine();
            PopupMessage("Lojistik ReportWan", (data != null) ? data : "İşleminiz yapılırken bilinmeyen bir hata oluştu.");
        })
        .error(function (data, status, headers, config) {
            PopupMessage("Lojistik ReportWan", (data.Message != null) ? data.Message : "İşleminiz yapılırken bilinmeyen bir hata oluştu.");
        })
        .finally($scope.finallycallback);



    };
}
app.controller('KullaniciEditCtrl', KullaniciEditCtrl);
