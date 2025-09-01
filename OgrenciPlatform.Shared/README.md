# OgrenciPlatform.Shared - Veri Transfer Katmanı (Data Transfer Layer)

**Türkçe** | [English](#english-version)

Bu proje, Öğrenci Platformu çözümünde katmanlar arası veri aktarımı için özel olarak tasarlanmış Data Transfer Objects (DTO) kütüphanesidir. Web API ve frontend katmanları arasında güvenli ve optimize edilmiş veri transferini sağlar.

## 📁 İçerik Yapısı

### DTO (Data Transfer Objects)
Katmanlar arası veri aktarımı için özelleştirilmiş sınıflar:
- **AdvisorDTO.cs** - Danışman bilgileri ve yetkilendirme modelleri
- **CoursesDTO.cs** - Ders bilgileri ve ders içerikleri
- **DepartmentsDTO.cs** - Bölüm ve fakülte bilgileri
- **EditUserDTO.cs** - Kullanıcı profil düzenleme modelleri
- **LoginRequestDTO.cs** - Giriş ve kimlik doğrulama istekleri
- **OfferedCoursesDTO.cs** - Sunulan ders katalog modelleri
- **SemesterDTO.cs** - Akademik dönem ve zaman yönetimi
- **StudentDTO.cs** - Öğrenci bilgileri ve akademik kayıtlar
- **UserDTO.cs** - Kullanıcı hesap bilgileri

## 🎯 Kullanım Amacı

Bu kütüphane özellikle şu senaryolarda kullanılır:
- **API Response Models** - Web API'den frontend'e veri aktarımı
- **Form Validation** - Frontend form doğrulamaları
- **Data Serialization** - JSON/XML serileştirme işlemleri
- **Security Filtering** - Hassas verilerin filtrelenmesi

## 🔧 Teknolojiler

- **.NET Framework 4.7.2**
- **ASP.NET MVC 5.3.0** - Model binding desteği
- **System.Web.Mvc** - MVC entegrasyonu

## 🔒 Güvenlik Özellikleri

- **Data Sanitization** - Input validation ve temizleme
- **Selective Serialization** - Sadece gerekli verilerin aktarımı
- **Type Safety** - Strongly typed model kullanımı

## 📦 Bağımlılıklar

- **Microsoft.AspNet.Mvc** (5.3.0)
- **Microsoft.AspNet.WebPages** (3.3.0)
- **Microsoft.Web.Infrastructure** (1.0.0.0)

## 🔄 Fark - Shared Projesi ile

Bu proje diğer `Shared` projesinden farklı olarak:
- **Sadece DTO'ları** içerir (Constants ve Enums içermez)
- **MVC bağımlılıkları** bulunur
- **Web uygulaması odaklı** tasarım
- **API serialization** için optimize edilmiştir

---

# English Version

This project is a specialized Data Transfer Objects (DTO) library designed for inter-layer data transfer in the Student Platform solution. It provides secure and optimized data transfer between Web API and frontend layers.

## 📁 Content Structure

### DTO (Data Transfer Objects)
Specialized classes for inter-layer data transfer:
- **AdvisorDTO.cs** - Advisor information and authorization models
- **CoursesDTO.cs** - Course information and content
- **DepartmentsDTO.cs** - Department and faculty information
- **EditUserDTO.cs** - User profile editing models
- **LoginRequestDTO.cs** - Login and authentication requests
- **OfferedCoursesDTO.cs** - Offered course catalog models
- **SemesterDTO.cs** - Academic semester and time management
- **StudentDTO.cs** - Student information and academic records
- **UserDTO.cs** - User account information

## 🎯 Purpose

This library is specifically used for:
- **API Response Models** - Data transfer from Web API to frontend
- **Form Validation** - Frontend form validations
- **Data Serialization** - JSON/XML serialization processes
- **Security Filtering** - Filtering sensitive data

## 🔧 Technologies

- **.NET Framework 4.7.2**
- **ASP.NET MVC 5.3.0** - Model binding support
- **System.Web.Mvc** - MVC integration

## 🔒 Security Features

- **Data Sanitization** - Input validation and cleaning
- **Selective Serialization** - Transfer only necessary data
- **Type Safety** - Strongly typed model usage

## 📦 Dependencies

- **Microsoft.AspNet.Mvc** (5.3.0)
- **Microsoft.AspNet.WebPages** (3.3.0)
- **Microsoft.Web.Infrastructure** (1.0.0.0)

## 🔄 Difference from Shared Project

This project differs from the other `Shared` project in that:
- Contains **only DTOs** (no Constants and Enums)
- Has **MVC dependencies**
- **Web application focused** design
- Optimized for **API serialization**