using LrsysIntegration.Models;
using LrsysIntegration.Repositories;
using System.Net;
using System.Web.Http;

namespace LrsysIntegration.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly AuthRepository _repo = new AuthRepository();

        [HttpGet]
        [Route("login")]
        public IHttpActionResult Login(
            [FromUri] string usercode,
            [FromUri] string passcode)
        {
            if (string.IsNullOrWhiteSpace(usercode) ||
                string.IsNullOrWhiteSpace(passcode))
            {
                return BadRequest("usercode and passcode required");
            }

            var employee = _repo.ValidateEmployee(
                usercode.Trim(),
                passcode.Trim()
            );

            if (employee == null)
            {
                return Content(HttpStatusCode.Unauthorized, new
                {
                    success = false,
                    message = "Invalid usercode or password"
                });
            }

            return Ok(new
            {
                success = true,
                userId = employee.UserID,
                userCode = employee.Usercode,
                fullName = employee.User_Fname + " " + employee.User_Lname,
                jobType = employee.JobType,
                message = "Login successful"
            });
        }
    }
}