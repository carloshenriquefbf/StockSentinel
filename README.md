# Stock Quote Alert

A console app that monitors B3 stock prices using the Alpha Vantage API and sends email alerts when buy/sell thresholds are reached.

## Download

Download the latest release from the [Releases](https://github.com/carloshenriquefbf/StockSentinel/releases) page or build from source.

## Setup

1. Create a `.env` file in the same folder as `StockSentinel.exe`:
```env
API_KEY=your_api_key # https://www.alphavantage.co/support/#api-key
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your-email@gmail.com
SMTP_PASSWORD=your-app-password # https://myaccount.google.com/apppasswords
TARGET_MAIL_ADDRESS=recipient@gmail.com
```

2. Your folder structure should look like this:
```
├── StockSentinel.exe
└── .env
```

## Usage

Open a terminal window in the folder containing the executable for your platform (e.g. Windows) and run:

```bash
StockSentinel.exe PETR4.SA 22.67 22.59
```

If on MacOS or Linux, make sure to give execute permissions:
```bash
chmod +x StockSentinel
```

**Parameters:**
- **ticker**: Stock symbol (e.g., PETR4.SA)
- **sellPrice**: Send sell alert when price >= this value
- **buyPrice**: Send buy alert when price <= this value

## How it works

- Checks stock price every 5 minutes
- Sends email when price hits buy/sell thresholds
- Continues monitoring until stopped (Ctrl+C)

## Currency Display
Note: Prices are displayed without currency symbols due to Alpha Vantage API limitations that don't provide currency information in their responses. The actual currency depends on the stock exchange:

- Brazilian stocks (e.g. PETR4.SA) are priced in BRL
- US stocks (e.g. IBM) are priced in USD
- Other international stocks follow their respective local currencies

Please be aware of the actual currency when interpreting price alerts.

## Building from Source

If you want to build the project yourself:

```bash
git clone https://github.com/carloshenriquefbf/StockSentinel.git
cd StockSentinel
dotnet publish -c Release -r ${PLATFORM} --self-contained true -p:PublishSingleFile=true -p:UseAppHost=true
```

Replace `${PLATFORM}` with your target platform (e.g., `win-x64`, `linux-x64`, `osx-x64`).

The executable will be in `src/StockSentinel/bin/Release/net9.0/${PLATFORM}/publish/`
