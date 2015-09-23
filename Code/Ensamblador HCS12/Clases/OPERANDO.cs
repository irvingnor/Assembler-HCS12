using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ensamblador_HCS12
{
    public class OPERANDO
    {
        String operando;
        String original;
        Boolean valido;
        Boolean fueraRango;
        String xb;
        int value,tam;

        public OPERANDO(String operando)
        {
            this.operando = operando.ToUpper();
            original = operando;
        }
        
        public void validaOperando()
        {
            valido = true;
        }
        public void generaXB(CODOP codop)
        {
            String xb1="", xb2="",s1="",s2="";
            tam = operando.Length;
            int i;
            for (i = 0; i < tam && operando[i]!=',';i++ )
            {
                s1 += operando[i];
            }
            for (i+=1; i < tam; i++)
            {
                s2 += operando[i];
            }
            xb1 = "%";
            xb2 = "%";
            Boolean band = false;
            baseNumerica b1 = new baseNumerica("0");
            switch(codop.getModoDireccionamiento())
            {
                case "IDX":
                    baseNumerica b2 = new baseNumerica("0") ,b3 = new baseNumerica("0");
                    if(s2[0]=='+' || s2[0] == '-' || s2[s2.Length-1] == '+' || s2[s2.Length-1] == '-')
                    {
                        xb += "";
                        String tmpReg = "";
                        for (int j = 0; j<s2.Length;j++ )
                        {
                            if(s2[j] != '+' && s2[j] != '-')
                            {
                                tmpReg += s2[j];
                            }
                        }
                        switch(tmpReg)
                        {
                            case "X":
                                xb1 += "001";
                                break;
                            case "Y":
                                xb1 += "011";
                                break;
                            case "SP":
                                xb1 += "101";
                                break;
                            case "PC":
                                xb1 += "111";
                                break;
                        }
                        Boolean flagSigno = false;
                        if(s2[0]=='+')
                        {
                            xb1 += "0";
                        }
                        else if(s2[0]=='-')
                        {
                            xb1 += "0";
                            flagSigno = true;
                        }
                        else if(s2[s2.Length-1] == '+')
                        {
                            xb1 += "1";
                        }
                        else if(s2[s2.Length-1]=='-')
                        {
                            xb1 += "1";
                            flagSigno = true;
                        }

                        if (flagSigno)
                        {
                            switch(s1)
                            {
                                case "1":
                                    xb2 += "1111";
                                    break;
                                case "2":
                                    xb2 += "1110";
                                    break;
                                case "3":
                                    xb2 += "1101";
                                    break;
                                case "4":
                                    xb2 += "1100";
                                    break;
                                case "5":
                                    xb2 += "1011";
                                    break;
                                case "6":
                                    xb2 += "1010";
                                    break;
                                case "7":
                                    xb2 += "1001";
                                    break;
                                case "8":
                                    xb2 += "1000";
                                    break;
                            }//Termina switch
                        }//Termina parte cuando es negativo
                        else
                        {
                            switch (s1)
                            {
                                case "1":
                                    xb2 += "0000";
                                    break;
                                case "2":
                                    xb2 += "0001";
                                    break;
                                case "3":
                                    xb2 += "0010";
                                    break;
                                case "4":
                                    xb2 += "0011";
                                    break;
                                case "5":
                                    xb2 += "0100";
                                    break;
                                case "6":
                                    xb2 += "0101";
                                    break;
                                case "7":
                                    xb2 += "0110";
                                    break;
                                case "8":
                                    xb2 += "0111";
                                    break;
                            }
                        }//Termina cuando es positivo
                        b2 = new baseNumerica(xb1);
                        b3 = new baseNumerica(xb2);
                    }//Termina Indizado pre/post
                    else if(s1[0]=='A' || s1[0]=='B' || s1[0]=='D')
                    {
                        xb1 += "111";
                        switch(s2)
                        {
                            case "X":
                                xb1 += "0";
                                xb2 += "01";
                                break;
                            case "Y":
                                xb1 += "0";
                                xb2 += "11";
                                break;
                            case "SP":
                                xb1 += "1";
                                xb2 += "01";
                                break;
                            case "PC":
                                xb1 += "1";
                                xb2 += "11";
                                break;
                        }
                        switch(s1)
                        {
                            case "A":
                                xb2 += "00";
                                break;
                            case "B":
                                xb2 += "01";
                                break;
                            case "D":
                                xb2 += "10";
                                break;
                        }
                        b2 = new baseNumerica(xb1);
                        b3 = new baseNumerica(xb2);
                    }
                    else//Indizado 5 Bits
                    {
                    switch(s2)
                    {
                        case "X":
                            xb1 += "00";
                        break;
                        case "Y":
                            xb1 += "01";
                        break;
                        case "SP":
                            xb1 += "10";
                        break;
                        case "PC":
                            xb1 += "11";
                        break;
                    }
                    xb1 = xb1 + "0";


                    band = false;
                    b1 = new baseNumerica("0");
                    if(s1=="")
                    {
                        s1 = "0";
                    }
                    
                    if(s1[0]=='-')
                    {
                        char[] caracteres = {'-'};
                        s1 = s1.TrimStart(caracteres);
                        b1 = new baseNumerica(s1,1);
                        band = true;
                    }
                    else
                    {
                        b1 = new baseNumerica(s1);
                    }

                    String tmp="";
                    if(band)
                    {
                        String tmp2 = b1.getNumberBinary();
                        int tamTMP = tmp2.Length;
                        //for (int j = tamTMP -  1; j >= tamTMP - 4; j--)
                        for (int j = tamTMP - 4; j < tamTMP; j++)
                        {
                            tmp += tmp2[j];
                        }
                        xb1 += tmp2[tamTMP - 5];
                        for (int j = 0; j < tmp.Length; j++)
                        {
                            xb2 += tmp[j];
                        }
                    }
                    else
                    {
                        tmp = b1.getNumberBinary().PadLeft(5, '0');
                        xb1 += tmp[0];
                        for (int j = 1; j < tmp.Length; j++)
                        {
                            xb2 += tmp[j];
                        }
                    }
                    
                    b2 = new baseNumerica(xb1);
                    b3 = new baseNumerica(xb2);
                    }//Termina Indizado 5 bits

                    xb = b2.getNumberHexadecimal() + b3.getNumberHexadecimal();
                    break;//Termina IDX
                case "IDX1":
                    band = false;
                    if (s1[0] == '-')
                    {
                        char[] caracteres = { '-' };
                        s1 = s1.TrimStart(caracteres);
                        b1 = new baseNumerica(s1, 1);
                        band = true;
                    }
                    else
                    {
                        b1 = new baseNumerica(s1);
                    }
                    xb1 += "111";
                    switch(s2)
                    {
                        case "X":
                            xb1 += "0";
                            xb2 += "000";
                            break;
                        case "Y":
                            xb1 += "0";
                            xb2 += "100";
                            break;
                        case "SP":
                            xb1 += "1";
                            xb2 += "000";
                            break;
                        case "PC":
                            xb1 += "1";
                            xb2 += "100";
                            break;
                    }
                    String xb3 = "";
                    if (band)
                    {
                        xb2 += "1";
                        int sizeTMP = b1.getNumberHexadecimal().Length;
                        for (int j = sizeTMP - 2 ; j < sizeTMP;j++ )
                        {
                            xb3 += b1.getNumberHexadecimal()[j];
                        }
                    }
                    else
                    {
                        xb2 += "0";
                        xb3 = b1.getNumberHexadecimal().PadLeft(2, '0');
                    }
                    b2 = new baseNumerica(xb1);
                    b3 = new baseNumerica(xb2);
                    xb = b2.getNumberHexadecimal() + b3.getNumberHexadecimal() + xb3;
                    break;
                case "IDX2":
                    band = false;
                    if (s1[0] == '-')
                    {
                        char[] caracteres = { '-' };
                        s1 = s1.TrimStart(caracteres);
                        b1 = new baseNumerica(s1, 1);
                        band = true;
                    }
                    else
                    {
                        b1 = new baseNumerica(s1);
                    }
                    xb1 += "111";
                    switch(s2)
                    {
                        case "X":
                            xb1 += "0";
                            xb2 += "001";
                            break;
                        case "Y":
                            xb1 += "0";
                            xb2 += "101";
                            break;
                        case "SP":
                            xb1 += "1";
                            xb2 += "001";
                            break;
                        case "PC":
                            xb1 += "1";
                            xb2 += "101";
                            break;
                    }
                    xb3 = "";
                    if (band)
                    {
                        xb2 += "1";
                        int sizeTMP = b1.getNumberHexadecimal().Length;
                        for (int j = sizeTMP - 4 ; j < sizeTMP;j++ )
                        {
                            xb3 += b1.getNumberHexadecimal()[j];
                        }
                    }
                    else
                    {
                        xb2 += "0";
                        xb3 = b1.getNumberHexadecimal().PadLeft(4, '0');
                    }
                    b2 = new baseNumerica(xb1);
                    b3 = new baseNumerica(xb2);
                    xb = b2.getNumberHexadecimal() + b3.getNumberHexadecimal() + xb3;
                    break;
            }
        }
        public void generaCodigoMaquina(baseNumerica contlocFile,CODOP codop,String contLocLabel)
        {
            baseNumerica contEtiqueta = new baseNumerica(contLocLabel);
            baseNumerica contLocNext = new baseNumerica(System.Convert.ToString(contlocFile.getNumberDecimal() + codop.getBytesTotales()));
            baseNumerica contLocNew = new baseNumerica("0");
            fueraRango = false;

            int valor = contEtiqueta.getNumberDecimal() - contLocNext.getNumberDecimal();

            if (valor < 0)
            {
                char[] caracteres = { '-' };
                String tmp = System.Convert.ToString(valor);
                tmp = tmp.TrimStart(caracteres);
                contLocNew = new baseNumerica(tmp,1);
                tmp = contLocNew.getNumberHexadecimal();
                xb = "";
                for (int i = (tmp.Length) - (2*codop.getBytesPorCalcular()); i<tmp.Length;i++ )
                {
                    xb += tmp[i]; 
                }
            }
            else
            {
                contLocNew = new baseNumerica(System.Convert.ToString(contEtiqueta.getNumberDecimal() - contLocNext.getNumberDecimal()));
                xb = contLocNew.getNumberHexadecimal();
                xb = xb.PadLeft(2 * codop.getBytesPorCalcular(), '0');
            }
            if(codop.getBytesPorCalcular() == 1)
            {
                if (!(contLocNew.getNumberDecimal() >= -128 && contLocNew.getNumberDecimal() <= 127))
                {
                    fueraRango = true;
                }
            }
            else
            {
                if (!(contLocNew.getNumberDecimal() >= -32768 && contLocNew.getNumberDecimal() <= 32767))
                {
                    fueraRango = true;
                }
            }
        }//Termina generaCodigoMaquina

        public String getOperando()
        {
            return operando;
        }
        public String getOriginal()
        {
            return original;
        }
        public String getXb()
        {
            return xb;
        }
        public Boolean getValido()
        {
            return valido;
        }
        public Boolean getFueraRango()
        {
            return fueraRango;
        }
        public int getSize()
        {
            return operando.Length;
        }
    }
}
