namespace Reserva.Dto.Cancha.TipoCancha
{
    public class GetTipoCanchaDto : TipoCanchaDto
    {
        public int IdTipoCancha { get; set; }
        public bool Activo { get; set; }
    }
}
