
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
        // Check if new files are uploaded
        if (files != null && files.Count > 0)
        {
            string wwwRootPath = webEnv.WebRootPath;
            var uploads = Path.Combine(wwwRootPath, "images");

            // Ensure the uploads directory exists
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // Clear existing images if necessary
            product.Image.Clear();

            // Loop through each file in the collection
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);

                    // Save the new file
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    // Add the new image path
                    product.Image.Add(@"\images\" + fileName + extension);
                }
            }
        }

        // Update the product in the database
        db.Products.Update(product);
        db.SaveChanges();
    }



    public void DeleteData(int id)
    {
        var product = SearchData(id);

        if (product != null)
        {
            db.Products.Remove(product);
            db.SaveChanges();
        }
    }

    public IEnumerable<Category> GetCategories() => db.categories.ToList();
    
    
}
