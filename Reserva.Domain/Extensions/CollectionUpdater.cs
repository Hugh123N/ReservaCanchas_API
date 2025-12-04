using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reclutamiento.Domain.Extensions
{
    public static class CollectionUpdater
    {
        public static void ActualizarColeccion<TEntidad, TDto>(
            this ICollection<TEntidad> entidades,
            IEnumerable<TDto> dtos,
            Func<TEntidad, int> getIdEntidad,
            Func<TDto, int> getIdDto,
            Action<TDto, TEntidad> actualizarEntidad,
            Func<TDto, TEntidad> crearEntidad)
            where TEntidad : class
        {
            var listaDtos = dtos.ToList();

            foreach (var entidad in entidades)
            {
                var dtoEncontrado = listaDtos.FirstOrDefault(d => getIdDto(d) == getIdEntidad(entidad));
                var existe = listaDtos.Any(d => getIdDto(d) == getIdEntidad(entidad));

                if (existe)
                {
                    //update
                    actualizarEntidad(dtoEncontrado, entidad);
                }
                else
                {
                    //delete
                    var propActivo = entidad.GetType().GetProperty("Activo");
                    if (propActivo != null)
                        propActivo.SetValue(entidad, false);
                }
            }
            //nuevos
            var idsExistentes = entidades.Select(getIdEntidad).ToHashSet();
            foreach (var dto in listaDtos.Where(d => !idsExistentes.Contains(getIdDto(d))))
            {
                var nuevo = crearEntidad(dto);
                entidades.Add(nuevo);
            }
        }

        /*private void ActualizarColeccion<TEntidad, TDto>(
            ICollection<TEntidad> entidades,
            IEnumerable<TDto> dtos,
            Func<TEntidad, int> getIdEntidad,
            Func<TDto, int> getIdDto,
            Action<TDto, TEntidad> actualizarEntidad,
            Func<TDto, TEntidad> crearEntidad)
            where TEntidad : class
        {
            var listaDtos = dtos.ToList();

            foreach (var entidad in entidades)
            {
                var dto = listaDtos.FirstOrDefault(d => getIdDto(d) == getIdEntidad(entidad));

                if (dto != null)
                {
                    //update
                    actualizarEntidad(dto, entidad);
                }
                else
                {
                    //delete
                    var propActivo = entidad.GetType().GetProperty("Activo");
                    if (propActivo != null)
                        propActivo.SetValue(entidad, false);
                }
            }
            //nuevos
            var idsExistentes = entidades.Select(getIdEntidad).ToHashSet();
            foreach (var dto in listaDtos.Where(d => !idsExistentes.Contains(getIdDto(d))))
            {
                var nuevo = crearEntidad(dto);
                entidades.Add(nuevo);
            }
        }*/
    }
}
