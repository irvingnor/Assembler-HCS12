using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class lineFile
    {
        String value1, label1, codop, operando;
        String label, value; 

        public void save(System.IO.FileStream fs,int file)
        {
            Byte[] info;
            switch(file)
            {
                case 1:
                    info = new System.Text.UTF8Encoding(true).GetBytes(value1 + "\t" + label1 + "\t" + codop + "\t" + operando + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10));
                    fs.Write(info, 0, info.Length);
                break;
                case 2:
                  info = new System.Text.UTF8Encoding(true).GetBytes(label + "\t" + value + (char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10)));
                  fs.Write(info, 0, info.Length);
                break;
            }
        }

        public void setValue1(String val)
        {
            value1 = val;
        }
        public void setLabel1(String val)
        {
            label1 = val;
        }
        public void setCODOP(String val)
        {
            codop = val;
        }
        public void setOperando(String val)
        {
            operando = val;
        }
        public void setLabel(String val)
        {
            label = val;
        }
        public void setValue(String val)
        {
            value = val;
        }

        public String getValue1()
        {
            return value1;
        }
        public String getLabel1()
        {
            return label1;
        }
        public String getCodop()
        {
            return codop;
        }
        public String getOperando()
        {
            return operando;
        }
        public String getLabel()
        {
            return label;
        }
        public String getValue()
        {
            return label;
        }
    }
}
