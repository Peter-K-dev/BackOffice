# Config Change Tracker – REST API

## Rationale

Store change Log every config change in domain config
Notifing external service of critical changes with retry in case of external service failure
Handle invalid config state transitions
Build in metrics and health check

## Assumptions

The API is deployed inside a trusted enterprise network (no auth)
Each rule is uniquely identified by a business key (Name)
Critical thresholds are defined in CriticalChangeDetector (minCredit less than 1000)

## Run Locally

# Clone the project

```
  git clone https://github.com/peter-k-dev/backoffice
```

# Swagger UI
```
https://localhost:7131/swagger
```

## Endpoints

```
GET /api/domainrules/1
```

```
GET /api/domainrules
```

```
POST /api/domainrules
Content-Type: application/json

{
  "name": "creditLimits",
  "data": {"minCredit" : 1500}
}
```

```
PUT /api/domainrules
Content-Type: application/json

{
  "id": 1,
  "data": {"minCredit" : 1400, "maxCredit": 1600},
  "rowVersion" : "AAAAAAAAAAE="
}
```

```
DELETE /api/domainrules/1
```

```
GET /health
```

```
GET /api/changelogs
```

```
GET /api/changelogs?orderby=type
```

```
GET /metrics
```