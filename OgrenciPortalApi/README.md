# OgrenciPortalApi - Web API Projesi

Bu proje, ��renci Platformu'nun t�m i� mant���n� ve veri eri�im katman�n� i�eren ASP.NET Web API projesidir. RESTful prensiplerine uygun olarak servis hizmeti sunar.

## Sorumluluklar�

-   Kullan�c� do�rulama ve JWT (JSON Web Token) tabanl� yetkilendirme.
-   ��renci, dan��man, ders, b�l�m ve d�nemlerle ilgili t�m CRUD (Create, Read, Update, Delete) i�lemleri.
-   Veritaban� ile do�rudan ileti�im kurarak veri tutarl�l���n� sa�lamak.
-   �� kurallar�n� (ders kontenjan�, �ak��ma kontrol� vb.) uygulamak.
-   �stemci uygulamalar�na (�rne�in `OgrenciPortali`) JSON format�nda veri sunmak.

## Teknolojiler

-   ASP.NET Web API 2
-   Entity Framework (Database First yakla��m� kullan�lm��)
-   JWT (JSON Web Token) ile token tabanl� kimlik do�rulama.
-   BCrypt.Net-Next ile parola hash'leme.
-   Swagger (API dok�mantasyonu i�in).

## Kurulum ve �al��t�rma

1.  **Veritaban�**: Proje, Entity Framework Database First yakla��m�n� kullanmaktad�r. `Web.config` dosyas�ndaki `connectionStrings` b�l�m�n� kendi veritaban� sunucunuza g�re d�zenleyin.
2.  **Environment Dosyas�**: Proje i�erisinde bir `.env` dosyas� olu�turun ve i�erisine JWT i�in gerekli gizli anahtar gibi ortam de�i�kenlerini ekleyin. �rnek olarak `.env.example` dosyas�n� kullanabilirsiniz.
3.  **Ba��ml�l�klar**: Proje i�in gerekli NuGet paketlerinin y�kl� oldu�undan emin olun.
    ```bash
    Update-Package -reinstall
    ```
4.  **Ba�latma**: Projeyi Visual Studio �zerinden IIS Express ile ba�latabilirsiniz. Proje, `http://localhost:8000/` portunda �al��acak �ekilde ayarlanm��t�r. API dok�mantasyonuna ve test aray�z�ne `/swagger` adresi �zerinden eri�ebilirsiniz.