namespace Core.Models
{
    public class Item : BaseEntity
    {

        /// <summary>
        /// Campo Descripcion de la cotizacion
        /// </summary>
        public int CotizacionId { get; set; }
        public string descripcion { get; set; }
        
        /// <summary>
        /// Rut del cliente asociado a la cotizacion.
        /// </summary>
        public int precio  { get; set; }
        
        public Cotizacion Cotizacion { get; set; }

        public override void Validate()
        {
            
        }
        
    }
}