
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using WebApplication2Homework.Data;
public class ProductService : IProductService
{
    private readonly Datacontext db;
    private readonly IWebHostEnvironment webEnv;

    public ProductService(Datacontext db, IWebHostEnvironment webEnv)
    {
        this.db = db;
        this.webEnv = webEnv;
    }

    public List<Product> GetAll(string keyword)
    {
        keyword = keyword?.ToUpper();

        // ใช้ Include เพื่อโหลด Category ที่เกี่ยวข้อง
        var query = db.Products.Include(p => p.category).AsQueryable();

        // การค้นหาผลิตภัณฑ์ตาม keyword
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(px => px.Name.ToUpper().Contains(keyword));
        }

        // เรียงลำดับและแปลงเป็น List
        return query.OrderByDescending(px => px.Id).ToList();
    }

    public void AddData(Product product, IFormFileCollection files)
    {
        string wwwRootPath = webEnv.WebRootPath;

        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var uploads = Path.Combine(wwwRootPath, "images");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    product.Image.Add(@"\images\" + fileName + extension); // เพิ่มเส้นทางของภาพเข้าไปในลิสต์
                }
            }
        }

        db.Products.Add(product);
        db.SaveChanges();
    }



    public Product SearchData(int id)
    {
        return db.Products.Include(p => p.category).FirstOrDefault(p => p.Id == id);
    }

    public void UpdateData(Product product, IFormFileCollection files)
    {
        string wwwRootPath = webEnv.WebRootPath;
        var uploads = Path.Combine(wwwRootPath, "images");

        if (!Directory.Exists(uploads))
            Directory.CreateDirectory(uploads);

        var existingProduct = db.Products.Include(p => p.category).FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("ไม่พบผลิตภัณฑ์.");
        }

        if (files != null && files.Count > 0)
        {
            foreach (var oldImagePath in existingProduct.Image)
            {
                var oldImageFullPath = Path.Combine(wwwRootPath, oldImagePath.TrimStart('\\'));
                if (File.Exists(oldImageFullPath))
                {
                    File.Delete(oldImageFullPath);
                }
            }

            existingProduct.Image.Clear();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    existingProduct.Image.Add(@"\images\" + fileName + extension);
                }
            }
        }

        existingProduct.Name = product.Name; // ตัวอย่างคุณสมบัติ
        existingProduct.Price = product.Price; // ตัวอย่างคุณสมบัติ
                                               // เพิ่มคุณสมบัติอื่น ๆ ตามที่ต้องการ...

        db.SaveChanges();
    }




    public void DeleteData(int id)
    {
        var product = SearchData(id);

        if (product != null)
        {
            // Delete images associated with the product from the file system
            string wwwRootPath = webEnv.WebRootPath;
            foreach (var imagePath in product.Image)
            {
                var fullImagePath = Path.Combine(wwwRootPath, imagePath.TrimStart('\\'));
                if (File.Exists(fullImagePath))
                {
                    File.Delete(fullImagePath);
                }
            }

            // Remove the product from the database
            db.Products.Remove(product);
            db.SaveChanges();
        }
    }

    public IEnumerable<Category> GetCategories() => db.categories.ToList();
    
    
}
