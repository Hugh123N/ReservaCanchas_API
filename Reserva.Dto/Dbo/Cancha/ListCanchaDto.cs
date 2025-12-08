
namespace Reserva.Dto.Dbo.Cancha
{
    public class ListCanchaDto: CanchaDto
    {
        public int IdEstadoCancha { get; set; }
        public string Codigo { get; set; } = null!;
    }
}
