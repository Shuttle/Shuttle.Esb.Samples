﻿using Microsoft.AspNetCore.Mvc;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ServerController : Controller
    {
        [HttpGet]
        [Route("server/properties")]
        public dynamic Properties()
        {
            return new
            {
                Version = "1.0.0"
            };
        }
    }
}