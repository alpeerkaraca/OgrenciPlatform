# OgrenciPortalApi - Web API Projesi

Bu proje, Öðrenci Platformu'nun tüm iþ mantýðýný ve veri eriþim katmanýný içeren ASP.NET Web API projesidir. RESTful prensiplerine uygun olarak servis hizmeti sunar.

## Sorumluluklarý

-   Kullanýcý doðrulama ve JWT (JSON Web Token) tabanlý yetkilendirme.
-   Öðrenci, danýþman, ders, bölüm ve dönemlerle ilgili tüm CRUD (Create, Read, Update, Delete) iþlemleri.
-   Veritabaný ile doðrudan iletiþim kurarak veri tutarlýlýðýný saðlamak.
-   Ýþ kurallarýný (ders kontenjaný, çakýþma kontrolü vb.) uygulamak.
-   Ýstemci uygulamalarýna (örneðin `OgrenciPortali`) JSON formatýnda veri sunmak.

## Teknolojiler

-   ASP.NET Web API 2
-   Entity Framework (Database First yaklaþýmý kullanýlmýþ)
-   JWT (JSON Web Token) ile token tabanlý kimlik doðrulama.
-   BCrypt.Net-Next ile parola hash'leme.
-   Swagger (API dokümantasyonu için).

## Kurulum ve Çalýþtýrma

1.  **Veritabaný**: Proje, Entity Framework Database First yaklaþýmýný kullanmaktadýr. `Web.config` dosyasýndaki `connectionStrings` bölümünü kendi veritabaný sunucunuza göre düzenleyin.
2.  **Environment Dosyasý**: Proje içerisinde bir `.env` dosyasý oluþturun ve içerisine JWT için gerekli gizli anahtar gibi ortam deðiþkenlerini ekleyin. Örnek olarak `.env.example` dosyasýný kullanabilirsiniz.
3.  **Baðýmlýlýklar**: Proje için gerekli NuGet paketlerinin yüklü olduðundan emin olun.
    ```bash
    Update-Package -reinstall
    ```
4.  **Baþlatma**: Projeyi Visual Studio üzerinden IIS Express ile baþlatabilirsiniz. Proje, `http://localhost:8000/` portunda çalýþacak þekilde ayarlanmýþtýr. API dokümantasyonuna ve test arayüzüne `/swagger` adresi üzerinden eriþebilirsiniz.