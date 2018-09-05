using System;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Core.DAO
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Cotizacion"></typeparam>
    public class CotizacionRepository : ModelRepository<Cotizacion>, ICotizacionRepository
    {
        /// <inheritdoc cref="ModelRepository{T}"/>
        public CotizacionRepository(DbContext dbContext) : base(dbContext)
        {
            // Nothing here
        }

        public List<Cotizacion>GetbyDate(DateTime d1, DateTime d2)
        {
            IList<Cotizacion> cotizaciones = this.GetAll();
            List<Cotizacion> cotizacionesEntreFechas = new List<Cotizacion>();
            foreach (Cotizacion cotizacion in cotizaciones)
            {
                if (cotizacion.FechaCreacion > d1 && cotizacion.FechaCreacion < d2)
                {
                    cotizacionesEntreFechas.Add(cotizacion);
                }
            }

            return cotizacionesEntreFechas;
        }
    }
}