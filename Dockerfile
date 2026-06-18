# GIAI ĐOẠN 1: BUILD 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# 1. Copy tất cả file .csproj vào đúng cấu trúc để restore thư viện trước (Tối ưu cache)
COPY SimpleShop.API/SimpleShop.API.csproj SimpleShop.API/
COPY SimpleShop.Repo/SimpleShop.Repo.csproj SimpleShop.Repo/
COPY SimpleShop.Service/SimpleShop.Service.csproj SimpleShop.Service/

# 2. Restore thư viện của dự án chính (nó sẽ tự động restore các dự án Repo và Service liên quan)
RUN dotnet restore SimpleShop.API/SimpleShop.API.csproj

# 3. Copy toàn bộ mã nguồn của tất cả các thư mục vào container
COPY . .

# 4. Biên dịch riêng dự án API vào thư mục /app/out
RUN dotnet publish SimpleShop.API/SimpleShop.API.csproj -c Release -o /app/out

# GIAI ĐOẠN 2: RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Cấu hình cổng mạng cho Render
ENV ASPNETCORE_URLS=http://+:10000

# Kích hoạt file dll chính xác của bạn
ENTRYPOINT ["dotnet", "SimpleShop.API.dll"]