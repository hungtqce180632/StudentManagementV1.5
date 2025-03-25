using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StudentManagementV1._5.Services
{
    // Lớp DatabaseService
    // + Tại sao cần sử dụng: Cung cấp kết nối và thực hiện các truy vấn đến cơ sở dữ liệu
    // + Lớp này được gọi từ các ViewModel và Service khác khi cần truy xuất hoặc cập nhật dữ liệu
    // + Chức năng chính: Thực hiện các truy vấn SQL và trả về kết quả cho các thành phần khác
    public class DatabaseService
    {
        // 1. Chuỗi kết nối đến cơ sở dữ liệu SQL Server
        // 2. Chứa thông tin máy chủ, database, tài khoản đăng nhập
        // 3. Nên được lưu trong cấu hình ứng dụng thay vì hard-code
        private readonly string _connectionString;

        // 1. Constructor của DatabaseService
        // 2. Khởi tạo chuỗi kết nối đến cơ sở dữ liệu
        // 3. Trong thực tế, nên đọc chuỗi kết nối từ cấu hình ứng dụng
        public DatabaseService()
        {
            _connectionString = "Server=localhost;Database=StudentManagementDB;User Id=sa;Password=123;TrustServerCertificate=True;";
        }

        // 1. Phương thức thực thi truy vấn và trả về DataTable
        // 2. Nhận câu truy vấn SQL và các tham số (nếu có)
        // 3. Trả về kết quả dưới dạng DataTable để hiển thị hoặc xử lý
        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            DataTable dataTable = new DataTable();
            await connection.OpenAsync();
            
            using SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);
            
            return dataTable;
        }

        // 1. Phương thức thực thi truy vấn không trả về dữ liệu (INSERT, UPDATE, DELETE)
        // 2. Nhận câu truy vấn SQL và các tham số (nếu có)
        // 3. Trả về số dòng bị ảnh hưởng bởi câu truy vấn
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        // 1. Phương thức thực thi truy vấn và trả về một giá trị đơn (COUNT, SUM, v.v.)
        // 2. Nhận câu truy vấn SQL và các tham số (nếu có)
        // 3. Trả về giá trị đầu tiên của kết quả truy vấn
        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            await connection.OpenAsync();
            return await command.ExecuteScalarAsync();
        }
    }
}