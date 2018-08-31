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
        public int id { get; set; }
        
        /// <summary>
        /// Rut del cliente asociado a la cotizacion.
        /// </summary>
        public string rut_cliente { get; set; }
        
        /// <summary>
        /// fecha la cual se realiaza la cotizacion.
        /// </summary>
        public string fecha_creacion { get; set; }
        
        /// <summary>
        /// Rut del usuario creador de la cotizacion.
        /// </summary>
        public string rut_Usuario_Creador { get; set; }
        
        /// <summary>
        /// Items de los cuales se componen la cotizacion
        /// </summary>
        public List<Item> listItems { get; set; }

        /// <summary>
        /// Lugar en el donde se guarda el archivo txt
        /// </summary>
        public byte[] archPDF { get; set; }

        public override void Validate()
        {
        }












    }
}