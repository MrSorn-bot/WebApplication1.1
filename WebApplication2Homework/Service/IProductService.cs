﻿public interface IProductService
{
    List<Product> GetAll(string keyword); // ดึงข้อมูลผลิตภัณฑ์ทั้งหมด โดยอิงจากคำค้นหา
    void  AddData(Product product, IFormFileCollection file); // เพิ่มผลิตภัณฑ์ใหม่
    Product SearchData(int id); // ค้นหาผลิตภัณฑ์ตาม ID
    void UpdateData(Product product, IFormFileCollection file); // อัปเดตข้อมูลผลิตภัณฑ์
    void DeleteData(int id); // ลบผลิตภัณฑ์ตาม ID
    IEnumerable<Category> GetCategories();
}
