using LrsysIntegration.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LrsysIntegration.Services
{
    public class MenuService
    {
        private readonly string _connStr;

        public MenuService()
        {
            _connStr =
                ConfigurationManager.ConnectionStrings["APIString"]?.ConnectionString
                ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // ================= MENU ITEMS =================
        public List<MenuItemDto> GetMenuItems()
        {
            var items = new List<MenuItemDto>();

            var connStr =
                System.Configuration.ConfigurationManager
                    .ConnectionStrings["APIString"]?.ConnectionString
                ?? System.Configuration.ConfigurationManager
                    .ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(_connStr))
            {
                con.Open();

                string sql = @"
                SELECT
                PLUQ.ProductIDQ AS ProductId,
                PLUQ.Description,
                PLUQ.VATID,
                ISNULL(v.VATValue,0) AS VATValue,

                CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM MealsMaster mm 
                        WHERE mm.ProductID = PLUQ.ProductIDQ
                        AND ISNULL(mm.Deleted,0)=0
                    )
                    THEN 1 ELSE 0
                END AS HasMealDeal,

                CASE 
                    WHEN ISNULL(SubDepartments.SubDepartment1BackColor,'')='' 
                    THEN ISNULL(DepartmentBackColor,'Black') 
                    ELSE SubDepartments.SubDepartment1BackColor 
                END AS BackColor,

                CASE 
                    WHEN ISNULL(SubDepartments.SubDepartment1ForeColor,'')='' 
                    THEN ISNULL(DepartmentForeColor,'White') 
                    ELSE SubDepartments.SubDepartment1ForeColor 
                END AS ForeColor,

                PLUQ.DepartmentID,
                DepartmentName,
                ProductCode,
                PLUQ.SellingPrice AS Price
                FROM Departments
                INNER JOIN PLUQ ON Departments.DepartmentID = PLUQ.DepartmentID
                LEFT JOIN VATMaster v ON PLUQ.VATID = v.VATID
                LEFT JOIN SubDepartments ON PLUQ.SubDepartmentID = SubDepartments.SubDepartmentID1
                WHERE ISNULL(PLUQ.Deleted,0)=0
                  AND ISNULL(QuickMenu,0)=1
                  AND ISNULL(WeightProduct,0)=0";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        items.Add(new MenuItemDto
                        {
                            ProductId = Convert.ToInt32(dr["ProductId"]),
                            Description = dr["Description"].ToString(),
                            BackColor = dr["BackColor"].ToString(),
                            ForeColor = dr["ForeColor"].ToString(),
                            DepartmentId = Convert.ToInt32(dr["DepartmentID"]),
                            DepartmentName = dr["DepartmentName"].ToString(),
                            ProductCode = dr["ProductCode"].ToString(),
                            VatId = dr["VATID"] != DBNull.Value ? Convert.ToInt32(dr["VATID"]) : 0,
                            VatPercent = dr["VATValue"] != DBNull.Value ? Convert.ToDecimal(dr["VATValue"]) : 0,
                            Price = dr["Price"] != DBNull.Value
                                    ? Convert.ToDecimal(dr["Price"])
                                    : 0,
                            HasMealDeal = dr["HasMealDeal"] != DBNull.Value
                                        && Convert.ToInt32(dr["HasMealDeal"]) == 1
                        });
                    }
                }

                // Now load notes options and attach to items by DepartmentId
                string notesSql = @"
                SELECT
                    CAST(LEFT(TableLinkedTo, CHARINDEX('/', TableLinkedTo) - 1) AS INT) AS DepartmentID,
                    GeneralID AS NoteId,
                    Name AS NoteName,
                    Backcolorstr,
                    Forecolorstr
                FROM dbo.GeneralList
                WHERE TableLinkedTo LIKE '%/Notes Options'
                ORDER BY DepartmentID, Name;";

                var notesByDept = new Dictionary<int, List<NoteOptionDto>>();

                using (SqlCommand cmdNotes = new SqlCommand(notesSql, con))
                using (var drn = cmdNotes.ExecuteReader())
                {
                    while (drn.Read())
                    {
                        int deptId = drn["DepartmentID"] != DBNull.Value ? Convert.ToInt32(drn["DepartmentID"]) : 0;
                        var note = new NoteOptionDto
                        {
                            DepartmentId = deptId,
                            NoteId = drn["NoteId"] != DBNull.Value ? Convert.ToInt32(drn["NoteId"]) : 0,
                            NoteName = drn["NoteName"].ToString(),
                            BackColor = drn["Backcolorstr"].ToString(),
                            ForeColor = drn["Forecolorstr"].ToString()
                        };

                        if (!notesByDept.ContainsKey(deptId))
                            notesByDept[deptId] = new List<NoteOptionDto>();

                        notesByDept[deptId].Add(note);
                    }
                }

                // Attach notes to each item
                foreach (var item in items)
                {
                    List<NoteOptionDto> deptList;
                    if (notesByDept.TryGetValue(item.DepartmentId, out deptList))
                    {
                        item.NotesOptions = deptList;
                    }
                }
            }

            return items;
        }

        // ================= MEAL GROUPS =================
        public List<MealGroupDto> GetMealGroups(int parentProductId)
        {
            var list = new List<MealGroupDto>();

            using (var con = new SqlConnection(_connStr))
            {
                con.Open();

                string sql = @"SELECT 
                        MealGroupID, 
                        MealDescription, 
                        MaxAllowed,
                        ButtonColor,
                        TextColor
                    FROM MealsMaster
                    WHERE ISNULL(Deleted,0)=0
                    AND ProductID=@ProductID";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ProductID", parentProductId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MealGroupDto
                            {
                                MealGroupId = Convert.ToInt32(dr["MealGroupID"]),
                                MealDescription = dr["MealDescription"].ToString(),
                                MaxAllowed = Convert.ToInt32(dr["MaxAllowed"]),
                                ButtonColor = dr["ButtonColor"]?.ToString(),
                                TextColor = dr["TextColor"]?.ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        // ================= MEAL ITEMS =================
        public List<MealItemDto> GetMealItems(int mealGroupId)
        {
            var list = new List<MealItemDto>();

            using (var con = new SqlConnection(_connStr))
            {
                con.Open();

                string sql = @"SELECT d.ProductID, p.Description, d.MealMasterID,
                               ISNULL(d.AdditionalPrice,0) AdditionalPrice
                               FROM MealsDtl d
                               INNER JOIN PLUQ p ON d.ProductID = p.ProductIDQ
                               WHERE d.MealMasterID=@MealGroupId";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MealGroupId", mealGroupId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MealItemDto
                            {
                                ProductId = Convert.ToInt32(dr["ProductID"]),
                                Description = dr["Description"].ToString(),
                                MealGroupId = Convert.ToInt32(dr["MealMasterID"]),
                                AdditionalPrice = Convert.ToDecimal(dr["AdditionalPrice"])
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
