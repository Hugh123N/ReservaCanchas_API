namespace Reserva.Dto.Dbo.Cancha;

public class CrearCanchaCompletaDto
{
    public CreateCanchaDto Cancha { get; set; } = new();
    public int IndicePrincipal { get; set; }
}
