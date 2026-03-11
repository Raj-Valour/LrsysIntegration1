using System.Linq;
using LrsysIntegration.DataLogic;
using LrsysIntegration.Models;

namespace LrsysIntegration.Repositories
{
    public class AuthRepository
    {
        public Employees ValidateEmployee(string usercode, string passcode)
        {
            using (var db = new LrsysContext())
            {
                return db.Employees.FirstOrDefault(e =>
                    e.Usercode == usercode &&
                    e.UserPassword.Trim() == passcode.Trim() &&
                    (e.Inactive == false || e.Inactive == null)
                );
            }
        }
    }
}