namespace Reserva.Dto.Dbo.PersonLookup;

public class DniLookupDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MaternalLastName { get; set; }
    public string DocumentNumber { get; set; } = null!;
}
