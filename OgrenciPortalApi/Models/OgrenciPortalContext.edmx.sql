
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/10/2025 17:11:50
-- Generated from EDMX file: C:\Users\alpeerkaraca\Desktop\Projeler\OgrenciPlatform\OgrenciPortalApi\Models\OgrenciPortalContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ogrenci_portal];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_Courses_dbo_Departments_DepartmentId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Courses] DROP CONSTRAINT [FK_dbo_Courses_dbo_Departments_DepartmentId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_OfferedCourses_dbo_Courses_CourseId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OfferedCourses] DROP CONSTRAINT [FK_dbo_OfferedCourses_dbo_Courses_CourseId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_OfferedCourses_dbo_Semesters_SemesterId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OfferedCourses] DROP CONSTRAINT [FK_dbo_OfferedCourses_dbo_Semesters_SemesterId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_OfferedCourses_dbo_Users_TeacherId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OfferedCourses] DROP CONSTRAINT [FK_dbo_OfferedCourses_dbo_Users_TeacherId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_StudentCourses_dbo_OfferedCourses_OfferedCourseId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentCourses] DROP CONSTRAINT [FK_dbo_StudentCourses_dbo_OfferedCourses_OfferedCourseId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_StudentCourses_dbo_Users_StudentId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentCourses] DROP CONSTRAINT [FK_dbo_StudentCourses_dbo_Users_StudentId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Users_dbo_Departments_DepartmentId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_dbo_Users_dbo_Departments_DepartmentId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Users_dbo_Users_AdvisorId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_dbo_Users_dbo_Users_AdvisorId];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Courses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Courses];
GO
IF OBJECT_ID(N'[dbo].[Departments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departments];
GO
IF OBJECT_ID(N'[dbo].[OfferedCourses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OfferedCourses];
GO
IF OBJECT_ID(N'[dbo].[Semesters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Semesters];
GO
IF OBJECT_ID(N'[dbo].[StudentCourses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudentCourses];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Courses'
CREATE TABLE [dbo].[Courses] (
    [CourseId] uniqueidentifier  NOT NULL,
    [CourseCode] nvarchar(10)  NOT NULL,
    [CourseName] nvarchar(100)  NOT NULL,
    [Credits] int  NOT NULL,
    [DepartmentId] uniqueidentifier  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL,
    [Description] nvarchar(512)  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [DepartmentId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL,
    [DepartmentCode] nvarchar(10)  NOT NULL,
    [DepartmentIdGen] nvarchar(10)  NULL
);
GO

-- Creating table 'OfferedCourses'
CREATE TABLE [dbo].[OfferedCourses] (
    [Id] uniqueidentifier  NOT NULL,
    [CourseId] uniqueidentifier  NOT NULL,
    [SemesterId] uniqueidentifier  NOT NULL,
    [TeacherId] uniqueidentifier  NOT NULL,
    [Quota] int  NOT NULL,
    [DayOfWeek] int  NOT NULL,
    [StartTime] time  NOT NULL,
    [EndTime] time  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL,
    [CurrentUserCount] int  NOT NULL,
    [CourseYear] int  NOT NULL,
    [Classroom] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'Semesters'
CREATE TABLE [dbo].[Semesters] (
    [SemesterId] uniqueidentifier  NOT NULL,
    [SemesterName] nvarchar(50)  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL
);
GO

-- Creating table 'StudentCourses'
CREATE TABLE [dbo].[StudentCourses] (
    [StudentId] uniqueidentifier  NOT NULL,
    [OfferedCourseId] uniqueidentifier  NOT NULL,
    [ApprovalStatus] int  NOT NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [UserId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Surname] nvarchar(50)  NOT NULL,
    [Email] nvarchar(100)  NOT NULL,
    [Password] nvarchar(100)  NOT NULL,
    [Role] int  NOT NULL,
    [IsActive] bit  NOT NULL,
    [IsFirstLogin] bit  NOT NULL,
    [StudentNo] nvarchar(max)  NULL,
    [DepartmentId] uniqueidentifier  NULL,
    [AdvisorId] uniqueidentifier  NULL,
    [CreatedAt] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [UpdatedAt] datetime  NOT NULL,
    [UpdatedBy] nvarchar(100)  NULL,
    [IsDeleted] bit  NOT NULL,
    [DeletedAt] datetime  NULL,
    [DeletedBy] nvarchar(100)  NULL,
    [RefreshToken] nvarchar(128)  NULL,
    [RefreshTokenExpTime] datetime  NULL,
    [PasswordResetToken] nvarchar(255)  NULL,
    [ResetTokenExpiry] datetime  NULL,
    [StudentYear] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [CourseId] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [PK_Courses]
    PRIMARY KEY CLUSTERED ([CourseId] ASC);
GO

-- Creating primary key on [DepartmentId] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([DepartmentId] ASC);
GO

-- Creating primary key on [Id] in table 'OfferedCourses'
ALTER TABLE [dbo].[OfferedCourses]
ADD CONSTRAINT [PK_OfferedCourses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [SemesterId] in table 'Semesters'
ALTER TABLE [dbo].[Semesters]
ADD CONSTRAINT [PK_Semesters]
    PRIMARY KEY CLUSTERED ([SemesterId] ASC);
GO

-- Creating primary key on [StudentId], [OfferedCourseId] in table 'StudentCourses'
ALTER TABLE [dbo].[StudentCourses]
ADD CONSTRAINT [PK_StudentCourses]
    PRIMARY KEY CLUSTERED ([StudentId], [OfferedCourseId] ASC);
GO

-- Creating primary key on [UserId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DepartmentId] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [FK_dbo_Courses_dbo_Departments_DepartmentId]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([DepartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Courses_dbo_Departments_DepartmentId'
CREATE INDEX [IX_FK_dbo_Courses_dbo_Departments_DepartmentId]
ON [dbo].[Courses]
    ([DepartmentId]);
GO

-- Creating foreign key on [CourseId] in table 'OfferedCourses'
ALTER TABLE [dbo].[OfferedCourses]
ADD CONSTRAINT [FK_dbo_OfferedCourses_dbo_Courses_CourseId]
    FOREIGN KEY ([CourseId])
    REFERENCES [dbo].[Courses]
        ([CourseId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_OfferedCourses_dbo_Courses_CourseId'
CREATE INDEX [IX_FK_dbo_OfferedCourses_dbo_Courses_CourseId]
ON [dbo].[OfferedCourses]
    ([CourseId]);
GO

-- Creating foreign key on [DepartmentId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_dbo_Users_dbo_Departments_DepartmentId]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([DepartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Users_dbo_Departments_DepartmentId'
CREATE INDEX [IX_FK_dbo_Users_dbo_Departments_DepartmentId]
ON [dbo].[Users]
    ([DepartmentId]);
GO

-- Creating foreign key on [SemesterId] in table 'OfferedCourses'
ALTER TABLE [dbo].[OfferedCourses]
ADD CONSTRAINT [FK_dbo_OfferedCourses_dbo_Semesters_SemesterId]
    FOREIGN KEY ([SemesterId])
    REFERENCES [dbo].[Semesters]
        ([SemesterId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_OfferedCourses_dbo_Semesters_SemesterId'
CREATE INDEX [IX_FK_dbo_OfferedCourses_dbo_Semesters_SemesterId]
ON [dbo].[OfferedCourses]
    ([SemesterId]);
GO

-- Creating foreign key on [TeacherId] in table 'OfferedCourses'
ALTER TABLE [dbo].[OfferedCourses]
ADD CONSTRAINT [FK_dbo_OfferedCourses_dbo_Users_TeacherId]
    FOREIGN KEY ([TeacherId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_OfferedCourses_dbo_Users_TeacherId'
CREATE INDEX [IX_FK_dbo_OfferedCourses_dbo_Users_TeacherId]
ON [dbo].[OfferedCourses]
    ([TeacherId]);
GO

-- Creating foreign key on [OfferedCourseId] in table 'StudentCourses'
ALTER TABLE [dbo].[StudentCourses]
ADD CONSTRAINT [FK_dbo_StudentCourses_dbo_OfferedCourses_OfferedCourseId]
    FOREIGN KEY ([OfferedCourseId])
    REFERENCES [dbo].[OfferedCourses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_StudentCourses_dbo_OfferedCourses_OfferedCourseId'
CREATE INDEX [IX_FK_dbo_StudentCourses_dbo_OfferedCourses_OfferedCourseId]
ON [dbo].[StudentCourses]
    ([OfferedCourseId]);
GO

-- Creating foreign key on [StudentId] in table 'StudentCourses'
ALTER TABLE [dbo].[StudentCourses]
ADD CONSTRAINT [FK_dbo_StudentCourses_dbo_Users_StudentId]
    FOREIGN KEY ([StudentId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AdvisorId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_dbo_Users_dbo_Users_AdvisorId]
    FOREIGN KEY ([AdvisorId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Users_dbo_Users_AdvisorId'
CREATE INDEX [IX_FK_dbo_Users_dbo_Users_AdvisorId]
ON [dbo].[Users]
    ([AdvisorId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------