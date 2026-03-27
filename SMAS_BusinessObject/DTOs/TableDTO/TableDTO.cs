using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.TableDTO
{
    // POST /api/tables/{tableCode}/open
    public class OpenTableResponseDto
    {
        public string TableCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime OpenedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    // POST /api/tables/{tableCode}/close
    public class CloseTableResponseDto
    {
        public string TableCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime ClosedAt { get; set; }
    }

    // POST /api/tables/{tableCode}/init  - Khách quét QR gọi
    public class TableInitResponseDto
    {
        public string AccessToken { get; set; } = null!;   // JWT ngắn 15-30 phút
        public string RefreshToken { get; set; } = null!;  // JWT dài theo phiên
        public string TableCode { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int ExpiresInSeconds { get; set; }
    }

    // POST /api/tables/refresh  - Refresh access token
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public int ExpiresInSeconds { get; set; }
    }

    // GET /api/tables/{tableCode}/active-session
    public class ActiveSessionResponseDto
    {
        public string TableCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime OpenedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class TableResponseDTO
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = null!;
        public string? TableType { get; set; }
        public int NumberOfPeople { get; set; }
        public string? Status { get; set; }   // ACTIVE, CLOSED
        public bool? IsActive { get; set; }
        public string? QrCode { get; set; }

        /// <summary>Số khách hiện tại (từ TableOrder đang active)</summary>
        public int CurrentGuests { get; set; }

        /// <summary>Doanh thu hiện tại của bàn (từ Order đang OPEN)</summary>
        public decimal CurrentAmount { get; set; }
    }
    public class CreateTableDto
    {
        public string TableName { get; set; } = null!;
        public string? TableType { get; set; }
        public int NumberOfPeople { get; set; }
    }

    public class UpdateTableDto
    {
        public string TableName { get; set; } = null!;
        public string? TableType { get; set; }
        public int NumberOfPeople { get; set; }
    }

}
