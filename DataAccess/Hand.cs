//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hand
    {
        public int idHand { get; set; }
        public Nullable<int> idCard { get; set; }
        public int idUser { get; set; }
        public int quantity { get; set; }
    
        public virtual Cards Cards { get; set; }
        public virtual Users Users { get; set; }
    }
}
