using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class baseNumerica
    {
        Boolean valido;
        String numberOriginal,numberHexadecimal,numberBinary;
        int type,tam,numberDecimal;

        //Constructor
        public baseNumerica(String number)
        {
            valido = true;
            numberOriginal = "";
            char basetipo = number[0];
            String tmp = "";
            tam = number.Length;
            int j=1;
            if(number[0]!='$' && number[0]!='%' && number[0]!='@')
            {
                j = 0;
            }
            for (; j<tam;j++ )
            {
                tmp += number[j];
            }
            number = tmp;
            tam--;
            int i = 1;
            if (number[0] != '$' && number[0] != '%' && number[0] != '@')
            {
                i = 0;
            }
            for (; i < number.Length && valido; i++)
            {
                numberOriginal += number[i];
                switch (basetipo)
                {
                    case '%':
                        if (number[i] - '0' > 2 || number[i] - '0' < 0)
                        {
                            valido = false;
                            return;
                        }
                        type = 0;
                        break;
                    case '@':
                        if (number[i] - '0' > 8 || number[i] - '0' < 0)
                        {
                            valido = false;
                            return;
                        }
                        type = 1;
                        break;
                    case '$':
                        if ((number[i] >= 'A' && number[i] <= 'F') || (number[i] - '0' < 10 && number[i] - '0' >= 0))
                        {
                        }
                        else
                        {
                            valido = false;
                            return;
                        }
                        type = 2;
                        break;
                    case '0':case '1':case '2':case '3':case '4':case '5':case '6': case '7': case '8':case '9':
                        if (!(number[i] - '0' >= 0 && number[i] - '0' <= 9))
                        {
                            valido = false;
                            return;
                        }
                        type = 3;
                        break;
                    default:
                        valido = false;
                        break;
                }//End switch
            }//End for
            convert2Dec();
            convert2Hex();
            convert2Bin();
        }
        //Constructor negativos
        public baseNumerica(String number, int x)
        {
            valido = true;
            numberOriginal = "";
            char basetipo = number[0];
            String tmp = "";
            tam = number.Length;
            int j = 1;
            if (number[0] != '$' && number[0] != '%' && number[0] != '@')
            {
                j = 0;
            }
            for (; j < tam; j++)
            {
                tmp += number[j];
            }
            number = tmp;
            tam--;
            int i = 1;
            if (number[0] != '$' && number[0] != '%' && number[0] != '@')
            {
                i = 0;
            }
            for (; i < number.Length && valido; i++)
            {
                numberOriginal += number[i];
                switch (basetipo)
                {
                    case '%':
                        if (number[i] - '0' > 2 || number[i] - '0' < 0)
                        {
                            valido = false;
                            return;
                        }
                        type = 0;
                        break;
                    case '@':
                        if (number[i] - '0' > 8 || number[i] - '0' < 0)
                        {
                            valido = false;
                            return;
                        }
                        type = 1;
                        break;
                    case '$':
                        if ((number[i] >= 'A' && number[i] <= 'F') || (number[i] - '0' < 10 && number[i] - '0' >= 0))
                        {
                        }
                        else
                        {
                            valido = false;
                            return;
                        }
                        type = 2;
                        break;
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
                        if (!(number[i] - '0' >= 0 && number[i] - '0' <= 9))
                        {
                            valido = false;
                            return;
                        }
                        type = 3;
                        break;
                    default:
                        valido = false;
                        break;
                }//End switch
            }//End for
            convert2Dec(x);
            convert2Hex();
            convert2Bin();
        }
        ~baseNumerica()
        {

        }

        public void convert2Dec()
        {
            if (!valido)
            {
                return;
            }
            for (int i = numberOriginal.Length - 1,j=0; i >= 0; i--,j++)
            {
                switch (type)
                {
                    case 0:
                        numberDecimal = (Convert.ToInt16(numberOriginal[i] - '0') * (int)Math.Pow(2, j )) + numberDecimal;
                        break;
                    case 1:
                        numberDecimal = (Convert.ToInt16(numberOriginal[i] - '0') * (int)Math.Pow(8, j )) + numberDecimal;
                        break;
                    case 2:
                        if ((System.Convert.ToInt16(numberOriginal[i]) >= 65 && System.Convert.ToInt16(numberOriginal[i]) <= 70) ||
                            (System.Convert.ToInt16(numberOriginal[i]) >= 97 && System.Convert.ToInt16(numberOriginal[i]) <= 102))
                        {
                            if (System.Convert.ToInt16(numberOriginal[i]) > 70)
                            {
                                numberDecimal = ((System.Convert.ToInt16(numberOriginal[i]) - 87) * (int)Math.Pow(16, j )) + numberDecimal;
                            }
                            else
                            {
                                numberDecimal = ((System.Convert.ToInt16(numberOriginal[i]) - 55) * (int)Math.Pow(16, j ))+numberDecimal;
                            }
                        }
                        else
                        {
                            numberDecimal = (Convert.ToInt16(numberOriginal[i]-48) * (int)Math.Pow(16, j)) + numberDecimal;
                        }
                        break;
                }//End Switch
            }//End for
            if (type == 3)
            {
                numberDecimal = System.Convert.ToInt32(numberOriginal);
            }
        }//End Convert2Dec
        public void convert2Dec(int x)
        {
            if (!valido)
            {
                return;
            }
            for (int i = numberOriginal.Length - 1, j = 0; i >= 0; i--, j++)
            {
                switch (type)
                {
                    case 0:
                        numberDecimal = (Convert.ToInt16(numberOriginal[i] - '0') * (int)Math.Pow(2, j)) + numberDecimal;
                        break;
                    case 1:
                        numberDecimal = (Convert.ToInt16(numberOriginal[i] - '0') * (int)Math.Pow(8, j)) + numberDecimal;
                        break;
                    case 2:
                        if ((System.Convert.ToInt16(numberOriginal[i]) >= 65 && System.Convert.ToInt16(numberOriginal[i]) <= 70) ||
                            (System.Convert.ToInt16(numberOriginal[i]) >= 97 && System.Convert.ToInt16(numberOriginal[i]) <= 102))
                        {
                            if (System.Convert.ToInt16(numberOriginal[i]) > 70)
                            {
                                numberDecimal = ((System.Convert.ToInt16(numberOriginal[i]) - 87) * (int)Math.Pow(16, j)) + numberDecimal;
                            }
                            else
                            {
                                numberDecimal = ((System.Convert.ToInt16(numberOriginal[i]) - 55) * (int)Math.Pow(16, j)) + numberDecimal;
                            }
                        }
                        else
                        {
                            numberDecimal = (Convert.ToInt16(numberOriginal[i] - 48) * (int)Math.Pow(16, j)) + numberDecimal;
                        }
                        break;
                }//End Switch
            }//End for
            if (type == 3)
            {
                numberDecimal = System.Convert.ToInt32(numberOriginal);
            }
            numberDecimal *= -1;
        }//End Convert2Dec Negativo
        public void convert2Hex()
        {
            numberHexadecimal = numberDecimal.ToString("X");
            //for (int i = numberHexadecimal.Length; i<4; i++)
            //{
            //    numberHexadecimal = "0" + numberHexadecimal;
            //}
        }//Fin de convert2Hex
        public void convert2Bin()
        {
            numberBinary = System.Convert.ToString(numberDecimal,2);//Revizar
        }//Fin de convert2Bin

        public Boolean getValido()
        {
            return valido;
        }
        public String getNumberOriginal()
        {
            return numberOriginal;
        }
        public String getNumberHexadecimal()
        {
            return numberHexadecimal;
        }
        public String getNumberBinary()
        {
            return numberBinary;
        }
        public int getType()
        {
            return type;
        }
        public int getTam()
        {
            return tam;
        }
        public int getNumberDecimal()
        {
            return numberDecimal;
        }

        public void setNumberHexadecimal(String numberHexadecimal)
        {
            this.numberHexadecimal = numberHexadecimal;
        }
    }
}
