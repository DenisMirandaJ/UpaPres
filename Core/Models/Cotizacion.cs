using System;
using System.Collections.Generic;

namespace Core.Models
    
{    
    /// <summary>
    /// Clase que representa una cotizacion en el sistema de presupuesto.
    /// </summary>
    public class Cotizacion : BaseEntity
    {
    
        /// <summary>
        /// Identificador unico.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Rut del cliente asociado a la cotizacion.
        /// </summary>
        public string rutCliente { get; set; }
        
        /// <summary>
        /// fecha la cual se realiaza la cotizacion.
        /// </summary>
        public DateTime fechaCreacion { get; set; }
        
        /// <summary>
        /// Rut del usuario creador de la cotizacion.
        /// </summary>
        public string rutUsuarioCreador { get; set; }
        
        /// <summary>
        /// Items de los cuales se componen la cotizacion
        /// </summary>
        //public List<Item> listItems { get; set; }
        public virtual ICollection<Item> items { get; set; }

        public void AddItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Un item en una cotizacion no puede ser NULL");
            }
            items.Add(item);
        }

        /// <summary>
        /// Lugar en el donde se guarda el archivo txt
        /// </summary>
        public byte[] archPDF { get; set; }

        public override void Validate()
        {
            if (String.IsNullOrEmpty(rutCliente))
            {
                throw new ModelException("Rut no puede ser null");
            }
            
            if (String.IsNullOrEmpty(rutUsuarioCreador))
            {
                throw new ModelException("Rut no puede ser null");
            }
            
            // Validacion del RUT
            Models.Validate.ValidarRut(rutCliente);
            Models.Validate.ValidarRut(rutUsuarioCreador);
        }












    }
}