# 📚 Shared Library - Common Components

The shared library containing common data structures, enumerations, constants, and utilities used across the Student Platform solution. This library ensures consistency and reusability between the API and MVC projects.

## 🎯 Purpose & Scope

The Shared library serves as the **common foundation** providing:

- **📋 Data Transfer Objects (DTOs)**: Standardized data contracts
- **🔢 Enumerations**: System-wide enumerated values
- **📌 Constants**: Shared constant values and configurations
- **🔧 Utilities**: Common helper functions and extensions
- **📊 Models**: Base classes and interfaces
- **🌐 Cross-Cutting Concerns**: Shared functionality across layers

## 🏗️ Library Structure

```
Shared/
├── DTO/                    # Data Transfer Objects
│   ├── AdvisorDTO.cs       # Advisor-related data contracts
│   ├── BaseClass.cs        # Base entity with audit fields
│   ├── ConflictsDTO.cs     # Course conflict information
│   ├── CoursesDTO.cs       # Course-related data contracts
│   ├── DepartmentsDTO.cs   # Department data structures
│   ├── EditUserDTO.cs      # User editing contracts
│   ├── OfferedCoursesDTO.cs # Course offering data
│   ├── SemesterDTO.cs      # Semester information
│   ├── StudentDTO.cs       # Student-related contracts
│   └── UserDTO.cs          # User data contracts
│
├── Enums/                  # System Enumerations
│   └── Enums.cs           # All system enums
│
├── Constants/              # System Constants
│   └── [Future constants]  # Configuration constants
│
└── Properties/             # Assembly Information
    └── AssemblyInfo.cs    # Assembly metadata
```

## 📋 Data Transfer Objects (DTOs)

### Base Classes

#### BaseClass.cs
```csharp
public abstract class BaseClass
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; }
}
```

**Purpose**: Provides standard audit trail functionality for all entities
**Features**:
- **Creation Tracking**: Who and when created
- **Modification Tracking**: Who and when last updated  
- **Soft Delete Support**: Logical deletion with timestamp
- **Audit Trail**: Complete change history

### User-Related DTOs

#### UserDTO.cs
```csharp
public class UserDTO : BaseClass
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsFirstLogin { get; set; }
    public string StudentNo { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AdvisorId { get; set; }
    public int? StudentYear { get; set; }
}
```

#### EditUserDTO.cs
```csharp
public class EditUserDTO
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
    public string StudentNo { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? AdvisorId { get; set; }
    public int? StudentYear { get; set; }
}
```

### Academic DTOs

#### CoursesDTO.cs
```csharp
public class CoursesDTO : BaseClass
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public string Description { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
}
```

#### OfferedCoursesDTO.cs
```csharp
public class OfferedCoursesDTO : BaseClass
{
    public Guid OfferedCourseId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; }
    public int Quota { get; set; }
    public int CurrentUserCount { get; set; }
    public DaysOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Classroom { get; set; }
}
```

#### StudentDTO.cs
```csharp
public class StudentDTO : BaseClass
{
    public Guid StudentId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string StudentNo { get; set; }
    public int StudentYear { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public Guid? AdvisorId { get; set; }
    public string AdvisorName { get; set; }
    public List<EnrolledCourseDTO> EnrolledCourses { get; set; }
}

public class EnrolledCourseDTO
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public ApprovalStatus Status { get; set; }
    public string Grade { get; set; }
}
```

### Administrative DTOs

#### DepartmentsDTO.cs
```csharp
public class DepartmentsDTO : BaseClass
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string DepartmentCode { get; set; }
    public string Description { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public List<UserDTO> Faculty { get; set; }
}
```

#### SemesterDTO.cs
```csharp
public class SemesterDTO : BaseClass
{
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; }
    public string SemesterCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
}
```

### Specialized DTOs

#### ConflictsDTO.cs
```csharp
public class ConflictsDTO
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string ConflictType { get; set; }
    public string ConflictDescription { get; set; }
    public List<ConflictingCourseDTO> ConflictingCourses { get; set; }
}

public class ConflictingCourseDTO
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public DaysOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
```

#### AdvisorDTO.cs
```csharp
public class AdvisorDTO : BaseClass
{
    public Guid AdvisorId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public List<StudentDTO> AssignedStudents { get; set; }
    public List<OfferedCoursesDTO> AssignedCourses { get; set; }
    public int TotalStudents { get; set; }
    public int PendingApprovals { get; set; }
}
```

## 🔢 System Enumerations

### Enums.cs
```csharp
namespace Shared.Enums
{
    /// <summary>
    /// User roles in the system
    /// </summary>
    public enum Roles
    {
        Admin = 1,      // System Administrator
        Danışman = 2,   // Academic Advisor  
        Öğrenci = 3,    // Student
    }

    /// <summary>
    /// Course enrollment approval status
    /// </summary>
    public enum ApprovalStatus
    {
        Bekliyor = 0,    // Pending Approval
        Onaylandı = 1,   // Approved
        Reddedildi = 2,  // Rejected
    }

    /// <summary>
    /// Days of the week for course scheduling
    /// </summary>
    public enum DaysOfWeek
    {
        Pazartesi = 1,   // Monday
        Salı = 2,        // Tuesday
        Çarşamba = 3,    // Wednesday
        Perşembe = 4,    // Thursday
        Cuma = 5,        // Friday
        Cumartesi = 6,   // Saturday
        Pazar = 7,       // Sunday
    }
}
```

### Enum Usage Examples

#### Role-Based Access Control
```csharp
// Check if user is admin
if (user.Role == Roles.Admin)
{
    // Admin-specific functionality
}

// Multiple role check
if (user.Role == Roles.Admin || user.Role == Roles.Danışman)
{
    // Admin or Advisor functionality
}
```

#### Approval Status Management
```csharp
// Update enrollment status
enrollment.Status = ApprovalStatus.Onaylandı;

// Check pending approvals
var pendingCount = enrollments.Count(e => e.Status == ApprovalStatus.Bekliyor);
```

#### Schedule Management
```csharp
// Create course schedule
var courseSchedule = new OfferedCoursesDTO
{
    DayOfWeek = DaysOfWeek.Pazartesi,
    StartTime = new TimeSpan(9, 0, 0),  // 09:00
    EndTime = new TimeSpan(11, 0, 0)    // 11:00
};
```

## 📌 Constants & Configuration

### Future Constants Structure
```csharp
namespace Shared.Constants
{
    public static class SystemConstants
    {
        // Password requirements
        public const int MIN_PASSWORD_LENGTH = 8;
        public const int MAX_PASSWORD_LENGTH = 50;
        
        // Course constraints
        public const int MAX_CREDITS_PER_SEMESTER = 30;
        public const int MIN_CREDITS_PER_SEMESTER = 12;
        
        // System limits
        public const int MAX_STUDENTS_PER_ADVISOR = 50;
        public const int MAX_COURSE_CAPACITY = 200;
        
        // Time constants
        public const int SESSION_TIMEOUT_MINUTES = 30;
        public const int TOKEN_EXPIRY_MINUTES = 15;
    }

    public static class ErrorMessages
    {
        public const string INVALID_CREDENTIALS = "Invalid email or password";
        public const string ACCESS_DENIED = "Access denied";
        public const string COURSE_CAPACITY_FULL = "Course capacity is full";
        public const string SCHEDULE_CONFLICT = "Schedule conflict detected";
    }

    public static class SuccessMessages
    {
        public const string USER_CREATED = "User created successfully";
        public const string ENROLLMENT_APPROVED = "Enrollment approved";
        public const string PROFILE_UPDATED = "Profile updated successfully";
    }
}
```

## 🔧 Utility Classes & Extensions

### Future Utility Structure
```csharp
namespace Shared.Utils
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekday(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && 
                   date.DayOfWeek != DayOfWeek.Sunday;
        }
        
        public static string ToAcademicYear(this DateTime date)
        {
            var year = date.Month >= 9 ? date.Year : date.Year - 1;
            return $"{year}-{year + 1}";
        }
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            // Return localized display name for enum values
        }
    }

    public static class ValidationHelpers
    {
        public static bool IsValidStudentNumber(string studentNo)
        {
            // Validate student number format
        }
        
        public static bool IsValidEmail(string email)
        {
            // Email validation logic
        }
    }
}
```

## 📦 NuGet Dependencies

The Shared library has minimal dependencies to maintain portability:

```xml
<packages>
  <package id="Newtonsoft.Json" version="13.0.3" targetFramework="net472" />
  <package id="System.ComponentModel.Annotations" version="5.0.0" targetFramework="net472" />
</packages>
```

### Key Dependencies
- **Newtonsoft.Json**: JSON serialization for DTOs
- **System.ComponentModel.Annotations**: Data annotations for validation

## 🧪 Usage Examples

### API Project Usage
```csharp
// In API Controllers
[HttpGet]
public IHttpActionResult GetStudents()
{
    var students = _db.Users
        .Where(u => u.Role == (int)Roles.Öğrenci)
        .Select(u => new StudentDTO
        {
            StudentId = u.UserId,
            Name = u.Name,
            Surname = u.Surname,
            Email = u.Email,
            StudentNo = u.StudentNo
        })
        .ToList();
    
    return Ok(students);
}
```

### MVC Project Usage
```csharp
// In MVC Controllers
public ActionResult Students()
{
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync($"{ApiBaseUrl}/api/students");
        var json = await response.Content.ReadAsStringAsync();
        var students = JsonConvert.DeserializeObject<List<StudentDTO>>(json);
        
        return View(students);
    }
}
```

### Model Validation
```csharp
// Using data annotations
public class UserDTO : BaseClass
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    public Roles Role { get; set; }
}
```

## 🔄 Versioning & Compatibility

### Semantic Versioning
- **Major Version**: Breaking changes to DTOs or enums
- **Minor Version**: New DTOs or enum values (backward compatible)
- **Patch Version**: Bug fixes and improvements

### Backward Compatibility Guidelines
1. **Never remove** existing DTO properties
2. **Never change** enum values or meanings
3. **Add new properties** as nullable when possible
4. **Mark obsolete** properties instead of removing them
5. **Document breaking changes** in release notes

## 📚 Development Guidelines

### DTO Design Principles
1. **Immutability**: Consider making DTOs immutable where appropriate
2. **Validation**: Include data annotations for validation
3. **Documentation**: Use XML comments for all public members
4. **Serialization**: Ensure JSON serialization compatibility
5. **Inheritance**: Use BaseClass for entities requiring audit trails

### Enum Best Practices
1. **Explicit Values**: Always specify explicit integer values
2. **Naming**: Use descriptive names in the target language
3. **Documentation**: Document the purpose of each enum value
4. **Backwards Compatibility**: Never change existing enum values

### Example Documentation
```csharp
/// <summary>
/// Represents a student's enrollment in a specific course offering
/// </summary>
public class EnrollmentDTO : BaseClass
{
    /// <summary>
    /// Unique identifier for the enrollment record
    /// </summary>
    public Guid EnrollmentId { get; set; }
    
    /// <summary>
    /// Reference to the student (foreign key)
    /// </summary>
    public Guid StudentId { get; set; }
    
    /// <summary>
    /// Reference to the offered course (foreign key)
    /// </summary>
    public Guid OfferedCourseId { get; set; }
    
    /// <summary>
    /// Current approval status of the enrollment
    /// </summary>
    public ApprovalStatus Status { get; set; }
    
    /// <summary>
    /// Final grade received for the course (null if not yet graded)
    /// </summary>
    public string Grade { get; set; }
}
```

## 🧪 Testing Considerations

### Unit Testing DTOs
```csharp
[TestFixture]
public class StudentDTOTests
{
    [Test]
    public void StudentDTO_ShouldInheritFromBaseClass()
    {
        // Arrange & Act
        var student = new StudentDTO();
        
        // Assert
        Assert.IsInstanceOf<BaseClass>(student);
        Assert.IsNotNull(student.CreatedAt);
        Assert.IsNotNull(student.UpdatedAt);
    }
    
    [Test]
    public void StudentDTO_ShouldSerializeToJson()
    {
        // Arrange
        var student = new StudentDTO
        {
            StudentId = Guid.NewGuid(),
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com"
        };
        
        // Act
        var json = JsonConvert.SerializeObject(student);
        var deserialized = JsonConvert.DeserializeObject<StudentDTO>(json);
        
        // Assert
        Assert.AreEqual(student.Name, deserialized.Name);
        Assert.AreEqual(student.Email, deserialized.Email);
    }
}
```

## 📈 Future Enhancements

### Planned Additions
1. **Validation Attributes**: Custom validation attributes
2. **Mapping Profiles**: AutoMapper profiles for DTO conversions
3. **Localization**: Multi-language support for enums and messages
4. **Extension Methods**: Common utility methods
5. **Constants**: System-wide configuration constants

### Integration Possibilities
- **FluentValidation**: Advanced validation rules
- **AutoMapper**: Automated object mapping
- **System.Text.Json**: Alternative JSON serialization
- **DataAnnotations**: Enhanced validation attributes

---

**🔗 Related Projects:**
- [Backend API (OgrenciPortalApi)](../OgrenciPortalApi/README.md)
- [Frontend MVC (OgrenciPortali)](../OgrenciPortali/README.md)
- [Main Documentation](../README.md)