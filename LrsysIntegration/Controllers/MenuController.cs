using LrsysIntegration.Services;
using LrsysIntegration.DTOs;
using System;
using System.Linq;
using System.Web.Http;

namespace LrsysIntegration.Controllers
{
    [RoutePrefix("api/menu")]
    public class MenuController : ApiController
    {
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;

        public MenuController()
        {
            _menuService = new MenuService();
            _orderService = new OrderService();
        }

        // GET api/menu/items
        [HttpGet]
        [Route("items")]
        public IHttpActionResult GetMenuItems()
        {
            try
            {
                var items = _menuService.GetMenuItems();

                if (items == null || !items.Any())
                    return Ok(new { message = "No menu items found", data = items });

                return Ok(items);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/menu/meal/groups/280095
        [HttpGet]
        [Route("meal/groups/{productId:int}")]
        public IHttpActionResult GetMealGroups(int productId)
        {
            try
            {
                var groups = _menuService.GetMealGroups(productId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/menu/meal/items/128
        [HttpGet]
        [Route("meal/items/{groupId:int}")]
        public IHttpActionResult GetMealItems(int groupId)
        {
            try
            {
                var items = _menuService.GetMealItems(groupId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
