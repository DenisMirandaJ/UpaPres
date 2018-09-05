using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.DAO
{
    /// <summary>
    /// Operaciones repositorio cotizacion.
    /// </summary>
    public interface ICotizacionRepository : IRepository<Cotizacion>
    {
        /// <inheritdoc cref="ModelRepository{T}"/>

        /// <summary>
        /// Busca una cotizacion por id
        /// </summary>
        /// <param name="id">RUT</param>
        /// <returns>The Personas</returns>
        Cotizacion GetById(int id);

        /// <summary>
        /// Obtiene las cotizaciones creadas entre dos fechas
        /// </summary>
        /// <returns></returns>
        List<Cotizacion> GetbyDate(DateTime d1, DateTime d2);
    }
}