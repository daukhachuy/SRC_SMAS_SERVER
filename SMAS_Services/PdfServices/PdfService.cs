using DinkToPdf;
using DinkToPdf.Contracts;
using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.DTOs.PDFDTO;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.ContractRepository;
using SMAS_Repositories.PdfRepositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SMAS_Services.PdfServices
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _converter;

        private readonly IPdfRepository _pdfRepository;

        public PdfService(IPdfRepository pdfRepository, IConverter converter)
        {
            _pdfRepository = pdfRepository;
            _converter = converter;
        }
        public async Task<byte[]> ExportInvoicePdfAsync(string invoiceId)
        {
            var invoice = await _pdfRepository.GetInvoiceByIdAsync(invoiceId);

            if (invoice == null || invoice.OrderCode == null)
                return Array.Empty<byte>(); 

            var html = BuildInvoiceHtml(invoice);

            return ConvertToPdf(html);
        }

        public async Task<byte[]> ExportContractPdfAsync(string contractId)
        {
            var contract = await _pdfRepository.GetContractByIdAsync(contractId);
            var bookentdetail = await _pdfRepository.GetBookEventdetailAsync(contractId);
            if (contract == null || contract.ContractCode == null)
                return Array.Empty<byte>();

            var html = BuildContractHtml(contract, bookentdetail);

            return ConvertToPdf(html);
        }
       

        //  HTML hóa đơn
        private string BuildInvoiceHtml(PdfInvoiceDTO invoice)
        {
            int index = 1;
            var details = string.Join("", invoice.Items.Select(d => $@"
        <tr>
            <td style='text-align:center'>{index++}</td>
            <td>{d.ItemName}</td>
            <td style='text-align:center'>{d.Quantity}</td>
            <td style='text-align:right'>{d.UnitPrice:N0}</td>
            <td style='text-align:right'>{(d.Subtotal ?? d.Quantity * d.UnitPrice):N0}</td>
        </tr>
    "));

            return $@"
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        @font-face {{font - family: 'Roboto'; src: url('file:///app/fonts/Roboto-Regular.ttf') format('truetype');}}
        body {{font - family: 'Roboto', sans-serif;}}
        .header {{ text-align: center; margin-bottom: 10px; }}
        .model-code {{ text-align: left; font-size: 13px; font-weight: bold; }}
        .invoice-title {{ text-align: center; font-size: 22px; font-weight: bold; margin: 10px 0; }}
        .invoice-meta {{ text-align: right; font-size: 14px; margin-bottom: 10px; }}
        
        .seller-info, .customer-section {{ margin-bottom: 15px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
        
        table {{ border-collapse: collapse; width: 100%; margin-top: 10px; }}
        th, td {{ border: 1px solid #333; padding: 8px; font-size: 13px; }}
        th {{ background-color: #f2f2f2; }}

        .summary-table {{ border: none; width: 100%; margin-top: 10px; }}
        .summary-table td {{ border: none; padding: 4px 0; }}
        .total-row {{ font-weight: bold; font-size: 16px; border-top: 1px double #333 !important; }}

        .signature-table {{ margin-top: 30px; width: 100%; text-align: center; }}
        .signature-table td {{ border: none; }}

        .footer-note {{ font-style: italic; font-size: 12px; text-align: center; margin-top: 40px; }}
    </style>
</head>

<body>

    <div class='model-code'>
        Mẫu hiển thị số 02: Hóa đơn điện tử bán hàng<br>
        (Dùng cho tổ chức, cá nhân khai thuế GTGT theo phương pháp trực tiếp)
    </div>

    <div class='invoice-title'>HÓA ĐƠN BÁN HÀNG</div>
    
    <div class='invoice-meta'>
        Ký hiệu: 2C21TBB<br>
        Số: {invoice.OrderCode}
    </div>

    <div style='text-align: center; margin-bottom: 15px;'>
        Ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}
    </div>

    <div class='seller-info'>
        <div><strong>Tên người bán:</strong> NHÀ HÀNG SMAS</div>
        <div><strong>Mã số thuế:</strong> 010023400</div>
        <div><strong>Địa chỉ:</strong> 123 Võ Nguyên Giáp, Đà Nẵng</div>
        <div><strong>Điện thoại:</strong> 0372205872</div>
    </div>

    <div class='customer-section'>
        <div><strong>Họ tên người mua hàng:</strong> {invoice.CustomerName}</div>
        <div><strong>Số lượng khách hàng :</strong> {invoice.NumberOfGuests}</div>
        <div><strong>Mã số thuế:</strong> ................................</div>
        <div><strong>Hình thức thanh toán:</strong> {invoice.PaymentMethod}</div>
    </div>

    <table>
        <thead>
            <tr>
                <th style='width: 5%'>STT</th>
                <th>Tên hàng hóa, dịch vụ</th>
                <th style='width: 10%'>Số lượng</th>
                <th style='width: 15%'>Đơn giá</th>
                <th style='width: 15%'>Thành tiền</th>
            </tr>
        </thead>
        <tbody>
            {details}
        </tbody>
    </table>


    <table class='summary-table'>
        <tr>
            <td style='text-align: left;'>Tạm tính:</td>
            <td style='text-align: right;'>{invoice.SubTotal ?? 0:N0}</td>
        </tr>
        <tr>
            <td style='text-align: left;'>Giảm giá:</td>
            <td style='text-align: right;'>- {invoice.DiscountAmount ?? 0:N0}</td>
        </tr>
        <tr>
            <td style='text-align: left;'>Thuế:</td>
            <td style='text-align: right;'>{invoice.TaxAmount ?? 0:N0}</td>
        </tr>
        <tr>
            <td style='text-align: left;'>Phí giao hàng:</td>
            <td style='text-align: right;'>{invoice.DeliveryPrice ?? 0:N0}</td>
        </tr>

        <tr class='total-row'>
            <td style='text-align: left;'>Tổng tiền thanh toán:</td>
            <td style='text-align: right;'>{invoice.TotalAmount:N0} VND</td>
        </tr>

        <tr>
            <td colspan='2'><em>Số tiền viết bằng chữ: .................................................</em></td>
        </tr>
    </table>


    <table class='signature-table'>
        <tr>
            <td><strong>Người mua hàng</strong></td>
            <td><strong>Người bán hàng</strong></td>
        </tr>
        <tr>
            <td style='height:80px'></td>
            <td></td>
        </tr>
        <tr>
            <td>(Ký, ghi rõ họ tên)</td>
            <td>(Ký, ghi rõ họ tên)</td>
        </tr>
    </table>

    <div class='footer-note'>
        (Cần kiểm tra, đối chiếu khi lập, nhận hóa đơn)
    </div>

</body>
</html>";
        }

        //  HTML hợp đồng
        private string BuildContractHtml(ContractResponseDTO contract, BookEventListResponseDTO bookentdetail)
        {
            // 1. Tính toán phần trăm tiền cọc
            string depositPercent = contract.TotalAmount > 0
                ? $"({(contract.DepositAmount / contract.TotalAmount * 100):0}%)"
                : "(3%)";

            // 2. Xây dựng bảng chi tiết chi phí (Giữ logic Món x Bàn)
            System.Text.StringBuilder tableRows = new System.Text.StringBuilder();
            decimal totalMenuPrice = 0;
            decimal totalServicePrice = 0;

            if (bookentdetail.Foods != null && bookentdetail.Foods.Any())
            {
                tableRows.AppendLine("<tr><td colspan='3' style='background:#f4f4f4; font-weight:bold;'>A. THỰC ĐƠN MÓN ĂN (Tính theo đơn vị Bàn)</td></tr>");
                foreach (var food in bookentdetail.Foods)
                {
                    decimal foodSubTotal = (food.UnitPrice ?? 0) * food.Quantity * bookentdetail.NumberOfGuests;
                    totalMenuPrice += foodSubTotal;
                    tableRows.AppendLine($@"
                <tr>
                    <td>{food.FoodName} <br/><small>(Đơn giá: {food.UnitPrice:N0} x {food.Quantity} món/bàn x {contract.NumberOfGuests} bàn)</small></td>
                    <td style='text-align:center;'>Món ăn</td>
                    <td style='text-align:right;'>{foodSubTotal:N0} VNĐ</td>
                </tr>");
                }
            }

            if (bookentdetail.Services != null && bookentdetail.Services.Any())
            {
                tableRows.AppendLine("<tr><td colspan='3' style='background:#f4f4f4; font-weight:bold;'>B. DỊCH VỤ ĐI KÈM</td></tr>");
                foreach (var service in bookentdetail.Services)
                {
                    decimal serviceSubTotal = (service.UnitPrice ?? 0) * (service.Quantity ?? 0);
                    totalServicePrice += serviceSubTotal;
                    tableRows.AppendLine($@"
                <tr>
                    <td>{service.ServiceName} <br/><small>(Số lượng: {service.Quantity} {service.Unit} x {service.UnitPrice:N0})</small></td>
                    <td style='text-align:center;'>Dịch vụ</td>
                    <td style='text-align:right;'>{serviceSubTotal:N0} VNĐ</td>
                </tr>");
                }
            }

            return $@"
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        @page {{ size: A4; margin: 15mm; }}
        @font-face {{font - family: 'Roboto'; src: url('file:///app/fonts/Roboto-Regular.ttf') format('truetype');}}
        body {{font - family: 'Roboto', sans-serif;}}
        .header {{ text-align: center; margin-bottom: 20px; }}
        .motto {{ font-size: 14px; font-weight: bold; text-transform: uppercase; }}
        .title {{ font-size: 22px; font-weight: bold; text-transform: uppercase; margin: 15px 0 5px 0; }}
        
        .party-info {{ display: flex; justify-content: space-between; margin-bottom: 20px; gap: 30px; }}
        .party-box {{ width: 50%; border: 1px solid #eee; padding: 10px; border-radius: 5px; }}
        .party-header {{ font-weight: bold; text-transform: uppercase; border-bottom: 1px solid #333; margin-bottom: 8px; font-size: 14px; }}
        
        .section-header {{ background: #333; color: #fff; padding: 5px 10px; font-weight: bold; text-transform: uppercase; margin-top: 15px; }}
        
        table {{ width: 100%; border-collapse: collapse; margin-top: 5px; }}
        th, td {{ border: 1px solid #000; padding: 6px; text-align: left; }}
        th {{ background: #f2f2f2; text-transform: uppercase; font-size: 12px; }}
        
        .money-box {{ border: 2px solid #000; padding: 10px; margin-top: 10px; background: #fafafa; }}
        .highlight-price {{ font-size: 16px; font-weight: bold; color: #d9534f; }}
        
        .terms {{ margin-top: 15px; text-align: justify; }}
        .terms b {{ text-decoration: underline; }}
        
        .signature-section {{ display: block; justify-content: space-between; margin-top: 30px; text-align: center; }}
        .sig-col {{ width: 45%; float: left; text-align: center; }}
        .signature-section::after {{content: "";display: table;clear: both; }}
        .digital-sig {{ border: 1px dashed #28a745; padding: 8px; color: #28a745; margin-top: 5px; font-size: 11px; }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='motto'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
        <div>Độc lập - Tự do - Hạnh phúc</div>
        <hr style='width: 150px;'>
        <div class='title'>HỢP ĐỒNG DỊCH VỤ TỔ CHỨC SỰ KIỆN</div>
        <div>Số: {contract.ContractCode} | Ngày lập: {contract.CreatedAt:dd/MM/yyyy}</div>
    </div>

    <div class='party-info'>
        <div class='party-box'>
            <div class='party-header'>BÊN A: NHÀ HÀNG SMAS</div>
            <div>Địa chỉ: 123 Võ Nguyên Giáp, Đà Nẵng</div>
            <div>Đại diện: <b>{bookentdetail.ConfirmedBy?.Fullname ?? contract.ManagerName}</b></div>
            <div>Chức vụ: Quản lý hệ thống</div>
            <div>Điện thoại: 0912.345.xxx</div>
        </div>
        <div class='party-box'>
            <div class='party-header'>BÊN B: KHÁCH HÀNG</div>
            <div>Ông/Bà: <b>{bookentdetail.Customer.Fullname}</b></div>
            <div>Điện thoại: {bookentdetail.Customer.Phone}</div>
            <div>Email: {bookentdetail.Customer.Email}</div>
            <div>Địa chỉ: [Cập nhật theo hồ sơ khách hàng]</div>
        </div>
    </div>

    <div class='section-header'>ĐIỀU 1: NỘI DUNG SỰ KIỆN</div>
    <p>- Loại hình sự kiện: {bookentdetail.Event.Title}</p>
    <p>- Thời gian: {bookentdetail.ReservationTime} ngày {bookentdetail.ReservationDate:dd/MM/yyyy}</p>
    <p>- Quy mô bàn: <b>{bookentdetail.NumberOfGuests} Bàn tiêu chuẩn</b></p>

    <div class='section-header'>ĐIỀU 2: CHI TIẾT THỰC ĐƠN VÀ DỊCH VỤ</div>
    <table>
        <thead>
            <tr>
                <th>Nội dung chi tiết</th>
                <th style='width:15%; text-align:center;'>Phân loại</th>
                <th style='width:25%; text-align:right;'>Thành tiền</th>
            </tr>
        </thead>
        <tbody>
            {tableRows.ToString()}
        </tbody>
    </table>

    <div class='section-header'>ĐIỀU 3: GIÁ TRỊ HỢP ĐỒNG VÀ PHƯƠNG THỨC THANH TOÁN</div>
    <div class='money-box'>
        <p>1. Tổng giá trị hợp đồng: <span class='highlight-price'>{contract.TotalAmount:N0} VNĐ</span></p>
        <p>2. Số tiền đặt cọc {depositPercent}: <span class='highlight-price'>{contract.DepositAmount:N0} VNĐ</span></p>
        <p>3. Số tiền còn lại phải thanh toán: <b>{(contract.TotalAmount - (contract.DepositAmount ?? 0)):N0} VNĐ</b></p>
    </div>

    <div class='section-header'>ĐIỀU 4: CÁC ĐIỀU KHOẢN THỎA THUẬN CHUNG (BẢN CỨNG)</div>
    <div class='terms'>
        <p><b>4.1. Trách nhiệm Bên A:</b> Đảm bảo chất lượng vệ sinh an toàn thực phẩm. Cung cấp đúng, đủ số lượng thực đơn và dịch vụ như đã thỏa thuận tại Điều 2. Đảm bảo không gian tổ chức đúng thời gian quy định.</p>
        <p><b>4.2. Trách nhiệm Bên B:</b> Thanh toán đầy đủ các khoản chi phí đúng hạn. Phối hợp với Bên A để chốt danh mục dịch vụ trước 03 ngày tổ chức. Đảm bảo an ninh trật tự trong suốt thời gian diễn ra sự kiện.</p>
        <p><b>4.3. Thay đổi và Hủy bỏ:</b> Mọi thay đổi về số lượng bàn vượt quá 10% phải thông báo trước 48 giờ. Trường hợp hủy hợp đồng sau khi đã đặt cọc, Bên B sẽ mất toàn bộ số tiền cọc trừ trường hợp bất khả kháng theo quy định pháp luật.</p>
        <p><b>4.4. Giải quyết tranh chấp:</b> Hai bên cam kết thực hiện đúng các điều khoản trên. Mọi tranh chấp phát sinh sẽ được giải quyết thông qua thương lượng, nếu không thành sẽ đưa ra cơ quan có thẩm quyền tại Đà Nẵng xử lý.</p>
    </div>
   <div style='margin-top: 30px; text-align: center; font-size: 11px; color: #666;'>
        <i>Hợp đồng này được lập thành 02 bản điện tử có giá trị pháp lý như nhau.</i>
    </div>
    <div class='signature-section'>
        <div class='sig-col'>
            <b>ĐẠI DIỆN BÊN A</b><br><i>(Ký, ghi rõ họ tên)</i>
            <div class='digital-sig'>✔ Đã xác thực điện tử<br>{bookentdetail.ConfirmedBy?.Fullname}</div>
            <br><b>{bookentdetail.ConfirmedBy?.Fullname}</b>
        </div>
        <div class='sig-col'>
            <b>ĐẠI DIỆN BÊN B</b><br><i>(Ký, ghi rõ họ tên)</i>
            <div class='digital-sig'>✔ Đã xác thực điện tử<br>{bookentdetail.Customer.Fullname}</div>
            <br><b>{bookentdetail.Customer.Fullname}</b>
        </div>
    </div>

   
</body>
</html>";
        }

        //private byte[] ConvertToPdf(string html)
        //{
        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = {
        //        PaperSize = PaperKind.A4
        //    },
        //        Objects = {
        //        new ObjectSettings() {
        //            HtmlContent = html,
        //            WebSettings = { DefaultEncoding = "utf-8" }
        //        }
        //    }
        //    };

        //    return _converter.Convert(doc);
        //}


        private byte[] ConvertToPdf(string html)
        {
            var exePath = Path.Combine(AppContext.BaseDirectory, "Libraries", "wkhtmltopdf.exe");

            if (!File.Exists(exePath))
                throw new Exception("wkhtmltopdf.exe NOT FOUND: " + exePath);

            var tempHtml = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.html");
            var tempPdf = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

            File.WriteAllText(tempHtml, html);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"--enable-local-file-access --encoding utf-8 \"{tempHtml}\" \"{tempPdf}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppContext.BaseDirectory
                }
            };

            process.Start();

            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception("wkhtmltopdf error: " + error);
            }

            var pdfBytes = File.ReadAllBytes(tempPdf);

            // cleanup
            File.Delete(tempHtml);
            File.Delete(tempPdf);

            return pdfBytes;
        }
    }
}
