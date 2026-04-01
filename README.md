# Fraud Detection API & UI

A full-stack application that simulates a fraud detection system by analyzing financial transactions and applying rule-based decision logic.

## 🚀 Features

- Submit transactions
- Fraud decision logic (Approved, Suspicious, Fraud)
- Reason explanations
- View transaction history
- Delete transactions

## 🏗️ Tech Stack

### Backend

- C#
- ASP.NET Core
- Entity Framework Core
- SQL Server

### Frontend

- React
- TypeScript
- Tailwind CSS

## Screenshots

- ![Form](docs/1.png)
- ![Transactions](docs/2.png)

## ⚙️ Running the Project

### Backend

```bash
cd FraudDetectionApi
dotnet run
```

### Frontend

```bash
cd fraud-detection-ui
npm install
npm run dev
```

## 🔌 API Endpoints

- GET /api/transactions
- GET /api/transactions/{id}
- POST /api/transactions
- DELETE /api/transactions/{id}

## 🧪 Example Request

```json
{
  "accountId": "ACC123",
  "amount": 1200,
  "country": "US",
  "accountHomeCountry": "US",
  "merchant": "Amazon",
  "occurredAt": "2026-03-31T12:00:00"
}
```

## 📊 Example Response

```json
{
  "id": 1,
  "accountId": "ACC123",
  "amount": 1200,
  "country": "US",
  "merchant": "Amazon",
  "occurredAt": "2026-03-31T12:00:00",
  "decision": "Fraud",
  "reason": "Transaction amount exceeds threshold"
}
```

## 💡 What This Demonstrates

- REST API design
- Layered backend architecture
- DTO usage
- Full-stack integration
- React state & forms
- Error handling

## 🔮 Future Improvements

- Authentication (JWT)
- Filtering & pagination
- Background processing
- ML-based fraud scoring

## 👤 Author

Jeff Ellis  
Full-Stack Software Engineer
