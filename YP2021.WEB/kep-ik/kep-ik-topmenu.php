
					<div class="row p-3 bg-white justify-content-center">
							<a href="yeni-kep-iletisi-olustur.php" class="text-dark">
								<div class="col">
									<center>
										
											<img src="img/icons/paper-plane.png"/>
											<hr style="margin: 10px 0 10px 0;">
											Yeni KEP <br> İletisi Oluştur
										
									</center>
								</div>
							</a>
							<span style="
							border:         none;
							border-left:    1px solid hsla(225, 12%, 93%,100);
							height:         auto;
							width:          1px;">
							</span>
							<a href="#" class="text-dark" data-toggle="tooltip" data-placement="bottom" title="Çalışanlarınıza ait ücret pusulalarının toplu halde bulunduğu PDF dosyasını bu alana yüklenerek çalışanlarınıza ait bordronun TCKN'ye öre eşleşmesini sağlanır.">
								<div class="col">
									<center>
										<img src="img/icons/pdf.png"/>
										<hr style="margin: 10px 0 10px 0;">
										PDF Parçala
									</center>
								</div>
							</a>
							<a href="#" class="text-dark" >
								<div class="col">
									<center>
										<img src="img/icons/pdf-2.png"/>
										<hr style="margin: 10px 0 10px 0;">
										PDF Seçimi
									</center>
								</div>
							</a>
							<a href="#" class="text-dark" data-toggle="tooltip" data-placement="bottom" title="Elinizdeki ücret pusulası Excel formatında ise, bu alandan yüklenerek PDF haline çevirilir." >
								<div class="col">
									<center>
										<img src="img/icons/excel.png"/>
										<hr style="margin: 10px 0 10px 0;">
										Excel <br> Aktarım
									</center>
								</div>
							</a>

							<a href="pdf-onizleme-ekrani.php" class="text-dark">
								<div class="col">
									<center>
										<img src="img/icons/search.png"/>
										<hr style="margin: 10px 0 10px 0;">
										PDF Önizleme
									</center>
								</div>
							</a>
							<a href="#" class="text-dark">
								<div class="col">
									<center>
										<img src="img/icons/reset.png"/>
										<hr style="margin: 10px 0 10px 0;">
										PDF'leri <br> Sıfırla
									</center>
								</div>
							</a>
							<span style="
							border:         none;
							border-left:    1px solid hsla(225, 12%, 93%,100);
							height:         auto;
							width:          1px;">
							</span>
							<a href="#" class="text-dark"  data-toggle="modal" data-target="#exampleModal2">
								<div class="col">
									<center>
										<img src="img/icons/contract.png"/>
										<hr style="margin: 10px 0 10px 0;">
										İmzalama <br> İşlemi
									</center>
								</div>
							</a>
							
							<!-- Modal -->
<div class="modal fade" id="exampleModal2" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h3 class="modal-title" id="exampleModalLabel" style="color:#0088cc;">Bordro İmzalama Ekranı</h3>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <form id="form2" class="form-horizontal form-bordered" method="post" action="#">
									<section class="card">
										<header class="card-header">
											<h2 class="card-title">Bordro İmzalama Ekranı</h2>
										</header>
										<div class="card-body">
											<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="eimza" required="">
														<option value="">e-İmza Seçiniz</option>
													</select>
												</div>
											</div>
										<div class="form-group row">
											<div class="col-sm-10">
												<input type="password" name="pin" placeholder="Pin" class="form-control">
											</div>
											<div class="col-sm-2">
												<div class="checkbox-custom checkbox-default" style="margin-top:6px;">
													<input type="checkbox"  id="checkboxExample3">
													<label for="checkboxExample3"></label>
												</div>
											</div>
										</div>
										<div class="form-group row">
											<div class="col-sm-12">
												<input type="text" name="imzanedeni" placeholder="İmza Nedeni" class="form-control">
											</div>
										</div>
										<div class="form-group row">
											<div class="col-sm-12">
												<input type="text" name="imzakonumu" placeholder="İmza Konumu" class="form-control">
											</div>
										</div>
										<div class="form-group row">
												<label class="col-lg-3 control-label text-lg-right pt-2">Kayıt Klasörü</label>
												<div class="col-lg-9">
													<div class="fileupload fileupload-new" data-provides="fileupload">
														<div class="input-append" >
															<div class="uneditable-input">
																<i class="fas fa-file fileupload-exists"></i>
																<span class="fileupload-preview"></span>
															</div>
															<span class="btn btn-default btn-file">
																<span class="fileupload-exists"><i class="fa fa-undo"></i></span>
																<span class="fileupload-new">...</span>
																<input type="file" />
															</span>
															<a href="#" class="btn btn-default fileupload-exists" data-dismiss="fileupload"><i class="fa fa-trash"></i></a>
														</div>
													</div>
												</div>
											</div>
										</div>
									</section>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-dark" data-dismiss="modal">Kapat</button>
        <button type="submit" class="btn btn-light" style="color:#0088cc;">Kaydet</button></form>
      </div>
	
    </div>
  </div>
</div>
							
							<a href="bordro-gonderim-ekrani.php" class="text-dark">
								<div class="col">
									<center>
										<img src="img/icons/mail.png"/>
										<hr style="margin: 10px 0 10px 0;">
										Bordroları <br> Gönderim Ekranı
									</center>
								</div>
							</a>
							<span style="
							border:         none;
							border-left:    1px solid hsla(225, 12%, 93%,100);
							height:         auto;
							width:          1px;">
							</span>
							<a href="calisanlar.php" class="text-dark">
								<div class="col">
									<center>
										<img src="img/icons/woman.png"/>
										<hr style="margin: 10px 0 10px 0;">
										Çalışanlar
									</center>
								</div>
							</a>
							<a href="#" class="text-dark"  data-toggle="modal" data-target="#exampleModal">
								<div class="col">
									<center>
										<img src="img/icons/add.png"/>
										<hr style="margin: 10px 0 10px 0;">
										Excel'den <br> Çalışan Ekle
									</center>
								</div>
							</a>
							<!-- Modal -->
<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h3 class="modal-title" id="exampleModalLabel" style="color:#0088cc;">Excel Çalışan Aktarımı</h3>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <form id="form2" class="form-horizontal form-bordered" method="post" action="#">
									<section class="card">
										<header class="card-header">
											<h2 class="card-title">Excel Seçenekleri</h2>
										</header>
										<div class="card-body">
											<div class="form-group row">
												<label class="col-lg-3 control-label text-lg-right pt-2">Excel Dosyası</label>
												<div class="col-lg-9">
													<div class="fileupload fileupload-new" data-provides="fileupload">
														<div class="input-append" >
															<div class="uneditable-input">
																<i class="fas fa-file fileupload-exists"></i>
																<span class="fileupload-preview"></span>
															</div>
															<span class="btn btn-default btn-file">
																<span class="fileupload-exists"><i class="fa fa-undo"></i></span>
																<span class="fileupload-new">...</span>
																<input type="file" />
															</span>
															<a href="#" class="btn btn-default fileupload-exists" data-dismiss="fileupload"><i class="fa fa-trash"></i></a>
														</div>
													</div>
												</div>
											</div>
											<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="eimza" required="">
														<option value="">Excel Sayfası</option>
													</select>
												</div>
											</div>
										</div>
									</section>
								<section class="card">
									<header class="card-header">

										<h2 class="card-title">Sütun Seçimi</h2>

									</header>
									<div class="card-body">
										<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="ad" required="">
														<option value="">Ad</option>
													</select>
												</div>
										</div>
										<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="soyad" required="">
														<option value="">Soyad</option>
													</select>
												</div>
										</div>
										<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="tckn" required="">
														<option value="">TCKN</option>
													</select>
												</div>
										</div>
										<div class="form-group row">
												<div class="col-sm-12">
													<select class="form-control mb-3" name="kepadresi" required="">
														<option value="">KEP Adresi</option>
													</select>
												</div>
										</div>
									</div>
								</section>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-dark" data-dismiss="modal">Kapat</button>
        <button type="submit" class="btn btn-light" style="color:#0088cc;">Kaydet</button></form>
      </div>

    </div>
  </div>
</div>
							<a href="#" class="text-dark" onclick="return confirm('Bütün çalışanlarınız silinecektir. Emin misiniz?');">
								<div class="col">
									<center>
										<img src="img/icons/unfollow.png"/>
										<hr style="margin: 10px 0 10px 0;">
										Çalışanları <br> Sıfırla
									</center>
								</div>
							</a>
					</div>

