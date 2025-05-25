namespace Reserva.Dto.Cancha.Ubigeo
{
    public class DepartamentoDto
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public IEnumerable<ProvinciaDto>? Provincias { get; set; }
    }
}
