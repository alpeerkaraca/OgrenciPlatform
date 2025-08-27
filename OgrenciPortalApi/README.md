# ⚡ OgrenciPortalApi - Güvenli Web API Projesi / Secure Web API Project

[🇹🇷 Türkçe](#türkçe) | [🇺🇸 English](#english)

---

## 🇹🇷 Türkçe

Bu proje, Öğrenci Platformu'nun tüm iş mantığını ve veri erişim katmanını içeren güvenli ASP.NET Web API projesidir. Modern güvenlik standartlarına uygun olarak RESTful prensiplerine göre servis hizmeti sunar.

### 🎯 Sorumlulukları

- **🔐 Güvenli Kimlik Doğrulama**: JWT (JSON Web Token) tabanlı kullanıcı doğrulama ve yetkilendirme sistemi
- **👥 Kullanıcı Yönetimi**: Öğrenci, danışman ve admin hesapları için güvenli CRUD işlemleri
- **📚 Akademik Veri Yönetimi**: Ders, bölüm, dönem ve not yönetimi güvenlik kontrolleri ile
- **🛡️ Veri Güvenliği**: Veritabanı ile güvenli iletişim ve veri tutarlılığını sağlama
- **📋 İş Kuralları**: Ders kontenjanı, çakışma kontrolü gibi güvenlik odaklı iş kurallarının uygulanması
- **🔒 API Güvenliği**: İstemci uygulamalara güvenli JSON formatında veri sunma

### 🛡️ Güvenlik Özellikleri

#### 🔒 JWT Tabanlı Kimlik Doğrulama
```csharp
public class TokenManager
{
    // Güvenli token üretimi
    public static string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtMasterKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = AppSettings.JwtIssuer,
            Audience = AppSettings.JwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}
```

#### 🔐 Güvenli Token Yönetimi
- **Access Token**: 1 saat geçerlilik süresi ile kısa süreli erişim
- **Refresh Token**: RNGCryptoServiceProvider ile güvenli rastgele token üretimi
- **Token Validation**: SigningCredentials ile token imza doğrulaması
- **Claims-based Authorization**: Kullanıcı rolleri ve izinlerin güvenli yönetimi

#### 🛡️ Parola Güvenliği
```csharp
// BCrypt.Net ile güvenli parola hash'leme
public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
}

public static bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```
- **BCrypt.Net-Next** ile güçlü parola hash'leme
- **Salt** değeri ile her parola için farklı hash
- **Cost Factor 12**: Brute force saldırılarına karşı koruma
- **Password Policy**: Güçlü parola gereksinimleri zorlama

#### 🔒 Role-Based Access Control (RBAC)
```csharp
[CustomAuth(Roles.Admin)]
public class AdminController : ApiController
{
    [HttpPost]
    [CustomAuth(Roles.Admin, Roles.Advisor)]
    public IHttpActionResult CreateCourse([FromBody] CreateCourseDTO dto)
    {
        // Sadece Admin ve Advisor rolleri erişebilir
    }
}
```
- **Admin**: Tam sistem erişimi ve kullanıcı yönetimi
- **Advisor**: Kendi öğrencileri ve derslerini yönetme
- **Student**: Sadece kendi verilerini görüntüleme ve güncelleme
- **Granular Permissions**: Action-level rol kontrolü

#### 🛡️ API Güvenlik Korumaları

**CSRF Protection**
```csharp
[ValidateJsonAntiForgeryToken]
public IHttpActionResult SecureAction([FromBody] DataModel data)
{
    // CSRF korumalı endpoint
}
```

**Input Validation**
```csharp
public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    public string Password { get; set; }
}
```

#### 🔍 Güvenlik İzleme ve Logging
```csharp
private static readonly ILog Logger = LogManager.GetLogger(typeof(AuthController));

public IHttpActionResult Login([FromBody] LoginDTO dto)
{
    try
    {
        // Login logic
        Logger.Info($"Successful login for user: {dto.Email}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Logger.Warn($"Failed login attempt for: {dto.Email} - {ex.Message}");
        return Unauthorized();
    }
}
```

- **log4net Integration**: Tüm güvenlik olaylarının kapsamlı loglanması
- **Failed Attempt Tracking**: Başarısız giriş ve erişim denemeleri
- **Security Event Logging**: Kritik güvenlik olaylarının audit trail'i
- **Error Handling**: Stack trace gizleme ve güvenli hata mesajları

#### 🔐 Veritabanı Güvenliği
- **Entity Framework**: SQL Injection koruması
- **Parameterized Queries**: Güvenli SQL sorgu oluşturma
- **Connection String Encryption**: Bağlantı dizelerinin şifrelenmiş saklanması
- **Database-First Approach**: Güvenli veri modelleme

### 🔧 Teknolojiler

#### Core Framework
- **ASP.NET Web API 2**: RESTful web servis framework
- **Entity Framework 6**: Database-First ORM
- **.NET Framework 4.8**: Stable enterprise platform

#### Security & Authentication
- **JWT (JSON Web Token)**: Stateless authentication
- **BCrypt.Net-Next**: Secure password hashing
- **System.IdentityModel.Tokens.Jwt**: Token validation
- **OWIN Security**: Authentication pipeline

#### Documentation & Testing
- **Swagger/OpenAPI**: Interactive API documentation
- **Newtonsoft.Json**: Secure JSON serialization
- **log4net**: Enterprise logging framework

### 🚀 Kurulum ve Çalıştırma

#### 1. Güvenlik Yapılandırması

**.env dosyası oluşturun:**
```bash
cp .env.example .env
```

**.env güvenlik ayarları:**
```ini
# JWT Güvenlik (256-bit anahtarlar kullanın)
JWT_MASTER_KEY="your_super_secure_256_bit_master_key_here_minimum_32_chars"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"

# API Configuration
API_BASE_ADDRESS="https://localhost:8000"

# Database (Production'da şifrelenmiş olmalı)
DB_CONNECTION_STRING="Data Source=server;Initial Catalog=db;Integrated Security=true;Encrypt=true;TrustServerCertificate=false"
```

⚠️ **KRİTİK GÜVENLİK UYARISI**: 
- JWT_MASTER_KEY en az 256-bit (32 karakter) güçlü olmalı
- Production'da HTTPS zorunlu
- Database bağlantısında şifreleme aktif

#### 2. Veritabanı Güvenlik Yapılandırması

**Connection String (Web.config):**
```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=server;Initial Catalog=OgrenciDB;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- HTTPS Enforcement -->
    <add key="RequireHttps" value="true" />
    <!-- JWT Settings -->
    <add key="JwtExpirationHours" value="1" />
    <add key="RefreshTokenExpirationDays" value="7" />
  </appSettings>
</configuration>
```

#### 3. IIS/Server Güvenlik Ayarları

**HTTPS Konfigürasyonu:**
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="HTTPS Redirect" stopProcessing="true">
        <match url="(.*)"/>
        <conditions>
          <add input="{HTTPS}" pattern="^OFF$"/>
        </conditions>
        <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent"/>
      </rule>
    </rules>
  </rewrite>
  
  <httpProtocol>
    <customHeaders>
      <add name="X-Frame-Options" value="DENY"/>
      <add name="X-Content-Type-Options" value="nosniff"/>
      <add name="X-XSS-Protection" value="1; mode=block"/>
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains"/>
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

#### 4. Bağımlılıkları Yükleyin

```bash
# Core packages
Install-Package Microsoft.AspNet.WebApi
Install-Package EntityFramework
Install-Package Microsoft.AspNet.WebApi.WebHost

# Security packages
Install-Package System.IdentityModel.Tokens.Jwt
Install-Package BCrypt.Net-Next
Install-Package Microsoft.Owin.Security.Jwt

# Documentation
Install-Package Swashbuckle

# Logging
Install-Package log4net
```

#### 5. Başlatma

```bash
# 1. Database oluşturun ve migration çalıştırın
Update-Database

# 2. SSL sertifikası yükleyin (production)
# 3. IIS'de HTTPS binding yapın

# 4. Visual Studio'dan başlatın
# Proje https://localhost:8000/ adresinde çalışacak

# 5. API dokümantasyonuna erişin
# https://localhost:8000/swagger
```

### 🔐 Güvenlik En İyi Uygulamaları

#### Development Environment
- [ ] HTTPS her zaman aktif
- [ ] Debug mode production'da kapalı  
- [ ] Strong JWT keys (256-bit minimum)
- [ ] Database encryption aktif
- [ ] Secure error pages

#### Production Deployment  
- [ ] SSL certificate yüklü ve geçerli
- [ ] Security headers yapılandırılmış
- [ ] Rate limiting implementasyonu
- [ ] API versioning stratejisi
- [ ] Database connection pooling
- [ ] Log rotation ve monitoring

#### API Security
- [ ] Input validation tüm endpoints
- [ ] Output encoding/sanitization  
- [ ] CORS policy yapılandırması
- [ ] API key management (eğer gerekli)
- [ ] Request/response logging
- [ ] Audit trail implementation

### 🐛 Güvenlik Test Procedures

#### Automated Security Testing
```bash
# OWASP ZAP API Security Test
zap.sh -cmd -quickurl https://localhost:8000/swagger

# SQL Injection Test
sqlmap -u "https://localhost:8000/api/users" --cookie="AuthToken=xxx"

# JWT Token Security Test
jwt-cli decode "your_jwt_token_here"
```

#### Manual Security Testing
```bash
# Test HTTPS enforcement
curl -I http://localhost:8000/api/auth/login

# Test JWT token validation  
curl -H "Authorization: Bearer invalid_token" \
     https://localhost:8000/api/users

# Test CORS policy
curl -H "Origin: https://malicious-site.com" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: X-Requested-With" \
     -X OPTIONS https://localhost:8000/api/auth/login
```

### 📊 Security Monitoring

#### Key Security Metrics
- **Authentication Failures**: Login başarısızlık oranı
- **Token Expiration**: Token renewal patterns
- **API Abuse**: Rate limiting violations  
- **Data Access Patterns**: Anormal veri erişimi
- **Error Rates**: 4xx/5xx response codes

#### Log Analysis Queries
```sql
-- Başarısız login denemeleri
SELECT COUNT(*), ClientIP, AttemptTime 
FROM SecurityLogs 
WHERE EventType = 'LoginFailure' 
  AND AttemptTime > DATEADD(hour, -1, GETDATE())
GROUP BY ClientIP, AttemptTime
HAVING COUNT(*) > 5

-- Şüpheli API çağrıları
SELECT Endpoint, COUNT(*), ClientIP
FROM APILogs
WHERE ResponseCode = 401
  AND RequestTime > DATEADD(hour, -24, GETDATE())
GROUP BY Endpoint, ClientIP
ORDER BY COUNT(*) DESC
```

### 📚 API Documentation

#### Swagger/OpenAPI
- **Interactive Documentation**: `https://localhost:8000/swagger`
- **API Testing Interface**: Built-in Swagger UI
- **Schema Definitions**: Automated from code annotations
- **Security Definitions**: JWT Bearer token requirements

#### API Versioning
```csharp
[RoutePrefix("api/v1/users")]
public class UsersV1Controller : ApiController
{
    [Route("")]
    [HttpGet]
    public IHttpActionResult GetUsers() { }
}
```

---

## 🇺🇸 English

This project is a secure ASP.NET Web API that contains all business logic and data access layer of the Student Platform. It provides services according to RESTful principles in compliance with modern security standards.

### 🎯 Responsibilities

- **🔐 Secure Authentication**: JWT (JSON Web Token) based user authentication and authorization system
- **👥 User Management**: Secure CRUD operations for student, advisor, and admin accounts
- **📚 Academic Data Management**: Course, department, term, and grade management with security controls
- **🛡️ Data Security**: Secure communication with database and ensuring data integrity
- **📋 Business Rules**: Implementation of security-focused business rules like course quotas and conflict checking
- **🔒 API Security**: Providing secure JSON-formatted data to client applications

### 🛡️ Security Features

#### 🔒 JWT-Based Authentication
```csharp
public class TokenManager
{
    // Secure token generation
    public static string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtMasterKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = AppSettings.JwtIssuer,
            Audience = AppSettings.JwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}
```

#### 🔐 Secure Token Management
- **Access Token**: Short-term access with 1-hour validity
- **Refresh Token**: Secure random token generation with RNGCryptoServiceProvider
- **Token Validation**: Token signature verification with SigningCredentials
- **Claims-based Authorization**: Secure management of user roles and permissions

#### 🛡️ Password Security
```csharp
// Secure password hashing with BCrypt.Net
public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
}

public static bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```
- **BCrypt.Net-Next** for strong password hashing
- **Salt** value for different hash per password
- **Cost Factor 12**: Protection against brute force attacks
- **Password Policy**: Enforcing strong password requirements

#### 🔒 Role-Based Access Control (RBAC)
```csharp
[CustomAuth(Roles.Admin)]
public class AdminController : ApiController
{
    [HttpPost]
    [CustomAuth(Roles.Admin, Roles.Advisor)]
    public IHttpActionResult CreateCourse([FromBody] CreateCourseDTO dto)
    {
        // Only Admin and Advisor roles can access
    }
}
```
- **Admin**: Full system access and user management
- **Advisor**: Manage own students and courses
- **Student**: View and update own data only
- **Granular Permissions**: Action-level role control

#### 🛡️ API Security Protections

**CSRF Protection**
```csharp
[ValidateJsonAntiForgeryToken]
public IHttpActionResult SecureAction([FromBody] DataModel data)
{
    // CSRF protected endpoint
}
```

**Input Validation**
```csharp
public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    public string Password { get; set; }
}
```

#### 🔍 Security Monitoring and Logging
```csharp
private static readonly ILog Logger = LogManager.GetLogger(typeof(AuthController));

public IHttpActionResult Login([FromBody] LoginDTO dto)
{
    try
    {
        // Login logic
        Logger.Info($"Successful login for user: {dto.Email}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Logger.Warn($"Failed login attempt for: {dto.Email} - {ex.Message}");
        return Unauthorized();
    }
}
```

- **log4net Integration**: Comprehensive logging of all security events
- **Failed Attempt Tracking**: Failed login and access attempts
- **Security Event Logging**: Audit trail of critical security events
- **Error Handling**: Stack trace hiding and secure error messages

#### 🔐 Database Security
- **Entity Framework**: SQL Injection protection
- **Parameterized Queries**: Secure SQL query construction
- **Connection String Encryption**: Encrypted storage of connection strings
- **Database-First Approach**: Secure data modeling

### 🔧 Technologies

#### Core Framework
- **ASP.NET Web API 2**: RESTful web service framework
- **Entity Framework 6**: Database-First ORM
- **.NET Framework 4.8**: Stable enterprise platform

#### Security & Authentication
- **JWT (JSON Web Token)**: Stateless authentication
- **BCrypt.Net-Next**: Secure password hashing
- **System.IdentityModel.Tokens.Jwt**: Token validation
- **OWIN Security**: Authentication pipeline

#### Documentation & Testing
- **Swagger/OpenAPI**: Interactive API documentation
- **Newtonsoft.Json**: Secure JSON serialization
- **log4net**: Enterprise logging framework

### 🚀 Installation and Setup

#### 1. Security Configuration

**Create .env file:**
```bash
cp .env.example .env
```

**.env security settings:**
```ini
# JWT Security (use 256-bit keys)
JWT_MASTER_KEY="your_super_secure_256_bit_master_key_here_minimum_32_chars"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"

# API Configuration
API_BASE_ADDRESS="https://localhost:8000"

# Database (should be encrypted in production)
DB_CONNECTION_STRING="Data Source=server;Initial Catalog=db;Integrated Security=true;Encrypt=true;TrustServerCertificate=false"
```

⚠️ **CRITICAL SECURITY WARNING**: 
- JWT_MASTER_KEY must be at least 256-bit (32 characters) strong
- HTTPS mandatory in production
- Database connection encryption active

#### 2. Database Security Configuration

**Connection String (Web.config):**
```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=server;Initial Catalog=OgrenciDB;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- HTTPS Enforcement -->
    <add key="RequireHttps" value="true" />
    <!-- JWT Settings -->
    <add key="JwtExpirationHours" value="1" />
    <add key="RefreshTokenExpirationDays" value="7" />
  </appSettings>
</configuration>
```

#### 3. IIS/Server Security Settings

**HTTPS Configuration:**
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="HTTPS Redirect" stopProcessing="true">
        <match url="(.*)"/>
        <conditions>
          <add input="{HTTPS}" pattern="^OFF$"/>
        </conditions>
        <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent"/>
      </rule>
    </rules>
  </rewrite>
  
  <httpProtocol>
    <customHeaders>
      <add name="X-Frame-Options" value="DENY"/>
      <add name="X-Content-Type-Options" value="nosniff"/>
      <add name="X-XSS-Protection" value="1; mode=block"/>
      <add name="Strict-Transport-Security" value="max-age=31536000; includeSubDomains"/>
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

#### 4. Install Dependencies

```bash
# Core packages
Install-Package Microsoft.AspNet.WebApi
Install-Package EntityFramework
Install-Package Microsoft.AspNet.WebApi.WebHost

# Security packages
Install-Package System.IdentityModel.Tokens.Jwt
Install-Package BCrypt.Net-Next
Install-Package Microsoft.Owin.Security.Jwt

# Documentation
Install-Package Swashbuckle

# Logging
Install-Package log4net
```

#### 5. Launch

```bash
# 1. Create database and run migrations
Update-Database

# 2. Install SSL certificate (production)
# 3. Configure HTTPS binding in IIS

# 4. Start from Visual Studio
# Project will run at https://localhost:8000/

# 5. Access API documentation
# https://localhost:8000/swagger
```

### 🔐 Security Best Practices

#### Development Environment
- [ ] HTTPS always active
- [ ] Debug mode off in production  
- [ ] Strong JWT keys (256-bit minimum)
- [ ] Database encryption active
- [ ] Secure error pages

#### Production Deployment  
- [ ] SSL certificate installed and valid
- [ ] Security headers configured
- [ ] Rate limiting implementation
- [ ] API versioning strategy
- [ ] Database connection pooling
- [ ] Log rotation and monitoring

#### API Security
- [ ] Input validation on all endpoints
- [ ] Output encoding/sanitization  
- [ ] CORS policy configuration
- [ ] API key management (if needed)
- [ ] Request/response logging
- [ ] Audit trail implementation

### 🐛 Security Testing Procedures

#### Automated Security Testing
```bash
# OWASP ZAP API Security Test
zap.sh -cmd -quickurl https://localhost:8000/swagger

# SQL Injection Test
sqlmap -u "https://localhost:8000/api/users" --cookie="AuthToken=xxx"

# JWT Token Security Test
jwt-cli decode "your_jwt_token_here"
```

#### Manual Security Testing
```bash
# Test HTTPS enforcement
curl -I http://localhost:8000/api/auth/login

# Test JWT token validation  
curl -H "Authorization: Bearer invalid_token" \
     https://localhost:8000/api/users

# Test CORS policy
curl -H "Origin: https://malicious-site.com" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: X-Requested-With" \
     -X OPTIONS https://localhost:8000/api/auth/login
```

### 📊 Security Monitoring

#### Key Security Metrics
- **Authentication Failures**: Login failure rate
- **Token Expiration**: Token renewal patterns
- **API Abuse**: Rate limiting violations  
- **Data Access Patterns**: Abnormal data access
- **Error Rates**: 4xx/5xx response codes

#### Log Analysis Queries
```sql
-- Failed login attempts
SELECT COUNT(*), ClientIP, AttemptTime 
FROM SecurityLogs 
WHERE EventType = 'LoginFailure' 
  AND AttemptTime > DATEADD(hour, -1, GETDATE())
GROUP BY ClientIP, AttemptTime
HAVING COUNT(*) > 5

-- Suspicious API calls
SELECT Endpoint, COUNT(*), ClientIP
FROM APILogs
WHERE ResponseCode = 401
  AND RequestTime > DATEADD(hour, -24, GETDATE())
GROUP BY Endpoint, ClientIP
ORDER BY COUNT(*) DESC
```

### 📚 API Documentation

#### Swagger/OpenAPI
- **Interactive Documentation**: `https://localhost:8000/swagger`
- **API Testing Interface**: Built-in Swagger UI
- **Schema Definitions**: Automated from code annotations
- **Security Definitions**: JWT Bearer token requirements

#### API Versioning
```csharp
[RoutePrefix("api/v1/users")]
public class UsersV1Controller : ApiController
{
    [Route("")]
    [HttpGet]
    public IHttpActionResult GetUsers() { }
}
```