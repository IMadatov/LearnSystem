using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface ITeacherService
    {
        Task<ServiceResultBase<bool>> CreateClass(ClassDto classDto);
        Task<ServiceResultBase<bool>> DeleteClasses(List<int> classDtos);
        Task<ServiceResultBase<bool>> CreateSubject(SubjectDto createSubjectDto);
        Task<ServiceResultBase<bool>> DeleteSubjects(List<SubjectDto> subjectDtos);
        Task<ServiceResultBase<PaginatedList<ClassDto>>> GetAllClass(int first, int row, string field, short order);
        Task<ServiceResultBase<List<UserDto>>> GetAllStudents(int first, int row, string field, short order);
        Task<ServiceResultBase<PaginatedList<SubjectDto>>> GetSubjects(int first,int row,string field,short order);
    }
}
