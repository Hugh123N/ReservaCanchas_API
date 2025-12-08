
namespace Reserva.Dto.Dbo.Cancha
{
    public class SelectCanchaDto: CanchaDto
    {
        public int IdEstadoCancha { get; set; }
        public string Codigo { get; set; } = null!;
        public int? IdCancha { get; set; }
    }
}
