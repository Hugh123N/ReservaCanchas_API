namespace Reserva.Dto.Dbo.Cliente
{
    public class UpdateClienteDto : ClienteDto
    {
        public int IdCliente { get; set; }

        // Vive en AspNetUsers, no en la entidad Cliente (mismo criterio que UpdateOperadorDto)
        public string? Imagen { get; set; }
    }
}
