var app = angular.module('FirmaApp', ['ngSanitize']);

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


function FirmaListCtrl($scope, $http) {

    $scope.dTable = $('#dt_basic').dataTable({
        "scrollX": true,
        "scrollY": "500px",
        "aoColumnDefs": [

                {
                    "aTargets": [11],
                    "render": function (data, type, row) {
                        // return Date.parse(data).toString('dd-MMM-yyyy'); //
                        var df = data == null ? "" : moment(new Date(data)).format('DD-MM-YYYY');
                        // return $.datepicker.formatDate('dd-mm-yy h', new Date(data)); // data.toFixed(2).replace(".", ",");
                        return df;
                    } //,

                    //"sClass": "text-align-right",
                    //"sStyle": "font-size:8px;",
                },
                { width: 150, targets: 2 },
                { width: 200, targets: 3 },
                { "aTargets": [9], "sClass": "text-align-center" },
                { width: 70, targets: 11 },

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

    $scope['Listele'] = function () {

        $scope.LoadingMessage = "Liste hazırlanıyor...";
        $("#LoadingBox").fadeIn("slow", function () { });

        $scope.ResetMessages();

        $http({
            url: "/api/Firma/List/"
            , method: "POST"
            , params: {}
            , contentType: "application/json"
            , data: {}
            , dataType: "json"
        })
        .success(function (data, status, headers, config) {
            $scope.dTable.dataTable().fnClearTable();
            angular.forEach(data, function (value, key) {
                $scope.dTable.dataTable().fnAddData([

                    value.FirmaID,
                    value.Firma,
                    value.FirmaAck,
                    value.Adres,
                    value.Semt,
                    value.Sehir,
                    value.KontakKisi,
                    value.KontakTelefon,
                    value.KontakEmail,
                    value.KullaniciAdedi + " / " + value.VarolanKullaniciAdedi,
                    value.AktiflikDurumuAck,
                    value.KayitTarihi,
                    value.KayitKullaniciAck,
                    "<a class=\"btn btn-success btn-sm\" href=\"Edit/" + value.FirmaID + "\"><i class=\"fa fa-pencil-square-o\" aria-hidden=\"true\"></i></a>",
                    "<a href=\"javascript:void(0);\" class=\"btn btn-danger btn-sm\" onclick=\"angular.element(this).scope().Delete(" + value.FirmaID + ")\"><i class=\"fa fa-trash-o fa-fw\"></i></a>",
                ]);
            });
        })
        .error($scope.errorcallback).finally($scope.finallycallback);
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

    $scope.Delete = function (id) {

        var DltMsgTitle = 'Firma silinecek, devam etmek istiyor musunuz?';
        var DltMsg = 'Firma No : ' + id + ' | Lojistik Reportwan';
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

                                ListItems($http, "/api/Firma/Delete/" + id, {}, "DELETE")
                                    .success(function (data, status, headers, config) {

                                        var table = $('#dt_basic').DataTable();
                                        table.row('.selected').remove().draw(false);

                                        $.SmartMessageBox(
                                            {
                                                title: 'Lojistik Reportwan',
                                                content: "Kayıt silindi." + id,
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
                                //
                            }

                        });

    };


    $scope.Init = function () {

        $scope.isLoad = 0;
        $scope.LoadingMessage = "Sayfa Yükleniyor...";
        $("#LoadingBox").fadeIn("slow", function () { });

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
app.controller('FirmaListCtrl', FirmaListCtrl);

function FirmaEditCtrl($scope, $http) {

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

        $scope.BindFirmaForm(id);
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

    $scope.BindFirmaForm = function (firmaID) {

        $scope.RowExists = true;

        if (firmaID == 0) {

            //$scope.DogrulamaYapildimi = 0;

            //$scope.AnaUrGrpAck = "";
            //$scope.UrunAck = "";
            //$scope.selected.GB_KontTip = "";

            //var dtNow = dateFilter(new Date, 'dd.MM.yyyy');
            //$scope.selected.MuracaatTarihi = dtNow;
            //$scope.selected.TeslimTarihi = "";
            //$scope.selected.SatinAlmaTarihi = "";

            $scope.FirmaDataRow = {
                "FirmaID": 0
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
                , "KullaniciAdedi" : 3
                , "VarolanKullaniciAdedi" : 0
                , "KayitTarihi": null
                , "KayitKullaniciID": null
                , "GuncellemeTarihi": null
                , "GuncellemeKullaniciID": null
                //, "IslemDurumuID": 0
            };

            //$scope.IslemFisiDataRow.Notlar = "";
            //$scope.AU_SN = "";

        } else {

            $scope.FirmaDataRow = [];

            ListItems($http, "/api/Firma/Get/" + firmaID, null, "GET")
                    .success(function (data, status, headers, config) {

                        $scope.RowExists = (data.length == 0 ? false : true);

                        $scope.FirmaDataRow = data[0];

                        //$scope.AU_SN = $scope.IslemFisiDataRow.AU_SN;
                        //// $scope.AnaUrGrpSelected = $scope.IslemFisiDataRow.AnaUrGrp;

                        //$scope.AnaUrGrpAck = $scope.IslemFisiDataRow.AnaUrGrpAck;
                        //$scope.UrunAck = $scope.IslemFisiDataRow.PNAck;

                        //$scope.AnaUrGrpTmp = $scope.IslemFisiDataRow.AnaUrGrp;
                        //$scope.PNTmp = $scope.IslemFisiDataRow.PN;
                        //$scope.IscilikToplam = $scope.IslemFisiDataRow.IscilikToplam;

                        //$scope.fillPNSelect($scope.IslemFisiDataRow.AnaUrGrp);

                        //$scope.GetYPFigureListByUrun();

                        //$scope.selected.MuracaatTarihi = dateFilter($scope.IslemFisiDataRow.MuracaatTarihi, 'dd.MM.yyyy');
                        //$scope.selected.TeslimTarihi = dateFilter($scope.IslemFisiDataRow.TeslimTarihi, 'dd.MM.yyyy');
                        //$scope.selected.SatinAlmaTarihi = dateFilter($scope.IslemFisiDataRow.SatinAlmaTarihi, 'dd.MM.yyyy');

                        //$scope.selected.GB_KontTip = $scope.IslemFisiDataRow.GB_KontrolTip;
                    })
                    .error(function (data, status, headers, config) {
                        $scope.RowExists = false;

                        if (status == 404)
                            $scope.ErrorMsg = "Aradığınız kayıt bulunamadı.";
                        else
                            $scope.ErrorMsg = "Sistem Hatası!";
                    });
        }
        // if ($scope.IslemFisiDataRow == null
    }

    $scope.Save = function (isValid) {

        $scope.submitted = true;

        if (!isValid) {
            return;
        }

        var yenikayitFirmaID = $scope.FirmaDataRow.FirmaID

        $http({
            url: "/api/Firma/Save/" //+ ID
                   , method: "POST"
                   , params: {}
                   , contentType: "application/json"
                   , data: $scope.FirmaDataRow
                   , dataType: "json"
        })
        .success(function (data, status, headers, config) {
            // PopupMessage("Kayıt işlemi tamamlandı.");

            var firmaID = data.FirmaID;

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
                                if (yenikayitFirmaID == 0)
                                    window.location.href = '/Firma/Edit/' + firmaID;
                            });
        })
        .error(function (data, status, headers, config) {
            PopupMessage("Uygulama Mesajı", (data.Message != null) ? data.Message : data);
            //$scope.ErrorMessage = "Sistem hatası! Durum Kodu:" + status;
            //$scope.error = true; 
        })
        .catch(function (response) {
            //PopupMessage("Sistem hatası!", "Lütfen tekrar deneyiniz."); console.error('Gists error', response.status, response.data);
        })
        .finally(function () {
        });

    }
  
}
app.controller('FirmaEditCtrl', FirmaEditCtrl);
