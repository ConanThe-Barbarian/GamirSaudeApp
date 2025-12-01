using GamirSaude.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GamirSaude.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaudosController : ControllerBase
    {
        [HttpGet("{idPaciente}")]
        public IActionResult GetLaudos(int idPaciente)
        {
            // MOCK: Simulando retorno do sistema de laboratório
            var laudos = new List<LaudoDto>
            {
                new LaudoDto
                {
                    Id = 1,
                    NomeExame = "HEMOGRAMA COMPLETO",
                    NomeMedico = "Laboratório Gamir",
                    DataExame = DateTime.Now.AddDays(-5),
                    Status = "Disponível",
                    // PDF de exemplo (manual do governo ou algo público)
                    UrlPdf = "https://www.thecampusqdl.com/uploads/files/pdf_sample_2.pdf"
                },
                new LaudoDto
                {
                    Id = 2,
                    NomeExame = "RAIO-X TÓRAX",
                    NomeMedico = "Dr. House",
                    DataExame = DateTime.Now.AddDays(-10),
                    Status = "Disponível",
                    UrlPdf = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf"
                },
                new LaudoDto
                {
                    Id = 3,
                    NomeExame = "TESTE ERGOMÉTRICO",
                    NomeMedico = "Dra. Grey",
                    DataExame = DateTime.Now.AddDays(-2),
                    Status = "Em Análise",
                    UrlPdf = "" // Ainda não tem PDF
                }
            };

            return Ok(laudos);
        }
    }
}