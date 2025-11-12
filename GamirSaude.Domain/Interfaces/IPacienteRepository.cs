using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamirSaude.Domain.Entities;

namespace GamirSaude.Domain.Interfaces
{
    public interface IPacienteRepository
    {
        Task<Paciente?> GetByCpfAsync(string cpf);
        Task AddAsync(Paciente paciente);
    }
}