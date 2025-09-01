# 🖥️ OgrenciPortali - MVC Frontend

The modern, responsive web interface for the Student Platform system. Built with ASP.NET MVC 5 and Bootstrap 5, providing an intuitive user experience for students, advisors, and administrators.

## 🎯 Purpose & Responsibilities

This MVC project serves as the **primary user interface** providing:

- **🎨 User Interface**: Modern, responsive web interface using Bootstrap 5
- **🔐 Authentication UI**: Login, registration, and password management screens
- **👥 Role-Based Views**: Customized interfaces for Admin, Advisor, and Student roles
- **📱 Responsive Design**: Mobile-first approach with cross-device compatibility
- **🌐 API Integration**: Seamless communication with OgrenciPortalApi backend
- **📊 Data Visualization**: Charts, tables, and dashboard components
- **🔄 Real-time Updates**: Dynamic content updates and notifications

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────┐
│                 Presentation Layer              │
│   ┌─────────────┬─────────────┬─────────────┐   │
│   │    Views    │ Partial     │  Layouts    │   │
│   │   (Razor)   │   Views     │ & Masters   │   │
│   └─────────────┴─────────────┴─────────────┘   │
└─────────────────────────────────────────────────┘
                        │
┌─────────────────────────────────────────────────┐
│                Controller Layer                 │
│   ┌─────────────┬─────────────┬─────────────┐   │
│   │    MVC      │   Custom    │   Base      │   │
│   │ Controllers │ Attributes  │ Controller  │   │
│   └─────────────┴─────────────┴─────────────┘   │
└─────────────────────────────────────────────────┘
                        │
┌─────────────────────────────────────────────────┐
│              Service Integration                │
│   ┌─────────────┬─────────────┬─────────────┐   │
│   │ API Proxy   │ AutoMapper  │   Cache     │   │
│   │  Services   │  Profiles   │ Management  │   │
│   └─────────────┴─────────────┴─────────────┘   │
└─────────────────────────────────────────────────┘
                        │
┌─────────────────────────────────────────────────┐
│               Client-Side Layer                 │
│   ┌─────────────┬─────────────┬─────────────┐   │
│   │  Bootstrap  │   jQuery    │   Custom    │   │
│   │   Styles    │ Validation  │ JavaScript  │   │
│   └─────────────┴─────────────┴─────────────┘   │
└─────────────────────────────────────────────────┘
```

## 🚀 Technology Stack

### Core Framework
- **ASP.NET MVC 5** (.NET Framework 4.7.2)
- **Razor View Engine** - Server-side rendering
- **C# 7.0+** - Programming language

### Frontend Technologies
- **Bootstrap 5.3.7** - Responsive CSS framework
- **jQuery 3.7.1** - JavaScript library
- **jQuery Validation 1.21.0** - Client-side form validation
- **Font Awesome/Bootstrap Icons** - Icon library
- **DataTables** - Advanced table functionality

### Integration & Communication
- **Microsoft.AspNet.WebApi.Client 6.0.0** - HTTP client for API calls
- **Newtonsoft.Json 13.0.3** - JSON serialization/deserialization
- **AutoMapper 15.0.1** - Object-to-object mapping

### Dependency Injection & Configuration
- **Autofac 6.4.0** - IoC container
- **Autofac.Mvc5 6.1.0** - MVC integration
- **DotNetEnv 3.1.1** - Environment variable management

### Security & Authentication
- **Microsoft.IdentityModel.Tokens.Jwt** - JWT token handling
- **Custom Authorization Attributes** - Role-based access control
- **Azure.Identity** - Cloud authentication support

### Development & Build Tools
- **Microsoft.Web.Infrastructure** - Web hosting infrastructure
- **Microsoft.AspNet.Web.Optimization** - Bundling and minification
- **WebGrease** - CSS and JavaScript optimization

## 🎨 User Interface Features

### Responsive Design
- **Mobile-First Approach**: Optimized for smartphones and tablets
- **Bootstrap Grid System**: Flexible, responsive layouts
- **Cross-Browser Compatibility**: Supports modern browsers
- **Accessibility Features**: WCAG 2.1 compliance

### Theme & Styling
```css
/* Custom CSS Variables */
:root {
  --primary-color: #007bff;
  --secondary-color: #6c757d;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --warning-color: #ffc107;
  --info-color: #17a2b8;
}
```

### Interactive Components
- **Dynamic Forms**: Client-side validation with jQuery
- **Modal Dialogs**: Bootstrap modals for user interactions
- **Data Tables**: Sortable, searchable, paginated tables
- **Toast Notifications**: Real-time user feedback
- **Progress Indicators**: Loading states and progress bars

## 🔐 Security Implementation

### Authentication Flow
```csharp
// Custom Authorization Attribute
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthAttribute : Attribute, IAuthorizationFilter
{
    public string Roles { get; set; }
    
    public void OnAuthorization(AuthorizationContext filterContext)
    {
        // JWT token validation logic
        // Role-based access control
        // Redirect to login if unauthorized
    }
}
```

### View-Level Security
- **Role-Based Rendering**: Conditional UI elements based on user roles
- **CSRF Protection**: Anti-forgery tokens in forms
- **Secure Cookies**: HttpOnly and Secure flags
- **XSS Prevention**: Input encoding and validation

## 📱 User Interfaces

### Role-Based Dashboards

#### 👤 Admin Dashboard
```
┌─────────────────────────────────────────────────┐
│ 📊 System Statistics        📈 Usage Analytics │
│ ├─ Total Users: 1,250       ├─ Daily Active    │
│ ├─ Active Courses: 45       ├─ Course Views    │
│ └─ Departments: 8           └─ Enrollments     │
├─────────────────────────────────────────────────┤
│ 🎛️ Quick Actions                                │
│ [Add User] [Create Course] [Manage Dept.]      │
├─────────────────────────────────────────────────┤
│ 📋 Recent Activities                            │
│ • New student registered - John Doe            │
│ • Course updated - Computer Science 101        │
│ • Enrollment approved - Jane Smith             │
└─────────────────────────────────────────────────┘
```

#### 👨‍🏫 Advisor Dashboard
```
┌─────────────────────────────────────────────────┐
│ 👨‍🎓 My Students           📚 Assigned Courses    │
│ ├─ Total: 25             ├─ Active: 5          │
│ ├─ Pending Approvals: 3  ├─ Capacity: 150/200  │
│ └─ Graduated: 12         └─ Avg. Grade: B+     │
├─────────────────────────────────────────────────┤
│ ⏰ Pending Approvals                            │
│ • Course Enrollment - Math 201 - Alex Johnson  │
│ • Schedule Change - Physics 101 - Sarah Lee    │
├─────────────────────────────────────────────────┤
│ 📅 Upcoming Schedule                            │
│ • Monday 10:00 - Office Hours                  │
│ • Wednesday 14:00 - CS 101 Lecture             │
└─────────────────────────────────────────────────┘
```

#### 👨‍🎓 Student Dashboard
```
┌─────────────────────────────────────────────────┐
│ 📚 Current Enrollment    🎯 Academic Progress   │
│ ├─ Enrolled: 5 courses   ├─ GPA: 3.45          │
│ ├─ Credits: 15/18        ├─ Credits: 90/120     │
│ └─ Status: Good Standing └─ Expected Grad: 2025 │
├─────────────────────────────────────────────────┤
│ 📅 This Week's Schedule                         │
│ • Mon 09:00 - Mathematics 201 - Room A101      │
│ • Wed 11:00 - Physics 101 - Lab B205           │
│ • Fri 14:00 - Computer Science - Room C301     │
├─────────────────────────────────────────────────┤
│ 🔔 Notifications                                │
│ • Assignment due: Math homework (Due: Tomorrow) │
│ • Grade posted: Physics Exam - B+ (85%)        │
└─────────────────────────────────────────────────┘
```

## ⚙️ Configuration

### Environment Variables (.env)
```bash
# API Connection
API_BASE_ADDRESS="https://localhost:44301/"
API_TIMEOUT_SECONDS=30

# Application Settings
APP_NAME="Student Platform"
APP_VERSION="1.0.0"
ENVIRONMENT="Development"

# Session Configuration
SESSION_TIMEOUT_MINUTES=30
COOKIE_SECURE=true
COOKIE_HTTPONLY=true

# UI Configuration
ITEMS_PER_PAGE=25
MAX_FILE_UPLOAD_MB=10
ENABLE_CACHING=true

# Feature Flags
ENABLE_DARK_MODE=true
ENABLE_NOTIFICATIONS=true
ENABLE_ANALYTICS=false
```

### Web.config Settings
```xml
<appSettings>
  <add key="ApiBaseAddress" value="https://localhost:44301/" />
  <add key="webpages:Version" value="3.0.0.0" />
  <add key="webpages:Enabled" value="false" />
  <add key="ClientValidationEnabled" value="true" />
  <add key="UnobtrusiveJavaScriptEnabled" value="true" />
</appSettings>
```

## 🚀 Getting Started

### Prerequisites
- .NET Framework 4.7.2+
- Visual Studio 2019+ or VS Code
- OgrenciPortalApi running (for backend services)
- Modern web browser (Chrome, Firefox, Safari, Edge)

### Installation Steps

1. **Clone and Navigate**
```bash
git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
cd OgrenciPlatform/OgrenciPortali
```

2. **Configure Environment**
```bash
# Copy environment template
copy .env.example .env

# Edit .env with your configuration
notepad .env
```

3. **Update API Configuration**
```csharp
// In Web.config, update API base address
<add key="ApiBaseAddress" value="https://localhost:44301/" />
```

4. **Install Dependencies**
```bash
# Restore NuGet packages
nuget restore
```

5. **Run the Application**
```bash
# Using Visual Studio: F5 or Ctrl+F5
# Web App available at: https://localhost:44302/
```

6. **Verify Installation**
- Open browser: `https://localhost:44302/`
- Check API connectivity in browser console
- Test login functionality

## 🎨 Customization

### Theme Configuration
```css
/* Custom theme variables in Content/Site.css */
:root {
  --brand-primary: #007bff;
  --brand-secondary: #6c757d;
  --sidebar-bg: #343a40;
  --content-bg: #ffffff;
  --text-primary: #212529;
  --text-secondary: #6c757d;
}

/* Dark mode support */
[data-theme="dark"] {
  --content-bg: #1a1a1a;
  --text-primary: #ffffff;
  --text-secondary: #cccccc;
}
```

### Custom JavaScript Modules
```javascript
// Scripts/app/modules/
├── auth.js          // Authentication handling
├── dashboard.js     // Dashboard functionality  
├── courses.js       // Course management
├── notifications.js // Real-time notifications
└── utils.js        // Common utilities
```

## 📊 Features by Role

### Admin Features
- **User Management**: Create, edit, delete users
- **Course Administration**: Manage courses and offerings
- **Department Management**: Organizational structure
- **System Configuration**: Settings and parameters
- **Reports & Analytics**: Comprehensive reporting
- **Audit Logs**: System activity tracking

### Advisor Features
- **Student Management**: View and manage assigned students
- **Course Approval**: Approve/reject enrollment requests
- **Academic Tracking**: Monitor student progress
- **Schedule Management**: Manage course schedules
- **Communication**: Messages and notifications
- **Reports**: Student and course reports

### Student Features
- **Course Enrollment**: Browse and enroll in courses
- **Schedule View**: Personal class schedule
- **Grade Tracking**: View grades and transcripts
- **Profile Management**: Update personal information
- **Course Materials**: Access course resources
- **Academic Progress**: Track graduation requirements

## 🧪 Testing

### Manual Testing
1. **User Interface Testing**
   - Test responsive design on different screen sizes
   - Verify form validation
   - Check navigation flow

2. **Role-Based Testing**
   - Test admin functionality
   - Verify advisor permissions
   - Validate student access

3. **Cross-Browser Testing**
   - Chrome, Firefox, Safari, Edge
   - Mobile browsers (iOS Safari, Chrome Mobile)

### Automated Testing
```javascript
// Example JavaScript unit test
describe('Course Enrollment', function() {
    it('should validate course capacity', function() {
        expect(validateEnrollment(courseId, studentId)).toBe(true);
    });
    
    it('should prevent double enrollment', function() {
        expect(checkDuplicateEnrollment(courseId, studentId)).toBe(false);
    });
});
```

## 🔧 Development Guidelines

### MVC Best Practices
- **Separation of Concerns**: Keep controllers lightweight
- **View Models**: Use specific models for views
- **Partial Views**: Reusable UI components
- **Action Filters**: Cross-cutting concerns
- **Model Binding**: Secure parameter binding

### Frontend Best Practices
- **Progressive Enhancement**: Works without JavaScript
- **Accessibility**: ARIA labels and keyboard navigation
- **Performance**: Minified CSS/JS, image optimization
- **SEO**: Semantic HTML and meta tags
- **Security**: XSS prevention, CSRF protection

### Code Organization
```
OgrenciPortali/
├── Controllers/     # MVC Controllers
├── Views/          # Razor Views
│   ├── Shared/     # Layout and partial views
│   ├── Home/       # Home controller views
│   └── Student/    # Student controller views
├── Models/         # View models and DTOs
├── Content/        # CSS, images, fonts
├── Scripts/        # JavaScript files
├── Attributes/     # Custom attributes
└── Utils/          # Helper classes
```

## 🐛 Troubleshooting

### Common Issues

**1. API Connection Issues**
```javascript
// Check API connectivity
console.log('Testing API connection...');
fetch('/api/health')
    .then(response => console.log('API Status:', response.status))
    .catch(error => console.error('API Error:', error));
```

**2. Authentication Problems**
- Check JWT token expiration
- Verify API base address configuration
- Validate cookie settings

**3. Layout Issues**
- Clear browser cache
- Check Bootstrap version compatibility
- Verify CSS bundle configuration

**4. Form Validation Problems**
- Enable client validation: `ClientValidationEnabled = true`
- Include jQuery validation scripts
- Check model validation attributes

## 📱 Mobile Support

### Responsive Breakpoints
```css
/* Bootstrap 5 breakpoints */
/* Small devices (landscape phones, 576px and up) */
@media (min-width: 576px) { ... }

/* Medium devices (tablets, 768px and up) */
@media (min-width: 768px) { ... }

/* Large devices (desktops, 992px and up) */
@media (min-width: 992px) { ... }

/* Extra large devices (large desktops, 1200px and up) */
@media (min-width: 1200px) { ... }
```

### Mobile-Specific Features
- **Touch-Friendly Interface**: Large buttons and touch targets
- **Swipe Gestures**: Navigate between sections
- **Offline Support**: Basic offline functionality
- **Push Notifications**: Course reminders and updates

## 🚀 Performance Optimization

### Bundle Configuration
```csharp
// App_Start/BundleConfig.cs
bundles.Add(new StyleBundle("~/Content/css").Include(
    "~/Content/bootstrap.css",
    "~/Content/site.css"
));

bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
    "~/Scripts/jquery-{version}.js"
));
```

### Caching Strategy
- **Output Caching**: Cache rendered views
- **Browser Caching**: Static resources
- **API Response Caching**: Cache API responses
- **Memory Caching**: Frequently accessed data

## 📚 Additional Resources

- [ASP.NET MVC 5 Documentation](https://docs.microsoft.com/en-us/aspnet/mvc/)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)
- [jQuery Documentation](https://api.jquery.com/)
- [Razor View Engine](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor)
- [AutoMapper Documentation](https://automapper.org/)

---

**🔗 Related Projects:**
- [Backend API (OgrenciPortalApi)](../OgrenciPortalApi/README.md)
- [Shared Library](../Shared/README.md)
- [Main Documentation](../README.md)