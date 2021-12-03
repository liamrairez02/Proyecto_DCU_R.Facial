using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_DCU_R.Facial
{
    public partial class FormRegistros : Form          //Hecho por Liam Ramirez - Matricula: 202010449
    {

        //variables vectores y Harcascades
        int con = 0;
        Image<Bgr, Byte> currentFrame;
        Capture Grabar;
        HaarCascade face;//Rostro
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.4d, 0.4d);
        Image<Gray, byte> result, TraineFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NombrePersonas = new List<string>();
        int ContTrain, numLabels, t;
        string Nombre;
        DataGridView d = new DataGridView();

        private void FrameGrabar(object sender, EventArgs e)
        {
            labelCantidad.Text = "0";
            NombrePersonas.Add("");

            try
            {
                currentFrame = Grabar.QueryFrame().Resize(392, 381, INTER.CV_INTER_CUBIC);

                gray = currentFrame.Convert<Gray, Byte>();

                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.5, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach (MCvAvgComp R in RostrosDetectados[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(R.rect, new Bgr(Color.Green), 1);


                    NombrePersonas[t - 1] = Nombre;
                    NombrePersonas.Add("");

                    labelCantidad.Text = RostrosDetectados[0].Length.ToString();
                }

                t = 0;
                imageBox1.Image = currentFrame;
                Nombre = "";
                NombrePersonas.Clear();
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }

        }

        private void reconocer()
        {
            try
            {
                Grabar = new Capture();
                Grabar.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabar);
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }
        }

        private void DetenerReconocer()
        {
            try
            {

                Application.Idle -= new EventHandler(FrameGrabar);
                Grabar.Dispose();
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void FormRegistros_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetenerReconocer();
        }

        private void imageBox2_Click(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void buttonRegistrar_Click(object sender, EventArgs e)
        {
            if (textNombre.Text != "")
            {
                labels.Add(textNombre.Text);
                labels.Add(textEdad.Text);
                labels.Add(textCorreo.Text);
                labels.Add(textTelefono.Text);

                ConexBD.GuardarImagen(textNombre.Text, Convert.ToInt32(this.textEdad.Text), textCorreo.Text, textTelefono.Text, ConexBD.ConvertImgToBinary(imageBox2.Image.Bitmap));


            }

            ConexBD.Consultar(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        

        private void buttonEliminar_Click(object sender, EventArgs e)
        {

            ConexBD.eliminar(int.Parse(dataGridView1.CurrentRow.Cells["id"].Value.ToString()));
            ConexBD.listaCaras(dataGridView1);

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        void limpiar()
        {
            imageBox1.Image = null;
            pictureBox1.Image = null;
            textNombre.Clear();
            textEdad.Clear();
            textCorreo.Clear();
            textTelefono.Clear();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void textNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonCapturar_Click(object sender, EventArgs e)
        {
          
               try
            {
                ContTrain += ContTrain;
                gray = currentFrame.Convert<Gray, Byte>();

                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach (MCvAvgComp R in RostrosDetectados[0])
                {

                    TraineFace = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    break;

                }
                TraineFace = result.Resize(100, 100, INTER.CV_INTER_CUBIC);
                trainingImages.Add(TraineFace);

                imageBox2.Image = TraineFace;
            }
            catch
            {

            }
                
               
        }

        public FormRegistros()
        {
            InitializeComponent();

            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                ConexBD.Consultar(d);

                string[] Labels = ConexBD.Nombre;
                numLabels = ConexBD.TotalRostros;
                ContTrain = numLabels;

                for (int i = 0; i < numLabels; i++)
                {
                    con = i;
                    Bitmap bmp = new Bitmap(ConexBD.ConvertBinaryToImg(con));

                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[i]);

                }

            }
            catch (Exception e)
            {

                MessageBox.Show("Error"+ e);
            }
        }

        private void FormRegistros_Load(object sender, EventArgs e)
        {
            reconocer();
            ConexBD.Consultar(dataGridView1);

        }
    }
}
