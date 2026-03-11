using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LrsysIntegration.Models;
using LrsysIntegration.DataLogic;
using LrsysIntegration.Filters;

namespace LrsysIntegration.Controllers
{
    [BasicAuthentication]
    public class InvoiceController : ApiController
    {
        public List<InvoiceModel.Invoice_Response> Post(List<InvoiceModel.Invoice> objInvoice)
        {

            SQLHelper objsql = new SQLHelper();
            return objsql.PostInvoice(objInvoice);

            //SalesHelper objSH = new SalesHelper();
            //string json_res = objSH.InsertSalesList(objSale);
            //HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.OK, "Sales Response");
            //response.Content = new StringContent(json_res, Encoding.Unicode);
            //return response;
        }

      



    }
}
