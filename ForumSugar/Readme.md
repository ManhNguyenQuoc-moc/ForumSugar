**#Chay database voi entity framework **
Add-Migration InitialCreate
Update-Database
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations add AddDateOfBirthToUser
dotnet ef database update
**#Thông tin dự án**
- dự án được build với công nghệ Net Core API để làm backend và fontend sử dụng bootrap 4 cùng với html và css
**# cách chạy dự án**
-import database vào SQL sever từ file ForumDatabase.pacpac trong thư mục data
+ kiểm tra chuỗi kết nối trong file appSetting.json
+ kiểm tra port của máy port hiện tại http://localhost/5093, thay đổi 
- tải các thư viện JWT, Entity Freamwork (tool,SQL,manager,core)
- nhấn nút run để mở Swager API để xem các API được viết
- mở file HTML và chạy file này lên web
- có thể tạo tài khoản mới để test
  +TK: username
  +Mk: password
**#Các chức năng đã thực hiện**
Đăng kí
Đăng Nhập
Đăng xuất
Xem bài viết
Duyệt bài
Xem trang cá nhân
Xem bài viết của bản thân
React bài viết
Thông báo khi bài viết được duyệt
