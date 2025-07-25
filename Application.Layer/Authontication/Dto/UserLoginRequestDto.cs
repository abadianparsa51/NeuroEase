﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Layer.Authontication.Dto
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

    }
}
