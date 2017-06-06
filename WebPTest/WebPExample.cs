﻿// clsWebP, by Jose M. Piñeiro
// Website: https://github.com/JosePineiro/WebP-wapper
// Version: 1.0.0.4 (Jun 6, 2017)

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WebPWrapper;


namespace WebPTest
{
    public partial class WebPExample : Form
    {
        #region | Constructors |
        public WebPExample()
        {
            InitializeComponent();
        }

        private void WebPExample_Load(object sender, EventArgs e)
        {
            try
            {
                //Inform of execution mode
                if (IntPtr.Size == 8)
                    this.Text = Application.ProductName + " x64 " + Application.ProductVersion;
                else
                    this.Text = Application.ProductName + " x86 " + Application.ProductVersion;

                //Inform of libWebP version
                using (WebP webp = new WebP())
                    this.Text = this.Text + " (libwebp v" + webp.GetVersion() + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.WebPExample_Load", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region << Events >>
        /// <summary>Test for load from file function</summary>
        private void buttonLoad_Click(object sender, System.EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "Image files (*.webp, *.png, *.tif, *.tiff)|*.webp;*.png;*.tif;*.tiff";
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        this.buttonSave.Enabled = true;
                        this.buttonSave.Enabled = true;
                        string pathFileName = openFileDialog.FileName;

                        if (Path.GetExtension(pathFileName) == ".webp")
                        {
                            using (WebP webp = new WebP())
                                this.pictureBox.Image = webp.Load(pathFileName);
                        }
                        else
                            this.pictureBox.Image = Image.FromFile(pathFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonLoad_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Test for load thumbnail function</summary>
        private void buttonThumbnail_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "WebP files (*.webp)|*.webp";
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string pathFileName = openFileDialog.FileName;

                        byte[] rawWebP = File.ReadAllBytes(pathFileName);
                        using (WebP webp = new WebP())
                            this.pictureBox.Image = webp.Thumbnail(rawWebP, 200, 150);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonThumbnail_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Test for advanced decode function</summary>
        private void buttonCropFlip_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "WebP files (*.webp)|*.webp";
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string pathFileName = openFileDialog.FileName;

                        byte[] rawWebP = File.ReadAllBytes(pathFileName);
                        WebPDecoderOptions decoderOptions = new WebPDecoderOptions();
                        decoderOptions.use_cropping = 1;
                        decoderOptions.crop_top = 100;       //Top beging of crop area
                        decoderOptions.crop_left = 100;      //Left beging of crop area
                        decoderOptions.crop_height = 400;   //Height of crop area
                        decoderOptions.crop_width = 400;    //Width of crop area
                        decoderOptions.use_threads = 1;     //Use multhreading
                        decoderOptions.flip = 1;            //Flip the image
                        using (WebP webp = new WebP())
                            this.pictureBox.Image = webp.Decode(rawWebP, decoderOptions);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonCrop_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            byte[] rawWebP;

            try
            {
                if (this.pictureBox.Image == null)
                    MessageBox.Show("Please, load an image first");

                //get the picturebox image
                Bitmap bmp = (Bitmap)pictureBox.Image;

                //Test simple encode lossly mode in memory with quality 75
                string lossyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SimpleLossy.webp");
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossy(bmp, 75);
                File.WriteAllBytes(lossyFileName, rawWebP);
                MessageBox.Show("Made " + lossyFileName);

                //Test encode lossly mode in memory with quality 75 and speed 9
                string advanceLossyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AdvanceLossy.webp");
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossy(bmp, 71, 9, true);
                File.WriteAllBytes(advanceLossyFileName, rawWebP);
                MessageBox.Show("Made " + advanceLossyFileName);
                
                //Test simple encode lossless mode in memory
                string simpleLosslessFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SimpleLossless.webp");
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossless(bmp);
                File.WriteAllBytes(simpleLosslessFileName, rawWebP);
                MessageBox.Show("Made " + simpleLosslessFileName);
                
                //Test advance encode lossless mode in memory with speed 9
                string losslessFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AdvanceLossless.webp");
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeLossless(bmp, 9, true);
                File.WriteAllBytes(losslessFileName, rawWebP);
                MessageBox.Show("Made " + losslessFileName);
                
                //Test encode near lossless mode in memory with quality 40 and speed 9
                // quality 100: No-loss (bit-stream same as -lossless).
                // quality 80: Very very high PSNR (around 54dB) and gets an additional 5-10% size reduction over WebP-lossless image.
                // quality 60: Very high PSNR (around 48dB) and gets an additional 20%-25% size reduction over WebP-lossless image.
                // quality 40: High PSNR (around 42dB) and gets an additional 30-35% size reduction over WebP-lossless image.
                // quality 20 (and below): Moderate PSNR (around 36dB) and gets an additional 40-50% size reduction over WebP-lossless image.
                string nearLosslessFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NearLossless.webp");
                using (WebP webp = new WebP())
                    rawWebP = webp.EncodeNearLossless(bmp, 40, 9, true);
                File.WriteAllBytes(nearLosslessFileName, rawWebP);
                MessageBox.Show("Made " + nearLosslessFileName);

                MessageBox.Show("End of Test");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonSave_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonMeasure_Click(object sender, EventArgs e)
        {
            if (this.pictureBox.Image == null)
                MessageBox.Show("Please, load an reference image first");

            using (OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "WebP images (*.webp)|*.webp";
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap source;
                    Bitmap reference;
                    float[] result;

                    //Load Bitmaps
                    source = (Bitmap)this.pictureBox.Image;
                    using (WebP webp = new WebP())
                        reference = webp.Load(openFileDialog.FileName);

                    //Measure PSNR
                    using (WebP webp = new WebP())
                        result = webp.GetPictureDistortion(source, reference, 0);
                    MessageBox.Show("Red: " + result[0] + "dB.\nGreen: " + result[1] + "dB.\nBlue: " + result[2] + "dB.\nAlpha: " + result[3] + "dB.\nAll: " + result[4] + "dB.", "PSNR");

                    //Measure SSIM
                    using (WebP webp = new WebP())
                        result = webp.GetPictureDistortion(source, reference, 1);
                    MessageBox.Show("Red: " + result[0] + "dB.\nGreen: " + result[1] + "dB.\nBlue: " + result[2] + "dB.\nAlpha: " + result[3] + "dB.\nAll: " + result[4] + "dB.", "SSIM");

                    //Measure LSIM
                    using (WebP webp = new WebP())
                        result = webp.GetPictureDistortion(source, reference, 2);
                    MessageBox.Show("Red: " + result[0] + "dB.\nGreen: " + result[1] + "dB.\nBlue: " + result[2] + "dB.\nAlpha: " + result[3] + "dB.\nAll: " + result[4] + "dB.", "LSIM");
                }
            }
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            int width;
            int height;
            bool has_alpha;
            bool has_animation;
            string format;

            try
            {
                using (OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "WebP images (*.webp)|*.webp";
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string pathFileName = openFileDialog.FileName;

                        byte[] rawWebp = File.ReadAllBytes(pathFileName);
                        using (WebP webp = new WebP())
                            webp.GetInfo(rawWebp, out width, out height, out has_alpha, out has_animation, out format);
                        MessageBox.Show("Width: " + width + "\n" +
                                        "Height: " + height + "\n" +
                                        "Has alpha: " + has_alpha + "\n" +
                                        "Is animation: " + has_animation + "\n" +
                                        "Format: " + format, "Information");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nIn WebPExample.buttonInfo_Click", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}

