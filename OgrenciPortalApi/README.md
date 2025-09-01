# 🌐 OgrenciPortalApi - Web API Backend

The robust, secure backend API powering the Student Platform system. Built with ASP.NET Web API 2 and Entity Framework 6, providing comprehensive RESTful services for academic management.

## 🎯 Purpose & Responsibilities

This Web API project serves as the **core backend system** handling:

- **🔐 Authentication & Authorization**: JWT-based security with role management
- **👥 User Management**: Complete CRUD operations for students, advisors, and administrators
- **📚 Academic Operations**: Course management, enrollment processing, and academic tracking
- **🏢 Administrative Functions**: Department, semester, and institutional data management
- **📊 Business Logic**: All core academic rules and validation logic
- **💾 Data Access Layer**: Entity Framework-based database operations with audit trails
- **📧 Communication Services**: Email notifications and password reset functionality

## 🚀 Technology Stack

### Core Framework
- **ASP.NET Web API 2** (.NET Framework 4.7.2)
- **Entity Framework 6.5.1** (Database-First approach)
- **OWIN 4.2.3** (Authentication middleware)

### Security & Authentication
- **System.IdentityModel.Tokens.Jwt 8.13.0** - JWT token management
- **Microsoft.Owin.Security.Jwt 4.2.3** - OWIN JWT middleware
- **BCrypt.Net-Next 4.0.3** - Password hashing and validation
- **Microsoft.AspNet.Cors 5.3.0** - Cross-origin resource sharing

### Data & Caching
- **Microsoft SQL Server** - Primary database
- **Redis Stack 2.8.58** - Distributed caching and session storage
- **NRedisStack 1.0.0** - Redis client for .NET

### Communication & Documentation
- **MailKit 4.13.0** - Email services (password reset, notifications)
- **Swashbuckle 5.6.0** - API documentation and Swagger UI
- **log4net 3.1.0** - Comprehensive logging system

### Dependency Injection & Configuration
- **Unity Container 5.11.8** - IoC container for dependency injection
- **DotNetEnv 3.1.1** - Environment variable management

## 🛡️ Security Implementation

### JWT Authentication Flow
```csharp
// Startup.cs - OWIN Configuration
public void ConfigureAuth(IAppBuilder app)
{
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secret),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    
    app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
    {
        AuthenticationMode = AuthenticationMode.Active,
        TokenValidationParameters = tokenValidationParameters
    });
}
```

### Authorization Attributes
- **`[Authorize(Roles = "Admin")]`** - Administrator-only endpoints
- **`[Authorize(Roles = "Danışman")]`** - Academic advisor access
- **`[Authorize(Roles = "Öğrenci")]`** - Student-specific operations

### Security Features
- **🔐 Password Hashing**: BCrypt with salt rounds
- **🔄 Refresh Tokens**: Secure session management
- **⏰ Token Expiration**: Configurable access token lifetime
- **📝 Audit Logging**: Complete user activity tracking
- **🛡️ Input Validation**: Comprehensive model validation
- **🚫 SQL Injection Protection**: Parameterized queries via EF

## 📋 API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | User login with credentials | ❌ |
| POST | `/api/auth/refresh` | Refresh access token | ✅ |
| POST | `/api/auth/logout` | Logout and revoke tokens | ✅ |
| POST | `/api/auth/forgot-password` | Password reset request | ❌ |
| POST | `/api/auth/reset-password` | Reset password with token | ❌ |

### User Management
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/user/profile` | Get current user profile | All |
| PUT | `/api/user/profile` | Update user profile | All |
| POST | `/api/user/change-password` | Change password | All |
| GET | `/api/user` | List all users | Admin |
| POST | `/api/user` | Create new user | Admin |
| PUT | `/api/user/{id}` | Update user | Admin |
| DELETE | `/api/user/{id}` | Delete user | Admin |

### Student Operations
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/student/my-courses` | Get enrolled courses | Student |
| GET | `/api/student/enroll` | Get available courses | Student |
| POST | `/api/student/enroll` | Enroll in course | Student |
| DELETE | `/api/student/enroll/{id}` | Withdraw from course | Student |
| GET | `/api/student/transcript` | Get academic transcript | Student, Advisor |

### Course Management
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/course` | List all courses | All |
| POST | `/api/course` | Create new course | Admin |
| PUT | `/api/course/{id}` | Update course | Admin |
| DELETE | `/api/course/{id}` | Delete course | Admin |
| GET | `/api/course/offered` | Get offered courses | All |
| POST | `/api/course/offered` | Create course offering | Admin |

### Administrative
| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/department` | List departments | All |
| POST | `/api/department` | Create department | Admin |
| GET | `/api/semester` | List semesters | All |
| POST | `/api/semester` | Create semester | Admin |
| GET | `/api/dashboard/stats` | Get dashboard statistics | Admin, Advisor |

## ⚙️ Configuration

### Environment Variables (.env)
```bash
# JWT Configuration
JWT_MASTER_KEY="your_256_bit_secret_key_here"
JWT_ISSUER="https://yourdomain.com"
JWT_AUDIENCE="https://yourdomain.com"
ACCESS_TOKEN_EXPIRATION_MINUTES=15
REFRESH_TOKEN_EXPIRATION_DAYS=7

# Database Configuration  
SQL_SERVER="your_server_name"
SQL_DATABASE="OgrenciPlatformDB"
SQL_USER="your_username"
SQL_PASSWORD="your_password"

# Email Configuration
SMTP_HOST="smtp.gmail.com"
SMTP_PORT=587
SMTP_USER="your-email@domain.com"
SMTP_PASS="your-app-password"

# Redis Configuration (Optional)
REDIS_CONNECTION="localhost:6379"
REDIS_ENABLED=true

# Logging Configuration
LOG_LEVEL="INFO"
LOG_FILE_PATH="./logs/api.log"
```

## 🚀 Getting Started

### Prerequisites
- .NET Framework 4.7.2+
- Microsoft SQL Server 2016+
- Visual Studio 2019+ or VS Code
- Redis Server (optional)

### Installation Steps

1. **Clone and Navigate**
```bash
git clone https://github.com/alpeerkaraca/OgrenciPlatform.git
cd OgrenciPlatform/OgrenciPortalApi
```

2. **Configure Environment**
```bash
# Copy environment template
copy .env.example .env

# Edit .env with your configuration
notepad .env
```

3. **Database Setup**
```bash
# Update connection string in Web.config
# Run Entity Framework migrations (if needed)
Update-Database
```

4. **Run the Application**
```bash
# Using Visual Studio: F5 or Ctrl+F5
# API available at: https://localhost:44301/
# Swagger UI: https://localhost:44301/swagger
```

## 🧪 Testing

### Manual Testing with Swagger
1. Navigate to `https://localhost:44301/swagger`
2. Authorize using JWT token
3. Test endpoints interactively

### Example API Calls
```bash
# Login
curl -X POST "https://localhost:44301/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password123"}'

# Get Profile (with token)
curl -X GET "https://localhost:44301/api/user/profile" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🔧 Development Guidelines

### Security Best Practices
- ✅ Always validate input parameters
- ✅ Use parameterized queries (EF handles this)
- ✅ Implement proper error handling without exposing sensitive data
- ✅ Log security events and failed attempts
- ✅ Use HTTPS in production
- ✅ Rotate JWT secrets regularly

### Database Best Practices
- ✅ Use Entity Framework for all database operations
- ✅ Implement audit fields (Created/Updated/Deleted timestamps)
- ✅ Use soft deletes with `IsDeleted` flag
- ✅ Maintain referential integrity
- ✅ Regular database backups

## 🐛 Troubleshooting

### Common Issues

**1. JWT Token Issues**
- Check JWT_MASTER_KEY length (minimum 32 characters)
- Validate issuer and audience settings
- Check token expiration settings

**2. Database Connection Issues**
- Check server name and database name
- Verify SQL Server authentication mode
- Test connection using SQL Server Management Studio

**3. Email Service Issues**
- Verify SMTP host and port settings
- Check email credentials
- Test SMTP connection separately

**4. Redis Connection Issues**
- Verify Redis server is running: `redis-cli ping`
- Check Redis connection string
- Disable Redis temporarily if not needed

## 📚 Additional Resources

- [ASP.NET Web API Documentation](https://docs.microsoft.com/en-us/aspnet/web-api/)
- [Entity Framework 6 Documentation](https://docs.microsoft.com/en-us/ef/ef6/)
- [JWT.IO - JWT Debugger](https://jwt.io/)
- [Swagger/OpenAPI Specification](https://swagger.io/specification/)
- [log4net Documentation](https://logging.apache.org/log4net/)

---

**🔗 Related Projects:**
- [Frontend (OgrenciPortali)](../OgrenciPortali/README.md)
- [Shared Library](../Shared/README.md)
- [Main Documentation](../README.md)