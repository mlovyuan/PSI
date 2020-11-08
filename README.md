# PSI - ERP Practice

### 專案分層設定
---
- UI（表現層）：展示用戶畫面。
- BLL（業務邏輯層）：將DAL返回的數據進行處理。
- DAL（資料訪問層）：撰寫SQL => 操作資料庫CRUD動作。
- Model（實體模型層）：於三層中傳遞數據。
- Common（輔助工具層）：專案中可能會用到的某些小工具。
  - CustomAttributes：用於對應程式中實體與資料庫table、欄位名稱可能不一致。
- DbUtility：作為DAL更底層的設計，不做SQL語句撰寫，只負責執行SQL Script。
