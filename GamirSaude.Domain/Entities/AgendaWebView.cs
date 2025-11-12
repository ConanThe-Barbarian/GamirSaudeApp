using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("vw_cad_prestadorMedicoEspecialidadeAgendaWeb")]
    public class AgendaWebView
    {
        [Key]
        public int idPrestadorMedicoEspecialidade { get; set; }
        public int idPrestadorMedico { get; set; }
        public string? EspecialidadeCBOS { get; set; }
        public string? PrestadorMedico { get; set; }
        public int? idProcedimentoTussNomenclatura { get; set; }
        public string? DescricaoAgenda { get; set; }
        // Colunas adicionadas para obter a descrição do procedimento (se a VIEW expuser)

        // 1. EscalaAgenda: Usada no cálculo de duração do slot (GetHorariosDisponiveis)
        // Mapeamos explicitamente para o nome da coluna no DB/View que é usada na lógica
        [Column("EscalaAgenda")]
        public int? EscalaAgenda { get; set; }

        // 2. Atende com Agenda Web: Usada para filtrar os médicos no GetMedicosPorEspecialidade
        [Column("AtendeComAgendaWeb")]
        public bool? AtendeComAgendaWeb { get; set; }
        [Column("AtendeComAgendaWebExame")]
        public bool AtendeComAgendaWebExame { get; set; }
        [Column("PMPDescricaoTussReferencia")] 
        public string? PMPDescricaoTussReferencia { get; set; }

        // --- PROPRIEDADES DE EXPEDIENTE E VAGAS ---
        public bool Segunda { get; set; }
        public bool Terca { get; set; }
        public bool Quarta { get; set; }
        public bool Quinta { get; set; }
        public bool Sexta { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }

        public int? SegundaQuantidadeMaxima { get; set; }
        public int? TercaQuantidadeMaxima { get; set; }
        public int? QuartaQuantidadeMaxima { get; set; }
        public int? QuintaQuantidadeMaxima { get; set; }
        public int? SextaQuantidadeMaxima { get; set; }
        public int? SabadoQuantidadeMaxima { get; set; }
        public int? DomingoQuantidadeMaxima { get; set; }

        // Horas de Expediente (Adicionado '?' e Mapeamento Explícito)
        [Column("SegundaHoraInicio")] public string? SegundaHoraInicio { get; set; }
        [Column("SegundaHoraFim")] public string? SegundaHoraFim { get; set; }
        [Column("TercaHoraInicio")] public string? TercaHoraInicio { get; set; }
        [Column("TercaHoraFim")] public string? TercaHoraFim { get; set; }
        [Column("QuartaHoraInicio")] public string? QuartaHoraInicio { get; set; }
        [Column("QuartaHoraFim")] public string? QuartaHoraFim { get; set; }
        [Column("QuintaHoraInicio")] public string? QuintaHoraInicio { get; set; }
        [Column("QuintaHoraFim")] public string? QuintaHoraFim { get; set; }
        [Column("SextaHoraInicio")] public string? SextaHoraInicio { get; set; }
        [Column("SextaHoraFim")] public string? SextaHoraFim { get; set; }
        [Column("SabadoHoraInicio")] public string? SabadoHoraInicio { get; set; }
        [Column("SabadoHoraFim")] public string? SabadoHoraFim { get; set; }
        [Column("DomingoHoraInicio")] public string? DomingoHoraInicio { get; set; }
        [Column("DomingoHoraFim")] public string? DomingoHoraFim { get; set; }

        // --- PROPRIEDADES DE ALMOÇO ---
        public bool SegundaAlmoco { get; set; }
        public bool TercaAlmoco { get; set; }
        public bool QuartaAlmoco { get; set; }
        public bool QuintaAlmoco { get; set; }
        public bool SextaAlmoco { get; set; }
        public bool SabadoAlmoco { get; set; }
        public bool DomingoAlmoco { get; set; }

        // Horas de Almoço (Adicionado '?' para null values e Mapeamento Explícito)
        [Column("SegundaAlmocoinicio")] public string? SegundaAlmocoinicio { get; set; }
        [Column("TercaAlmocoinicio")] public string? TercaAlmocoinicio { get; set; }
        [Column("QuartaAlmocoinicio")] public string? QuartaAlmocoinicio { get; set; }
        [Column("QuintaAlmocoinicio")] public string? QuintaAlmocoinicio { get; set; }
        [Column("SextaAlmocoinicio")] public string? SextaAlmocoinicio { get; set; }
        [Column("SabadoAlmocoinicio")] public string? SabadoAlmocoinicio { get; set; }
        [Column("DomingoAlmocoinicio")] public string? DomingoAlmocoinicio { get; set; }

        [Column("SegundaAlmocoFim")] public string? SegundaAlmocoFim { get; set; }
        [Column("TercaAlmocoFim")] public string? TercaAlmocoFim { get; set; }
        [Column("QuartaAlmocoFim")] public string? QuartaAlmocoFim { get; set; }
        [Column("QuintaAlmocoFim")] public string? QuintaAlmocoFim { get; set; }
        [Column("SextaAlmocoFim")] public string? SextaAlmocoFim { get; set; }
        [Column("SabadoAlmocoFim")] public string? SabadoAlmocoFim { get; set; }
        [Column("DomingoAlmocoFim")] public string? DomingoAlmocoFim { get; set; }
    }
}