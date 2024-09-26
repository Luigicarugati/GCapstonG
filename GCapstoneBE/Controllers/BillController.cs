using GCapstoneBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace GCapstoneBE.Controllers
{
    [RoutePrefix("api/bill")]
    public class BillController : ApiController
    {
        BarEntities db = new BarEntities();
        Response response = new Response();
        private string pdfpath = @"C:\pdf\";


        [HttpPost, Route("generateReport")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateReport([FromBody] Bill bill)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueId = ticks.ToString() + '-' + guid;
                bill.createdBy = tokenClaim.Email;
                bill.uuid = uniqueId;
                db.Bills.Add(bill);
                db.SaveChanges();
                Get(bill);
                return Request.CreateResponse(HttpStatusCode.OK, new { uuid = bill.uuid });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }

        private void Get(Bill bill)
        {
            try
            {
                dynamic productDetails = JsonConvert.DeserializeObject(bill.productDetails);
                var todayDate = "Date: " + Convert.ToDateTime(DateTime.Today).ToString("dd/MM/yyyy");
                PdfWriter writer = new PdfWriter(pdfpath + bill.uuid + ".pdf");
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                //Intestazione
                Paragraph header = new Paragraph("Gestionale bar ")
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(25);
                document.Add(header);

                // Nuova linea 
                Paragraph NewLine = new Paragraph(new Text("/n"));

                // lINEA DIVISORIA
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                // Dettagli cliente 
                Paragraph customerDetails = new Paragraph("Name:" + bill.name + "/nEmail:" + bill.email + "/nContactNumber:" + bill.contactNumber + "/nPayment Method:" + bill.paymentMethod);
                document.Add(customerDetails);




                // Tabella

                Table table = new Table(5, false);
                table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));

                // intestazione

                Cell headerName = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Name"));

                Cell headerCategory = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Category"));

                Cell headerQuantity = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Quantity"));

                Cell headerPrice = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Price"));

                Cell headerSubTtotal = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Sub Total"));



                table.AddCell(headerName);
                table.AddCell(headerCategory);
                table.AddCell(headerQuantity);
                table.AddCell(headerPrice);
                table.AddCell(headerSubTtotal);

                foreach (JObject product in productDetails)
                {
                    Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["name"].ToString()));

                    Cell categoryCell = new Cell(1, 1)
                     .SetTextAlignment(TextAlignment.CENTER)
                     .Add(new Paragraph(product["category"].ToString()));

                    Cell quantityCell = new Cell(1, 1)
                     .SetTextAlignment(TextAlignment.CENTER)
                     .Add(new Paragraph(product["quantity"].ToString()));

                    Cell priceCell = new Cell(1, 1)
                     .SetTextAlignment(TextAlignment.CENTER)
                     .Add(new Paragraph(product["price"].ToString()));

                    Cell totalCell = new Cell(1, 1)
                     .SetTextAlignment(TextAlignment.CENTER)
                     .Add(new Paragraph(product["total"].ToString()));


                    table.AddCell(nameCell);
                    table.AddCell(categoryCell);
                    table.AddCell(quantityCell);
                    table.AddCell(priceCell);
                    table.AddCell(totalCell);


                }
                document.Add(table);
                Paragraph last = new Paragraph("Total: " + bill.totalAmount + "/nGrazie per la visita, torna a trovarci presto ");
                document.Add(last);
                document.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }


        [HttpPost, Route("getPdf")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetPdf([FromBody] Bill bill)
        {
            try
            {
                if (bill.name != null)
                {
                    Get(bill);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                string filepath = pdfpath + bill.uuid.ToString() + ".pdf";

                byte[] bytes = File.ReadAllBytes(filepath);

                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.ContentLength = bytes.Length;

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");

                response.Content.Headers.ContentDisposition.FileName = bill.uuid.ToString() + ".pdf";

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(bill.uuid.ToString() + ".pdf"));

                return response;

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}