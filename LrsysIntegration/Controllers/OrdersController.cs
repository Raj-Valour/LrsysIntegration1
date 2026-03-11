using LrsysIntegration.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.IO;
using LrsysIntegration.Services;
using LrsysIntegration.Models;

namespace LrsysIntegration.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly string _conn =
            ConfigurationManager.ConnectionStrings["APIString"].ConnectionString;
        private readonly OrderService _orderService;

        public OrdersController()
        {
            _orderService = new OrderService();
        }
        /* ============================================================
           GET: api/orders/check-table/{tableNumber}
           Creates suspended order (Status = 15)
        ============================================================ */

        [HttpGet]
        [Route("check-table/{tableNumber}")]
        public IHttpActionResult CheckTable(string tableNumber)
        {
            if (string.IsNullOrWhiteSpace(tableNumber))
                return BadRequest("Invalid table number");

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM EposSalesMst
                WHERE OrderNotes = @TableNumber
                AND OrderStatus IN (13,15)", conn);

                cmd.Parameters.AddWithValue("@TableNumber", tableNumber);

                int count = (int)cmd.ExecuteScalar();

                return Ok(new { Exists = count > 0 });
            }
        }

        /* ============================================================
           POST: api/orders
           Creates suspended order (Status = 15)
        ============================================================ */
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostOrder(OrderDto dto)
        {

            if (dto == null || dto.Items == null || dto.Items.Count == 0)
                return BadRequest("No items in order");

            System.Diagnostics.Debug.WriteLine("USER ID RECEIVED: " + dto.UserID);

            if (dto.UserID <= 0)
                return BadRequest("UserID is required");

            long orderRef;

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // ✅  Create new master directly
                        orderRef = _orderService.InsertOrderMaster(conn, tran, dto);

                        // ✅ Insert all items (no upsert here)
                        foreach (var item in dto.Items)
                        {
                            _orderService.InsertOrderDetail(conn, tran, orderRef, item, dto.OrderType, dto.UserID);

                        }


                        // ✅ Recalculate totals
                        using (var totalCmd = new SqlCommand("UpdateSalesMst_Total", conn, tran))
                        {
                            totalCmd.CommandType = CommandType.StoredProcedure;
                            totalCmd.Parameters.AddWithValue("@SalesOrderRef", orderRef);
                            totalCmd.Parameters.AddWithValue("@OrderType", dto.OrderType);
                            totalCmd.CommandTimeout = 0;
                            totalCmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        try
                        {
                            var path = System.Web.Hosting.HostingEnvironment
                                         .MapPath("~/order_errors.txt");

                            System.IO.File.AppendAllText(
                                path,
                                DateTime.Now + " - " + ex.ToString() + Environment.NewLine
                            );
                        }
                        catch { }

                        return InternalServerError(ex);
                    }
                }
            }

            return Ok(new { OrderRef = orderRef });
        }

        /* ============================================================
               Patch: api/orders/{orderRef}
               Returns full order (master + items)
            ============================================================ */
        [HttpPatch]
        [AcceptVerbs("PATCH")]
        [Route("{orderRef:long}")]
        public IHttpActionResult UpdateOrder(long orderRef, OrderDto dto)
        {
            if (dto == null || dto.Items == null)
                return BadRequest("Invalid data");

            if (dto.UserID <= 0)
                return BadRequest("UserID is required");

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 🔹 Check if order exists
                        using (var checkCmd = new SqlCommand(
                            "SELECT COUNT(1) FROM EposSalesMst WHERE OrderRef=@OrderRef",
                            conn, tran))
                        {
                            checkCmd.Parameters.AddWithValue("@OrderRef", orderRef);

                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (exists == 0)
                            {
                                tran.Rollback();
                                return NotFound();
                            }
                        }

                        // 🔹 Update master
                        using (var updateCmd = new SqlCommand(@"
                        UPDATE EposSalesMst
                        SET OrderNotes = @Notes,
                            OrderStatus = @OrderStatus,
                            LastUpdatedAt = GETDATE()
                        WHERE OrderRef = @OrderRef", conn, tran))
                        {// 🔹 Save note as "TABLE 454"
                            var note = "TABLE " + (dto.TableNumber ?? "");

                            updateCmd.Parameters.AddWithValue("@Notes", note); 
                            updateCmd.Parameters.AddWithValue("@OrderStatus", dto.OrderStatus);
                            updateCmd.Parameters.AddWithValue("@OrderRef", orderRef);

                            updateCmd.ExecuteNonQuery();
                        }


                        // 🔥 DELETE ALL EXISTING DETAILS (SAFE FOR MEAL DEALS)
                        using (var delCmd = new SqlCommand(
                            "DELETE FROM EposSalesDtl WHERE OrderMstRef = @OrderRef",
                            conn, tran))
                        {
                            delCmd.Parameters.AddWithValue("@OrderRef", orderRef);
                            delCmd.ExecuteNonQuery();
                        }

                        // 🔥 REINSERT ALL ITEMS
                        foreach (var item in dto.Items)
                        {
                            _orderService.InsertOrderDetail(conn, tran, orderRef, item, dto.OrderType,dto.UserID);
                        }

                        // 🔥 Recalculate totals
                        using (var totalCmd = new SqlCommand("UpdateSalesMst_Total", conn, tran))
                        {
                            totalCmd.CommandType = CommandType.StoredProcedure;
                            totalCmd.Parameters.AddWithValue("@SalesOrderRef", orderRef);
                            totalCmd.Parameters.AddWithValue("@OrderType", dto.OrderType);
                            totalCmd.CommandTimeout = 0;
                            totalCmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        try
                        {
                            var path = Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "order_errors.txt"
                            );

                            File.AppendAllText(
                                path,
                                DateTime.Now + " - PATCH - " + ex.ToString() + Environment.NewLine
                            );
                        }
                        catch { }

                        return InternalServerError(ex);
                    }
                }
            }

            return Ok(new { OrderRef = orderRef });
        }

        /* ============================================================
               GET: api/orders/{orderRef}
               Returns full order (master + items)
            ============================================================ */
        [HttpGet]
        [Route("{orderRef:long}")]
        public IHttpActionResult GetOrder(long orderRef)
        {
            string tableNumber = "";
            string notes = "";
            decimal vatTotal = 0;
            decimal orderTotal = 0;

            var items = new List<object>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                // 🔹 1️⃣ Get Master
                var masterCmd = new SqlCommand(@"
            SELECT OrderNotes, VATTotal, OrderTotal
            FROM EposSalesMst
            WHERE OrderRef = @OrderRef", conn);

                masterCmd.Parameters.AddWithValue("@OrderRef", orderRef);

                using (var reader = masterCmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return NotFound();

                    notes = reader["OrderNotes"]?.ToString() ?? "";
                    tableNumber = notes.Replace("TABLE ", "");

                    vatTotal = reader["VATTotal"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(reader["VATTotal"]);

                    orderTotal = reader["OrderTotal"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(reader["OrderTotal"]);
                }

                // 🔹 2️⃣ Get Items (🔥 UPDATED QUERY)
                var itemsCmd = new SqlCommand(@"
            SELECT 
                SalesDtlId,
                PLUID,
                Description,
                Qty,
                UnitPrice,
                VATPerc,
                InternalSalesDtlID
            FROM EposSalesDtl
            WHERE OrderMstRef = @OrderRef
            AND ISNULL(VoidLine,0)=0
            ORDER BY SalesDtlId", conn);

                itemsCmd.Parameters.AddWithValue("@OrderRef", orderRef);

                using (var reader = itemsCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var salesDtlId = Convert.ToInt64(reader["SalesDtlId"]);
                        var parentId = reader["InternalSalesDtlID"] == DBNull.Value
                            ? 0
                            : Convert.ToInt64(reader["InternalSalesDtlID"]);

                        items.Add(new
                        {
                            InternalSalesDtlId = salesDtlId,
                            ParentSalesDtlId = parentId == 0 ? (long?)null : parentId,
                            ProductId = Convert.ToInt32(reader["PLUID"]),
                            Description = reader["Description"]?.ToString() ?? "",
                            Quantity = Convert.ToInt32(reader["Qty"]),

                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),   // ✅ IMPORTANT
                            LineTotal = Convert.ToDecimal(reader["UnitPrice"]) * Convert.ToInt32(reader["Qty"]), // ✅
                            LineVAT = reader["VATPerc"] == DBNull.Value
        ? 0
        : (Convert.ToDecimal(reader["UnitPrice"]) * Convert.ToInt32(reader["Qty"]))
          - ((Convert.ToDecimal(reader["UnitPrice"]) * Convert.ToInt32(reader["Qty"]))
             / (1 + (Convert.ToDecimal(reader["VATPerc"]) / 100m))), // ✅

                            VatPercent = reader["VATPerc"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["VATPerc"]),

                            IsMealParent = parentId == 0
                        });
                    }
                }
            }

            return Ok(new
            {
                OrderRef = orderRef,
                TableNumber = tableNumber,
                Notes = notes,
                OrderTotal = orderTotal,
                Items = items
            });
        }


        /* ============================================================
               POST: api/orders/{orderRef}/print
            ============================================================ */
        /* [HttpPost]
         [Route("{orderRef:long}/print")]
         public IHttpActionResult Print(long orderRef)
         {
             if (orderRef <= 0)
                 return BadRequest("Invalid OrderRef");

             try
             {
                 using (var conn = new SqlConnection(_conn))
                 {
                     conn.Open();

                     using (var cmd = new SqlCommand("PrintOrderReceipt", conn))
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.Parameters.Add("@OrderRef", SqlDbType.BigInt).Value = orderRef;

                         cmd.ExecuteNonQuery();   // ✅ THIS WAS MISSING
                     }
                 }

                 return Ok(new { message = "Print triggered successfully" });
             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
         }*/


        /* ============================================================
           GET: api/orders/suspended
        ============================================================ */
        [HttpGet]
        [Route("suspended")]
        public IHttpActionResult GetSuspendedOrders()
        {
            var list = new List<SuspendedOrderDto>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                SELECT TOP 200
                    OrderRef,
                    OrderDate,
                    OrderTotal,
                    OrderNotes,
                    LastUpdatedAt
                FROM EposSalesMst
                WHERE OrderStatus IN (13,14,15)
                AND OrderNotes LIKE 'TABLE%'
                ORDER BY OrderDate DESC", conn);

                cmd.CommandTimeout = 120;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tableNumber = reader["OrderNotes"]?.ToString() ?? "";

                        list.Add(new SuspendedOrderDto
                        {
                            OrderRef = Convert.ToInt64(reader["OrderRef"]),

                            OrderDate = reader["OrderDate"] == DBNull.Value
                                ? DateTime.Now
                                : Convert.ToDateTime(reader["OrderDate"]),

                            Total = reader["OrderTotal"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(reader["OrderTotal"]),

                            Table = tableNumber,

                            LastUpdated = reader["LastUpdatedAt"] == DBNull.Value
                                ? DateTime.Now
                                : Convert.ToDateTime(reader["LastUpdatedAt"])
                        });
                    }
                }
            }

            return Ok(list);
        }

        /* ============================================================
           GET: api/orders/company-info
        ============================================================ */
        [HttpGet]
        [Route("company-info")]
        public IHttpActionResult GetCompanyInfo()
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
            SELECT TOP 1 CompanyID, ReceiptNotes
            FROM CompanyInfo
        ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return NotFound();

                    return Ok(new
                    {
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        ReceiptNotes = reader["ReceiptNotes"]?.ToString() ?? ""
                    });
                }
            }
        }
    }

}