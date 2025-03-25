using System.Windows;

/*
 * File AssemblyInfo.cs
 * 
 * Tại sao sử dụng:
 * - Chứa các thuộc tính cấp assembly cho ứng dụng WPF
 * - Cấu hình cách tài nguyên XAML được tìm kiếm và áp dụng
 * 
 * Quan hệ với các file khác:
 * - Ảnh hưởng đến cách tài nguyên được tìm kiếm trong App.xaml và các ResourceDictionary khác
 * - Hỗ trợ hệ thống theme và styling của WPF
 * 
 * Chức năng chính:
 * - Định nghĩa vị trí tài nguyên chung và chủ đề cho ứng dụng
 * - Cấu hình cách ứng dụng tìm kiếm và sử dụng các tài nguyên XAML
 */
[assembly:ThemeInfo(
    ResourceDictionaryLocation.None,            // Vị trí của từ điển tài nguyên chủ đề cụ thể
                                                // (sử dụng khi không tìm thấy tài nguyên trong trang,
                                                // hoặc từ điển tài nguyên ứng dụng)
    ResourceDictionaryLocation.SourceAssembly   // Vị trí của từ điển tài nguyên chung
                                                // (sử dụng khi không tìm thấy tài nguyên trong trang,
                                                // ứng dụng, hoặc bất kỳ từ điển tài nguyên chủ đề cụ thể nào)
)]
