using iTextSharp.text;
using iTextSharp.text.pdf;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ReportPDFxController : Controller
    {
        // GET: ReportPDF
        public string PaySlip()
        {
            Rectangle Rec = new Rectangle(504, 244);
            Document doc = new Document(Rec, 10, 10, 10, 10);
            System.IO.FileStream file =
                new System.IO.FileStream(Server.MapPath("~/Pdf/PdfSample") +
                DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf",
                System.IO.FileMode.OpenOrCreate);
            PdfWriter writer = PdfWriter.GetInstance(doc, file);
            // calling PDFFooter class to Include in document
            writer.PageEvent = new PDFLayout();
            doc.Open();
            PdfPTable tab = new PdfPTable(3);
            PdfPCell cell = new PdfPCell(new Phrase("Header",
                                new Font(Font.FontFamily.HELVETICA, 24F)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                                          //Style
            cell.BorderColor = new BaseColor(System.Drawing.Color.Red);
            cell.Border = Rectangle.BOTTOM_BORDER; // | Rectangle.TOP_BORDER;
            cell.BorderWidthBottom = 3f;
            tab.AddCell(cell);
            //row 1
            tab.AddCell("R1C1");
            tab.AddCell("R1C2");
            tab.AddCell("R1C3");
            //row 2
            tab.AddCell("R2C1");
            tab.AddCell("R2C2");
            tab.AddCell("R2C3");
            cell = new PdfPCell();
            cell.Colspan = 3;
            iTextSharp.text.List pdfList = new List(List.UNORDERED);
            pdfList.Add(new iTextSharp.text.ListItem(new Phrase("Unorder List 1")));
            pdfList.Add("Unorder List 2");
            pdfList.Add("Unorder List 3");
            pdfList.Add("Unorder List 4");
            cell.AddElement(pdfList);
            tab.AddCell(cell);
            doc.Add(tab);
            doc.Close();
            file.Close();

            return "YES";
        }
    }
}