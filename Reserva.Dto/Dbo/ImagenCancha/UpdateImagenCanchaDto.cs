namespace Reserva.Dto.Dbo.ImagenCancha
{
    /// <summary>
    /// DTO para actualizar propiedades de imágenes existentes.
    /// El upload se realiza mediante IFormFileCollection en endpoints dedicados.
    /// </summary>
    public class UpdateImagenCanchaDto : ImagenCanchaDto
    {
        /// <summary>
        /// ID de la imagen (null si es nueva)
        /// </summary>
        public int? IdImagenCancha { get; set; }
    }
}
