using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace MNIST_READER
{
    public class MNISTFile
    {

        public string FilePath;
        public string LABELS_PATH;
        public string Name
        { get => FilePath; }
        public string LABELS_FILEPATH
        {
            get => LABELS_PATH;
        }


        public string[] READ_MNIST_TRAIN_LABELS()
        {
            try
            {
                using (FileStream fs = new FileStream(Name, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(fs, new ASCIIEncoding()))
                    {

                        byte[] buffer = new byte[4];
                        buffer = reader.ReadBytes(4);
                        Array.Reverse(buffer);
                        int MagicNumber = BitConverter.ToInt32(buffer, 0);
                        if (MagicNumber == 2049)
                        {
                            byte[] buffer1 = new byte[4];
                            buffer1 = reader.ReadBytes(4);
                            Array.Reverse(buffer1);
                            int NumberOfElements = BitConverter.ToInt32(buffer1, 0);

                            byte[] Result = new byte[60000];
                            string[] results = new string[60000];

                            for (int i = 0; i < NumberOfElements - 1; i++)
                            {
                                Result[i] = reader.ReadByte();
                            }
                            for (int i = 0; i < Result.Length - 1; i++)
                            {
                                results[i] = Convert.ToString(Convert.ToInt32(Result[i]));
                            }
                            return results;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Int32[,]> READ_MNIST_PICTURES()
        {
            int LastCommaPos = 0;
            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == '.') LastCommaPos = i;
            }
            LastCommaPos++;
            string FILE_EXTENSION = Name.Substring(LastCommaPos);
            if (FILE_EXTENSION == "idx3-ubyte")
            {
                using (FileStream fs = new FileStream(Name, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(fs, new ASCIIEncoding()))
                    {
                        try
                        {
                            byte[] MAGIC_NUMBER = new byte[4];
                            MAGIC_NUMBER = reader.ReadBytes(4);
                            Array.Reverse(MAGIC_NUMBER);
                            Int32 MAGIC_NUMBER_INT = BitConverter.ToInt32(MAGIC_NUMBER, 0);
                            if (MAGIC_NUMBER_INT == 2051)
                            {
                                byte[] NUMBER_OF_ELEMENTS = new byte[4];
                                NUMBER_OF_ELEMENTS = reader.ReadBytes(4);
                                Array.Reverse(NUMBER_OF_ELEMENTS);
                                Int32 NUMBER_OF_ELEMENTS_INT = BitConverter.ToInt32(NUMBER_OF_ELEMENTS, 0);

                                byte[] NUMBER_OF_ROWS = new byte[4];
                                NUMBER_OF_ROWS = reader.ReadBytes(4);
                                Array.Reverse(NUMBER_OF_ROWS);
                                Int32 NUMBER_OF_ROWS_INT = BitConverter.ToInt32(NUMBER_OF_ROWS, 0);

                                byte[] NUMBER_OF_COLUMNS = new byte[4];
                                NUMBER_OF_COLUMNS = reader.ReadBytes(4);
                                Array.Reverse(NUMBER_OF_COLUMNS);
                                Int32 NUMBER_OF_COLUMNS_INT = BitConverter.ToInt32(NUMBER_OF_COLUMNS, 0);

                                List<Int32[,]> Results = new List<int[,]>();

                                for (int i = 0; i < NUMBER_OF_ELEMENTS_INT; i++)
                                {
                                    byte CURRENT_BYTE = new byte();
                                    Int32[,] TEMP = new Int32[NUMBER_OF_ROWS_INT, NUMBER_OF_COLUMNS_INT];
                                    for (int j = 0; j < NUMBER_OF_ROWS_INT; j++)
                                    {
                                        for (int k = 0; k < NUMBER_OF_COLUMNS_INT; k++)
                                        {
                                            CURRENT_BYTE = reader.ReadByte();
                                            TEMP[j, k] = Convert.ToInt32(CURRENT_BYTE);
                                        }
                                    }
                                    Results.Add(TEMP);
                                }
                                return Results;

                            }

                            return null;
                        }
                        catch (IOException e)
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

        }
        public Object GET_PICTURE(int offset)
        {
            List<int[,]> Pictures = new List<int[,]>();
            Pictures = READ_MNIST_PICTURES();
            string[] Labels = READ_MNIST_TRAIN_LABELS();
            Bitmap CURRENT_PICTURE = new Bitmap(Pictures[offset].GetLength(0), Pictures[offset].GetLength(1));
            for (int i = 0; i < Pictures[offset].GetLength(0); i++)
            {
                for (int j = 0; j < Pictures[offset].GetLength(1); j++)
                {
                    Color Current_Col = Color.FromArgb(Pictures[offset][i,j], Pictures[offset][i, j], Pictures[offset][i, j]);
                    CURRENT_PICTURE.SetPixel(i, j, Current_Col);
                }
            }
            Object[] RESULT = new Object[] {Labels[offset],CURRENT_PICTURE };
            return RESULT;
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            MNISTFile FilE = new MNISTFile();
            FilE.FilePath = "";
            FilE.LABELS_PATH = "";
            List<int[,]> Pictures = FilE.READ_MNIST_PICTURES();
            string[] Labels = FilE.READ_MNIST_TRAIN_LABELS();
            InitializeComponent();
        }
    }
}
