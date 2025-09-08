# 🏗️ OgrenciPlatform - System Architecture Documentation

## 📋 Table of Contents
1. [Overview](#overview)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Database Schema](#database-schema)
5. [Authentication & Security](#authentication--security)
6. [API Endpoints](#api-endpoints)
7. [Frontend Architecture](#frontend-architecture)
8. [Data Flow](#data-flow)
9. [Advanced Features](#advanced-features)
10. [Deployment & Configuration](#deployment--configuration)
11. [Scaling Considerations](#scaling-considerations)

---

## 📖 Overview

**OgrenciPlatform** is a comprehensive, modern student management system built with Microsoft .NET technologies. The platform consists of two main applications:

### Core Applications
- **OgrenciPortali** - ASP.NET MVC 5 Frontend Application
- **OgrenciPortalApi** - ASP.NET Web API 2 Backend Service

### Supporting Libraries
- **Shared** - Common DTOs, Enums, and Constants
- **OgrenciPlatform.Shared** - Web-optimized Data Transfer Objects

The system serves three main user roles:
- **Admin** - System administration and management
- **Danışman (Advisor)** - Academic advisors managing students
- **Öğrenci (Student)** - Students enrolling in courses and managing academics

---

## 🏗️ System Architecture

### Layered Architecture Pattern

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│  ┌─────────────────────────────────────────────────────┐   │
│  │         OgrenciPortali (MVC Frontend)               │   │
│  │  • Controllers (MVC)                                │   │
│  │  • Views (Razor)                                    │   │
│  │  • Custom Authorization Attributes                  │   │
│  │  • Client-side JavaScript (jQuery, Bootstrap)      │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

                              │
                              ▼
                         HTTP/HTTPS
                         JSON/REST API
                              │
                              ▼

┌─────────────────────────────────────────────────────────────┐
│                     SERVICE LAYER                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │        OgrenciPortalApi (Web API Backend)           │   │
│  │  • API Controllers                                  │   │
│  │  • JWT Authentication                               │   │
│  │  • Business Logic                                   │   │
│  │  • Background Jobs (Hangfire)                      │   │
│  │  • External Integrations (AI, Email)               │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

                              │
                              ▼
                      Entity Framework 6.5.1
                              │
                              ▼

┌─────────────────────────────────────────────────────────────┐
│                      DATA LAYER                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │             SQL Server Database                     │   │
│  │  • Users, Courses, Departments                      │   │
│  │  • StudentCourses, OfferedCourses                   │   │
│  │  • Semesters, Enrollment Management                 │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    EXTERNAL SERVICES                        │
│  • Redis Cache (Real-time validation)                      │
│  • Deepseek AI API (Course descriptions)                   │
│  • Microsoft Azure AD (SSO Authentication)                 │
│  • SMTP Server (Email notifications)                       │
│  • Hangfire Dashboard (Job monitoring)                     │
└─────────────────────────────────────────────────────────────┘
```

### Component Interaction Diagram

```
Frontend (MVC)     ←→     Backend (API)     ←→     Database
      │                        │                      │
      ├── Authentication ──────┼── JWT Validation ────┤
      ├── User Interface ──────┼── Business Logic ────┤
      ├── Form Validation ─────┼── Data Validation ───┤
      ├── AJAX Calls ──────────┼── JSON Responses ────┤
      └── Session Management ──┼── Token Management ──┤
                               │
                               ├── Redis Cache ───────── Real-time Features
                               ├── Hangfire ─────────── Background Jobs
                               ├── AI Integration ───── Course Generation
                               └── Email Service ───── Notifications
```

---

## 🛠️ Technology Stack

### Frontend (OgrenciPortali)
| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Framework** | ASP.NET MVC | 5.3.0 | Web application framework |
| **UI Library** | Bootstrap | 5.3.7 | Responsive UI components |
| **JavaScript** | jQuery | 3.7.1 | DOM manipulation and AJAX |
| **Dependency Injection** | Autofac | 6.4.0 | IoC container |
| **Object Mapping** | AutoMapper | 15.0.1 | DTO mappings |
| **Authentication** | Azure Identity | 1.14.2 | Microsoft SSO integration |
| **Validation** | jQuery Validation | 1.21.0 | Client-side form validation |
| **Logging** | log4net | 3.1.0 | Application logging |

### Backend (OgrenciPortalApi)
| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Framework** | ASP.NET Web API | 5.3.0 | RESTful API services |
| **ORM** | Entity Framework | 6.5.1 | Data access layer |
| **Database** | SQL Server | - | Primary data storage |
| **Authentication** | JWT | - | Stateless authentication |
| **Background Jobs** | Hangfire | 1.8.21 | Scheduled task processing |
| **Caching** | Redis | - | Real-time data caching |
| **Email** | MailKit | 4.13.0 | Email notifications |
| **AI Integration** | Deepseek API | - | Course description generation |
| **Documentation** | Swagger | - | API documentation |
| **Security** | BCrypt.Net | 4.0.3 | Password hashing |

### Development & Deployment
| Component | Technology | Purpose |
|-----------|------------|---------|
| **Runtime** | .NET Framework | 4.7.2 |
| **IDE** | Visual Studio | 2022+ |
| **Version Control** | Git | Source control |
| **Configuration** | DotNetEnv | Environment variables |
| **Logging** | log4net | Application monitoring |

---

## 🗄️ Database Schema

### Core Entities

#### Users Table
```sql
Users {
    UserId (Guid, PK)
    Name (nvarchar)
    Surname (nvarchar)
    Email (nvarchar, Unique)
    Password (nvarchar, Hashed)
    Role (int) -- 1: Admin, 2: Advisor, 3: Student
    StudentNo (nvarchar, Nullable)
    DepartmentId (Guid, FK)
    AdvisorId (Guid, FK, Self-Reference)
    StudentYear (int, Nullable)
    IsActive (bit)
    IsFirstLogin (bit)
    RefreshToken (nvarchar)
    RefreshTokenExpTime (datetime)
    PasswordResetToken (nvarchar)
    ResetTokenExpiry (datetime)
    -- Audit Fields
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    IsDeleted, DeletedAt, DeletedBy
}
```

#### Courses Table
```sql
Courses {
    CourseId (Guid, PK)
    CourseCode (nvarchar)
    CourseName (nvarchar)
    Credits (int)
    DepartmentId (Guid, FK)
    Description (nvarchar, AI-Generated)
    -- Audit Fields
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    IsDeleted, DeletedAt, DeletedBy
}
```

#### OfferedCourses Table
```sql
OfferedCourses {
    Id (Guid, PK)
    CourseId (Guid, FK)
    SemesterId (Guid, FK)
    TeacherId (Guid, FK)
    Quota (int)
    CurrentUserCount (int)
    DayOfWeek (int) -- 1-7 (Monday-Sunday)
    StartTime (time)
    EndTime (time)
    CourseYear (int) -- 1-4
    Classroom (nvarchar)
    -- Audit Fields
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    IsDeleted, DeletedAt, DeletedBy
}
```

#### StudentCourses Table
```sql
StudentCourses {
    StudentId (Guid, PK, FK)
    OfferedCourseId (Guid, PK, FK)
    ApprovalStatus (int) -- 0: Pending, 1: Approved, 2: Rejected
    -- Audit Fields
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    IsDeleted, DeletedAt, DeletedBy
}
```

### Entity Relationships

```
Users (1) ──────────── (N) StudentCourses
   │                        │
   │                        │
   │                   OfferedCourses (1) ── (N)
   │                        │
   └── DepartmentId ────────┼── Courses (N) ── (1) Departments
                            │
                       Semesters (1) ── (N)
```

### Database Design Principles
- **GUID Primary Keys** - For distributed system compatibility
- **Soft Delete Pattern** - Logical deletion with audit trail
- **Audit Trail** - Complete change tracking on all entities
- **Self-Referencing** - Users table for advisor-student relationships
- **Normalization** - 3NF compliance for data integrity

---

## 🔐 Authentication & Security

### JWT-Based Authentication Flow

```
1. Login Request
   ┌─────────────┐    POST /api/auth/login    ┌──────────────┐
   │   Client    │ ──────────────────────────→ │   API Server │
   │ (Frontend)  │                            │ (Backend)    │
   └─────────────┘                            └──────────────┘
                                                      │
                                               Validate Credentials
                                                      │
                                                      ▼
   ┌─────────────┐    JWT + Refresh Token     ┌──────────────┐
   │   Client    │ ←────────────────────────── │   API Server │
   │ (Frontend)  │                            │ (Backend)    │
   └─────────────┘                            └──────────────┘

2. Subsequent API Calls
   ┌─────────────┐    Authorization: Bearer   ┌──────────────┐
   │   Client    │ ──────────────────────────→ │   API Server │
   │ (Frontend)  │         JWT Token          │ (Backend)    │
   └─────────────┘                            └──────────────┘

3. Token Refresh
   ┌─────────────┐    POST /api/auth/refresh  ┌──────────────┐
   │   Client    │ ──────────────────────────→ │   API Server │
   │ (Frontend)  │      Refresh Token         │ (Backend)    │
   └─────────────┘                            └──────────────┘
```

### Security Implementation

#### 1. **JWT Token Management**
```csharp
// Token Generation (TokenManager.cs)
public static string GenerateAccessToken(IEnumerable<Claim> claims)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(AppSettings.JwtMasterKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(AppSettings.AccessTokenExpMinutes),
        Issuer = AppSettings.JwtIssuer,
        Audience = AppSettings.JwtAudience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
```

#### 2. **Role-Based Authorization**
```csharp
// Custom Authorization Attribute
[CustomAuth(Roles.Admin, Roles.Danışman)]
public class CourseController : BaseApiController
{
    // Admin and Advisor only actions
}

// Frontend Authorization
[System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
[RoutePrefix("api/courses")]
public class CourseController : BaseApiController
```

#### 3. **Password Security**
```csharp
// Password Hashing with BCrypt
public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}

public static bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

#### 4. **Microsoft Azure SSO Integration**
- Azure Active Directory integration for single sign-on
- OAuth 2.0 authentication flow
- Automatic user provisioning from Azure AD

#### 5. **Security Headers & CORS**
```csharp
// CORS Configuration
[EnableCors(origins: "*", headers: "*", methods: "*")]
public class WebApiConfig
{
    // Cross-origin request handling
}
```

### Security Best Practices Implemented
- ✅ **JWT with Refresh Tokens** - Secure stateless authentication
- ✅ **Password Hashing** - BCrypt with salt
- ✅ **Role-Based Access Control** - Granular permissions
- ✅ **HTTPS Enforcement** - Secure data transmission
- ✅ **SQL Injection Prevention** - Entity Framework parameterization
- ✅ **XSS Protection** - Input validation and encoding
- ✅ **CSRF Protection** - Anti-forgery tokens
- ✅ **Session Management** - Secure cookie handling

---

## 🌐 API Endpoints

### Authentication Endpoints
```http
POST   /api/auth/login              # User authentication
POST   /api/auth/refresh-token      # Token refresh
POST   /api/auth/sso-login         # Microsoft Azure SSO
POST   /api/auth/logout            # User logout
```

### User Management
```http
GET    /api/user/list              # List all users (Admin)
POST   /api/user/create            # Create new user (Admin)
PUT    /api/user/update/{id}       # Update user (Admin)
DELETE /api/user/delete/{id}       # Delete user (Admin)
GET    /api/user/profile           # Get user profile
PUT    /api/user/profile           # Update profile
POST   /api/user/change-password   # Change password
POST   /api/user/forgot-password   # Password reset request
POST   /api/user/reset-password    # Password reset
```

### Course Management
```http
GET    /api/courses/list           # List all courses
GET    /api/courses/{id}           # Get course details
POST   /api/courses/create         # Create course (Admin)
PUT    /api/courses/update/{id}    # Update course (Admin)
DELETE /api/courses/delete/{id}    # Delete course (Admin)
POST   /api/courses/generate-description  # AI description generation
```

### Student Operations
```http
GET    /api/student/my-courses     # Get enrolled courses
POST   /api/student/enroll         # Enroll in course
POST   /api/student/drop           # Drop from course
GET    /api/student/transcript     # Academic transcript
GET    /api/student/conflicts      # Schedule conflict check
GET    /api/student/available-courses  # Available courses for enrollment
```

### Advisor Operations
```http
GET    /api/advisor/students       # Get assigned students
GET    /api/advisor/pending-approvals  # Pending course approvals
POST   /api/advisor/approve-course # Approve student enrollment
POST   /api/advisor/reject-course  # Reject student enrollment
GET    /api/advisor/student-schedule/{id}  # Student schedule
```

### Department & Semester Management
```http
GET    /api/department/list        # List departments
POST   /api/department/create      # Create department (Admin)
PUT    /api/department/update/{id} # Update department (Admin)
GET    /api/semester/list          # List semesters
POST   /api/semester/create        # Create semester (Admin)
GET    /api/semester/active        # Get active semester
```

### Background Jobs & Monitoring
```http
GET    /api/dashboard/stats        # System statistics
GET    /hangfire                   # Hangfire dashboard
POST   /api/test/email-exists      # Real-time email validation
```

### API Response Format
All API endpoints return standardized JSON responses:

```json
// Success Response
{
    "success": true,
    "data": { ... },
    "message": "Operation completed successfully"
}

// Error Response
{
    "success": false,
    "error": {
        "code": "ERROR_CODE",
        "message": "Human-readable error message",
        "details": { ... }
    }
}
```

---

## 🎨 Frontend Architecture

### MVC Pattern Implementation

#### Controllers
```
OgrenciPortali/Controllers/
├── BaseController.cs          # Common functionality
├── HomeController.cs          # Dashboard and home page
├── UserController.cs          # User management
├── StudentController.cs       # Student operations
├── AdvisorController.cs       # Advisor operations
├── CoursesController.cs       # Course management
├── DepartmentController.cs    # Department management
├── SemesterController.cs      # Semester management
├── OfferedCoursesController.cs # Course offerings
└── ApiProxyController.cs      # API proxy for AJAX
```

#### Views Structure
```
OgrenciPortali/Views/
├── Shared/
│   ├── _Layout.cshtml         # Main layout template
│   ├── _StudentSchedule.cshtml # Schedule component
│   └── Error.cshtml           # Error pages
├── Home/
│   └── Index.cshtml           # Dashboard
├── User/
│   ├── Login.cshtml           # Login page
│   ├── List.cshtml            # User management
│   └── ResetPassword.cshtml   # Password reset
├── Student/
│   ├── MyCourses.cshtml       # Enrolled courses
│   ├── Enroll.cshtml          # Course enrollment
│   └── Transcript.cshtml      # Academic transcript
└── [Other role-specific views]
```

### Frontend Technologies

#### 1. **Responsive Design**
```css
/* CSS Custom Properties for Theme */
:root {
    --color-primary: hsl(240, 100%, 50%);
    --color-success: hsl(120, 100%, 40%);
    --color-warning: hsl(46, 100%, 61%);
    --color-danger: hsl(0, 100%, 50%);
}

/* Responsive Breakpoints */
@media (min-width: 32rem) { /* Mobile */ }
@media (min-width: 48rem) { /* Tablet */ }
@media (min-width: 64rem) { /* Desktop */ }
@media (min-width: 80rem) { /* Large Desktop */ }
```

#### 2. **JavaScript Architecture**
```javascript
// Main Application (main.js)
const App = {
    init() {
        this.bindEvents();
        this.initializeModals();
        this.setupAjaxDefaults();
    },
    
    bindEvents() {
        // Event delegation for dynamic content
    },
    
    api: {
        call(endpoint, method, data) {
            // Centralized API communication
        }
    }
};

// Utility Functions (util.js)
const Utils = {
    showModal(modalId) { /* Modal management */ },
    validateForm(formElement) { /* Form validation */ },
    showNotification(message, type) { /* User feedback */ }
};
```

#### 3. **AJAX Integration**
```javascript
// Modern Fetch API Usage
async function loadCourses() {
    try {
        const response = await fetch('/api/courses/list', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) throw new Error('Failed to load courses');
        
        const data = await response.json();
        renderCourses(data);
    } catch (error) {
        Utils.showNotification('Error loading courses', 'error');
    }
}
```

### UI/UX Features

#### 1. **Real-time Validation**
- Instant email existence checking via Redis
- Dynamic form validation with visual feedback
- AJAX-powered course conflict detection

#### 2. **Modal-Based Interactions**
- Non-disruptive user experience
- Dynamic content loading
- Form submission without page refresh

#### 3. **Responsive Schedule Display**
- Grid-based weekly schedule layout
- Color-coded course information
- Mobile-optimized time slots

#### 4. **Modern Design System**
- Bootstrap 5.3.7 components
- Custom CSS properties for theming
- Consistent typography and spacing
- Accessibility-compliant markup

---

## 🔄 Data Flow

### Frontend to Backend Communication

#### 1. **Authentication Flow**
```
User Login → Frontend Controller → API Authentication → JWT Generation → Frontend Storage
     ↓
User Action → AJAX Call with JWT → API Authorization → Business Logic → Database → Response
```

#### 2. **Course Enrollment Process**
```
1. Student Request
   └── Frontend: Enroll.cshtml
       └── AJAX: GET /api/student/available-courses
           └── API: StudentController.GetAvailableCourses()
               └── Database Query: Available courses with conflict check

2. Course Selection
   └── Frontend: Course selection with validation
       └── AJAX: POST /api/student/enroll
           └── API: StudentController.EnrollInCourse()
               └── Business Logic: Quota check, conflict detection
                   └── Database: Insert StudentCourse record

3. Advisor Approval
   └── Frontend: Advisor dashboard
       └── AJAX: GET /api/advisor/pending-approvals
           └── API: AdvisorController.GetPendingApprovals()
               └── Database Query: Pending student enrollments

4. Approval Process
   └── Frontend: Approve/Reject actions
       └── AJAX: POST /api/advisor/approve-course
           └── API: AdvisorController.ApproveCourse()
               └── Database Update: Approval status change
                   └── Background Job: Email notification
```

#### 3. **Real-time Email Validation**
```
Frontend Input → Redis Cache Check → Instant Visual Feedback
      ↑                                          ↓
Background Job ← Database Sync ← Redis Update ←─┘
(Every 15 min)
```

### Data Transformation Pipeline

#### 1. **Entity to DTO Mapping**
```csharp
// AutoMapper Configuration
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Users, UserDTO>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => ((Roles)src.Role).ToString()));
            
        CreateMap<Courses, ListCoursesDTO>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Departments.Name));
    }
}
```

#### 2. **API Response Standardization**
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public object Error { get; set; }
}
```

### Caching Strategy

#### 1. **Redis Cache Implementation**
```csharp
public static class RedisService
{
    private static IDatabase GetDatabase() => Connection.GetDatabase();
    
    public static async Task<bool> EmailExistsAsync(string email)
    {
        var db = GetDatabase();
        return await db.SetContainsAsync("user_emails", email);
    }
    
    public static async Task CacheAllEmailsAsync()
    {
        // Background job to refresh email cache every 15 minutes
    }
}
```

#### 2. **Cache Invalidation Strategy**
- **Time-based**: 15-minute refresh cycle for email cache
- **Event-based**: Cache updates on user creation/modification
- **Manual**: Admin-triggered cache refresh via Hangfire dashboard

---

## ⚡ Advanced Features

### 1. **AI-Powered Course Description Generation**

#### Integration with Deepseek API
```csharp
public async Task<string> GenerateCourseDescriptionAsync(string courseName, string courseCode)
{
    var request = new AiRequestDto
    {
        model = "deepseek-chat",
        messages = new List<Message>
        {
            new Message
            {
                role = "user",
                content = $"Generate a comprehensive course description for: {courseName} ({courseCode})"
            }
        }
    };
    
    var response = await httpClient.PostAsJsonAsync(AppSettings.DeepseekApiUrl, request);
    var aiResponse = await response.Content.ReadAsAsync<AiResponseDto>();
    
    return aiResponse.choices.FirstOrDefault()?.message?.content;
}
```

#### Features
- **Automatic Generation**: AI creates detailed course descriptions
- **Context-Aware**: Uses course name and code for relevant content
- **Manual Override**: Instructors can edit AI-generated content
- **Background Processing**: Hangfire jobs for bulk generation

### 2. **Background Job Processing with Hangfire**

#### Job Configuration
```csharp
public void Configuration(IAppBuilder app)
{
    // Hangfire SQL Server storage
    GlobalConfiguration.Configuration
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            SchemaName = "hangfire"
        });
    
    app.UseHangfireServer();
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
    });
}
```

#### Scheduled Jobs
```csharp
// Recurring Jobs
RecurringJob.AddOrUpdate(
    "cache-user-addresses",
    () => CheckUserData.CacheUserAddressesAsync(),
    "*/15 * * * *"  // Every 15 minutes
);

// Background Jobs
BackgroundJob.Enqueue(() => SendEmailNotification(userId, message));
```

#### Job Types
- **Email Cache Refresh**: Every 15 minutes
- **Email Notifications**: Course approval/rejection alerts
- **Data Cleanup**: Periodic soft-deleted record cleanup
- **Report Generation**: Scheduled academic reports

### 3. **Real-time Email Validation**

#### Implementation
```javascript
// Frontend: Real-time validation
$('#email').on('input', debounce(async function() {
    const email = $(this).val();
    if (isValidEmail(email)) {
        const exists = await checkEmailExists(email);
        showValidationFeedback(exists ? 'taken' : 'available');
    }
}, 300));

async function checkEmailExists(email) {
    const response = await fetch('/api/test/email-exists', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Email: email })
    });
    const result = await response.json();
    return result.exists;
}
```

#### Backend Implementation
```csharp
[HttpPost]
[Route("email-exists")]
public async Task<IHttpActionResult> CheckEmailExists([FromBody] TestEmailDto request)
{
    var exists = await RedisService.EmailExistsAsync(request.Email);
    return Ok(new { exists });
}
```

### 4. **Microsoft Azure SSO Integration**

#### Azure AD Configuration
```csharp
public async Task<IHttpActionResult> SsoLogin([FromBody] SsoLoginDto request)
{
    try
    {
        var credential = new ClientSecretCredential(
            AppSettings.AzureTenantId,
            AppSettings.AzureClientId,
            AppSettings.AzureClientSecret);
            
        var graphClient = new GraphServiceClient(credential);
        var user = await graphClient.Me.GetAsync();
        
        // User provisioning and JWT generation
        var claims = await CreateUserClaimsAsync(user);
        var token = TokenManager.GenerateAccessToken(claims);
        
        return Ok(new { token, user = userDto });
    }
    catch (Exception ex)
    {
        return BadRequest("SSO authentication failed");
    }
}
```

### 5. **Advanced Conflict Detection**

#### Schedule Conflict Algorithm
```csharp
public async Task<List<ConflictDto>> CheckScheduleConflictsAsync(Guid studentId, Guid offeredCourseId)
{
    var studentCourses = await GetStudentEnrolledCoursesAsync(studentId);
    var newCourse = await GetOfferedCourseAsync(offeredCourseId);
    
    var conflicts = studentCourses
        .Where(sc => sc.DayOfWeek == newCourse.DayOfWeek)
        .Where(sc => TimeSlotOverlaps(sc.StartTime, sc.EndTime, newCourse.StartTime, newCourse.EndTime))
        .Select(sc => new ConflictDto
        {
            ConflictingCourse = sc.CourseName,
            TimeSlot = $"{sc.StartTime:HH:mm} - {sc.EndTime:HH:mm}",
            DayOfWeek = ((DaysOfWeek)sc.DayOfWeek).ToString()
        }).ToList();
        
    return conflicts;
}
```

---

## 🚀 Deployment & Configuration

### Environment Configuration

#### 1. **Environment Variables (.env)**
```bash
# API Configuration
JWT_MASTER_KEY="your_256_bit_secret_key"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"
API_BASE_ADDRESS="https://api.yourdomain.com/"

# Database Configuration
SQL_CONNECTION_STRING="Server=server;Database=db;User=user;Password=pass;"
DB_SERVER="your_db_server"
DB_NAME="OgrenciPortalDB"
DB_USER="api_user"

# Redis Configuration
REDIS_HOST="localhost:6379"
REDIS_USER="default"
REDIS_PASS="your_redis_password"

# Email Configuration
SMTP_HOST="mail.yourdomain.com"
SMTP_PORT=587
SMTP_USER="noreply@yourdomain.com"
SMTP_PASS="smtp_password"

# External Services
DEEPSEEK_API_KEY="your_deepseek_api_key"
AZURE_TENANT_ID="your_azure_tenant_id"
AZURE_CLIENT_ID="your_azure_app_id"
AZURE_CLIENT_SECRET="your_azure_secret"

# Application Settings
ACCESS_TOKEN_EXPIRATION_MINUTES=60
REFRESH_TOKEN_EXPIRATION_DAYS=7
```

### IIS Deployment Configuration

#### 1. **Web.config Transformations**
```xml
<!-- Web.Release.config -->
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=prod-server;Database=OgrenciPortal;..."
         xdt:Transform="SetAttributes" xdt:Locator="Match(name)"/>
  </connectionStrings>
  
  <system.web>
    <compilation xdt:Transform="RemoveAttributes(debug)" />
    <customErrors mode="RemoteOnly" xdt:Transform="Replace" />
  </system.web>
</configuration>
```

#### 2. **Application Pool Settings**
- **.NET Framework Version**: 4.7.2+
- **Pipeline Mode**: Integrated
- **Identity**: ApplicationPoolIdentity
- **Load User Profile**: True
- **Idle Timeout**: 20 minutes

### Database Deployment

#### 1. **Entity Framework Migrations**
```csharp
// Package Manager Console Commands
Enable-Migrations
Add-Migration InitialCreate
Update-Database
```

#### 2. **SQL Server Configuration**
```sql
-- Database Creation
CREATE DATABASE OgrenciPortalDB
COLLATE Turkish_CI_AS;

-- User Creation
CREATE LOGIN api_user WITH PASSWORD = 'SecurePassword123!';
CREATE USER api_user FOR LOGIN api_user;
ALTER ROLE db_datareader ADD MEMBER api_user;
ALTER ROLE db_datawriter ADD MEMBER api_user;
ALTER ROLE db_ddladmin ADD MEMBER api_user;

-- Hangfire Schema
CREATE SCHEMA hangfire;
```

### Redis Configuration

#### 1. **Redis Setup**
```bash
# Redis Installation (Ubuntu/Debian)
sudo apt update
sudo apt install redis-server

# Configuration
sudo nano /etc/redis/redis.conf
# Set: requirepass your_secure_password
# Set: bind 127.0.0.1 ::1

# Service Management
sudo systemctl restart redis-server
sudo systemctl enable redis-server
```

### Monitoring & Logging

#### 1. **log4net Configuration**
```xml
<log4net>
  <appender name="FileAppender" type="log4net.Appender.FileAppender">
    <file value="logs/application.log" />
    <appendToFile value="true" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" />
    </layout>
  </appender>
  
  <root>
    <level value="INFO" />
    <appender-ref ref="FileAppender" />
  </root>
</log4net>
```

#### 2. **Health Check Endpoints**
```csharp
[HttpGet]
[Route("health")]
[AllowAnonymous]
public IHttpActionResult HealthCheck()
{
    var health = new
    {
        Status = "Healthy",
        Timestamp = DateTime.UtcNow,
        Database = CheckDatabaseConnection(),
        Redis = CheckRedisConnection(),
        Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
    };
    
    return Ok(health);
}
```

---

## 📈 Scaling Considerations

### Performance Optimization

#### 1. **Database Optimization**
```sql
-- Indexing Strategy
CREATE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
CREATE INDEX IX_StudentCourses_Student ON StudentCourses(StudentId) INCLUDE (ApprovalStatus);
CREATE INDEX IX_OfferedCourses_Semester ON OfferedCourses(SemesterId) WHERE IsDeleted = 0;

-- Query Optimization
-- Use Entity Framework Include() for eager loading
-- Implement repository pattern for complex queries
-- Consider stored procedures for heavy operations
```

#### 2. **Caching Strategy**
```csharp
// Multi-level Caching
public async Task<List<CourseDto>> GetCoursesAsync()
{
    // L1: Memory Cache (Application level)
    var cached = MemoryCache.Default.Get("courses") as List<CourseDto>;
    if (cached != null) return cached;
    
    // L2: Redis Cache (Distributed)
    var redisData = await RedisService.GetAsync<List<CourseDto>>("courses");
    if (redisData != null)
    {
        MemoryCache.Default.Add("courses", redisData, DateTimeOffset.Now.AddMinutes(5));
        return redisData;
    }
    
    // L3: Database
    var data = await _db.Courses.ToListAsync();
    await RedisService.SetAsync("courses", data, TimeSpan.FromMinutes(30));
    return data;
}
```

### Horizontal Scaling

#### 1. **Load Balancer Configuration**
```
Internet
    │
    ▼
Load Balancer (IIS ARR / nginx)
    │
    ├── Web Server 1 (Frontend)
    ├── Web Server 2 (Frontend)
    └── Web Server 3 (Frontend)
                │
                ▼
    ┌─── API Server 1 ────┐
    ├─── API Server 2 ────┤
    └─── API Server 3 ────┘
                │
                ▼
        Database Cluster
        (Primary/Secondary)
```

#### 2. **Microservices Migration Path**
```
Monolithic API → Modular Services
    │
    ├── User Service (Authentication, User Management)
    ├── Course Service (Course Management, Catalog)
    ├── Enrollment Service (Student Enrollments)
    ├── Notification Service (Email, SMS)
    └── Report Service (Analytics, Transcripts)
```

### Cloud Deployment Options

#### 1. **Azure App Service**
```yaml
# Azure DevOps Pipeline (azure-pipelines.yml)
trigger:
  branches:
    include:
    - master

pool:
  vmImage: 'windows-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Restore'
  inputs:
    command: 'restore'
    projects: '**/*.csproj'

- task: DotNetCoreCLI@2
  displayName: 'Build'
  inputs:
    command: 'build'
    projects: '**/*.csproj'

- task: AzureWebApp@1
  displayName: 'Deploy to Azure'
  inputs:
    azureSubscription: 'Azure Connection'
    appName: 'ogrenci-platform'
    package: '$(System.DefaultWorkingDirectory)/**/*.zip'
```

#### 2. **Containerization with Docker**
```dockerfile
# Dockerfile for API
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
WORKDIR /inetpub/wwwroot
COPY . .
EXPOSE 80
```

### Monitoring & Analytics

#### 1. **Application Insights Integration**
```csharp
// Telemetry Configuration
public void Application_Start()
{
    TelemetryConfiguration.Active.InstrumentationKey = "your-app-insights-key";
    TelemetryConfiguration.Active.TelemetryInitializers.Add(new MyTelemetryInitializer());
}
```

#### 2. **Performance Metrics**
- **Response Time**: < 200ms for API calls
- **Throughput**: 1000+ concurrent users
- **Availability**: 99.9% uptime SLA
- **Error Rate**: < 0.1% of requests

---

## 📋 Summary

### Turkish Summary (Türkçe Özet)

**OgrenciPlatform**, modern .NET teknolojileri kullanılarak geliştirilmiş kapsamlı bir öğrenci yönetim sistemidir. Sistem, katmanlı mimari prensiplerine uygun olarak tasarlanmış ve aşağıdaki temel bileşenlerden oluşmaktadır:

#### Teknik Özellikler
- **Frontend**: ASP.NET MVC 5 ile responsive web arayüzü
- **Backend**: ASP.NET Web API 2 ile RESTful servisler
- **Veritabanı**: Entity Framework 6.5.1 ile SQL Server entegrasyonu
- **Kimlik Doğrulama**: JWT tabanlı stateless authentication
- **Önbellekleme**: Redis ile gerçek zamanlı performans optimizasyonu
- **Arka Plan İşleri**: Hangfire ile otomatik görev yönetimi
- **AI Entegrasyonu**: Deepseek API ile ders açıklaması üretimi
- **SSO**: Microsoft Azure Active Directory entegrasyonu

#### Fonksiyonel Özellikler
- **Rol Tabanlı Erişim**: Admin, Danışman ve Öğrenci rolleri
- **Ders Yönetimi**: Ders oluşturma, düzenleme ve AI destekli açıklama üretimi
- **Kayıt Sistemi**: Otomatik çakışma kontrolü ile ders kaydı
- **Gerçek Zamanlı Validasyon**: Redis tabanlı e-posta kontrol sistemi
- **Danışman Onay Sistemi**: Öğrenci ders kayıtlarının danışman onayı
- **Transkript Yönetimi**: Detaylı akademik kayıt takibi
- **Şifre Sıfırlama**: E-posta tabanlı güvenli şifre yenileme

#### Güvenlik Özellikleri
- JWT refresh token mekanizması
- BCrypt ile şifre hashleme
- CORS ve CSRF koruması
- SQL injection ve XSS koruması
- Azure AD SSO entegrasyonu

### English Summary

**OgrenciPlatform** is a comprehensive student management system developed using modern .NET technologies. The system follows layered architecture principles and consists of the following core components:

#### Technical Features
- **Frontend**: Responsive web interface with ASP.NET MVC 5
- **Backend**: RESTful services with ASP.NET Web API 2
- **Database**: SQL Server integration with Entity Framework 6.5.1
- **Authentication**: JWT-based stateless authentication
- **Caching**: Real-time performance optimization with Redis
- **Background Jobs**: Automated task management with Hangfire
- **AI Integration**: Course description generation with Deepseek API
- **SSO**: Microsoft Azure Active Directory integration

#### Functional Features
- **Role-Based Access**: Admin, Advisor, and Student roles
- **Course Management**: Course creation, editing, and AI-powered description generation
- **Enrollment System**: Course registration with automatic conflict detection
- **Real-time Validation**: Redis-based email verification system
- **Advisor Approval System**: Advisor approval for student course registrations
- **Transcript Management**: Detailed academic record tracking
- **Password Reset**: Email-based secure password recovery

#### Security Features
- JWT refresh token mechanism
- BCrypt password hashing
- CORS and CSRF protection
- SQL injection and XSS protection
- Azure AD SSO integration

### Architecture Highlights

The system demonstrates modern software development practices including:

- **Clean Architecture**: Clear separation of concerns across layers
- **SOLID Principles**: Dependency injection with Autofac, single responsibility
- **Security-First Design**: Multiple layers of security implementation
- **Performance Optimization**: Multi-level caching and background processing
- **Scalability**: Horizontal scaling capabilities and microservices readiness
- **Maintainability**: Comprehensive logging, monitoring, and documentation

This architecture documentation provides a complete technical overview for developers, system administrators, and stakeholders to understand, deploy, and maintain the OgrenciPlatform system effectively.