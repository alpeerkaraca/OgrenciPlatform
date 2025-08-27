# 🎓 Öğrenci Platformu Projesi / Student Platform Project

[🇹🇷 Türkçe](#türkçe) | [🇺🇸 English](#english)

---

## 🇹🇷 Türkçe

Bu proje, güvenlik odaklı bir öğrenci bilgi sistemini hayata geçirmek amacıyla geliştirilmiştir. Proje, modern web uygulama mimarilerine uygun olarak, birbirinden bağımsız çalışan iki ana katmandan oluşmaktadır:

### 🏗️ Proje Mimarisi

1. **🌐 OgrenciPortali (ASP.NET MVC)**: Kullanıcı arayüzü ve istemci tarafı etkileşimlerinden sorumlu olan ön yüz projesidir.
2. **⚡ OgrenciPortalApi (ASP.NET Web API)**: Tüm iş mantığı, veri erişimi ve servis hizmetlerini sunan arka yüz projesidir.
3. **📚 Shared**: Ortak veri modelleri ve DTO'ların bulunduğu kütüphane projesidir.

Bu mimari sayesinde, gelecekte farklı istemciler (mobil uygulama, masaüstü uygulaması vb.) geliştirilmesi durumunda aynı API altyapısı kolayca kullanılabilir.

### 🔒 Güvenlik Özellikleri

Bu platform, modern güvenlik standartlarına uygun olarak geliştirilmiştir:

#### 🛡️ Kimlik Doğrulama ve Yetkilendirme
- **JWT (JSON Web Token)** tabanlı güvenli kimlik doğrulama
- **Role-based access control** (Admin, Öğrenci, Danışman rolleri)
- Güvenli refresh token yönetimi
- **HttpOnly cookies** ile güvenli oturum yönetimi

#### 🔐 Parola Güvenliği
- **Güçlü parola politikası** (minimum 8 karakter, büyük/küçük harf, rakam, özel karakter)
- **BCrypt.Net** ile güvenli parola hash'leme
- Parola değiştirme zorunluluğu (ilk giriş)
- Real-time parola doğrulama

#### 🛡️ Güvenlik Korumaları
- **CSRF (Cross-Site Request Forgery)** koruması
- **Anti-forgery token** doğrulaması
- **Input validation** ve model doğrulama
- **SQL Injection** koruması (Entity Framework)
- Güvenli API endpoint'leri

#### 📊 Güvenlik İzleme
- **log4net** ile kapsamlı güvenlik logları
- Başarısız giriş denemeleri takibi
- Yetkisiz erişim girişimleri loglaması
- Hata yönetimi ve güvenli hata mesajları

### 📦 Projeler

- **🌐 [OgrenciPortali](OgrenciPortali/README.md)**: Kullanıcıların (öğrenci, danışman, admin) sisteme giriş yapabildiği, ders seçimi, not görüntüleme gibi işlemleri gerçekleştirebildiği güvenli web arayüzü.
- **⚡ [OgrenciPortalApi](OgrenciPortalApi/README.md)**: Sistemin beyni olarak çalışan, veritabanı işlemlerini yürüten ve MVC projesine güvenli veri sağlayan RESTful servis.

### 🚀 Hızlı Başlangıç

1. **Depoyu klonlayın:**
   ```bash
   git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
   cd OgrenciPlatform
   ```

2. **Ortam değişkenlerini ayarlayın:**
   ```bash
   # Her iki projede de .env dosyası oluşturun
   cp OgrenciPortalApi/.env.example OgrenciPortalApi/.env
   cp OgrenciPortali/.env.example OgrenciPortali/.env
   ```

3. **Güvenlik yapılandırması:**
   - `.env` dosyalarında güçlü JWT anahtarları ayarlayın
   - Veritabanı bağlantı dizelerini yapılandırın
   - HTTPS kullanımını aktif edin

4. **Projeleri başlatın:**
   - Önce API projesini (port: 8000)
   - Sonra MVC projesini (port: 3000)

### ⚙️ Sistem Gereksinimleri

- **.NET Framework 4.8** veya üzeri
- **SQL Server** 2016 veya üzeri
- **IIS Express** (geliştirme ortamı)
- **Visual Studio 2019** veya üzeri (önerilen)

### 📚 Teknolojiler

- **Backend**: ASP.NET Web API 2, Entity Framework 6
- **Frontend**: ASP.NET MVC 5, Razor, Bootstrap 5
- **Güvenlik**: JWT, BCrypt, CSRF Protection
- **Veritabanı**: SQL Server
- **API Dokümantasyonu**: Swagger

### 🤝 Katkıda Bulunma

1. Bu depoyu fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

### ⚠️ Güvenlik Bildirimi

Güvenlik zafiyeti keşfederseniz, lütfen public issue açmak yerine doğrudan proje sahipleriyle iletişime geçin.

---

## 🇺🇸 English

This project is a security-focused student information system developed using modern web application architectures. The project consists of two independent main layers:

### 🏗️ Project Architecture

1. **🌐 OgrenciPortali (ASP.NET MVC)**: Frontend project responsible for user interface and client-side interactions.
2. **⚡ OgrenciPortalApi (ASP.NET Web API)**: Backend project that provides all business logic, data access, and service functionalities.
3. **📚 Shared**: Library project containing common data models and DTOs.

This architecture allows easy reuse of the same API infrastructure when developing different clients (mobile app, desktop application, etc.) in the future.

### 🔒 Security Features

This platform is developed in accordance with modern security standards:

#### 🛡️ Authentication and Authorization
- **JWT (JSON Web Token)** based secure authentication
- **Role-based access control** (Admin, Student, Advisor roles)
- Secure refresh token management
- **HttpOnly cookies** for secure session management

#### 🔐 Password Security
- **Strong password policy** (minimum 8 characters, upper/lower case, numbers, special characters)
- **BCrypt.Net** secure password hashing
- Password change requirement (first login)
- Real-time password validation

#### 🛡️ Security Protections
- **CSRF (Cross-Site Request Forgery)** protection
- **Anti-forgery token** validation
- **Input validation** and model validation
- **SQL Injection** protection (Entity Framework)
- Secure API endpoints

#### 📊 Security Monitoring
- Comprehensive security logs with **log4net**
- Failed login attempt tracking
- Unauthorized access attempt logging
- Error handling and secure error messages

### 📦 Projects

- **🌐 [OgrenciPortali](OgrenciPortali/README.md)**: Secure web interface where users (students, advisors, admins) can log in and perform operations like course selection and grade viewing.
- **⚡ [OgrenciPortalApi](OgrenciPortalApi/README.md)**: RESTful service that acts as the system's brain, manages database operations, and provides secure data to the MVC project.

### 🚀 Quick Start

1. **Clone the repository:**
   ```bash
   git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
   cd OgrenciPlatform
   ```

2. **Set up environment variables:**
   ```bash
   # Create .env files in both projects
   cp OgrenciPortalApi/.env.example OgrenciPortalApi/.env
   cp OgrenciPortali/.env.example OgrenciPortali/.env
   ```

3. **Security configuration:**
   - Set strong JWT keys in .env files
   - Configure database connection strings
   - Enable HTTPS usage

4. **Start the projects:**
   - First the API project (port: 8000)
   - Then the MVC project (port: 3000)

### ⚙️ System Requirements

- **.NET Framework 4.8** or higher
- **SQL Server** 2016 or higher
- **IIS Express** (development environment)
- **Visual Studio 2019** or higher (recommended)

### 📚 Technologies

- **Backend**: ASP.NET Web API 2, Entity Framework 6
- **Frontend**: ASP.NET MVC 5, Razor, Bootstrap 5
- **Security**: JWT, BCrypt, CSRF Protection
- **Database**: SQL Server
- **API Documentation**: Swagger

### 🤝 Contributing

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Create a Pull Request

### ⚠️ Security Notice

If you discover a security vulnerability, please contact the project owners directly instead of opening a public issue.