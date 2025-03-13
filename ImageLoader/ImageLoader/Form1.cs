using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLoader
{
    public partial class Form1 : Form
    {

        Bitmap img1;
        Bitmap img2;
        byte[,] vImg1Gray;

        byte[,] vImg1R;
        byte[,] vImg1G;
        byte[,] vImg1B;
        byte[,] vImg1A;

        public Form1()
        {
            InitializeComponent();
        }

        private void btImg1_Click(object sender, EventArgs e)
        {
            // Configurações iniciais da OpenFileDialogBox
            var filePath = string.Empty;
            openFileDialog1.InitialDirectory = "C:\\Matlab";
            openFileDialog1.Filter = "TIFF image (*.tif)|*.tif|JPG image (*.jpg)|*.jpg|BMP image (*.bmp)|*.bmp|PNG image (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            // Se um arquivo foi localizado com sucesso...
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Armnazena o path do arquivo de imagem
                filePath = openFileDialog1.FileName;


                bool bLoadImgOK = false;
                try
                {
                    img1 = new Bitmap(filePath);
                    img2 = new Bitmap(img1.Width, img1.Height);
                    bLoadImgOK = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro ao abrir imagem...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bLoadImgOK = false;
                }

                // Se a imagem carregou perfeitamente...
                if (bLoadImgOK == true)
                {
                    // Adiciona imagem na PictureBox
                    pictureBox1.Image = img1;
                    vImg1Gray = new byte[img1.Width, img1.Height];
                    vImg1R = new byte[img1.Width, img1.Height];
                    vImg1G = new byte[img1.Width, img1.Height];
                    vImg1B = new byte[img1.Width, img1.Height];
                    vImg1A = new byte[img1.Width, img1.Height];

                    // Percorre todos os pixels da imagem...
                    for (int i = 0; i < img1.Width; i++)
                    {
                        for (int j = 0; j < img1.Height; j++)
                        {
                            Color pixel = img1.GetPixel(i, j);

                            // Para imagens em escala de cinza, extrair o valor do pixel com...
                            //byte pixelIntensity = Convert.ToByte((pixel.R + pixel.G + pixel.B) / 3);
                            byte pixelIntensity = Convert.ToByte((pixel.R + pixel.G + pixel.B) / 3);
                            vImg1Gray[i, j] = pixelIntensity;

                            // Para imagens RGB, extrair o valor do pixel com...
                            byte R = pixel.R;
                            byte G = pixel.G;
                            byte B = pixel.B;
                            byte A = pixel.A;

                            vImg1R[i, j] = R;
                            vImg1G[i, j] = G;
                            vImg1B[i, j] = B;
                            vImg1A[i, j] = A;

                            Color cor = Color.FromArgb(
                                255,
                                vImg1Gray[i, j],
                                vImg1Gray[i, j],
                                vImg1Gray[i, j]);

                            img2.SetPixel(i, j, cor);
                        }
                    }

                    pictureBox2.Image = img1;
                }

            }
        }

        // Variável para rastrear o valor acumulado
        public int accumulatedValue = 0;

        // Botão de aumentar pixels
        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtAddValue.Text, out int valueToAdd))
            {
                if (valueToAdd > 255)
                {
                    MessageBox.Show("O valor não pode ser maior que 255.");
                    return;
                }

                // Acumula o valor a ser somado
                accumulatedValue += valueToAdd;

                if(accumulatedValue > 255)
                {
                    accumulatedValue = 255;
                }

                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        // Obtém a cor do pixel
                        Color pixelColor = img1.GetPixel(i, j);

                        // Ajusta cada componente RGB, garantindo que o valor não ultrapasse 255
                        int newRed = pixelColor.R + accumulatedValue;
                        int newGreen = pixelColor.G + accumulatedValue;
                        int newBlue = pixelColor.B + accumulatedValue;

                        // Garantir que o valor de cada componente fique dentro do intervalo 0-255
                        newRed = Math.Min(Math.Max(newRed, 0), 255);
                        newGreen = Math.Min(Math.Max(newGreen, 0), 255);
                        newBlue = Math.Min(Math.Max(newBlue, 0), 255);

                        // Cria uma nova cor com os valores ajustados
                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);

                        // Define o novo valor do pixel
                        img2.SetPixel(i, j, newColor);
                    }
                }

                // Atualiza a imagem no PictureBox
                pictureBox2.Image = img2;
            }
            else
            {
                MessageBox.Show("Por favor, insira um valor válido.");
            }
        }

        // Botão de diminuir pixels
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (int.TryParse(txtSubValue.Text, out int valueToSubtract))
            {
                if (valueToSubtract < 0)
                {
                    // Caso o valor seja menor que 0, exibe uma mensagem ou trata o erro
                    MessageBox.Show("O valor não pode ser menor que 0.");
                    return; // Interrompe a execução do método
                }
                // Subtrai o valor acumulado
                accumulatedValue -= valueToSubtract;

                if (accumulatedValue < 0)
                {
                    accumulatedValue = 0;
                    MessageBox.Show("O valor não pode ser menor que 0.");
                }

                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        // Obtém a cor do pixel
                        Color pixelColor = img1.GetPixel(i, j);

                        // Ajusta cada componente RGB, garantindo que o valor não fique abaixo de 0
                        int newRed = pixelColor.R - accumulatedValue;
                        int newGreen = pixelColor.G - accumulatedValue;
                        int newBlue = pixelColor.B - accumulatedValue;

                        // Garantir que o valor de cada componente fique dentro do intervalo 0-255
                        newRed = Math.Min(Math.Max(newRed, 0), 255);
                        newGreen = Math.Min(Math.Max(newGreen, 0), 255);
                        newBlue = Math.Min(Math.Max(newBlue, 0), 255);

                        // Cria uma nova cor com os valores ajustados
                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);

                        // Define o novo valor do pixel
                        img2.SetPixel(i, j, newColor);
                    }
                }

                // Atualiza a imagem no PictureBox
                pictureBox2.Image = img2;
            }
            else
            {
                MessageBox.Show("Por favor, insira um valor válido.");
            }
        }


        private void resp_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OperacaoImagem(bool somar)
        {
            if (img1 == null || img2 == null)
            {
                MessageBox.Show("Carregue duas imagens antes de realizar a operação.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (img1.Width != img2.Width || img1.Height != img2.Height)
            {
                MessageBox.Show("As imagens precisam ter o mesmo tamanho!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap resultado = new Bitmap(img1.Width, img1.Height);

            for (int i = 0; i < img1.Width; i++)
            {
                for (int j = 0; j < img1.Height; j++)
                {
                    Color pixel1 = img1.GetPixel(i, j);
                    Color pixel2 = img2.GetPixel(i, j);

                    int newRed = somar ? pixel1.R + pixel2.R : pixel1.R - pixel2.R;
                    int newGreen = somar ? pixel1.G + pixel2.G : pixel1.G - pixel2.G;
                    int newBlue = somar ? pixel1.B + pixel2.B : pixel1.B - pixel2.B;

                    // Garantindo que os valores estejam entre 0 e 255
                    newRed = Math.Min(255, Math.Max(0, newRed));
                    newGreen = Math.Min(255, Math.Max(0, newGreen));
                    newBlue = Math.Min(255, Math.Max(0, newBlue));

                    resultado.SetPixel(i, j, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }

            pictureBox2.Image = resultado;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // Configurações iniciais da OpenFileDialogBox
            var filePath = string.Empty;
            openFileDialog1.InitialDirectory = "C:\\Matlab";
            openFileDialog1.Filter = "TIFF image (*.tif)|*.tif|JPG image (*.jpg)|*.jpg|BMP image (*.bmp)|*.bmp|PNG image (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            // Se um arquivo foi localizado com sucesso...
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Armazena o path do arquivo de imagem
                filePath = openFileDialog1.FileName;

                bool bLoadImgOK = false;
                try
                {
                    img1 = new Bitmap(filePath);
                    img2 = new Bitmap(img1.Width, img1.Height);
                    bLoadImgOK = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro ao abrir imagem...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bLoadImgOK = false;
                }

                // Se a imagem carregou corretamente...
                if (bLoadImgOK == true)
                {
                    // Atribui a imagem carregada no PictureBox 3
                    pictureBox3.Image = img1;

                    vImg1Gray = new byte[img1.Width, img1.Height];
                    vImg1R = new byte[img1.Width, img1.Height];
                    vImg1G = new byte[img1.Width, img1.Height];
                    vImg1B = new byte[img1.Width, img1.Height];
                    vImg1A = new byte[img1.Width, img1.Height];

                    // Percorre todos os pixels da imagem...
                    for (int i = 0; i < img1.Width; i++)
                    {
                        for (int j = 0; j < img1.Height; j++)
                        {
                            Color pixel = img1.GetPixel(i, j);

                            // Para imagens em escala de cinza, extrair o valor do pixel...
                            byte pixelIntensity = Convert.ToByte((pixel.R + pixel.G + pixel.B) / 3);
                            vImg1Gray[i, j] = pixelIntensity;

                            // Para imagens RGB, extrair o valor do pixel...
                            byte R = pixel.R;
                            byte G = pixel.G;
                            byte B = pixel.B;
                            byte A = pixel.A;

                            vImg1R[i, j] = R;
                            vImg1G[i, j] = G;
                            vImg1B[i, j] = B;
                            vImg1A[i, j] = A;

                            Color cor = Color.FromArgb(255, vImg1Gray[i, j], vImg1Gray[i, j], vImg1Gray[i, j]);

                            img2.SetPixel(i, j, cor);
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Verifica se as imagens foram carregadas
            if (img1 != null && img2 != null)
            {
                // Cria uma nova imagem do tamanho máximo entre as duas imagens
                int width = Math.Max(img1.Width, img2.Width);
                int height = Math.Max(img1.Height, img2.Height);
                Bitmap resultImg = new Bitmap(width, height);

                // Cria um objeto Graphics para desenhar na nova imagem
                using (Graphics g = Graphics.FromImage(resultImg))
                {
                    // Define o modo de mistura de imagens (Alpha Blending)
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    // Desenha a imagem de fundo
                    g.DrawImage(img1, 0, 0, img1.Width, img1.Height);

                    // Define a transparência da imagem do menino (Alpha)
                    float alpha = 0.5f; // O valor pode ser entre 0 (totalmente transparente) e 1 (totalmente opaco)

                    // Cria uma imagem de "máscara" com a transparência desejada
                    ColorMatrix colorMatrix = new ColorMatrix();
                    colorMatrix.Matrix33 = alpha; // Aplique a transparência apenas na imagem do menino

                    // Aplica a matriz de cores à imagem do menino
                    ImageAttributes imgAttr = new ImageAttributes();
                    imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    // Desenha a imagem do menino com transparência sobre o fundo
                    g.DrawImage(img2, new Rectangle(0, 0, img2.Width, img2.Height), 0, 0, img2.Width, img2.Height, GraphicsUnit.Pixel, imgAttr);
                }

                // Exibe a imagem resultante no PictureBox
                pictureBox2.Image = resultImg;
            }
            else
            {
                MessageBox.Show("As imagens não foram carregadas corretamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Mesclar Imagens
        private void button5_Click(object sender, EventArgs e)
        {
           
        }

        //Salvar Imagem
        private void button6_Click(object sender, EventArgs e)
        {
            // Cria um SaveFileDialog para o usuário escolher onde salvar a imagem
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Define os filtros para salvar arquivos de imagem (por exemplo, PNG ou JPEG)
            saveFileDialog.Filter = "Arquivos PNG|*.png|Arquivos JPEG|*.jpg;*.jpeg|Todos os Arquivos|*.*";
            saveFileDialog.Title = "Salvar Imagem";

            // Exibe o dialog para o usuário escolher onde salvar a imagem
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Salva a imagem no caminho especificado
                    img2.Save(saveFileDialog.FileName);
                    MessageBox.Show("Imagem salva com sucesso!");
                }
                catch (Exception ex)
                {
                    // Se ocorrer algum erro ao salvar a imagem
                    MessageBox.Show("Erro ao salvar a imagem: " + ex.Message);
                }
            }
        }
    }
}
