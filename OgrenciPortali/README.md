# OgrenciPortali - MVC Frontend Project

**Türkçe** | [English](#english-version)

Bu proje, Öğrenci Platformu'nun kullanıcı arayüzünü oluşturan ASP.NET MVC projesidir. Modern, responsive ve güvenli web arayüzü ile kullanıcıların sistemle etkileşime girdiği tüm ekranları barındırır.

## 🎯 Temel Sorumluluklar

### User Interface & Experience
- **Responsive Web Design**: Bootstrap 5.3.7 ile mobile-first yaklaşım
- **Modern UI Components**: Interactive modals, real-time form validation
- **Multi-role Dashboards**: Admin, Danışman ve Öğrenci için özelleştirilmiş arayüzler
- **Real-time User Feedback**: Instant validation ve AJAX-based interactions

### Authentication & Security Frontend
- **Secure Login Interface**: JWT token handling ve session management
- **Role-based UI**: Kullanıcı rolüne göre dinamik menü ve erişim kontrolü
- **CSRF Protection**: Anti-forgery token implementation
- **Input Validation**: Client-side validation ve sanitization

### Real-time Features
- **Instant Email Validation**: Redis-based real-time email existence checking
- **Live Form Feedback**: Kullanıcılar form girişlerinde anında geri bildirim alır
- **Dynamic Content Loading**: AJAX calls ile smooth user experience
- **Auto-complete Functions**: Real-time data suggestions

### Academic Management Interface
- **Course Enrollment System**: Öğrenciler için ders seçme ve kayıt arayüzleri
- **Schedule Management**: Ders programı görüntüleme ve çakışma kontrolü
- **Advisor Dashboard**: Danışmanlar için öğrenci ve ders onay işlemleri
- **Admin Panel**: Kullanıcı, bölüm, ders ve dönem yönetimi

## 🚀 Teknoloji Stack'i

### Frontend Framework
- **ASP.NET MVC 5** (.NET Framework 4.7.2) - Web application framework
- **Razor View Engine** - Server-side rendering
- **Autofac 6.4.0** - Dependency injection container

### UI & Styling
- **Bootstrap 5.3.7** - Responsive CSS framework
- **jQuery 3.7.1** - DOM manipulation ve event handling
- **jQuery.Validation 1.21.0** - Client-side form validation
- **Font Awesome Icons** - Modern icon library
- **Custom CSS** - Tailored theme ve brand styling

### Data & Communication
- **AutoMapper 15.0.1** - Object-to-object mapping
- **Microsoft.AspNet.WebApi.Client 6.0.0** - HTTP client for API calls
- **Newtonsoft.Json** - JSON serialization/deserialization
- **Fetch API Integration** - Modern asynchronous HTTP requests

### Security & Authentication
- **Microsoft.AspNet.Mvc 5.3.0** - MVC framework with security features  
- **DotNetEnv 3.1.1** - Environment variables management
- **Azure Integration** - Azure Identity ve cloud services support

### Development & Monitoring
- **log4net 3.1.0** - Application logging
- **Microsoft.Extensions.Logging** - Structured logging
- **Development Tools** - Hot reload, debugging support

## 🎨 Key Features

### Real-time Validation System
```javascript
// Email validation with Redis cache
async function validateEmail(email) {
    const response = await fetch('/api/user/test-email', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: email })
    });
    return await response.json();
}
```

### Dynamic UI Components
- **Interactive Modals**: Modern popup dialogs for user actions
- **Progress Indicators**: Real-time feedback for background operations
- **Responsive Tables**: Mobile-friendly data display
- **Smart Forms**: Auto-validation ve user guidance

### Multi-role Dashboard Support
- **Admin Dashboard**: System management ve user administration
- **Advisor Dashboard**: Student management ve course approval workflows
- **Student Dashboard**: Course enrollment, schedule viewing, transcript access

## ⚙️ Kurulum ve Yapılandırma

### 1. Environment Configuration (.env)
```bash
# API Configuration
API_BASE_ADDRESS="https://localhost:44301/"

# JWT Configuration (Frontend)
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"

# Redis Configuration (for real-time features)
REDIS_CONNECTION_STRING="localhost:6379"

# Application Settings
APPLICATION_NAME="Öğrenci Platformu"
SUPPORT_EMAIL="support@yourschool.edu"
```

### 2. API Connection Setup
```xml
<!-- Web.config app settings -->
<appSettings>
  <add key="ApiBaseAddress" value="https://localhost:44301/" />
  <add key="EnableRealTimeValidation" value="true" />
  <add key="CacheTimeout" value="900" />
</appSettings>
```

### 3. Real-time Features Configuration
- **Auto-validation**: Form fields validate as user types
- **Instant feedback**: Success/error messages appear immediately  
- **Cache integration**: Email validation uses Redis for speed
- **Progressive enhancement**: Works without JavaScript (graceful degradation)

## 🔒 Security Implementation

### Client-side Security
- **Input Sanitization**: XSS prevention on all user inputs
- **CSRF Token Validation**: Anti-forgery tokens on all forms
- **Secure Cookie Handling**: HttpOnly flags ve secure transmission
- **Content Security Policy**: CSP headers for additional protection

### Authentication Flow
1. **Login Process**: Credentials sent to API, JWT token received
2. **Token Storage**: Secure storage in HttpOnly cookies
3. **Automatic Refresh**: Silent token refresh before expiration
4. **Secure Logout**: Token invalidation ve session cleanup

## 📱 Responsive Design

### Mobile-First Approach
- **Breakpoint Strategy**: Mobile (576px), Tablet (768px), Desktop (992px+)
- **Touch-Friendly Interface**: Button sizing ve spacing for touch devices
- **Adaptive Navigation**: Collapsible menu for mobile devices
- **Performance Optimization**: Lazy loading ve image optimization

### Cross-Browser Compatibility  
- **Modern Browsers**: Chrome, Firefox, Safari, Edge support
- **Progressive Enhancement**: Core functionality without JavaScript
- **Polyfill Support**: Backwards compatibility for older browsers

## 🎯 Development & Testing

### Local Development
```bash
# Start the frontend (after API is running)
1. Ensure OgrenciPortalApi is running on https://localhost:44301
2. Open OgrenciPortali project in Visual Studio
3. Start with IIS Express
4. Navigate to https://localhost:44302
```

### Real-time Features Testing
- **Email Validation**: Test instant email existence checking
- **Form Interactions**: Verify dynamic form behavior
- **AJAX Calls**: Monitor network requests for API integration
- **Cache Performance**: Test Redis-based validation speed

---

## English Version

# OgrenciPortali - MVC Frontend Project

This project contains the user interface for the Student Platform built with ASP.NET MVC. It provides a modern, responsive, and secure web interface for all user interactions with the system.

## 🎯 Core Responsibilities

[Same structure as Turkish version with English descriptions]

## 🚀 Technology Stack

[Same technology structure as Turkish version with English descriptions]

## 🎨 Key Features

[Same features structure as Turkish version with English descriptions]

## ⚙️ Installation and Configuration

[Same configuration structure as Turkish version with English descriptions]

## 🔒 Security Implementation

[Same security structure as Turkish version with English descriptions]

## 📱 Responsive Design

[Same responsive design structure as Turkish version with English descriptions]

## 🎯 Development & Testing

[Same development structure as Turkish version with English descriptions]