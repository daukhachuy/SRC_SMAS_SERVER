using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.AiBaseServices;
using SMAS_Services.ComboServices;
using SMAS_Services.InventoryServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AIAssistantController : Controller
    {
       
    }
}
