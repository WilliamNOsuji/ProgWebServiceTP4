using PostHubAPI.Data;
using PostHubAPI.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace PostHubAPI.Services
{
    public class PictureService
    {
        private readonly PostHubAPIContext _context;
        private readonly String dossierCommentaire= "/images/full/";
        public PictureService(PostHubAPIContext context) 
        {
            _context = context;
        }

        private bool IsContextNull() => _context.Pictures == null;
        public async Task<Boolean> addPicture(Picture image)
        {
            if (image != null)
            {
                _context.Entry(image);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<Byte[]> getImageData(String name,String dossier)
        {
            if (IsContextNull()) return null;
            //if(!(Regex.Match(Size,""))
            byte[] data=System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory()+"/images/"+dossier+name);
            return data;
        }
        public async Task<Picture> getImageObj(int id){
            if (IsContextNull()) return null;
            return await _context.Pictures.FindAsync(id);
        }
            public async Task<Boolean>  sauvegarderImage(IFormFile? file){
            Picture imageObj = new Picture()
            {
                Id = 0,
                FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
                MimeType = file.ContentType
            };
            Image imageData = Image.Load(file.OpenReadStream());
            imageData.Save(Directory.GetCurrentDirectory() + dossierCommentaire + imageObj.FileName);
            imageData.Mutate(i =>
                i.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Min,
                    Size = new Size() { Width = 320 }
                })
            );

            imageData.Save(Directory.GetCurrentDirectory() + "/images/thumbnail/" + imageObj.FileName);
            if (imageObj != null) {
                 await _context.AddAsync(imageObj);
                await _context.SaveChangesAsync();
            }
            return true;
        }
     private Picture convertisseurImage(IFormFile? file)
        {
            return new Picture()
            {
                Id = 0,
                FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
                MimeType = file.ContentType
            };
        }   
    }
    
}
