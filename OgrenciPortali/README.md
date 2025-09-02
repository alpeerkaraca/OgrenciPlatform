# OgrenciPortali - MVC Projesi

Bu proje, ��renci Platformu'nun kullan�c� aray�z�n� olu�turan ASP.NET MVC projesidir. Kullan�c�lar�n sistemle etkile�ime girdi�i t�m ekranlar burada yer al�r.

## Sorumluluklar�

-   Kullan�c� giri�i, kayd� ve yetkilendirme y�netimi aray�zleri.
-   ��renciler i�in ders se�me ve g�r�nt�leme ekranlar�.
-   Dan��manlar i�in ��renci ve ders onay� i�lemleri.
-   Adminler i�in kullan�c�, b�l�m, ders ve d�nem y�netimi panelleri.
-   `OgrenciPortalApi` projesi ile HTTP istekleri �zerinden ileti�im kurarak verileri g�r�nt�lemek ve i�lemek.
-   **Real-time Form Validation**: E-posta adresleri i�in anl�k varlık kontrol� ve kullan�c� geri bildirimi.

## Teknolojiler

-   ASP.NET MVC 5
-   Razor View Engine
-   Bootstrap & CSS
-   jQuery & JavaScript - Real-time form validation ve AJAX istekleri i�in
-   Entity Framework (Veritaban� i�lemleri i�in API'ye istek atar)
-   **Async/Await**: E-posta validation i�in asenkron API istekleri

## Kurulum ve �al��t�rma

1.  **API Ba�lant�s�**: Projenin `Web.config` veya `.env` dosyas�nda yer alan `ApiBaseAddress` ayar�n�n, �al��an `OgrenciPortalApi` projesinin adresini do�ru bir �ekilde g�sterdi�inden emin olun.
2.  **Ba��ml�l�klar**: Proje i�in gerekli NuGet paketlerinin y�kl� oldu�undan emin olun.
    ```bash
    Update-Package -reinstall
    ```
3.  **Ba�latma**: Projeyi Visual Studio �zerinden IIS Express ile ba�latabilirsiniz. Proje, `http://localhost:3000/` portunda �al��acak �ekilde ayarlanm��t�r.