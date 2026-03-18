using LrsysIntegration.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace LrsysIntegration.Services

{
    public class OrderService
    {
        private readonly string _connStr;

        public OrderService()
        {
            _connStr = ConfigurationManager
                .ConnectionStrings["APIString"].ConnectionString;
        }

        private void GetCompanyInfo(SqlConnection conn, SqlTransaction tran, out int companyId, out string receiptNotes)
        {
            companyId = 1;
            receiptNotes = "";

            using (var cmd = new SqlCommand("SELECT TOP 1 CompanyID, ReceiptNotes FROM CompanyInfo", conn, tran))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        companyId = Convert.ToInt32(reader["CompanyID"]);
                        receiptNotes = reader["ReceiptNotes"] == DBNull.Value
                            ? ""
                            : reader["ReceiptNotes"].ToString();
                    }
                }
            }
        }

        /* ============================================================
            Insert Master
        ============================================================ */
        public long InsertOrderMaster(
            SqlConnection conn,
            SqlTransaction tran,
            OrderDto dto)
        {
            int companyId;
            string receiptNotes;

            GetCompanyInfo(conn, tran, out companyId, out receiptNotes);

            using (var cmd = new SqlCommand("InsertEposSalesMst", conn, tran))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var orderRefParam = new SqlParameter("@OrderRef", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };

                var orderDateParam = new SqlParameter("@OrderDate", SqlDbType.DateTime)
                {
                    Direction = ParameterDirection.Output
                };

                var errorParam = new SqlParameter("@Error", SqlDbType.VarChar, 1000)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(orderRefParam);
                cmd.Parameters.Add(orderDateParam);
                cmd.Parameters.Add(errorParam);

                cmd.Parameters.AddWithValue("@OrderType",
                        dto.OrderType == 1 ? "True" : "CashnCarry");
                cmd.Parameters.AddWithValue("@OrderTotal", 0);
                cmd.Parameters.AddWithValue("@PaidTotal", 0);
                cmd.Parameters.AddWithValue("@VATTotal", 0);
                cmd.Parameters.AddWithValue("@ExportOrder", false);
                cmd.Parameters.AddWithValue("@UserDiscountTotal", 0);
                cmd.Parameters.AddWithValue("@UserDiscountReason", "");
                cmd.Parameters.AddWithValue("@PromoDiscountTotal", 0);
                cmd.Parameters.AddWithValue("@OrderTakenBy", dto.UserID);
                cmd.Parameters.AddWithValue("@OrderCommissionTo", dto.UserID);
                cmd.Parameters.AddWithValue("@OrderNotes", "TABLE " + dto.TableNumber);
                cmd.Parameters.AddWithValue("@OrderStatus", dto.OrderStatus);
                cmd.Parameters.AddWithValue("@DeliveryStatus", 0);
                cmd.Parameters.AddWithValue("@CustomerID", 0);
                cmd.Parameters.AddWithValue("@CompanyID", companyId);
                cmd.Parameters.AddWithValue("@TillID", dto.TillID);
                cmd.Parameters.AddWithValue("@DeliveryName", "");
                cmd.Parameters.AddWithValue("@DeliveryAddress", "");
                cmd.Parameters.AddWithValue("@DeliveryTown", "");
                cmd.Parameters.AddWithValue("@DeliveryCounty", "");
                cmd.Parameters.AddWithValue("@DeliveryPostCode", "");
                cmd.Parameters.AddWithValue("@HOUploaded", false);
                cmd.Parameters.AddWithValue("@CustomerOrderRef", "");
                cmd.Parameters.AddWithValue("@CustomerOrderNotes", "");
                cmd.Parameters.AddWithValue("@ReceiptNotes", receiptNotes);
                cmd.Parameters.AddWithValue("@NoOfItems", 0);
                cmd.Parameters.AddWithValue("@Change", 0);

                cmd.ExecuteNonQuery();

                var error = errorParam.Value?.ToString();
                if (!string.IsNullOrEmpty(error))
                    throw new Exception(error);

                return Convert.ToInt64(orderRefParam.Value);
            }
        }

        /* ============================================================
            Insert Detail
        ============================================================ */
        public void InsertOrderDetail(
        SqlConnection conn,
        SqlTransaction tran,
        long orderRef,
        OrderItemDto item,
        short orderType,
        int userId,
        int tillId)
        {
            long parentId;

            int companyId;
            string receiptNotes;

            GetCompanyInfo(conn, tran, out companyId, out receiptNotes);

            using (var cmd = new SqlCommand("InsertorUpdateSalesDtl", conn, tran))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InsertedSalesDtlId", 0);
                cmd.Parameters.AddWithValue("@OrderMasterRef", orderRef);
                cmd.Parameters.AddWithValue("@PLUID", item.ProductId);
                cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                cmd.Parameters.AddWithValue("@PLUDescription", item.Description ?? "");
                cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@Addedby", userId);
                cmd.Parameters.AddWithValue("@barcode", item.Barcode ?? "");
                cmd.Parameters.AddWithValue("@packSize", 1);
                cmd.Parameters.AddWithValue("@CompanyID", companyId);
                cmd.Parameters.AddWithValue("@TillID", tillId);
                cmd.Parameters.AddWithValue("@WeightProduct", false);
                cmd.Parameters.AddWithValue("@Discount", 0);
                cmd.Parameters.AddWithValue("@DiscountType", "");

                string unitNote = item.ItemNote;
                cmd.Parameters.AddWithValue("@DiscountReason",
                    string.IsNullOrWhiteSpace(item.VoidReason) ? (object)DBNull.Value : item.VoidReason);

                cmd.Parameters.AddWithValue("@OrderType", orderType);
                cmd.Parameters.AddWithValue("@RefundPrice", 0);
                cmd.Parameters.AddWithValue("@MealDealItem", item.IsMealDeal);
                cmd.Parameters.AddWithValue("@InternalSalesDtlID", 0);

                var salesDtlIdParam = new SqlParameter("@salesDtlID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(salesDtlIdParam);

                var errorParam = new SqlParameter("@Error", SqlDbType.NVarChar, 1000)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(errorParam);

                cmd.ExecuteNonQuery();

                var error = errorParam.Value?.ToString();
                if (!string.IsNullOrEmpty(error))
                    throw new Exception(error);

                parentId = Convert.ToInt64(salesDtlIdParam.Value);

                if (!string.IsNullOrWhiteSpace(unitNote))
                {
                    using (var noteCmd = new SqlCommand(@"
                            UPDATE EposSalesDtl
                            SET UnitName = @Note
                            WHERE SalesDtlId = @SalesDtlId
                        ", conn, tran))
                    {
                        noteCmd.Parameters.AddWithValue("@Note", unitNote);
                        noteCmd.Parameters.AddWithValue("@SalesDtlId", parentId);
                        noteCmd.ExecuteNonQuery();
                    }
                }

                if (!string.IsNullOrWhiteSpace(item.VoidReason))
                {
                    using (var voidCmd = new SqlCommand(@"
                        UPDATE EposSalesDtl
                        SET VoidLine = 1,
                            VoidedBy = @UserId,
                            VoidedAt = GETDATE(),
                            DiscountReason = @Reason
                        WHERE SalesDtlId = @SalesDtlId
                    ", conn, tran))
                    {
                        voidCmd.Parameters.AddWithValue("@UserId", userId);
                        voidCmd.Parameters.AddWithValue("@SalesDtlId", parentId);
                        voidCmd.Parameters.AddWithValue("@Reason", item.VoidReason);

                        voidCmd.ExecuteNonQuery();
                    }
                }
            }

            if (item.IsMealDeal && item.MealItems != null && item.MealItems.Count > 0)
            {
                foreach (var child in item.MealItems)
                {
                    using (var childCmd = new SqlCommand("InsertorUpdateSalesDtl", conn, tran))
                    {
                        childCmd.CommandType = CommandType.StoredProcedure;

                        childCmd.Parameters.AddWithValue("@InsertedSalesDtlId", 0);
                        childCmd.Parameters.AddWithValue("@OrderMasterRef", orderRef);
                        childCmd.Parameters.AddWithValue("@PLUID", child.ProductId);
                        childCmd.Parameters.AddWithValue("@Qty", child.Quantity);
                        childCmd.Parameters.AddWithValue("@PLUDescription", child.Description ?? "");
                        childCmd.Parameters.AddWithValue("@UnitPrice", child.UnitPrice);
                        childCmd.Parameters.AddWithValue("@Addedby", userId);
                        childCmd.Parameters.AddWithValue("@barcode", child.Barcode ?? "");
                        childCmd.Parameters.AddWithValue("@packSize", 1);
                        childCmd.Parameters.AddWithValue("@CompanyID", companyId);
                        childCmd.Parameters.AddWithValue("@TillID", tillId);
                        childCmd.Parameters.AddWithValue("@WeightProduct", false);
                        childCmd.Parameters.AddWithValue("@Discount", 0);
                        childCmd.Parameters.AddWithValue("@DiscountType", "");
                        object childDiscountReason;

                        if (string.IsNullOrWhiteSpace(child.VoidReason))
                            childDiscountReason = "Meal Deal";
                        else
                            childDiscountReason = child.VoidReason;

                        childCmd.Parameters.AddWithValue("@DiscountReason", childDiscountReason);
                        childCmd.Parameters.AddWithValue("@OrderType", orderType);
                        childCmd.Parameters.AddWithValue("@RefundPrice", 0);

                        childCmd.Parameters.AddWithValue("@MealDealItem", true);
                        childCmd.Parameters.AddWithValue("@InternalSalesDtlID", parentId);

                        var childOutId = new SqlParameter("@salesDtlID", SqlDbType.BigInt)
                        {
                            Direction = ParameterDirection.Output
                        };
                        childCmd.Parameters.Add(childOutId);

                        var childError = new SqlParameter("@Error", SqlDbType.NVarChar, 1000)
                        {
                            Direction = ParameterDirection.Output
                        };
                        childCmd.Parameters.Add(childError);

                        childCmd.ExecuteNonQuery();

                        if (!string.IsNullOrWhiteSpace(child.ItemNote))
                        {
                            using (var noteCmd = new SqlCommand(@"
                                UPDATE EposSalesDtl
                                SET UnitName = @Note
                                WHERE SalesDtlId = @SalesDtlId
                            ", conn, tran))
                            {
                                noteCmd.Parameters.AddWithValue("@Note", child.ItemNote);
                                noteCmd.Parameters.AddWithValue("@SalesDtlId", childOutId.Value);
                                noteCmd.ExecuteNonQuery();
                            }
                        }

                        var err = childError.Value?.ToString();
                        if (!string.IsNullOrEmpty(err))
                            throw new Exception(err);

                        using (var fixCmd = new SqlCommand(@"
                            UPDATE EposSalesDtl
                            SET UnitPrice = 0,
                                LineTotal = 0,
                                LineVAT = 0
                            WHERE SalesDtlId = @SalesDtlId
                        ", conn, tran))
                        {
                            fixCmd.Parameters.AddWithValue("@SalesDtlId", childOutId.Value);
                            fixCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}