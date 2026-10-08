namespace Reserva.Dto.Dbo.Operador
{
    public class UpdateOperadorDto : OperadorDto
    {
        public int IdOperador { get; set; }
        public List<int>? CanchaIds { get; set; }
        // Vive en AspNetUsers, no en la entidad Operador
        public string? Imagen { get; set; }
    }
}
