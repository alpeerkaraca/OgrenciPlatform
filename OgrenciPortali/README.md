# 🌐 OgrenciPortali - Güvenli MVC Projesi / Secure MVC Project

[🇹🇷 Türkçe](#türkçe) | [🇺🇸 English](#english)

---

## 🇹🇷 Türkçe

Bu proje, Öğrenci Platformu'nun güvenli kullanıcı arayüzünü oluşturan ASP.NET MVC projesidir. Modern güvenlik standartları ile geliştirilmiş, kullanıcıların sistemle güvenli bir şekilde etkileşime girdiği tüm ekranlar burada yer alır.

### 🎯 Sorumlulukları

- **🔐 Güvenli Kimlik Doğrulama**: Kullanıcı girişi, kaydı ve JWT tabanlı yetkilendirme yönetimi arayüzleri
- **👨‍🎓 Öğrenci İşlemleri**: Güvenli ders seçme, not görüntüleme ve profil yönetimi ekranları
- **👨‍🏫 Danışman Paneli**: Öğrenci ve ders onayı işlemleri için güvenli yönetim arayüzü
- **👨‍💼 Admin Kontrol Paneli**: Kullanıcı, bölüm, ders ve dönem yönetimi için kapsamlı güvenlik kontrolleri
- **🔒 API İletişimi**: OgrenciPortalApi projesi ile güvenli HTTP istekleri üzerinden şifrelenmiş veri alışverişi

### 🛡️ Güvenlik Özellikleri

#### 🔒 Kimlik Doğrulama ve Yetkilendirme
- **JWT Token Tabanlı Kimlik Doğrulama**: Güvenli token yönetimi ve otomatik yenileme
- **Role-Based Access Control**: CustomAuthAttribute ile rol tabanlı erişim kontrolü
  - Admin: Tam sistem erişimi
  - Danışman: Kendi öğrencileri ve dersler
  - Öğrenci: Kendi verileri ve ders seçimi
- **Secure Session Management**: HttpOnly cookies ile güvenli oturum yönetimi
- **Auto Logout**: Güvenlik için otomatik oturum sonlandırma

#### 🔐 Parola Güvenliği
```csharp
// Güçlü parola politikası
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
```
- **Minimum 8 karakter** zorunluluğu
- **En az 1 büyük harf** (A-Z)
- **En az 1 küçük harf** (a-z)
- **En az 1 rakam** (0-9)
- **En az 1 özel karakter** (@$!%*?&)
- **Real-time parola doğrulama** JavaScript ile
- **İlk giriş parola değiştirme** zorunluluğu

#### 🛡️ CSRF Koruması
```csharp
[ValidateJsonAntiForgeryToken]
public class ExampleController : Controller
{
    // CSRF korumalı action'lar
}
```
- **Anti-forgery token** doğrulaması
- **Cross-Site Request Forgery** saldırılarına karşı koruma
- **RequestVerificationToken** header kontrolü

#### 📊 Input Validation
- **Model Validation**: DataAnnotations ile kapsamlı doğrulama
- **XSS Prevention**: Razor Engine otomatik encoding
- **SQL Injection Prevention**: Parameterized queries
- **Client-side Validation**: jQuery Validation ile real-time kontrol

#### 🔍 Güvenlik İzleme
- **log4net Integration**: Tüm güvenlik olaylarının loglanması
- **Failed Login Tracking**: Başarısız giriş denemeleri takibi
- **Error Handling**: Güvenli hata mesajları ve stack trace gizleme
- **Audit Logging**: Kritik işlemlerin audit trail'i

### 🔧 Teknolojiler

#### Backend
- **ASP.NET MVC 5**: Modern web framework
- **Razor View Engine**: Güvenli template rendering
- **AutoMapper**: Secure object mapping
- **log4net**: Kapsamlı logging

#### Frontend & Security
- **Bootstrap 5**: Modern responsive UI
- **jQuery 3.x**: DOM manipülasyonu ve AJAX
- **Client-side Validation**: Real-time form doğrulama
- **Content Security Policy**: XSS koruması

#### Authentication & Authorization
- **JWT (JSON Web Token)**: Stateless authentication
- **Custom Authorization Attributes**: Role-based access
- **Secure Cookies**: HttpOnly ve Secure flags

### 🚀 Kurulum ve Çalıştırma

#### 1. Güvenlik Yapılandırması

**.env dosyası oluşturun:**
```bash
cp .env.example .env
```

**.env içeriği:**
```ini
JWT_MASTER_KEY="your_super_secure_256_bit_key_here"
JWT_ISSUER="https://yourapp.com"
JWT_AUDIENCE="https://yourapp.com"
API_BASE_ADDRESS="https://localhost:8000"
```

⚠️ **GÜVENLİK UYARISI**: JWT_MASTER_KEY en az 256 bit güçlü bir anahtar olmalıdır!

#### 2. Web.config Güvenlik Ayarları

```xml
<configuration>
  <appSettings>
    <!-- HTTPS zorunlu hale getirin -->
    <add key="RequireSSL" value="true" />
    <!-- Güvenli cookie ayarları -->
    <add key="SecureCookies" value="true" />
  </appSettings>
  
  <system.web>
    <!-- Session timeout -->
    <sessionState timeout="30" />
    <!-- CSRF koruması -->
    <httpCookies requireSSL="true" httpOnlyCookies="true" />
  </system.web>
</configuration>
```

#### 3. API Bağlantısı

Projenin `Web.config` veya `.env` dosyasında yer alan `ApiBaseAddress` ayarının, çalışan `OgrenciPortalApi` projesinin HTTPS adresini doğru bir şekilde gösterdiğinden emin olun.

#### 4. Bağımlılıkları Yükleyin

```bash
# NuGet paketlerini yükle
Update-Package -reinstall

# Güvenlik paketlerini kontrol et
Install-Package Antlr
Install-Package Microsoft.AspNet.Mvc
Install-Package Newtonsoft.Json
```

#### 5. Başlatma

- Projeyi Visual Studio üzerinden **HTTPS** ile IIS Express kullanarak başlatın
- Proje `https://localhost:3000/` adresinde çalışacaktır
- **HTTP bağlantıları otomatik olarak HTTPS'e yönlendirilir**

### 🔐 Güvenlik En İyi Uygulamaları

#### Geliştirme Ortamı
- [ ] HTTPS her zaman aktif
- [ ] Debug mode production'da kapalı
- [ ] Güvenli bağlantı dizesi kullanımı
- [ ] Log seviyesi production'da ayarlanmalı

#### Production Deployment
- [ ] SSL sertifikası yüklü ve geçerli
- [ ] Security headers yapılandırılmalı
- [ ] Error pages özelleştirilmeli
- [ ] File upload restrictions aktif
- [ ] Rate limiting uygulanmalı

#### Kullanıcı Yönetimi
- [ ] Güçlü parola politikası zorunlu
- [ ] Account lockout politikası aktif
- [ ] Password reset güvenli kanal ile
- [ ] Two-factor authentication (gelecek güncelleme)

### 🐛 Güvenlik Test

#### Manual Testing
```bash
# HTTPS yönlendirme testi
curl -I http://localhost:3000/

# CSRF token kontrolü
curl -X POST https://localhost:3000/User/Login \
  -H "Content-Type: application/json" \
  -d '{"Email":"test@test.com","Password":"test"}'
```

#### Automated Security Scan
- OWASP ZAP entegrasyonu önerilir
- Security headers analizi
- SQL injection test
- XSS vulnerability scan

### 📊 Monitoring ve Logging

#### Log Levels
- **ERROR**: Güvenlik ihlalleri, sistem hataları
- **WARN**: Başarısız giriş denemeleri, şüpheli aktiviteler  
- **INFO**: Başarılı işlemler, sistem bilgileri
- **DEBUG**: Geliştirme amaçlı detaylar (production'da kapalı)

#### Key Metrics
- Failed login attempts per IP
- Session duration and anomalies
- API response times
- Error rates by endpoint

---

## 🇺🇸 English

This project is the secure user interface of the Student Platform, built with ASP.NET MVC. Developed with modern security standards, it contains all screens where users interact securely with the system.

### 🎯 Responsibilities

- **🔐 Secure Authentication**: User login, registration, and JWT-based authorization management interfaces
- **👨‍🎓 Student Operations**: Secure course selection, grade viewing, and profile management screens
- **👨‍🏫 Advisor Panel**: Secure management interface for student and course approval operations
- **👨‍💼 Admin Control Panel**: Comprehensive security controls for user, department, course, and term management
- **🔒 API Communication**: Encrypted data exchange via secure HTTP requests with OgrenciPortalApi project

### 🛡️ Security Features

#### 🔒 Authentication and Authorization
- **JWT Token-Based Authentication**: Secure token management and automatic refresh
- **Role-Based Access Control**: Role-based access control with CustomAuthAttribute
  - Admin: Full system access
  - Advisor: Own students and courses
  - Student: Own data and course selection
- **Secure Session Management**: Secure session management with HttpOnly cookies
- **Auto Logout**: Automatic session termination for security

#### 🔐 Password Security
```csharp
// Strong password policy
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
```
- **Minimum 8 characters** requirement
- **At least 1 uppercase letter** (A-Z)
- **At least 1 lowercase letter** (a-z)  
- **At least 1 number** (0-9)
- **At least 1 special character** (@$!%*?&)
- **Real-time password validation** with JavaScript
- **First login password change** requirement

#### 🛡️ CSRF Protection
```csharp
[ValidateJsonAntiForgeryToken]
public class ExampleController : Controller
{
    // CSRF protected actions
}
```
- **Anti-forgery token** validation
- **Cross-Site Request Forgery** attack protection
- **RequestVerificationToken** header control

#### 📊 Input Validation
- **Model Validation**: Comprehensive validation with DataAnnotations
- **XSS Prevention**: Automatic encoding with Razor Engine
- **SQL Injection Prevention**: Parameterized queries
- **Client-side Validation**: Real-time control with jQuery Validation

#### 🔍 Security Monitoring
- **log4net Integration**: Logging of all security events
- **Failed Login Tracking**: Failed login attempt tracking
- **Error Handling**: Secure error messages and stack trace hiding
- **Audit Logging**: Audit trail for critical operations

### 🔧 Technologies

#### Backend
- **ASP.NET MVC 5**: Modern web framework
- **Razor View Engine**: Secure template rendering
- **AutoMapper**: Secure object mapping
- **log4net**: Comprehensive logging

#### Frontend & Security
- **Bootstrap 5**: Modern responsive UI
- **jQuery 3.x**: DOM manipulation and AJAX
- **Client-side Validation**: Real-time form validation
- **Content Security Policy**: XSS protection

#### Authentication & Authorization
- **JWT (JSON Web Token)**: Stateless authentication
- **Custom Authorization Attributes**: Role-based access
- **Secure Cookies**: HttpOnly and Secure flags

### 🚀 Installation and Setup

#### 1. Security Configuration

**Create .env file:**
```bash
cp .env.example .env
```

**.env content:**
```ini
JWT_MASTER_KEY="your_super_secure_256_bit_key_here"
JWT_ISSUER="https://yourapp.com"
JWT_AUDIENCE="https://yourapp.com"
API_BASE_ADDRESS="https://localhost:8000"
```

⚠️ **SECURITY WARNING**: JWT_MASTER_KEY must be at least 256-bit strong key!

#### 2. Web.config Security Settings

```xml
<configuration>
  <appSettings>
    <!-- Force HTTPS -->
    <add key="RequireSSL" value="true" />
    <!-- Secure cookie settings -->
    <add key="SecureCookies" value="true" />
  </appSettings>
  
  <system.web>
    <!-- Session timeout -->
    <sessionState timeout="30" />
    <!-- CSRF protection -->
    <httpCookies requireSSL="true" httpOnlyCookies="true" />
  </system.web>
</configuration>
```

#### 3. API Connection

Ensure that the `ApiBaseAddress` setting in the project's `Web.config` or `.env` file correctly points to the HTTPS address of the running `OgrenciPortalApi` project.

#### 4. Install Dependencies

```bash
# Install NuGet packages
Update-Package -reinstall

# Check security packages
Install-Package Antlr
Install-Package Microsoft.AspNet.Mvc
Install-Package Newtonsoft.Json
```

#### 5. Launch

- Start the project from Visual Studio using IIS Express with **HTTPS**
- Project will run at `https://localhost:3000/`
- **HTTP connections are automatically redirected to HTTPS**

### 🔐 Security Best Practices

#### Development Environment
- [ ] HTTPS always active
- [ ] Debug mode off in production
- [ ] Secure connection string usage
- [ ] Log level set for production

#### Production Deployment
- [ ] SSL certificate installed and valid
- [ ] Security headers configured
- [ ] Error pages customized
- [ ] File upload restrictions active
- [ ] Rate limiting implemented

#### User Management
- [ ] Strong password policy mandatory
- [ ] Account lockout policy active
- [ ] Password reset via secure channel
- [ ] Two-factor authentication (future update)

### 🐛 Security Testing

#### Manual Testing
```bash
# HTTPS redirect test
curl -I http://localhost:3000/

# CSRF token check
curl -X POST https://localhost:3000/User/Login \
  -H "Content-Type: application/json" \
  -d '{"Email":"test@test.com","Password":"test"}'
```

#### Automated Security Scan
- OWASP ZAP integration recommended
- Security headers analysis
- SQL injection test
- XSS vulnerability scan

### 📊 Monitoring and Logging

#### Log Levels
- **ERROR**: Security violations, system errors
- **WARN**: Failed login attempts, suspicious activities
- **INFO**: Successful operations, system information
- **DEBUG**: Development details (off in production)

#### Key Metrics
- Failed login attempts per IP
- Session duration and anomalies
- API response times
- Error rates by endpoint