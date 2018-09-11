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
        public string RutCliente { get; set; }
        
        /// <summary>
        /// fecha la cual se realiaza la cotizacion.
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        
        /// <summary>
        /// Rut del usuario creador de la cotizacion.
        /// </summary>
        public string RutUsuarioCreador { get; set; }
        
        /// <summary>
        /// Items de los cuales se componen la cotizacion
        /// </summary>
        //public List<Item> listItems { get; set; }
        public IList<Item> Items { get; set; }



        /// <summary>
        /// Lugar en el donde se guarda el archivo txt
        /// </summary>
        public byte[] archPDF { get; set; }

        public override void Validate()
        {
            if (String.IsNullOrEmpty(RutCliente))
            {
                throw new ModelException("Rut no puede ser null");
            }
            
            if (String.IsNullOrEmpty(RutUsuarioCreador))
            {
                throw new ModelException("Rut no puede ser null");
            }

            if (DateTime.Compare(DateTime.Now, FechaCreacion) < 0)
            {
                throw new ModelException("La fecha no puede ser en el futuro");
            }
            
            // Validacion del RUT
            Models.Validate.ValidarRut(RutCliente);
            Models.Validate.ValidarRut(RutUsuarioCreador);
        }












    }
}