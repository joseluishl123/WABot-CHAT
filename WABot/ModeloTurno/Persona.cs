using System;
using System.Collections.Generic;

#nullable disable

namespace WABot.ModeloTurno
{
    public partial class Persona
    {
        public Persona()
        {
            Turnos = new HashSet<Turno>();
        }

        public int PerId { get; set; }
        public string PerIdentificacion { get; set; }
        public string PerNombre { get; set; }
        public string PerTelefono { get; set; }
        public string PerDireccion { get; set; }

        public virtual ICollection<Turno> Turnos { get; set; }
    }
}
