# ��renci Platformu Projesi

Bu proje, bir ��renci bilgi sistemini hayata ge�irmek amac�yla geli�tirilmi�tir. Proje, modern web uygulama mimarilerine uygun olarak, birbirinden ba��ms�z �al��an iki ana katmandan olu�maktad�r:

1.  **OgrenciPortali (ASP.NET MVC)**: Kullan�c� aray�z� ve istemci taraf� etkile�imlerinden sorumlu olan �n y�z projesidir.
2.  **OgrenciPortalApi (ASP.NET Web API)**: T�m i� mant���, veri eri�imi ve servis hizmetlerini sunan arka y�z projesidir.

Bu mimari sayesinde, gelecekte farkl� istemciler (mobil uygulama, masa�st� uygulamas� vb.) geli�tirilmesi durumunda ayn� API altyap�s� kolayca kullan�labilir.

## Projeler

-   **OgrenciPortali**: Kullan�c�lar�n (��renci, dan��man, admin) sisteme giri� yapabildi�i, ders se�imi, not g�r�nt�leme gibi i�lemleri ger�ekle�tirebildi�i web aray�z�. Detayl� bilgi i�in [OgrenciPortali/README.md](OgrenciPortali/README.md) dosyas�na g�z at�n.
-   **OgrenciPortalApi**: Sistemin beyni olarak �al��an, veritaban� i�lemlerini y�r�ten ve MVC projesine veri sa�layan RESTful servis. Detayl� bilgi i�in [OgrenciPortalApi/README.md](OgrenciPortalApi/README.md) dosyas�na g�z at�n.

## Kurulum ve �al��t�rma

Her projenin kendi ba��ml�l�klar� ve kurulum ad�mlar� bulunmaktad�r. L�tfen ilgili projenin `README.md` dosyas�n� takip ediniz. Genel olarak ad�mlar �unlard�r:

1.  Veritaban� ba�lant� ayarlar�n� yap�n.
2.  API projesini ba�lat�n.
3.  MVC projesini ba�lat�n.