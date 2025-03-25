using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudentManagementV1._5.ViewModels
{
    // Lớp ViewModelBase
    // + Tại sao cần sử dụng: Cung cấp lớp cơ sở cho tất cả các ViewModel với các chức năng thông báo thay đổi thuộc tính
    // + Lớp này được kế thừa bởi tất cả các ViewModel trong ứng dụng
    // + Chức năng chính: Triển khai INotifyPropertyChanged để hỗ trợ binding dữ liệu từ ViewModel đến View
    public class ViewModelBase : INotifyPropertyChanged
    {
        // 1. Sự kiện thông báo khi thuộc tính thay đổi
        // 2. Được sử dụng bởi các binding trong XAML để cập nhật UI
        // 3. Cần thiết cho việc binding hai chiều trong mẫu thiết kế MVVM
        public event PropertyChangedEventHandler? PropertyChanged;

        // 1. Phương thức kích hoạt sự kiện PropertyChanged
        // 2. Được gọi khi một thuộc tính thay đổi giá trị
        // 3. Sử dụng CallerMemberName để tự động nhận tên thuộc tính từ trình gọi
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 1. Phương thức tiện ích để cập nhật giá trị thuộc tính
        // 2. Kiểm tra nếu giá trị mới khác giá trị hiện tại thì mới cập nhật
        // 3. Tự động kích hoạt sự kiện PropertyChanged khi giá trị thay đổi
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}