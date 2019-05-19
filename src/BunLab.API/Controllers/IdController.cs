using BunLab.API.Models.GUIDs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace BunLab.API.Controllers
{
    /// <summary>
    /// 专注处理 Id 的协议
    /// </summary>
    [Route("ids")]
    [ApiController]
    public class IdController : ControllerBase
    {
        /// <summary>
        /// 生成一个新的 GUID
        /// </summary>
        /// <returns></returns>
        [HttpPost("guids")]
        [SwaggerResponse(StatusCodes.Status201Created, "返回新生成的 GUID", typeof(NewGUIDResponse))]
        public IActionResult NewId()
        {
            return Ok(new NewGUIDResponse
            {
                GUID = Guid.NewGuid().ToString()
            });
        }
    }
}