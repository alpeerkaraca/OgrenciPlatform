# Öðrenci Platformu Projesi

Bu proje, bir öðrenci bilgi sistemini hayata geçirmek amacýyla geliþtirilmiþtir. Proje, modern web uygulama mimarilerine uygun olarak, birbirinden baðýmsýz çalýþan iki ana katmandan oluþmaktadýr:

1.  **OgrenciPortali (ASP.NET MVC)**: Kullanýcý arayüzü ve istemci tarafý etkileþimlerinden sorumlu olan ön yüz projesidir.
2.  **OgrenciPortalApi (ASP.NET Web API)**: Tüm iþ mantýðý, veri eriþimi ve servis hizmetlerini sunan arka yüz projesidir.

Bu mimari sayesinde, gelecekte farklý istemciler (mobil uygulama, masaüstü uygulamasý vb.) geliþtirilmesi durumunda ayný API altyapýsý kolayca kullanýlabilir.

## Projeler

-   **OgrenciPortali**: Kullanýcýlarýn (öðrenci, danýþman, admin) sisteme giriþ yapabildiði, ders seçimi, not görüntüleme gibi iþlemleri gerçekleþtirebildiði web arayüzü. Detaylý bilgi için [OgrenciPortali/README.md](OgrenciPortali/README.md) dosyasýna göz atýn.
-   **OgrenciPortalApi**: Sistemin beyni olarak çalýþan, veritabaný iþlemlerini yürüten ve MVC projesine veri saðlayan RESTful servis. Detaylý bilgi için [OgrenciPortalApi/README.md](OgrenciPortalApi/README.md) dosyasýna göz atýn.

## Kurulum ve Çalýþtýrma

Her projenin kendi baðýmlýlýklarý ve kurulum adýmlarý bulunmaktadýr. Lütfen ilgili projenin `README.md` dosyasýný takip ediniz. Genel olarak adýmlar þunlardýr:

1.  Veritabaný baðlantý ayarlarýný yapýn.
2.  API projesini baþlatýn.
3.  MVC projesini baþlatýn.