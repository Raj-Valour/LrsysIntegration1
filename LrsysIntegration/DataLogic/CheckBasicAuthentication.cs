using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


namespace LrsysIntegration.DataLogic
{
    public class CheckBasicAuthentication
    {

            public static bool ValidateUser(string Aname, string Apwd)
            {
                 SQLHelper objSqlHelper = new SQLHelper();
                string query = "select * from AConfig where Aname='" + Aname + "' and Apwd='" + Apwd + "'";
                DataTable dtauthenticate = objSqlHelper.Getdatatable(query);

            if (dtauthenticate.Rows.Count==0)
            {
                return false;
            }
                else
            {
                return true;
            }
                
                return false;
            }
        

    }
}