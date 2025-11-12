using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamirSaude.Domain.Entities;
using GamirSaude.Domain.Interfaces;
using GamirSaude.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GamirSaude.Infrastructure.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly GamirSaudeDbContext _context;

        public PacienteRepository(GamirSaudeDbContext context)
        {
            _context = context;
        }

        public async Task<Paciente?> GetByCpfAsync(string cpf)
        {
            return await _context.Pacientes.FirstOrDefaultAsync(p => p.Cpf == cpf);
        }

        public async Task AddAsync(Paciente paciente)
        {
            await _context.Pacientes.AddAsync(paciente);
            await _context.SaveChangesAsync();
        }
    }
}
