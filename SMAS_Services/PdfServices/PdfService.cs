using SMAS_BusinessObject.DTOs.BookEventDTO;
using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.DTOs.PDFDTO;
using SMAS_Repositories.PdfRepositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace SMAS_Services.PdfServices
{
    public class PdfService : IPdfService
    {
        private readonly IPdfRepository _pdfRepository;

        public PdfService(IPdfRepository pdfRepository)
        {
            _pdfRepository = pdfRepository;
        }

        // ================== INVOICE ==================
        public async Task<byte[]> ExportInvoicePdfAsync(string invoiceId)
        {
            var invoice = await _pdfRepository.GetInvoiceByIdAsync(invoiceId);
            
            if (invoice == null || invoice.OrderCode == null)
                return Array.Empty<byte>();

            return new InvoiceDocument(invoice).GeneratePdf();
        }

        // ================== CONTRACT ==================
        public async Task<byte[]> ExportContractPdfAsync(string contractId)
        {
            var contract = await _pdfRepository.GetContractByIdAsync(contractId);
            var detail = await _pdfRepository.GetBookEventdetailAsync(contractId);

            if (contract == null || contract.ContractCode == null)
                return Array.Empty<byte>();

            return new ContractDocument(contract, detail).GeneratePdf();
        }

        // =========================================================
        // ================== DOCUMENT: INVOICE =====================
        // =========================================================
        private class InvoiceDocument : QuestPDF.Infrastructure.IDocument
        {
            private readonly PdfInvoiceDTO _invoice;

            public InvoiceDocument(PdfInvoiceDTO invoice)
            {
                _invoice = invoice;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);

                    page.Content().Column(col =>
                    {
                        // ===== HEADER =====
                        col.Item().Text("Mẫu hiển thị số 02: Hóa đơn điện tử bán hàng")
                            .FontSize(10);

                        col.Item().Text("(Dùng cho tổ chức, cá nhân khai thuế GTGT theo phương pháp trực tiếp)")
                            .FontSize(10);

                        col.Item().AlignCenter().Text("HÓA ĐƠN BÁN HÀNG")
                            .FontSize(20).Bold();

                        col.Item().AlignRight().Text(text =>
                        {
                            text.Span("Ký hiệu: ").SemiBold();
                            text.Span("2C21TBB\n");
                            text.Span("Số: ").SemiBold();
                            text.Span(_invoice.OrderCode);
                        });

                        col.Item().AlignCenter().Text(
                            $"Ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}"
                        );

                        // ===== SELLER + CUSTOMER =====
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().BorderBottom(1).PaddingBottom(5).Column(c =>
                            {
                                c.Item().Text("Tên người bán: NHÀ HÀNG SMAS").SemiBold();
                                c.Item().Text("Mã số thuế: 010023400");
                                c.Item().Text("Địa chỉ: 123 Võ Nguyên Giáp, Đà Nẵng");
                                c.Item().Text("Điện thoại: 0372205872");
                            });

                            row.RelativeItem().BorderBottom(1).PaddingBottom(5).Column(c =>
                            {
                                c.Item().Text($"Họ tên người mua: {_invoice.CustomerName}").SemiBold();
                                c.Item().Text($"Số khách: {_invoice.NumberOfGuests}");
                                c.Item().Text("Mã số thuế: ....................");
                                c.Item().Text($"Thanh toán: {_invoice.PaymentMethod}");
                            });
                        });

                        // ===== TABLE =====
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn();
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(90);
                            });

                            // HEADER
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(5).Text("STT").Bold();
                                header.Cell().Border(1).Padding(5).Text("Tên hàng hóa, dịch vụ").Bold();
                                header.Cell().Border(1).Padding(5).AlignCenter().Text("SL").Bold();
                                header.Cell().Border(1).Padding(5).AlignRight().Text("Đơn giá").Bold();
                                header.Cell().Border(1).Padding(5).AlignRight().Text("Thành tiền").Bold();
                            });

                            int i = 1;

                            foreach (var item in _invoice.Items)
                            {
                                table.Cell().Border(1).Padding(5).Text(i++.ToString());
                                table.Cell().Border(1).Padding(5).Text(item.ItemName);
                                table.Cell().Border(1).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Border(1).Padding(5).AlignRight().Text(item.UnitPrice.ToString("N0"));
                                table.Cell().Border(1).Padding(5).AlignRight().Text(
                                    (item.Subtotal ?? item.Quantity * item.UnitPrice).ToString("N0")
                                );
                            }
                        });

                        // ===== SUMMARY =====
                        col.Item().AlignRight().Width(250).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            void Row(string label, string value, bool bold = false)
                            {
                                table.Cell().Text(label);
                                table.Cell().AlignRight().Text(value).Style(bold ? TextStyle.Default.Bold() : TextStyle.Default);
                            }

                            Row("Tạm tính:", $"{_invoice.SubTotal ?? 0:N0}");
                            Row("Giảm giá:", $"- {_invoice.DiscountAmount ?? 0:N0}");
                            Row("Thuế:", $"{_invoice.TaxAmount ?? 0:N0}");
                            Row("Phí giao hàng:", $"{_invoice.DeliveryPrice ?? 0:N0}");

                            Row("TỔNG THANH TOÁN:", $"{_invoice.TotalAmount:N0} VND", true);
                        });

                        col.Item().Text("Số tiền viết bằng chữ: .................................................")
                            .Italic();

                        // ===== SIGNATURE =====
                        col.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Text("Người mua hàng").Bold();
                                c.Item().Height(60);
                                c.Item().Text("(Ký, ghi rõ họ tên)");
                            });

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Text("Người bán hàng").Bold();
                                c.Item().Height(60);
                                c.Item().Text("(Ký, ghi rõ họ tên)");
                            });
                        });

                        // ===== FOOTER =====
                        col.Item().AlignCenter().PaddingTop(20)
                            .Text("(Cần kiểm tra, đối chiếu khi lập, nhận hóa đơn)")
                            .Italic()
                            .FontSize(10);
                    });
                });
            }
        }

        // =========================================================
        // ================== DOCUMENT: CONTRACT ===================
        // =========================================================
        private class ContractDocument : QuestPDF.Infrastructure.IDocument
        {
            private readonly ContractResponseDTO _contract;
            private readonly BookEventListResponseDTO _detail;

            public ContractDocument(ContractResponseDTO contract, BookEventListResponseDTO detail)
            {
                _contract = contract;
                _detail = detail;
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                   
                    page.Content().Column(col =>
                    {
                        // ===== HEADER =====
                        col.Item().AlignCenter().Column(c =>
                        {
                            c.Item().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                                .Bold().FontSize(12);
                            c.Item().Text("Độc lập - Tự do - Hạnh phúc");
                            c.Item().AlignCenter().Width(150).LineHorizontal(1);

                            c.Item().PaddingTop(10)
                                .Text("HỢP ĐỒNG DỊCH VỤ TỔ CHỨC SỰ KIỆN")
                                .Bold().FontSize(18);

                            c.Item().Text($"Số: {_contract.ContractCode} | Ngày: {_contract.CreatedAt:dd/MM/yyyy}");
                        });

                        // ===== PARTY INFO =====
                        col.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(10).Column(c =>
                            {
                                c.Item().Text("BÊN A: NHÀ HÀNG SMAS").Bold();
                                c.Item().Text("Địa chỉ: 123 Võ Nguyên Giáp, Đà Nẵng");
                                c.Item().Text($"Đại diện: {_detail.ConfirmedBy?.Fullname ?? _contract.ManagerName}");
                                c.Item().Text("Chức vụ: Quản lý");
                                c.Item().Text($"Điện thoại:{ _contract.ManagerPhone} ");
                            });

                            row.ConstantItem(10);

                            row.RelativeItem().Border(1).Padding(10).Column(c =>
                            {
                                c.Item().Text("BÊN B: KHÁCH HÀNG").Bold();
                                c.Item().Text($"Ông/Bà: {_detail.Customer.Fullname}");
                                c.Item().Text($"Điện thoại: {_detail.Customer.Phone}");
                                c.Item().Text($"Email: {_detail.Customer.Email}");
                                c.Item().Text("Địa chỉ: ....................");
                            });
                        });

                        // ===== ĐIỀU 1 =====
                        col.Item().PaddingTop(15).Background("#A9A9A9").Padding(5).Text("ĐIỀU 1: NỘI DUNG SỰ KIỆN").Bold().FontColor("#fff");

                        col.Item().Text($"- Loại hình: {_detail.Event.Title}");
                        col.Item().Text($"- Thời gian: {_detail.ReservationTime} ngày {_detail.ReservationDate:dd/MM/yyyy}");
                        col.Item().Text($"- Quy mô: {_contract.NumberOfGuests} bàn");

                        // ===== ĐIỀU 2 =====
                        col.Item().PaddingTop(10).Background("#A9A9A9").Padding(5).Text("ĐIỀU 2: CHI TIẾT THỰC ĐƠN VÀ DỊCH VỤ")
                            .Bold().FontColor("#fff");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(120);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Border(1).Padding(5).Text("Nội dung");
                                h.Cell().Border(1).Padding(5).AlignCenter().Text("Loại");
                                h.Cell().Border(1).Padding(5).AlignRight().Text("Thành tiền");
                            });

                            // ===== FOOD =====
                            if (_detail.Foods != null && _detail.Foods.Any())
                            {
                                table.Cell().ColumnSpan(3)
                                    .Background("#f4f4f4")
                                    .Padding(5)
                                    .Text("A. THỰC ĐƠN (tính theo bàn)").Bold();

                                foreach (var food in _detail.Foods)
                                {
                                    var total = (food.UnitPrice ?? 0) * food.Quantity * _contract.NumberOfGuests;

                                    table.Cell().Border(1).Padding(5).Text(
                                        $"{food.FoodName}\n( {food.UnitPrice:N0} x {food.Quantity} x {_contract.NumberOfGuests} bàn )"
                                    );

                                    table.Cell().Border(1).AlignCenter().Text("Món");
                                    table.Cell().Border(1).AlignRight().Text($"{total:N0} VNĐ");
                                }
                            }

                            // ===== SERVICE =====
                            if (_detail.Services != null && _detail.Services.Any())
                            {
                                table.Cell().ColumnSpan(3)
                                    .Background("#f4f4f4")
                                    .Padding(5)
                                    .Text("B. DỊCH VỤ").Bold();

                                foreach (var s in _detail.Services)
                                {
                                    var total = (s.UnitPrice ?? 0) * (s.Quantity ?? 0);

                                    table.Cell().Border(1).Padding(5).Text(
                                        $"{s.ServiceName}\n( {s.Quantity} {s.Unit} x {s.UnitPrice:N0} )"
                                    );

                                    table.Cell().Border(1).AlignCenter().Text("DV");
                                    table.Cell().Border(1).AlignRight().Text($"{total:N0} VNĐ");
                                }
                            }
                        });

                        // ===== ĐIỀU 3 =====
                        col.Item().PaddingTop(10).Background("#A9A9A9").Padding(5).Text("ĐIỀU 3: THANH TOÁN")
                            .Bold().FontColor("#fff");

                        var depositPercent = _contract.TotalAmount > 0
                            ? (_contract.DepositAmount / _contract.TotalAmount * 100)
                            : 0;

                        col.Item().Border(2).Padding(10).Column(c =>
                        {
                            var tax = _contract.TotalAmount / 1.1m * 0.1m; 
                            var subtotal = _contract.TotalAmount - tax;

                            c.Item().Text($"Tạm tính: {subtotal:N0} VNĐ").Bold();
                            c.Item().Text($"Thuế (10%): {tax:N0} VNĐ").Bold();
                            c.Item().Text($"Tổng giá trị: {_contract.TotalAmount:N0} VNĐ").Bold();
                            c.Item().Text($"Đặt cọc ({depositPercent:0}%): {_contract.DepositAmount:N0} VNĐ");
                            c.Item().Text($"Còn lại: {(_contract.TotalAmount - (_contract.DepositAmount ?? 0)):N0} VNĐ");
                        });

                        // ===== ĐIỀU 4 =====
                        col.Item().PaddingTop(10).Background("#A9A9A9").Padding(5).Text("ĐIỀU 4: ĐIỀU KHOẢN")
                            .Bold().FontColor("#fff");

                        col.Item().Text("4.1. Bên A đảm bảo chất lượng dịch vụ và đúng thỏa thuận.");
                        col.Item().Text("4.2. Bên B thanh toán đầy đủ đúng hạn.");
                        col.Item().Text("4.3. Hủy hợp đồng sẽ mất cọc.");
                        col.Item().Text("4.4. Tranh chấp giải quyết theo pháp luật.");
                        if (!string.IsNullOrWhiteSpace(_contract.TermsAndConditions))
                        {
                            col.Item()
                                .PaddingTop(10)
                                .Background("#2c3e50")
                                .Padding(5)
                                .Text("ĐIỀU 5: ĐIỀU KHOẢN BỔ SUNG")
                                .Bold()
                                .FontColor("#fff");

                            col.Item()
                                .Padding(5)
                                .Border(1)
                                .Text(_contract.TermsAndConditions)
                                .FontSize(11);
                        }
                        col.Item().PaddingTop(10).AlignCenter()
                            .Text("Hợp đồng lập thành 02 bản có giá trị như nhau")
                            .Italic()
                            .FontSize(10);

                        // ===== SIGNATURE =====
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Text("ĐẠI DIỆN BÊN A").Bold();
                                c.Item().Height(50);
                                c.Item().Border(1).Padding(5)
                                    .Text($"✔ Đã ký\n{_contract.ManagerName ?? "Chưa Ký"}");
                            });

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Text("ĐẠI DIỆN BÊN B").Bold();
                                c.Item().Height(50);
                                if (_contract.Status == "Draft")
                                {
                                    c.Item().Border(1).Padding(5).Text($"Chờ ký hợp đồng !");                                 
                                }else if (_contract.Status == "Sent")
                                 {
                                     c.Item().Border(1).Padding(5).Text($"Đã gửi mail xác nhận");
                                }else if (_contract.Status == "Signed")
                                 {
                                     c.Item().Border(1).Padding(5).Text($"Chờ thanh toán tiền cọc");
                                }
                                else
                                {
                                    c.Item().Border(1).Padding(5).Text($"✔ Đã ký\n{_contract.CustomerName}");
                                }

                            });
                        });
                    });
                });
            }
        }
    }
}