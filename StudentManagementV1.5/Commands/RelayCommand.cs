using System;
using System.Windows.Input;

namespace StudentManagementV1._5.Commands
{
    // Lớp RelayCommand
    // + Tại sao cần sử dụng: Dùng để triển khai mẫu Command trong MVVM, giúp liên kết lệnh từ View đến ViewModel
    // + Lớp này được gọi từ các ViewModel và kết nối với các sự kiện từ giao diện người dùng (như nút bấm)
    // + Chức năng chính: Chuyển tiếp lệnh và điều kiện thực thi lệnh từ ViewModel đến UI
    public class RelayCommand : ICommand
    {
        // 1. Biến lưu trữ hành động sẽ được thực thi khi lệnh được gọi
        // 2. Thường là một phương thức từ ViewModel
        // 3. Nhận tham số là object và không trả về giá trị
        private readonly Action<object?> _execute;
        
        // 1. Biến lưu trữ hàm kiểm tra điều kiện thực thi lệnh
        // 2. Có thể null nếu lệnh luôn có thể thực thi
        // 3. Trả về boolean để xác định lệnh có thể thực thi hay không
        private readonly Func<object?, bool>? _canExecute;

        // 1. Constructor nhận hành động thực thi và điều kiện thực thi (tùy chọn)
        // 2. Kiểm tra và gán giá trị cho các biến thành viên
        // 3. Ném ra exception nếu hành động thực thi là null
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // 1. Sự kiện được kích hoạt khi điều kiện thực thi lệnh thay đổi
        // 2. Đăng ký và hủy đăng ký với CommandManager.RequerySuggested
        // 3. Giúp UI cập nhật trạng thái của các thành phần điều khiển (như nút bấm) khi điều kiện thay đổi
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // 1. Kiểm tra xem lệnh có thể thực thi với tham số hiện tại không
        // 2. Gọi hàm _canExecute nếu nó không null
        // 3. Trả về true nếu _canExecute là null hoặc _canExecute trả về true
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // 1. Thực thi lệnh với tham số được truyền vào
        // 2. Gọi hàm _execute đã được cung cấp trong constructor
        // 3. Thường được gọi từ UI khi người dùng tương tác (như click vào nút)
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        // 1. Phương thức yêu cầu hệ thống kiểm tra lại điều kiện thực thi lệnh
        // 2. Thông báo cho UI cập nhật trạng thái của các thành phần điều khiển
        // 3. Thường được gọi khi có thay đổi ảnh hưởng đến điều kiện thực thi lệnh
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}