# OgrenciPortalApi - Web API Backend Project

**Türkçe** | [English](#english-version)

Bu proje, Öğrenci Platformu'nun tüm iş mantığını, veri erişim katmanını ve backend servislerini içeren ASP.NET Web API projesidir. Modern RESTful prensiplerine uygun olarak tasarlanmış, güvenli ve ölçeklenebilir API servisleri sunar.

## 🎯 Temel Sorumluluklar

### Kimlik Doğrulama ve Yetkilendirme
- JWT (JSON Web Token) tabanlı stateless authentication
- Refresh token mekanizması ile güvenli oturum yenileme
- Rol tabanlı erişim kontrolü (Admin, Danışman, Öğrenci)
- Claims-based authorization ile granüler yetki yönetimi

### Veri Yönetimi ve İş Mantığı
- Kullanıcı, öğrenci, danışman yönetimi ve CRUD işlemleri
- Ders, bölüm, dönem yönetimi ve akademik işlemler
- Ders kayıt işlemleri ve çakışma kontrolü
- Veritabanı ile güvenli veri tutarlılığı

### Modern Backend Özellikler
- **Redis Cache Integration**: Real-time email validation ve performance optimization
- **AI-Powered Content Generation**: Deepseek API ile otomatik ders açıklaması üretimi
- **Background Job Processing**: Hangfire ile scheduled tasks ve automated operations
- **Real-time Data Validation**: İstant form validation ve user feedback

### API Servisleri
- JSON formatında structured data response
- Comprehensive error handling ve logging
- Swagger documentation ve API testing interface
- CORS support for cross-origin requests

## 🚀 Teknoloji Stack'i

### Core Technologies
- **ASP.NET Web API 2** (.NET Framework 4.7.2) - RESTful API framework
- **Entity Framework 6.5.1** - ORM (Database-First approach)
- **Microsoft SQL Server** - Primary database
- **OWIN 4.2.3** - Middleware pipeline

### Cache & Performance
- **Redis Stack 2.8.58** - In-memory caching ve data store
- **StackExchange.Redis 2.8.58** - Redis client library
- **Hangfire 1.8.21** - Background job processing
- **Hangfire.SqlServer 1.8.21** - SQL Server storage for jobs

### Security & Authentication
- **Microsoft.Owin.Security.Jwt 4.2.3** - JWT middleware
- **System.IdentityModel.Tokens.Jwt 8.13.0** - JWT token handling
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Microsoft.AspNet.Cors 5.3.0** - Cross-origin request support

### AI & External Integrations
- **Deepseek API** - AI content generation
- **MailKit 4.13.0** - Modern email handling
- **MimeKit 4.13.0** - Email message formatting

### Development & Utility
- **Swashbuckle 5.6.0** - API documentation (Swagger UI)
- **log4net 3.1.0** - Comprehensive logging
- **Newtonsoft.Json 13.0.3** - JSON serialization
- **DotNetEnv 3.1.1** - Environment configuration
- **Unity.Container 5.11.8** - Dependency injection

## 📡 API Endpoints

### Authentication & Authorization
```http
POST /api/user/login                    # User login with credentials
POST /api/auth/refresh-token           # Refresh access token
POST /api/auth/logout                  # Secure user logout
```

### User Management
```http
GET  /api/user/profile                 # Get user profile data
POST /api/user/change-password         # Change user password
POST /api/user/test-email              # Real-time email validation (Redis)
GET  /api/user/list                    # List all users (Admin)
POST /api/user/create                  # Create new user (Admin)
PUT  /api/user/update/{id}             # Update user data (Admin)
DELETE /api/user/delete/{id}           # Delete user (Admin)
```

### Course Management
```http
GET  /api/courses/list                 # List all courses
GET  /api/courses/{id}                 # Get course details
POST /api/courses/create               # Create new course (Admin)
POST /api/courses/generate-description # AI-powered description generation
PUT  /api/courses/update/{id}          # Update course (Admin)
DELETE /api/courses/delete/{id}        # Delete course (Admin)
```

### Student Operations
```http
GET  /api/student/my-courses           # Get student's enrolled courses
POST /api/student/enroll               # Enroll in course
POST /api/student/drop                 # Drop from course
GET  /api/student/transcript           # Get academic transcript
GET  /api/student/conflicts            # Check schedule conflicts
```

### Department & Semester Management
```http
GET  /api/departments/list             # List departments
GET  /api/semesters/current            # Get current semester
GET  /api/semesters/list               # List all semesters
```

### Background Jobs & Monitoring
```http
GET  /hangfire                         # Background jobs dashboard (Admin)
GET  /api/dashboard/stats              # System statistics
```

## ⚙️ Kurulum ve Yapılandırma

### 1. Veritabanı Kurulumu
```xml
<!-- Web.config connection string -->
<connectionStrings>
  <add name="OgrenciPortalContext" 
       connectionString="Data Source=SERVER;Initial Catalog=DBNAME;User ID=USER;Password=PASS;TrustServerCertificate=true" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 2. Environment Variables (.env)
```bash
# JWT Configuration
JWT_MASTER_KEY="your_super_secret_jwt_key_minimum_256_bits_long"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"
ACCESS_TOKEN_EXPIRATION_MINUTES=15
REFRESH_TOKEN_EXPIRATION_DAYS=7

# Redis Configuration
REDIS_CONNECTION_STRING="localhost:6379,abortConnect=false"

# AI Configuration
DEEPSEEK_API_KEY="your_deepseek_api_key"
DEEPSEEK_API_URL="https://api.deepseek.com/chat/completions"

# Email Configuration
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="your-email@gmail.com"
SMTP_PASS="your-app-password"

# Background Jobs
HANGFIRE_DASHBOARD_USERNAME="admin"
HANGFIRE_DASHBOARD_PASSWORD="secure_password_here"
```

### 3. Redis Cache Setup
Redis otomatik olarak aşağıdaki işlemleri gerçekleştirir:
- **Startup Cache**: Uygulama başlangıcında tüm email adresleri cache'lenir
- **Scheduled Updates**: Her 15 dakikada cache otomatik olarak yenilenir
- **Real-time Validation**: Form validation instant response sağlar

### 4. Hangfire Background Jobs
```csharp
// Startup.cs içinde otomatik konfigürasyon
RecurringJob.AddOrUpdate(
    "cache-user-addresses",
    () => CheckUserData.CacheUserAddressesAsync(),
    "*/15 * * * *"  // Her 15 dakikada çalışır
);
```

## 🔒 Güvenlik Yapılandırması

### JWT Token Security
- **Access Token**: 15 dakika geçerlilik süresi
- **Refresh Token**: 7 gün geçerlilik süresi  
- **Token Rotation**: Her refresh işleminde yeni token üretimi
- **Secure Storage**: HttpOnly cookies ile XSS koruması

### API Security Best Practices
- **CORS Policy**: Sadece izin verilen origin'lere erişim
- **Rate Limiting**: API endpoint'leri için istek sınırlaması
- **Input Validation**: Model validation ve sanitization
- **SQL Injection Protection**: Entity Framework parameterized queries

### Background Jobs Security
- **Dashboard Authentication**: Hangfire dashboard için yetkilendirme
- **Job Authorization**: Sadece admin kullanıcılar background jobs yönetimi
- **Secure Connections**: Redis ve SQL bağlantıları için TLS/SSL

## 🚀 Development & Testing

### API Testing
- **Swagger UI**: `https://localhost:44301/swagger` - Interactive API documentation
- **Postman Collections**: Hazır test koleksiyonları
- **Unit Tests**: Controller ve service layer testleri

### Monitoring & Debugging
- **log4net Integration**: Comprehensive application logging
- **Hangfire Dashboard**: Background job monitoring ve management
- **Redis Monitoring**: Cache performance ve hit/miss oranları
- **Application Insights**: Production monitoring (optional)

---

## English Version

# OgrenciPortalApi - Web API Backend Project

This project contains all business logic, data access layer, and backend services for the Student Platform. Designed following modern RESTful principles, it provides secure and scalable API services.

## 🎯 Core Responsibilities

### Authentication & Authorization  
- JWT (JSON Web Token) based stateless authentication
- Refresh token mechanism for secure session renewal
- Role-based access control (Admin, Advisor, Student)
- Claims-based authorization with granular permission management

### Data Management & Business Logic
- User, student, advisor management and CRUD operations
- Course, department, semester management and academic operations
- Course enrollment operations and conflict checking  
- Secure database data consistency

### Modern Backend Features
- **Redis Cache Integration**: Real-time email validation and performance optimization
- **AI-Powered Content Generation**: Automated course description generation via Deepseek API
- **Background Job Processing**: Scheduled tasks and automated operations with Hangfire
- **Real-time Data Validation**: Instant form validation and user feedback

### API Services
- Structured data response in JSON format
- Comprehensive error handling and logging
- Swagger documentation and API testing interface
- CORS support for cross-origin requests

## 🚀 Technology Stack

### Core Technologies
- **ASP.NET Web API 2** (.NET Framework 4.7.2) - RESTful API framework
- **Entity Framework 6.5.1** - ORM (Database-First approach)
- **Microsoft SQL Server** - Primary database
- **OWIN 4.2.3** - Middleware pipeline

### Cache & Performance
- **Redis Stack 2.8.58** - In-memory caching and data store
- **StackExchange.Redis 2.8.58** - Redis client library
- **Hangfire 1.8.21** - Background job processing
- **Hangfire.SqlServer 1.8.21** - SQL Server storage for jobs

### Security & Authentication
- **Microsoft.Owin.Security.Jwt 4.2.3** - JWT middleware
- **System.IdentityModel.Tokens.Jwt 8.13.0** - JWT token handling
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Microsoft.AspNet.Cors 5.3.0** - Cross-origin request support

### AI & External Integrations
- **Deepseek API** - AI content generation
- **MailKit 4.13.0** - Modern email handling
- **MimeKit 4.13.0** - Email message formatting

### Development & Utility
- **Swashbuckle 5.6.0** - API documentation (Swagger UI)
- **log4net 3.1.0** - Comprehensive logging
- **Newtonsoft.Json 13.0.3** - JSON serialization
- **DotNetEnv 3.1.1** - Environment configuration
- **Unity.Container 5.11.8** - Dependency injection

## 📡 API Endpoints

[Same endpoint structure as Turkish version]

## ⚙️ Installation and Configuration

[Same configuration structure as Turkish version with English descriptions]

## 🔒 Security Configuration  

[Same security structure as Turkish version with English descriptions]

## 🚀 Development & Testing

[Same development structure as Turkish version with English descriptions]