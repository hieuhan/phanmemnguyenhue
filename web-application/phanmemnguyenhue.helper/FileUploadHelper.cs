using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace phanmemnguyenhue.helper
{
    public class FileUploadHelper
    {
        public static bool IsImageFile(string fileName)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(fileName)) return retVal;

            string fileExt = Path.GetExtension(fileName).ToLower();
            string imageFile = ".jpg;.gif;.png;.bmp;.jpeg";
            if (imageFile.IndexOf(fileExt, StringComparison.Ordinal) >= 0) retVal = true;
            return retVal;
        }

        public static bool IsImage(string contentType = "", string fileName = "")
        {
            if (contentType.Contains("image"))
            {
                return true;
            }

            var formats = new string[] { ".jpg", ".png", ".gif", ".jpeg", ".bmp", ".svg" };

            return formats.Any(item => fileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetDirByFileType(string fileName)
        {
            string retVal = "Others";
            if (!string.IsNullOrEmpty(fileName))
            {
                string fileExt = Path.GetExtension(fileName).ToLower();
                string imageFile = ".jpg;.jpeg;.gif;.png;.bmp";
                string videoFile = ".3gp;.mp4;.flv";
                string audioFile = ".mp3;.wav;.wma";
                string m3u8File = ".m3u8";

                if (imageFile.IndexOf(fileExt, StringComparison.Ordinal) >= 0) retVal = "Images/Original";
                else if (videoFile.IndexOf(fileExt, StringComparison.Ordinal) >= 0) retVal = "Videos";
                else if (audioFile.IndexOf(fileExt, StringComparison.Ordinal) >= 0) retVal = "Audios";
                else if (m3u8File.IndexOf(fileExt, StringComparison.Ordinal) >= 0) retVal = "M3u8";
            }
            return retVal;
        }

        public static string GetFileName(string filePath)
        {
            string retVal = "";
            if (!string.IsNullOrEmpty(filePath))
            {
                filePath = filePath.Replace("\\", "/");
                if (filePath.Contains("/"))
                {
                    int mPos = filePath.LastIndexOf("/", StringComparison.Ordinal);
                    retVal = filePath.Substring(mPos + 1);
                }
                else
                {
                    retVal = filePath;
                }
            }
            return retVal;
        }

        public static bool IsValidFileUpload(string fileName)
        {
            bool retVal = false;
            List<string> listFileExt = ConstantHelper.FileAllowedUpload.Split(',').ToList<string>();
            string fileExt = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            if (listFileExt.Find(i => i.ToString() == fileExt.ToLower()) != null) retVal = true;
            return retVal;
        }

        public static string SaveFile(HttpPostedFileBase httpPostedFileBase, string physicalApplicationPath, string virtualPath, bool createImageThumbnail = false)
        {
            string retVal = "";
            try
            {
                if (httpPostedFileBase != null)
                {
                    virtualPath = Path.Combine(virtualPath, GetDirByFileType(httpPostedFileBase.FileName), DateTime.Now.ToString("yyyy/MM/dd"));
                    string filePath = "";
                    string fileName = "";
                    string directoryPath = Path.Combine(physicalApplicationPath, virtualPath);

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    fileName = StringHelper.RemoveSign4VietnameseString(Path.GetFileNameWithoutExtension(httpPostedFileBase.FileName)).Replace(" ", "_");
                    fileName = fileName + "_" + DateTime.Now.ToString("ddMMHHmmss");
                    fileName = fileName + Path.GetExtension(httpPostedFileBase.FileName);
                    fileName = fileName.Replace(" ", "_");
                    filePath = Path.Combine(directoryPath, fileName);
                    httpPostedFileBase.SaveAs(filePath);

                    //Create thumbnail if image
                    if (createImageThumbnail && IsImageFile(httpPostedFileBase.FileName))
                    {
                        if (ConstantHelper.MediaThumnailWidth > 0)
                        {
                            int imgWidth = ConstantHelper.MediaThumnailWidth;
                            int imgHeight = ConstantHelper.MediaThumnailHeight;
                            if (imgWidth > 0 && imgHeight > 0)
                            {
                                string fileThumb = filePath.Replace("Original\\", "Thumb\\");
                                string dirThumb = directoryPath.Replace("Original\\", "Thumb\\");
                                if (!Directory.Exists(dirThumb))
                                {
                                    Directory.CreateDirectory(dirThumb);
                                }
                                ResizeImage(filePath, imgWidth, imgHeight).Save(fileThumb, ImageFormat.Jpeg);
                            }
                        }
                        if (ConstantHelper.MediaWidth > 0)
                        {
                            int imgWidth = ConstantHelper.MediaWidth;
                            int imgHeight = ConstantHelper.MediaHeight;
                            if (imgWidth > 0 && imgHeight > 0)
                            {
                                string fileThumb = filePath.Replace("Original\\", "Standard\\");
                                string dirThumb = directoryPath.Replace("Original\\", "Standard\\");
                                if (!Directory.Exists(dirThumb))
                                {
                                    Directory.CreateDirectory(dirThumb);
                                }
                                ResizeImage(filePath, imgWidth, imgHeight).Save(fileThumb, ImageFormat.Jpeg);
                            }
                        }
                        if (ConstantHelper.MediaMobileWidth > 0)
                        {
                            int imgWidth = ConstantHelper.MediaMobileWidth;
                            int imgHeight = ConstantHelper.MediaMobileHeight;
                            if (imgWidth > 0 && imgHeight > 0)
                            {
                                string fileThumb = filePath.Replace("Original\\", "Mobile\\");
                                string dirThumb = directoryPath.Replace("Original\\", "Mobile\\");
                                if (!Directory.Exists(dirThumb))
                                {
                                    Directory.CreateDirectory(dirThumb);
                                }
                                ResizeImage(filePath, imgWidth, imgHeight).Save(fileThumb, ImageFormat.Jpeg);
                            }
                        }
                        if (ConstantHelper.MediaIconWidth > 0)
                        {
                            int imgWidth = ConstantHelper.MediaIconWidth;
                            int imgHeight = ConstantHelper.MediaIconHeight;
                            if (imgWidth > 0 && imgHeight > 0)
                            {
                                string fileThumb = filePath.Replace("Original\\", "Icon\\");
                                string dirThumb = directoryPath.Replace("Original\\", "Icon\\");
                                if (!Directory.Exists(dirThumb))
                                {
                                    Directory.CreateDirectory(dirThumb);
                                }
                                ResizeImage(filePath, imgWidth, imgHeight).Save(fileThumb, ImageFormat.Jpeg);
                            }
                        }
                    }

                    retVal = Path.Combine(virtualPath, fileName).Replace("\\", "/");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        //Image Resize Helper Method
        private static Bitmap ResizeImage(string filename, int maxWidth, int maxHeight)
        {
            using (Image originalImage = Image.FromFile(filename))
            {
                //Caluate new Size
                int newWidth = originalImage.Width;
                int newHeight = originalImage.Height;
                double aspectRatio = (double)originalImage.Width / (double)originalImage.Height;

                if (aspectRatio <= 1 && originalImage.Width > maxWidth)
                {
                    newWidth = maxWidth;
                    newHeight = (int)Math.Round(newWidth / aspectRatio);
                }
                else if (aspectRatio > 1 && originalImage.Height > maxHeight)
                {
                    newHeight = maxHeight;
                    newWidth = (int)Math.Round(newHeight * aspectRatio);
                }

                Bitmap newImage = new Bitmap(newWidth, newHeight);

                using (Graphics g = Graphics.FromImage(newImage))
                {
                    //--Quality Settings Adjust to fit your application
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.DrawImage(originalImage, 0, 0, newImage.Width, newImage.Height);
                    return newImage;
                }
            }
        }

        public static void DeleteFile(string fileDeletePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileDeletePath))
                {
                    FileInfo file = new FileInfo(fileDeletePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    if (IsImageFile(fileDeletePath))
                    {
                        fileDeletePath = fileDeletePath.Replace("/Original/", "/Standard/");
                        file = new FileInfo(fileDeletePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        fileDeletePath = fileDeletePath.Replace("/Standard/", "/Mobile/");
                        file = new FileInfo(fileDeletePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        fileDeletePath = fileDeletePath.Replace("/Mobile/", "/Thumb/");
                        file = new FileInfo(fileDeletePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        fileDeletePath = fileDeletePath.Replace("/Thumb/", "/Icon/");
                        file = new FileInfo(fileDeletePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
