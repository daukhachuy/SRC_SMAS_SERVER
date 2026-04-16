# SignalR Frontend Integration Guide

## Cai dat

```bash
npm install @microsoft/signalr
```

---

# 1. KITCHEN REALTIME

## Ket noi

- **Hub URL:** `/hubs/kitchen`
- **Roles:** Kitchen, Waiter, Manager, Admin
- **Auth:** Truyen JWT qua `accessTokenFactory` — backend nhan token tu query string `?access_token=...`
- Khi connect, server tu dong them user vao group theo role (Kitchen/Waiter/...) — khong can goi them gi

## Events lang nghe

### `OrderItemStatusChanged`

Khi **1 mon** thay doi trang thai. Payload:

```
{
  orderId: number,
  orderItemId: number,
  status: "Preparing" | "Ready" | "Served" | "Cancelled",
  updatedAt: string (ISO datetime)
}
```

Mapping:
- Nut ▶ (nau 1 mon) → `PATCH /api/order-items/{id}/preparing` → status "Preparing"
- Mon nau xong → `PATCH /api/order-items/{id}/ready` → status "Ready"
- Waiter phuc vu → `PATCH /api/order-items/{id}/Served` → status "Served"
- Huy mon → `POST /api/order-items/{id}/cancel` → status "Cancelled"

### `AllItemsStatusChanged`

Khi **tat ca items** cua 1 order duoc cap nhat dong loat. Payload:

```
{
  orderId: number,
  orderItemIds: number[],
  status: "Preparing" | "Ready",
  updatedAt: string (ISO datetime)
}
```

Mapping:
- "BAT DAU TAT CA" → `PATCH /api/orders/{orderId}/items/all-preparing`
- "HOAN THANH TAT CA" → `PATCH /api/orders/{orderId}/items/all-ready`

### `NewOrderItems`

Khi co **mon moi** duoc them vao don (khach order them). Payload:

```
{
  orderId: number,
  orderCode: string,
  createdAt: string (ISO datetime)
}
```

Khi nhan event nay, fetch lai `GET /api/order-items/pending` de cap nhat danh sach.

---

# 2. CHAT REALTIME

## Ket noi

- **Hub URL:** `/hubs/chat`
- **Roles:** Manager, Customer, Admin
- **Auth:** Tuong tu kitchen — truyen JWT qua `accessTokenFactory`
- Khi connect, server tu dong them user vao group `user_{userId}` (de nhan thong bao conversation moi)

## Server Methods (client goi len server)

| Method | Khi nao goi | Tham so |
|--------|-------------|---------|
| `JoinConversation` | Mo 1 cuoc hoi thoai | `conversationId: number` |
| `LeaveConversation` | Dong/chuyen cuoc hoi thoai | `conversationId: number` |

**Bat buoc** goi `JoinConversation` truoc khi co the nhan `ReceiveMessage` va `MessagesRead` cua conversation do.

## Events lang nghe

### `ReceiveMessage`

Khi co **tin nhan moi** trong conversation dang mo. Payload:

```
{
  messageId: number,
  conversationId: number,
  senderId: number,
  content: string | null,
  attachmentUrl: string | null,
  messageType: string,
  sentAt: string (ISO datetime),
  isRead: boolean
}
```

Trigger boi: `POST /api/conversation/send-messages`

### `MessagesRead`

Khi doi phuong **da doc** tin nhan. Payload:

```
{
  conversationId: number,
  readByUserId: number,
  readAt: string (ISO datetime)
}
```

Trigger boi: `PUT /api/conversation/{conversationId}/read`

### `NewConversation`

Khi doi phuong **tao conversation moi** voi minh. Payload:

```
{
  conversationId: number,
  userId: number,
  userName: string,
  userAvatar: string | null,
  lastMessage: string | null,
  lastMessageAt: string | null (ISO datetime),
  unreadCount: number
}
```

Trigger boi: `POST /api/conversation/manager-create` hoac `POST /api/conversation/customer-create`

---

# 3. LUU Y QUAN TRONG

- **API calls giu nguyen** — SignalR chi bo sung nhan event realtime tu phia nguoi khac
- Nguoi bam nut nhan response tu REST API, nguoi con lai nhan event tu SignalR
- Dung `withAutomaticReconnect()` khi tao connection
- Khi reconnect thanh cong, can **re-join** conversation group (neu dang mo chat)
- Luon `connection.stop()` khi component unmount de tranh memory leak
