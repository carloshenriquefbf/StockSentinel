# Stock Quote Alert

A console app that monitors B3 stock prices using the Alpha Vantage API and sends email alerts when buy/sell thresholds are reached.

## Setup

1. Create a `.env` file:
```env
API_KEY=your_api_key # https://www.alphavantage.co/support/#api-key
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password # https://myaccount.google.com/apppasswords
TARGET_MAIL_ADDRESS=recipient@gmail.com
```

2. Build and run:
```bash
dotnet build
dotnet run <ticker> <sellPrice> <buyPrice>
```

## Usage

```bash
dotnet run PETR4.SA 22.67 22.59
```

- **ticker**: Stock symbol (e.g., PETR4.SA)
- **sellPrice**: Send sell alert when price >= this value
- **buyPrice**: Send buy alert when price <= this value

## How it works

- Checks stock price every 5 minutes
- Sends email when price hits buy/sell thresholds
- Continues monitoring until stopped
