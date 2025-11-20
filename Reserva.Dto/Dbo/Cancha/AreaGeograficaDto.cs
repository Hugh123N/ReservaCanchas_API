namespace Reserva.Dto.Dbo.Cancha
{
    /// <summary>
    /// DTO que representa los límites geográficos de un área rectangular en el mapa.
    /// Utilizado para filtrar canchas dentro de un área visible del mapa (bounding box).
    /// </summary>
    public class AreaGeograficaDto
    {
        public decimal Norte { get; set; }
        
        public decimal Sur { get; set; }

        public decimal Este { get; set; }

        public decimal Oeste { get; set; }
    }
}
