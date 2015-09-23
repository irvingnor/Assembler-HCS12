using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Ensamblador_HCS12
{
    public class CODOP
    {
        public string codop;
        public bool necesitaOperando;
        public string modoDireccionamiento;
        public string codigoMaquina;
        public int bytesCalculados;
        public int bytesPorCalcular;
        public int bytesTotales;
        public Boolean existe;
        public Boolean valido;
        public Boolean encontrado;
        int iniciaCODOP;
        Boolean[] flags = new Boolean[8];//[0]Coma[1]Corchete[2]Operando[3]Signo +[4]Signo -[5]Guardo[6]#[7]RegistroPre/Post
        String mensajeResultado;

        public CODOP()
        {
        }
        public CODOP(String codop)
        {
            this.codop = codop.ToUpper();
            mensajeResultado = "";
            bytesPorCalcular = 0;
        }

         ~CODOP()
        {
        }

        public void setCODOP(string codop, bool operando, string modoDireccionamiento, string codigoMaquina,
                     int bytesCalculados, int bytesPorCalcular, int bytesTotales)
        {
            this.codop = codop;
            this.necesitaOperando = operando;
            this.modoDireccionamiento = modoDireccionamiento;
            this.codigoMaquina = codigoMaquina;
            this.bytesCalculados = bytesCalculados;
            this.bytesPorCalcular = bytesPorCalcular;
            this.bytesTotales = bytesTotales;
        }


        public int calculaBytes(string codigoM)
        {
            return codigoM.Length / 2;
        }
        public void validaCODOP()
        {
            valido = Regex.IsMatch(codop, @"^[a-zA-Z]+(\.)?[a-zA-Z]+$") && codop.Length <= 5;
        }
        public void revizaModoDireccionamiento(CODOP[] arrayCODOP, int x1, int x2, OPERANDO operando)
        {
            checkNedOperando(arrayCODOP,x1,x2);
            for (int i = 0; i<flags.Length;i++ )
            {
                flags[0] = false;
            }
            int Tmpx1 = x1;
            for (int i = 0; i < operando.getSize(); i++)
            {
                if (operando.getOperando()[i] == ',')
                {
                    flags[0] = true;
                }
                else if (operando.getOperando()[i] == '[' || operando.getOperando()[i] == ']')
                {
                    flags[1] = true;
                }
                else if (operando.getOperando()[i] == '+' && flags[0])
                {
                    flags[2] = true;
                    flags[3] = true;
                }
                else if (operando.getOperando()[i] == '-' && flags[0])
                {
                    flags[4] = true;
                    flags[2] = true;
                }
                else if (operando.getOperando()[i] == '#')
                {
                    flags[6] = true;
                }
                else if(operando.getOperando()[i] >= 'A' && operando.getOperando()[i] <= 'Z' && flags[0])
                {
                    flags[7] = true;
                }
            }
            //existe = false;
            for (; x1 < x2 && !flags[5] && !encontrado; x1++)
            {
                //flags[2] = necesitaOperando;
                //string modoDireccionamiento = Convert.ToString(arrayCODOP[x1].getModoDireccionamiento());
                try
                {
                    if (!encontrado)
                    {
                        flags[2] = necesitaOperando;
                        string modoDireccionamiento = Convert.ToString(arrayCODOP[x1].getModoDireccionamiento());
                        switch (modoDireccionamiento)
                        {
                            case "INH":
                                if (!flags[0])
                                    validaInh(operando.getOperando());
                                break;
                            case "IMM":
                                if (!flags[0])
                                    validaInm(operando.getOperando());
                                break;
                            case "DIR":
                            case "EXT":
                                if (!flags[0])
                                    validaDir(operando.getOperando());
                                break;
                            case "REL":
                                if (!flags[0])
                                    validaRel(operando.getOperando());
                                break;
                            case "IDX":
                            case "IDX1":
                            case "IDX2":
                            case "[IDX2]":
                            case "[D,IDX]":
                                validaIdx(operando.getOperando());
                                break;
                        }//Finaliza switch
                        //checkTam(arrayCODOP, Tmpx1, x2, modoDireccionamiento);
                    }//Fin de la condicional para saber si ya se encontro
                    //else
                    //{
                    //    checkTam(arrayCODOP, Tmpx1, x2, modoDireccionamiento);
                    //}
                }//Fin del try
                catch (Exception ex)
                {
                    mensajeResultado = "[Error] El operando no esta completo";
                    flags[5] = false;
                }
            }//Fin bucle for
            if (!encontrado)
            {
                Boolean flagSigno = flags[3] || flags[4];
                if ((operando.getOperando()[0] == 'A' || operando.getOperando()[0] == 'B' || operando.getOperando()[0] == 'D') && flags[0])
                {
                    mensajeResultado = "[Error] Al validar Indizado de acumulador, las reglas son (A|B|D) seguido de (,) y seguido de (X|Y|SP|PC)";
                }
                else if ((flags[7] && flagSigno) || flagSigno)
                {
                    mensajeResultado = "[Error] Al validar pre/post incremento/decremento, las reglas son valores del 1 al 8 una (,) y seguido de un + o - con algun registro como X,Y o SP o bien un registro ed los anteriores seguido de un + o -";
                }
                else if ((flagSigno && flags[0]))
                {
                    mensajeResultado = "[Error] Al validar Indexado, las reglas son valores del -16 al 65535 una (,) y seguido de un registro valido(X,Y,SP,PC) ";
                }
                else if (flags[1])
                {
                    mensajeResultado = "[Error] Al validar Indexado indirecto o indexado de acumulador indirecto";
                }
                else if (flags[6])
                {
                    mensajeResultado = "[Error] Al validar Inmediato. Sintaxis: #(%|@|$ o niguna base) seguido de un numero valido para la base, el rango de valores es 0 a 65535";
                }
                else if (flags[0])
                {
                    //Validar caso especial de Indexado
                    if (operando.getOperando()[0] == ',' && operando.getOperando().Length > 1 && operando.getOperando().Length <= 3)
                    {
                        if (operando.getOperando().Length == 2)
                        {
                            if ((operando.getOperando()[1] == 'X' || operando.getOperando()[1] == 'Y'))
                            {
                                //Caso especial del indizado de 5 bits
                                mensajeResultado = "Indizado de 5 bits";
                                modoDireccionamiento = "IDX";
                                bytesTotales = 2;
                                codigoMaquina = "A6";
                                encontrado = true;
                            }
                            else
                            {
                                mensajeResultado = "[Error] Al validar Indexado, las reglas son valores del -16 al 65535 una (,) y seguido de un registro valido(X,Y,SP,PC) ";
                            }
                        }
                        else
                        {
                            if (((operando.getOperando()[1] == 'S' && operando.getOperando()[2] == 'P') || (operando.getOperando()[1] == 'P' && operando.getOperando()[2] == 'C')))
                            {
                                //Caso especial del indizado de 5 bits
                                mensajeResultado = "Indizado de 5 bits";
                                modoDireccionamiento = "IDX";
                                bytesTotales = 2;
                                codigoMaquina = "A6";
                            }
                            else
                            {
                                mensajeResultado = "[Error] Al validar Indexado, las reglas son valores del -16 al 65535 una (,) y seguido de un registro valido(X,Y,SP,PC) ";
                            }
                        }
                    }
                    else
                    {
                        mensajeResultado = "[Error] Al validar Indexado, las reglas son valores del -16 al 65535 una (,) y seguido de un registro valido(X,Y,SP,PC) ";
                    }
                }
            }
            else
            {
                checkTam(arrayCODOP, Tmpx1, x2, modoDireccionamiento);
            }
        }//Fin RevizaModoDireccionamiento

        //<ModosDireccionamiento>
        public void validaInh(String operando)
        {
            if (operando == "NULL")
            {
                mensajeResultado = "Inherente";
                modoDireccionamiento = "INH";
                encontrado = true;
            }
            else
            {
                encontrado = false;
            }
        }
        public void validaInm(String operando)
        {
            if (operando[0] == '#')
            {
                int i = 1;
                String tmp = "";
                int tam = operando.Length;
                if (operando[1] == '@' || operando[1] == '%' || operando[1] == '$')
                {
                    i = 2;
                }
                for (; i < tam; i++)
                {
                    tmp += operando[i];
                }
                int num = Convert.ToInt32(tmp);
                if (num >= 0 && num <= 65535)
                {
                    mensajeResultado = "Inmediato";
                    modoDireccionamiento = "IMM";
                    encontrado = true;
                    return;
                }
                else
                {
                    mensajeResultado = "[Error] Inmediato fuera de rango";
                    encontrado = false;
                    return;
                    //Fuera de rango
                }
            }
            encontrado = false;
        }
        public void validaDir(String operando)
        {
            //Validar para cuando es extendido por etiqueta
            switch (operando[0])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '@':
                case '%':
                case '$':
                case '-':
                case '+':

                    break;
                case '#':
                    encontrado = false;
                    return;
                    break;
                case '[':
                    encontrado = false;
                    return;
                    break;
                default:
                    LABEL etiqueta = new LABEL(operando);
                    etiqueta.validaEtiqueta();
                    if (etiqueta.getValida())
                    {
                        mensajeResultado = "Extendido";
                        modoDireccionamiento = "EXT";
                        encontrado = true;
                        return;
                    }
                    else
                    {
                        mensajeResultado = "Etiqueta no valida para extendido";
                        encontrado = false;
                        return;
                        //Etiqueta no valida Error
                    }
                    break;
            }
            if (operando.Length == 1 && !isDigit(operando[0]))
            {
                mensajeResultado = "Error en directo: Sintaxis (%|@|$ o ninguna base) y despues valores correspondientes a la base";
                encontrado = false;
                return;
            }
            baseNumerica b1 = new baseNumerica(operando);
            //b1.convert2Dec();
            if(!b1.getValido())
            {
                String menbase = "";
                String menVal = "";
                switch(operando[0])
                {
                    case '%':
                        menbase = "Binario";
                        menVal = "1 - 0";
                        break;
                    case '@':
                        menbase = "Octal";
                        menVal = "0 - 7";
                        break;
                    case '$':
                        menbase = "Hexadecimal";
                        menVal = "0-9 o las letras de la A - F";
                        break;
                    default:
                        menbase = "Decimal";
                        menVal = "0 - 9";
                        break;
                }
                mensajeResultado = "Valores invalidos para la base." + menbase + " solo acepta los digitos del " + menVal;
                encontrado = false;
                return;
            }
            if (b1.getNumberDecimal() >= 0 && b1.getNumberDecimal() <= 255)
            {
                mensajeResultado = "Directo";
                modoDireccionamiento = "DIR";
                encontrado = true;
                return;
            }
            else if (b1.getNumberDecimal() >= 256 && b1.getNumberDecimal() <= 65535)
            {
                mensajeResultado = "Extendido";
                modoDireccionamiento = "EXT";
                encontrado = true;
                return;
            }
            else
            {
                mensajeResultado = "Fuera de rango en Extendido";
                encontrado = false;
                return;
                //Fuera de rango
            }
        }
        public void validaRel(String operando)
        {
            LABEL etiqueta = new LABEL(operando);
            etiqueta.validaEtiqueta();
            if (etiqueta.getValida())
            {
               mensajeResultado = "Relativo";
               modoDireccionamiento = "REL";
               encontrado = true;
               return;
            }
            else
            {
                mensajeResultado = "Etiqueta no valida para Relativo";
                encontrado = false;
                return;
                //Relativo no valido
            }
        }
        public void validaIdx(String operando)
        {
            switch (operando[0])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    validaIdx1(operando);
                    break;
                case 'A':
                case 'B':
                case 'D':
                    validaIdx2(operando);
                    break;
                case '[':
                    validaIdx3(operando);
                    break;
            }
        }
        public void validaIdx1(String operando)
        {
            int[,] states = new int[6, 5]{
                //d     |   ,   |   Reg     |   +   |   -   
                {0,         1,      6,          6,      6},//0
                {6,         6,      2,          3,      3},//1
                {6,         6,      6,          4,      4},//2
                {6,         6,      5,          6,      6},//3
                {6,         6,      6,          6,      6},//4
                {6,         6,      6,          6,      6}//5
            };
            int state = 0, input = 0, sizeOp = operando.Length, i = 0;
            bool flagNegativo = false;
            if (operando[0] == '-')
            {
                flagNegativo = true;
                i++;
            }
            String num = "";
            Boolean registroPC = false;
            while (i < sizeOp && state <= 5)
            {
                switch (operando[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        input = 0;
                        break;
                    case ',':
                        input = 1;
                        break;
                    case 'X':
                    case 'Y':
                        input = 2;
                        break;
                    case 'S':
                        if (operando[i + 1] == 'P')
                        {
                            input = 2;
                            i++;
                        }
                        break;
                    case 'P':
                        if (operando[i + 1] == 'C')
                        {
                            input = 2;
                            i++;
                            registroPC = true;
                        }
                        break;
                    case '+':
                        input = 3;
                        break;
                    case '-':
                        input = 4;
                        break;
                }
                state = states[state, input];
                switch (state)
                {
                    case 0:
                        num += operando[i];
                        break;
                }
                i++;
            }//Fin del while
            if (state == 2)
            {
                int tmp = Convert.ToInt32(num);
                if (flagNegativo)
                {
                    tmp *= -1;
                }
                //Todo salio bien
                //Validar de que tipo es
                if (tmp >= -16 && tmp <= 15)
                {
                    mensajeResultado = "Indizado de 5 bits";
                    modoDireccionamiento = "IDX";
                    encontrado = true;
                    return;
                }
                else if ((tmp >= -256 && tmp <= -17) || (tmp >= 16 && tmp <= 255))
                {
                    mensajeResultado = "Indizado de 9 bits";
                    modoDireccionamiento = "IDX1";
                    encontrado = true;
                    return;
                }
                else if (tmp >= 256 && tmp <= 65535)
                {
                    mensajeResultado = "Indizado de 16 bits";
                    modoDireccionamiento = "IDX2";
                    encontrado = true;
                    return;
                }
                else
                {
                    //Fuera de rango Indizado 16 bits
                    mensajeResultado = "Indizado de 16 bits fuera de rango";
                    encontrado = false;
                    return;
                }
            }
            else if ((state == 4 || state == 5) && !registroPC)
            {
                int tmp = Convert.ToInt32(num);
                String modoTmp = "";
                if (state == 4)
                {
                    modoTmp = "Post";
                }
                else
                {
                    modoTmp = "Pre";
                }
                if (flags[4])
                {
                    modoTmp += " decremento ";
                }
                else
                {
                    modoTmp += " incremento ";
                }
                if (flagNegativo)
                {
                    tmp *= -1;
                }
                if (tmp >= 1 && tmp <= 8)
                {
                    mensajeResultado = "Indizado " + modoTmp;
                    modoDireccionamiento = "IDX";
                    encontrado = true;
                    return;
                }
                else
                {
                    //Fuera de rango indizado Pre/post
                    mensajeResultado = "Indizado" + modoTmp + "fuera de rango";
                    encontrado = false;
                    return;
                }
            }
            else
            {
                //Algun error
                encontrado = false;
            }
        }
        public void validaIdx2(String operando)
        {
            int[,] states = new int[3, 2]{
            //, |   Reg    
            {1,     3},//0
            {3,     2},//1
            {3,     3}//2
            };
            int state = 0, input = 0, sizeOp = operando.Length, i = 1;
            while (i < sizeOp && state <= 2)
            {
                switch (operando[i])
                {
                    case ',':
                        input = 0;
                        break;
                    case 'X':
                    case 'Y':
                        input = 1;
                        break;
                    case 'S':
                        if (operando[i + 1] == 'P')
                        {
                            input = 1;
                            i++;
                        }
                        break;
                    case 'P':
                        if (operando[i + 1] == 'C')
                        {
                            input = 1;
                            i++;
                        }
                        break;
                }
                state = states[state, input];
                i++;
            }//Fin del while
            if (state == 2)
            {
                mensajeResultado = "Indexado de Acumulador";
                modoDireccionamiento = "IDX";
                encontrado = true;
                return;
            }
            else
            {
                encontrado = false;
                return;
                //Algun error
            }
        }
        public void validaIdx3(String operando)
        {
            int[,] states = new int[9, 5]{
                //d     |   D   |   ,   |   Reg |   ]
                {1,         5,      9,      9,      9},//0
                {1,         9,      2,      9,      9},//1
                {9,         9,      9,      3,      9},//2
                {9,         9,      9,      9,      4},//3
                {9,         9,      9,      9,      9},//4
                {9,         9,      6,      9,      9},//5
                {9,         9,      9,      7,      9},//6
                {9,         9,      9,      9,      8},//7
                {9,         9,      9,      9,      9}//8
            };
            int state = 0, input = 0, sizeOp = operando.Length, i = 1;
            String num = "";
            while (i < sizeOp && state <= 8)
            {
                switch (operando[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        input = 0;
                        break;
                    case 'D':
                        input = 1;
                        break;
                    case ',':
                        input = 2;
                        break;
                    case 'X':
                    case 'Y':
                        input = 3;
                        break;
                    case 'S':
                        if (operando[i + 1] == 'P')
                        {
                            input = 3;
                            i++;
                        }
                        break;
                    case 'P':
                        if (operando[i + 1] == 'C')
                        {
                            input = 3;
                            i++;
                        }
                        break;
                    case ']':
                        input = 4;
                        break;
                }
                state = states[state, input];
                switch (state)
                {
                    case 1:
                        num += operando[i];
                        break;
                }
                i++;
            }//Fin del while
            if (state == 4)
            {
                int tmp = Convert.ToInt32(num);
                if (tmp >= 0 && tmp <= 65535)
                {
                    mensajeResultado = "Indirecto 16 bits";
                    modoDireccionamiento = "[IDX2]";
                    encontrado = true;
                    return;
                }
                else
                {
                    mensajeResultado = "Indirecto de 16 bits fuera de rango";
                    encontrado = false;
                    return;
                    //Fuera de rango
                }
            }
            else if (state == 8)
            {
                mensajeResultado = "Acumulador Indirecto";
                modoDireccionamiento = "[D,IDX]";
                encontrado = true;
                return;
            }
            else
            {
                encontrado = false;
                return;
                //Algun error
            }
        }
        //</ModosDireccionamiento>

        public string getCodop()
        {
            return this.codop;
        }
        public bool getNecesitaOperando()
        {
            return this.necesitaOperando;
        }
        public string getModoDireccionamiento()
        {
            return this.modoDireccionamiento;
        }
        public string getCodigoMaquina()
        {
            return this.codigoMaquina;
        }
        public int getBytesCalculados()
        {
            return this.bytesCalculados;
        }
        public int getBytesPorCalcular()
        {
            return this.bytesPorCalcular;
        }
        public int getBytesTotales()
        {
            return this.bytesTotales;
        }
        public Boolean getValido()
        {
            return valido;
        }
        public Boolean getExiste()
        {
            return existe;
        }
        public Boolean getEncontrado()
        {
            return encontrado;
        }
        public String getMensajeResultado()
        {
            return mensajeResultado;
        }
        public int getIniciaCODOP()
        {
            return iniciaCODOP;
        }

        public void setIniciaCODOP(int iniciaCODOP)
        {
            this.iniciaCODOP = iniciaCODOP;
            existe = true;
        }
        public void setNoExiste()
        {
            existe = false;
        }

        public bool isDigit(char c)
        {
            return (System.Convert.ToInt16(c) >= 48 && System.Convert.ToInt16(c) <= 57);
        }
        public void checkTam(CODOP[] arrayCODOP,int x1, int x2, String modo)
        {
            bool found = false;
            for (; x1 < x2 && !found; x1++)
            {
                if (Convert.ToString(arrayCODOP[x1].getModoDireccionamiento()) == modoDireccionamiento)
                {
                    bytesTotales = arrayCODOP[x1].getBytesTotales();
                    this.codigoMaquina = arrayCODOP[x1].getCodigoMaquina();
                    this.bytesPorCalcular = arrayCODOP[x1].getBytesPorCalcular();
                    this.bytesCalculados = arrayCODOP[x1].getBytesCalculados();
                    found = true;
                }
            }
        }//Finaliza checkModo
        public void checkNedOperando(CODOP[] arrayCODOP, int x1,int x2)
        {
            if (arrayCODOP[x1].getNecesitaOperando())
            {
                this.necesitaOperando = true;
            }
        }
    }
}
