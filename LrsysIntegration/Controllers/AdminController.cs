using LrsysIntegration.DataLogic;
using LrsysIntegration.DTOs;
using LrsysIntegration.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;


namespace LrsysIntegration.Controllers
{
    [RoutePrefix("api/admin")]
    public class AdminController : ApiController
    {
        private readonly LrsysContext db = new LrsysContext();

        // 🔹 Employees (DTO)

        //
        [HttpGet]
        [Route("employees")]
        public IHttpActionResult GetEmployees()
        {
            try
            {
                var data = db.Employees
                    .Where(e => e.Inactive == false)
                    .Select(e => new EmployeeDTO
                    {
                        UserID = e.UserID,


                        UserCode = e.Usercode.ToString(),

                        FullName =
                            (e.User_Fname ?? "") + " " + (e.User_Lname ?? ""),

                        MobileNo = e.Usr_MobileNo,
                        JobType = e.JobType,
                        Inactive = e.Inactive
                    })
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                // simple + safe
                return InternalServerError(ex);
            }
        }


        //// 🔹 Application Error Logs (DTO)

        // 
        [HttpGet]
        [Route("errorlogs")]
        public IHttpActionResult GetErrorLogs()
        {
            try
            {
                var data = db.ApplicationErrorLog
                    .OrderByDescending(e => e.ErrordateTime)
                    .Take(1000)
                    .Select(e => new ApplicationErrorLogDTO
                    {
                        ErrorLogID = e.ErrorLogID,
                        Errordescription = e.Errordescription,
                        ErrorModule = e.ErrorModule,
                        Errorform = e.Errorform,
                        ErrordateTime = e.ErrordateTime
                    })
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //POST : /api/admin/errorlogs
        [HttpPost]
        [Route("errorlogs")]
        public IHttpActionResult PostErrorLog(ApplicationErrorLogDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            try
            {
                // Output parameters
                var idParam = new SqlParameter("@ErrorLogID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var dateParam = new SqlParameter("@ErrordateTime", SqlDbType.DateTime)
                {
                    Direction = ParameterDirection.Output
                };

                var errorParam = new SqlParameter("@Error", SqlDbType.VarChar, 1000)
                {
                    Direction = ParameterDirection.Output
                };

                // Execute stored procedure
                db.Database.ExecuteSqlCommand(
                    "EXEC InsertApplicationErrorLog @ErrorLogID OUT, @Errordescription, @ErrorModule, @Errorform, @ErrordateTime OUT, @Error OUT",
                    idParam,
                    new SqlParameter("@Errordescription", dto.Errordescription ?? (object)DBNull.Value),
                    new SqlParameter("@ErrorModule", dto.ErrorModule ?? (object)DBNull.Value),
                    new SqlParameter("@Errorform", dto.Errorform ?? (object)DBNull.Value),
                    dateParam,
                    errorParam
                );

                // Read output values
                int newId = (idParam.Value != DBNull.Value) ? Convert.ToInt32(idParam.Value) : -1;
                string spError = errorParam.Value?.ToString();

                // If SP returned error
                if (!string.IsNullOrEmpty(spError))
                {
                    return Content(System.Net.HttpStatusCode.BadRequest, new
                    {
                        Message = "Stored procedure error",
                        Error = spError
                    });
                }

                return Ok(new
                {
                    Message = "Error log saved successfully",
                    ErrorLogID = newId,
                    CreatedDate = dateParam.Value
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}