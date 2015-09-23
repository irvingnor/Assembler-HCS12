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
        public int lineasArchivo = 0;
        public bool banderaEnd = false;

        public Form1()
        {
            InitializeComponent();
        }

        public bool validaEtiqueta(String etiqueta)
        {
            return Regex.IsMatch(etiqueta, @"^[a-zA-Z][\w]*$") && etiqueta.Length <= 8;
        }

        public bool validaCodop(String codop)
        {
            return Regex.IsMatch(codop, @"^[a-zA-Z]+(\.)?[a-zA-Z]+$") && codop.Length <= 5;
        }

        public bool validaOperando(String operando)
        {
            return true;
        }

        private void separaCadena(String cadena)
        {
            if (cadena[0] == ';')
            {
                txtMensajes.Text += "Comentario";
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                return;
            }
            char[] delimitadores = { ' ', '\t' };
            ///String[] separados = cadena.Split(delimitadores);//[0]Etiqueta [1]CODOP [2]OPERANDO
            String patron = @"\s";
            String[] separados = Regex.Split(cadena,patron);
            ArrayList a1 = new ArrayList();
            int cont = 0;
            foreach (String s1 in separados )
            {
                if(s1!="" || cont==0)
                    a1.Add(s1);
                cont++;
            }

            String[] separados1 = new string[separados.Length];
            Array.Copy(separados, separados1, separados.Length);
            bool[] validos = new bool[3];

            //Ajustes para obtener solo 3 elementos
            if (separados[0] == "") { separados[0] = "NULL"; }//Que pasa si hay un espacio despues de CODOP?
            if (separados.Length == 2)
            {
                Array.Resize(ref separados, 3);
                separados[2] = "NULL";
            }
            if (separados1.Length > 3)
            {
                Array.Clear(separados1, 0, 2);
                int tam = separados1.Length;
                Array.Resize(ref separados1, tam);
                separados[2] = string.Join(" ", separados1.ToArray());
                separados[2] = separados[2].Trim();
            }

            //Valido las combinaciones
            if (separados[1] == "")
            {
                txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                txtMensajes.Text += "Error en la combinacion, no se encontro OPERANDO, solo se permiten combinaciones como: 1) ETIQUETA,CODOP,OPERANDO 2) ETIQUETA,CODOP 3) CODOP,OPERANDO 4) CODOP";
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                return;
            }

            //Valido las cadenas
            if (separados[0] != "NULL") { validos[0] = validaEtiqueta(separados[0]); } else { validos[0] = true; }
            if (separados[1] != "NULL") { validos[1] = validaCodop(separados[1]); } else { validos[1] = true; }//Siempre tiene que tener algun caracter maximo despues del . ?
            if (separados[2] != "NULL") { validos[2] = validaOperando(separados[2]); } else { validos[2] = true; }


            if (validos[0])
            {
                txtMensajes.Text += "ETIQUETA = " + separados[0];//Etiqueta
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
            }
            else
            {
                txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                txtMensajes.Text += "La ETIQUETA no es valida";
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
            }

            if (validos[1])
            {
                txtMensajes.Text += "CODOP = " + separados[1];//Codop
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
            }
            else
            {
                txtMensajes.Text += "Linea: " + (lineasArchivo + 1);
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                txtMensajes.Text += "El CODOP no es valido";
                txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
            }

            txtMensajes.Text += "OPERANDO = " + separados[2];//Operando
            txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
            txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);

            //Verifico si hay un END de cierre
            separados[1] = separados[1].ToUpper();
            if (separados[1] == "END") { banderaEnd = true; }
        }

        private void abreArchivo()
        {
            OpenFileDialog ofD = new OpenFileDialog();
            ofD.Filter = "Ensamblador|*.asm|Documento de texto|*.txt|Todos los archivos|*.*";
            if (ofD.ShowDialog() == DialogResult.OK)
            {
                StreamReader objReader = new StreamReader(ofD.FileName, System.Text.Encoding.Default, true);
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
                foreach (string sOutput in arrText)
                {
                    todo += sOutput;
                    separaCadena(sOutput);
                    todo += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                    lineasArchivo++;
                }
                txtCodigo.Text = todo;
                label2.Text = "Total de lineas: " + lineasArchivo;
                if (!banderaEnd)
                {
                    txtMensajes.Text += "Linea: " + lineasArchivo;
                    txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                    txtMensajes.Text += "No se encontro END";
                    txtMensajes.Text += char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10);
                }
                lineasArchivo = 0;
            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abreArchivo();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcercaDe acerca = new AcercaDe();
            acerca.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // -------  Cargamos y mostramos el formulario Splash durante 3 segundos  -----
            frmSplash f1 = new frmSplash(3);
            f1.ShowDialog(this);     // Mostramos el formulario de forma modal.
            f1.Dispose();
        }
    }
}
