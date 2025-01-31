﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Menominee.Api.Features
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policies.RequireAuthenticatedUser)]
    public class BaseApplicationController<T> : ControllerBase
    {
        protected readonly ILogger<T> Logger;

        public BaseApplicationController(ILogger<T> logger)
        {
            Logger = logger;
        }
    }
}
