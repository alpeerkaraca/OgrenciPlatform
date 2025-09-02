# 🎓 Öğrenci Platformu (Student Platform)

**Türkçe** | [English](#english-version)

Bu proje, güvenlik odaklı bir öğrenci bilgi sistemi olarak geliştirilmiştir. Modern web uygulama mimarilerine uygun olarak tasarlanan sistem, JWT tabanlı kimlik doğrulama, rol tabanlı yetkilendirme ve kapsamlı güvenlik önlemleriyle kurumsal kullanıma hazır bir platform sunmaktadır.

## 🔒 Güvenlik Özellikleri

### Kimlik Doğrulama ve Yetkilendirme
- **JWT Token Tabanlı Kimlik Doğrulama**: Stateless authentication ile ölçeklenebilir güvenlik
- **Refresh Token Mekanizması**: Güvenli oturum yenileme ve uzun süreli erişim yönetimi
- **Rol Tabanlı Erişim Kontrolü**: Admin, Danışman ve Öğrenci rolleri ile detaylı yetki yönetimi
- **Özel Yetkilendirme Filtreleri**: CustomAuth attribute ile granüler erişim kontrolü

### Parola Güvenliği
- **BCrypt Hash Algoritması**: Endüstri standardı parola şifreleme
- **Güçlü Parola Politikaları**: Minimum karmaşıklık gereksinimleri
- **Güvenli Parola Sıfırlama**: Token tabanlı sıfırlama mekanizması (15 dakika geçerlilik)

### Oturum Güvenliği
- **HttpOnly ve Secure Çerezler**: XSS ve MITM saldırılarına karşı koruma
- **CSRF Koruması**: Anti-forgery token implementasyonu
- **Oturum Zaman Aşımı**: Yapılandırılabilir token yaşam süresi
- **Güvenli Çıkış**: Refresh token'ların sunucu tarafında iptal edilmesi

### Veri Güvenliği
- **Input Validasyonu**: Model state validation ve form doğrulama
- **SQL Injection Koruması**: Entity Framework ORM kullanımı
- **Güvenli API Endpoint'leri**: Yetkilendirme kontrollü REST servisleri

## 🏗️ Proje Mimarisi

### Katmanlı Mimari
```
├── OgrenciPortali (MVC Frontend)     # Kullanıcı Arayüzü Katmanı
│   ├── Controllers/                  # MVC Controllers
│   ├── Views/                        # Razor Views
│   ├── Attributes/                   # Custom Authorization
│   └── Utils/                        # Helper Classes
│
├── OgrenciPortalApi (Web API)        # İş Mantığı ve Veri Katmanı
│   ├── Controllers/                  # API Controllers
│   ├── Models/                       # Entity Framework Models
│   ├── Utils/                        # JWT, Security Utilities
│   └── Areas/SwaggerUI/              # API Documentation
│
├── Shared                            # Ortak Bileşenler Kütüphanesi
│   ├── DTO/                          # Data Transfer Objects
│   ├── Enums/                        # Sistem Enums
│   └── Constants/                    # Sabit Değerler
│
└── OgrenciPlatform.Shared            # Veri Transfer Katmanı
    └── DTO/                          # Web-Optimized DTOs
```

### Kullanıcı Rolleri ve Yetkileri
- **👤 Admin**: Sistem yöneticisi - Tüm modüllere erişim
- **👨‍🏫 Danışman**: Akademik danışman - Ders ve öğrenci yönetimi
- **👨‍🎓 Öğrenci**: Öğrenci kullanıcı - Ders kayıt ve takip işlemleri

## 🚀 Teknoloji Yığını

### Backend Teknolojileri
- **ASP.NET Web API 2** (.NET Framework 4.7.2) - RESTful API servisleri
- **ASP.NET MVC 5** (.NET Framework 4.7.2) - Web uygulama framework'ü
- **Entity Framework 6.5.1** - ORM ve veritabanı erişimi (Database-First)
- **Microsoft SQL Server** - Veritabanı yönetim sistemi
- **Redis Stack 2.8.58** - In-memory caching ve real-time data validation
- **Hangfire 1.8.21** - Background job processing ve scheduled tasks

### Güvenlik ve Kimlik Doğrulama
- **Microsoft.Owin 4.2.3** - OWIN middleware
- **Microsoft.Owin.Security.Jwt 4.2.3** - JWT token middleware
- **System.IdentityModel.Tokens.Jwt 8.13.0** - JWT token işlemleri
- **BCrypt.Net-Next 4.0.3** - Parola hashleme
- **Claims-based Authentication** - Kullanıcı bilgileri ve roller

### AI ve Automation
- **Deepseek API Integration** - AI-powered course description generation
- **Hangfire Background Processing** - Automated cache updates ve scheduled tasks
- **Real-time Validation** - Redis-based instant form validation

### İstemci Tarafı
- **Bootstrap 5.3.7** - Responsive UI framework
- **jQuery 3.7.1** - JavaScript kütüphanesi ve validation
- **Fetch API** - Modern asenkron HTTP istekleri
- **Real-time AJAX** - Instant user feedback ve validation

### Communication & Email
- **MailKit 4.13.0** - Modern email handling
- **MimeKit 4.13.0** - Email formatting and parsing

### Geliştirici Araçları
- **AutoMapper 15.0.1** - Object mapping
- **Autofac 6.4.0** / **Unity 5.11.8** - Dependency injection containers
- **log4net 3.1.0** - Comprehensive logging framework
- **Newtonsoft.Json 13.0.3** - JSON serialize/deserialize
- **DotNetEnv 3.1.1** - Environment variables yönetimi
- **Swashbuckle 5.6.0** - API documentation ve testing

## ⚙️ Kurulum ve Yapılandırma

### Ön Gereksinimler
- Visual Studio 2019 veya üzeri
- .NET Framework 4.7.2
- Microsoft SQL Server 2016 veya üzeri
- IIS Express veya IIS

### 1. Projeyi Klonlayın
```bash
git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
cd OgrenciPlatform
```

### 2. Veritabanı Kurulumu
1. SQL Server'da yeni bir veritabanı oluşturun
2. `OgrenciPortalApi/Web.config` dosyasında connection string'i güncelleyin:
```xml
<connectionStrings>
  <add name="OgrenciPortalContext" 
       connectionString="Data Source=SERVER;Initial Catalog=DBNAME;User ID=USER;Password=PASS" />
</connectionStrings>
```

### 3. Çevresel Değişkenler
API projesinde `.env` dosyası oluşturun (`.env.example`'dan kopyalayın):
```bash
# JWT Configuration
JWT_MASTER_KEY="your_super_secret_jwt_key_minimum_256_bits"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"
ACCESS_TOKEN_EXPIRATION_MINUTES=15
REFRESH_TOKEN_EXPIRATION_DAYS=7

# API Configuration
API_BASE_ADDRESS="https://localhost:44301/"

# Email Configuration (Password Reset)
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="your-email@gmail.com"
SMTP_PASS="your-app-password"

# Redis Configuration
REDIS_CONNECTION_STRING="localhost:6379"

# AI Configuration (Deepseek API)
DEEPSEEK_API_KEY="your_deepseek_api_key"
DEEPSEEK_API_URL="https://api.deepseek.com"

# Background Jobs Configuration
HANGFIRE_DASHBOARD_USERNAME="admin"
HANGFIRE_DASHBOARD_PASSWORD="your_secure_password"
```

### 4. Güvenlik Yapılandırması
⚠️ **Önemli Güvenlik Notları:**
- JWT_MASTER_KEY en az 256 bit (32 karakter) olmalıdır
- Üretim ortamında güçlü, rastgele bir anahtar kullanın
- SMTP bilgilerini güvenli şekilde saklayın
- SQL Server bağlantı bilgilerini şifreleyin

### 5. Projeyi Çalıştırma
1. **İlk olarak API'yi başlatın**:
   - Visual Studio'da OgrenciPortalApi projesini başlatın
   - Swagger UI: `https://localhost:44301/swagger`

2. **Ardından MVC uygulamasını başlatın**:
   - OgrenciPortali projesini başlatın
   - Web arayüzü: `https://localhost:44302`

## 🔐 Güvenlik Best Practices

### Geliştirme Ortamında
- [ ] `.env` dosyalarını `.gitignore`'a ekleyin
- [ ] Varsayılan admin hesabı için güçlü parola kullanın
- [ ] Development sertifikalarını kullanın (HTTPS)
- [ ] SQL Server authentication yerine Windows Authentication tercih edin

### Üretim Ortamında
- [ ] JWT anahtarlarını Azure Key Vault veya benzeri servislerde saklayın
- [ ] SSL/TLS sertifikası yapılandırın (Let's Encrypt)
- [ ] Database connection string'lerini şifreleyin
- [ ] Application Insights veya benzeri monitoring ekleyin
- [ ] Rate limiting implementasyonu
- [ ] IP whitelist/blacklist yapılandırması
- [ ] Güvenlik başlıkları (HSTS, CSP, X-Frame-Options)

### Monitoring ve Logging
- [ ] log4net ile güvenlik olaylarını loglayin
- [ ] Başarısız giriş denemelerini takip edin
- [ ] API rate limiting logları
- [ ] Kritik işlemler için audit trail

## 📚 API Dokümantasyonu

API endpoint'leri ve güvenlik modeli için Swagger UI kullanın:
- **Development**: `https://localhost:44301/swagger`
- **API Base URL**: `https://localhost:44301/api/`

### Temel API Endpoint'leri
```
# Authentication & Authorization
POST /api/user/login          # Kullanıcı girişi
POST /api/auth/refresh-token  # Token yenileme
POST /api/auth/logout         # Güvenli çıkış

# User Management
GET  /api/user/profile        # Kullanıcı profili
POST /api/user/change-password # Parola değişikliği
POST /api/user/test-email     # Real-time email validation (Redis)

# Course Management
GET  /api/courses/list        # Ders listesi
POST /api/courses/generate-description # AI-powered course description

# Student Operations
GET  /api/student/my-courses  # Öğrencinin dersleri
POST /api/student/enroll      # Ders kaydı

# Background Jobs
GET  /hangfire                # Background jobs dashboard (Admin only)
```

## 📋 Son Güncellemeler

### 2025-09-02 - Redis Entegrasyonu ve Real-time Validation
- **⚡ Redis Cache System**: Kullanıcı e-posta adresleri Redis'te cache'lenerek lightning-fast validation sağlandı
- **🔄 Otomatik Cache Güncelleme**: Hangfire background job'ları ile cache her 15 dakikada bir yenilenir
- **⏱️ Real-time Form Validation**: Kullanıcılar form girişlerinde anında email existence feedback alır
- **🚀 Performance Enhancement**: Email validation için veritabanı sorgu yükü önemli ölçüde azaltıldı
- **📡 Yeni API Endpoint**: `POST /api/user/test-email` endpoint'i eklendi

### 2025-09-01 - AI Entegrasyonu ve Hangfire Kurulumu  
- **🤖 AI Course Description Generation**: Deepseek API entegrasyonu ile otomatik ders açıklaması üretimi
- **⚙️ Hangfire Background Processing**: Scheduled task'lar ve background job management
- **🎯 Automated Content Generation**: AI destekli içerik üretim sistemi
- **📊 Background Jobs Dashboard**: Admin kullanıcılar için job monitoring ve management
- **🔄 Scheduled Cache Updates**: Otomatik cache yenileme job'ları

### 2025-09-01 - Dokümantasyon Geliştirmeleri
- **✅ Kapsamlı README dosyaları**: Tüm projeler için detaylı dokümantasyon eklendi
- **📚 Shared Kütüphane Dokümantasyonu**: `Shared` ve `OgrenciPlatform.Shared` projelerine README dosyaları eklendi
- **🏗️ Mimari Güncellemeleri**: Proje mimarisi diyagramı iki ayrı Shared kütüphanesini yansıtacak şekilde güncellendi
- **🔄 Tutarlılık İyileştirmeleri**: Çok dilli dokümantasyon tutarlılığı sağlandı

### 2025-09-01 - UI/UX Geliştirmeleri
- **🎨 Modern Theme Implementation**: Yeni renk paleti ve geliştirilmiş layout stilleri
- **📱 Enhanced Modal System**: Improved modal behavior ve button state handling
- **⚡ AJAX Functionality**: Fetch API ile modern asenkron HTTP istekleri
- **✨ Real-time Validation**: Instant form validation ve user feedback

## 🤝 Katkı Sağlama

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

### Güvenlik Raporu
Güvenlik açığı tespit ederseniz, lütfen public issue açmak yerine doğrudan [maintainer]'a ulaşın.

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakın.

---

## English Version

# 🎓 Student Platform (Öğrenci Platformu)

This project is a security-focused student information system developed with modern web application architecture. The system offers an enterprise-ready platform with JWT-based authentication, role-based authorization, and comprehensive security measures.

## 🔒 Security Features

### Authentication and Authorization
- **JWT Token-Based Authentication**: Scalable security with stateless authentication
- **Refresh Token Mechanism**: Secure session renewal and long-term access management
- **Role-Based Access Control**: Detailed permission management with Admin, Advisor, and Student roles
- **Custom Authorization Filters**: Granular access control with CustomAuth attributes

### Password Security
- **BCrypt Hash Algorithm**: Industry-standard password encryption
- **Strong Password Policies**: Minimum complexity requirements
- **Secure Password Reset**: Token-based reset mechanism (15-minute validity)

### Session Security
- **HttpOnly and Secure Cookies**: Protection against XSS and MITM attacks
- **CSRF Protection**: Anti-forgery token implementation
- **Session Timeout**: Configurable token lifespan
- **Secure Logout**: Server-side refresh token revocation

### Data Security
- **Input Validation**: Model state validation and form verification
- **SQL Injection Protection**: Entity Framework ORM usage
- **Secure API Endpoints**: Authorization-controlled REST services

## 🏗️ Project Architecture

### Layered Architecture
```
├── OgrenciPortali (MVC Frontend)     # User Interface Layer
│   ├── Controllers/                  # MVC Controllers
│   ├── Views/                        # Razor Views
│   ├── Attributes/                   # Custom Authorization
│   └── Utils/                        # Helper Classes
│
├── OgrenciPortalApi (Web API)        # Business Logic and Data Layer
│   ├── Controllers/                  # API Controllers
│   ├── Models/                       # Entity Framework Models
│   ├── Utils/                        # JWT, Security Utilities
│   └── Areas/SwaggerUI/              # API Documentation
│
├── Shared                            # Common Components Library
│   ├── DTO/                          # Data Transfer Objects
│   ├── Enums/                        # System Enums
│   └── Constants/                    # Constants
│
└── OgrenciPlatform.Shared            # Data Transfer Layer
    └── DTO/                          # Web-Optimized DTOs
```

### User Roles and Permissions
- **👤 Admin**: System administrator - Full system access
- **👨‍🏫 Advisor (Danışman)**: Academic advisor - Course and student management
- **👨‍🎓 Student (Öğrenci)**: Student user - Course registration and tracking

## 🚀 Technology Stack

### Backend Technologies
- **ASP.NET Web API 2** (.NET Framework 4.7.2) - RESTful API services
- **ASP.NET MVC 5** (.NET Framework 4.7.2) - Web application framework
- **Entity Framework 6.5.1** - ORM and database access (Database-First)
- **Microsoft SQL Server** - Database management system
- **Redis Stack 2.8.58** - In-memory caching and real-time data validation
- **Hangfire 1.8.21** - Background job processing and scheduled tasks

### Security and Authentication
- **Microsoft.Owin 4.2.3** - OWIN middleware
- **Microsoft.Owin.Security.Jwt 4.2.3** - JWT token middleware
- **System.IdentityModel.Tokens.Jwt 8.13.0** - JWT token operations
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Claims-based Authentication** - User information and roles

### AI and Automation
- **Deepseek API Integration** - AI-powered course description generation
- **Hangfire Background Processing** - Automated cache updates and scheduled tasks
- **Real-time Validation** - Redis-based instant form validation

### Client-Side
- **Bootstrap 5.3.7** - Responsive UI framework
- **jQuery 3.7.1** - JavaScript library and validation
- **Fetch API** - Modern asynchronous HTTP requests
- **Real-time AJAX** - Instant user feedback and validation

### Communication & Email
- **MailKit 4.13.0** - Modern email handling
- **MimeKit 4.13.0** - Email formatting and parsing

### Developer Tools
- **AutoMapper 15.0.1** - Object mapping
- **Autofac 6.4.0** / **Unity 5.11.8** - Dependency injection containers
- **log4net 3.1.0** - Comprehensive logging framework
- **Newtonsoft.Json 13.0.3** - JSON serialize/deserialize
- **DotNetEnv 3.1.1** - Environment variables management
- **Swashbuckle 5.6.0** - API documentation and testing

## ⚙️ Installation and Setup

### Prerequisites
- Visual Studio 2019 or higher
- .NET Framework 4.7.2
- Microsoft SQL Server 2016 or higher
- IIS Express or IIS

### 1. Clone the Repository
```bash
git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
cd OgrenciPlatform
```

### 2. Database Setup
1. Create a new database in SQL Server
2. Update the connection string in `OgrenciPortalApi/Web.config`:
```xml
<connectionStrings>
  <add name="OgrenciPortalContext" 
       connectionString="Data Source=SERVER;Initial Catalog=DBNAME;User ID=USER;Password=PASS" />
</connectionStrings>
```

### 3. Environment Variables
Create a `.env` file in the API project (copy from `.env.example`):
```bash
# JWT Configuration
JWT_MASTER_KEY="your_super_secret_jwt_key_minimum_256_bits"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"
ACCESS_TOKEN_EXPIRATION_MINUTES=15
REFRESH_TOKEN_EXPIRATION_DAYS=7

# API Configuration
API_BASE_ADDRESS="https://localhost:44301/"

# Email Configuration (Password Reset)
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="your-email@gmail.com"
SMTP_PASS="your-app-password"

# Redis Configuration
REDIS_CONNECTION_STRING="localhost:6379"

# AI Configuration (Deepseek API)
DEEPSEEK_API_KEY="your_deepseek_api_key"
DEEPSEEK_API_URL="https://api.deepseek.com"

# Background Jobs Configuration
HANGFIRE_DASHBOARD_USERNAME="admin"
HANGFIRE_DASHBOARD_PASSWORD="your_secure_password"
```

### 4. Security Configuration
⚠️ **Important Security Notes:**
- JWT_MASTER_KEY must be at least 256 bits (32 characters)
- Use a strong, random key in production
- Secure SMTP credentials
- Encrypt SQL Server connection information

### 5. Running the Project
1. **Start the API first**:
   - Launch OgrenciPortalApi project in Visual Studio
   - Swagger UI: `https://localhost:44301/swagger`

2. **Then start the MVC application**:
   - Launch OgrenciPortali project
   - Web interface: `https://localhost:44302`

## 🔐 Security Best Practices

### Development Environment
- [ ] Add `.env` files to `.gitignore`
- [ ] Use strong passwords for default admin accounts
- [ ] Use development certificates (HTTPS)
- [ ] Prefer Windows Authentication over SQL Server authentication

### Production Environment
- [ ] Store JWT keys in Azure Key Vault or similar services
- [ ] Configure SSL/TLS certificates (Let's Encrypt)
- [ ] Encrypt database connection strings
- [ ] Add Application Insights or similar monitoring
- [ ] Implement rate limiting
- [ ] Configure IP whitelist/blacklist
- [ ] Set security headers (HSTS, CSP, X-Frame-Options)

### Monitoring and Logging
- [ ] Log security events with log4net
- [ ] Track failed login attempts
- [ ] Monitor API rate limiting
- [ ] Implement audit trail for critical operations

## 📚 API Documentation

Use Swagger UI for API endpoints and security model:
- **Development**: `https://localhost:44301/swagger`
- **API Base URL**: `https://localhost:44301/api/`

### Core API Endpoints
```
# Authentication & Authorization
POST /api/user/login          # User login
POST /api/auth/refresh-token  # Token refresh
POST /api/auth/logout         # Secure logout

# User Management
GET  /api/user/profile        # User profile
POST /api/user/change-password # Password change
POST /api/user/test-email     # Real-time email validation (Redis)

# Course Management
GET  /api/courses/list        # Course listing
POST /api/courses/generate-description # AI-powered course description

# Student Operations
GET  /api/student/my-courses  # Student's courses
POST /api/student/enroll      # Course enrollment

# Background Jobs
GET  /hangfire                # Background jobs dashboard (Admin only)
```

## 📋 Recent Updates

### 2025-09-02 - Redis Integration and Real-time Validation
- **⚡ Redis Cache System**: User email addresses cached in Redis for lightning-fast validation
- **🔄 Automated Cache Updates**: Hangfire background jobs refresh cache every 15 minutes
- **⏱️ Real-time Form Validation**: Users get instant email existence feedback during form input
- **🚀 Performance Enhancement**: Significantly reduced database query load for email validation
- **📡 New API Endpoint**: Added `POST /api/user/test-email` endpoint

### 2025-09-01 - AI Integration and Hangfire Setup
- **🤖 AI Course Description Generation**: Deepseek API integration for automated course description generation
- **⚙️ Hangfire Background Processing**: Scheduled tasks and background job management
- **🎯 Automated Content Generation**: AI-powered content generation system
- **📊 Background Jobs Dashboard**: Job monitoring and management for admin users
- **🔄 Scheduled Cache Updates**: Automated cache refresh background jobs

### 2025-09-01 - Documentation Improvements
- **✅ Comprehensive README Files**: Detailed documentation added for all projects
- **📚 Shared Library Documentation**: README files added to `Shared` and `OgrenciPlatform.Shared` projects
- **🏗️ Architecture Updates**: Project architecture diagram updated to reflect both Shared libraries
- **🔄 Consistency Improvements**: Multilingual documentation consistency ensured

### 2025-09-01 - UI/UX Enhancements
- **🎨 Modern Theme Implementation**: New color palette and improved layout styles
- **📱 Enhanced Modal System**: Improved modal behavior and button state handling
- **⚡ AJAX Functionality**: Fetch API integration for modern asynchronous HTTP requests
- **✨ Real-time Validation**: Instant form validation and user feedback

## 🤝 Contributing

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Create a Pull Request

### Security Reporting
If you discover a security vulnerability, please contact the maintainer directly instead of opening a public issue.

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.