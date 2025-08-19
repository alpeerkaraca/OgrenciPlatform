# OgrenciPortali - MVC Projesi

Bu proje, Öðrenci Platformu'nun kullanýcý arayüzünü oluþturan ASP.NET MVC projesidir. Kullanýcýlarýn sistemle etkileþime girdiði tüm ekranlar burada yer alýr.

## Sorumluluklarý

-   Kullanýcý giriþi, kaydý ve yetkilendirme yönetimi arayüzleri.
-   Öðrenciler için ders seçme ve görüntüleme ekranlarý.
-   Danýþmanlar için öðrenci ve ders onayý iþlemleri.
-   Adminler için kullanýcý, bölüm, ders ve dönem yönetimi panelleri.
-   `OgrenciPortalApi` projesi ile HTTP istekleri üzerinden iletiþim kurarak verileri görüntülemek ve iþlemek.

## Teknolojiler

-   ASP.NET MVC 5
-   Razor View Engine
-   Bootstrap & CSS
-   jQuery & JavaScript
-   Entity Framework (Veritabaný iþlemleri için API'ye istek atar)

## Kurulum ve Çalýþtýrma

1.  **API Baðlantýsý**: Projenin `Web.config` veya `.env` dosyasýnda yer alan `ApiBaseAddress` ayarýnýn, çalýþan `OgrenciPortalApi` projesinin adresini doðru bir þekilde gösterdiðinden emin olun.
2.  **Baðýmlýlýklar**: Proje için gerekli NuGet paketlerinin yüklü olduðundan emin olun.
    ```bash
    Update-Package -reinstall
    ```
3.  **Baþlatma**: Projeyi Visual Studio üzerinden IIS Express ile baþlatabilirsiniz. Proje, `http://localhost:3000/` portunda çalýþacak þekilde ayarlanmýþtýr.