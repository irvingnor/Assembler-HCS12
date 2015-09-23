using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Ensamblador_HCS12
{
    public class LABEL
    {
        String label;
        Boolean valida;

        public LABEL(String label)
        {
            this.label = label;
        }

        public void validaEtiqueta()
        {
            valida = Regex.IsMatch(label, @"^[a-zA-Z][\w]*$") && label.Length <= 8;
        }

        public String getLabel()
        {
            return label;
        }
        public Boolean getValida()
        {
            return valida;
        }
    }
}
