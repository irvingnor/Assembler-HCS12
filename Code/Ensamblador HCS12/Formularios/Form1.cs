using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Ensamblador_HCS12
{
    public partial class Form1 : Form
    {
        #region "Variables Globales"
        public int lineasArchivo = 0, dirIni = 0;
        public bool banderaEnd = false, flagOperando = false, flagMinus = false,flagPlus = false, flagORG = false;
        public TMPCODOP[] arrayTMPCODOP = new TMPCODOP[202];//200
        public CODOP[] arrayCODOP = new CODOP[580];
        public String ResultadoModo = "", sizeModo = "", modoDireccionamiento = "", fdOriginal = "",nameFile="";
        public System.IO.FileStream fsTMP,fsTabsim,fsS19;
        public lineFile lfile = new lineFile();
        public String[] mensajesError = new String[10];
        LABEL label;
        CODOP codop;
        OPERANDO operando;
        DIRECTIVA directive;
        baseNumerica contLoc = new baseNumerica(System.Convert.ToString(0));
        baseNumerica dirInicial = new baseNumerica(System.Convert.ToString(0));
        public ArrayList listaEtiquetas = new ArrayList();
        ArrayList dirEtiquetas = new ArrayList();
        public ArrayList arrayDirS1 = new ArrayList();//S1
        public ArrayList arrayCodMaqinaS1 = new ArrayList();//S1
        #endregion

        bool isDigit(char c)
        {
            return (System.Convert.ToInt16(c) >= 48 && System.Convert.ToInt16(c) <= 57);
        }
        public Form1()
        {
            InitializeComponent();
        }
        public void binarySearch(int ini,int fin,string elemento)
        {
            int medio=-1;
            bool flag = false;
            while ((ini <= fin) && (!flag))
            {
                medio = (ini + fin) / 2;
                if (elemento == arrayTMPCODOP[medio].getName()) { flag = true; }
                else if (elemento.CompareTo(arrayTMPCODOP[medio].getName()) > 0) ini = medio + 1;
                else if (elemento.CompareTo(arrayTMPCODOP[medio].getName()) < 0) fin = medio - 1;
                else flag = true;
            }
            if (flag)
            {
                codop.setIniciaCODOP(medio);
            }
            else
            {
                codop.setNoExiste();
            }
        }
        public string regresaDireccionamiento(string dir)
        {
            switch(dir)
            {
                case "INH":
                    dir = "Inherente";
                    break;
                case "IMM":
                    dir = "Inmediato";
                    break;
                case "DIR":
                    dir = "Directo";
                    break;
                case "EXT":
                    dir = "Extendido";
                    break;
                case "REL":
                    dir = "Relativo";
                    break;
                case "IDX1":
                    dir = "Indexado de 9 bits";
                    break;
                case "IDX2":
                    dir = "Indexado de 16 bits";
                    break;
                case "[IDX2]":
                    dir = "Indexado indirecto";
                    break;
                case "[D,IDX]":
                    dir = "Indexado de acumulador indirecto";
                    break;
            }
            return dir;
        }

        private void separaCadena(String cadena)
        {
            if (cadena[0] == ';')
            {
                txtMensajes.Text += "Comentario";
                printEnter();
                return;
            }
            //##############################################################################################
            //Separo la cadena con un automata
            String[] separados = new String[3];//[0]Etiqueta [1]CODOP [2]OPERANDO
            bool[] validos = new bool[3];
            int[,] estados = new int[5,2]{
            {0,1},
            {2,1},
            {2,3},
            {4,3},
            {4,4}};
            int state=0,input=0,sizeString=cadena.Length,sizeTotal=0,i=0;
            while(i<sizeString)
            {
                switch(cadena[i])
                {
                    case ' ': case '\t':
                        input = 1;
                        break;
                    default:
                        input = 0;
                        break;
                }
                state = estados[state,input];
                switch(state){
                    case 0:
                        separados[sizeTotal] += cadena[i++];
                        break;

                    case 1:case 3:
                        i++;
                        if (i < sizeString && cadena[i] != ' ' && cadena[i] != '\t')
                        {
                            sizeTotal++;
                        }
                        break;

                    case 2:
                        separados[sizeTotal] += cadena[i++];
                        break;

                    case 4:
                        separados[sizeTotal] += cadena[i++];
                        break;
                }
            }

            for (int j = 0; j < 3; j++)
            {
                if (separados[j] == null)
                    separados[j] = "NULL";
            }
                //##############################################################################################

            //Creo los objetos de las respectivas clases.
            label = new LABEL(separados[0]);
            label.validaEtiqueta();
            codop = new CODOP(separados[1]);
            codop.validaCODOP();
            operando = new OPERANDO(separados[2]);
            operando.validaOperando();

           //Valido las combinaciones
           if (codop.getCodop() == "NULL")
           {
                    txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                    printEnter();
                    txtMensajes.Text += mensajesError[0];//Dice cuales son las combinaciones validas...
                    printEnter();
                    printEnter();
                    return;
           }

            //Validaciones para la etiqueta
            if (label.getValida())
            {
                txtMensajes.Text += "ETIQUETA = " + label.getLabel();//Etiqueta
                printEnter();
                //Guardo en el tabsim la etiqueta
                if (label.getLabel() != "NULL" && codop.getCodop() != "EQU")
                {
                    Boolean flagEncontrada = false;
                    foreach(String labelTMP in listaEtiquetas)
                    {
                        if(label.getLabel() == labelTMP)
                        {
                            flagEncontrada = true;
                        }
                    }

                    if (!flagEncontrada)
                    {
                        lfile.setLabel(label.getLabel());
                        lfile.setValue(contLoc.getNumberHexadecimal().PadLeft(4, '0'));//Modifica1 Contoloc
                        lfile.save(fsTabsim, 2);
                        listaEtiquetas.Add(label.getLabel());
                        dirEtiquetas.Add(contLoc.getNumberHexadecimal());
                    }
                    else
                    {
                        txtMensajes.Text +=  mensajesError[7];
                    }
                }
                //Prepara para el archivo TMP
                lfile.setLabel1(label.getLabel());
            }
            else
            {
                txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                printEnter();
                txtMensajes.Text += mensajesError[1];//Etiqueta no valida
                printEnter();
            }

            //Validaciones para el CODOP
            if (codop.getValido())
            {
                txtMensajes.Text += "CODOP = " + codop.getCodop();//Codop
                printEnter();
                binarySearch(0,200,codop.getCodop());
                if (codop.getExiste())
                {
                    codop.revizaModoDireccionamiento(arrayCODOP,arrayTMPCODOP[codop.getIniciaCODOP()].start,arrayTMPCODOP[codop.getIniciaCODOP()].end,operando);
                    //Preparo para guardar en el archivo TMP
                    lfile.setCODOP(codop.getCodop());
                }
                else
                {
                    directive = new DIRECTIVA(codop.getCodop(),operando,label);
                    if (directive.getExiste())
                    {
                        if(directive.getValida())
                        {
                            //Preparo para guardar en el archivo TMP
                            if(codop.getCodop() == "ORG")
                            {
                                baseNumerica b1 = new baseNumerica(System.Convert.ToString(operando.getOperando()));
                                contLoc = b1;
                                dirInicial = b1;
                                //contLoc.setNumberHexadecimal(b1.getNumberHexadecimal());
                            }
                            else if(codop.getCodop() == "EQU")
                            {
                                baseNumerica b1 = new baseNumerica(System.Convert.ToString(operando.getOperando()));

                                Boolean flagExist = false;
                                foreach(String stmp in listaEtiquetas)
                                {
                                    if(stmp == label.getLabel())
                                    {
                                        flagExist = true;
                                    }
                                }

                                if (flagExist)
                                {
                                    txtMensajes.Text += mensajesError[7];
                                    printEnter();
                                }
                                else
                                {
                                    lfile.setLabel(label.getLabel());
                                    //lfile.setValue(directive.getValueHexadecimal());
                                    lfile.setValue(b1.getNumberHexadecimal().PadLeft(4, '0'));//Modifca2 Contloc
                                    lfile.save(fsTabsim, 2);
                                    listaEtiquetas.Add(label.getLabel());
                                    dirEtiquetas.Add(b1.getNumberHexadecimal().PadLeft(4,'0'));
                                    lfile.setValue1(b1.getNumberHexadecimal().PadLeft(4, '0'));
                                }                  
                            }
                            lfile.setCODOP(codop.getCodop());
                        }
                        else
                        {
                            txtMensajes.Text += directive.getImprimirMensaje();
                            printEnter();
                        }
                    }
                    else
                    {
                        txtMensajes.Text += mensajesError[5];//El CODOP ingresado no existe
                        printEnter();
                    }
                }
            }
            else
            {
                txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                printEnter();
                txtMensajes.Text += mensajesError[2];//Codop no valido
                printEnter();
            }

            //Validaciones para el OPERANDO
            txtMensajes.Text += "OPERANDO = " + operando.getOperando();//Operando
            printEnter();
            //Preparo para guardar operando
            lfile.setOperando(operando.getOriginal());

            txtMensajes.Text += codop.getMensajeResultado() + "  ";

            if (codop.getBytesTotales() > 0)
            {
                txtMensajes.Text +=  Convert.ToString(codop.getBytesTotales()) + " bytes";
            }
            printEnter();

            //Verifico si el codop necesita un operando
            if (operando.getOperando() == "NULL" && codop.getNecesitaOperando() && codop.getExiste())
            {
                txtMensajes.Text += mensajesError[3];//El codop necesita operando
                printEnter();
            }
            else if(operando.getOperando() != "NULL" && !codop.getNecesitaOperando() && codop.getExiste())
            {
                txtMensajes.Text += mensajesError[4];//El codop no necesita operando
                printEnter();
            }
            printEnter();

            //Verifico si hay un END de cierre
            if (codop.getCodop() == "END") { banderaEnd = true; }

            baseNumerica OV = new baseNumerica(System.Convert.ToString(0));

            //Recalculo el ContLOc
            if (codop.getExiste())
            {
                OV = new baseNumerica(System.Convert.ToString(codop.getBytesTotales()));
            }
            else if(directive.getValida())
            {
                OV = new baseNumerica(System.Convert.ToString(directive.getSize()));
            }

            //int temporal = contLoc.getNumberDecimal() + OV.getNumberDecimal();
            //baseNumerica TMP = new baseNumerica(System.Convert.ToString(temporal));

            //contLoc = TMP;

            if (codop.getCodop() != "EQU")
            {
                lfile.setValue1(contLoc.getNumberHexadecimal().PadLeft(4, '0'));//Modifica3 Contloc
            }
            //Guardo el TMP
            lfile.save(fsTMP,1);

            int temporal = contLoc.getNumberDecimal() + OV.getNumberDecimal();
            baseNumerica TMP = new baseNumerica(System.Convert.ToString(temporal));

            contLoc = TMP;

            //Llamo al destructor
            codop = null;
            GC.Collect();
        }
        private void segundoPaso(String cadena)
        {
            String codigoMaquina = "";
            int tam = cadena.Length;
            String[] arrayLine = new String[4];
            int i = 0,j=0;
            while (i<tam)
            {
                if (cadena[i] != '\t')
                {
                    arrayLine[j] += cadena[i++];
                }
                else
                {
                    j++;
                    i++;
                }
            }
            //Cargo las 4 partes de la linea
            baseNumerica contLocFile = new baseNumerica("$"+arrayLine[0]);
            LABEL labelFile = new LABEL(arrayLine[1]);
            codop = new CODOP(arrayLine[2]);
            OPERANDO operandoFile = new OPERANDO(arrayLine[3]);

            labelFile.validaEtiqueta();
            codop.validaCODOP();
            operandoFile.validaOperando();

            if(codop.getCodop()=="END")
            {
                return;
            }

            if (codop.getValido())
            {
                binarySearch(0, 200, codop.getCodop());
                codop.revizaModoDireccionamiento(arrayCODOP, arrayTMPCODOP[codop.getIniciaCODOP()].start, arrayTMPCODOP[codop.getIniciaCODOP()].end, operandoFile);
            }
            else
            {
                txtMensajes.Text += mensajesError[2];//El CODOP no es valido
                printEnter();
                return;
            }

            if(!codop.getExiste())
            {
                DIRECTIVA d1 = new DIRECTIVA(codop.getCodop(),operandoFile,labelFile);

                if (d1.getExiste() && d1.getValida())
                {
                    txtMensajes.Text += d1.getCodigoMaquina();//11111111111111111111111Imprimo codigo Maquina de la directiva
                    codigoMaquina = d1.getCodigoMaquina();
                    if(codigoMaquina != "")
                    {
                        arrayCodMaqinaS1.Add(codigoMaquina);
                        arrayDirS1.Add(contLocFile);
                    }
                }
                else
                {
                    txtMensajes.Text += mensajesError[6];//No se encontro
                }
                printEnter();
                return;
            }
            //Verifico que tipo de direccionamiento tiene y hago creo el codigo maquina en base a eso
            String xb = "";
            switch(codop.getModoDireccionamiento())
            {
                case "INH":
                    txtMensajes.Text += codop.getCodigoMaquina();//22222222222222222222Imprimo codigo Maquina de codop
                    codigoMaquina = codop.getCodigoMaquina();
                    break;
                case "DIR":
                case "EXT":
                    baseNumerica b1 = new baseNumerica("0");
                    if(operandoFile.getOperando()[0] >= 'A' && operandoFile.getOperando()[0] <= 'Z')
                    {
                        int ilabel = 0,indiceLabel = 0;
                        Boolean labelFound = false;
                        foreach(String lTMP in listaEtiquetas)
                        {
                            if(lTMP == operandoFile.getOriginal())
                            {
                                indiceLabel = ilabel;
                                labelFound = true;
                            }
                            ilabel++;
                        }

                        if(labelFound)
                        {
                            ilabel = 0;
                            String labelExtendido = "";
                            foreach(String lTMP in dirEtiquetas)
                            {
                                if(ilabel == indiceLabel)
                                {
                                    labelExtendido = lTMP;
                                }
                                ilabel++;
                            }//Finaliza bucle foreach
                            labelExtendido = "$" + labelExtendido;
                            b1 = new baseNumerica(labelExtendido);//333333333333Imprimo codigo maquina de codop
                            txtMensajes.Text += codop.getCodigoMaquina() + System.Convert.ToString(b1.getNumberHexadecimal()).PadLeft(2 * codop.getBytesPorCalcular(), '0');
                            codigoMaquina = codop.getCodigoMaquina() + System.Convert.ToString(b1.getNumberHexadecimal()).PadLeft(2 * codop.getBytesPorCalcular(), '0');
                        }
                        else
                        {
                            txtMensajes.Text += mensajesError[8]; //Etiqueta no encontrada
                        }
                    }//Termina cuando es extendido con Etiqueta
                    else
                    {
                        b1 = new baseNumerica(arrayLine[3]);//444444444444444444444444Imprimo codigo maquina de codop
                        txtMensajes.Text += codop.getCodigoMaquina() + System.Convert.ToString(b1.getNumberHexadecimal()).PadLeft(2*codop.getBytesPorCalcular(),'0');
                        codigoMaquina = codop.getCodigoMaquina() + System.Convert.ToString(b1.getNumberHexadecimal()).PadLeft(2 * codop.getBytesPorCalcular(), '0');
                    }
                    break;
                case "IMM":
                    char[] car = {'#'};
                    arrayLine[3] = arrayLine[3].TrimStart(car);
                    baseNumerica b2 = new baseNumerica(arrayLine[3]);//55555555555555555555Imprimo codigo maquina de codop
                    txtMensajes.Text += codop.getCodigoMaquina() + System.Convert.ToString(b2.getNumberHexadecimal()).PadLeft(2 * codop.getBytesPorCalcular(), '0');
                    codigoMaquina = codop.getCodigoMaquina() + System.Convert.ToString(b2.getNumberHexadecimal()).PadLeft(2 * codop.getBytesPorCalcular(), '0');
                    break;
                case "[IDX2]":
                    int sizeTMP = arrayLine[3].Length - 1;
                    String registro = "", operandoTMP = "";
                    Boolean band1 = false;
                    for (i=1; i < sizeTMP ;i++ )
                    {
                        if (arrayLine[3][i]==',')
                        {
                            band1 = true;
                        }
                        else if(!band1)
                        {
                            operandoTMP += arrayLine[3][i];
                        }
                        else
                        {
                            registro += arrayLine[3][i];
                        }
                    }//Fin del for
                    switch(registro)
                    {
                        case "X":
                            xb = "E3";
                            break;
                        case "Y":
                            xb = "EB";
                            break;
                        case "SP":
                            xb = "F3";
                            break;
                        case "PC":
                            xb = "FB";
                            break;
                    }
                    baseNumerica bI16 = new baseNumerica(operandoTMP);//666666666666666Imprimo codigo maquina de codop
                    txtMensajes.Text += codop.getCodigoMaquina() + xb + System.Convert.ToString(bI16.getNumberHexadecimal()).PadLeft(2 * (codop.getBytesPorCalcular()-1), '0');
                    codigoMaquina = codop.getCodigoMaquina() + xb + System.Convert.ToString(bI16.getNumberHexadecimal()).PadLeft(2 * (codop.getBytesPorCalcular() - 1), '0');
                    break;
                case "[D,IDX]":
                    xb = "";
                    switch(arrayLine[3][3])
                    {
                        case 'X':
                            xb = "E7";
                            break;
                        case 'Y':
                            xb = "EF";
                            break;
                        case 'S':
                            xb = "F7";
                            break;
                        case 'P':
                            xb = "FF";
                            break;
                    }
                    txtMensajes.Text += codop.getCodigoMaquina() + xb;//77777777777777777777Imprimo codigo maquina de codop
                    codigoMaquina = codop.getCodigoMaquina() + xb;
                    break;
                case "IDX":
                    operandoFile.generaXB(codop);
                    txtMensajes.Text += codop.getCodigoMaquina() + operandoFile.getXb().PadLeft(2,'0');//88888888888Imprimo Codigo maquina de codop
                    codigoMaquina = codop.getCodigoMaquina() + operandoFile.getXb().PadLeft(2, '0');
                    break;
                case "IDX1":
                    operandoFile.generaXB(codop);
                    txtMensajes.Text += codop.getCodigoMaquina() + operandoFile.getXb();//999999999999999999999Imprimo codigo maquina de codop
                    codigoMaquina = codop.getCodigoMaquina() + operandoFile.getXb();
                    break;
                case "IDX2":
                    operandoFile.generaXB(codop);
                    txtMensajes.Text += codop.getCodigoMaquina() + operandoFile.getXb();//AAAAAAAAAAAAAAAImprimo codigo maquina de codop
                    codigoMaquina = codop.getCodigoMaquina() + operandoFile.getXb();
                    break;
                case "REL":
                    int indice = 0,tmp = 0;
                    Boolean flagFound = false;
                    foreach (String labelTMP in listaEtiquetas)
                    {
                        if (operandoFile.getOperando() == labelTMP)
                        {
                            indice = tmp;
                            flagFound = true;
                        }    
                        tmp++;
                    }

                    if(flagFound)
                    {
                        tmp = 0;
                        String contLocLabel = "$";
                        foreach(String contlocTMP in dirEtiquetas)
                        {
                            if(tmp == indice)
                            {
                                contLocLabel += contlocTMP;
                            }
                            tmp++;
                        }
                        operandoFile.generaCodigoMaquina(contLocFile,codop,contLocLabel);
                        if (operandoFile.getFueraRango())
                        {
                            txtMensajes.Text += "FUERA DE RANGO ";
                            if(codop.getBytesPorCalcular() == 1)
                            {
                                txtMensajes.Text += "los rangos permitidos son de -128 hasta 127";
                            }
                            else
                            {
                                txtMensajes.Text += "los rangos permitidos son de -32768 hasta 32767";
                            }
                        }
                        else
                        {
                            txtMensajes.Text += codop.getCodigoMaquina().PadLeft(2, '0') + operandoFile.getXb();//BBBBBBBBBBBBBImprimo codigo maquina de codop
                            codigoMaquina = codop.getCodigoMaquina().PadLeft(2, '0') + operandoFile.getXb();
                        }
                    }
                    else
                    {
                        txtMensajes.Text += mensajesError[8];//Etiqueta no encontrada
                    }
                    break;
            }

            //Agregando informacion para posteriormente construir el S1
            arrayDirS1.Add(contLocFile);
            arrayCodMaqinaS1.Add(codigoMaquina);
            printEnter();
            GC.Collect();
        }
        private void partesS1()
        {
            //int elementoActual = 0;
            S1 s1 = new S1();
            baseNumerica temporalAct = new baseNumerica("0");
            baseNumerica direccionActual = new baseNumerica("$" + ((baseNumerica)arrayDirS1[0]).getNumberHexadecimal());
            s1.direccion = direccionActual.getNumberHexadecimal();
            Boolean flagPase = false;

            for (int elementoActual = 0; elementoActual < arrayCodMaqinaS1.Count; elementoActual++)
            {
                temporalAct = new baseNumerica("$" + ((baseNumerica)arrayDirS1[elementoActual]).getNumberHexadecimal());
                if (temporalAct.getNumberHexadecimal() != direccionActual.getNumberHexadecimal())//Recorro todos los elementos de los arreglos que cree
                {
                    if (flagPase)
                    {
                        if (s1.direccion == "")
                            s1.direccion = temporalAct.getNumberHexadecimal();
                        baseNumerica longitudA = new baseNumerica(System.Convert.ToString(s1.codMaquina.Length / 2));
                        direccionActual = new baseNumerica(System.Convert.ToString(direccionActual.getNumberDecimal() - longitudA.getNumberDecimal()));
                        flagPase = false;
                    }
                    else
                    {
                        if (s1.direccion == "")
                        {
                            s1.direccion = temporalAct.getNumberHexadecimal();                        
                            ////    baseNumerica tmpComp = new baseNumerica(System.Convert.ToString(direccionActual.getNumberDecimal() - (s1.codMaquina.Length / 2)));
                            ////    if (elementoActual - (s1.codMaquina.Length / 2) >= 0 && ((baseNumerica)arrayDirS1[elementoActual - (s1.codMaquina.Length / 2)]).getNumberHexadecimal() == tmpComp.getNumberHexadecimal())
                            ////    {
                            ////        s1.direccion = tmpComp.getNumberHexadecimal();
                            ////    }
                            ////    else
                            ////    {
                            ////        s1.direccion = direccionActual.getNumberHexadecimal();
                            ////    }
                            ////}
                        }
                    }
                    s1.calculaLongitud();
                    s1.calculaCRC();
                    s1.saveFile(fsS19);
                    s1 = new S1();
                    if (s1.direccion != "")
                    {
                        //s1.direccion = ((baseNumerica)arrayDirS1[elementoActual]).getNumberHexadecimal();
                        s1.direccion = temporalAct.getNumberHexadecimal();
                    }
                    direccionActual = new baseNumerica("$" + s1.direccion);
                }
                for (int i = 0; i < ((String)arrayCodMaqinaS1[elementoActual]).Length; i += 2)
                {
                    if ((s1.codMaquina.Length / 2) < 16)
                    {
                        String tmp = System.Convert.ToString(((String)arrayCodMaqinaS1[elementoActual])[i]) + System.Convert.ToString(((String)arrayCodMaqinaS1[elementoActual])[i + 1]);
                        s1.addCodigoMaquina(tmp);
                        direccionActual = new baseNumerica(System.Convert.ToString(direccionActual.getNumberDecimal() + 1));
                    }
                    else
                    {
                        flagPase = true;
                        i -= 2;
                        s1.calculaLongitud();
                        s1.calculaCRC();
                        s1.saveFile(fsS19);
                        s1 = new S1();
                        s1.direccion = direccionActual.getNumberHexadecimal();
                    }
                }//Fin del bucle for para recorrer el elemento actual
            }//Fin del bucle for que recorre todos los elementos
            if(s1.codMaquina!="")//Agrego el ultimo bloque
            {
                s1.calculaLongitud();
                s1.calculaCRC();
                s1.saveFile(fsS19);
            }
        }//Termina partesS1

        //Esenciales para el formulario
        private void printEnter()
        {
            txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
        }
        private void cargaMensajesError()
        {
            String msgError = "[Error] ";
            mensajesError[0] = msgError + "La combinacion, no se encontro OPERANDO, solo se permiten combinaciones como: 1) ETIQUETA,CODOP,OPERANDO 2) ETIQUETA,CODOP 3) CODOP,OPERANDO 4) CODOP";
            mensajesError[1] = msgError + "La ETIQUETA no es valida";
            mensajesError[2] = msgError + "El CODOP no es valido";
            mensajesError[3] = msgError + "El CODOP necesita OPERANDO";
            mensajesError[4] = msgError + "El CODOP no necesita OPERANDO";
            mensajesError[5] = msgError + "El CODOP ingresado no existe";
            mensajesError[6] = msgError + "No encontrado";
            mensajesError[7] = msgError + "Etiqueta repetida";
            mensajesError[8] = msgError + "Etiqueta no encontrada";
        }
        private void abreArchivo()
        {
            OpenFileDialog ofD = new OpenFileDialog();
            ofD.Filter = "Ensamblador|*.asm|Documento de texto|*.txt|Todos los archivos|*.*";
            if (ofD.ShowDialog() == DialogResult.OK)
            {
                StreamReader objReader = new StreamReader(ofD.FileName, System.Text.Encoding.Default, true);
                //Leo el archivo
                Boolean flagPoint = false;
                for(int i=ofD.FileName.Length-1;ofD.FileName[i] != '\\' && i>0;i--)
                {
                    if (flagPoint)
                    {
                        fdOriginal = ofD.FileName[i] + fdOriginal;
                    }else if(ofD.FileName[i]=='.')
                    {
                        flagPoint = true;
                    }
                }
                nameFile = ofD.FileName;
                String sLine = "";
                String todo = "";
                ArrayList arrText = new ArrayList();
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        arrText.Add(sLine);
                }
                objReader.Close();
                fsTMP = System.IO.File.Create(fdOriginal+".tmp", 1024);
                fsTabsim = System.IO.File.Create(fdOriginal+".tabsim",1024);
                fsS19 = System.IO.File.Create(fdOriginal+".s19");
                foreach (string sOutput in arrText)
                {
                    todo += sOutput;
                    separaCadena(sOutput);//Llamada Para separar y analizar cadena. Kernel del programa
                    todo += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                    lineasArchivo++;
                }
                txtCodigo.Text = todo;
                fsTMP.Close();
                fsTabsim.Close();
                label2.Text = "Total de lineas: " + lineasArchivo;
                if (!banderaEnd)
                {
                    txtMensajes.Text += "Linea: " + lineasArchivo;
                    txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                    txtMensajes.Text += "No se encontro END";
                    txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                }
                //Archivo S0 del s019
                S0 s0 = new S0();
                s0.addCodigoMaquina(nameFile);
                s0.calculaLongitud();
                s0.calculaCRC();
                s0.saveFile(fsS19);
                //<Segundo Paso>
                StreamReader objReader1 = new StreamReader(fdOriginal + ".tmp", System.Text.Encoding.Default, true);
                String sLine1 = "";
                ArrayList arrText1 = new ArrayList();
                while (sLine1 != null)
                {
                    sLine1 = objReader1.ReadLine();
                    if (sLine1 != null)
                        arrText1.Add(sLine1);
                }
                objReader1.Close();
                txtMensajes.Text += "###Paso 2###";
                printEnter();
                foreach (string sOutput in arrText1)
                {
                    try
                    {
                        segundoPaso(sOutput);
                    }
                    catch (Exception ex)
                    {
                        txtMensajes.Text += "Error al analizar la linea";
                        printEnter();
                    }
                }
                //</Segundo Paso>
                if(arrayCodMaqinaS1.Count > 0)
                {
                    partesS1();//Funcion que crea todos los fragmentos S1
                }
                //Archivo s9
                S9 s9 = new S9();
                s9.saveFile(fsS19);
                fsS19.Close();
                txtMensajes.Text += "El tamaño del archivo es:";
                printEnter();
                baseNumerica tmp = new baseNumerica(System.Convert.ToString(contLoc.getNumberDecimal() - dirInicial.getNumberDecimal()));
                txtMensajes.Text += System.Convert.ToString(tmp.getNumberDecimal());
                lineasArchivo = 0;
            }
        }
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cargaMensajesError();
            txtMensajes.Text = "";
            txtCodigo.Text = "";
            fdOriginal = "";
            abreArchivo();
        }
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcercaDe acerca = new AcercaDe();
            acerca.Show();
        }
        private void cargaCodop()
        {
            StreamReader objReader = new StreamReader("TABOP.data");
            String sLine = "";
            ArrayList arrText = new ArrayList();
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                    arrText.Add(sLine);
            }
            objReader.Close();
            char[] delimitadores = {'|'};
            int tam = arrText.Count;
            for(int i=0,j=0;i<tam;j++)
            {
                string tmp = Convert.ToString(arrText[i]);
                string[] partes = tmp.Split(delimitadores);
                string actual = Convert.ToString(partes[0]),analizando;
                analizando = actual;
                arrayTMPCODOP[j] = new TMPCODOP();
                arrayTMPCODOP[j].setName(actual);
                arrayTMPCODOP[j].setStart(i);
                while (actual == analizando)
                {
                    arrayCODOP[i] = new CODOP();
                    bool necesita;
                    if (Convert.ToChar(partes[1]) == 'f')
                    {
                        necesita = false;
                    }
                    else
                    {
                        necesita = true;
                    }
                    arrayCODOP[i].setCODOP(Convert.ToString(partes[0]), necesita, Convert.ToString(partes[2]),
                    Convert.ToString(partes[3]), Convert.ToInt16(partes[4]), Convert.ToInt16(partes[5]), Convert.ToInt16(partes[6]));

                    tmp = Convert.ToString(arrText[++i]);
                    partes = tmp.Split(delimitadores);
                    analizando = Convert.ToString(partes[0]);

                }
                arrayTMPCODOP[j].setEnd(i);
                if (i == 576) { i++; }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // -------  Cargamos y mostramos el formulario Splash durante 3 segundos  -----
            try
            {
                cargaCodop();
            }catch(Exception ex)
            {
                MessageBox.Show("Error: "+Convert.ToString(ex));
            }
            frmSplash f1 = new frmSplash(3);
            f1.ShowDialog(this);     // Mostramos el formulario de forma modal.
            f1.Dispose();
        }
    }
}
