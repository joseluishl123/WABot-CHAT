using System;
using System.Collections.Generic;

#nullable disable

namespace WABot.Models
{
    public partial class Paciente
    {
        public int PacId { get; set; }
        public string PacTipoIdentificacion { get; set; }
        public string PacIdentificacion { get; set; }
        public string PacNombre1 { get; set; }
        public string PacNombre2 { get; set; }
        public string PacApellido1 { get; set; }
        public string PacApellido2 { get; set; }
        public DateTime? PacFechaNacimiento { get; set; }
        public string PacCodGenero { get; set; }
        public string PacCodDepto { get; set; }
        public string PacCodCiudad { get; set; }
        public string PacDireccion { get; set; }
        public int? PacCodNivelEducativo { get; set; }
        public string PacCodProfesion { get; set; }
        public int? PacTipoSangre { get; set; }
        public int? PacEstadoCivil { get; set; }
        public string PacTelefono { get; set; }
        public byte[] PacFoto { get; set; }
        public byte[] PacHuella { get; set; }
        public byte[] PacFirma { get; set; }
        public int? PacDominanciaCodigo { get; set; }
        public DateTime? PacFecha { get; set; }
    }
}
