# GoBetGoal_BackEnd 減重習慣平台 - 後端 API 專案 

本專案是「減重習慣平台」的後端 API 服務，負責處理所有核心業務邏輯、資料庫存取與使用者驗證。專案採用前後端分離架構，為前端應用程式（Web/Mobile）提供標準化的 RESTful API。

## 核心技術棧

| 類別 | 技術 | 版本/說明 |
| --- | --- | --- |
| **後端框架** | ASP.NET Web API | .NET Framework 4.7.2 |
| **資料庫** | MS SQL Server | 建議使用 2019 或以上版本 |
| **ORM** | Entity Framework | Code First |
| **部署目標** | Azure Virtual Machine | Windows Server, IIS |
| **版本控制** | Git / GitHub |  |

---

## 環境建置

在開始開發前，請確保您的開發環境已安裝以下軟體：

1. **Visual Studio 2022** (或 2019)
    - 請務必安裝「ASP.NET 與網頁程式開發」工作負載。
2. **.NET Framework 4.7.2 Developer Pack**
    - Visual Studio 安裝程式通常會包含，若無請自行下載安裝。
3. **MS SQL Server**
    - 建議安裝 [SQL Server Developer Edition](https://www.microsoft.com/zh-tw/sql-server/sql-server-downloads) 或 Express Edition。
4. **Git**
    - [官方網站下載](https://git-scm.com/downloads)。
5. **API 測試工具**
    - Postman。

---

## 專案啟動流程

請依照以下步驟來設定並在本機執行專案：

1. **Clone 專案庫**Bash
    
    ```bash
    cd [專案資料夾]
    git clone [您的 GitHub Repo URL]
    ```
    
    - **第一次建立專案時：創建`.gitignore` 。**
2. **設定資料庫連線**XML
    - 在專案根目錄中找到 `Web.config.example` 檔案，並將其複製一份，重新命名為 `Web.config`。
    - **注意：`Web.config` 已被加入 `.gitignore`，不會被提交到版本庫，以保護敏感資訊。**
    - 開啟 `Web.config`，找到 `<connectionStrings>` 區塊，修改 `DefaultConnection` 的 `connectionString` 以符合您本機的 SQL Server 設定。
        
        ```csharp
        <connectionStrings>
          <add name="DefaultConnection" 
               connectionString="Data Source=localhost;Initial Catalog=資料庫名稱;Integrated Security=True" 
               providerName="System.Data.SqlClient" />
        </connectionStrings>
        ```
        
3. **還原 NuGet 套件**
    - 在 Visual Studio 中，對著方案 (Solution) 按右鍵，選擇「還原 NuGet 套件」。
4. **執行資料庫遷移 (Migration)**
    - 開啟 Visual Studio 中的「套件管理器主控台」 (Package Manager Console)。
    - 執行以下指令以建立並更新資料庫：`Update-Database -Verbose`
    - 這會根據 `Migrations` 資料夾中的設定，自動在您的 SQL Server 中建立資料庫和資料表。
5. **建置並執行專案**
    - 在 Visual Studio 中按下 `F5` 或點擊「IIS Express」按鈕來啟動專案。
    - 專案啟動後，瀏覽器會自動開啟，並導向 Swagger API 文件頁面（例如 `https://localhost:443xx/swagger`）。

---

## API 設計規範與文件

為確保 API 的一致性與易用性，我們遵循以下設計規範：

1. **API 路徑**
    - 所有 API 路徑皆以 `/api` 為前綴。
    - 採用版本控制，例如：`/api/v1/users`, `/api/v1/records`。
2. **命名慣例**
    - 使用複數名詞來命名資源，例如 `users`, `habits`。
    - URL 路徑使用小寫。
3. **HTTP 動詞**
    - `GET`：讀取資源。
    - `POST`：新增資源。
    - `PUT`：完整更新資源。
    - `PATCH`：部分更新資源。
    - `DELETE`：刪除資源。
4. **回應格式**JSON
    - 所有 API 回應皆為 JSON 格式。
    - 設計統一的回應結構，方便前端處理：
        
        ```json
        // 成功
        {
          "success": true,
          "data": { ... } // 回傳的資料
        }
        
        // 失敗
        {
          "success": false,
          "error": {
            "code": "ErrorCode", // 自定義錯誤碼
            "message": "錯誤訊息描述"
          }
        }
        ```
        
5. **API 文件**
    - 開發新的 API 時，務必為 Controller 和 Action 加上 XML 註解，以產生清晰的文件說明。

---

## Git 協作流程

- **英文一率小寫**

為確保協作順暢，我們採用簡化的 Git-Flow 模型：

1. **主要分支**
    - `main`: 主分支，存放穩定且可隨時部署到正式環境的程式碼。**不允許直接 commit 到 `main`。**
    - `dev`: 開發分支，整合所有已完成的功能。是建立功能分支的基礎。
2. **開發流程**
    - **Step 1: 建立功能分支 (Feature Branch)**
        - 從最新的 `dev` 分支建立新的功能分支。分支命名規則：`feature/[頁面名稱]-[子頁面/功能]`。
        
        ```bash
        git checkout dev
        git pull
        git checkout -b feature/[頁面名稱]
        ```
        
    - **Step 2: 開發與提交**
        - 在自己的功能分支上進行開發，並頻繁提交 (commit)。
            - Commit Message: “提交動作: 動作說明”(說明一般為動詞開頭)
    - **Step 3: 發起合併請求 (Pull Request, PR)**
        - 功能開發完成後，將分支推送到 GitHub。
        - 在 GitHub 上建立一個 Pull Request，目標分支為 `dev`。
    - **Step 4: 程式碼審查 (Code Review)**
        - 另一位協作者需要對 PR 進行審查。
        - 審查通過後，由 PR 的發起者將其合併 (Merge) 到 `dev` 分支。

---

## 程式碼風格與規範

- **命名**:
    - `class`, `method`, `property` 使用 `PascalCase`。
    - 私有欄位 (private field) 使用 `_camelCase`。
    - 參數與區域變數使用 `camelCase`。
---

## 資料庫遷移 (Database Migration)

本專案使用 Entity Framework Code First Migrations 來管理資料庫結構的變更。

- **新增 Migration**: 當你修改了 Model (位於 `Models` 或 `Entities` 資料夾) 後，執行以下指令來產生新的遷移腳本：
    
    ```csharp
    # PM>
    Add-Migration [你的遷移名稱，例如 AddUserWeightColumn]
    ```
    
- **套用 Migration**: 執行以下指令將變更應用到資料庫：
    
    ```csharp
    # PM>
    Update-Database
    ```
    
- **還原 Migration**: 如果需要，可以還原到指定的遷移版本。

---

## Azure VM 部署指南

以下是在 Azure VM 上部署本專案的概要步驟：

1. **VM 環境準備**:
    - 在 Azure 上建立一台 Windows Server 2019/2022 虛擬機。
    - 遠端登入 VM，並安裝以下角色與功能：
        - **IIS (Internet Information Services)**: 確保勾選 ASP.NET 4.7/4.8 支援。
        - **.NET Framework 4.7.2** (或更高版本)。
        - **URL Rewrite Module**: 用於處理路由。
2. **發布專案**:
    - 在 Visual Studio 中，對著專案按右鍵，選擇「發佈 (Publish)」。
    - 建立一個新的發佈設定檔，選擇「資料夾 (Folder)」作為目標。
    - 設定目標路徑，並點擊「發佈」。這會在指定資料夾中產生所有部署所需的檔案。
3. **部署到 IIS**:
    - 將發佈產生的所有檔案複製到 VM 的網站目錄中 (例如 `C:\inetpub\wwwroot\habitapi`)。
    - 在 IIS 管理器中，新增一個網站，將實體路徑指向該目錄。
    - 設定應用程式集區 (Application Pool) 的 .NET CLR 版本為 `v4.0`。
4. **設定 Web.config**:
    - 修改部署在 VM 上的 `Web.config` 檔案，將資料庫連線字串指向 Azure 上的 SQL 資料庫。**此步驟至關重要。**
5. **防火牆與網路安全群組 (NSG)**:
    - 確保 Azure 的 NSG 已開啟 Port 80 (HTTP) 和/或 Port 443 (HTTPS) 的傳入規則，以允許外部流量訪問您的 API。

---

## 專案結構 (建議)

```bash
/GoBetGoal_Backend
│
├── /GoBetGoal_Backend.API             // Web API 主專案
│   ├── /App_Start               // 設定檔 (WebApiConfig, SwaggerConfig)
│   ├── /Controllers             // API 控制器
│   ├── /Migrations              // EF 遷移腳本
│   ├── /Models                  // Entity Framework 資料模型
│   └── Web.config
│
├── .gitignore
└── README.md
```

---

## 主要聯絡人

| 姓名 | 職責 | GitHub |
| --- | --- | --- |
| [後端開發者 Kelly] | [@kc34522] |
| [後端開發者 Wein] | [@weiweins] |
