namespace Reserva.Dto.Cancha.Ubigeo
{
    public class ProvinciaDto
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public IEnumerable<DistritoDto>? Distritos { get; set; }
    }
}
