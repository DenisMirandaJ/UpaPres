using System;
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
    }
}