using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EchoApi.Controllers
{
    [Route("api/[controller]")]
    public class EchoController : Controller
    {
        // GET api/values
        [HttpGet]
        public string SayHello()
        {
            return $"Hello World, it's me {System.Environment.MachineName}";
        }
    }
}
