﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Models;

public class LoginResponseModel
{
    public string UserName { get; set; }

    public string UserToken { get; set; }
}
