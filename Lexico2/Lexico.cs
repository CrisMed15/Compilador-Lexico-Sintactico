using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace Lexico2
{
    public partial class Lexico : Form
    {
        int CARACTER;
        public Lexico()
        {
            InitializeComponent();
        }


        RegexLexer lexer = new RegexLexer();
        bool load;
        List<string> palabrasReservadas;

        private void AnalizarCodigo()
        {
            dgvTabla.Rows.Clear();
            int n = 0, e = 0;

            foreach (var tk in lexer.GetTokens(textoEntrada.Text))
            {
                if (tk.Name == "ERROR") e++;

                if (tk.Name == "IDENTIFICADOR")
                    if (palabrasReservadas.Contains(tk.Lexema))
                        tk.Name = "RESERVADO";
                dgvTabla.Rows.Add(tk.Name, tk.Lexema, tk.Index, tk.Linea, tk.Columna);
                n++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
        }

        private void Lexico_Load_1(object sender, EventArgs e)
        {
            timer1.Interval = 10;
            timer1.Start();
            //using (System.IO.StreamReader sr = new StreamReader(@"C:\Users\Cristopher Medina\source\repos\Lexico2\Lexico2\RegexLexer.cs"))
            //{
                lexer.AddTokenRule(@"\s+", "ESPACIO", true);
                lexer.AddTokenRule(@"\b[_a-zA-Z][\w]*\b", "IDENTIFICADOR");
                lexer.AddTokenRule("\".*?\"", "CADENA");
                lexer.AddTokenRule(@"'\\.'|'[^\\]'", "CARACTER");
                lexer.AddTokenRule(@"//[^\r\n]*", "COMENTARIO1");
                lexer.AddTokenRule(@"/[*].*?[*]/", "COMENTARIO2");
                lexer.AddTokenRule(@"\d*\.?\d+", "NUMERO");
                lexer.AddTokenRule(@"[\(\)\{\}\[\];,]", "DELIMITADOR");
                lexer.AddTokenRule(@"[\.=\+\-/*%]", "OPERADOR");
                lexer.AddTokenRule(@">|<|==|>=|<=|!", "COMPARADOR");

                palabrasReservadas = new List<string>() { "abstract",  "async", "await",
                "checked",   "default", "break", "case",
                "do", "else", "event", "explicit", "extends", "false", "finally",
                "fixed", "for", "foreach", "if", "implicit","implements", "in", "interface",
                 "new", "null", "catch",
                "out", "override",  "private", "protected", "public", 
                 "return", "static",
                "switch", "this", "throw", "true", "try",
                "void", "while", "float", "int", "long", "object",
                "get", "set", "new", "yield", "add", "remove", "value",
                "from", "group", "into", "orderby", "select", "where",
                "join", "equals","boolean", "byte", "char", "decimal", "double", "dynamic",
                "short", "string", "var", "class","import","final","package","super","throws"};

                lexer.Compile(RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                load = true;
                AnalizarCodigo();
                textoEntrada.Focus();
           // }
        }

        private void dgvTabla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            CARACTER = 0; //SE INICIALIZA A 0 EN CADA REPINTADO
            int ALTURA = textoEntrada.GetPositionFromCharIndex(0).Y - 1;//coordenada del primer caracter
            Color Verde = Color.FromArgb(97, 236, 243);
            SolidBrush myBrush = new SolidBrush(Verde);


            if (textoEntrada.Lines.Length > 0)//si hay alguna linea las recorrera todas y escribira numero
            {
                for (int i = 0; i < textoEntrada.Lines.Length; i++)
                {
                    e.Graphics.DrawString(Convert.ToString(i + 1), textoEntrada.Font, myBrush, pictureBox2.Width - (e.Graphics.MeasureString(Convert.ToString(i + 1), textoEntrada.Font).Width + 10), ALTURA);
                    CARACTER = CARACTER + textoEntrada.Lines[i].Length + 1;
                    ALTURA = textoEntrada.GetPositionFromCharIndex(CARACTER).Y;
                }
            }
            else
            {
                e.Graphics.DrawString("1", textoEntrada.Font, myBrush, pictureBox2.Width - (e.Graphics.MeasureString("1", textoEntrada.Font).Width + 10), ALTURA);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox2.Refresh();
        }

        private void textoEntrada_TextChanged(object sender, EventArgs e)
        {
            AnalizarCodigo();
            txtConsola.Text = "";
            Sintactico.imprimirerrores = "";
            bool resultado = Sintactico.Parse(textoEntrada.Text);
            string textoResultado;
            if (resultado)
            {
                txtConsola.ForeColor = Color.Green;
                textoResultado = "Compilación Sintáctica Correcta";
            }
            else
            {
                txtConsola.ForeColor = Color.Red;
                textoResultado = Sintactico.imprimirerrores;
            }
            txtConsola.Text = textoResultado;

        }



        private void btnCompilar_Click(object sender, EventArgs e)
        {

        }

        private void txtConsola_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (textoEntrada.SelectedText != "")
            {
                Clipboard.SetDataObject(textoEntrada.SelectedText);
            }
            else
            {
                Clipboard.SetDataObject(textoEntrada.Text);
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                if (textoEntrada.SelectedText != "")
                {
                   textoEntrada.SelectedText = Clipboard.GetText();
                }
                else
                {
                    textoEntrada.Text += Clipboard.GetText();
                }

            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JAVA|*.java";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, textoEntrada.Text);
            }
        }

        OpenFileDialog ofd = new OpenFileDialog();
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            ofd.Filter = "JAVA|*.java";
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                string direccion = ofd.FileName;
                StreamReader leer = new StreamReader(@direccion);
                string linea;
                try
                {
                    textoEntrada.Text = "";
                    dgvTabla.Rows.Clear();
                    linea = leer.ReadLine();
                    while (linea != null)
                    {
                        textoEntrada.AppendText(linea + "\n");
                        linea = leer.ReadLine();
                    }
                }
                catch
                {
                    MessageBox.Show("Error");
                }
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea borrar todo el texto?", "FLASHCODE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dgvTabla.Rows.Clear();
                textoEntrada.Clear();
            }
        }
    }
}
