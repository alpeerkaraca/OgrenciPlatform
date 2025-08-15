namespace OgrenciPortali.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ReacreatewithGuidandactivitytracking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                {
                    CourseId = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    CourseCode = c.String(nullable: false, maxLength: 10),
                    CourseName = c.String(nullable: false, maxLength: 100),
                    Credits = c.Int(nullable: false),
                    DepartmentId = c.Guid(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: false)
                .Index(t => t.DepartmentId);

            CreateTable(
                "dbo.Departments",
                c => new
                {
                    DepartmentId = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    Name = c.String(nullable: false, maxLength: 100),
                    IsActive = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.DepartmentId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    UserId = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    Name = c.String(nullable: false, maxLength: 50),
                    Surname = c.String(nullable: false, maxLength: 50),
                    Email = c.String(nullable: false, maxLength: 100),
                    Password = c.String(nullable: false, maxLength: 100),
                    Role = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    IsFirstLogin = c.Boolean(nullable: false),
                    StudentNo = c.String(),
                    DepartmentId = c.Guid(),
                    AdvisorId = c.Guid(),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.AdvisorId, cascadeDelete: false)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: false)
                .Index(t => t.Email, unique: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.AdvisorId);

            CreateTable(
                "dbo.StudentCourses",
                c => new
                {
                    StudentId = c.Guid(nullable: false),
                    OfferedCourseId = c.Guid(nullable: false),
                    ApprovalStatus = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => new { t.StudentId, t.OfferedCourseId })
                .ForeignKey("dbo.OfferedCourses", t => t.OfferedCourseId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.StudentId, cascadeDelete: false)
                .Index(t => t.StudentId)
                .Index(t => t.OfferedCourseId);

            CreateTable(
                "dbo.OfferedCourses",
                c => new
                {
                    Id = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    CourseId = c.Guid(nullable: false),
                    SemesterId = c.Guid(nullable: false),
                    TeacherId = c.Guid(nullable: false),
                    Quota = c.Int(nullable: false),
                    DayOfWeek = c.Int(nullable: false),
                    StartTime = c.Time(nullable: false, precision: 7),
                    EndTime = c.Time(nullable: false, precision: 7),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: false)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.TeacherId, cascadeDelete: false)
                .Index(t => t.CourseId)
                .Index(t => t.SemesterId)
                .Index(t => t.TeacherId);

            CreateTable(
                "dbo.Semesters",
                c => new
                {
                    SemesterId = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    SemesterName = c.String(nullable: false, maxLength: 50),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    CreatedBy = c.String(maxLength: 100),
                    UpdatedAt = c.DateTime(nullable: false),
                    UpdatedBy = c.String(maxLength: 100),
                    IsDeleted = c.Boolean(nullable: false),
                    DeletedAt = c.DateTime(),
                    DeletedBy = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.SemesterId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.StudentCourses", "StudentId", "dbo.Users");
            DropForeignKey("dbo.OfferedCourses", "TeacherId", "dbo.Users");
            DropForeignKey("dbo.OfferedCourses", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.StudentCourses", "OfferedCourseId", "dbo.OfferedCourses");
            DropForeignKey("dbo.OfferedCourses", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Users", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Users", "AdvisorId", "dbo.Users");
            DropForeignKey("dbo.Courses", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.OfferedCourses", new[] { "TeacherId" });
            DropIndex("dbo.OfferedCourses", new[] { "SemesterId" });
            DropIndex("dbo.OfferedCourses", new[] { "CourseId" });
            DropIndex("dbo.StudentCourses", new[] { "OfferedCourseId" });
            DropIndex("dbo.StudentCourses", new[] { "StudentId" });
            DropIndex("dbo.Users", new[] { "AdvisorId" });
            DropIndex("dbo.Users", new[] { "DepartmentId" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Courses", new[] { "DepartmentId" });
            DropTable("dbo.Semesters");
            DropTable("dbo.OfferedCourses");
            DropTable("dbo.StudentCourses");
            DropTable("dbo.Users");
            DropTable("dbo.Departments");
            DropTable("dbo.Courses");
        }
    }
}