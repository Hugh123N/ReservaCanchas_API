namespace Reserva.Dto.Dbo.Cancha
{
    public class SearchCanchaFilterDto
    {
        public string? Nombre { get; set; }
        public string? CodigoUbigeo { get; set; }
        public int? IdTipoDeporte { get; set; }
        public DateTimeOffset? Fecha { get; set; }
        public string? Hora { get; set; }
        public int? IdEstadoCancha { get; set; }
        public AreaGeograficaDto? Area { get; set; }

        /// <summary>
        /// ID del usuario para filtrar por favoritos.
        /// Requerido cuando SoloFavoritos es true.
        /// </summary>
        public Guid? IdUsuario { get; set; }

        /// <summary>
        /// Indica si se deben retornar solo las canchas favoritas del usuario.
        /// Si es true, IdUsuario debe estar presente.
        /// </summary>
        public bool? SoloFavoritos { get; set; }

        /// <summary>
        /// ID del proveedor para filtrar sus canchas.
        /// Permite que un proveedor vea solo las canchas que le pertenecen.
        /// </summary>
        public int? IdProveedor { get; set; }
    }
}
