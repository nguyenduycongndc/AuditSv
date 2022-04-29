
## .NET 5.0
## Visual studio 2019

# Key path thư mục dùng chung
  "Upload": {
    "AuditDocsPath": "/opt/kitano_app/file_folder/AuditDocs"
  },
  "Template": {
    "AuditDocsTemplate": "/opt/kitano_app/file_folder/AuditDocs/Templates"
  },

# Hướng dẫn cài đặt, biên dịch và chạy ứng dụng WEB API Core Service 

Cài đặt các phần mềm liên quan
- **Bộ ASP.NET Core SDK .NET 5.0** (tải .NET 5.0 tại https://dotnet.microsoft.com/download)
- **Một môi trường phát triển ứng dụng tích hợp như Visual Studio 2019, JetBrains Rider hoặc một code editor như Visual Studio Code**
- **Swashbuckle.AspNetCore (update version nếu có)**
- **Library sp PostgreSQL** :
	Npgsql.EntityFrameworkCore.PostgreSQL, 
	Npgsql.EntityFrameworkCore.PostgreSQL.Design, 
	Microsoft.EntityFrameworkCore.Tools
- **Library sp JWT** : 
	System.IdentityModel.Tokens.Jwt, 
	Microsoft.AspNetCore.Authentication.JwtBearer
- **Library sp Redis** :
	StackExchange.Redis.StrongName.
- **Library sp Mongo** :
	MongoDB.Driver.
## Cài đặt ASP.NET Core SDK trên Window
- Để thực thi ứng dụng ASP.NET Core, cần cài đặt ASP.NET Core Runtime. Để phát triển ứng dụng, cần cài đặt ASP.NET Core SDK. 
Khi cài đặt SDK sẽ đồng thời cài đặt luôn Runtime.

### 1.Cài đặt không dùng Visual Studio 2019
- Tải bộ cài về và cài đặt vào máy.
- Sau khi cài đặt xong, chạy lệnh dotnet --version trên Command Prompt hoặc PowerShell để kiểm tra kết quả version.

### 2.Dùng Visual Studio 2019
- Lưu ý, Visual Studio 2017 chỉ hỗ trợ các phiên bản trước của .NET Core.
- Nếu chưa có hoặc đang dùng một bản Visual Studio cũ, hãy cài đặt Visual Studio (community) 2019. 
Trong quá trình cài đặt, hãy chọn Workloads ASP.NET and web development (phát triển ứng dụng trên cả ASP.NET và ASP.NET Core) hoặc .NET Core cross-platform development (phát triển ứng dụng trên .NET Core và ASP.NET Core).
- Nếu đã cài đặt sẵn Visual Studio 2019, hãy update lên build mới nhất. 
Sau đó chạy chương trình Visual Studio Installer => chọn Modify => chọn tab Workloads và cũng lựa chọn một trong hai mục như trên.
- Khi quá trình cài đặt hoàn tất, đã sẵn sàng cho cả việc phát triển và chạy ứng dụng ASP.NET Core.
Chạy lệnh dotnet --version trên Command Prompt hoặc PowerShell để kiểm tra kết quả version.


