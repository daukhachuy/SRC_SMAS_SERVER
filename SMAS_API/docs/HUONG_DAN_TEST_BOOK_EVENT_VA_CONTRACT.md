# Hướng dẫn test API: BookEvent → Hợp đồng → Ký → Cọc PayOS → Xác nhận

Tài liệu bám **code hiện tại** (không dùng mô tả cũ “sau ký → BookEvent Confirmed”).  
Chuẩn bị: Swagger (`/swagger`), Postman, hoặc `curl`. Base URL ví dụ: `https://localhost:7001`.

---

## 1. Luồng nghiệp vụ (đúng với code)

| Bước | Hành động                           | BookEvent (`Status`)                      | Contract (`Status`)     | Ghi chú                                                           |
| ---- | ----------------------------------- | ----------------------------------------- | ----------------------- | ----------------------------------------------------------------- |
| 1    | Khách tạo đặt chỗ                   | `Pending`                                 | —                       | JWT Customer                                                      |
| 2    | Manager duyệt                       | `Approved`                                | —                       | `POST .../review`                                                 |
| 3    | Manager tạo HĐ từ sự kiện           | `Approved` (giữ)                          | `Draft`                 | `POST .../contract`                                               |
| 4    | Manager gửi mail ký                 | `Approved`                                | `Sent`                  | `POST .../send-sign`                                              |
| 5    | Khách xem HĐ (token)                | `Approved`                                | `Sent`                  | `GET /api/contract/sign?token=`                                   |
| 6    | Khách ký                            | **`Approved`** (chỉ cập nhật `UpdatedAt`) | `Signed`, có `SignedAt` | **Không** còn set BookEvent → `Confirmed`                         |
| 7    | Trong **N giờ** (UTC): tạo link cọc | `Approved`                                | `Signed`                | `DepositDueUtc` = `SignedAt + N` (DTO)                            |
| 8    | PayOS callback / webhook            | **`Active`**                              | `Deposited`             | Sau khi ghi payment cọc thành công sẽ tự động kích hoạt BookEvent |

**Hết hạn cọc:** Job nền có thể đặt **Contract** và **BookEvent** liên quan → `Cancelled` nếu vẫn `Signed` và quá `SignedAt + DepositDeadlineHoursAfterSign` (cấu hình `App`).

---

## 2. Cấu hình cần biết khi test

Trong `appsettings.json` → mục **`App`**:

| Key                                      | Ý nghĩa                                                                |
| ---------------------------------------- | ---------------------------------------------------------------------- |
| `PublicBaseUrl`                          | URL API công khai (build link ký, callback PayOS).                     |
| `FrontendBaseUrl`                        | Redirect sau callback PayOS.                                           |
| `DepositDeadlineHoursAfterSign`          | Số giờ (UTC) từ `SignedAt` để được phép cọc (mặc định trong code: 24). |
| `ContractExpirationSweepIntervalMinutes` | Chu kỳ job hủy HĐ quá hạn cọc.                                         |

PayOS: mục **`PayOS`** (ClientId, ApiKey, ChecksumKey) — cần hợp lệ nếu test thật.

---

## 3. Chuẩn bị JWT

- **Customer:** đăng nhập `POST /api/auth/login` (hoặc register) → token → dùng cho `book-event/create`, `book-event/my`.
- **Manager:** user role Manager → token cho review, contract, send-sign, asc-created-at, …
- **Staff:** token cho `GET /api/book-event/{id}/detail`.
- **Cashier:** token cho `POST /api/contract/{id}/deposit` (cùng Manager).

Header mọi request có auth:

```http
Authorization: Bearer <access_token>
```

---

## 4. API `api/book-event` — test từng bước

### 4.1. Tạo đặt chỗ — `POST /api/book-event/create`

- **Auth:** bắt buộc (controller class `[Authorize]`). Thực tế cần user có quyền gọi được endpoint; thường là **Customer** (role trong token).
- **Body (JSON):** `CreateBookEventApiRequestDTO`

```json
{
  "eventId": 1,
  "numberOfGuests": 2,
  "reservationDate": "2026-05-01",
  "reservationTime": "18:30:00",
  "note": "Sinh nhật",
  "area": "Trong nhà",
  "services": [],
  "foods": []
}
```

- **Lưu ý:** `numberOfGuests` là **số bàn** (theo code). `customerId` **không** gửi — lấy từ JWT.
- **Kỳ vọng:** `200` — `data.bookEventId`, `bookingCode`, … DB: `BookEvent.Status = Pending`.

### 4.2. (Tuỳ chọn) Lịch sử khách — `GET /api/book-event/my`

- **Auth:** `Customer`
- **Kỳ vọng:** Danh sách BookEvent của user (theo JWT).

### 4.3. Danh sách theo thời gian tạo — `GET /api/book-event/asc-created-at`

- **Auth:** `Manager`
- **Kỳ vọng:** Mảng book event (DTO manager).

### 4.4. Danh sách “active” (chưa Completed/Cancelled) — `GET /api/book-event/active`

- **Auth:** `Admin`, `Manager`

### 4.5. Duyệt đặt chỗ — `POST /api/book-event/{id}/review`

- **Auth:** `Manager`
- **Body:**

```json
{
  "decision": "Approved",
  "note": null
}
```

hoặc `"Rejected"` + `note` khi từ chối.

- **Kỳ vọng:** `200` — `BookEvent.Status = Approved` (nếu Approved).

### 4.6. Tạo hợp đồng từ sự kiện — `POST /api/book-event/{id}/contract`

- **Auth:** `Manager`
- **Body:** `CreateContractFromBookEventRequestDTO`

```json
{
  "depositPercent": 30,
  "termsAndConditions": "Điều khoản...",
  "note": null
}
```

- **Kỳ vọng:** `201` — `contractId`, `contractCode`, …
- **DB:** `Contract.Status = Draft`, liên kết `BookEvent`.

### 4.7. Chi tiết sự kiện (workflow + thanh toán) — `GET /api/book-event/{id}/detail`

- **Auth:** `Manager`, `Staff`
- **Kỳ vọng:** `BookEventDetailResponseDTO` — trong `contract` có `depositDueUtc` khi HĐ đã `Signed` (tính từ `SignedAt` + cấu hình).

### 4.9. Khác

- `GET /api/book-event/history` — Admin, Manager (Completed/Cancelled).
- `GET /api/book-event/{bookEventId}` — Admin, Manager (theo id).

---

## 5. API `api/contract` — test từng bước

### 5.1. Số HĐ “cần ký” — `GET /api/contract/number-need-signed`

- **Auth:** `Manager`
- **Ý nghĩa (code):** đếm hợp đồng có **`Status` là `Draft` hoặc `Sent`** (chưa tới bước khách ký → `Signed`).
- **Nếu `totalCount` = 0:** mọi HĐ của bạn đã qua bước ký (`Signed`, `Deposited`, …) hoặc đã `Cancelled`, hoặc **chưa tạo HĐ** từ BookEvent (`POST .../contract`).

### 5.2. Gửi email ký — `POST /api/contract/{id}/send-sign`

- **Auth:** `Manager`
- **Path `id`:** `contractId` (không phải bookEventId).
- **Kỳ vọng:** `200` — email gửi (nếu SMTP đúng); DB: `Contract.Status = Sent`, token lưu ở `SignMethod`.

### 5.3. Xem HĐ trước khi ký — `GET /api/contract/sign?token={token}`

- **Auth:** không (AllowAnonymous)
- **Query:** `token` = giá trị `SignMethod` (hoặc copy từ link email).

### 5.4. Ký HĐ — `POST /api/contract/sign`

- **Auth:** AllowAnonymous
- **Body:**

```json
{
  "token": "<chuỗi token>"
}
```

**Làm sao biết người ký là ai dù không có JWT?**

- Hợp đồng **đã gắn sẵn khách** qua `Contract.CustomerId` (tạo từ BookEvent của khách đó).
- **Token** trong body là chuỗi bí mật lưu ở `Contract.SignMethod`, **chỉ gửi tới email** `Customer` khi Manager gọi `send-sign` (giống “magic link”).
- API không ghi thêm “userId người bấm ký”; nghiệp vụ coi **ai nắm link hợp lệ trong hạn** thì đại diện khách xác nhận ký — tương tự ký qua email/link phổ biến.
- **Rủi ro / cải tiến (nếu sau này cần):** bắt đăng nhập Customer + kiểm tra `CustomerId` khớp contract; hoặc OTP; hoặc chữ ký điện tử mạnh hơn.

- **Kỳ vọng:** `200` — `signedAt`, **`depositDueUtc`** (hạn cọc).
- **DB:** `Contract.Signed`, `SignedAt`; BookEvent **vẫn Approved** (chỉ `UpdatedAt`).

### 6. Tự động kích hoạt BookEvent khi cọc thành công

Khi PayOS callback/webhook ghi nhận cọc thành công:

- `Contract.Status` → `Deposited`
- `Payment.PaymentStatus` → `Paid` (note = `deposit`)
- `BookEvent.Status` → `Active` (**tự động**, API `book-event/confirm` đã bị loại bỏ)

---

## 7. Test end-to-end (gợi ý thứ tự)

1. Login **Customer** → `POST /api/book-event/create` → ghi `bookEventId`, `bookingCode`.
2. Login **Manager** → `POST /api/book-event/{bookEventId}/review` với `Approved`.
3. `POST /api/book-event/{bookEventId}/contract` → ghi `contractId`.
4. `POST /api/contract/{contractId}/send-sign` → lấy token từ DB (`Contract.SignMethod`) hoặc email.
5. (Không JWT) `GET /api/contract/sign?token=...` → `POST /api/contract/sign`.
6. **Manager/Cashier** → `POST /api/contract/{contractId}/deposit` → thanh toán sandbox PayOS.
7. Kiểm tra DB: `Contract.Status = Deposited`, bảng `Payment` có dòng cọc `Paid`.
8. Sau khi PayOS ghi nhận cọc (callback/webhook) → `Contract.Status = Deposited`, bảng `Payment` có dòng cọc `Paid`, và `BookEvent.Status = Active` tự động.

---

## 8. Số tiền hợp đồng / cọc quá lớn — có sửa DB được không?

**Có**, khi **test nội bộ** bạn có thể:

1. **Giảm `DepositAmount` (và tương ứng `TotalAmount` nếu cần nhất quán)** trên bảng `Contract` để số tiền gửi PayOS (làm tròn `int` VND trong code) nằm trong giới hạn sandbox và kiểu `int`.
2. Hoặc chỉnh **`depositPercent`** khi tạo HĐ (bước tạo contract) để cọc nhỏ hơn — **chỉ áp dụng được trước khi** đã tính xong tổng; nếu đã có HĐ thì sửa DB hoặc tạo HĐ mới.

**Lưu ý:**

- Sửa tay DB có thể **lệch** với tổng dịch vụ/món trên BookEvent; chỉ nên dùng cho **môi trường dev/test**.
- Cột `RemainingAmount` có thể là **computed** trong SQL Server — khi sửa `DepositAmount`/`TotalAmount` cần tuân theo quy tắc DB.
- Sau khi đã `Deposited` / đã có `Payment`, không nên đổi lung tung số tiền mà không đồng bộ payment.

---

## 8. Query string callback (tham chiếu)

Sau `deposit/callback`, frontend có thể nhận:

| `payment`          | Ý nghĩa (gần đúng theo code)                                    |
| ------------------ | --------------------------------------------------------------- |
| `success`          | Thanh toán xác thực OK và đã ghi cọc (hoặc đã có cọc trước đó). |
| `cancel`           | User huỷ ở PayOS.                                               |
| `expired`          | Hết hạn cọc hoặc repo từ chối ghi (race / deadline).            |
| `contract_invalid` | HĐ không còn `Signed` (vd đã `Cancelled`).                      |
| `error`            | Lỗi verify hoặc exception.                                      |

---

## 9. File code tham chiếu nhanh

- `SMAS_API/Controllers/BookEventController.cs`
- `SMAS_API/Controllers/ContractController.cs`
- `SMAS_Services/ContractWorkflow/ContractWorkflowService.cs`
- `SMAS_DataAccess/DAO/ContractDAO.cs`
- `SMAS_API/BackgroundJobs/ContractDepositExpirationHostedService.cs`

---

_Tài liệu sinh ra để test thủ công; nếu đổi route hoặc role trong code, cập nhật lại bảng tương ứng._
