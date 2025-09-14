# Öğrenci Platformu Geliştirme Günlüğü

## Eylül 2025 Geliştirme Kayıtları

### 5 Eylül Cuma: 
Kullanıcı çıkış yaptıktan sonra her ne kadar işlem yapamıyor olsa bile ana sayfa gibi sayfaları tarayıcıdaki önbellekleme sebebiyle görebilir durumdaydı bu durumu ortadan kaldırmak için bir tane daha Attribute yazdım bu sayede tarayıcıya her sayfa için bu sayfayı önbelleğe alma dedim ve geçmişe alması durumuna karşı önbelleğin süresini bir yıl öncesi gibi gösterterek otomatik olarak temizlettirdim.

### 8 Eylül Pazartesi:

### 9 Eylül Salı:
CORS (Cross-Origin Resource Sharing) ayarlarını güncelleyerek yeni domain (https://ogrenciportal.alpeerkaraca.site) üzerinden API'ye erişim izni verdim. Bu sayede farklı subdomain'lerden gelen isteklerin güvenli bir şekilde işlenebilmesi sağlandı. Ayrıca güvenlik taraması için ggshield entegrasyonu yaparak hassas bilgilerin kod içerisinde yanlışlıkla paylaşılmasını önleyecek bir sistem kurdum.

### 10 Eylül Çarşamba:
E-posta bildirimi sistemini geliştirdim ve öğretim görevlilerinin ders durumu değişikliklerini öğrencilere otomatik olarak bildirebilmesi için AdvisorController'a yeni fonksiyonlar ekledim. MailService servisini genişleterek farklı e-posta şablonları (account-created-template.html, course-status-update-template.html, password-reset-template.html) kullanabilme özelliği getirdim. Bu şablonlar responsive tasarım ve modern CSS stilleri içeriyor. Aynı zamanda StudentController'da ders kayıt işlemleri sırasında daha detaylı loglama ve hata yönetimi implementasyonu gerçekleştirdim.

### 11 Eylül Perşembe:

### 12 Eylül Cuma:

### 13 Eylül Cumartesi:
Redis önbellekleme sistemini geliştirerek öğrenci numaralarının da e-posta adresleri gibi önbelleklenmesini sağladım. CheckUserData servisinde hem e-posta ("user:email:{email}") hem de öğrenci numarası ("user:studentno:{studentNo}") kontrollerinin Redis üzerinden hızlı bir şekilde yapılabilmesi için yeni fonksiyonlar ekledim. Kullanıcı arayüzünde tipografi iyileştirmeleri yaparak Nunito Sans font ailesini cust-fonts.css dosyası ile entegre ettim ve Bootstrap Icons'ın v1.13.1 sürümüne güncelleyerek daha zengin ikon kütüphanesi sağladım.