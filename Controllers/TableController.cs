using Cafee_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Cafee_Prototype.Controllers;

[Authorize]
public class TableController: Controller
{
    private readonly CafeeDbContext _cafeeDbContext;
    public TableController(CafeeDbContext cafeeDbContext)
    {
        _cafeeDbContext = cafeeDbContext;
    }

    private async Task TableNumManagerAsync()
    {
        var list = _cafeeDbContext.Tables;
        int num = 1;

        foreach(var table in list)
        {
            table.TableNum = num;
            num++;
            _cafeeDbContext.Update(table);
        }
        await _cafeeDbContext.SaveChangesAsync();
    }

    public async Task<IActionResult> TableList()
    {
        return View(await _cafeeDbContext.Tables.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddTable()
    {
        //Creating an empty table so I can use it's id to generate my qr code
        Table newTable = new Table();
        await _cafeeDbContext.Tables.AddAsync(newTable);
        await _cafeeDbContext.SaveChangesAsync();

        string qrCodePath = "http://localhost:5096/Order/MakeOrder/" + newTable.TableId.ToString(); 

        Guid newGuid = Guid.NewGuid();
        string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes", newGuid.ToString() + ".png");

        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePath, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            System.IO.File.WriteAllBytes(savePath, qrCodeImage);
        }

        
        newTable.TableQrCode = newGuid + ".png";
        _cafeeDbContext.Tables.Update(newTable);
        
        await _cafeeDbContext.SaveChangesAsync();

        await TableNumManagerAsync();

        return RedirectToAction("TableList");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTable(int id)
    {

        var order = await _cafeeDbContext.Orders.AnyAsync(o => o.Table.TableId == id);
        if (order)
        {
            return BadRequest("This table is used in an order. Delete all the orders that uses this table before trying to delete this table.");
        }

        var table = await _cafeeDbContext.Tables.FirstOrDefaultAsync(t => t.TableId == id);
        if(table == null)
        {
            return BadRequest("table does not exist");
        }
        _cafeeDbContext.Tables.Remove(table);
        
        //Deleting image
        if(table.TableQrCode != null)
        {
            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/qrcodes", table.TableQrCode);
            
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        await _cafeeDbContext.SaveChangesAsync();

        await TableNumManagerAsync();

        return RedirectToAction("TableList");
    }

}