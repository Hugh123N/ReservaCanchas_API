namespace Reserva.Domain.Services.PersonLookup;

public sealed class PersonLookupException : Exception
{
    public PersonLookupException(string message) : base(message)
    {
    }
}
