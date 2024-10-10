using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface ITeacherService
    {
        Task<ServiceResultBase<object>> CreateClass();
        Task<ServiceResultBase<bool>> CreateSubject(SubjectDto createSubjectDto);
        Task<ServiceResultBase<bool>> DeleteSubjects(List<SubjectDto> subjectDtos);
        Task<ServiceResultBase<List<UserDto>>> GetAllStudents();
        Task<ServiceResultBase<PaginatedList<SubjectDto>>> GetSubjects(int first,int row,string field,short order);
    }
}
