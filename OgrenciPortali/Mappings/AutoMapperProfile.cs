using AutoMapper;
using OgrenciPortali.ViewModels;
using Shared.DTO;
using System.Linq;
using System.Web.Mvc;

namespace OgrenciPortali.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // --- User Mappings ---
            // API'den gelen kullanıcı listesini ViewModel'e dönüştür
            CreateMap<UserDTO, UserViewModel>();

            // Kullanıcı kayıt ViewModel'ini API'ye gönderilecek DTO'ya dönüştür
            CreateMap<RegisterViewModel, RegisterDataDto>();

            // Kullanıcı güncelleme ViewModel'ini API'ye gönderilecek DTO'ya dönüştür
            CreateMap<UpdateUserViewModel, EditUserDTO>();
            // API'den gelen kullanıcı düzenleme verisini ViewModel'e dönüştür
            CreateMap<EditUserDTO, UpdateUserViewModel>()
                .ForMember(dest => dest.RolesList,
                    opt => opt.MapFrom(src => new SelectList(src.RolesList, "Value", "Text")))
                .ForMember(dest => dest.DepartmentsList,
                    opt => opt.MapFrom(src => new SelectList(src.DepartmentsList, "Value", "Text")))
                .ForMember(dest => dest.AdvisorsList,
                    opt => opt.MapFrom(src => new SelectList(src.AdvisorsList, "Value", "Text")));

            // Şifre değiştirme ViewModel'ini DTO'ya dönüştür
            CreateMap<ChangePasswordViewModel, ChangePasswordDTO>();
            //Giriş viewmodelini DTO'ya dönüştür
            CreateMap<LoginUserViewModel, LoginUserDTO>();


            // --- Department Mappings ---
            CreateMap<ListDepartmentsDTO, DepartmentListViewModel>();
            CreateMap<DepartmentAddViewModel, AddDepartmentDTO>();
            CreateMap<DepartmentEditViewModel, EditDepartmentDTO>();
            CreateMap<EditDepartmentDTO, DepartmentEditViewModel>();


            // --- Semester Mappings ---
            CreateMap<ListSemestersDTO, SemesterListViewModel>();
            CreateMap<SemesterAddViewModel, AddSemesterDTO>();
            CreateMap<SemesterUpdateviewModel, EditSemesterDTO>();
            CreateMap<EditSemesterDTO, SemesterUpdateviewModel>();


            // --- Course Mappings ---
            CreateMap<AddCourseViewModel, AddCourseDTO>();
            CreateMap<EditCourseViewModel, EditCourseDTO>();
            CreateMap<EditCourseDTO, EditCourseViewModel>()
                .ForMember(dest => dest.DepartmentList,
                    opt => opt.MapFrom(src =>
                        new SelectList(
                            src.DepartmentsList.Select(d => new SelectListItem
                                { Value = d.DepartmentId.ToString(), Text = d.DepartmentName }), "Value", "Text")));
            CreateMap<AddCourseDTO, AddCourseViewModel>()
                .ForMember(dest => dest.DepartmentList, // Hedefteki DepartmentList özelliği için
                    opt => opt.MapFrom(src => // Kaynaktan nasıl doldurulacağını belirt
                        new SelectList(
                            src.DepartmentsList,
                            "DepartmentId",
                            "DepartmentName")));
            // --- Offered Course Mappings ---
            CreateMap<AddOfferedCourseViewModel, AddOfferedCourseDTO>();
            CreateMap<EditOfferedCourseViewModel, EditOfferedCoursesDTO>();
            CreateMap<EditOfferedCoursesDTO, EditOfferedCourseViewModel>()
                .ForMember(dest => dest.CourseList,
                    opt => opt.MapFrom(src => new SelectList(src.CourseList, "Value", "Text")))
                .ForMember(dest => dest.SemesterList,
                    opt => opt.MapFrom(src => new SelectList(src.SemesterList, "Value", "Text")))
                .ForMember(dest => dest.AdvisorList,
                    opt => opt.MapFrom(src => new SelectList(src.AdvisorList, "Value", "Text")))
                .ForMember(dest => dest.DaysList,
                    opt => opt.MapFrom(src => new SelectList(src.DaysOfWeek, "Value", "Text")));
            CreateMap<AddOfferedCourseDTO, AddOfferedCourseViewModel>()
                .ForMember(dest => dest.TeacherId,
                    opt => opt.MapFrom(src => src.AdvisorId))
                .ForMember(dest => dest.CourseList,
                    opt => opt.MapFrom(src => new SelectList(src.CourseList, "Value", "Text")))
                .ForMember(dest => dest.SemesterList,
                    opt => opt.MapFrom(src => new SelectList(src.SemesterList, "Value", "Text")))
                .ForMember(dest => dest.AdvisorList,
                    opt => opt.MapFrom(src => new SelectList(src.AdvisorList, "Value", "Text")))
                .ForMember(dest => dest.DaysList,
                    opt => opt.MapFrom(src => new SelectList(src.DaysOfWeek, "Value", "Text")));

            // --- Student & Enrollment Mappings ---
            CreateMap<EnrollDTO, EnrollViewModel>();
            CreateMap<EnrollPageDTO, EnrollmentPageViewModel>();
            CreateMap<ApprovalRequestDto, AdvisorApprovalViewModel>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")));
        }
    }
}