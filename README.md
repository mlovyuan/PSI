# PSI - ERP Practice

### 專案分層設定
---
- UI（表現層）：展示用戶畫面。
- BLL（業務邏輯層）：將DAL返回的數據進行處理。
- DAL（資料訪問層）：操作資料庫CRUD動作。
- Model（實體模型層）：於三層中傳遞數據。
- Common（輔助工具層）
- DbUtility：作為DAL更底層的設計，決定連接使用的資料庫種類等等。
