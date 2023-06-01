# Actual File Importer
This is an automated CSV importer for the [Actual](https://github.com/actualbudget/actual/) budgeting / money management system.  It will check the configured account folders every five minutes for new CSVs to import.

## Environment Variables
The container can be configured using the following environment variables:

| Variable             | Required | Default | Description                                                                                                                                       |
|:---------------------|:---------|:--------|:--------------------------------------------------------------------------------------------------------------------------------------------------|
| SERVER_URL           | Yes      |         | The base URL of the Actual server.<br/>e.g., `http://actual:5006`                                                                                 |
| SERVER_PASSWORD      | No*      |         | The Actual server password.                                                                                                                       |
| SERVER_PASSWORD_FILE | No*      |         | The path to the file containing the Actual server password.<br/>Only used if SERVER_PASSWORD is not provided.<br/>e.g., `/run/secrets/actual.pwd` |
| BUDGET_SYNC_ID       | Yes      |         | The `Sync ID` of the Actual budget file.  (Settings > Show advanced settings > Sync ID)<br/>e.g., `6B29FC40-CA47-1067-B31D-00DD010662DA`          |
| IMPORT_BASE_PATH     | No       | /import | The base directory where account definitions exist and CSV files will be dropped.                                                                 |
&ast; *The server password **must** be provided using **one** of these variables.*

## Account Definitions
Each account should be in its own folder under `IMPORT_BASE_PATH` and contain a JSON file named `account.json` that describes the CSV files that will be dropped in the folder for consumption:
```json
{
  "Account": "12345678-BBBb-cCCCC-0000-123456789012",
  "Delimiter": ",",
  "HeaderRows": 1,
  
  "DateColumn": 0, 
  "PayeeColumn": 4,
  "AmountColumn": 2,
  
  "DateFormat": "yyyy-MM-dd"
}
```
### Account Definition Values
| Variable     | Required | Default    | Description                                                                                                                              |
|:-------------|:---------|:-----------|:-----------------------------------------------------------------------------------------------------------------------------------------|
| Account      | Yes      |            | The ID of the account.<br/>(This can be found in the browser URL when the account is loaded.)                                            |
| Delimiter    | No       | ,          | The column delimiter for the CSV files.                                                                                                  |
| HeaderRows   | No       | 0          | The number of rows to skip before hitting data.                                                                                          |
| DateColumn   | No       |            | The 0-indexed column number holding the transaction date.<br/>If not provided, date will not be included with imported transactions.     |
| PayeeColumn  | No       |            | The 0-indexed column number holding the transaction payee.<br/>If not provided, payee will not be included with imported transactions.   |
| AmountColumn | No       |            | The 0-indexed column number holding the transaction amount.<br/>If not provided, amount will not be included with imported transactions. |
| DateFormat   | No       | yyyy-MM-dd | The format of the date in the date column.                                                                                               |

## Docker Compose Example
This example assume you are using traefik as a reverse proxy, because it's dope.  But you do you.

```yaml
version: '3.8'

services:

  actual:
    image: 'actualbudget/actual-server'
    networks:
      - traefik
      - actual
    [... clipped for brevity ...]
    
  importer:
    image: 'cinderblockgames/actual-file-importer'
    container_name: 'actual-file-importer'
    restart: unless-stopped
    environment:
      # required
      - 'SERVER_URL=http://actual:5006'
      - 'SERVER_PASSWORD_FILE=/run/secrets/actual.pwd'
      - 'BUDGET_SYNC_ID=6B29FC40-CA47-1067-B31D-00DD010662DA'
      # optional
      - 'IMPORT_BASE_PATH=/import'
    volumes:
      - '/data/actual/import:/import'
    networks:
      - actual

networks:
  traefik:
    external: true
  actual:
    external: true
```