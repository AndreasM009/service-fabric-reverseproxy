using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    public class CalculatorController : Controller
    {
        // GET api/values
        [HttpGet("{val1}/{val2}")]
        public int Add(int val1, int val2)
        {
            return val1 + val2;
        }
    }
}
