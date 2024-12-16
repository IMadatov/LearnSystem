using BaseCrud.Abstractions.Entities;
using BaseCrud.PrimeNg;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{

    

    [Authorize(Roles ="teacher, admin")]
    public class TeacherController(
        ITeacherService teacherService,
        ITeacherBaseCrudService teacherBaseCrudService,
        ISubjectBaseCrudService subjectBaseCrudService,
        IClassBaseCrudService classBaseCrudService
        ):BaseController
    {
       

        [HttpPost]
        public async Task<ActionResult<QueryResult<SubjectDto>?>>GetSubjects(PrimeTableMetaData primeTableMetaData) =>
            await FromServiceResult(subjectBaseCrudService.GetAllAsync(primeTableMetaData,UserProfile));

        [HttpPost]
        public async Task<ActionResult> CreateSubject(SubjectDto createSubjectDto) =>
            await FromServiceResultBaseAsync(teacherService.CreateSubject(createSubjectDto));

        [HttpDelete]
        public async Task<ActionResult> DeleteSubjects([FromBody]List<SubjectDto> subjectDtos) =>
            await FromServiceResultBaseAsync(teacherService.DeleteSubjects(subjectDtos));

        [HttpPost]
        public async Task<ActionResult<ClassFullDto?>> CreateClass(ClassFullDto classDto)=>
            await FromServiceResult(classBaseCrudService.InsertAsync(classDto,UserProfile));

        [HttpPost]
        public async Task<ActionResult<QueryResult<ClassDto>?>> GetAllClass(PrimeTableMetaData primeTableMetaData) =>
           await FromServiceResult(classBaseCrudService.GetAllAsync(primeTableMetaData, UserProfile));

        [HttpDelete]
        public async Task<ActionResult> DeleteClasses([FromBody]List<int> classDtos)=>
            await FromServiceResultBaseAsync(teacherService.DeleteClasses(classDtos));




        protected async Task<ActionResult> FromServiceResultBaseAsync<T>(Task<ServiceResultBase<T>> task)
        {
            var result = await task;

            if (result == null)
                return NoContent();
            var isOk = result.StatusCode < 400;

            if (isOk)
            {
                return StatusCode(result.StatusCode, result.Result);
            }
            return StatusCode(result.StatusCode, "Request failed");
        }

    }
}
