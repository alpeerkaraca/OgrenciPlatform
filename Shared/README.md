# Shared - Ortak Kütüphane (Shared Library)

**Türkçe** | [English](#english-version)

Bu proje, Öğrenci Platformu çözümünde ortak olarak kullanılan sınıfları, sabitleri, enum'ları ve veri transfer objelerini (DTO) içeren paylaşımlı kütüphane projesidir.

## 📁 İçerik Yapısı

### DTO (Data Transfer Objects)
Katmanlar arası veri aktarımı için kullanılan sınıflar:
- **AdvisorDTO.cs** - Danışman bilgileri
- **AiRequestDto.cs** - AI entegrasyonu için istek modelleri  
- **BaseClass.cs** - Temel DTO sınıfı
- **ConflictsDTO.cs** - Ders çakışma bilgileri
- **CoursesDTO.cs** - Ders bilgileri
- **DepartmentsDTO.cs** - Bölüm bilgileri
- **EditUserDTO.cs** - Kullanıcı düzenleme modelleri
- **OfferedCoursesDTO.cs** - Sunulan dersler
- **SemesterDTO.cs** - Dönem bilgileri
- **StudentDTO.cs** - Öğrenci bilgileri
- **TestEmailDto.cs** - E-posta varlık kontrolü için istek modeli
- **UserDTO.cs** - Kullanıcı bilgileri

### Constants (Sabitler)
- **AppRoles.cs** - Uygulama rolleri sabitleri

### Enums (Numaralandırmalar)
- **Enums.cs** - Sistem genelinde kullanılan numaralandırmalar

## 🎯 Kullanım Amacı

Bu kütüphane aşağıdaki projelerde referans alınarak kullanılır:
- **OgrenciPortalApi** - Web API katmanında model binding ve response model olarak
- **OgrenciPortali** - MVC frontend'de view model ve form model olarak

## 🔧 Teknolojiler

- **.NET Framework 4.7.2**
- **Class Library** projesi

## 📦 Bağımlılıklar

Minimal bağımlılıklarla tasarlanmış, sadece temel .NET Framework bileşenlerini kullanır.

---

# English Version

This project contains shared classes, constants, enums, and data transfer objects (DTOs) used commonly across the Student Platform solution.

## 📁 Content Structure

### DTO (Data Transfer Objects)
Classes used for data transfer between layers:
- **AdvisorDTO.cs** - Advisor information
- **AiRequestDto.cs** - Request models for AI integration
- **BaseClass.cs** - Base DTO class
- **ConflictsDTO.cs** - Course conflict information
- **CoursesDTO.cs** - Course information
- **DepartmentsDTO.cs** - Department information
- **EditUserDTO.cs** - User editing models
- **OfferedCoursesDTO.cs** - Offered courses
- **SemesterDTO.cs** - Semester information
- **StudentDTO.cs** - Student information
- **TestEmailDto.cs** - Request model for email existence checking
- **UserDTO.cs** - User information

### Constants
- **AppRoles.cs** - Application role constants

### Enums
- **Enums.cs** - System-wide enumerations

## 🎯 Purpose

This library is referenced and used by:
- **OgrenciPortalApi** - As model binding and response models in Web API layer
- **OgrenciPortali** - As view models and form models in MVC frontend

## 🔧 Technologies

- **.NET Framework 4.7.2**
- **Class Library** project

## 📦 Dependencies

Designed with minimal dependencies, using only basic .NET Framework components.