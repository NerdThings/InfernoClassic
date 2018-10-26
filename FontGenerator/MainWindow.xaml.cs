using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Inferno;
using Inferno.Graphics.Text;
using Color = Inferno.Graphics.Color;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace FontGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (var fontFamily in System.Drawing.FontFamily.Families)
            {
                combFonts.Items.Add(fontFamily.Name);
            }

            combFonts.Items.RemoveAt(0);
            combFonts.SelectedIndex = 0;
        }

        private void combFonts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combFonts.SelectedItem is string s && preview != null)
                preview.FontFamily = new FontFamily(s);
        }

        private void fontSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(fontSize.Text, out var size) && preview != null && size > 0)
            {
                preview.FontSize = size;
            }
        }

        private unsafe void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Fonts"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Fonts");

            var standardSizes = new[] {8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 86, 72};

            foreach (var size in standardSizes)
            {
                foreach (var fontFamily in System.Drawing.FontFamily.Families)
                {
                    Console.WriteLine(fontFamily.Name + " : " + size);
                    using (var stream =
                        new FileStream(
                            Directory.GetCurrentDirectory() + "\\Fonts\\" + fontFamily.Name + "_" + size + ".fnt",
                            FileMode.Create))
                    {
                        var fntStuff = FontBuilder.CreateFontStuff(fontFamily.Name, size);

                        //Get data from bitmap
                        var data = new Inferno.Graphics.Color[fntStuff.Item1.Width * fntStuff.Item1.Height];

                        var bitmapData = fntStuff.Item1.LockBits(new System.Drawing.Rectangle(0, 0, fntStuff.Item1.Width, fntStuff.Item1.Height), ImageLockMode.ReadOnly, fntStuff.Item1.PixelFormat);

                        switch (fntStuff.Item1.PixelFormat)
                        {
                            case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                            {
                                var ptr = (byte*)bitmapData.Scan0;
                                var i = 0;
                                for (var y = 0; y < fntStuff.Item1.Height; ++y)
                                {
                                    for (var x = 0; x < fntStuff.Item1.Width; ++x)
                                    {
                                        var c = new Inferno.Graphics.Color(*(ptr + 2), *(ptr + 1), *ptr, *(ptr + 3));
                                        data[i] = c;

                                        i++;
                                        ptr += 4;
                                    }
                                }
                                break;
                            }

                            case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                            {
                                var ptr = (byte*)bitmapData.Scan0;
                                var i = 0;
                                for (var y = 0; y < fntStuff.Item1.Height; ++y)
                                {
                                    for (var x = 0; x < fntStuff.Item1.Width; ++x)
                                    {
                                        var c = new Color(*(ptr + 2), *(ptr + 1), *ptr, (byte)255);
                                        data[i] = c;

                                        i++;
                                        ptr += 3;
                                    }
                                }
                                break;
                            }
                            default:
                                throw new Exception("Other pixel data types are not supported.");
                        }

                        fntStuff.Item1.UnlockBits(bitmapData);

                        //Write
                    }
                }
            }

            MessageBox.Show("Complete");
        }
    }
}