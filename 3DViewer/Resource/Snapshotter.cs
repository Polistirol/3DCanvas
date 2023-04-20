using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimaPower.Resource
{
    public class Snapshotter
    {
       public Canvas Canvas { get; set; }

        public static void SaveCanvasSnapshot(Canvas canvas, string filePath,Size size )
        {

            canvas.Measure(size);
            canvas.Arrange(new Rect(0, 0, size.Width, size.Height));

            // Create a RenderTargetBitmap object with the desired dimensions and DPI
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            // Render the Canvas to the RenderTargetBitmap
            rtb.Render(canvas);

            // Create a PngBitmapEncoder and add the RenderTargetBitmap to it
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the image to a file without displaying it on the screen
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }


        

    }
}
