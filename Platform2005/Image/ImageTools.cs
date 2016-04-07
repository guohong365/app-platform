namespace Platform.Image
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class ImageTools
    {
        private static Hashtable encoders = new Hashtable();
        private static Image.GetThumbnailImageAbort thumbnailCallback = new Image.GetThumbnailImageAbort(ImageTools.ThumbnailCallback);

        static ImageTools()
        {
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                encoders[info.MimeType] = info;
            }
        }

        public static Bitmap BrightAndContrast(Image img, int bright, int contrast)
        {
            float num = (((float) bright) / 50f) - 1f;
            float num2 = ((float) contrast) / 50f;
            float num3 = 1f;
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(image);
            ImageAttributes imageAttr = new ImageAttributes();
            float[][] newColorMatrix = new float[5][];
            float[] numArray2 = new float[5];
            numArray2[0] = num2;
            numArray2[4] = 1f;
            newColorMatrix[0] = numArray2;
            float[] numArray3 = new float[5];
            numArray3[1] = num2;
            numArray3[4] = 1f;
            newColorMatrix[1] = numArray3;
            float[] numArray4 = new float[5];
            numArray4[2] = num2;
            numArray4[4] = 1f;
            newColorMatrix[2] = numArray4;
            float[] numArray5 = new float[5];
            numArray5[3] = 1f;
            numArray5[4] = 1f;
            newColorMatrix[3] = numArray5;
            newColorMatrix[4] = new float[] { num * num3, num * num3, num * num3, 1f, 1f };
            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            imageAttr.SetColorMatrix(matrix);
            graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            return image;
        }

        public static Image BrightContrastQuality(Image img, int bright, int contrast, int quality)
        {
            if (img == null)
            {
                return null;
            }
            float num = (((float) bright) / 50f) - 1f;
            float num2 = ((float) contrast) / 50f;
            float num3 = 1f;
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(image);
            ImageAttributes imageAttr = new ImageAttributes();
            float[][] newColorMatrix = new float[5][];
            float[] numArray2 = new float[5];
            numArray2[0] = num2;
            numArray2[4] = 1f;
            newColorMatrix[0] = numArray2;
            float[] numArray3 = new float[5];
            numArray3[1] = num2;
            numArray3[4] = 1f;
            newColorMatrix[1] = numArray3;
            float[] numArray4 = new float[5];
            numArray4[2] = num2;
            numArray4[4] = 1f;
            newColorMatrix[2] = numArray4;
            float[] numArray5 = new float[5];
            numArray5[3] = 1f;
            numArray5[4] = 1f;
            newColorMatrix[3] = numArray5;
            newColorMatrix[4] = new float[] { num * num3, num * num3, num * num3, 1f, 1f };
            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            imageAttr.SetColorMatrix(matrix);
            graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            if (quality > 0x63)
            {
                return ImageChangeFormat(image, ImageFormat.Jpeg);
            }
            string key = "image/jpeg";
            if (!encoders.ContainsKey(key))
            {
                throw new Exception("ÎÞÐ§Í¼Æ¬±àÂëÆ÷£º" + key);
            }
            ImageCodecInfo encoder = encoders[key] as ImageCodecInfo;
            EncoderParameters encoderParams = new EncoderParameters(1);
            EncoderParameter parameter = new EncoderParameter(Encoder.Quality, (long) quality);
            encoderParams.Param[0] = parameter;
            MemoryStream stream = new MemoryStream();
            img.Save(stream, encoder, encoderParams);
            stream.Seek((long) 0, SeekOrigin.Begin);
            return Image.FromStream(stream);
        }

        public static string BytesToBase64String(byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            return Convert.ToBase64String(input);
        }

        public static Image GetBase64Image(string base64Data)
        {
            try
            {
                if (base64Data == null)
                {
                    return null;
                }
                return GetImage(Convert.FromBase64String(base64Data));
            }
            catch
            {
                return null;
            }
        }

        private static string GetHexString(byte[] input)
        {
            string text = "";
            for (int i = 0; i < input.Length; i++)
            {
                text = text + input[i].ToString("x2");
            }
            return text;
        }

        public static Image GetImage(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream(data);
            stream.Seek((long) 0, SeekOrigin.Begin);
            return Image.FromStream(stream);
        }

        public static byte[] GetImageData(Image img)
        {
            return GetImageData(img, ImageFormat.Jpeg);
        }

        public static byte[] GetImageData(Image img, ImageFormat format)
        {
            if (img == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream();
            img.Save(stream, format);
            return stream.ToArray();
        }

        public static string GetImageString(Image img)
        {
            if (img == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            return Convert.ToBase64String(stream.ToArray());
        }

        public static string GetImageString(string filename)
        {
            try
            {
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                return Convert.ToBase64String(buffer);
            }
            catch
            {
                return null;
            }
        }

        public static Image GetThumbnailImage(Image img, double bi)
        {
            if (img == null)
            {
                return null;
            }
            int thumbWidth = (int) (img.Width * bi);
            int thumbHeight = (int) (img.Height * bi);
            return img.GetThumbnailImage(thumbWidth, thumbHeight, thumbnailCallback, IntPtr.Zero);
        }

        public static Image GetThumbnailImage(Image img, int width, int height)
        {
            if (img == null)
            {
                return null;
            }
            return img.GetThumbnailImage(width, height, thumbnailCallback, IntPtr.Zero);
        }

        public static Image ImageChangeFormat(Image img, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            img.Save(stream, format);
            stream.Seek((long) 0, SeekOrigin.Begin);
            return Image.FromStream(stream);
        }

        public static Bitmap ImageCut(Image img, int left, int top, int width, int height)
        {
            if ((width <= 0) || (height <= 0))
            {
                return null;
            }
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(image).DrawImage(img, new Rectangle(0, 0, width, height), left, top, width, height, GraphicsUnit.Pixel);
            return image;
        }

        public static Image ImageCut(Image img, int left, int top, int width, int height, ImageFormat format)
        {
            if ((width <= 0) || (height <= 0))
            {
                return null;
            }
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(image).DrawImage(img, new Rectangle(0, 0, width, height), left, top, width, height, GraphicsUnit.Pixel);
            return ImageChangeFormat(image, format);
        }

        public static Image ImageQuality(Image img, int qualityLevel)
        {
            string key = "image/jpeg";
            if (!encoders.ContainsKey(key))
            {
                throw new Exception("ÎÞÐ§Í¼Æ¬±àÂëÆ÷£º" + key);
            }
            ImageCodecInfo encoder = encoders[key] as ImageCodecInfo;
            EncoderParameters encoderParams = new EncoderParameters(1);
            EncoderParameter parameter = new EncoderParameter(Encoder.Quality, (long) qualityLevel);
            encoderParams.Param[0] = parameter;
            MemoryStream stream = new MemoryStream();
            img.Save(stream, encoder, encoderParams);
            stream.Seek((long) 0, SeekOrigin.Begin);
            return Image.FromStream(stream);
        }

        public static Bitmap ImageResize(Image img, int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(image).DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            return image;
        }

        public static Image ImageResize(Image img, int width, int height, ImageFormat format)
        {
            if ((width <= 0) || (height <= 0))
            {
                return null;
            }
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(image).DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            return ImageChangeFormat(image, format);
        }

        public static int ImageSize(Image img, ImageFormat format)
        {
            if (img == null)
            {
                return 0;
            }
            MemoryStream stream = new MemoryStream();
            img.Save(stream, format);
            stream.Seek((long) 0, SeekOrigin.Begin);
            return (int) stream.Length;
        }

        private static bool ThumbnailCallback()
        {
            return false;
        }
    }
}
