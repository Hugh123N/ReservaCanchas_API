namespace Reserva.Domain.Services.Storage;

/// <summary>
/// Interface para servicios de almacenamiento de archivos en la nube
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Sube un archivo al storage y retorna la URL pública
    /// </summary>
    /// <param name="file">Stream del archivo</param>
    /// <param name="path">Ruta relativa en el bucket (ej: "canchas/1/imagen.jpg")</param>
    /// <param name="contentType">Tipo MIME (ej: "image/jpeg")</param>
    /// <returns>URL pública del archivo</returns>
    Task<string> UploadAsync(Stream file, string path, string contentType);

    /// <summary>
    /// Elimina un archivo del storage
    /// </summary>
    /// <param name="path">Ruta relativa en el bucket</param>
    Task DeleteAsync(string path);

    /// <summary>
    /// Elimina una carpeta completa del storage
    /// </summary>
    /// <param name="folderPath">Ruta de la carpeta (ej: "canchas/1/")</param>
    Task DeleteFolderAsync(string folderPath);

    /// <summary>
    /// Lista archivos en una carpeta
    /// </summary>
    /// <param name="folderPath">Ruta de la carpeta</param>
    /// <returns>Lista de rutas de archivos</returns>
    Task<IEnumerable<string>> ListAsync(string folderPath);

    /// <summary>
    /// Obtiene la URL pública de un archivo
    /// </summary>
    /// <param name="path">Ruta relativa en el bucket</param>
    /// <returns>URL pública completa</returns>
    string GetPublicUrl(string path);
}
