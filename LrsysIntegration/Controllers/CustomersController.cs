using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LrsysIntegration.Models;
using LrsysIntegration.Filters;
using LrsysIntegration.DataLogic;

namespace LrsysIntegration.Controllers
{
    public class CustomersController : ApiController
    {
        [BasicAuthentication]
        public List<Customers> Get(DateTime? modifiedDate,string email)
        {

            SQLHelper objSql = new SQLHelper();
            //  return objPH.GetHospitalityProductSummaryList(modifiedDate, null);
            return objSql.GetCustomersList(modifiedDate,email);

        }
    }
}
