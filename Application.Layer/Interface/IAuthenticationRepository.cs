using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Layer.Authontication.Dto;
using Core.Model.Layer.Model;

namespace Application.Layer.Interface
{
    public interface IAuthenticationRepository
    {
        Task<AuthResult> RegisterAsync(UserLoginRequestDto dto);
        Task<AuthResult> LoginAsync(UserLoginRequestDto dto);
    }
}
