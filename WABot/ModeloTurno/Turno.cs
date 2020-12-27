using System;
using System.Collections.Generic;

#nullable disable

namespace WABot.ModeloTurno
{
    public partial class Turno
    {
        public int TurnId { get; set; }
        public string TurnCodigo { get; set; }
        public int? TurnIdPaciente { get; set; }
        public string TurnFecha { get; set; }
        public int? TurnEstado { get; set; }

        public virtual Estado TurnEstadoNavigation { get; set; }
        public virtual Persona TurnIdPacienteNavigation { get; set; }
    }
}
