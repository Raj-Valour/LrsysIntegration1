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
    public class PLUController : ApiController
    {
        [BasicAuthentication]
        public List<PLU> Get(DateTime? modifiedDate)
        {
            
            SQLHelper objSql= new SQLHelper();
            //  return objPH.GetHospitalityProductSummaryList(modifiedDate, null);
            return objSql.GetPLUList(modifiedDate);

        }

    }
}
