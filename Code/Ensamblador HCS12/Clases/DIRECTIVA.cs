using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class DIRECTIVA
    {

        Boolean existe,valida;
        String imprimirMensaje,valueHex,codigoMaquina;
        int size;

        public DIRECTIVA(String directive,OPERANDO operando,LABEL label)
        {
            baseNumerica base1 = new baseNumerica(operando.getOperando());
            //base1.convert2Dec();
            codigoMaquina = "";
            valida = true;
            size = 0;
            valueHex = "";
            if (!base1.getValido() && directive != "FCC" && directive != "END")
            {
                imprimirMensaje = "[Error]El operando contiene valores que no son validos en su base";
                valida = false;
            }
            if (operando.getOperando() == "NULL" && directive != "END")
            {
                imprimirMensaje = "[Error]La Directiva necesita operando";
                valida = false;
            }
            switch (directive)
            {
                case "ORG":
                    existe = true;
                    if (!(base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 65535))
                    {
                       imprimirMensaje = "[Error]Valor invalido para " + directive + " los valores van de 0-65535";
                       valida = false;
                    }
                    break;
                case "END":
                    existe = true;
                    if(operando.getOperando()!="NULL")
                    {
                        imprimirMensaje = "[Error]END no debe llevar operando";
                        valida = false;
                    }
                    break;
                case "DB":
                case "DC.B":
                case "FCB":
                    existe = true;
                    if (base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 255)
                    {
                        size = 1;
                        try
                        {
                            codigoMaquina = base1.getNumberHexadecimal().PadLeft(2, '0');
                        }
                        catch(Exception ex)
                        {
                            imprimirMensaje = "[Error] Valores invalidos para la base";
                            valida = false;
                        }
                    }
                    else
                    {
                        imprimirMensaje = "[Error]Valor invalido para " + directive + " los valores van de 0-255";
                        valida = false;
                    }
                    break;
                case "DW":
                case "DC.W":
                case "FDB":
                    existe = true;
                    if (base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 65535)
                    {
                        size = 2;
                        try
                        {
                            codigoMaquina = base1.getNumberHexadecimal().PadLeft(4, '0');
                        }
                        catch(Exception ex)
                        {
                            imprimirMensaje = "[Error] Valores invalidos para la base";
                            valida = false;
                        }
                    }
                    else
                    {
                        imprimirMensaje = "[Error]Valor invalido para " + directive + " los valores van de 0-65535";
                        valida = false;
                    }
                    break;
                case "FCC":
                    existe = true;
                    if ((@operando.getOperando()[0] == '"' && @operando.getOperando()[operando.getOperando().Length - 1] == '"') && operando.getOperando().Length >= 2)
                    {
                        size = operando.getOperando().Length - 2;
                        baseNumerica b2 = new baseNumerica("0");
                        for (int i = 1; i<operando.getOriginal().Length-1;i++ )
                        {
                            int tmpIntFCC = 0;
                            tmpIntFCC = System.Convert.ToInt16(operando.getOriginal()[i]);
                            b2 = new baseNumerica(System.Convert.ToString(tmpIntFCC));
                            codigoMaquina += b2.getNumberHexadecimal();
                        }
                    }
                    else
                    {
                        imprimirMensaje = "[Error]El operando no tiene ambas comillas";
                        valida = false;
                    }
                    break;
                case "DS":
                case "DS.B":
                case "RMB":
                    existe = true;
                    if (base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 65535)
                    {
                        size = 1 * base1.getNumberDecimal();
                    }
                    else
                    {
                        imprimirMensaje = "[Error]Valor invalido para " + directive + " los valores van de 0-65535";
                        valida = false;
                    }
                    break;
                case "DS.W":
                case "RMW":
                    existe = true;
                    if (base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 65535)
                    {
                        size = 2 * base1.getNumberDecimal();
                    }
                    else
                    {
                        imprimirMensaje = "[Error]Valor invalido para " + directive + " los valores van de 0-65535";
                        valida = false;
                    }
                    break;
                case "EQU":
                    existe = true;

                    if(label.getLabel() == "NULL")
                    {
                        imprimirMensaje = "[Error]EQU debe tener etiqueta";
                        valida = false;
                    }
                    else if (!(base1.getNumberDecimal() >= 0 && base1.getNumberDecimal() <= 65535))
                    {
                        imprimirMensaje += "[Error]Valor invalido para " + directive + " los valores van de 0-65535";
                        valida = false;
                    }
                    else
                    {
                        baseNumerica b1 = new baseNumerica(operando.getOperando());
                        valueHex = b1.getNumberHexadecimal();
                    }
                    break;
                default:
                    existe = valida = false;
                    break;
            }//End switch
        }
        ~DIRECTIVA()
        {
        }

        public Boolean getExiste()
        {
            return existe;
        }
        public Boolean getValida()
        {
            return valida;
        }
        public String getImprimirMensaje()
        {
            return imprimirMensaje;
        }
        public String getValueHexadecimal()
        {
            return valueHex;
        }
        public String getCodigoMaquina()
        {
            return codigoMaquina;
        }
        public int getSize()
        {
            return size;
        }
    }//Fin Class DIRECTIVA
}
