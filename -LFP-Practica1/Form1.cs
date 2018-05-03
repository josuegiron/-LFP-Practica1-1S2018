using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _LFP_Proyecto1
{
    public partial class Form1 : Form
    {
        public List<tab> Tabs = new List<tab>();

        public Form1()
        {
            InitializeComponent();
            
        }

        public Pestania nuevaPestania()
        {
            int NewTabCount = Contenedor.TabCount;
            Pestania NewTab;
            NewTab = new Pestania(this.Contenedor);
            NewTab.Name = "Nuevo proyecto " + NewTabCount;
            NewTab.Padding = new System.Windows.Forms.Padding(3);
            NewTab.TabIndex = NewTabCount;
            NewTab.Text = "Nuevo proyecto " + NewTabCount;
            NewTab.UseVisualStyleBackColor = true;
            Contenedor.Controls.Add(NewTab);
            Contenedor.SelectedIndex = NewTab.TabIndex;

            return NewTab;
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nuevaPestania();
        }


        
        private void cargarPestanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog();
            oFD.Title = "Abrir proyecto";
            oFD.Filter = "Todos los archivos (*.*)|*.*";

            if (oFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Pestania NewTab = nuevaPestania();
                NewTab.rutaArchivo = oFD.FileName;
                NewTab.Codigo.Text = System.IO.File.ReadAllText(NewTab.rutaArchivo);
                
                NewTab.Text = oFD.SafeFileName;
               
                NewTab.Linealizar();
            }
        }

        private void guardarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pestania selectTab = Contenedor.SelectedTab as Pestania;
            string texto = selectTab.Codigo.Text;
            if (selectTab.rutaArchivo == null)
            {
                selectTab.rutaArchivo = guardarComo(texto);
            }
            else
            {
                StreamWriter fichero = new StreamWriter(selectTab.rutaArchivo);
                fichero.WriteLine(texto);
                fichero.Close();
            }
        }
        public string guardarComo(string texto)
        {
            Pestania selectTab = Contenedor.SelectedTab as Pestania;
           
            SaveFileDialog sFD = new SaveFileDialog();
            sFD.Title = "Guardar proyecto " + selectTab.Text;
            sFD.Filter = "Cualquier proyecto (*.*) |*.txt";

            sFD.DefaultExt = "txt";
            sFD.AddExtension = true;
            sFD.RestoreDirectory = true;
            sFD.InitialDirectory = @"H:\LO DEL ESCRITORIO";

            if (sFD.ShowDialog() == DialogResult.OK)
            {
                selectTab.rutaArchivo = sFD.FileName;

                StreamWriter fichero = new StreamWriter(selectTab.rutaArchivo);
                fichero.Write(texto);
                fichero.Close();
                selectTab.Text = sFD.FileName.Substring(sFD.FileName.LastIndexOf("\\") + 1);
                return selectTab.rutaArchivo;
            }
            else
            {
                sFD.Dispose();
                sFD = null;
                return null;
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pestania selectTab = Contenedor.SelectedTab as Pestania;
            string texto = selectTab.Codigo.Text;
            guardarComo(Text);
        }

        bool compilado = false;
        Pestania tabA, tabB;
        private void compilarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Contenedor.TabCount ==2)
            {
                Contenedor.SelectedIndex=0;
                tabA = Contenedor.SelectedTab as Pestania;
                analizadorLecxico(tabA);

                Contenedor.SelectedIndex = 1;
                tabB = Contenedor.SelectedTab as Pestania;
                analizadorLecxico(tabB);

                compilado = true;
            }
            else if (Contenedor.TabCount > 2)
            {
                List<tab> Tabs = new List<tab>();
                for (int i = 0; i < Contenedor.TabCount; i++)
                {
                    Contenedor.SelectedIndex = i;
                    Tabs.Add(new tab() { numTab = i, nomTab = Contenedor.SelectedTab.Text});
                }
                Seleccion Seleccionar = new Seleccion(Tabs);
                Seleccionar.Visible=true;
                Contenedor.SelectedIndex = Seleccionar.Sel("a");
                tabA = Contenedor.SelectedTab as Pestania;
                analizadorLecxico(tabA);
                Contenedor.SelectedIndex = Seleccionar.Sel("b");
                tabB = Contenedor.SelectedTab as Pestania;
                analizadorLecxico(tabB);
                compilado = true;
            }
            else
            {
                MessageBox.Show("Solo existe una pestania", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
            
        }

        private void buscarCoincidenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (compilado)
            {
                double porcentaje = 0;
                

                List<lexema> iguales = new List<lexema>();
                iguales = (from t in tabA.tablaDeSimbolos where tabB.tablaDeSimbolos.Any(x => x.nombre == t.nombre) select t).ToList();
                List<lexema> distintos = new List<lexema>();
                distintos = (from t in tabA.tablaDeSimbolos where !tabB.tablaDeSimbolos.Any(x => x.nombre == t.nombre) select t).ToList();

                porcentaje = (iguales.Count * 100) / (iguales.Count + distintos.Count);
                MessageBox.Show("Porcentaje de parecido "+ porcentaje + "%", "Resultado", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("No se ha compilado el codigo", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
        }


        public void analizadorLecxico(Pestania selectTab)
        {
            selectTab.tablaDeSimbolos.Clear();
            selectTab.tablaDeErrores.Clear();
            string cadena = selectTab.Codigo.Text+"\n";
            analizarLenguaje(cadena);
            selectTab.generarTablaDeSimbolos();
            selectTab.generarTablaDeErrores();
        }
        private Boolean comparar(string[] matrizDeCaracteres, string caracter)
        {
            string caracterStr = caracter.ToString();
            for (int i = 0; i < matrizDeCaracteres.Length; i++)
            {
                if (caracterStr == matrizDeCaracteres[i])
                {
                    return true;
                }
            }
            return false;
        }
        private void analizarLenguaje(string cadena)
        {
            Console.WriteLine("PRESIONASTE EL BOTON");
            int estadoInicial = 0;
            int estadoActual = 0;
            char caracterActual;
            string lexema = "";

            string[] L = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "\xD1", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "\xF1", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string[] N = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string[] R = { "=" };
            string[] PN = { "." };
            string[] P = { ";" };
            string[] D = { "(" };
            string[] I = { ")" };
            string[] C = { "," };
            string[] G = { "_" };
            string[] X = { " ", "+", "-", "*", "/", "^" };
            string[] SL = { "\n" };
            string[] CM = { "/" };
            string[] A = { "*" };
            string[] B = { "\"" };



            //string N = "ABCDEFGHI";

            int colActual = 0, filaActual = 1, fila = 0, columna = 0;

            //  INICIO DE LAS INTERACIONES

            for (estadoInicial = 0; estadoInicial < cadena.Length; estadoInicial++)
            {
                caracterActual = cadena[estadoInicial];
                string caracterActualStr = caracterActual.ToString();

                colActual++;
                //  AFD
                switch (estadoActual)
                {

                    case 0: //  ESTADO 0

                        fila = filaActual;
                        columna = colActual;
                        if (comparar(CM, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 8;
                        }
                        else if (comparar(N, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 1;
                        }
                        else if (comparar(L, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 2;
                        }
                        else if (comparar(P, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 3;
                        }
                        else if (comparar(R, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 4;
                        }
                        else if (comparar(D, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 5;
                        }
                        else if (comparar(I, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 6;
                        }
                        else if (comparar(C, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 7;
                        }
                        else if (comparar(B, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 14;
                        }

                        else
                        {
                            switch (caracterActual)
                            {
                                case ' ':
                                case '\t':
                                case '\b':
                                case '\f':
                                case '\r':
                                    estadoActual = 0;
                                    break;
                                case '\n':
                                    filaActual++;
                                    colActual = 0;
                                    estadoActual = 0;
                                    break;
                                default:

                                    lexema += caracterActual;
                                    estadoActual = -1;
                                    colActual--;
                                    break;
                            }

                        }
                        break;

                    case 1:     //  ESTADO 1
                        if (comparar(N, caracterActualStr))
                        {

                            estadoActual = 1;
                            lexema += caracterActual;
                        }
                        else if (comparar(L, caracterActualStr))
                        {
                            estadoActual = 9;
                            lexema += caracterActual;
                        }
                        else if (comparar(PN, caracterActualStr))
                        {
                            estadoActual = 16;
                            lexema += caracterActual;
                        }
                        else if (!comparar(N, caracterActualStr))
                        {
                            // metodo enviar lexema
                            // ver si es un lexema valido
                            validarLexema(lexema, fila, columna, "numero");
                            estadoActual = 0;
                            lexema = "";
                            estadoInicial--;
                            colActual--;

                        }


                        break;
                    case 2:     //  ESTADO 2
                        if (comparar(N, caracterActualStr))
                        {

                            estadoActual = 2;
                            lexema += caracterActual;
                        }
                        else if (comparar(L, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 2;
                        }
                        else if (comparar(G, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 2;
                        }
                        else
                        {
                            // metodo enviar lexema
                            // ver si es un lexema valido
                            validarLexema(lexema, fila, columna, "reservado");
                            estadoActual = 0;
                            lexema = "";
                            estadoInicial--;
                            colActual--;

                        }
                        break;
                    case 3:     //  ESTADO 3
                        validarLexema(lexema, fila, columna, "reservado");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 4:     //  ESTADO 4
                        validarLexema(lexema, fila, columna, "reservado");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 5:     //  ESTADO 5
                        validarLexema(lexema, fila, columna, "reservado");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 6:     //  ESTADO 6
                        validarLexema(lexema, fila, columna, "reservado");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 7:     //  ESTADO 7
                        validarLexema(lexema, fila, columna, "reservado");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 8:     //  ESTADO 8
                        if (comparar(CM, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 10;
                        }
                        else if (comparar(A, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 12;
                        }
                        else
                        {
                            switch (caracterActual)
                            {
                                case ' ':
                                case '\t':
                                case '\b':
                                case '\f':
                                case '\r':
                                    estadoActual = -1;
                                    break;
                                case '\n':
                                    filaActual++;
                                    colActual = 0;
                                    estadoActual = -1;
                                    break;
                                default:

                                    lexema += caracterActual;
                                    estadoActual = -1;
                                    colActual--;
                                    break;
                            }

                        }
                        break;
                    case 9:     //  ESTADO ERROR

                        if (comparar(L, caracterActualStr))
                        {
                            estadoActual = 9;
                            lexema += caracterActual;
                        }
                        else
                        {

                            agregarError(lexema, fila, columna);

                            estadoActual = 0;
                            lexema = "";
                            estadoInicial--;
                            colActual--;

                        }

                        break;

                    case 10:     //  ESTADO 10
                        if (comparar(SL, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 11;
                        }
                        else
                        {
                            lexema += caracterActual;
                            estadoActual = 10;
                        }
                        break;

                    case 11:     //  ESTADO 11
                        validarLexema(lexema, fila, columna, "comentario");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 12:     //  ESTADO 12
                        if (comparar(A, caracterActualStr))
                        {
                            if (comparar(CM, cadena[estadoInicial + 1].ToString()))
                            {
                                lexema += caracterActual;
                                lexema += cadena[estadoInicial + 1].ToString();
                                estadoActual = 13;
                            }
                            else
                            {
                                lexema += caracterActual;
                                estadoActual = 12;
                            }
                        }
                        else
                        {
                            lexema += caracterActual;
                            estadoActual = 12;
                        }
                        break;

                    case 13:     //  ESTADO 13
                        validarLexema(lexema, fila, columna, "comentario");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 14:     //  ESTADO 14
                        if (comparar(B, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 15;
                        }
                        else
                        {
                            lexema += caracterActual;
                            estadoActual = 14;
                        }
                        break;

                    case 15:     //  ESTADO 11
                        validarLexema(lexema, fila, columna, "comentario");
                        estadoActual = 0;
                        lexema = "";
                        estadoInicial--;
                        colActual--;
                        break;
                    case 16:     //  ESTADO 16
                        if (comparar(N, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 17;
                        }
                        else
                        {
                            switch (caracterActual)
                            {
                                case ' ':
                                case '\t':
                                case '\b':
                                case '\f':
                                case '\r':
                                    estadoActual = -1;
                                    break;
                                case '\n':
                                    filaActual++;
                                    colActual = 0;
                                    estadoActual = -1;
                                    break;
                                default:

                                    lexema += caracterActual;
                                    estadoActual = -1;
                                    colActual--;
                                    break;
                            }
                        }

                        break;
                    case 17:     //  ESTADO 17
                        if (comparar(N, caracterActualStr))
                        {
                            lexema += caracterActual;
                            estadoActual = 17;
                        }
                        else
                        {
                            validarLexema(lexema, fila, columna, "numero");
                            estadoActual = 0;
                            lexema = "";
                            estadoInicial--;
                            colActual--;
                        }

                        break;

                    default:
                        agregarError(lexema, fila, columna);
                        estadoInicial--;
                        estadoActual = 0;
                        lexema = "";
                        break;

                }

            }
        }

        string[,] token = new string[,] {
                { "ID", "Token", "Descripcion" },
                { "1", "Numero", "Secuencia de numeros" },
                { "2", "id", "Cadena de numeros y letras" },
                { "3", "Escribir", "Palabra reservada, unción imprime en pantalla" },
                { "4", "asg", "Palabra reservada, asignación de variables" },
                { "5", "tamañolienzo", "Palabra reservada, puede modificar las dimensiones del lienzo, abreviatura 'tl'" },
                { "6", "colorlienzo", "Palabra reservada" },
                { "7", "avanzar", "Palabra reservada" },
                { "8", "retroceder", "Palabra reservada" },
                { "9", "girarIzq", "Palabra reservada" },
                { "10", "girarDer", "Palabra reservada" },
                { "11", "centrar", "Palabra reservada" },
                { "12", "ir", "Palabra reservada" },
                { "13", "irX", "Palabra reservada" },
                { "14", "irY", "Palabra reservada" },
                { "15", "subirPincel", "Palabra reservada" },
                { "16", "bajarPincel", "Palabra reservada" },
                { "17", "colorPincel", "Palabra reservada" },
                { "18", "Operador", "Operador"},
                { "19", ";", "Final de comando"},
                { "20", "(", "Parentesis abierto"},
                { "21", ")", "Parentesis cerrado"},
                { "22", ",", "Coma"},
                { "23", "=", "Signo igual, asignacion"}
            };

        string[,] palabrasReservadas = new string[,] {
                { "3", "Escribir" }, { "3", "escribir" }, { "3", "ESCRIBIR" },
                { "4", "asg" }, { "5", "tl" }, { "5", "tamañolienzo" }, { "6", "colorlienzo" },
                { "6", "cl" }, { "7", "avanzar" }, { "7", "avz" },
                { "8", "retroceder" }, { "8", "ret" }, { "9", "girarIzq" },
                { "9", "izq" }, { "10", "girarDer" }, { "10", "der" },
                { "11", "centrar" }, { "12", "ir" }, { "13", "irX" },
                { "13", "ix" }, { "14", "irY" }, { "14", "iy" },
                { "15", "subirPincel" }, { "15", "spl" }, { "17", "colorPincel" },
                { "16", "bajarPincel" }, { "16", "bpl" }, { "17", "cpl" },
                { "18", "*"}, { "18", "/"},{ "18", "+"},
                { "18", "-"}, { "18", "^"},{ "19", ";"},
                { "20", "("}, { "21", ")"},{ "22", ","},{ "23", "="}
            };

        private string validarLexema(string lexema, int fila, int columna, string tipo)
        {
            // tokens y palabras reservadas

            if (tipo == "numero")   //  Si viene un numero:
            {
                lexema = lexema.Replace(" ", "");
                agregarLexema(token[1, 0], lexema, fila, columna, token[1, 2]);
                return "+ TOKEN: " + lexema + "\t(Fila: " + fila + ", Col: " + columna + ")" + "\tId Token: " + token[1, 0] + "\tToken: " + token[1, 2];

            }
            else if (tipo == "reservado")   //   Si vienen ID o simbolos:
            {
                lexema = lexema.Replace(" ", "");
                for (int i = 0; i < palabrasReservadas.GetLength(0); i++)
                {
                    if (lexema == palabrasReservadas[i, 1])
                    {
                        int id = Int32.Parse(palabrasReservadas[i, 0]);
                        agregarLexema(token[id, 0], lexema, fila, columna, token[id, 2]);
                        return "+ TOKEN: " + lexema + "\t(Fila: " + fila + ", Col: " + columna + ")" + "\tId Token: " + token[id, 0] + "\tToken: " + token[id, 2];

                    }
                }
                agregarLexema(token[2, 0], lexema, fila, columna, token[2, 2]);
                return "+ TOKEN: " + lexema + "\t(Fila: " + fila + ", Col: " + columna + ")" + "\tId Token: " + token[2, 0] + "\tToken: " + token[2, 2];

            }
            else if (tipo == "comentario")   //  Si viene un numero:
            {
                agregarLexema(token[2, 0], lexema, fila, columna, token[2, 2]);
                return "+ TOKEN: " + lexema + "\t(Fila: " + fila + ", Col: " + columna + ")" + "\tId Token: " + token[2, 0] + "\tToken: " + token[2, 2];

            }

            return "ERROR INESPERADO...";
        }

        int num = 1;
        int numError = 1;

        private void agregarLexema(string idToken, string lexema, int fila, int columna, string token)
        {
            Pestania selectTab = Contenedor.SelectedTab as Pestania;
            selectTab.tablaDeSimbolos.Add(new lexema() { id = num, idToken = idToken, nombre = lexema, fila = fila, columna = columna, token = token });
            num++;
        }

        
        private void agregarError(string caracter, int fila, int columna)
        {
            Pestania selectTab = Contenedor.SelectedTab as Pestania;
            selectTab.tablaDeErrores.Add(new error() { id = numError, caracter = caracter, fila = fila, columna = columna });
            numError++;
        }
    }
    

}

public class Pestania : TabPage
{

    public System.Windows.Forms.RichTextBox Codigo;
    public System.Windows.Forms.Label Scroll;
    public System.Windows.Forms.Label Lineas;
    public System.Windows.Forms.TabControl Control;
    public string rutaArchivo = null;
    string rutaTablaDeSimbolos = " ";
    string rutaTablaDeErrores = "";

    public List<lexema> tablaDeSimbolos = new List<lexema>();
    public List<error> tablaDeErrores = new List<error>();

    public Pestania(TabControl Control)
    {
        InitializeComponent(Control);
    }
    private void InitializeComponent( TabControl Control)
    {
        this.Codigo = new System.Windows.Forms.RichTextBox();
        this.Scroll = new System.Windows.Forms.Label();
        this.Lineas = new System.Windows.Forms.Label();

        this.Codigo.Location = new System.Drawing.Point(83, 54);
        this.Codigo.Name = "Codigo";
        this.Codigo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Codigo.Size = new System.Drawing.Size(707, 458);
        this.Codigo.TabIndex = 0;
        this.Codigo.Text = "";
        this.Codigo.WordWrap = false;
        this.Codigo.SelectionChanged += new System.EventHandler(this.Codigo_SelectionChanged);
        this.Codigo.TextChanged += new System.EventHandler(this.Codigo_TextChanged);
        this.Codigo.Click += new System.EventHandler(this.Codigo_Click);


        // Scroll
        // 
        this.Scroll.AutoSize = true;
        this.Scroll.Location = new System.Drawing.Point(360, 28);
        this.Scroll.Name = "Scroll";
        this.Scroll.Size = new System.Drawing.Size(35, 13);
        this.Scroll.TabIndex = 2;
        this.Scroll.Text = "label1";
        // 
        // Lineas
        // 
        this.Lineas.BackColor = System.Drawing.Color.Gray;
        this.Lineas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Lineas.ForeColor = System.Drawing.SystemColors.ButtonFace;
        this.Lineas.Location = new System.Drawing.Point(27, 54);
        this.Lineas.Name = "Lineas";
        this.Lineas.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.Lineas.Size = new System.Drawing.Size(50, 458);
        this.Lineas.TabIndex = 1;
        this.Lineas.Text = "1";
        this.Lineas.Click += new System.EventHandler(Lineas_Click);

        this.Controls.Add(this.Scroll);
        this.Controls.Add(this.Lineas);
        this.Controls.Add(this.Codigo);
        
        // 
    }

    int linea, lineaAnterior, lineaArriba, lineaAbajo;
    public void Linealizar()
    {
        var closestChar = Codigo.SelectionStart;
        linea = Codigo.GetLineFromCharIndex(closestChar) + 1;
        Scroll.Text = Convert.ToString(linea);

        if (linea + 34 == lineaAbajo && linea < lineaAnterior)
        {
            lineaArriba = linea;
            Lineas.Text = "";

            for (int i = linea; i < lineaAbajo; i++)
            {
                Lineas.Text = Lineas.Text + (i) + "\n";
            }
            lineaAbajo--;
        }

        if (linea - 34 == lineaArriba && linea > lineaAnterior)
        {

            Lineas.Text = "";

            for (int i = linea - 33; i <= linea; i++)
            {
                Lineas.Text = Lineas.Text + (i) + "\n";
            }
            lineaArriba++;
            lineaAbajo = linea;
        }
        lineaAnterior = linea;
    }

    private void Codigo_SelectionChanged(object sender, EventArgs e)
    {
        Linealizar();
    }

    private void Codigo_Click(object sender, EventArgs e)
    {
        var closestChar = Codigo.SelectionStart;
        Scroll.Text = Convert.ToString(Codigo.GetLineFromCharIndex(closestChar) + 1);
    }
    private void Codigo_TextChanged(object sender, EventArgs e)
    {

        var closestChar = Codigo.SelectionStart;
        linea = Codigo.GetLineFromCharIndex(closestChar) + 1;

        if (linea == Codigo.Lines.Length)
        {
            Lineas.Text = "";
            if (Codigo.Lines.Length <= 34)
            {
                for (int i = 0; i < Codigo.Lines.Length; i++)
                {
                    Lineas.Text = Lineas.Text + (i + 1) + "\n";
                }
            }
            else
            {
                for (int i = Codigo.Lines.Length - 34; i < Codigo.Lines.Length; i++)
                {
                    Lineas.Text = Lineas.Text + (i + 1) + "\n";
                }
            }
        }



    }
     public void generarTablaDeSimbolos()
        {

            string html = "<center><h1 style=\'text-align: center;\'>" + this.Text + "</h1><h2 style=\'text-align: center;\'>Tabla de simbolos:</h2><hr /><p>​&nbsp;</p><table style=\'width: 800px;\' border=\'1\' cellspacing=\'1\' cellpadding=\'1\'><thead><tr><th scope=\'col\'><span style=\'color: #000000;\'>#</span></th><th scope=\'col\'><span style=\'color: #000000;\'>Lexema</span></th>"
            + "<th scope=\'col\'><span style=\'color: #000000;\'>Fila</span></th><th scope=\'col\'><span style=\'color: #000000;\'>Columna</span></th>"
            + "<th scope=\'col\'><span style=\'color: #000000;\'>Id Token</span></th><th scope=\'col\'><span style=\'color: #000000;\'>Token</span></th>"
            + "</tr></thead><tbody>";

            for (int i = 0; i < tablaDeSimbolos.Count; i++)
            {
                string lexemas = "<tr><td style=\'text-align: center;\'> " + tablaDeSimbolos[i].id + "</td><td style=\'text-align: center;\'> " + tablaDeSimbolos[i].nombre + "</td><td style=\'text-align: center;\'>" + tablaDeSimbolos[i].fila + "</td><td style=\'text-align: center;\'>" + tablaDeSimbolos[i].columna + "</td><td style=\'text-align: center;\'>" + tablaDeSimbolos[i].idToken + "</td><td>" + tablaDeSimbolos[i].token + "</td></tr>";
                html += lexemas;
            }

            html += "</tbody></table><p>&nbsp;</p><hr /><p>&nbsp;</p></center>";

            rutaTablaDeSimbolos = rutaArchivo + "-TablaDeSimbolos.html";

            StreamWriter fichero = new StreamWriter(rutaTablaDeSimbolos);
            fichero.Write(html);
            fichero.Close();
            Process.Start(rutaTablaDeSimbolos);
            
        }
    public void generarTablaDeErrores()
    {

        string html = "<center><h1 style=\'text-align: center;\'>"+this.Text+"</h1><h2 style=\'text-align: center;\'>Tabla de errores:</h2><hr /><p>​&nbsp;</p><table style=\'width: 800px;\' border=\'1\' cellspacing=\'1\' cellpadding=\'1\'><thead><tr><th scope=\'col\'><span style=\'color: #000000;\'>#</span></th><th scope=\'col\'><span style=\'color: #000000;\'>Fila</span></th>"
        + "<th scope=\'col\'><span style=\'color: #000000;\'>Columna</span></th><th scope=\'col\'><span style=\'color: #000000;\'>Caracter</span></th>"
        + "<th scope=\'col\'><span style=\'color: #000000;\'>Id Descripcion</span>"
        + "</tr></thead><tbody>";

        for (int i = 0; i < tablaDeErrores.Count; i++)
        {
            string lexemas = "<tr><td style=\'text-align: center;\'> " + tablaDeErrores[i].id + "</td><td style=\'text-align: center;\'> " + tablaDeErrores[i].fila + "</td><td style=\'text-align: center;\'>" + tablaDeErrores[i].columna + "</td><td style=\'text-align: center;\'>" + tablaDeErrores[i].caracter + "</td><td>Desconocido</td></tr>";
            html += lexemas;
        }

        html += "</tbody></table><p>&nbsp;</p><hr /><p>&nbsp;</p></center>";

        rutaTablaDeErrores = rutaArchivo+"-TablaDeErrores.html";
        StreamWriter fichero = new StreamWriter(rutaTablaDeErrores);
        fichero.Write(html);
        fichero.Close();
        Process.Start(rutaTablaDeErrores);
       

    }


    private void Lineas_Click(object sender, EventArgs e)
    {

    }
}

public class lexema
{
    public int hola;
    public int id { get; set; }
    public string idToken { get; set; }
    public string nombre { get; set; }
    public int fila { get; set; }
    public int columna { get; set; }
    public string token { get; set; }
}

public class error
{
    public int id { get; set; }
    public int fila { get; set; }
    public int columna { get; set; }
    public string caracter { get; set; }

}

public class tab
{
    public int numTab { get; set; }
    public string nomTab { get; set; }
}