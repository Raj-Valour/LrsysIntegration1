using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

[RoutePrefix("api/device")]
public class DeviceController : ApiController
{
    private readonly string _conn =
        ConfigurationManager.ConnectionStrings["APIString"].ConnectionString;

    [HttpGet]
    [Route("validate")]
    public IHttpActionResult ValidateDevice(string deviceId)
    {
        using (SqlConnection conn = new SqlConnection(_conn))
        {
            conn.Open();

            string sql = @"SELECT SystemID 
                           FROM SystemSettings
                           WHERE DID = @DeviceId";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DeviceId", deviceId);

                var result = cmd.ExecuteScalar();

                if (result == null)
                    return BadRequest("Device not registered");

                int tillId = Convert.ToInt32(result);

                return Ok(new { TillID = tillId });
            }
        }
    }
}