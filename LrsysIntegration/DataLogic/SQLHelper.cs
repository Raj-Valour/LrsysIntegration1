using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using LrsysIntegration.Models;

namespace LrsysIntegration.DataLogic
{
    public class SQLHelper
    {
        public string Connectionstr = ConfigurationManager.ConnectionStrings["APIString"].ConnectionString + ";User Id=Lrsys;Password=LrSyS0807;Integrated Security=false;";

        public DataTable Getdatatable(string SQL)
        {

           
            DataTable dtData = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(Connectionstr))
                {
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        cmd.CommandTimeout = 0;
                        using (SqlDataAdapter DTADP = new SqlDataAdapter(cmd))
                        {
                            DTADP.Fill(dtData);
                            DTADP.Dispose();
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { }
            return dtData;
        }


        public string PostIndividualInvoice(InvoiceModel.Invoice objIndividualSale)
        {
           // string err = "";

            string errfield = "";
            string err1="";

            try
            {
                // .Invoice_Header.InvoiceRef

                String InvoiceRef;
                DateTime Invoicedate;
                decimal Invoice_OrderTotal;
                string Invoice_CustomerID;
                string Invoice_BarcodeAllocated;
                string Invoice_Customername;
                string Invoice_Address;
                string Invoice_City;
                string Invoice_PostCode;
                int Invoice_LoyaltyPointsGained;
                int Invoice_LoyaltyPointsRedeemed;
                decimal Invoice_DeliveryCost;
                string Invoice_Email;
                string Invoice_PhoneNumner;




                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.InvoiceRef.ToString()))
                {
                    return "Empty Invoice Ref.";
                }
                else
                {
                    InvoiceRef = objIndividualSale.Invoice_Header.InvoiceRef.ToString();
                }


                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoicedate.ToString()))
                {
                    return "Empty Invoice Date";
                }
                else
                {
                    Invoicedate = objIndividualSale.Invoice_Header.Invoicedate;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_OrderTotal.ToString()))
                {
                    return "Empty Invoice Total.";
                }
                else
                {
                    Invoice_OrderTotal = objIndividualSale.Invoice_Header.Invoice_OrderTotal;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_CustomerID.ToString()))
                {
                    Invoice_CustomerID = "0";
                }
                else
                {
                    Invoice_CustomerID = objIndividualSale.Invoice_Header.Invoice_CustomerID;

                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_BarcodeAllocated.ToString()))
                {
                    Invoice_BarcodeAllocated = "";
                }
                else
                {
                    Invoice_BarcodeAllocated = objIndividualSale.Invoice_Header.Invoice_BarcodeAllocated;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_Customername.ToString()))
                {
                    Invoice_Customername = "";
                }
                else
                {
                    Invoice_Customername = objIndividualSale.Invoice_Header.Invoice_Customername;
                }


                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_Address.ToString()))
                {
                    Invoice_Address = "";
                }
                else
                {
                    Invoice_Address = objIndividualSale.Invoice_Header.Invoice_Address;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_City.ToString()))
                {
                    Invoice_City = "";
                }
                else
                {
                    Invoice_City = objIndividualSale.Invoice_Header.Invoice_City;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_PostCode.ToString()))
                {
                    Invoice_PostCode = "";
                }
                else
                {
                    Invoice_PostCode = objIndividualSale.Invoice_Header.Invoice_PostCode;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_PostCode.ToString()))
                {
                    Invoice_PostCode = "";
                }
                else
                {
                    Invoice_PostCode = objIndividualSale.Invoice_Header.Invoice_PostCode;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_LoyaltyPointsGained.ToString()))
                {
                    Invoice_LoyaltyPointsGained = 0;
                }
                else
                {
                    Invoice_LoyaltyPointsGained = objIndividualSale.Invoice_Header.Invoice_LoyaltyPointsGained;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_LoyaltyPointsRedeemed.ToString()))
                {
                    Invoice_LoyaltyPointsRedeemed = 0;
                }
                else
                {
                    Invoice_LoyaltyPointsRedeemed = objIndividualSale.Invoice_Header.Invoice_LoyaltyPointsRedeemed;
                }


                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_DeliveryCost.ToString()))
                {
                    Invoice_DeliveryCost = 0;
                }
                else
                {
                    Invoice_DeliveryCost = objIndividualSale.Invoice_Header.Invoice_DeliveryCost;
                }


                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_Email.ToString()))
                {
                    Invoice_Email = "";
                }
                else
                {
                    Invoice_Email = objIndividualSale.Invoice_Header.Invoice_Email;
                }

                if (string.IsNullOrEmpty(objIndividualSale.Invoice_Header.Invoice_PhoneNumner.ToString()))
                {
                    Invoice_PhoneNumner = "";
                }
                else
                {
                    Invoice_PhoneNumner = objIndividualSale.Invoice_Header.Invoice_PhoneNumner;
                }

                DataTable dtInvoiceDtl = new DataTable();
                dtInvoiceDtl.Columns.Add("PLUID", typeof(Int32));

                dtInvoiceDtl.Columns.Add("Qty", typeof(Decimal));
                dtInvoiceDtl.Columns.Add("Description", typeof(string));
                dtInvoiceDtl.Columns.Add("ItemPrice", typeof(decimal));
                dtInvoiceDtl.Columns.Add("TotalItemsPrice", typeof(decimal));
                dtInvoiceDtl.Columns.Add("Vat", typeof(decimal));

                foreach (InvoiceModel.Invoice_Detail InvoiceDtl in objIndividualSale.Invoice_Detail)
                {
                    DataRow row = dtInvoiceDtl.NewRow();
                    errfield = "PLUID";
                    if (string.IsNullOrEmpty(InvoiceDtl.PLUID.ToString()))
                    {
                        return "Empty PLU";
                    }
                    else
                    {
                        row["PLUID"] = InvoiceDtl.PLUID;
                    }
                        
                    errfield = "Qty";

                    if (string.IsNullOrEmpty(InvoiceDtl.Qty.ToString()))
                    {
                        return "Empty Qty";
                    }
                    else
                    {
                        row["Qty"] = InvoiceDtl.Qty;
                    }

                   
                    errfield = "Description";


                    if (string.IsNullOrEmpty(InvoiceDtl.Description.ToString()))
                    {
                        return "Empty Description";
                    }
                    else
                    {
                        row["Description"] = InvoiceDtl.Description;
                    }


                    errfield = "ItemPrice";
                    if (string.IsNullOrEmpty(InvoiceDtl.ItemPrice.ToString()))
                    {
                        return "Empty ItemPrice";
                    }
                    else
                    {
                        row["ItemPrice"] = InvoiceDtl.ItemPrice;
                    }


                    
                    errfield = "TotalItemsPrice";

                    if (string.IsNullOrEmpty(InvoiceDtl.TotalItemsPrice.ToString()))
                    {
                        return "Empty TotalItemsPrice";
                    }
                    else
                    {
                        row["TotalItemsPrice"] = InvoiceDtl.TotalItemsPrice;
                    }


                    errfield = "Vat";
                    if (string.IsNullOrEmpty(InvoiceDtl.Vat.ToString()))
                    {
                        return "Empty VAT";
                    }
                    else
                    {
                        row["VAT"] = InvoiceDtl.Vat;
                    }

                    dtInvoiceDtl.Rows.Add(row);


                }

                DataTable dtPayments = new DataTable();
                dtPayments.Columns.Add("Paymentby", typeof(string));
                dtPayments.Columns.Add("Paymentref", typeof(string));
                dtPayments.Columns.Add("PaidAmount", typeof(decimal));


                foreach (InvoiceModel.Invoice_Payments InvoicePayments in objIndividualSale.Invoice_Payments)
                {
                    DataRow row = dtPayments.NewRow();

                    errfield = "Paymentby";

                    if (string.IsNullOrEmpty(InvoicePayments.Paymentby.ToString()))
                    {
                        return "Empty Payment by";
                    }
                    else
                    { 
                        row["Paymentby"] = InvoicePayments.Paymentby;
                    }


                    errfield = "Paymentref";
                    if (string.IsNullOrEmpty(InvoicePayments.Paymentref.ToString()))
                    {
                        row["Paymentref"] = DBNull.Value;
                    }
                    else
                    {
                        row["Paymentref"] = InvoicePayments.Paymentref;
                    }

                    
                    errfield = "PaidAmount";


                    if (string.IsNullOrEmpty(InvoicePayments.PaidAmount.ToString()))
                    {
                        return "Empty PaidAmount";
                    }
                    else
                    {
                        row["PaidAmount"] = InvoicePayments.PaidAmount;
                    }

                    dtPayments.Rows.Add(row);


                }


                try
                {
                    using (SqlConnection con = new SqlConnection(Connectionstr))
                    {
                        using (SqlCommand cmd = new SqlCommand("API_POSTSALE", con))
                        {
                            if (con.State == ConnectionState.Closed)
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@InvoiceRef", InvoiceRef);
                            cmd.Parameters.AddWithValue("@Invoicedate", Invoicedate);
                            cmd.Parameters.AddWithValue("@Invoice_OrderTotal", Invoice_OrderTotal);
                            cmd.Parameters.AddWithValue("@Invoice_CustomerID", Invoice_CustomerID);
                            cmd.Parameters.AddWithValue("@Invoice_BarcodeAllocated", Invoice_BarcodeAllocated);
                            cmd.Parameters.AddWithValue("@Invoice_Customername", Invoice_Customername);
                            cmd.Parameters.AddWithValue("@Invoice_Address", Invoice_Address);
                            cmd.Parameters.AddWithValue("@Invoice_City", Invoice_City);
                            cmd.Parameters.AddWithValue("@Invoice_PostCode", Invoice_PostCode);
                            cmd.Parameters.AddWithValue("@Invoice_LoyaltyPointsGained", Invoice_LoyaltyPointsGained);
                            cmd.Parameters.AddWithValue("@Invoice_LoyaltyPointsRedeemed", Invoice_LoyaltyPointsRedeemed);
                            cmd.Parameters.AddWithValue("@Invoice_DeliveryCost", Invoice_DeliveryCost);
                            cmd.Parameters.AddWithValue("@Invoice_Email", Invoice_Email);
                            cmd.Parameters.AddWithValue("@Invoice_PhoneNumber", Invoice_PhoneNumner);

                            
                            SqlParameter dtParam = cmd.Parameters.AddWithValue("@Invoice_dtlList", dtInvoiceDtl);
                            dtParam.SqlDbType = SqlDbType.Structured;

                            //dtParam.SqlDbType = SqlDbType.Structured;

                            SqlParameter dtParam1 = cmd.Parameters.AddWithValue("@Invoice_PaymentsList", dtPayments);
                            dtParam1.SqlDbType = SqlDbType.Structured;

                            cmd.Parameters.Add("@Err", SqlDbType.NVarChar, 500);
                            cmd.Parameters["@Err"].Direction = ParameterDirection.Output;
                            
                            
                            cmd.CommandTimeout = 0;
                            //int res = 
                                cmd.ExecuteNonQuery();
                            if (cmd.Parameters["@Err"].Value != null)
                            {
                                //string resul = cmd.Parameters["@status_message"].Value.ToString();
                                err1 = cmd.Parameters["@Err"].Value.ToString();
                            }
                            
                                cmd.Dispose();
                                con.Close();
                                con.Dispose();
                                return err1;

                                
                            

                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }




                return err1;
            }
            
            catch (Exception ex)
            {
                return errfield + ' ' + ex.Message;
            }
        }

        public void Exqry(string queryString, String Connstring)
        {
            try
            {
                SqlConnection DbConn = new SqlConnection(Connstring);
                                
                if (DbConn.State == 0)
                {
                    DbConn.Open();
                }

                SqlCommand cmdexec = new SqlCommand(queryString, DbConn);
                
                cmdexec.CommandTimeout = 60000;
                cmdexec.ExecuteNonQuery();

                cmdexec.Dispose();
                DbConn.Close();
            }
            catch
            {
                //captureErrorLog("Error while executing", queryString);
            }

        }

        public void CaptureLog(string err,string module,string form)
        {

            try
            {
                 
                SqlCommand cmd = new SqlCommand();
                SqlConnection con = new SqlConnection(Connectionstr);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Errordescription", err);
                cmd.Parameters.AddWithValue("@ErrorModule", module);
                cmd.Parameters.AddWithValue("@Errorform", form);

                cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 500);
                cmd.Parameters["@Error"].Direction = ParameterDirection.Output;


                cmd.Parameters.Add("@ErrordateTime", SqlDbType.DateTime);
                cmd.Parameters["@ErrordateTime"].Direction = ParameterDirection.Output;


                cmd.CommandTimeout = 0;
                string err1;
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@Error"].Value != null)
                {
                
                    err1 = cmd.Parameters["@Error"].Value.ToString();
                }
            }

            catch
            {

            }
        }

        public List<InvoiceModel.Invoice_Response> PostInvoice(List<InvoiceModel.Invoice> objInvoice)
        {
            List<InvoiceModel.Invoice_Response> objResponse = new List<InvoiceModel.Invoice_Response>();
            try
            {                
                foreach (InvoiceModel.Invoice s in objInvoice)
                {
                    string result = "Success";

                     result = PostIndividualInvoice(s);

                    InvoiceModel.Invoice_Response obj_Individualresponse= new InvoiceModel.Invoice_Response();

                   // CaptureLog(s.Invoice_Header.InvoiceRef.ToString() + " " + result.ToString(), "Invoice","API Invoice");

                    //Exqry("Insert into ")

                    obj_Individualresponse.InvoiceRef = s.Invoice_Header.InvoiceRef;
                    if (result != "Success")
                    {
                        obj_Individualresponse.Reason = result;
                        obj_Individualresponse.InvoiceStatus = "Failed";
                    }
                    else
                    {
                        obj_Individualresponse.Reason = "";
                        obj_Individualresponse.InvoiceStatus = "Success";
                    }
                    objResponse.Add(obj_Individualresponse);
                }
                return objResponse;                
            }
            catch (Exception ex)
            {
                InvoiceModel.Invoice_Response obj_Individualresponse = new InvoiceModel.Invoice_Response();
                obj_Individualresponse.InvoiceRef = "0";
                obj_Individualresponse.InvoiceStatus = "Failed";
                obj_Individualresponse.Reason = ex.Message.ToString();
                objResponse.Add(obj_Individualresponse);
                return objResponse;
            }

        }


        public List<Customers> GetCustomersList(DateTime? modifiedDate,string Email)
        {
            List<Customers> CustomersList = new List<Customers>();
            try
            {
                DataTable dt = new DataTable();
                dt = Getdatatable("exec API_CustomerDetails '" + modifiedDate + "','" + Email + "'");

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Customers objCustomers = new Customers();
                        objCustomers.CustomerID= row["CustomerID"].ToString();
                        objCustomers.CustomerName = row["CustomerName"].ToString();
                        objCustomers.CustomerAddress = row["CustomerAddress"].ToString();
                        objCustomers.CustomerTown = row["CustomerTown"].ToString();
                        objCustomers.CustomerCounty = row["CustomerCounty"].ToString();
                        objCustomers.CustomerPostCode = row["CustomerPostCode"].ToString();
                        objCustomers.CustomerTelephone= row["CustomerTelephone"].ToString();
                        objCustomers.CustomerMobile= row["CustomerMobile"].ToString();
                        objCustomers.CustomerEmail = row["CustomerEmail"].ToString();
                        objCustomers.CustomerVATNo = row["CustomerVATNo"].ToString();
                        objCustomers.TotalLoyaltyPoints=Convert.ToInt32(row["TotalLoyaltyPoints"].ToString());
                        objCustomers.BarcodeAllocated = row["BarcodeAllocated"].ToString();
                        objCustomers.LoyaltyLastUpdatedAt = row["LoyaltyLastUpdatedAt"].ToString();
                        objCustomers.CustomerDOB = row["CustomerDOB"].ToString();
                        CustomersList.Add(objCustomers);
                    }
                }

               
            }
            catch
            {
                Customers objCustomers = new Customers();
                objCustomers.CustomerName = "";
                objCustomers.CustomerAddress ="";
                objCustomers.CustomerTown = "";
                objCustomers.CustomerCounty = "";
                objCustomers.CustomerPostCode = "";
                objCustomers.CustomerTelephone = "";
                objCustomers.CustomerMobile = "";
                objCustomers.CustomerEmail = "";
                objCustomers.CustomerVATNo = "";
                objCustomers.TotalLoyaltyPoints = 0;
                objCustomers.BarcodeAllocated = "";
                objCustomers.LoyaltyLastUpdatedAt ="";
                objCustomers.CustomerDOB = "";
                CustomersList.Add(objCustomers);
               // return CustomersList;
            }
            return CustomersList;
        }

        public List<PLU> GetPLUList(DateTime? modifiedDate)
        {           
            List<PLU> PluList = new List<PLU>();
            try
            {
                DataTable dt = new DataTable();
                dt=Getdatatable("exec API_PLULIst '" + modifiedDate + "'");

               // dt = Getdatatable("API_PLULIst '09/26/2022'");

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        PLU objPLU = new PLU();
                        objPLU.PLUID = Convert.ToInt32(row["ProductIDQ"].ToString());
                        objPLU.PLUCode = row["ProductCode"].ToString();
                        objPLU.Description= row["Description"].ToString();
                        objPLU.Department= row["DepartmentName"].ToString();
                        objPLU.SubDepartment = row["SubDepartment1Name"].ToString();
                       objPLU.BrandName = row["BrandName"].ToString();
                        objPLU.Barcode = row["Barcode"].ToString();
                        objPLU.MinStock= Convert.ToDecimal(row["MinStock"]);
                        objPLU.MaxStock = Convert.ToDecimal(row["MaxStock"]);
                        objPLU.ProductStock = Convert.ToDecimal(row["InStock"]);
                        objPLU.VAT = Convert.ToDecimal(row["VATValue"]);
                        objPLU.Cost = Convert.ToDecimal(row["CostPrice"]);
                        objPLU.Price = Convert.ToDecimal(row["SellingPrice"]);
                        objPLU.Deleted = Convert.ToInt32(row["InStock"]);
                        objPLU.LastModifiedAt = row["ModifiedDateTime"].ToString();

                        objPLU.warehouseprice = Convert.ToDecimal(row["WarehousePrice"]);
                        objPLU.WarehouseStock = Convert.ToDecimal(row["WarehouseStock"]);
                        objPLU.Warehouse_LastModifiedAt = row["Warehousemodifiedat"].ToString();

                        PluList.Add(objPLU);
                    }
                }
                return PluList;
             }           
            catch
            {
                PLU objPLU = new PLU();
                objPLU.PLUID = 0;
                objPLU.PLUCode = "";
                objPLU.Description = "";
                objPLU.Department = "";
                objPLU.SubDepartment ="";
                objPLU.BrandName = "";
                objPLU.Barcode = "";
                objPLU.MinStock = 0;
                objPLU.MaxStock = 0;
                objPLU.ProductStock = 0;
                objPLU.VAT = 0;
                objPLU.Cost = 0;
                objPLU.Price = 0;
                objPLU.Deleted =0;
                objPLU.LastModifiedAt ="01/01/2010";
                objPLU.warehouseprice = 0;
                objPLU.WarehouseStock = 0;
                objPLU.Warehouse_LastModifiedAt = "01/01/2010";


                PluList.Add(objPLU);
                return PluList;
            }
        }

        public string Gstr(object val)
        {
            string Gstr1;
            if (val == null)
            {
                Gstr1 = "";
                return Gstr1;
            }
            if (val == System.DBNull.Value)
            {
                Gstr1 = "";
                return Gstr1;
            }

            char[] charsToTrim = { '*', ' ', '\'' };
            if (val.ToString().Trim(charsToTrim) == "")
            {
                Gstr1 = "";
                return Gstr1;
            }
            Gstr1 = val.ToString();
            return Gstr1;
        }

        public decimal Gdec(object val)
        {
            try
            {


                decimal GDecimalValue;
                if (val == null)
                {
                    GDecimalValue = 0;
                    return GDecimalValue;
                }
                if (val == System.DBNull.Value)
                {
                    GDecimalValue = 0;
                    return GDecimalValue;
                }
                char[] charsToTrim = { '*', ' ', '\'' };
                if (val.ToString().Trim(charsToTrim) == "")
                {
                    GDecimalValue = 0;
                    return GDecimalValue;
                }
                GDecimalValue = (Decimal)val;
                return GDecimalValue;
            }
            catch
            {
                return 0;
            }
        }

        public bool GBool(object val)
        {
            bool GBool1;
            if (val == null)
            {
                GBool1 = false;
                return GBool1;
            }
            if (val == System.DBNull.Value)
            {
                GBool1 = false;
                return GBool1;
            }
            GBool1 = (bool)val;
            return GBool1;
        }



    }
}