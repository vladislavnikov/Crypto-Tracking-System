# Crypto Tracking System

## Prerequisites

- .NET 8 SDK 
- SQL Server



## Setup

1. Set your SQL Server instance in CryptoTrackingSystem.Api/appsettings.json:

```http
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CryptoTracking;Trusted_Connection=True;TrustServerCertificate=True"
}
```

2. In Package Manager Console, set startup project to CryptoTrackingSystem.Api and run:


```http
update-database
```


## Run

Depends on the project * `(.Console or .API)`, select it as Startup Project, then run it.

Both apps start a live Binance WebSocket stream in the background and populates the database automatically.


# API Endpoints

The API provides endpoints for retrieving historical price statistics and calculating moving averages for a given trading symbol.

## Content Negotiation

Both endpoints support the following response formats:

* `application/json` is default response format
* `application/xml` is XML response format

Set the desired format using the `Accept` HTTP header:

```http
Accept: application/json
```

or

```http
Accept: application/xml
```

## 24-Hour Average Price

### `GET /api/{symbol}/24hAvgPrice`

Returns the average price of the specified trading symbol over the previous 24-hour period.

### Path Parameters

| Parameter | Type   | Required | Description                     |
| --------- | ------ | -------- | ------------------------------- |
| `symbol`  | string | Yes      | Trading symbol, e.g. `BTCUSDT`. |

### Example Request

```http
GET /api/BTCUSDT/24hAvgPrice
```

### Example Response

**JSON**

```json
{
  "symbol": "BTCUSDT",
  "averagePrice": 77000.00,
  "from": "2026-09-12T00:00:00Z",
  "to": "2026-09-13T00:00:00Z"
}
```

### Response Fields

| Field          | Type              | Description                                      |
| -------------- | ----------------- | ------------------------------------------------ |
| `symbol`       | string            | The requested trading symbol.                    |
| `averagePrice` | number            | Average price calculated over the last 24 hours. |
| `from`         | string (ISO 8601) | Start of the calculation period in UTC.          |
| `to`           | string (ISO 8601) | End of the calculation period in UTC.            |

### Status Codes

| Status Code     | Description                                          |
| --------------- | ---------------------------------------------------- |
| `200 OK`        | Average price successfully calculated.               |
| `404 Not Found` | No price data is available for the specified symbol. |


## Simple Moving Average

### `GET /api/{symbol}/SimpleMovingAverage`

Calculates the **Simple Moving Average (SMA)** for a specified number of periods.

The calculation uses `n` periods of size `p` and ends at the specified date/time. If `s` is not provided, the current UTC time is used.

### Path Parameters

| Parameter | Type   | Required | Description                     |
| --------- | ------ | -------- | ------------------------------- |
| `symbol`  | string | Yes      | Trading symbol, e.g. `BTCUSDT`. |

### Query Parameters

| Parameter | Type              | Required | Description                                                                |
| --------- | ----------------- | -------- | -------------------------------------------------------------------------- |
| `n`       | integer           | Yes      | Number of periods to include in the SMA calculation.                       |
| `p`       | string            | Yes      | Size of each period. Supported values: `1m`, `5m`, `30m`, `1d`, `1w`.      |
| `s`       | string (ISO 8601) | No       | End date/time of the calculation window. Defaults to the current UTC time. |

### Supported Periods

| Value | Description    |
| ----- | -------------- |
| `1m`  | One minute     |
| `5m`  | Five minutes   |
| `30m` | Thirty minutes |
| `1d`  | One day        |
| `1w`  | One week       |

### Example Requests

Calculate the 5-period daily SMA using the current UTC time:

```http
GET /api/BTCUSDT/SimpleMovingAverage?n=5&p=1d
```

Calculate the 10-period daily SMA ending on September 1, 2026:

```http
GET /api/BTCUSDT/SimpleMovingAverage?n=10&p=1d&s=2026-09-01T00:00:00Z
```

### Example Response

**JSON**

```json
{
  "symbol": "BTCUSDT",
  "sma": 76000.00,
  "numberOfDataPoints": 5,
  "periodLabel": "OneDay",
  "from": "2026-09-08T00:00:00Z",
  "to": "2026-09-13T00:00:00Z"
}
```

### Response Fields

| Field                | Type              | Description                                    |
| -------------------- | ----------------- | ---------------------------------------------- |
| `symbol`             | string            | The requested trading symbol.                  |
| `sma`                | number            | Calculated Simple Moving Average.              |
| `numberOfDataPoints` | integer           | Number of data points used in the calculation. |
| `periodLabel`        | string            | Human-readable name of the selected period.    |
| `from`               | string (ISO 8601) | Start of the SMA calculation window in UTC.    |
| `to`                 | string (ISO 8601) | End of the SMA calculation window in UTC.      |

### Status Codes

| Status Code       | Description                                                                                         |
| ----------------- | --------------------------------------------------------------------------------------------------- |
| `200 OK`          | SMA successfully calculated.                                                                        |
| `400 Bad Request` | One or more query parameters are invalid or missing. This includes an invalid or missing `p` value. |
| `404 Not Found`   | No price data is available for the specified symbol or calculation window.                          |


## Notes

* All timestamps are represented in **UTC** using the ISO 8601 format.
* The `s` parameter represents the **end of the calculation window**, not the start date.
* If `s` is omitted, the calculation uses the current UTC time.
* The API returns `application/json` by default when no `Accept` header is specified.