﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Proyecto_DCU_R.Facial                        //Hecho por Liam Ramirez - Matricula: 202010449
{
    public static class ConexBD
    {
        private static OleDbConnection cnx = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source =Proyect_DCU_Facial.accdb;");

        public static string[] Nombre;
        public static int[] Edad;
        public static string[] Correo;
        public static string[] Telefono;

        private static byte[] Rostro;
        public static List<byte[]> ListadoRostros = new List<byte[]>();
        public static int TotalRostros;

        public static bool GuardarImagen(string Nombre, int Edad, string Correo, string Telefono, byte[] Imagen)
        {
            cnx.Close();
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("INSERT INTO ROSTROS (Nombre,Edad,Correo,Telefono,Imagen) Values ('" + Nombre + "' , '"+Edad+ "','"+Correo +"','" +Telefono + "',?);", cnx);
            OleDbParameter parImagen = new OleDbParameter("@Imagen", OleDbType.VarBinary, Imagen.Length);
            parImagen.Value = Imagen;
            cmd.Parameters.Add(parImagen);
            int Resultado = cmd.ExecuteNonQuery();
            cnx.Close();

            return Convert.ToBoolean(Resultado);

        }

        public static DataTable Consultar(DataGridView DATA)
        {
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Rostros;", cnx);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DATA.DataSource = dt;
            int Cont = dt.Rows.Count;
            Nombre = new string[Cont];
            Edad = new int[Cont];
            Correo = new string[Cont];
            Telefono = new string[Cont];

            cnx.Close();
            for (int i = 0; i < Cont; i++)
            {
                Nombre[i] = dt.Rows[i]["Nombre"].ToString();
                Edad[i] = Convert.ToInt32(dt.Rows[i]["Edad"].ToString());
                Correo[i] = dt.Rows[i]["Correo"].ToString();
                Telefono[i] = dt.Rows[i]["Telefono"].ToString();
                Rostro = (byte[])dt.Rows[i]["Imagen"];
                ListadoRostros.Add(Rostro);

            }


            try
            {
                DATA.Columns[0].Width = 60;
                DATA.Columns[1].Width = 160;
                DATA.Columns[2].Width = 160;
                DATA.Columns[3].Width = 160;
                DATA.Columns[4].Width = 160;


                for (int i = 0; i < Cont; i++)
                {

                    DATA.Rows[i].Height = 110;
                }
            }
            catch
            {

            }

            TotalRostros = Cont;

            return dt;

        }

        public static byte[] ConvertImgToBinary(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            MemoryStream Memoria = new MemoryStream();
            bmp.Save(Memoria, ImageFormat.Bmp);

            byte[] imagen = Memoria.ToArray();

            return imagen;/// arreglo de Binario de la imagen

        }

        public static Image ConvertBinaryToImg(int C)
        {
            Image Imagen;
            byte[] img = ListadoRostros[C];
            MemoryStream Memoria = new MemoryStream(img);
            Imagen = Image.FromStream(Memoria);
            Memoria.Close();
            return Imagen;
        }


        public static void listaCaras(DataGridView data)
        {
            cnx.Open();
            OleDbCommand comando = new OleDbCommand("SELECT * FROM Rostros", cnx);
            comando.Connection = cnx;
            comando.ExecuteNonQuery();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter(comando);
            da.Fill(dt);
            data.DataSource = dt;
            data.Columns[0].Width = 60;
            data.Columns[1].Width = 165;
            data.Columns[2].Width = 165;
            int cont = data.RowCount;
            int i;
            for (i = 0; i < cont; i++)
            {
                data.Rows[i].Height = 110;

            }


            cnx.Close();
        }

        public static void eliminar(int Id)
        {
            DialogResult resultado = MessageBox.Show("¿Estas Seguro de Eliminar el Registro Seleccionado?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.No)
            {
                return;
            }
            OleDbCommand cmd = new OleDbCommand("DELETE FROM Rostros WHERE ID =" + Id, cnx);


            cnx.Open();
            cmd.ExecuteNonQuery();
            cnx.Close();
            MessageBox.Show("Borrado Exitoso", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }


    }
}

