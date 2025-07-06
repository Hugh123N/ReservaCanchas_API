namespace Reserva.Dto.Cancha.CanchaFavorita
{
    public class GetCanchaFavoritaDto : CanchaFavoritaDto
    {
        public int IdCanchaFavorita { get; set; }
        public bool Activo { get; set; }
    }
}
