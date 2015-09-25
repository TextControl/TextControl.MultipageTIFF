using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace tx_import_multipage_tiff
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void InsertImageOnSamePage_Click(object sender, EventArgs e)
        {
            InsertTIFF("image.tif", false);
        }

        private void InsertImageOnNewPage_Click(object sender, EventArgs e)
        {
            InsertTIFF("image.tif", true);
        }

        /***************************************************************
         * InsertTIFF method
         * description: Insert a new section for each page and inserts
         *              the images
         * 
         * param:       image - the image path
         *              startAtNewPage - specifies whether the first
         *              image is inserted on a new page or not
         ***************************************************************/
        private void InsertTIFF(string image, bool startAtNewPage)
        {
            // make sure the page unit is twips
            textControl1.PageUnit = TXTextControl.MeasuringUnit.Twips;

            bool bFirstPage = false;

            // retrieve the images
            List<Image> images = SplitMultipageTIFF(image);

            foreach (Image img in images)
            {
                // create a new TXTextControl.Image from each image
                TXTextControl.Image TXImg = new TXTextControl.Image(img);

                if (bFirstPage == false && startAtNewPage == true)
                {
                    // insert a new section
                    textControl1.Sections.Add(
                        TXTextControl.SectionBreakKind.BeginAtNewPage);
                    bFirstPage = true;
                }

                // add the image and adjust the page size and margins according
                // to the image
                textControl1.Images.Add(TXImg, 
                    textControl1.InputPosition.Page, 
                    new Point(0, 0), 
                    TXTextControl.ImageInsertionMode.FixedOnPage);
                textControl1.Sections.GetItem().Format.PageMargins = 
                    new TXTextControl.PageMargins(0, 0, 0, 0);
                textControl1.Sections.GetItem().Format.PageSize = 
                    new TXTextControl.PageSize(TXImg.Size.Width, TXImg.Size.Height);
            }
        }

        /***************************************************************
         * SplitMultipageTIFF method
         * description: Splits a multipage TIFF into separate images
         * 
         * param:       image - the image path
         * return val:  A list of images
         ***************************************************************/
        private List<Image> SplitMultipageTIFF(string image)
        {
            List<Image> listImages = new List<Image>();
            Bitmap bmp = (Bitmap)Image.FromFile(image);
            int iCount = bmp.GetFrameCount(FrameDimension.Page);
            
            for (int i = 0; i < iCount; i++)
            {
                // store each frame
                bmp.SelectActiveFrame(FrameDimension.Page, i);
                MemoryStream byteStream = new MemoryStream();
                bmp.Save(byteStream, ImageFormat.Tiff);

                // assemble new image
                listImages.Add(Image.FromStream(byteStream));
            }

            return listImages;
        }
    }
}
