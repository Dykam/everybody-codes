using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Search.Lib;

namespace Search.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamerasController : ControllerBase
    {
        private readonly ICameraRepository cameras;

        public CamerasController(ICameraRepository cameras)
        {
            this.cameras = cameras;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Camera>> Get()
        {
            return new JsonResult(cameras.GetSnapshot());
        }

        // GET api/values/5
        [HttpGet("{number}")]
        public async Task<ActionResult<Camera>> Get(int number)
        {
            return await cameras.Get(number);
        }

        // GET api/values/5
        [HttpGet("name/{name}")]
        public async Task<ActionResult<Camera>> GetByName(string name)
        {
            return await cameras.Get(name);
        }

        // GET api/values/5
        [HttpGet("search/{partialName}")]
        public async Task<ActionResult<Camera>> SearchByPartialName(string partialName)
        {
            return new JsonResult(await cameras.Search(partialName));
        }
    }
}
