namespace Reserva.Dto.Dbo.Cancha;

public class ActualizarCanchaCompletaDto
{
    public UpdateCanchaDto Cancha { get; set; } = new();
    public DateTimeOffset Version { get; set; }
    public List<int> ImagenesConservar { get; set; } = new();
    public int? IdImagenPrincipal { get; set; }
    public int? IndicePrincipalNueva { get; set; }
}
