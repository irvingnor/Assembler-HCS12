using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class S9
    {
        #region "Atributos"
        private String _tipo;
        private String _longitud;
        private String _direccion;
        private String _crc;
        #endregion

        #region "Propiedades"

        #endregion

        #region "Metodos"
        #region "Constructores"
        public S9()
        {
            _tipo = "S9";
            _longitud = "03";
            _direccion = "0000";
            _crc = "FC";
        }
        #endregion
        #region "Publicos"
        public void saveFile(System.IO.FileStream fs)
        {
            Byte[] info;
            info = new System.Text.UTF8Encoding(true).GetBytes(_tipo + _longitud + _direccion + _crc + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10));
            fs.Write(info, 0, info.Length);
        }
        #endregion
        #region "Privados"
        #endregion
        #endregion
    }
}
