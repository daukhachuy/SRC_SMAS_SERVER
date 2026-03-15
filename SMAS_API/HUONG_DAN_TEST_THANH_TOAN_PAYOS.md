# Hướng dẫn test API thanh toán PayOS

## Chuẩn bị

- **Base URL API:** `https://localhost:7xxx` hoặc `http://localhost:5xxx` (xem khi chạy `dotnet run`).
- **JWT:** Đăng nhập (Auth) để lấy token, dùng header: `Authorization: Bearer <token>`.
- User phải có role **Customer** để tạo đơn và tạo link thanh toán.

---

## Luồng test
### Bước 1: Tạo đơn giao hàng

**Endpoint:** `POST /api/order/create/delivery`  
**Header:** `Authorization: Bearer <your_jwt>`

**Body mẫu (JSON):**

```json
{
  "discountCode": null,
  "note": null,
  "items": [
    { "foodId": 1, "comboId": null, "quantity": 2 }
  ],
  "deliveryInfo": {
    "recipientName": "Nguyễn Văn A",
    "recipientPhone": "0901234567",
    "address": "123 Đường ABC, Quận 1, Đà Nẵng",
    "note": null
  }
}
```

- Thay `foodId` bằng ID món có trong DB (hoặc dùng `comboId` nếu có combo).
- `deliveryInfo` là bắt buộc cho đơn giao hàng.

**Response thành công (200):**

```json
{
  "success": true,
  "message": "Đơn hàng của bạn đã thêm thành công.",
  "orderId": 123,
  "orderCode": "ORD-638123456789"
}
```

→ Ghi lại **orderId** để dùng ở bước 2.

---

### Bước 2: Tạo link thanh toán PayOS

**Endpoint:** `POST /api/payment/create-link`  
**Header:** `Authorization: Bearer <your_jwt>`

**Body (JSON):**

```json
{
  "orderId": 123,
  "returnUrl": "http://localhost:3000/order/success",
  "cancelUrl": "http://localhost:3000/order/cancel"
}
```

- `orderId`: lấy từ response bước 1.
- `returnUrl`: trang frontend khi thanh toán **thành công** (PayOS redirect về).
- `cancelUrl`: trang frontend khi user **hủy** thanh toán.

**Response thành công (200):**

```json
{
  "success": true,
  "message": null,
  "checkoutUrl": "https://pay.payos.vn/...",
  "qrCode": "data:image/...",
  "paymentLinkId": "..."
}
```

→ Mở **checkoutUrl** trong trình duyệt để vào trang thanh toán PayOS và thực hiện thanh toán (sandbox hoặc thật tùy kênh PayOS của bạn).

---

### Bước 3: Kiểm tra sau khi thanh toán

- PayOS redirect user về **returnUrl** (kèm query nếu có).
- PayOS gọi **webhook** của bạn để báo kết quả → backend cập nhật trạng thái đơn sang **Paid**.
- Có thể kiểm tra trạng thái đơn qua API lấy danh sách đơn (filter theo status) hoặc trực tiếp trong database (cột `OrderStatus`).

---

## Test bằng Swagger

1. Chạy API: `dotnet run` (trong thư mục SMAS_API).
2. Mở trình duyệt: `https://localhost:7xxx/swagger` (hoặc http nếu dùng port http).
3. **Authorize:** nhấn "Authorize", nhập `Bearer <token>` (có dấu cách sau Bearer).
4. Gọi lần lượt:
   - `POST /api/order/create/delivery` với body mẫu trên.
   - `POST /api/payment/create-link` với `orderId` vừa nhận, `returnUrl` và `cancelUrl` (có thể dùng `https://localhost:7xxx` tạm nếu chưa có frontend).

---

## Test Webhook (PayOS gọi về)

Webhook: `POST /api/payment/webhook` — **không** cần JWT.

**Cách 1 – Đăng ký với PayOS (khuyến nghị):**

1. Expose API local qua tunnel (vd: **ngrok**):  
   `ngrok http https://localhost:7xxx`
2. Đăng ký URL webhook trên https://my.payos.vn:  
   `https://<ngrok-id>.ngrok.io/api/payment/webhook`
3. Thanh toán thử trên PayOS → PayOS sẽ POST payload về URL này.

**Cách 2 – Gửi request mẫu (Postman / curl):**

Chỉ để kiểm tra backend có xử lý đúng body hay không. **Signature phải đúng** thì backend mới cập nhật đơn (verify HMAC). Có thể tạm thời bỏ qua verify trong code để test flow, hoặc tính đúng signature theo [tài liệu PayOS](https://payos.vn/docs/tich-hop-webhook/kiem-tra-du-lieu-voi-signature/).

Body mẫu PayOS gửi (tham khảo):

```json
{
  "code": "00",
  "desc": "success",
  "success": true,
  "data": {
    "orderCode": 123,
    "amount": 50000,
    "description": "ORD-638123456789",
    "accountNumber": "12345678",
    "reference": "TF230204212323",
    "transactionDateTime": "2023-02-04 18:25:00",
    "currency": "VND",
    "paymentLinkId": "124c33293c43417ab7879e14c8d9eb18"
  },
  "signature": "<HMAC_SHA256 của chuỗi data đã sort theo key>"
}
```

---

## Lỗi thường gặp

| Triệu chứng | Nguyên nhân / Cách xử lý |
|-------------|---------------------------|
| 401 Unauthorized | Chưa gửi JWT hoặc token hết hạn. Đăng nhập lại lấy token mới. |
| 400 "Đơn hàng không tồn tại" | `orderId` sai hoặc đơn chưa được tạo. Kiểm tra lại bước 1. |
| 400 "Đơn hàng không ở trạng thái chờ thanh toán" | Đơn đã Paid/Cancelled. Tạo đơn mới (bước 1) rồi gọi lại create-link. |
| 400 "Bạn không có quyền thanh toán đơn hàng này" | JWT là user khác với user tạo đơn. Dùng đúng tài khoản Customer đã tạo đơn. |
| PayOS trả lỗi khi create-link | Kiểm tra PayOS config trong `appsettings.json` (ClientId, ApiKey, ChecksumKey) và kênh thanh toán đang bật trên my.payos.vn. |
