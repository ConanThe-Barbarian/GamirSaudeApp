using GamirSaude.Application.DTOs;
using GamirSaude.Domain.Entities;
using GamirSaude.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // Para DbTransaction (apesar de usarmos ADO.NET puro)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GamirSaude.API.Controllers
{
}