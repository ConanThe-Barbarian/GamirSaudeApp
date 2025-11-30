using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamirSaude.Application.Services;
using GamirSaude.Domain.Entities;

namespace GamirSaude.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(UsuarioApp usuario);
    }
}