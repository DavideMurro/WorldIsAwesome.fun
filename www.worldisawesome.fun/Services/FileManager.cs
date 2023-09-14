using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using www.worldisawesome.fun.DBContext;

namespace www.worldisawesome.fun.Services
{
    // TODO: usare questa classe per gestire i files!
    public static class FileManager
    {
        public static async Task<FileStream> GetFile(Guid fileId)
        {
            try
            {
                using (var _dbContext = new WorldIsAwesomeContext())
                {
                    // search if exists in db
                    var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
                    if (file == null)
                        throw new Exception("File not present");

                    // get the file
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Media", file.FileName);
                    var media = System.IO.File.OpenRead(path);
                    // var mediaFile = File(media, file.FileType);
                    return media;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task<Guid> InsertFile(IFormFile file)
        {
            try
            {
                using (var _dbContext = new WorldIsAwesomeContext())
                {
                    // save the file
                    if (file == null || file.Length == 0)
                        throw new Exception("file not selected");

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Media", file.FileName);

                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Flush();
                    stream.Close();

                    // insert the file into db
                    var fileIdGuid = Guid.NewGuid();
                    var fileNew = new DataModels.Files
                    {
                        Id = fileIdGuid,
                        FileName = file.FileName,
                        FileType = file.ContentType
                    };
                    await _dbContext.Files.AddAsync(fileNew);
                    await _dbContext.SaveChangesAsync();

                    return fileIdGuid;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task DeleteFile(Guid fileId)
        {
            try
            {
                using (var _dbContext = new WorldIsAwesomeContext())
                {
                    // search if exists in db
                    var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
                    if (file == null)
                        throw new Exception("File not present");

                    // TODO: delete file and row on db
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
