using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Layer.Model;
using MediatR;

namespace Application.Layer.Authontication.Command
{
    public class RegisterCommand : IRequest<AuthResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
