using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles ="teacher")]
    public class TeacherController(ITeacherService teacherService):ControllerBase
    {
       

        [HttpGet]
        public async Task<ActionResult> GetSubjects([FromQuery] int first = 0, [FromQuery] int row = 10, [FromQuery]string field = "Name", [FromQuery]short order=1) =>
            await FromServiceResultBaseAsync(teacherService.GetSubjects(first, row,field,order));

        [HttpPost]
        public async Task<ActionResult> CreateSubject(SubjectDto createSubjectDto) =>
            await FromServiceResultBaseAsync(teacherService.CreateSubject(createSubjectDto));

        [HttpDelete]
        public async Task<ActionResult> DeleteSubjects([FromBody]List<SubjectDto> subjectDtos) =>
            await FromServiceResultBaseAsync(teacherService.DeleteSubjects(subjectDtos));

        [HttpPost]
        public async Task<ActionResult> CreateClass(ClassDto classDto)=>
            await FromServiceResultBaseAsync(teacherService.CreateClass(classDto));

        [HttpGet]
        public async Task<ActionResult> GetAllClass([FromQuery] int first = 0, [FromQuery] int row = 10, [FromQuery] string field = "Name", [FromQuery] short order = 1) =>
           await FromServiceResultBaseAsync(teacherService.GetAllClass(first,row,field,order));

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
