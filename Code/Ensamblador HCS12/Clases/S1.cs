using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class S1
    {
        #region "Atributos"
        private String _tipo;
        private String _longitud;
        private String _direccion;
        private String _codMaquina;
        private String _crc;
        #endregion

        #region "Propiedades"
        public String tipo
        {
            get
            {
                return _tipo;
            }
            set
            {
                _tipo = value;
            }
        }
        public String longitud
        {
            get
            {
                return _longitud;
            }
            set
            {
                _longitud = value.PadLeft(2,'0');
            }
        }
        public String direccion
        {
            get
            {
                return _direccion;
            }
            set
            {
                _direccion = value.PadLeft(4,'0');
            }
        }
        public String codMaquina
        {
            get
            {
                return _codMaquina;
            }
            set
            {
                _codMaquina = value;
            }
        }
        public String crc
        {
            get
            {
                return _crc;
            }
            set
            {
                _crc = value;
            }
        }
        #endregion

        #region "Metodos"
        #region "Constructores"
        public S1()
        {
            _tipo = "S1";
            _longitud = "";
            _direccion = "0000";
            _codMaquina = "";
            _crc = "";
        }
        #endregion

        #region "Publicos"
        public void addCodigoMaquina(String codigo)
        {
            //for (int i = 0; i < codigo.Length; i += 2)
            //{
            //    String tmp = System.Convert.ToString(codigo[i]) + System.Convert.ToString(codigo[i + 1]);
            //    baseNumerica b1 = new baseNumerica(tmp);
            //    _codMaquina += b1.getNumberHexadecimal();
            //}
            _codMaquina += codigo;
        }
        public void saveFile(System.IO.FileStream fs)
        {
            Byte[] info;
            info = new System.Text.UTF8Encoding(true).GetBytes(_tipo + _longitud + _direccion + _codMaquina + _crc + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10));
            fs.Write(info, 0, info.Length);
        }
        public void calculaLongitud()
        {
            baseNumerica b1 = new baseNumerica(System.Convert.ToString(3 + (_codMaquina.Length / 2)));
            _longitud = (b1.getNumberHexadecimal()).PadLeft(2,'0');
        }
        public void calculaCRC()
        {
            baseNumerica b1 = new baseNumerica("0000");
            b1 = new baseNumerica("$"+_longitud);
            for (int i = 0; i<4;i+=2 )
            {
                String tmp = System.Convert.ToString(_direccion[i]) + System.Convert.ToString(_direccion[i+1]);
                baseNumerica tmpNum = new baseNumerica("$"+tmp);
                b1 = new baseNumerica(System.Convert.ToString(b1.getNumberDecimal()+tmpNum.getNumberDecimal()));
            }
            for (int i = 0; i < _codMaquina.Length; i += 2)
            {
                String tmp = System.Convert.ToString(_codMaquina[i]) + System.Convert.ToString(_codMaquina[i + 1]);
                baseNumerica b2 = new baseNumerica("$"+tmp);
                b1 = new baseNumerica(System.Convert.ToString(b1.getNumberDecimal() + b2.getNumberDecimal()));
            }
            baseNumerica max = new baseNumerica("$FFFF");
            b1 = new baseNumerica(System.Convert.ToString(max.getNumberDecimal()-b1.getNumberDecimal()));
            for (int i = b1.getNumberHexadecimal().Length - 1; i > b1.getNumberHexadecimal().Length - 3; i--)
            {
                _crc = b1.getNumberHexadecimal()[i] + _crc;
            }
        }
        #endregion

        #region "Privados"
        #endregion
        #endregion
    }
}
