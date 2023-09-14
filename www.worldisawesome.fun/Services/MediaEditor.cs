using Microsoft.AspNetCore.Http;
using NReco.VideoConverter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;
using www.worldisawesome.fun.ExceptionModels;

namespace www.worldisawesome.fun.Services
{
    public static class MediaEditor
    {
        public static bool CreatePreviewMedia(IFormFile originalFile, string folderToSave, string newFileName)
        {
            try
            {
                var inputFile = Path.Combine(folderToSave, newFileName);
                var outputFile = Path.Combine(folderToSave, "Previews", newFileName);

                if (originalFile.ContentType.StartsWith("image/"))
                {
                    using (var media = Image.Load(inputFile))
                    {
                        media.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Crop,
                            Size = new Size(1024, 1024)
                        }));
                        var encoder = new JpegEncoder()
                        {
                            Quality = 50
                        };
                        media.Save(outputFile, encoder);
                    }
                }
                else if (originalFile.ContentType.StartsWith("video/"))
                {
                    var inputFormat = originalFile.ContentType.Replace("video/", "");

                    var convertSettings = new ConvertSettings();
                    convertSettings.SetVideoFrameSize(1024, 1024);
                    convertSettings.Seek = 0;
                    convertSettings.MaxDuration = 5;
                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                    // ffMpeg.FFMpegToolPath = Path.Combine(Directory.GetCurrentDirectory(), "Custom_Lib");
                    ffMpeg.ConvertMedia(inputFile, inputFormat, outputFile, inputFormat, convertSettings);
                }
                else if (originalFile.ContentType.StartsWith("audio/"))
                {
                    throw new MyException("There is no a procedure to compress audio files"); ;
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool CompressMedia(IFormFile originalFile, string folderToSave, string newFileName)
        {
            try
            {
                var tempFile = Path.Combine(folderToSave, "Temp", newFileName);
                var outputFile = Path.Combine(folderToSave, newFileName);


                if (originalFile.ContentType.StartsWith("image/"))
                {
                    using (var media = Image.Load(tempFile))
                    {
                        var encoder = new JpegEncoder()
                        {
                            Quality = 75
                        };
                        media.Save(outputFile, encoder);
                    }
                }
                else if (originalFile.ContentType.StartsWith("video/"))
                {
                    var inputFormat = originalFile.ContentType.Replace("video/", "");

                    var convertSettings = new ConvertSettings();
                    convertSettings.SetVideoFrameSize(1280, 720);
                    convertSettings.VideoCodec = "h264";
                    convertSettings.AudioCodec = "mp2";
                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                    // ffMpeg.FFMpegToolPath = Path.Combine(Directory.GetCurrentDirectory(), "Custom_Lib");
                    ffMpeg.ConvertMedia(tempFile, inputFormat, outputFile, inputFormat, convertSettings);
                }
                else if (originalFile.ContentType.StartsWith("audio/"))
                {

                }


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static async Task<bool> SaveMedia(IFormFile originalFile, string folderToSave, string newFileName)
        {
            try
            {
                if (System.IO.File.Exists(Path.Combine(folderToSave, newFileName)))
                {
                    throw new MyException("File already exists! can t override");
                }


                // controllo se esiste TEMP
                if (!Directory.Exists(Path.Combine(folderToSave, "Temp")))
                    Directory.CreateDirectory(Path.Combine(folderToSave, "Temp"));
                var tempFile = Path.Combine(folderToSave, "Temp", newFileName);


                // salvo in TEMP
                var stream = new FileStream(tempFile, FileMode.Create);
                await originalFile.CopyToAsync(stream);
                stream.Flush();
                stream.Close();


                // Poi comprimo (Check image / video / audio)
                if(!CompressMedia(originalFile, folderToSave, newFileName))
                {
                    throw new MyException("CompressMedia didn t work");
                }


                // creo la preview
                if (!CreatePreviewMedia(originalFile, folderToSave, newFileName))
                {
                    throw new MyException("CreatePreviewMedia didn t work");
                }


                // poi elimino in TEMP
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        
        public static bool DeleteMedia(string folderToDelete, string fileName)
        {
            try
            {
                var oldMediaPath = Path.Combine(Directory.GetCurrentDirectory(), folderToDelete, fileName);
                var oldMediaPreviewPath = Path.Combine(Directory.GetCurrentDirectory(), folderToDelete, "Previews", fileName);
                var oldMediaTempPath = Path.Combine(Directory.GetCurrentDirectory(), folderToDelete, "Temp", fileName);
                if (System.IO.File.Exists(oldMediaPath))
                {
                    System.IO.File.Delete(oldMediaPath);
                }
                else
                {
                    throw new MyException("File to delete doesn t exists");
                }
                if (System.IO.File.Exists(oldMediaPreviewPath))
                {
                    System.IO.File.Delete(oldMediaPreviewPath);
                }
                else
                {
                    throw new MyException("File Preview to delete doesn t exists");
                }
                if (System.IO.File.Exists(oldMediaTempPath))
                {
                    System.IO.File.Delete(oldMediaTempPath);
                }
                else
                {
                    throw new MyException("File Temp to delete doesn t exists");
                }


                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
