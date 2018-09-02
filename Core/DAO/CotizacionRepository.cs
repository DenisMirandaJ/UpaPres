using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}