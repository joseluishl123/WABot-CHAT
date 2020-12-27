using System;
using System.Collections.Generic;

#nullable disable

namespace WABot.ModeloTurno
{
    public partial class Estado
    {
        public Estado()
        {
            Turnos = new HashSet<Turno>();
        }

        public int EstId { get; set; }
        public string EstDescripcion { get; set; }

        public virtual ICollection<Turno> Turnos { get; set; }
    }
}
